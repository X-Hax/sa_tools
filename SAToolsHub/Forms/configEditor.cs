using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAModel.SAEditorCommon.ModManagement;

namespace SAToolsHub.Forms
{
	public partial class configEditor : Form
	{
		public ConfigSchema schema = new ConfigSchema();
		public List<ConfigSchemaGroup> groups = new List<ConfigSchemaGroup>();
		public List<ConfigSchemaEnum> enums = new List<ConfigSchemaEnum>();
		public List<ConfigSchemaProperty> properties = new List<ConfigSchemaProperty>();
		public List<ConfigSchemaEnumMember> members = new List<ConfigSchemaEnumMember>();

		public List<string> dataTypes = new List<string>()
		{
			"bool",
			"int",
			"float"
		};

		public List<string> propBtns;

		public List<string> enumBtns;

		public List<string> boolTypes = new List<string>()
		{
			"True",
			"False"
		};

		public configEditor()
		{
			InitializeComponent();

			propBtns = new List<string>()
			{
				tsPropsAdd.Name,
				tsPropSeparator.Name,
				tsPropsRem.Name
			};

			enumBtns = new List<string>()
			{
				tsEnumsAdd.Name,
				tsEnumSeparator.Name,
				tsEnumsRem.Name
			};
		}

		private void OpenFile(string path)
		{
			schema = ConfigSchema.Load(path);

			if (schema.Groups != null)
				groups.AddRange(schema.Groups);

			if (schema.Enums != null)
				enums.AddRange(schema.Enums);

			foreach (ConfigSchemaGroup group in groups)
			{
				ToolStripMenuItem item = new ToolStripMenuItem(group.DisplayName);
				tsPropsGroup.DropDownItems.Add(item);
			}

			foreach (ConfigSchemaEnum csEnum in enums)
			{
				ToolStripMenuItem item = new ToolStripMenuItem(csEnum.Name);
				tsEnumsGroup.DropDownItems.Add(item);
				dataTypes.Add(csEnum.Name);
			}
		}

		private void SaveFile(string path)
		{

		}

		private string GetPropertyDefault(ConfigSchemaProperty prop)
		{
			var propType = prop.Type;
			string propDefault = "";
			switch (propType)
			{
				case "bool":
					propDefault = boolTypes.Find(x => x == prop.DefaultValue.ToString());
					break;
				case "int":
				case "float":
					propDefault = prop.DefaultValue;
					break;
				default:
					ConfigSchemaEnum csEnum = enums.Find(x => x.Name == propType);
					var tmp = csEnum.Members.Find(x => x.Name == prop.DefaultValue || x.DisplayName == prop.DefaultValue);
					if (tmp.DisplayName != null)
						propDefault = tmp.DisplayName;
					else
						propDefault = tmp.Name;
					break;
			}
			return propDefault;
		}

		private void LoadProperties(ConfigSchemaGroup group)
		{
			if (dataGridView1.Columns.Count != 8)
				BuildPropertiesGrid();
			else
				dataGridView1.Rows.Clear();

			properties.Clear();
			properties.AddRange(group.Properties);
			foreach (ConfigSchemaProperty prop in properties)
			{
				var propType = dataTypes.Find(x => x == prop.Type);
				var propDefault = GetPropertyDefault(prop);
				var propInclude = boolTypes.Find(x => x == prop.AlwaysInclude.ToString());
				DataGridViewRow row = new DataGridViewRow();
				dataGridView1.Rows.Add(prop.Name, prop.DisplayName, 
					propType, propDefault, prop.MinValue, prop.MaxValue, propInclude, prop.HelpText);
			}
		}

