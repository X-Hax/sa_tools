using SAModel.Direct3D.TextureSystem;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.UI;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAModel.SALVL
{
    public partial class MainForm
    {
        // Import models to stage
        private void ImportToStage(bool multiple = false)
        {
            importFileDialog.InitialDirectory = modFolder;
            if (importFileDialog.ShowDialog() != DialogResult.OK)
                return;

            DialogResult userClearLevelResult = MessageBox.Show("Do you want to clear the level models first?", "Clear Level?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (userClearLevelResult == DialogResult.Cancel)
                return;

            if (userClearLevelResult == DialogResult.Yes)
            {
                DialogResult clearAnimsResult = MessageBox.Show("Do you also want to clear any animated level models?", "Clear anims too?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                LevelData.ClearLevelGeometry();

                if (clearAnimsResult == DialogResult.Yes)
                {
                    LevelData.ClearLevelGeoAnims();
                }
                selectedItems.Clear();
            }

            foreach (string s in importFileDialog.FileNames)
            {
                List<Item> newItems = LevelData.ImportFromFile(s, cam, out bool errorFlag, out string errorMsg, selectedItems, osd, multiple);
                if (errorFlag)
                    osd.AddMessage(errorMsg + "\n", 300);
                if (!multiple)
                    selectedItems.Add(newItems);
                else
                    selectedItems.Clear();
            }

            LevelData.InvalidateRenderState();
            unsaved = true;
        }

        // Export models from stage
        private void ExportLevelObj(string fileName, bool selectedOnly)
        {
            int stepCount = 0;
            int numSteps = 0;
            if (LevelData.TextureBitmaps != null && LevelData.TextureBitmaps.Count > 0)
            {
                stepCount = LevelData.TextureBitmaps[LevelData.leveltexs].Length;
                numSteps = stepCount;
            }
            List<COL> cols = LevelData.geo.COL;
            if (selectedOnly)
                cols = selectedItems.Items.OfType<LevelItem>().Select(a => a.CollisionData).ToList();
            stepCount += cols.Count;
            if (LevelData.geo.Anim != null)
                stepCount += LevelData.geo.Anim.Count;

            Assimp.AssimpContext context = new Assimp.AssimpContext();
            Assimp.Scene scene = new Assimp.Scene();
            scene.Materials.Add(new Assimp.Material());
            Assimp.Node n = new Assimp.Node();
            n.Name = "RootNode";
            scene.RootNode = n;
            string rootPath = Path.GetDirectoryName(fileName);
            List<string> texturePaths = new List<string>();

            ProgressDialog progress = new ProgressDialog("Exporting stage: " + levelName, stepCount, true, false);
            progress.Show(this);
            progress.SetTaskAndStep("Exporting...");

            for (int i = 0; i < numSteps; i++)
            {
                BMPInfo bmp = LevelData.TextureBitmaps[LevelData.leveltexs][i];
                texturePaths.Add(Path.Combine(rootPath, bmp.Name + ".png"));
                bmp.Image.Save(Path.Combine(rootPath, bmp.Name + ".png"));
                progress.Step = $"Texture {i + 1}/{numSteps}";
                progress.StepProgress();
                Application.DoEvents();
            }

            for (int i = 0; i < cols.Count; i++)
            {
                SAEditorCommon.Import.AssimpStuff.AssimpExport(cols[i].Model, scene, Matrix.Identity, texturePaths.Count > 0 ? texturePaths.ToArray() : null, scene.RootNode);

                progress.Step = $"Mesh {i + 1}/{cols.Count}";
                progress.StepProgress();
                Application.DoEvents();
            }

            if (SAEditorCommon.DataTypes.LevelData.geo.Anim != null && !selectedOnly)
                for (int i = 0; i < LevelData.geo.Anim.Count; i++)
                {
                    SAEditorCommon.Import.AssimpStuff.AssimpExport(LevelData.geo.Anim[i].Model, scene, Matrix.Identity, texturePaths.Count > 0 ? texturePaths.ToArray() : null, scene.RootNode);
                    progress.Step = $"Animation {i + 1}/{LevelData.geo.Anim.Count}";
                    progress.StepProgress();
                    Application.DoEvents();
                }

            string ftype = "collada";
            switch (Path.GetExtension(fileName).ToLowerInvariant())
            {
                case ".fbx":
                    ftype = "fbx";
                    break;
                case ".obj":
                    ftype = "obj";
                    break;
            }
            context.ExportFile(scene, fileName, ftype, Assimp.PostProcessSteps.ValidateDataStructure | Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.FlipUVs);//

            progress.SetTaskAndStep("Export complete!");
        }

        // Export C structs
        private void exportStructs(string filename, bool selectedOnly)
        {

            LandTableFormat fmt = LevelData.geo.Format;
            switch (fmt)
            {
                case LandTableFormat.SA1:
                case LandTableFormat.SADX:
                    if (usetBasicDXFormatToolStripMenuItem.Checked) fmt = LandTableFormat.SADX;
                    else fmt = LandTableFormat.SA1;
                    break;
            }
            List<string> labels = new List<string>() { LevelData.geo.Name };
            string[] texnames = null;
            if (LevelData.leveltexs != null && exportTextureNamesToolStripMenuItem.Checked)
            {
                texnames = new string[LevelData.TextureBitmaps[LevelData.leveltexs].Length];
                for (int i = 0; i < LevelData.TextureBitmaps[LevelData.leveltexs].Length; i++)
                    texnames[i] = string.Format("{0}TexName_{1}", LevelData.leveltexs,
                        LevelData.TextureBitmaps[LevelData.leveltexs][i].Name);

            }
            if (!selectedOnly)
            {
                using (StreamWriter sw = File.CreateText(filename))
                {
                    WriteStructMetadata(sw, true, fmt, texnames);
                    LevelData.geo.ToStructVariables(sw, fmt, labels, texnames);
                    return;
                }
            }
            else
            {
                foreach (Item selectedItem in selectedItems.Items)
                {
                    if (selectedItem is LevelItem)
                    {
                        LevelItem levelItem = selectedItem as LevelItem;
                        string path = Path.Combine(filename, levelItem.CollisionData.Model.Name + ".c");
                        using (StreamWriter sw = File.CreateText(path))
                        {
                            WriteStructMetadata(sw, false, fmt, texnames);
                            levelItem.CollisionData.Model.ToStructVariables(sw, usetBasicDXFormatToolStripMenuItem.Checked, labels, texnames);
                        }
                    }
                }
                return;
            }
        }

        private void WriteStructMetadata(StreamWriter sw, bool level, LandTableFormat fmt, string[] texnames = null)
        {
            sw.Write("/* Sonic Adventure ");
            switch (fmt)
            {
                case LandTableFormat.SA1:
                    sw.Write("1");
                    break;
                case LandTableFormat.SADX:
                    sw.Write("DX");
                    break;
                case LandTableFormat.SA2:
                    sw.Write("2");
                    break;
                case LandTableFormat.SA2B:
                    sw.Write("2: Battle");
                    break;
            }
            if (level)
                sw.WriteLine(" LandTable");
            else
                sw.WriteLine(" Model");
            sw.WriteLine(" * ");
            sw.WriteLine(" * Generated by SALVL");
            sw.WriteLine(" * ");
            if (!string.IsNullOrEmpty(LevelData.geo.Description))
            {
                sw.Write(" * Description: ");
                sw.WriteLine(LevelData.geo.Description);
                sw.WriteLine(" * ");
            }
            if (!string.IsNullOrEmpty(LevelData.geo.Author))
            {
                sw.Write(" * Author: ");
                sw.WriteLine(LevelData.geo.Author);
                sw.WriteLine(" * ");
            }
            sw.WriteLine(" */");
            sw.WriteLine();
            if (texnames != null)
            {
                sw.Write("enum {0}TexName", LevelData.leveltexs);
                sw.WriteLine();
                sw.WriteLine("{");
                sw.WriteLine("\t" + string.Join("," + Environment.NewLine + "\t", texnames));
                sw.WriteLine("};");
                sw.WriteLine();
            }
        }

    }
}
