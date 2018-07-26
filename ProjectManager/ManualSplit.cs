using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

using SonicRetro.SAModel.SAEditorCommon.UI;

namespace ProjectManager
{
    public partial class ManualSplit : Form
    {
        public Action OnSplitFinished;
        public Action OnSplitCanceled;        

        List<SplitUIControl> splitControls = new List<SplitUIControl>();

        public ManualSplit()
        {
            InitializeComponent();

            HelpButton.Image = SystemIcons.Question.ToBitmap();

            SplitUIControl firstControl = new SplitUIControl(false, true);

            firstControl.Parent = flowLayoutPanel1;

            firstControl.OnJobAdded += SplitControlControl_OnJobAdded;
            splitControls.Add(firstControl);

            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
        }

        private void SplitControlControl_OnJobAdded(SplitUIControl sender)
        {
            SplitUIControl newControl = new SplitUIControl(true, true);
            newControl.Parent = flowLayoutPanel1;

            foreach (SplitUIControl splitUI in splitControls)
            {
                splitUI.DisableAddingNew();
            }

            newControl.OnJobAdded += SplitControlControl_OnJobAdded;
            newControl.OnJobRemoved += NewControl_OnJobRemoved;

            splitControls.Add(newControl);
        }

        private void NewControl_OnJobRemoved(SplitUIControl sender)
        {
            splitControls.Remove(sender);
            sender.Dispose();

            splitControls.Last().EnableAddingNew();
        }

        private void SplitButton_Click(object sender, EventArgs e)
        {
            bool valid = true;

            foreach(SplitUIControl control in splitControls)
            {
                valid |= control.IsValid();
            }

            if(!valid)
            {
                MessageBox.Show("One or more split jobs is invalid!");
            }
            else
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void BinSplitData_Sub(SplitData splitData, string outputFolder)
        {
            string datafilename = splitData.dataFile;
            string inifilename = splitData.iniFile;
            string projectFolderName = outputFolder;

            if (projectFolderName[projectFolderName.Length - 1] != '/') projectFolderName = string.Concat(projectFolderName, '/');

            #region Validating Inputs
            if (!File.Exists(datafilename))
            {
                Console.WriteLine(datafilename + " not found. Aborting.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();

                throw new Exception(ERRORVALUE.NoSourceFile.ToString());
                //return (int)ERRORVALUE.NoSourceFile;
            }

            if (!File.Exists(inifilename))
            {
                Console.WriteLine(inifilename + " not found. Aborting.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();

                throw new Exception(ERRORVALUE.NoDataMapping.ToString());
                //return (int)ERRORVALUE.NoDataMapping;
            }

            if (!Directory.Exists(projectFolderName))
            {
                // try creating the directory
                bool created = true;

                try
                {
                    // check to see if trailing charcter closes 
                    Directory.CreateDirectory(projectFolderName);
                }
                catch
                {
                    created = false;
                }

                if (!created)
                {
                    // couldn't create directory.
                    Console.WriteLine("Output folder did not exist and couldn't be created.");
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadLine();

                    throw new Exception(ERRORVALUE.InvalidProject.ToString());
                    //return (int)ERRORVALUE.InvalidProject;
                }
            }
            #endregion

            // switch on file extension - if dll, use dll splitter
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(datafilename);

            int result = (fileInfo.Extension.ToLower().Contains("dll")) ? SplitDLL.SplitDLL.SplitDLLFile(datafilename, inifilename, projectFolderName) :
                Split.Split.SplitFile(datafilename, inifilename, projectFolderName);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            using (ProgressDialog progress = new ProgressDialog("Splitting", splitControls.Count, false, true))
            {
                foreach(SplitUIControl splitControl in splitControls)
                {
                    string shortName = Path.GetFileName(splitControl.GetFilePath());

                    progress.SetStep(shortName);

                    string outputFolder = splitControl.GetOutputFolder();

                    SplitType splitType = splitControl.GetSplitType();
                    
                    switch (splitType)
                    {
                        case SplitType.BinDLL:
                            // do our normal split
                            SplitData splitData = new SplitData()
                            {
                                dataFile = splitControl.GetFilePath(),
                                iniFile = splitControl.GetDataMappingPath()
                            };

                            string oldEnvironment = Environment.CurrentDirectory;

                            Environment.CurrentDirectory = Path.GetDirectoryName(splitData.dataFile);
                            BinSplitData_Sub(splitData, outputFolder);

                            break;

                        case SplitType.MDL:
                            SplitMDL.SplitMDL.Split(splitControl.IsBigEndian(), splitControl.GetFilePath(),
                                outputFolder, splitControl.AnimFiles);
                            break;

                        case SplitType.MTN:
                            // use the command line split
                            break;

                        case SplitType.NB:
                            // use the command line split
                            break;

                        default:
                            break;
                    }

                    progress.StepProgress();
                }
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           if(OnSplitFinished != null)
            {
                Hide();
                OnSplitFinished.Invoke();
            }
        }

        private void ManualSplit_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
            OnSplitCanceled.Invoke();
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            string message = "This tool is for extracting game files for non-mod purposes.\nIf you're just looking to get at game data without making a mod project, this is the tool you want.\nIf you ARE making a mod, on the other hand, exit this and go back, the data splitting is handled automatically for mod projects.";

            MessageBox.Show(message, "About this", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