		private void BuildPropertiesGrid()
		{
			dataGridView1.Columns.Clear();
			dataGridView1.Rows.Clear();
			DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
			colName.Name = "name";
			colName.Width = 150;
			colName.HeaderText = "Name";
			colName.SortMode = DataGridViewColumnSortMode.NotSortable;
			DataGridViewTextBoxColumn colDisplayName = new DataGridViewTextBoxColumn();
			colDisplayName.Name = "displayname";
			colDisplayName.Width = 150;
			colDisplayName.HeaderText = "Display Name";
			colDisplayName.SortMode = DataGridViewColumnSortMode.NotSortable;
			DataGridViewComboBoxColumn colType = new DataGridViewComboBoxColumn();
			colType.Name = "type";
			colType.Width = 75;
			colType.HeaderText = "Data Type";
			colType.Items.AddRange(dataTypes.ToArray());
			DataGridViewColumn colValue = new DataGridViewColumn();
			colValue.Name = "defaultvalue";
			colValue.Width = 75;
			colValue.HeaderText = "Default Value";
			colValue.CellTemplate = new DataGridViewTextBoxCell();
			DataGridViewTextBoxColumn colMin = new DataGridViewTextBoxColumn();
			colMin.Name = "minvalue";
			colMin.Width = 75;
			colMin.HeaderText = "Min Value";
			colMin.SortMode = DataGridViewColumnSortMode.NotSortable;
			DataGridViewTextBoxColumn colMax = new DataGridViewTextBoxColumn();
			colMax.Name = "maxvalue";
			colMax.Width = 75;
			colMax.HeaderText = "Max Value";
			colMax.SortMode = DataGridViewColumnSortMode.NotSortable;
			DataGridViewComboBoxColumn colInclude = new DataGridViewComboBoxColumn();
			colInclude.Name = "alwaysinclude";
			colInclude.Width = 75;
			colInclude.HeaderText = "Always Include";
			colInclude.Items.AddRange(boolTypes.ToArray());
			DataGridViewTextBoxColumn colHelpText = new DataGridViewTextBoxColumn();
			colHelpText.Name = "helptext";
			colHelpText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			colHelpText.HeaderText = "Help Text";
			colHelpText.SortMode = DataGridViewColumnSortMode.NotSortable;

			dataGridView1.Columns.Add(colName);
			dataGridView1.Columns.Add(colDisplayName);
			dataGridView1.Columns.Add(colType);
			dataGridView1.Columns.Add(colValue);
			dataGridView1.Columns.Add(colMin);
			dataGridView1.Columns.Add(colMax);
			dataGridView1.Columns.Add(colInclude);
			dataGridView1.Columns.Add(colHelpText);
		}

		private void LoadEnums(ConfigSchemaEnum csEnum)
		{
			if (dataGridView1.Columns.Count != 2)
				BuildEnumGrid();
			else
				dataGridView1.Rows.Clear();

			members.Clear();
			members.AddRange(csEnum.Members);
			foreach (ConfigSchemaEnumMember member in csEnum.Members)
			{
				dataGridView1.Rows.Add(member.Name, member.DisplayName);
			}
		}

		private void BuildEnumGrid()
		{
			dataGridView1.Columns.Clear();
			dataGridView1.Rows.Clear();
			DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
			colName.Name = "name";
			colName.Width = 300;
			colName.HeaderText = "Name";
			DataGridViewTextBoxColumn colDisplayName = new DataGridViewTextBoxColumn();
			colDisplayName.Name = "displayname";
			colDisplayName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			colDisplayName.HeaderText = "Display Name";

			dataGridView1.Columns.Add(colName);
			dataGridView1.Columns.Add(colDisplayName);
		}

