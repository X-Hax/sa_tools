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
using System.Linq;

namespace SAModel.SALVL
{
    public partial class MainForm
    {
        private static List<MetadataReference> objectDefinitionReferences;

        // Initializes the list of references used to compile object definitions
        private void InitObjDefReferences()
        {
            // System, System.Core, System.Drawing, SharpDX, SharpDX.Mathematics, SharpDX.Direct3D9,
            // SALVL, SAModel, SAModel.Direct3D, SA Tools, SAEditorCommon
            objectDefinitionReferences = new List<MetadataReference>
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
            Assembly.GetEntryAssembly().GetReferencedAssemblies().ToList().ForEach(a => objectDefinitionReferences.Add(MetadataReference.CreateFromFile(Assembly.Load(a).Location)));
        }

        // Load a level's object list and compile object definitions
        private void LoadObjectList(string objectList, bool Mission = false)
        {
			bool skipDefsNow = skipDefs;
            List<ObjectData> objectErrors = new List<ObjectData>();
            ObjectListEntry[] objlstini = ObjectList.Load(objectList, false);
			if (Mission)
				LevelData.MisnObjDefs = new List<ObjectDefinition>(new ObjectDefinition[objlstini.Length]);
			else
			{
				LevelData.ObjDefs = new List<ObjectDefinition>(new ObjectDefinition[objlstini.Length]);
				Directory.CreateDirectory("dllcache").Attributes |= FileAttributes.Hidden;
			}

            List<KeyValuePair<string, string>> compileErrors = new List<KeyValuePair<string, string>>();

			System.Threading.Tasks.Parallel.For(0, objlstini.Length, ID =>
			{
				string codeaddr = objlstini[ID].Name;
				ObjectData defgroup;
				ObjectDefinition def;
				if (objdefini == null)
				{
					skipDefsNow = true;
					defgroup = new ObjectData();
				}
				else
				{
					if (!objdefini.ContainsKey(codeaddr))
						codeaddr = "0";
					defgroup = objdefini[codeaddr];
				}

				if (!skipDefsNow && !string.IsNullOrEmpty(defgroup.CodeFile))
				{
					if (progress != null) progress.SetStep("Compiling: " + defgroup.CodeFile);

					bool errorOccured = false;
					string errorText = "";

					def = CompileObjectDefinition(defgroup, out errorOccured, out errorText);
					if (errorOccured)
					{
						KeyValuePair<string, string> errorValue = new KeyValuePair<string, string>(
							defgroup.CodeFile, errorText);
						log.Add(errorValue.Value);
						compileErrors.Add(errorValue);
					}
				}
				else
				{
					def = new DefaultObjectDefinition();
				}

				if (Mission)
					LevelData.MisnObjDefs[ID] = def;
				else
					LevelData.ObjDefs[ID] = def;

				// The only reason .Model is checked for null is for objects that don't yet have any
				// models defined for them. It would be annoying seeing that error all the time!
				if (string.IsNullOrEmpty(defgroup.CodeFile) && !string.IsNullOrEmpty(defgroup.Model))
				{
					if (progress != null) progress.SetStep("Loading: " + defgroup.Model);
					// Otherwise, if the model file doesn't exist and/or no texture file is defined,
					// load the "default object" instead ("?").
					if (!File.Exists(defgroup.Model) || // Model file missing OR
						string.IsNullOrEmpty(defgroup.Texture) || // Texture file undefined OR
						LevelData.Textures == null ||  // Textures not loaded OR
								((!LevelData.Textures.ContainsKey(defgroup.Texture) && // Texture file not among loaded textures 1 AND
								(!LevelData.Textures.ContainsKey(defgroup.Texture.ToLowerInvariant()) && // Texture file not among loaded textures 2 AND
								(!LevelData.Textures.ContainsKey(defgroup.Texture.ToUpperInvariant())))))) // Texture file not among loaded textures 3
					{
						ObjectData error = new ObjectData { Name = defgroup.Name, Model = defgroup.Model, Texture = defgroup.Texture };
						objectErrors.Add(error);
						defgroup.Model = null;
					}
				}

				def.Init(defgroup, objlstini[ID].Name);
				def.SetInternalName(objlstini[ID].Name);
			});

            if (compileErrors.Count > 0)
            {
                DialogResult result = MessageBox.Show(this, "Could not compile object definitions. Would you like to update them to the latest defaults? This will reset any custom object code.",
                    "Object Definitions Compilation Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
                    bool texExists = (!string.IsNullOrEmpty(o.Texture) && LevelData.Textures != null && (LevelData.Textures.ContainsKey(o.Texture) || LevelData.Textures.ContainsKey(o.Texture.ToLowerInvariant())));
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
                string originalObjdefsPath = GetObjDefsDirectory(salvlini.IsSA2);

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
                if (objdefFileInfo.Name.Equals("SADXObjectDefinitions.csproj") || objdefFileInfo.Name.Equals("SA2ObjectDefinitions.csproj"))
						continue;

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

        private string GetObjDefsDirectory(bool sa2)
        {
			string objdp = Path.GetDirectoryName(Application.ExecutablePath) + (sa2 ? "/../../SA2Tools/SA2ObjectDefinitions/" : "/../../SA1Tools/SADXObjectDefinitions/");
            if (Directory.Exists(objdp)) 
				return objdp;
            else 
				return Path.GetDirectoryName(Application.ExecutablePath) + (sa2 ? "/../GameConfig/PC_SA2/objdefs/" : "/../GameConfig/PC_SADX/objdefs/");
        }

    }
}
