using System;
using System.Collections.Generic;
using System.Windows.Forms;

using SAModel.SAEditorCommon.DataTypes;

namespace SAModel.SAEditorCommon.UI
{
	public partial class SceneGraphControl : UserControl
	{
		EditorItemSelection selection;

		TreeNode levelItemNode;
		TreeNode exLevelItemNode;
		TreeNode levelAnimNode;
		TreeNode deathZoneNode;
		TreeNode setNode;
		TreeNode camNode;
		TreeNode missionSETNode;
		TreeNode splineNode;

		bool suppressSelectionEvents = false;

		public SceneGraphControl()
		{
			InitializeComponent();
		}

		public void InitSceneControl(EditorItemSelection selection)
		{
			this.selection = selection;

			// subscribe to our editor select change events
			selection.SelectionChanged += Selection_SelectionChanged;
			sceneTreeView.AfterSelect += SceneTreeView_AfterSelect;
		}

		private void SceneTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			suppressSelectionEvents = true;

			// Find items that were in the selection but are no longer selected
			List<Item> removeitems = new List<Item>();
			foreach (Item olditem in selection.Items)
			{
				bool found = false;
				foreach (TreeNode selectedNode in sceneTreeView.SelectedNodes)
				{
					Item _item = GetItemForNode(selectedNode);
					if (olditem == _item) found = true;
				}
				if (!found) removeitems.Add(olditem);
			}

			// Remove items that are no longer selected
			foreach (Item remove in removeitems)
			{
				selection.Remove(remove);
			}

			// Add items that weren't selected previously
			foreach (TreeNode selectedNode in sceneTreeView.SelectedNodes)
			{
				// figure out which kind of node we are by looking at the immediate parent.
				Item _item = GetItemForNode(selectedNode);
				if (selectedNode == sceneTreeView.TopNode) continue;
				if (selectedNode == levelItemNode || selectedNode == exLevelItemNode || selectedNode == levelAnimNode || selectedNode == deathZoneNode ||
					selectedNode == setNode || selectedNode == camNode ||
					selectedNode == missionSETNode || selectedNode == splineNode) continue;

				if (!selection.Contains(_item))
					selection.Add(_item);
			}

			suppressSelectionEvents = false;
		}

		private Item GetItemForNode(TreeNode node)
		{
			TreeNode parent = node.Parent;

			if (parent == levelItemNode)
			{
				return LevelData.GetLevelitemAtIndex(node.Index);
			}
			else if (parent == exLevelItemNode)
			{
				return LevelData.GetEXLevelitemAtIndex(node.Index);
			}
			else if (parent == levelAnimNode)
			{
				return LevelData.GetLevelAnimAtIndex(node.Index);
			}
			else if (parent == deathZoneNode)
			{
				return LevelData.DeathZones[node.Index];
			}
			else if (parent == setNode)
			{
				return LevelData.GetSetItemAtIndex(LevelData.Character, node.Index);
			}
			else if (parent == camNode)
			{
				return LevelData.CAMItems[LevelData.Character][node.Index];
			}
			else if (parent == missionSETNode)
			{
				return LevelData.MissionSETItems[LevelData.Character][node.Index];
			}
			else if (parent == splineNode)
			{
				return LevelData.LevelSplines[node.Index];
			}

			return null; // error finding this node
		}

		private void Selection_SelectionChanged(EditorItemSelection sender)
		{
			if (suppressSelectionEvents) return;

			// match our tree to our editor selection
			List<TreeNode> selectedNodes = new List<TreeNode>();

			foreach (Item item in sender.Items)
			{
				if (item is LevelItem)
				{
					LevelItem levelItem = (LevelItem)item;
					// find the index of the level item
					int index = LevelData.GetIndexOfItem(levelItem);
					int indexex = 0;
					if (index >= 0)
						selectedNodes.Add(levelItemNode.Nodes[index]);
					else
					{
						indexex = LevelData.GetIndexOfEXItem(levelItem);
						selectedNodes.Add(exLevelItemNode.Nodes[indexex]);
					}
				}
				else if (item is LevelAnim)
				{
					LevelAnim levelAnim = (LevelAnim)item;

					// find the index of the level animation
					int index = LevelData.GetIndexOfItem(levelAnim);

					selectedNodes.Add(levelAnimNode.Nodes[index]);
				}
				else if (item is DeathZoneItem)
				{
					DeathZoneItem deathZoneItem = (DeathZoneItem)item;

					int index = LevelData.DeathZones.IndexOf(deathZoneItem);

					selectedNodes.Add(deathZoneNode.Nodes[index]);
				}
				else if (item is MissionSETItem)
				{
					MissionSETItem miSetItem = (MissionSETItem)item;

					int index = LevelData.MissionSETItems[LevelData.Character].IndexOf(miSetItem);

					selectedNodes.Add(missionSETNode.Nodes[index]);
				}
				else if (item is SETItem)
				{
					SETItem setItem = (SETItem)item;
					int index;
					if (LevelData.isSA2)
					{
						index = LevelData.GetIndexOfSETItem(LevelData.SA2Set, setItem);//LevelData.SETItems[LevelData.Character].IndexOf(setItem);
					}
					else
						index = LevelData.GetIndexOfSETItem(LevelData.Character, setItem);//LevelData.SETItems[LevelData.Character].IndexOf(setItem);

					selectedNodes.Add(setNode.Nodes[index]);
				}
				else if (item is CAMItem)
				{
					CAMItem camItem = (CAMItem)item;

					int index = LevelData.CAMItems[LevelData.Character].IndexOf(camItem);

					selectedNodes.Add(camNode.Nodes[index]);
				}
				else if (item is SplineData)
				{
					SplineData spline = (SplineData)item;

					int index = LevelData.LevelSplines.IndexOf(spline);

					selectedNodes.Add(splineNode.Nodes[index]);
				}
			}

			sceneTreeView.SelectedNodes = selectedNodes;
		}

