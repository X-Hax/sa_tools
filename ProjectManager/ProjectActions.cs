using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjectManager
{
    public partial class ProjectActions : Form
    {
        public Action NavigateBack;

        SonicRetro.SAModel.SAEditorCommon.StructConverter.StructConverterUI structConverterUI = 
            new SonicRetro.SAModel.SAEditorCommon.StructConverter.StructConverterUI();
        
        SonicRetro.SAModel.SAEditorCommon.DLLModGenerator.DLLModGenUI modGenUI = 
           new SonicRetro.SAModel.SAEditorCommon.DLLModGenerator.DLLModGenUI();

        SA_Tools.Game game;
        string projectFolder;
        string projectName;

        public ProjectActions()
        {
            InitializeComponent();
        }

        private void ProjectActions_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            NavigateBack.Invoke();
        }

        public void Init(SA_Tools.Game game, string projectName, string projectFolder)
        {
            this.game = game;
            this.projectName = projectName;
            ProjectNameLAbel.Text = projectName + " : " +  game.ToString();
            this.projectFolder = projectFolder;
        }

        private void BuildDLLDerivedData_Click(object sender, EventArgs e)
        {
            modGenUI.SetProjectFolder(projectFolder);
            modGenUI.SetModFolder(Path.Combine(Program.Settings.GetModPathForGame(game),
                projectName));

            modGenUI.ShowDialog();
        }

        private void ExeBuildButton_Click(object sender, EventArgs e)
        {
            structConverterUI.SetProjectFolder(projectFolder);
            structConverterUI.SetModFolder(Path.Combine(Program.Settings.GetModPathForGame(game),
                projectName));

            // we need to set the ini file to open properly.
            // we can know which one to load because there's only one exe per game
            string iniFileToOpen = (game == SA_Tools.Game.SADX) ? "sonic_data.ini" : "sonic2app_data.ini";

            structConverterUI.OpenFile(iniFileToOpen);
        }

        private void SADXLVL2Button_Click(object sender, EventArgs e)
        {
            // launch sadxlvl2
            string sadxlvl2Path = "";

#if DEBUG
            sadxlvl2Path = Path.GetDirectoryName(Application.ExecutablePath) + "/../../../SADXLVL2/bin/Debug/SADXLVL2.exe";
#endif
#if !DEBUG
            sadxlvl2Path = Path.GetDirectoryName(Application.ExecutablePath) + "/../SADXPC/SADXLVL2/SADXLVL2.exe";
#endif

            System.Diagnostics.ProcessStartInfo sadxlvl2StartInfo = new System.Diagnostics.ProcessStartInfo(
                Path.GetFullPath(sadxlvl2Path),
                string.Format("\"{0}\"", Path.GetFullPath(projectFolder + "/sadxlvl.ini")));

            System.Diagnostics.Process sadxlvl2Process = System.Diagnostics.Process.Start(sadxlvl2StartInfo);
        }

        private void SAMDLButton_Click(object sender, EventArgs e)
        {
            // launch samdl
            string samdlPath = "";

#if DEBUG
            samdlPath = Path.GetDirectoryName(Application.ExecutablePath) + "/../../../SAMDL/bin/Debug/SAMDL.exe";
#endif
#if !DEBUG
            samdlPath = Path.GetDirectoryName(Application.ExecutablePath) + "/../SADXPC/SAMDL/SAMDL.exe";
#endif

            System.Diagnostics.ProcessStartInfo samdlStartInfo = new System.Diagnostics.ProcessStartInfo(
                Path.GetFullPath(samdlPath)//,
                /*Path.GetFullPath(projectFolder)*/);

            System.Diagnostics.Process samdlProcess = System.Diagnostics.Process.Start(samdlStartInfo);
        }

        private void SADXTweaker2Button_Click(object sender, EventArgs e)
        {
            // launch sadxtweaker2
            string sadxtweaker2Path = "";

#if DEBUG
            sadxtweaker2Path = Path.GetDirectoryName(Application.ExecutablePath) + "/../../../SADXTweaker2/bin/Debug/SADXTweaker2.exe";
#endif
#if !DEBUG
            sadxtweaker2Path = Path.GetDirectoryName(Application.ExecutablePath) + "/../SADXPC/SADXTweaker2/SADXTweaker2.exe.exe";
#endif
            string sonicDataPath = Path.GetFullPath(Path.Combine(projectFolder, "sonic_data.ini"));
            System.Diagnostics.ProcessStartInfo sadxTweaker2StartInfo = new System.Diagnostics.ProcessStartInfo(
                Path.GetFullPath(sadxtweaker2Path),
                string.Format("\"{0}\"", sonicDataPath));

            System.Diagnostics.Process sadxTweaker2Process = System.Diagnostics.Process.Start(sadxTweaker2StartInfo);
        }
    }
}
