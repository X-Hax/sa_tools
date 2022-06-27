using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAModel.SAEditorCommon;
using SAModel.SAEditorCommon.SETEditing;
using SAModel.SAEditorCommon.UI;
using SplitTools;

namespace SAModel.SALVL
{
	public partial class ObjListEditor : Form
	{
		enum Reset
		{
			list,
			script,
			code
		};
		List<string> rottype = new List<string>
		{
			"",
			"X",
			"Y",
			"Z",
			"XY",
			"XZ",
			"YX",
			"YZ",
			"ZX",
			"ZY",
			"XYZ",
			"XZY",
			"YXZ",
			"YZX",
			"ZXY",
			"ZYX",
			"None"
		};
		List<string> scltype = new List<string>
		{
			"",
			"X",
			"Y",
			"Z",
			"XY",
			"XZ",
			"YZ",
			"XYZ",
			"AllX",
			"AllY",
			"AllZ",
			"None"
		};
		static List<ObjectListEntry> objList;
		static Dictionary<string, ObjectData> objDefinitions;
		static string modFolder;
		static string objListString;
		static string objDefString;
		static bool isSA2;
		static bool saved;
		static bool modified;
		static ObjectListEntry curObj;
		static ObjectData curObjDef;
		static IniDataSALVL salvl;

		//public ObjListEditor()
		public ObjListEditor(string objlistpath, string objdefspath, string folder, IniDataSALVL lvlconfig)
		{
			InitializeComponent();
			salvl = lvlconfig;
			modFolder = folder;
			objListString = Path.Combine(folder, objlistpath);
			isSA2 = salvl.IsSA2;
			saved = true;
			if (File.Exists(objListString))
			{
				ObjectListEntry[] objListArr = ObjectList.Load(objListString, isSA2);
				objList = new List<ObjectListEntry>(objListArr);
			}

			if (objdefspath == "" || !File.Exists(Path.Combine(folder, objdefspath)))
			{
				DialogResult error = MessageBox.Show(("Level Object Definitions not found. Please select a location to save the Definitions file."), "Definitions Not Located", MessageBoxButtons.OK);
				if (error == DialogResult.OK)
					CreateDefaultObjDefs();
			}
			else
			{
				objDefString = Path.Combine(folder, objdefspath);
				if (File.Exists(objDefString))
					objDefinitions = IniSerializer.Deserialize<Dictionary<string, ObjectData>>(objDefString);
			}
		}

		void ResetBoxes(Reset reset)
		{
			switch (reset)
			{
				case Reset.list:
					numArgs1.Value = 0;
					numArgs2.Value = 0;
					numFlags.Value = 0;
					numDistance.Value = 0;
					numObjCodeAddr.Value = 0;
					txtObjIntName.ResetText();
					break;
				case Reset.script:
					txtScriptName.ResetText();
					txtScriptModel.ResetText();
					txtScriptTexture.ResetText();
					txtScriptTexlist.ResetText();
					lstRotType.SelectedIndex = -1;
					lstSclType.SelectedIndex = -1;
					numGrndDist.Value = 0;
					numDefRotX.Value = 0;
					numDefRotY.Value = 0;
					numDefRotZ.Value = 0;
					numDefSclX.Value = 0;
					numDefSclY.Value = 0;
					numDefSclZ.Value = 0;
					numAddRotX.Value = 0;
					numAddRotY.Value = 0;
					numAddRotZ.Value = 0;
					numAddSclX.Value = 0;
					numAddSclY.Value = 0;
					numAddSclZ.Value = 0;
					break;
				case Reset.code:
					txtCodeFile.ResetText();
					txtCodeClass.ResetText();
					break;
				default:
					break;
			}
		}

		private void CreateDefaultObjDefs()
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = "Object Definition Files (*.ini)|*.ini";
			dialog.InitialDirectory = modFolder;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				Dictionary<string, ObjectData> tempObjDefs = new Dictionary<string, ObjectData>();
				tempObjDefs.Add("0", new ObjectData());
				foreach (ObjectListEntry obj in objList)
				{
					if (!tempObjDefs.ContainsKey(obj.Name))
					{
						ObjectData objData = new ObjectData();
						objData.Name = obj.Name;
						objData.RotType = "XYZ";
						objData.SclType = "None";
						if (!tempObjDefs.ContainsKey(obj.Name))
							tempObjDefs.Add(obj.Name, objData);
					}
				}

