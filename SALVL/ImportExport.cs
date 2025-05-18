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
		// Add a level animation
		private void ImportLevelAnimation()
		{
			importFileDialog.InitialDirectory = modFolder;
			if (importFileDialog.ShowDialog() != DialogResult.OK)
				return;
			string animpath = Path.ChangeExtension(importFileDialog.FileName, ".saanim");
			// Load saanim file if it isn't found
			if (!File.Exists(animpath))
				using (OpenFileDialog ofd = new OpenFileDialog
				{
					Title = "Import Level Animation",
					DefaultExt = "saanim",
					Filter = "Animation Files|*.saanim",
					Multiselect = false
				})
				{
					if (ofd.ShowDialog() != DialogResult.OK)
						return;
					animpath = ofd.FileName;
				}
			// Update level animations
			LevelAnim newanim = LevelData.ImportLevelAnimation(new ModelFile(importFileDialog.FileName).Model, NJS_MOTION.Load(animpath), selectedItems);
			if (LevelData.LevelAnims == null)
				LevelData.ClearLevelGeoAnims();
			LevelData.geo.Anim.Add(newanim.GeoAnimationData);
			LevelData.AddLevelAnim(newanim);
			unsaved = true;
			selectedItems.Clear();
			selectedItems.Add(newanim);
			LevelData.InvalidateRenderState();
			playAnimButton.Enabled = prevFrameButton.Enabled = nextFrameButton.Enabled = resetAnimButton.Enabled = LevelData.LevelAnimCount > 0;
		}

		// Import models to stage
		private void ImportLevelItem(bool multiple = false)
		{
			bool isSA2 = isSA2LVL();
			bool isVisible = true;
			bool isCol = true;
			ModelFormat fmt = ModelFormat.BasicDX;
			importFileDialog.InitialDirectory = modFolder;

			if (isSA2)
			{
				isCol = false;
				if (LevelData.geo.Format == LandTableFormat.SA2)
				{
					importFileDialog.Filter = "Model Files|*.sa2mdl;*.obj;*.objf;*.txt;*.dae;*.fbx";
					fmt = ModelFormat.Chunk;
				}
				else
				{
					importFileDialog.Filter = "Model Files|*.sa2bmdl;*.obj;*.objf;*.txt;*.dae;*.fbx";
					fmt = ModelFormat.GC;
				}
					
			}

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
				List<Item> newItems = LevelData.ImportFromFile(s, cam, out bool errorFlag, out string errorMsg, selectedItems, osd, multiple, isVisible, isCol, fmt);
				if (errorFlag)
					osd.AddMessage(errorMsg + "\n", 300);
				if (!multiple)
					selectedItems.Add(newItems);
				else
					selectedItems.Clear();
			}

			if (isSA2)
			{
				DialogResult impCollision = MessageBox.Show("Would you like to select any models to import as collision items?", "Import Collision Models", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (impCollision == DialogResult.Yes)
				{
					importFileDialog.Filter = "Model Files|*.sa1mdl;*.obj;*.objf;*.txt;*.dae;*.fbx";
					if (importFileDialog.ShowDialog() != DialogResult.OK)
						return;

					isVisible = false;
					isCol = true;
					foreach (string s in importFileDialog.FileNames)
					{
						List<Item> newItems = LevelData.ImportFromFile(s, cam, out bool errorFlag, out string errorMsg, selectedItems, osd, multiple, isVisible, isCol, ModelFormat.Basic);
						if (errorFlag)
							osd.AddMessage(errorMsg + "\n", 300);
						if (!multiple)
							selectedItems.Add(newItems);
						else
							selectedItems.Clear();
					}
				}
			}

			LevelData.InvalidateRenderState();
			unsaved = true;
		}

		// Export models from stage (Assimp)
		private void ExportLevelObj(string fileName, bool selectedOnly, bool exportSolidOnly, bool exportVisibleOnly)
		{
			int stepCount = 0;
			int numSteps = 0;
			if (LevelData.TextureBitmaps != null && LevelData.TextureBitmaps.Count > 0)
			{
				stepCount = LevelData.TextureBitmaps[LevelData.leveltexs].Length;
				numSteps = stepCount;
			}

			//Sort out level data
			List<LevelItem> items;
			List<GeoAnimData> anims;
			if (selectedOnly)
			{
				items = selectedItems.Items.OfType<LevelItem>().ToList();
				anims = selectedItems.Items.OfType<LevelAnim>().Select(a => a.GeoAnimationData).ToList();
			}
			else
			{
				items = LevelData.LevelItems.ToList();
				anims = LevelData.geo.Anim;
			}

			//Sort out collision and regular models
			if (exportSolidOnly)
			{
				items = items.Where(a => a.Solid == true).ToList();
			}
			else if (exportVisibleOnly)
			{
				items = items.Where(a => a.Visible == true).ToList();
			}

			List<COL> cols = items.Select(a => a.CollisionData).ToList();

			stepCount += cols.Count;
			stepCount += anims.Count;

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

			List<string> texlist = new List<string>();

			for (int i = 0; i < numSteps; i++)
			{
				BMPInfo bmp = LevelData.TextureBitmaps[LevelData.leveltexs][i];
				texlist.Add(bmp.Name);
				texturePaths.Add(Path.Combine(rootPath, bmp.Name + ".png"));
				if (!File.Exists(Path.Combine(rootPath, bmp.Name + ".png")))
					bmp.Image.Save(Path.Combine(rootPath, bmp.Name + ".png"));
				progress.Step = $"Texture {i + 1}/{numSteps}";
				progress.StepProgress();
				Application.DoEvents();
			}

			// Save texture list
			SplitTools.NJS_TEXLIST textureNamesArray = new SplitTools.NJS_TEXLIST(texlist.ToArray());
			textureNamesArray.Save(Path.Combine(rootPath, Path.GetFileNameWithoutExtension(fileName) + ".satex"));

			for (int i = 0; i < cols.Count; i++)
			{
				SAEditorCommon.Import.AssimpStuff.AssimpExport(cols[i].Model, scene, Matrix.Identity, texturePaths.Count > 0 ? texturePaths.ToArray() : null, scene.RootNode);

				progress.Step = $"Mesh {i + 1}/{cols.Count}";
				progress.StepProgress();
				Application.DoEvents();
			}

			for (int i = 0; i < anims.Count; i++)
			{
				SAEditorCommon.Import.AssimpStuff.AssimpExport(anims[i].Model, scene, Matrix.Identity, texturePaths.Count > 0 ? texturePaths.ToArray() : null, scene.RootNode);
				progress.Step = $"Animation {i + 1}/{anims.Count}";
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
			context.ExportFile(scene, fileName, ftype, Assimp.PostProcessSteps.ValidateDataStructure | Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.FlipUVs);

			progress.SetTaskAndStep("Export complete!");
		}

		// Export C structs
		private void exportStructs(string filename, bool selectedOnly, bool exportOnlySolid, bool exportOnlyNonSolid)
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

						//Only export based on collision flag
						bool export = true;
						if (exportOnlySolid)
						{
							if(levelItem.Solid == false)
							{
								export = false;
							}
						} else if(exportOnlyNonSolid)
						{
							if (levelItem.Solid == true)
							{
								export = false;
							}
						}

						if (export)
						{
							string path = Path.Combine(filename, levelItem.CollisionData.Model.Name + ".c");
							using (StreamWriter sw = File.CreateText(path))
							{
								WriteStructMetadata(sw, false, fmt, texnames);
								levelItem.CollisionData.Model.ToStructVariables(sw, usetBasicDXFormatToolStripMenuItem.Checked, labels, texnames);
							}
						}
					}
					else if (selectedItem is LevelAnim)
					{
						LevelAnim levelAnim = selectedItem as LevelAnim;
						string path = Path.Combine(filename, levelAnim.GeoAnimationData.Animation.ActionName + ".c");
						using (StreamWriter sw = File.CreateText(path))
						{
							WriteStructMetadata(sw, false, fmt, texnames);
							levelAnim.GeoAnimationData.ToStructVariables(sw, usetBasicDXFormatToolStripMenuItem.Checked, labels, texnames);
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