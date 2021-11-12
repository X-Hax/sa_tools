using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.SETEditing;
using SplitTools;
using System;
using System.Text;
using System.Reflection;
using SAModel.Direct3D;
using SharpDX;
using SharpDX.Direct3D9;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace SAModel.SALVL
{
    public partial class MainForm
    {
        // Load a level's object list and compile object definitions
        private void LoadObjectList(string objectList, bool Mission = false)
        {
            List<ObjectData> objectErrors = new List<ObjectData>();
            ObjectListEntry[] objlstini = ObjectList.Load(objectList, false);
            if (Mission)
                LevelData.MisnObjDefs = new List<ObjectDefinition>();
            else
            {
                LevelData.ObjDefs = new List<ObjectDefinition>();
                Directory.CreateDirectory("dllcache").Attributes |= FileAttributes.Hidden;
            }

            List<KeyValuePair<string, string>> compileErrors = new List<KeyValuePair<string, string>>();

            for (int ID = 0; ID < objlstini.Length; ID++)
            {
                string codeaddr = objlstini[ID].CodeString;
                ObjectData defgroup;
                ObjectDefinition def;
                if (objdefini == null)
                {
                    skipDefs = true;
                    defgroup = new ObjectData();
                }
                else
                {
                    if (!objdefini.ContainsKey(codeaddr))
                        codeaddr = "0";
                    defgroup = objdefini[codeaddr];
                }

                if (!skipDefs && !string.IsNullOrEmpty(defgroup.CodeFile))
                {
                    if (progress != null) progress.SetStep("Compiling: " + defgroup.CodeFile);

                    bool errorOccured = false;
                    string errorText = "";

                    def = CompileObjectDefinition(defgroup, out errorOccured, out errorText);

                    if (errorOccured)
                    {
                        KeyValuePair<string, string> errorValue = new KeyValuePair<string, string>(
                            defgroup.CodeFile, errorText);

                        compileErrors.Add(errorValue);
                    }
                }
                else
                {
                    def = new DefaultObjectDefinition();
                }

                if (Mission)
                    LevelData.MisnObjDefs.Add(def);
                else
                    LevelData.ObjDefs.Add(def);

                // The only reason .Model is checked for null is for objects that don't yet have any
                // models defined for them. It would be annoying seeing that error all the time!
                if (string.IsNullOrEmpty(defgroup.CodeFile) && !string.IsNullOrEmpty(defgroup.Model))
                {
                    if (progress != null) progress.SetStep("Loading: " + defgroup.Model);
                    // Otherwise, if the model file doesn't exist and/or no texture file is defined,
                    // load the "default object" instead ("?").
                    if (!File.Exists(defgroup.Model) || string.IsNullOrEmpty(defgroup.Texture) ||
                        (LevelData.Textures == null || !LevelData.Textures.ContainsKey(defgroup.Texture)))
                    {
                        ObjectData error = new ObjectData { Name = defgroup.Name, Model = defgroup.Model, Texture = defgroup.Texture };
                        objectErrors.Add(error);
                        defgroup.Model = null;
                    }
                }

                def.Init(defgroup, objlstini[ID].Name);
                def.SetInternalName(objlstini[ID].Name);
            }

            if (compileErrors.Count > 0)
            {
                DialogResult result = MessageBox.Show("There were compile errors. Would you like to try upgrading the object definitions? This will over-write any changes to them that you've made!",
                    "Would you like to try upgrading?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    initerror = true;
                    bool error = CopyDefaultObjectDefinitions();
                    if (!error)
                        MessageBox.Show("Please restart SALVL to complete the operation.", "SALVL", MessageBoxButtons.OK);
                    return;
                }
            }

            if (objectErrors.Count > 0)
            {
                int count = objectErrors.Count;
                List<string> errorStrings = new List<string> { "\nSET object load errors:" };

                foreach (ObjectData o in objectErrors)
                {
                    bool texEmpty = string.IsNullOrEmpty(o.Texture);
                    bool texExists = (!string.IsNullOrEmpty(o.Texture) && LevelData.Textures != null && LevelData.Textures.ContainsKey(o.Texture));
                    errorStrings.Add("");
                    errorStrings.Add("Object:\t\t" + o.Name);
                    errorStrings.Add("\tModel:");
                    errorStrings.Add("\t\tName:\t" + o.Model);
                    errorStrings.Add("\t\tExists:\t" + File.Exists(o.Model));
                    errorStrings.Add("\tTexture:");
                    errorStrings.Add("\t\tName:\t" + ((texEmpty) ? "(N/A)" : o.Texture));
                    errorStrings.Add("\t\tExists:\t" + texExists);
                }
                log.AddRange(errorStrings);
                osd.AddMessage(levelName + ":\n" + count + (Mission ? " Mission SET" : " SET") + (count == 1 ? " object " : " objects ") + "failed to load their model(s).\n"
                                    + "Please check SET object load errors in the log for details.\n", 300);
                log.WriteLog();
            }

            LevelData.StateChanged += LevelData_StateChanged;
            LevelData.InvalidateRenderState();

        }

		// System, System.Core, System.Drawing, SharpDX, SharpDX.Mathematics, SharpDX.Direct3D9,
		// SALVL, SAModel, SAModel.Direct3D, SA Tools, SAEditorCommon
		private static readonly MetadataReference[] objectDefinitionReferences =
		new[]
		{
			MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
			MetadataReference.CreateFromFile(typeof(System.Drawing.Bitmap).Assembly.Location),
			MetadataReference.CreateFromFile(typeof(SharpDX.Mathematics.Interop.RawBool).Assembly.Location),
			MetadataReference.CreateFromFile(typeof(Vector3).Assembly.Location),
			MetadataReference.CreateFromFile(typeof(Device).Assembly.Location),
			MetadataReference.CreateFromFile(typeof(LandTable).Assembly.Location),
			MetadataReference.CreateFromFile(typeof(EditorCamera).Assembly.Location),
			MetadataReference.CreateFromFile(typeof(SA1LevelAct).Assembly.Location),
			MetadataReference.CreateFromFile(typeof(ObjectDefinition).Assembly.Location)
		};

		private static readonly CSharpCompilationOptions objectDefinitionOptions =
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
				.WithOverflowChecks(true)
				.WithOptimizationLevel(OptimizationLevel.Release);

		private static ObjectDefinition CompileObjectDefinition(ObjectData defgroup, out bool errorOccured, out string errorText)
        {
            ObjectDefinition def;
            errorOccured = false;
            errorText = "";
            string codeType = defgroup.CodeType;
            string dllfile = Path.Combine("dllcache", codeType + ".dll");
			string pdbfile = Path.Combine("dllcache", codeType + ".pdb");
			DateTime modDate = DateTime.MinValue;
            if (File.Exists(dllfile)) modDate = File.GetLastWriteTime(dllfile);
            string fp = defgroup.CodeFile.Replace('/', Path.DirectorySeparatorChar);
            if (modDate >= File.GetLastWriteTime(fp) && modDate > File.GetLastWriteTime(Application.ExecutablePath))
            {
                def =
                    (ObjectDefinition)
                        Activator.CreateInstance(
                            Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, dllfile))
                                .GetType(codeType));
            }
            else
            {
				SyntaxTree[] st = new[] { SyntaxFactory.ParseSyntaxTree(File.ReadAllText(fp), CSharpParseOptions.Default, fp, Encoding.UTF8) };

				CSharpCompilation compilation =
						CSharpCompilation.Create(codeType, st, objectDefinitionReferences, objectDefinitionOptions);

				try
				{
					EmitResult result = compilation.Emit(dllfile, pdbfile);

					if (!result.Success)
					{
						errorOccured = true;
						foreach (Diagnostic diagnostic in result.Diagnostics)
						{
							errorText += String.Format("\n\n{0}", diagnostic.ToString());
						}

						File.Delete(dllfile);
						File.Delete(pdbfile);

						def = new DefaultObjectDefinition();
					}
					else
					{
						def =
							(ObjectDefinition)
								Activator.CreateInstance(
									Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, dllfile))
										.GetType(codeType));
					}
				}
				catch (Exception e)
				{
					errorOccured = true;
					errorText = String.Format("\n\n{0}", e.ToString());

					File.Delete(dllfile);
					File.Delete(pdbfile);

					def = new DefaultObjectDefinition();
				}
			}
            return def;
        }

        // Default definitions
        private bool CopyDefaultObjectDefinitions()
        {
            bool error = false;
            try
            {
                // get our original objdefs folder
                string originalObjdefsPath = GetObjDefsDirectory();

                // get our project objdefs folder
                string projectObjdefsPath = Path.Combine(modFolder, "objdefs");

                // clear the project objdefs folder
                if (Directory.Exists(projectObjdefsPath)) Directory.Delete(projectObjdefsPath, true);

                // recusrively copy the original objdefs folder to project folder
                CopyFolder(originalObjdefsPath, projectObjdefsPath);
                if (Directory.Exists(Path.Combine(modFolder, "dllcache")))
                {
                    byte[] emptyBytes = new byte[1];
                    File.WriteAllBytes(Path.Combine(modFolder, "dllcache", "DELETE"), emptyBytes);
                }
            }
            catch (Exception ex)
            {
                error = true;
                MessageBox.Show("Error copying object definitions:\n" + ex.ToString(), "SALVL Error");
                log.Add("Error copying object definitions:\n" + ex.ToString());
                log.WriteLog();
            }
            return error;
        }

        private void CopyFolder(string sourceFolder, string destinationFolder)
        {
            string[] files = Directory.GetFiles(sourceFolder);

            Directory.CreateDirectory(destinationFolder);

            foreach (string objdef in files)
            {
                System.IO.FileInfo objdefFileInfo = new System.IO.FileInfo(objdef);
                if (objdefFileInfo.Name.Equals("SADXObjectDefinitions.csproj")) continue;

                // copy
                string filePath = Path.Combine(sourceFolder, objdefFileInfo.Name);
                string destinationPath = Path.Combine(destinationFolder, objdefFileInfo.Name);
                File.Copy(filePath, destinationPath, true);
            }

            string[] directories = Directory.GetDirectories(sourceFolder);

            foreach (string directory in directories)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                if (directoryInfo.Name.Equals("bin") || directoryInfo.Name.Equals("obj")) continue;

                string copySrcDir = Path.Combine(sourceFolder, directoryInfo.Name);
                string copyDstDir = Path.Combine(destinationFolder, directoryInfo.Name);

                CopyFolder(copySrcDir, copyDstDir);
            }
        }

        private string GetObjDefsDirectory()
        {
            string objdp = Path.GetDirectoryName(Application.ExecutablePath) + "/../../SA1Tools/SADXObjectDefinitions/";
            if (Directory.Exists(objdp)) return objdp;
            else return Path.GetDirectoryName(Application.ExecutablePath) + "/../GameConfig/PC_SADX/objdefs/";
        }

    }
}