		private void SceneGraphControl_Load(object sender, EventArgs e)
		{
			if (!this.DesignMode)
			{
				// go through the level data and add all the things
				sceneTreeView.BeginUpdate();
				levelItemNode = sceneTreeView.Nodes.Add("Level Objects");
				exLevelItemNode = sceneTreeView.Nodes.Add("Extra Level Objects");
				levelAnimNode = sceneTreeView.Nodes.Add("Level Animations");
				deathZoneNode = sceneTreeView.Nodes.Add("Death Zones");
				setNode = sceneTreeView.Nodes.Add("SET Items");
				camNode = sceneTreeView.Nodes.Add("CAM Items");
				splineNode = sceneTreeView.Nodes.Add("Spline Objects");
				missionSETNode = sceneTreeView.Nodes.Add("MI SET Items");
				sceneTreeView.EndUpdate();

				// subscribe to our level data changed event
				LevelData.StateChanged += LevelData_StateChanged;

				// subscribe to our character changed event
				LevelData.CharacterChanged += LevelData_CharacterChanged;

				// subscribe to our SA2 SET type changed event
				LevelData.SetTypeChanged += LevelData_SetTypeChanged;
			}
		}

		private void LevelData_CharacterChanged()
		{
			LoadFullTree();
		}

		private void LevelData_SetTypeChanged()
		{
			LoadFullTree();
		}

		private void LevelData_StateChanged()
		{
			LoadFullTree();
		}

		void LoadFullTree()
		{
			sceneTreeView.BeginUpdate();

			levelItemNode.Nodes.Clear();
			exLevelItemNode.Nodes.Clear();
			levelAnimNode.Nodes.Clear();
			deathZoneNode.Nodes.Clear();
			setNode.Nodes.Clear();
			camNode.Nodes.Clear();
			splineNode.Nodes.Clear();
			missionSETNode.Nodes.Clear();

			// level items
			foreach (LevelItem levelItem in LevelData.LevelItems)
			{
				levelItemNode.Nodes.Add(levelItem.Name);
			}
			foreach (LevelItem levelItem in LevelData.EXLevelItems)
			{
				exLevelItemNode.Nodes.Add(levelItem.Name);
			}
			// level animations
			foreach (LevelAnim levelAnimItem in LevelData.LevelAnims)
			{
				levelAnimNode.Nodes.Add(levelAnimItem.ActionName);
			}

			// death zones
			if (LevelData.DeathZones != null)
			{
				foreach (DeathZoneItem deathZone in LevelData.DeathZones)
				{
					deathZoneNode.Nodes.Add(deathZone.Name);
				}
			}

			// set node
			if (!LevelData.SETItemsIsNull() && LevelData.CharHasSETItems(LevelData.Character))
				foreach (SETItem setItem in LevelData.SETItems(LevelData.Character))
				{
					setNode.Nodes.Add(setItem.Name);
				}

			// cam node
			if (LevelData.CAMItems != null && LevelData.CAMItems[LevelData.Character] != null)
			{
				foreach (CAMItem camItem in LevelData.CAMItems[LevelData.Character])
				{
					camNode.Nodes.Add(camItem.CamType.ToString());
				}
			}

			if (LevelData.LevelSplines != null)
			{
				foreach (SplineData splineData in LevelData.LevelSplines)
				{
					splineNode.Nodes.Add("spline_" + splineData.Code.ToString("X"));
				}
			}

			if (LevelData.MissionSETItems != null && LevelData.MissionSETItems[LevelData.Character] != null)
				foreach (MissionSETItem missionSet in LevelData.MissionSETItems[LevelData.Character])
				{
					missionSETNode.Nodes.Add(missionSet.Name);
				}

			sceneTreeView.EndUpdate();
		}

		void ChangeCharacterRelevantNodes()
		{
			LoadFullTree();
		}
	}
}
