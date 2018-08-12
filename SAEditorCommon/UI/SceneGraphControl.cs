using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SonicRetro.SAModel.SAEditorCommon.DataTypes;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
    public partial class SceneGraphControl : UserControl
    {
        EditorItemSelection selection;

        TreeNode levelItemNode;
        TreeNode deathZoneNode;
        TreeNode setNode;
        TreeNode camNode;
        TreeNode missionSETNode;
        TreeNode splineNode;

        public SceneGraphControl()
        {
            InitializeComponent();
        }

        public SceneGraphControl(EditorItemSelection selection)
        {
            InitializeComponent();

            this.selection = selection;

            // subscribe to our editor select change events

        }

        private void SceneGraphControl_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                // go through the level data and add all the things
                levelItemNode = sceneTreeView.Nodes.Add("Level Objects");
                deathZoneNode = sceneTreeView.Nodes.Add("Death Zones");
                setNode = sceneTreeView.Nodes.Add("SET Items");
                camNode = sceneTreeView.Nodes.Add("CAM Items");
                splineNode = sceneTreeView.Nodes.Add("Spline Objects");
                missionSETNode = sceneTreeView.Nodes.Add("MI SET Items");

                // subscrube to our level data changed event
                LevelData.StateChanged += LevelData_StateChanged;

                // subscribe to our character changed event
                LevelData.CharacterChanged += LevelData_CharacterChanged;

                // load the full tree
                //LoadFullTree();
            }
        }

        private void LevelData_CharacterChanged()
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
            deathZoneNode.Nodes.Clear();
            setNode.Nodes.Clear();
            camNode.Nodes.Clear();
            splineNode.Nodes.Clear();
            missionSETNode.Nodes.Clear();

            // level items
            foreach(LevelItem levelItem in LevelData.LevelItems)
            {
                levelItemNode.Nodes.Add(levelItem.Name);
            }

            foreach(DeathZoneItem deathZone in LevelData.DeathZones)
            {
                deathZoneNode.Nodes.Add(deathZone.Name);
            }

            // set node
            foreach(SETItem setItem in LevelData.SETItems[LevelData.Character])
            {
                setNode.Nodes.Add(setItem.Name);
            }

            // cam node
            foreach(CAMItem camItem in LevelData.CAMItems[LevelData.Character])
            {
                camNode.Nodes.Add(camItem.CamType.ToString());
            }

            foreach(SplineData splineData in LevelData.LevelSplines)
            {
                splineNode.Nodes.Add("spline_" + splineData.Code.ToString());
            }

            foreach(MissionSETItem missionSet in LevelData.MissionSETItems[LevelData.Character])
            {
                missionSETNode.Nodes.Add(missionSETNode.Name);
            }

            sceneTreeView.EndUpdate();
        }

        void ChangeCharacterRelevantNodes()
        {
            LoadFullTree();
        }
    }
}
