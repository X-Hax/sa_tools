using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using System.IO;

namespace ProjectManager
{
	internal enum SplitType
	{
		BinDLL,
		MDL,
		MTN,
		NB
	}

	public partial class SplitUIControl : UserControl
	{
		
		public delegate void JobChangedHandler(SplitUIControl sender);
		public event JobChangedHandler OnJobAdded;
		public event JobChangedHandler OnJobRemoved;

		SplitType splitType = SplitType.BinDLL;

		string[] animFiles = new string[0];
		bool isBigEndian = false;

		public string[] AnimFiles { get { return animFiles; } }

		private const int splitBinHeight = 299;
		private const int splitMDLHeight = 414;

		public SplitUIControl(bool canRemove, bool canAdd)
		{
			InitializeComponent();

			removeSplitJob.Visible = canRemove;
			removeSplitJob.Enabled = canRemove;

			AddJobButton.Enabled = canAdd;
			AddJobButton.Visible = canAdd;

			splitTypeComboBox.SelectedIndex = 0;

			SetControls();

			bigEndianCheckbox.Visible = false;
			bigEndianCheckbox.Enabled = false;
		}

		private void SetControls()
		{
			switch (splitType)
			{
				case SplitType.BinDLL:
					this.Size = new Size(this.Size.Width, splitBinHeight);
					this.animFilesList.Visible = false;
					this.animFilesList.Enabled = false;
					this.AddNewAnimFileButton.Visible = false;
					this.AddNewAnimFileButton.Enabled = false;
					this.RemoveAnimButton.Visible = false;
					this.RemoveAnimButton.Enabled = false;

					this.dataMappingPathText.Visible = true;
					this.dataMappingPathText.Enabled = true;
					DataMappingLabel.Text = "Data Mapping:";
					break;

				case SplitType.MDL:
					this.Size = new Size(this.Size.Width, splitMDLHeight);
					this.animFilesList.Visible = true;
					this.animFilesList.Enabled = true;
					this.AddNewAnimFileButton.Visible = true;
					this.AddNewAnimFileButton.Enabled = true;
					this.RemoveAnimButton.Visible = true;
					this.RemoveAnimButton.Enabled = animFilesList.SelectedItems.Count > 0;

					this.dataMappingPathText.Visible = false;
					this.dataMappingPathText.Enabled = false;
					DataMappingLabel.Text = "Animation Files";
					break;

				case SplitType.MTN:
					break;
				case SplitType.NB:
					break;
				default:
					break;
			}
		}

		public void DisableAddingNew()
		{
			AddJobButton.Enabled = false;
		}

		public void EnableAddingNew()
		{
			AddJobButton.Enabled = true;
		}

		public bool IsValid()
		{
			return File.Exists(filePathText.Text) && File.Exists(dataMappingPathText.Text) &&
				Directory.Exists(outputFolderPath.Text);
		}

		public string GetFilePath()
		{
			return filePathText.Text;
		}

		public string GetDataMappingPath()
		{
			return dataMappingPathText.Text;
		}

		public string GetOutputFolder()
		{
			return outputFolderPath.Text;
		}

		public bool IsBigEndian()
		{
			return isBigEndian;
		}

		internal SplitType GetSplitType()
		{
			return splitType;
		}

		private void AddJobButton_Click(object sender, EventArgs e)
		{
			if(OnJobAdded != null)
			{
				JobChangedHandler dispatch = OnJobAdded;

				dispatch(this);
			}
		}

		private void removeSplitJob_Click(object sender, EventArgs e)
		{
			if (OnJobRemoved != null)
			{
				JobChangedHandler dispatch = OnJobRemoved;

				dispatch(this);
			}
		}

		private void fileBrowseButton_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog())
			{
				if(fileDialog.ShowDialog() == DialogResult.OK)
				{
					filePathText.Text = fileDialog.FileName;
				}
			}
		}

		private void dataMappingBrowseButton_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog() { Filter = "Data Mapping|*.ini", Multiselect = false })
			{
				if (fileDialog.ShowDialog() == DialogResult.OK)
				{
					dataMappingPathText.Text = fileDialog.FileName;
				}
			}
		}

		private void outputFolderBrowseButton_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
			{
				if (folderDialog.ShowDialog() == DialogResult.OK)
				{
					outputFolderPath.Text = folderDialog.SelectedPath;
				}
			}
		}

		private void splitTypeComboBox_DropDownClosed(object sender, EventArgs e)
		{
			splitType = (SplitType)splitTypeComboBox.SelectedIndex;
			bigEndianCheckbox.Visible = splitType != SplitType.BinDLL && splitType != SplitType.NB;
			bigEndianCheckbox.Enabled = bigEndianCheckbox.Visible;

			SetControls();
		}

		private void animFilesList_SelectedIndexChanged(object sender, EventArgs e)
		{
			RemoveAnimButton.Enabled = animFilesList.SelectedItems.Count > 0;
		}

		private void AddNewAnimFileButton_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog fileDialog = new OpenFileDialog())
			{
				if(fileDialog.ShowDialog() == DialogResult.OK)
				{
					if (animFiles.Contains(fileDialog.FileName))
					{
						MessageBox.Show("Cannot add the same file twice.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
					else if (animFiles.Contains(filePathText.Text))
					{
						MessageBox.Show("Model file is not a valid animation file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					else
					{
						animFilesList.Items.Add(fileDialog.FileName);

						RemoveAnimButton.Enabled = true;

						animFiles = new string[animFilesList.Items.Count];

						for (int i = 0; i < animFilesList.Items.Count; i++)
						{
							animFiles[i] = animFilesList.Items[i].Text;
						}
					}
				}
			}
		}

		private void RemoveAnimButton_Click(object sender, EventArgs e)
		{
			if(animFilesList.SelectedItems.Count > 0)
			{
				List<ListViewItem> removeItems = new List<ListViewItem>();

				foreach (ListViewItem removeItem in animFilesList.SelectedItems)
				{
					removeItems.Add(removeItem);
				}

				foreach(ListViewItem removeItem in removeItems)
				{
					animFilesList.Items.Remove(removeItem);
				}

				RemoveAnimButton.Enabled = animFilesList.SelectedItems.Count > 0;

				animFiles = new string[animFilesList.Items.Count];

				for (int i = 0; i < animFilesList.Items.Count; i++)
				{
					animFiles[i] = animFilesList.Items[i].Text;
				}
			}
		}

		private void bigEndianCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			isBigEndian = bigEndianCheckbox.Checked;
		}
	}
}