		private void tsOpen_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Config Schema (*.xml)|*.xml";

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				OpenFile(openFileDialog.FileName);
			}
		}

		private void ClearCheckedButtons()
		{
			foreach (ToolStripMenuItem item in tsPropsGroup.DropDownItems.OfType<ToolStripMenuItem>())
				item.Checked = false;

			foreach (ToolStripMenuItem item in tsEnumsGroup.DropDownItems.OfType<ToolStripMenuItem>())
				item.Checked = false;
		}

		private void tsPropsGroup_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (!propBtns.Contains(e.ClickedItem.Name))
			{
				ClearCheckedButtons();

				ToolStripMenuItem btn = e.ClickedItem as ToolStripMenuItem;
				btn.Checked = true;
				ConfigSchemaGroup group = groups.Find(x => x.DisplayName == e.ClickedItem.Text);
				LoadProperties(group);
			}
		}

		private void tsEnumsGroup_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (!enumBtns.Contains(e.ClickedItem.Name))
			{
				ClearCheckedButtons();

				ToolStripMenuItem btn = e.ClickedItem as ToolStripMenuItem;
				btn.Checked = true;
				ConfigSchemaEnum csEnum = enums.Find(x => x.Name == e.ClickedItem.Text);
				LoadEnums(csEnum);
			}
		}

		private void tsNew_Click(object sender, EventArgs e)
		{
			schema = new ConfigSchema();
			groups = new List<ConfigSchemaGroup>();
			enums = new List<ConfigSchemaEnum>();

			dataGridView1.Columns.Clear();
			dataGridView1.Rows.Clear();

			tsPropsGroup.DropDownItems.Clear();
			tsPropsGroup.DropDownItems.Add(tsPropsAdd);
			tsPropsGroup.DropDownItems.Add(tsPropsRem);
			tsPropsGroup.DropDownItems.Add(tsPropSeparator);
			tsEnumsGroup.DropDownItems.Clear();
			tsEnumsGroup.DropDownItems.Add(tsEnumsAdd);
			tsEnumsGroup.DropDownItems.Add(tsEnumsRem);
			tsEnumsGroup.DropDownItems.Add(tsEnumSeparator);
		}

		private void RefreshDataGrid()
		{
			dataGridView1.Refresh();
		}

		private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			string type = dataGridView1.Rows[e.RowIndex].Cells[2].Value as string;

			if (type != null)
			{
				// Validating Type
				if (e.ColumnIndex == 3)
				{
					switch (type)
					{
						case "bool":
							if (!boolTypes.Contains((string)e.FormattedValue))
							{
								MessageBox.Show("Invalid input!\n\nPlease enter a valid input:\nValid Inputs: True, False");
								dataGridView1.CurrentCell.Value = null;
								e.Cancel = true;
							}
							break;
						case "int":
							if (!int.TryParse((string)e.FormattedValue, out int iResult))
							{
								MessageBox.Show("Invalid input!\n\nPlease enter a int value.");
								dataGridView1.CurrentCell.Value = null;
								e.Cancel = true;
							}
							break;
						case "float":
							if (!float.TryParse((string)e.FormattedValue, out float fResult))
							{
								MessageBox.Show("Invalid input!\n\nPlease enter a float value.");
								dataGridView1.CurrentCell.Value = null;
								e.Cancel = true;
							}
							break;
						default:
							List<string> enumList = GetEnumMembers(type);

							string validData = "";
							foreach (string str in enumList)
							{
								if (str == enumList[enumList.Count - 1])
									validData += (str);
								else
									validData += (str + ", ");
							}

							if (!enumList.Contains((string)e.FormattedValue))
							{
								MessageBox.Show("Invalid input!\n\nPlease a valid input:\nValid Inputs: " + validData);
								dataGridView1.CurrentCell.Value = null;
								e.Cancel = true;
							}
							break;
					}
				}

				// Validating Min/Max Value
				if (e.ColumnIndex == 4 || e.ColumnIndex == 5)
				{
					switch (type)
					{
						case "int":
							if (!int.TryParse((string)e.FormattedValue, out int iResult))
							{
								MessageBox.Show("Invalid input!\n\nPlease enter a int value.");
								dataGridView1.CurrentCell.Value = null;
								e.Cancel = true;
							}
							break;
						case "float":
							if (!float.TryParse((string)e.FormattedValue, out float fResult))
							{
								MessageBox.Show("Invalid input!\n\nPlease enter a float value.");
								dataGridView1.CurrentCell.Value = null;
								e.Cancel = true;
							}
							break;
						default:
							if (e.FormattedValue != "")
							{
								MessageBox.Show(type + " is not int or float. Please leave this empty.");
								dataGridView1.CurrentCell.Value = null;
								e.Cancel = true;
							}
							break;
					}
				}
			}
		}

		private List<string> GetEnumMembers(string type)
		{
			List<string> enumList = new List<string>();
			ConfigSchemaEnum csEnum = enums.Find(x => x.Name == type);
			foreach (ConfigSchemaEnumMember member in csEnum.Members)
			{
				if (member.DisplayName != null)
					enumList.Add(member.DisplayName);
				else
					enumList.Add(member.Name);
			}

			return enumList;
		}

		private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 2)
			{
				string type = dataGridView1.Rows[e.RowIndex].Cells[2].Value as string;

				switch(type)
				{
					case "bool":
						if (dataGridView1.Rows[e.RowIndex].Cells[3].Value == null || !boolTypes.Contains(dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString()))
							dataGridView1.Rows[e.RowIndex].Cells[3].Value = boolTypes[0];
						dataGridView1.Rows[e.RowIndex].Cells[4].Value = null;
						dataGridView1.Rows[e.RowIndex].Cells[5].Value = null;
						break;
					case "int":
						if (!int.TryParse((string)dataGridView1.Rows[e.RowIndex].Cells[3].Value, out int defIntResult))
							dataGridView1.Rows[e.RowIndex].Cells[3].Value = 0;
						if (!int.TryParse((string)dataGridView1.Rows[e.RowIndex].Cells[4].Value, out int minIntResult))
							dataGridView1.Rows[e.RowIndex].Cells[4].Value = 0;
						if (!int.TryParse((string)dataGridView1.Rows[e.RowIndex].Cells[5].Value, out int maxIntResult))
							dataGridView1.Rows[e.RowIndex].Cells[5].Value = 0;
						break;
					case "float":
						if (!float.TryParse((string)dataGridView1.Rows[e.RowIndex].Cells[3].Value, out float defFloatResult))
							dataGridView1.Rows[e.RowIndex].Cells[3].Value = 0;
						if (!float.TryParse((string)dataGridView1.Rows[e.RowIndex].Cells[4].Value, out float minFloatResult))
							dataGridView1.Rows[e.RowIndex].Cells[4].Value = 0;
						if (!float.TryParse((string)dataGridView1.Rows[e.RowIndex].Cells[5].Value, out float maxFloatResult))
							dataGridView1.Rows[e.RowIndex].Cells[5].Value = 0;
						break;
					default:
						List<string> enumList = GetEnumMembers(type);
						if (!enumList.Contains((string)dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString()))
							dataGridView1.Rows[e.RowIndex].Cells[3].Value = enumList[0];
						dataGridView1.Rows[e.RowIndex].Cells[4].Value = null;
						dataGridView1.Rows[e.RowIndex].Cells[5].Value = null;
						break;
				}
			}
		}

		private void tsElementAdd_Click(object sender, EventArgs e)
		{
			ConfigSchemaProperty newProp = new ConfigSchemaProperty();
			string rowId = (dataGridView1.Rows.Count + 1).ToString();
			newProp.Name = "Property " + rowId;
			newProp.DisplayName = "Display Name " + rowId;
			newProp.Type = dataTypes[0];
			newProp.DefaultValue = boolTypes[0];
			newProp.AlwaysInclude = false;
			properties.Add(newProp);
			dataGridView1.Rows.Add(newProp.Name, newProp.DisplayName,
					newProp.Type, newProp.DefaultValue, newProp.MinValue, newProp.MaxValue, "False", newProp.HelpText);
		}

		private void tsElementRem_Click(object sender, EventArgs e)
		{
			var rowId = dataGridView1.SelectedRows[0].Index;
		}
	}
}