				IniSerializer.Serialize(tempObjDefs, dialog.FileName);
				objDefString = dialog.FileName;
				objDefinitions = tempObjDefs;
			}
			else
				this.Close();
		}

		private bool UpdateObjListSelection()
		{
			int sel = lstObjects.SelectedIndex;
			if (sel >= 0)
			{
				curObj = objList[sel];
				numArgs1.Value = curObj.Arg1;
				numArgs2.Value = curObj.Arg2;
				numDistance.Value = (decimal)curObj.Distance;
				numFlags.Value = curObj.Flags;
				txtObjIntName.Text = curObj.Name;
				numObjCodeAddr.Value = curObj.Code;
			}
			return true;
		}

		private bool UpdateObjDefSelection()
		{
			string key = objList[lstObjects.SelectedIndex].Name;
			if (key != null)
			{
				curObjDef = objDefinitions[key];

				txtCodeFile.Text = curObjDef.CodeFile;
				txtCodeClass.Text = curObjDef.CodeType;
				txtScriptName.Text = curObjDef.Name;
				txtScriptModel.Text = curObjDef.Model;
				txtScriptTexture.Text = curObjDef.Texture;
				txtScriptTexlist.Text = curObjDef.Texlist;
				lstRotType.SelectedItem = curObjDef.RotType;
				lstSclType.SelectedItem = curObjDef.SclType;

				if (curObjDef.GndDst != null)
					numGrndDist.Value = (decimal)curObjDef.GndDst;
				else
					numGrndDist.Value = 0;
				if (curObjDef.DefXRot != null)
					numDefRotX.Value = (decimal)curObjDef.DefXRot;
				else
					numDefRotX.Value = 0;
				if (curObjDef.DefYRot != null)
					numDefRotY.Value = (decimal)curObjDef.DefYRot;
				else
					numDefRotY.Value = 0;
				if (curObjDef.DefZRot != null)
					numDefRotZ.Value = (decimal)curObjDef.DefZRot;
				else
					numDefRotZ.Value = 0;
				if (curObjDef.DefXScl != null)
					numDefSclX.Value = (decimal)curObjDef.DefXScl;
				else
					numDefSclX.Value = 0;
				if (curObjDef.DefYScl != null)
					numDefSclY.Value = (decimal)curObjDef.DefYScl;
				else
					numDefSclY.Value = 0;
				if (curObjDef.DefZScl != null)
					numDefSclZ.Value = (decimal)curObjDef.DefZScl;
				else
					numDefSclZ.Value = 0;
				if (curObjDef.AddXRot != null)
					numAddRotX.Value = (decimal)curObjDef.AddXRot;
				else
					numAddRotX.Value = 0;
				if (curObjDef.AddYRot != null)
					numAddRotY.Value = (decimal)curObjDef.AddYRot;
				else
					numAddRotY.Value = 0;
				if (curObjDef.AddZRot != null)
					numAddRotZ.Value = (decimal)curObjDef.AddZRot;
				else
					numAddRotZ.Value = 0;
				if (curObjDef.AddXScl != null)
					numAddSclX.Value = (decimal)curObjDef.AddXScl;
				else
					numAddSclX.Value = 0;
				if (curObjDef.AddYScl != null)
					numAddSclY.Value = (decimal)curObjDef.AddYScl;
				else
					numAddSclY.Value = 0;
				if (curObjDef.AddZScl != null)
					numAddSclZ.Value = (decimal)curObjDef.AddZScl;
				else
					numAddSclZ.Value = 0;
			}
			return true;
		}

		private void lstObjects_SelectedIndexChanged(object sender, EventArgs e)
		{
			//ResetBoxes();
			modified = false;
			bool objupdated = UpdateObjListSelection();
			bool objdefupdate = UpdateObjDefSelection();

			if (objupdated && objdefupdate)
				modified = true;
		}

		private void ObjListEditor_Shown(object sender, EventArgs e)
		{
			foreach (ObjectListEntry obj in objList)
				lstObjects.Items.Add(obj.Name);
			foreach (string str in rottype)
				lstRotType.Items.Add(str);
			foreach (string str in scltype)
				lstSclType.Items.Add(str);
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			ObjectList.Save(objList.ToArray(), objListString);
			IniSerializer.Serialize(objDefinitions, objDefString);
			saved = true;
		}

		private void btnAddObj_Click(object sender, EventArgs e)
		{
			saved = false;
			ObjectListEntry newObj;
			if (isSA2)
				newObj = new SA2ObjectListEntry();
			else
				newObj = new SA1ObjectListEntry();

			ObjectData newObjDef = new ObjectData();

			SimpleInputForm dlg = new SimpleInputForm("Name", "New Object");
			dlg.ShowDialog();

			if (dlg.useOK)
				newObj.Name = dlg.outputText;
			else
				newObj.Name = "Object " + (objList.Count + 1).ToString();
			newObj.Code = 0;

			newObjDef.Name = newObj.Name;
			newObjDef.RotType = "XYZ";
			newObjDef.SclType = "None";

			objList.Add(newObj);
			objDefinitions.Add(newObj.Name, newObjDef);

			lstObjects.Items.Add(newObj.Name);
			lstObjects.SelectedIndex = (lstObjects.Items.Count - 1);

		}

		private void btnDelObj_Click(object sender, EventArgs e)
		{
			saved = false;
			int delSel = lstObjects.SelectedIndex;
			lstObjects.SelectedIndex = (delSel - 1);
			objDefinitions.Remove(objList[delSel].Name);
			objList.Remove(objList[delSel]);
			lstObjects.Items.RemoveAt(delSel);
		}

		private void btnImpObj_Click(object sender, EventArgs e)
		{
			ObjListImport imp = new ObjListImport(salvl, modFolder);
			imp.ShowDialog();

			if (imp.impObjListItem != null)
			{
				if (!objList.Contains(imp.impObjListItem))
				{
					objList.Add(imp.impObjListItem);
					objDefinitions.Add(imp.impObjListItem.Name, imp.impObjDefsItem);
					lstObjects.Items.Add(imp.impObjListItem.Name);
				}
			}

		}

		private void button1_Click(object sender, EventArgs e)
		{
			string oldFilter = "objdefs.ini (objdefs.ini)|objdefs.ini|Ini Files (*.ini)|*.ini";
			string newFilter = "Definition Files (*.defs)|*.defs|Ini Files (*.ini)|*.ini";

			bool useOldDefs = false;
			DialogResult dlg = MessageBox.Show(("You are about to import objdef information." +
				"\nAre you importing from the old style objdef.ini file?"), "ObjDef Import", MessageBoxButtons.YesNo);

			if (dlg == DialogResult.Yes)
				useOldDefs = true;

			OpenFileDialog openFile = new OpenFileDialog();
			if (useOldDefs)
				openFile.Filter = oldFilter;
			else
				openFile.Filter = newFilter;
			openFile.InitialDirectory = modFolder;
			Dictionary<string, ObjectData> oldDefs;

			if (openFile.ShowDialog() == DialogResult.OK)
			{
				oldDefs = IniSerializer.Deserialize<Dictionary<string, ObjectData>>(openFile.FileName);

				foreach (ObjectListEntry obj in objList)
				{
					string key;
					if (useOldDefs)
						key = obj.CodeString;
					else
						key = obj.Name;

					if (oldDefs.ContainsKey(key))
					{
						ObjectData oldobjdef = oldDefs[key];
						ObjectData newobjdef = objDefinitions[obj.Name];

						newobjdef.CodeFile = oldobjdef.CodeFile;
						newobjdef.CodeType = oldobjdef.CodeType;
						newobjdef.Name = oldobjdef.Name;
						newobjdef.Model = oldobjdef.Model;
						newobjdef.Texture = oldobjdef.Texture;
						newobjdef.Texlist = oldobjdef.Texlist;
						newobjdef.RotType = oldobjdef.RotType;
						newobjdef.SclType = oldobjdef.SclType;
						newobjdef.GndDst = oldobjdef.GndDst;
					}
				}
			}
			if (lstObjects.SelectedIndex != -1)
				UpdateObjDefSelection();
		}

		private void btnBrowseModel_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Model Files (*.sa1mdl;*.sa2mdl;*.sa2bmdl)|*.sa1mdl;*.sa2mdl;*.sa2bmdl";
			dlg.InitialDirectory = modFolder;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				string file = Path.GetRelativePath(modFolder, dlg.FileName);
				txtScriptModel.Text = file;
			}
		}

		private void btnBrowseTexture_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Texture Files (*.pvm;*.prs;*.gvm;*.xvm;*.pak)|*.pvm;*.prs;*.gvm;*.xvm;*.pak";
			dlg.InitialDirectory = modFolder;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				string file = Path.GetFileNameWithoutExtension(dlg.FileName);
				txtScriptTexture.Text = file;
			}
		}

		private void btnCodeBrowse_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Code Files (*.cs)|*.cs";
			dlg.InitialDirectory = modFolder;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				string file = Path.GetRelativePath(modFolder, dlg.FileName);
				txtCodeFile.Text = file;
			}
		}

		private void btnBrowseTexlist1_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Texlist Files (*.satex)|*.satex";
			dlg.InitialDirectory = modFolder;
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				string file = Path.GetRelativePath(modFolder, dlg.FileName);
				txtScriptTexlist.Text = file;
			}
		}

		#region Box Change Checks
		private void numArgs1_ValueChanged(object sender, EventArgs e)
		{
			saved = false;
			curObj.Arg1 = (byte)numArgs1.Value;
		}

		private void numArgs2_ValueChanged(object sender, EventArgs e)
		{
			saved = false;
			curObj.Arg2 = (byte)numArgs2.Value;
		}

		private void numFlags_ValueChanged(object sender, EventArgs e)
		{
			saved = false;
			curObj.Flags = (ushort)numFlags.Value;
		}

		private void numDistance_ValueChanged(object sender, EventArgs e)
		{
			saved = false;
			curObj.Distance = (float)numDistance.Value;
		}

		private void UpdateEntries(object sender, EventArgs e)
		{
			if (modified)
			{
				saved = false;
				if (txtCodeClass.Text != "")
					curObjDef.CodeFile = txtCodeFile.Text;
				else
					curObjDef.CodeFile = null;

				if (txtCodeFile.Text != "")
					curObjDef.CodeType = txtCodeClass.Text;
				else
					curObjDef.CodeType = null;

				if (txtScriptName.Text != "")
					curObjDef.Name = txtScriptName.Text;
				else
					curObjDef.Name = null;

				if (txtScriptModel.Text != "")
					curObjDef.Model = txtScriptModel.Text;
				else
					curObjDef.Model = null;

				if (txtScriptTexture.Text != "")
					curObjDef.Texture = txtScriptTexture.Text;
				else
					curObjDef.Texture = null;

				if (txtScriptTexlist.Text != "")
					curObjDef.Texlist = txtScriptTexlist.Text;
				else
					curObjDef.Texlist = null;

				if (lstRotType.SelectedIndex > -1)
					curObjDef.RotType = lstRotType.Text;
				else
					curObjDef.SclType = null;

				if (lstSclType.SelectedIndex > -1)
					curObjDef.SclType = lstSclType.Text;
				else
					curObjDef.SclType = null;

				if (numGrndDist.Value > 0)
					curObjDef.GndDst = (float)numGrndDist.Value;
				else
					curObjDef.GndDst = null;

				if (numDefRotX.Value > 0)
					curObjDef.DefXRot = (ushort)numDefRotX.Value;
				else
					curObjDef.DefXRot = null;

				if (numDefRotY.Value > 0)
					curObjDef.DefYRot = (ushort)numDefRotY.Value;
				else
					curObjDef.DefYRot = null;

				if (numDefRotZ.Value > 0)
					curObjDef.DefZRot = (ushort)numDefRotZ.Value;
				else
					curObjDef.DefZRot = null;

				if (numDefSclX.Value > 0)
					curObjDef.DefXScl = (float)numDefSclX.Value;
				else
					curObjDef.DefXScl = null;

				if (numDefSclY.Value > 0)
					curObjDef.DefYScl = (float)numDefSclY.Value;
				else
					curObjDef.DefYScl = null;

				if (numDefSclZ.Value > 0)
					curObjDef.DefZScl = (float)numDefSclZ.Value;
				else
					curObjDef.DefZScl = null;

				if (numAddRotX.Value > 0)
					curObjDef.AddXRot = (ushort)numAddRotX.Value;
				else
					curObjDef.AddXRot = null;

				if (numAddRotY.Value > 0)
					curObjDef.AddYRot = (ushort)numAddRotY.Value;
				else
					curObjDef.AddYRot = null;

				if (numAddRotZ.Value > 0)
					curObjDef.AddZRot = (ushort)numAddRotZ.Value;
				else
					curObjDef.AddZRot = null;

				if (numAddSclX.Value > 0)
					curObjDef.AddXScl = (float)numAddSclX.Value;
				else
					curObjDef.AddXScl = null;

				if (numAddSclY.Value > 0)
					curObjDef.AddYScl = (float)numAddSclY.Value;
				else
					curObjDef.AddYScl = null;

				if (numAddSclZ.Value > 0)
					curObjDef.AddZScl = (float)numAddSclZ.Value;
				else
					curObjDef.AddZScl = null;
			}
		}
		#endregion

		private void ObjListEditor_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!saved)
			{
				DialogResult save = MessageBox.Show(("Your changes have not been saved. Would you like to save?"), "Files Not Saved", MessageBoxButtons.YesNo);
				if (save == DialogResult.Yes)
				{
					btnSave_Click(sender, e);
				}
			}
		}

		private void ObjListEditor_HelpButtonClicked(object sender, CancelEventArgs e)
		{
			DialogResult help = MessageBox.Show("This tool is for editing object lists and object definitions for objects to display in SALVL. " +
				"\n\nThe Object List Editor will update the object list with changes. If you build a mod with a modified object list," +
				" you may run into issues if you added a new object or imported a new object from another object list. You will usually" +
				" need to add the texture to the level texture list using SADXTweaker. For newly added objects, your mod will need custom " +
				"code for implementing the object into the game.", "Object Editor Help", MessageBoxButtons.OK);
		}
	}
}
