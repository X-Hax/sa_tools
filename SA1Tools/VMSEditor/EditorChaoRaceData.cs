using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VMSEditor
{
	public partial class EditorChaoRaceData : Form
	{
		public VMS_ChaoGardenSave SaveFile;

		public EditorChaoRaceData()
		{
			InitializeComponent();
		}

		private void SetData()
		{
			// Pearl
			textBoxNamePearl1.Text = SaveFile.PearlCourseNames[0];
			textBoxNamePearl2.Text = SaveFile.PearlCourseNames[1];
			textBoxNamePearl3.Text = SaveFile.PearlCourseNames[2]; 
			numericUpDownMilisecondsPearl1.Value = SaveFile.PearlCourseTimes[0] * 10 / 6;
			numericUpDownSecondsPearl1.Value = SaveFile.PearlCourseTimes[1];
			numericUpDownMinutesPearl1.Value = SaveFile.PearlCourseTimes[2];
			numericUpDownMilisecondsPearl2.Value = SaveFile.PearlCourseTimes[3] * 10 / 6;
			numericUpDownSecondsPearl2.Value = SaveFile.PearlCourseTimes[4];
			numericUpDownMinutesPearl2.Value = SaveFile.PearlCourseTimes[5];
			numericUpDownMilisecondsPearl3.Value = SaveFile.PearlCourseTimes[6] * 10 / 6;
			numericUpDownSecondsPearl3.Value = SaveFile.PearlCourseTimes[7];
			numericUpDownMinutesPearl3.Value = SaveFile.PearlCourseTimes[8];
			// Amethyst
			textBoxNameAme1.Text = SaveFile.AmethystCourseNames[0];
			textBoxNameAme2.Text = SaveFile.AmethystCourseNames[1];
			textBoxNameAme3.Text = SaveFile.AmethystCourseNames[2];
			numericUpDownMilisecondsAme1.Value = SaveFile.AmethystCourseTimes[0] * 10 / 6;
			numericUpDownSecondsAme1.Value = SaveFile.AmethystCourseTimes[1];
			numericUpDownMinutesAme1.Value = SaveFile.AmethystCourseTimes[2];
			numericUpDownMilisecondsAme2.Value = SaveFile.AmethystCourseTimes[3] * 10 / 6;
			numericUpDownSecondsAme2.Value = SaveFile.AmethystCourseTimes[4];
			numericUpDownMinutesAme2.Value = SaveFile.AmethystCourseTimes[5];
			numericUpDownMilisecondsAme3.Value = SaveFile.AmethystCourseTimes[6] * 10 / 6;
			numericUpDownSecondsAme3.Value = SaveFile.AmethystCourseTimes[7];
			numericUpDownMinutesAme3.Value = SaveFile.AmethystCourseTimes[8];
			// Sapphire
			textBoxNameSap1.Text = SaveFile.SapphireCourseNames[0];
			textBoxNameSap2.Text = SaveFile.SapphireCourseNames[1];
			textBoxNameSap3.Text = SaveFile.SapphireCourseNames[2];
			numericUpDownMilisecondsSap1.Value = SaveFile.SapphireCourseTimes[0] * 10 / 6;
			numericUpDownSecondsSap1.Value = SaveFile.SapphireCourseTimes[1];
			numericUpDownMinutesSap1.Value = SaveFile.SapphireCourseTimes[2];
			numericUpDownMilisecondsSap2.Value = SaveFile.SapphireCourseTimes[3] * 10 / 6;
			numericUpDownSecondsSap2.Value = SaveFile.SapphireCourseTimes[4];
			numericUpDownMinutesSap2.Value = SaveFile.SapphireCourseTimes[5];
			numericUpDownMilisecondsSap3.Value = SaveFile.SapphireCourseTimes[6] * 10 / 6;
			numericUpDownSecondsSap3.Value = SaveFile.SapphireCourseTimes[7];
			numericUpDownMinutesSap3.Value = SaveFile.SapphireCourseTimes[8];
			// Ruby
			textBoxNameRub1.Text = SaveFile.RubyCourseNames[0];
			textBoxNameRub2.Text = SaveFile.RubyCourseNames[1];
			textBoxNameRub3.Text = SaveFile.RubyCourseNames[2];
			numericUpDownMilisecondsRub1.Value = SaveFile.RubyCourseTimes[0] * 10 / 6;
			numericUpDownSecondsRub1.Value = SaveFile.RubyCourseTimes[1];
			numericUpDownMinutesRub1.Value = SaveFile.RubyCourseTimes[2];
			numericUpDownMilisecondsRub2.Value = SaveFile.RubyCourseTimes[3] * 10 / 6;
			numericUpDownSecondsRub2.Value = SaveFile.RubyCourseTimes[4];
			numericUpDownMinutesRub2.Value = SaveFile.RubyCourseTimes[5];
			numericUpDownMilisecondsRub3.Value = SaveFile.RubyCourseTimes[6] * 10 / 6;
			numericUpDownSecondsRub3.Value = SaveFile.RubyCourseTimes[7];
			numericUpDownMinutesRub3.Value = SaveFile.RubyCourseTimes[8];
			// Emerald
			textBoxNameEme1.Text = SaveFile.EmeraldCourseNames[0];
			textBoxNameEme2.Text = SaveFile.EmeraldCourseNames[1];
			textBoxNameEme3.Text = SaveFile.EmeraldCourseNames[2];
			numericUpDownMilisecondsEme1.Value = SaveFile.EmeraldCourseTimes[0] * 10 / 6;
			numericUpDownSecondsEme1.Value = SaveFile.EmeraldCourseTimes[1];
			numericUpDownMinutesEme1.Value = SaveFile.EmeraldCourseTimes[2];
			numericUpDownMilisecondsEme2.Value = SaveFile.EmeraldCourseTimes[3] * 10 / 6;
			numericUpDownSecondsEme2.Value = SaveFile.EmeraldCourseTimes[4];
			numericUpDownMinutesEme2.Value = SaveFile.EmeraldCourseTimes[5];
			numericUpDownMilisecondsEme3.Value = SaveFile.EmeraldCourseTimes[6] * 10 / 6;
			numericUpDownSecondsEme3.Value = SaveFile.EmeraldCourseTimes[7];
			numericUpDownMinutesEme3.Value = SaveFile.EmeraldCourseTimes[8];
			// Flags
			numericUpDownFlag0.Value = SaveFile.UnknownFlag0;
			numericUpDownFlag1.Value = SaveFile.UnknownFlag1;
			numericUpDownFlag2.Value = SaveFile.UnknownFlag2;
			numericUpDownFlag3.Value = SaveFile.UnknownFlag3;
			numericUpDownFlag4.Value = SaveFile.UnknownFlag4;
			numericUpDownFlag5.Value = SaveFile.UnknownFlag5;
			numericUpDownFlag6.Value = SaveFile.UnknownFlag6;
			numericUpDownFlag7.Value = SaveFile.UnknownFlag7;
			numericUpDownFlag8.Value = SaveFile.UnknownFlag8;
		}

		private void SaveData()
		{
			// Pearl
			SaveFile.PearlCourseNames[0] = textBoxNamePearl1.Text;
			SaveFile.PearlCourseNames[1] = textBoxNamePearl2.Text;
			SaveFile.PearlCourseNames[2] = textBoxNamePearl3.Text;
			SaveFile.PearlCourseTimes[0] = (byte)(numericUpDownMilisecondsPearl1.Value * 6 / 10);
			SaveFile.PearlCourseTimes[1] = (byte)numericUpDownSecondsPearl1.Value;
			SaveFile.PearlCourseTimes[2] = (byte)numericUpDownMinutesPearl1.Value;
			SaveFile.PearlCourseTimes[3] = (byte)(numericUpDownMilisecondsPearl2.Value * 6 / 10);
			SaveFile.PearlCourseTimes[4] = (byte)numericUpDownSecondsPearl2.Value;
			SaveFile.PearlCourseTimes[5] = (byte)numericUpDownMinutesPearl2.Value;
			SaveFile.PearlCourseTimes[6] = (byte)(numericUpDownMilisecondsPearl3.Value * 6 / 10);
			SaveFile.PearlCourseTimes[7] = (byte)numericUpDownSecondsPearl3.Value;
			SaveFile.PearlCourseTimes[8] = (byte)numericUpDownMinutesPearl3.Value;
			// Amethyst
			SaveFile.AmethystCourseNames[0] = textBoxNameAme1.Text;
			SaveFile.AmethystCourseNames[1] = textBoxNameAme2.Text;
			SaveFile.AmethystCourseNames[2] = textBoxNameAme3.Text;
			SaveFile.AmethystCourseTimes[0] = (byte)(numericUpDownMilisecondsAme1.Value * 6 / 10);
			SaveFile.AmethystCourseTimes[1] = (byte)numericUpDownSecondsAme1.Value;
			SaveFile.AmethystCourseTimes[2] = (byte)numericUpDownMinutesAme1.Value;
			SaveFile.AmethystCourseTimes[3] = (byte)(numericUpDownMilisecondsAme2.Value * 6 / 10);
			SaveFile.AmethystCourseTimes[4] = (byte)numericUpDownSecondsAme2.Value;
			SaveFile.AmethystCourseTimes[5] = (byte)numericUpDownMinutesAme2.Value;
			SaveFile.AmethystCourseTimes[6] = (byte)(numericUpDownMilisecondsAme3.Value * 6 / 10);
			SaveFile.AmethystCourseTimes[7] = (byte)numericUpDownSecondsAme3.Value;
			SaveFile.AmethystCourseTimes[8] = (byte)numericUpDownMinutesAme3.Value;
			// Sapphire
			SaveFile.SapphireCourseNames[0] = textBoxNameSap1.Text;
			SaveFile.SapphireCourseNames[1] = textBoxNameSap2.Text;
			SaveFile.SapphireCourseNames[2] = textBoxNameSap3.Text;
			SaveFile.SapphireCourseTimes[0] = (byte)(numericUpDownMilisecondsSap1.Value * 6 / 10);
			SaveFile.SapphireCourseTimes[1] = (byte)numericUpDownSecondsSap1.Value;
			SaveFile.SapphireCourseTimes[2] = (byte)numericUpDownMinutesSap1.Value;
			SaveFile.SapphireCourseTimes[3] = (byte)(numericUpDownMilisecondsSap2.Value * 6 / 10);
			SaveFile.SapphireCourseTimes[4] = (byte)numericUpDownSecondsSap2.Value;
			SaveFile.SapphireCourseTimes[5] = (byte)numericUpDownMinutesSap2.Value;
			SaveFile.SapphireCourseTimes[6] = (byte)(numericUpDownMilisecondsSap3.Value * 6 / 10);
			SaveFile.SapphireCourseTimes[7] = (byte)numericUpDownSecondsSap3.Value;
			SaveFile.SapphireCourseTimes[8] = (byte)numericUpDownMinutesSap3.Value;
			// Ruby
			SaveFile.RubyCourseNames[0] = textBoxNameRub1.Text;
			SaveFile.RubyCourseNames[1] = textBoxNameRub2.Text;
			SaveFile.RubyCourseNames[2] = textBoxNameRub3.Text;
			SaveFile.RubyCourseTimes[0] = (byte)(numericUpDownMilisecondsRub1.Value * 6 / 10);
			SaveFile.RubyCourseTimes[1] = (byte)numericUpDownSecondsRub1.Value;
			SaveFile.RubyCourseTimes[2] = (byte)numericUpDownMinutesRub1.Value;
			SaveFile.RubyCourseTimes[3] = (byte)(numericUpDownMilisecondsRub2.Value * 6 / 10);
			SaveFile.RubyCourseTimes[4] = (byte)numericUpDownSecondsRub2.Value;
			SaveFile.RubyCourseTimes[5] = (byte)numericUpDownMinutesRub2.Value;
			SaveFile.RubyCourseTimes[6] = (byte)(numericUpDownMilisecondsRub3.Value * 6 / 10);
			SaveFile.RubyCourseTimes[7] = (byte)numericUpDownSecondsRub3.Value;
			SaveFile.RubyCourseTimes[8] = (byte)numericUpDownMinutesRub3.Value;
			// Emerald
			SaveFile.EmeraldCourseNames[0] = textBoxNameEme1.Text;
			SaveFile.EmeraldCourseNames[1] = textBoxNameEme2.Text;
			SaveFile.EmeraldCourseNames[2] = textBoxNameEme3.Text;
			SaveFile.EmeraldCourseTimes[0] = (byte)(numericUpDownMilisecondsEme1.Value * 6 / 10);
			SaveFile.EmeraldCourseTimes[1] = (byte)numericUpDownSecondsEme1.Value;
			SaveFile.EmeraldCourseTimes[2] = (byte)numericUpDownMinutesEme1.Value;
			SaveFile.EmeraldCourseTimes[3] = (byte)(numericUpDownMilisecondsEme2.Value * 6 / 10);
			SaveFile.EmeraldCourseTimes[4] = (byte)numericUpDownSecondsEme2.Value;
			SaveFile.EmeraldCourseTimes[5] = (byte)numericUpDownMinutesEme2.Value;
			SaveFile.EmeraldCourseTimes[6] = (byte)(numericUpDownMilisecondsEme3.Value * 6 / 10);
			SaveFile.EmeraldCourseTimes[7] = (byte)numericUpDownSecondsEme3.Value;
			SaveFile.EmeraldCourseTimes[8] = (byte)numericUpDownMinutesEme3.Value;
			// Flags
			SaveFile.UnknownFlag0 = (byte)numericUpDownFlag0.Value;
			SaveFile.UnknownFlag1 = (byte)numericUpDownFlag1.Value;
			SaveFile.UnknownFlag2 = (byte)numericUpDownFlag2.Value;
			SaveFile.UnknownFlag3 = (byte)numericUpDownFlag3.Value;
			SaveFile.UnknownFlag4 = (byte)numericUpDownFlag4.Value;
			SaveFile.UnknownFlag5 = (byte)numericUpDownFlag5.Value;
			SaveFile.UnknownFlag6 = (byte)numericUpDownFlag6.Value;
			SaveFile.UnknownFlag7 = (byte)numericUpDownFlag7.Value;
			SaveFile.UnknownFlag8 = (byte)numericUpDownFlag8.Value;
		}

		public EditorChaoRaceData(VMS_ChaoGardenSave save)
		{
			InitializeComponent();
			SaveFile = save;
			SetData();
		}

		private void buttonReset_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "This operation cannot be undone. Continue?", "Chao Editor Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
				return;
			List<VMS_Chao> vmschao = new List<VMS_Chao>();
			foreach (VMS_Chao chao in SaveFile.GardenChao)
				vmschao.Add(chao);
			SaveFile = new VMS_ChaoGardenSave();
			SaveFile.GardenChao.Clear();
			foreach (VMS_Chao chao in vmschao)
				SaveFile.GardenChao.Add(chao);
			SetData();
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			SaveData();
		}
	}
}
