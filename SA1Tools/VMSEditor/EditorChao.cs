using VMSEditor.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace VMSEditor
{
	public partial class EditorChao : Form
	{
		enum ChaoSaveMode
		{
			DownloadData,
			ChaoAdventure,
			UploadData,
			GardenFile
		}

		public List<VMS_Chao> chaoDataList = new List<VMS_Chao>();
		string currentFilename = "";
		int previousChaoIndex = -1;
		private readonly Form ProgramModeSelectorForm;

		public EditorChao(Form parent = null)
		{
			ProgramModeSelectorForm = parent;
			InitializeComponent();
			DrawChaoEvolution();
			CreateFacePreview();
			if (Program.args.Length > 0)
				if (File.Exists(Program.args[0]))
					LoadChaoVMSFile(Program.args[0]);
			if (parent == null)
				Application.ThreadException += ProgramModeSelector.Application_ThreadException;
		}

		private void RefreshLabels()
		{
			if (listBoxDataSlots.SelectedIndex == -1)
				return;
			VMS_Chao chaoData = chaoDataList[listBoxDataSlots.SelectedIndex];
			checkBoxDeerBow.Checked = checkBoxSwallowTwirl.Checked = checkBoxSkunkDraw.Checked = checkBoxRabbitSomersault.Checked =
				checkBoxPeacockPose.Checked = checkBoxElephantSumo.Checked = checkBoxGorillaChest.Checked = checkBoxKoalaTrumpet.Checked =
				checkBoxLionWash.Checked = checkBoxMoleDig.Checked = checkBoxOtterSwim.Checked = checkBoxParrotSing.Checked =
				checkBoxPenguinSkate.Checked = checkBoxSealDance.Checked = checkBoxWallabyPunch.Checked = false;
			textBoxName.Text = chaoData.Name;
			comboBoxType.SelectedIndex = (byte)chaoData.Type;
			comboBoxGarden.SelectedIndex = (byte)chaoData.Garden;
			trackBarHappy.Value = chaoData.Happiness;
			numericUpDownKey1.Value = chaoData.Key1;
			numericUpDownKey2.Value = chaoData.Key2;
			numericUpDownKey3.Value = chaoData.Key3;
			numericUpDownKey4.Value = chaoData.Key4;
			numericUpDownSwim.Value = chaoData.Swim;
			numericUpDownFly.Value = chaoData.Fly;
			numericUpDownRun.Value = chaoData.Run;
			numericUpDownPower.Value = chaoData.Power;
			numericUpDownHP.Value = chaoData.HP;
			numericUpDownMaxHP.Value = chaoData.HP_Max;
			comboBoxFruit0.SelectedIndex = (int)chaoData.Fruits[0];
			comboBoxFruit1.SelectedIndex = (int)chaoData.Fruits[1];
			comboBoxFruit2.SelectedIndex = (int)chaoData.Fruits[2];
			comboBoxFruit3.SelectedIndex = (int)chaoData.Fruits[3];
			comboBoxFruit4.SelectedIndex = (int)chaoData.Fruits[4];
			comboBoxFruit5.SelectedIndex = (int)chaoData.Fruits[5];
			comboBoxFruit6.SelectedIndex = (int)chaoData.Fruits[6];
			comboBoxFruit7.SelectedIndex = (int)chaoData.Fruits[7];
			trackBarSwimFly.Value = (int)(chaoData.FlyOrSwim * 100);
			trackBarRunPower.Value = (int)(chaoData.RunOrPower * 100);
			trackBarMagnitude.Value = (int)(chaoData.Magnitude * 100);
			trackBarAffection.Value = chaoData.Affection;
			numericUpDownLifeSpan.Value = chaoData.LifeLeft;
			numericUpDownAgingFactor.Value = chaoData.AgingFactor;
			// Abilities
			if (chaoData.AnimalAbilities.HasFlag(AnimalFlagsSA1.Deer))
				checkBoxDeerBow.Checked = true;
			if (chaoData.AnimalAbilities.HasFlag(AnimalFlagsSA1.Gorilla))
				checkBoxGorillaChest.Checked = true;
			if (chaoData.AnimalAbilities.HasFlag(AnimalFlagsSA1.Elephant))
				checkBoxElephantSumo.Checked = true;
			if (chaoData.AnimalAbilities.HasFlag(AnimalFlagsSA1.Koala))
				checkBoxKoalaTrumpet.Checked = true;
			if (chaoData.AnimalAbilities.HasFlag(AnimalFlagsSA1.Lion))
				checkBoxLionWash.Checked = true;
			if (chaoData.AnimalAbilities.HasFlag(AnimalFlagsSA1.Mole))
				checkBoxMoleDig.Checked = true;
			if (chaoData.AnimalAbilities.HasFlag(AnimalFlagsSA1.Otter))
				checkBoxOtterSwim.Checked = true;
			if (chaoData.AnimalAbilities.HasFlag(AnimalFlagsSA1.Parrot))
				checkBoxParrotSing.Checked = true;
			if (chaoData.AnimalAbilities.HasFlag(AnimalFlagsSA1.Peacock))
				checkBoxPeacockPose.Checked = true;
			if (chaoData.AnimalAbilities.HasFlag(AnimalFlagsSA1.Penguin))
				checkBoxPenguinSkate.Checked = true;
			if (chaoData.AnimalAbilities.HasFlag(AnimalFlagsSA1.Rabbit))
				checkBoxRabbitSomersault.Checked = true;
			if (chaoData.AnimalAbilities.HasFlag(AnimalFlagsSA1.Seal))
				checkBoxSealDance.Checked = true;
			if (chaoData.AnimalAbilities.HasFlag(AnimalFlagsSA1.Skunk))
				checkBoxSkunkDraw.Checked = true;
			if (chaoData.AnimalAbilities.HasFlag(AnimalFlagsSA1.Swallow))
				checkBoxSwallowTwirl.Checked = true;
			if (chaoData.AnimalAbilities.HasFlag(AnimalFlagsSA1.Wallaby))
				checkBoxWallabyPunch.Checked = true;
			// Jewels
			if (chaoData.Jewels.HasFlag(ChaoJewelsSA1.Amethyst))
				checkBoxAmethyst.Checked = true;
			if (chaoData.Jewels.HasFlag(ChaoJewelsSA1.Pearl))
				checkBoxPearl.Checked = true;
			if (chaoData.Jewels.HasFlag(ChaoJewelsSA1.Ruby))
				checkBoxRuby.Checked = true;
			if (chaoData.Jewels.HasFlag(ChaoJewelsSA1.Sapphire))
				checkBoxSapphire.Checked = true;
			if (chaoData.Jewels.HasFlag(ChaoJewelsSA1.Emerald))
				checkBoxEmerald.Checked = true;
			checkBoxColorFlag0x1.Checked = checkBoxColorFlag0x10.Checked = checkBoxColorFlag0x20.Checked = checkBoxColorFlag0x80.Checked =
				checkBoxColorFlagBlack.Checked = checkBoxColorFlagSilver.Checked = checkBoxColorFlagGold.Checked = checkBoxColorFlagJewel.Checked = comboBoxJewelColor.Enabled = false;
			numericUpDown_ColorFlags.Value = (ushort)chaoData.Flags;
			if (chaoData.Flags.HasFlag(ChaoColorFlagsSA1.Flag1))
				checkBoxColorFlag0x1.Checked = true;
			if (chaoData.Flags.HasFlag(ChaoColorFlagsSA1.Flag10))
				checkBoxColorFlag0x10.Checked = true;
			if (chaoData.Flags.HasFlag(ChaoColorFlagsSA1.Flag20))
				checkBoxColorFlag0x20.Checked = true;
			if (chaoData.Flags.HasFlag(ChaoColorFlagsSA1.Flag80))
				checkBoxColorFlag0x80.Checked = true;
			if (chaoData.Flags.HasFlag(ChaoColorFlagsSA1.Black))
				checkBoxColorFlagBlack.Checked = true;
			if (chaoData.Flags.HasFlag(ChaoColorFlagsSA1.Silver))
				checkBoxColorFlagSilver.Checked = true;
			if (chaoData.Flags.HasFlag(ChaoColorFlagsSA1.Gold))
				checkBoxColorFlagGold.Checked = true;
			if (chaoData.Flags.HasFlag(ChaoColorFlagsSA1.Jewel))
				checkBoxColorFlagJewel.Checked = comboBoxJewelColor.Enabled = true;
			numericUpDownX.Value = (int)chaoData.Position.X;
			numericUpDownY.Value = (int)chaoData.Position.Y;
			numericUpDownZ.Value = (int)chaoData.Position.Z;
			numericUpDownAge.Value = chaoData.Age;
			numericUpDownID.Value = chaoData.ID;
			comboBoxAnimalHeadFront.SelectedIndex = (byte)chaoData.AnimalParts[0] != 0xFF ? (byte)chaoData.AnimalParts[0] : 16;
			comboBoxAnimalHeadBack.SelectedIndex = (byte)chaoData.AnimalParts[1] != 0xFF ? (byte)chaoData.AnimalParts[1] : 16;
			comboBoxAnimalEars.SelectedIndex = (byte)chaoData.AnimalParts[2] != 0xFF ? (byte)chaoData.AnimalParts[2] : 16;
			comboBoxAnimalWings.SelectedIndex = (byte)chaoData.AnimalParts[3] != 0xFF ? (byte)chaoData.AnimalParts[3] : 16;
			comboBoxAnimalArms.SelectedIndex = (byte)chaoData.AnimalParts[4] != 0xFF ? (byte)chaoData.AnimalParts[4] : 16;
			comboBoxAnimalFeet.SelectedIndex = (byte)chaoData.AnimalParts[5] != 0xFF ? (byte)chaoData.AnimalParts[5] : 16;
			comboBoxAnimalTail.SelectedIndex = (byte)chaoData.AnimalParts[6] != 0xFF ? (byte)chaoData.AnimalParts[6] : 16;
			pictureBoxHeadFront.Image = GetAnimalPartPicture((int)chaoData.AnimalParts[0]);
			pictureBoxHeadBack.Image = GetAnimalPartPicture((int)chaoData.AnimalParts[1]);
			pictureBoxEars.Image = GetAnimalPartPicture((int)chaoData.AnimalParts[2]);
			pictureBoxWings.Image = GetAnimalPartPicture((int)chaoData.AnimalParts[3]);
			pictureBoxArms.Image = GetAnimalPartPicture((int)chaoData.AnimalParts[4]);
			pictureBoxFeet.Image = GetAnimalPartPicture((int)chaoData.AnimalParts[5]);
			pictureBoxTail.Image = GetAnimalPartPicture((int)chaoData.AnimalParts[6]);
			// Face
			numericUpDownKindness.Value = chaoData.Kindness;
			numericUpDownAggressive.Value = chaoData.Aggressive;
			numericUpDownCurious.Value = chaoData.Curiosity;
			// Emotion
			numericUpDownCharm.Value = chaoData.Charm;
			numericUpDownHorny.Value = chaoData.Breed;
			numericUpDownSleepy.Value = chaoData.Sleep;
			numericUpDownHungry.Value = chaoData.Hunger;
			numericUpDownBored.Value = chaoData.Tedious;
			numericUpDownTired.Value = chaoData.Tiredness;
			numericUpDownStressed.Value = chaoData.Stress;
			numericUpDownNarrow.Value = chaoData.Narrow;
			numericUpDownJoyful.Value = chaoData.Pleasure;
			numericUpDownAngry.Value = chaoData.Anger;
			numericUpDownSad.Value = chaoData.Sorrow;
			numericUpDownFearful.Value = chaoData.Fear;
			numericUpDownLonely.Value = chaoData.Loneliness;
			// Character memories
			trackBarBondSonic.Value = chaoData.Memories.player[0].like;
			numericUpDownMeetSonic.Value = chaoData.Memories.player[0].meet;
			trackBarBondTails.Value = chaoData.Memories.player[1].like;
			numericUpDownMeetTails.Value = chaoData.Memories.player[1].meet;
			trackBarBondKnuckles.Value = chaoData.Memories.player[2].like;
			numericUpDownMeetKnuckles.Value = chaoData.Memories.player[2].meet;
			trackBarBondAmy.Value = chaoData.Memories.player[3].like;
			numericUpDownMeetAmy.Value = chaoData.Memories.player[3].meet;
			trackBarBondGamma.Value = chaoData.Memories.player[4].like;
			numericUpDownMeetGamma.Value = chaoData.Memories.player[4].meet;
			trackBarBondBig.Value = chaoData.Memories.player[5].like;
			numericUpDownMeetBig.Value = chaoData.Memories.player[5].meet;
			// Chao memories
			ReadChaoMemories();
			// Other stuff
			numericUpDownReincarnations.Value = chaoData.Reincarnations;
			numericUpDownRaceTrack.Value = chaoData.Lane;
			numericUpDownExists.Value = chaoData.Exists;
			numericUpDownCocoonTimer.Value = chaoData.CocoonTimer;
			// Chao Race results
			numericUpDownRace0.Value = chaoData.RaceTime[0];
			numericUpDownRace1.Value = chaoData.RaceTime[1];
			numericUpDownRace2.Value = chaoData.RaceTime[2];
			numericUpDownRace3.Value = chaoData.RaceTime[3];
			numericUpDownRace4.Value = chaoData.RaceTime[4];
			numericUpDownRace5.Value = chaoData.RaceTime[5];
			numericUpDownRace6.Value = chaoData.RaceTime[6];
			numericUpDownRace7.Value = chaoData.RaceTime[7];
			numericUpDownRace8.Value = chaoData.RaceTime[8];
			numericUpDownRace9.Value = chaoData.RaceTime[9];
			numericUpDownRace10.Value = chaoData.RaceTime[10];
			numericUpDownRace11.Value = chaoData.RaceTime[11];
			numericUpDownRace12.Value = chaoData.RaceTime[12];
			numericUpDownRace13.Value = chaoData.RaceTime[13];
			numericUpDownRace14.Value = chaoData.RaceTime[14];
			numericUpDownRace15.Value = chaoData.RaceTime[15];
			numericUpDownRace16.Value = chaoData.RaceTime[16];
			numericUpDownRace17.Value = chaoData.RaceTime[17];
			numericUpDownRace18.Value = chaoData.RaceTime[18];
			numericUpDownRace19.Value = chaoData.RaceTime[19];
			// Jewel breeds
			comboBoxJewelColor.SelectedIndex = (byte)chaoData.JewelBreed;
			// Evolution graph
			DrawChaoEvolution();
			// Face
			CreateFacePreview();
		}

		private void RefreshData(int index)
		{
			if (index == -1 || (chaoDataList.Count - 1) < index)
				return;
			VMS_Chao chaoData = chaoDataList[index];
			chaoData.Name = textBoxName.Text;
			chaoData.Type = (ChaoTypeSA1)comboBoxType.SelectedIndex;
			chaoData.Garden = (ChaoLocationSA1)comboBoxGarden.SelectedIndex;
			chaoData.Happiness = (sbyte)trackBarHappy.Value;
			chaoData.Key1 = (byte)numericUpDownKey1.Value;
			chaoData.Key2 = (byte)numericUpDownKey2.Value;
			chaoData.Key3 = (byte)numericUpDownKey3.Value;
			chaoData.Key4 = (byte)numericUpDownKey4.Value;
			chaoData.Fly = (ushort)numericUpDownFly.Value;
			chaoData.Run = (ushort)numericUpDownRun.Value;
			chaoData.Power = (ushort)numericUpDownPower.Value;
			chaoData.HP = (ushort)numericUpDownHP.Value;
			chaoData.HP_Max = (ushort)numericUpDownMaxHP.Value;
			chaoData.Fruits[0] = (ChaoFruitsSA1)comboBoxFruit0.SelectedIndex;
			chaoData.Fruits[1] = (ChaoFruitsSA1)comboBoxFruit1.SelectedIndex;
			chaoData.Fruits[2] = (ChaoFruitsSA1)comboBoxFruit2.SelectedIndex;
			chaoData.Fruits[3] = (ChaoFruitsSA1)comboBoxFruit3.SelectedIndex;
			chaoData.Fruits[4] = (ChaoFruitsSA1)comboBoxFruit4.SelectedIndex;
			chaoData.Fruits[5] = (ChaoFruitsSA1)comboBoxFruit5.SelectedIndex;
			chaoData.Fruits[6] = (ChaoFruitsSA1)comboBoxFruit6.SelectedIndex;
			chaoData.Fruits[7] = (ChaoFruitsSA1)comboBoxFruit7.SelectedIndex;
			chaoData.FlyOrSwim = (float)trackBarSwimFly.Value / 100.0f;
			chaoData.RunOrPower = (float)trackBarRunPower.Value / 100.0f;
			chaoData.Magnitude = (float)trackBarMagnitude.Value / 100.0f;
			chaoData.Affection = (ushort)trackBarAffection.Value;
			chaoData.LifeLeft = (ushort)numericUpDownLifeSpan.Value;
			chaoData.AgingFactor = (ushort)numericUpDownAgingFactor.Value;
			// Abilities
			AnimalFlagsSA1 flags = 0;
			if (checkBoxDeerBow.Checked)
				flags |= AnimalFlagsSA1.Deer;
			if (checkBoxGorillaChest.Checked)
				flags |= AnimalFlagsSA1.Gorilla;
			if (checkBoxElephantSumo.Checked)
				flags |= AnimalFlagsSA1.Elephant;
			if (checkBoxKoalaTrumpet.Checked)
				flags |= AnimalFlagsSA1.Koala;
			if (checkBoxLionWash.Checked)
				flags |= AnimalFlagsSA1.Lion;
			if (checkBoxMoleDig.Checked)
				flags |= AnimalFlagsSA1.Mole;
			if (checkBoxOtterSwim.Checked)
				flags |= AnimalFlagsSA1.Otter;
			if (checkBoxParrotSing.Checked)
				flags |= AnimalFlagsSA1.Parrot;
			if (checkBoxPeacockPose.Checked)
				flags |= AnimalFlagsSA1.Peacock;
			if (checkBoxPenguinSkate.Checked)
				flags |= AnimalFlagsSA1.Penguin;
			if (checkBoxRabbitSomersault.Checked)
				flags |= AnimalFlagsSA1.Rabbit;
			if (checkBoxSealDance.Checked)
				flags |= AnimalFlagsSA1.Seal;
			if (checkBoxSkunkDraw.Checked)
				flags |= AnimalFlagsSA1.Skunk;
			if (checkBoxSwallowTwirl.Checked)
				flags |= AnimalFlagsSA1.Swallow;
			if (checkBoxWallabyPunch.Checked)
				flags |= AnimalFlagsSA1.Wallaby;
			chaoData.AnimalAbilities = flags;
			// Chao Race Jewels
			ChaoJewelsSA1 raceJewelFlags = 0;
			if (checkBoxPearl.Checked)
				raceJewelFlags |= ChaoJewelsSA1.Pearl;
			if (checkBoxAmethyst.Checked)
				raceJewelFlags |= ChaoJewelsSA1.Amethyst;
			if (checkBoxSapphire.Checked)
				raceJewelFlags |= ChaoJewelsSA1.Sapphire;
			if (checkBoxRuby.Checked)
				raceJewelFlags |= ChaoJewelsSA1.Ruby;
			if (checkBoxEmerald.Checked)
				raceJewelFlags |= ChaoJewelsSA1.Emerald;
			chaoData.Jewels = raceJewelFlags;
			// Color
			ChaoColorFlagsSA1 colorFlags = 0;
			if (checkBoxColorFlag0x1.Checked)
				colorFlags |= ChaoColorFlagsSA1.Flag1;
			if (checkBoxColorFlag0x10.Checked)
				colorFlags |= ChaoColorFlagsSA1.Flag10;
			if (checkBoxColorFlag0x20.Checked)
				colorFlags |= ChaoColorFlagsSA1.Flag20;
			if (checkBoxColorFlag0x80.Checked)
				colorFlags |= ChaoColorFlagsSA1.Flag80;
			if (checkBoxColorFlagBlack.Checked)
				colorFlags |= ChaoColorFlagsSA1.Black;
			if (checkBoxColorFlagSilver.Checked)
				colorFlags |= ChaoColorFlagsSA1.Silver;
			if (checkBoxColorFlagGold.Checked)
				colorFlags |= ChaoColorFlagsSA1.Gold;
			if (checkBoxColorFlagJewel.Checked)
				colorFlags |= ChaoColorFlagsSA1.Jewel;
			chaoData.Flags = colorFlags;
			chaoData.Position.X = (float)numericUpDownX.Value;
			chaoData.Position.Y = (float)numericUpDownY.Value;
			chaoData.Position.Z = (float)numericUpDownZ.Value;
			chaoData.Age = (uint)numericUpDownAge.Value;
			chaoData.ID = (uint)numericUpDownID.Value;
			chaoData.AnimalParts[0] = comboBoxAnimalHeadFront.SelectedIndex != 16 ? (AnimalPartsSA1)comboBoxAnimalHeadFront.SelectedIndex : AnimalPartsSA1.None;
			chaoData.AnimalParts[1] = comboBoxAnimalHeadBack.SelectedIndex != 16 ? (AnimalPartsSA1)comboBoxAnimalHeadBack.SelectedIndex : AnimalPartsSA1.None;
			chaoData.AnimalParts[2] = comboBoxAnimalEars.SelectedIndex != 16 ? (AnimalPartsSA1)comboBoxAnimalEars.SelectedIndex : AnimalPartsSA1.None;
			chaoData.AnimalParts[3] = comboBoxAnimalWings.SelectedIndex != 16 ? (AnimalPartsSA1)comboBoxAnimalWings.SelectedIndex : AnimalPartsSA1.None;
			chaoData.AnimalParts[4] = comboBoxAnimalArms.SelectedIndex != 16 ? (AnimalPartsSA1)comboBoxAnimalArms.SelectedIndex : AnimalPartsSA1.None;
			chaoData.AnimalParts[5] = comboBoxAnimalFeet.SelectedIndex != 16 ? (AnimalPartsSA1)comboBoxAnimalFeet.SelectedIndex : AnimalPartsSA1.None;
			chaoData.AnimalParts[6] = comboBoxAnimalTail.SelectedIndex != 16 ? (AnimalPartsSA1)comboBoxAnimalTail.SelectedIndex : AnimalPartsSA1.None;
			// Face
			chaoData.Kindness = (sbyte)numericUpDownKindness.Value;
			chaoData.Aggressive = (sbyte)numericUpDownAggressive.Value;
			chaoData.Curiosity = (sbyte)numericUpDownCurious.Value;
			// Emotion
			chaoData.Charm = (byte)numericUpDownCharm.Value;
			chaoData.Breed = (byte)numericUpDownHorny.Value;
			chaoData.Sleep = (byte)numericUpDownSleepy.Value;
			chaoData.Hunger = (byte)numericUpDownHungry.Value;
			chaoData.Tedious = (byte)numericUpDownBored.Value;
			chaoData.Tiredness = (byte)numericUpDownTired.Value;
			chaoData.Stress = (byte)numericUpDownStressed.Value;
			chaoData.Narrow = (byte)numericUpDownNarrow.Value;
			chaoData.Pleasure = (byte)numericUpDownJoyful.Value;
			chaoData.Anger = (byte)numericUpDownAngry.Value;
			chaoData.Sorrow = (byte)numericUpDownSad.Value;
			chaoData.Fear = (byte)numericUpDownFearful.Value;
			chaoData.Loneliness = (byte)numericUpDownLonely.Value;
			// Character memories
			chaoData.Memories.player[0].like = (sbyte)trackBarBondSonic.Value;
			chaoData.Memories.player[0].meet = (byte)numericUpDownMeetSonic.Value;
			chaoData.Memories.player[1].like = (sbyte)trackBarBondTails.Value;
			chaoData.Memories.player[1].meet = (byte)numericUpDownMeetTails.Value;
			chaoData.Memories.player[2].like = (sbyte)trackBarBondKnuckles.Value;
			chaoData.Memories.player[2].meet = (byte)numericUpDownMeetKnuckles.Value;
			chaoData.Memories.player[3].like = (sbyte)trackBarBondAmy.Value;
			chaoData.Memories.player[3].meet = (byte)numericUpDownMeetAmy.Value;
			chaoData.Memories.player[4].like = (sbyte)trackBarBondGamma.Value;
			chaoData.Memories.player[4].meet = (byte)numericUpDownMeetGamma.Value;
			chaoData.Memories.player[5].like = (sbyte)trackBarBondBig.Value;
			chaoData.Memories.player[5].meet = (byte)numericUpDownMeetBig.Value;
			// Chao memories
			ReadChaoMemories();
			// Other stuff
			chaoData.Reincarnations = (byte)numericUpDownReincarnations.Value;
			chaoData.Lane = (byte)numericUpDownRaceTrack.Value;
			chaoData.Exists = (sbyte)numericUpDownExists.Value;
			chaoData.CocoonTimer = (ushort)numericUpDownCocoonTimer.Value;
			// Chao Race results
			chaoData.RaceTime[0] = (byte)numericUpDownRace0.Value;
			chaoData.RaceTime[1] = (byte)numericUpDownRace1.Value;
			chaoData.RaceTime[2] = (byte)numericUpDownRace2.Value;
			chaoData.RaceTime[3] = (byte)numericUpDownRace3.Value;
			chaoData.RaceTime[4] = (byte)numericUpDownRace4.Value;
			chaoData.RaceTime[5] = (byte)numericUpDownRace5.Value;
			chaoData.RaceTime[6] = (byte)numericUpDownRace6.Value;
			chaoData.RaceTime[7] = (byte)numericUpDownRace7.Value;
			chaoData.RaceTime[8] = (byte)numericUpDownRace8.Value;
			chaoData.RaceTime[9] = (byte)numericUpDownRace9.Value;
			chaoData.RaceTime[10] = (byte)numericUpDownRace10.Value;
			chaoData.RaceTime[11] = (byte)numericUpDownRace11.Value;
			chaoData.RaceTime[12] = (byte)numericUpDownRace12.Value;
			chaoData.RaceTime[13] = (byte)numericUpDownRace13.Value;
			chaoData.RaceTime[14] = (byte)numericUpDownRace14.Value;
			chaoData.RaceTime[15] = (byte)numericUpDownRace15.Value;
			chaoData.RaceTime[16] = (byte)numericUpDownRace16.Value;
			chaoData.RaceTime[17] = (byte)numericUpDownRace17.Value;
			chaoData.RaceTime[18] = (byte)numericUpDownRace18.Value;
			chaoData.RaceTime[19] = (byte)numericUpDownRace19.Value;
			// Jewel breeds
			chaoData.JewelBreed = (ChaoJewelBreedsSA1)comboBoxJewelColor.SelectedIndex;
		}

		private void RefreshChaoList()
		{
			listBoxDataSlots.Items.Clear();
			for (int i = 0; i < chaoDataList.Count; i++)
			{
				if (chaoDataList[i].Type != ChaoTypeSA1.EmptySlot)
					listBoxDataSlots.Items.Add(i.ToString() + ": " + chaoDataList[i].Name + " " + ToStringEnums(chaoDataList[i].Type) + " in " + ToStringEnums(chaoDataList[i].Garden));
				else
					listBoxDataSlots.Items.Add("Empty");
			}
		}

		private void LoadChaoVMSFile(string filename)
		{
			byte[] file = File.ReadAllBytes(filename);
			byte[] chaodata_s1 = new byte[512];
			byte[] chaodata_s2 = new byte[512];
			bool found = false;
			// Garden save
			if (System.Text.Encoding.GetEncoding(932).GetString(file, 0, 6) == "CHAO_S" || System.Text.Encoding.GetEncoding(932).GetString(file, 0, 6) == "A-LIFE")
			{
				listBoxDataSlots.Items.Clear();
				chaoDataList.Clear();
				for (int i = 0; i < 24; i++)
				{
					byte[] data = new byte[512];
					Array.Copy(file, 0x800 + i * 512, data, 0, 512);
					chaoDataList.Add(new VMS_Chao(data));
				}
				RefreshChaoList();
				listBoxDataSlots.SelectedIndex = 0;
				RefreshLabels();
				currentFilename = filename;
				toolStripStatusLabelFilename.Text = Path.GetFileName(currentFilename);
				return;
			}
			// Download Data / Chao Adventure
			for (int u = 0; u < file.Length - 11; u++)
			{
				if (System.Text.Encoding.GetEncoding(932).GetString(file, u, 11) == "CHAO ACCEPT")
				{
					Array.Copy(file, BitConverter.ToInt16(file, u + 0xC), chaodata_s1, 0, 512);
					short slot2addr = BitConverter.ToInt16(file, u + 0xE);
					if (slot2addr != 0)
						Array.Copy(file, BitConverter.ToInt16(file, u + 0xE), chaodata_s2, 0, 512);
					found = true;
					break;
				}
			}
			if (found)
			{
				chaoDataList.Clear();
				chaoDataList.Add(new VMS_Chao(chaodata_s1));
				VMS_Chao slot2 = new VMS_Chao(chaodata_s2);
				// Slot 2 can only contain an egg
				if (slot2.Type == ChaoTypeSA1.ChaoEgg)
					chaoDataList.Add(slot2);
				RefreshChaoList();
				listBoxDataSlots.SelectedIndex = 0;
				RefreshLabels();
				currentFilename = filename;
				toolStripStatusLabelFilename.Text = Path.GetFileName(currentFilename);
				return;
			}
			// Upload Data
			byte[] hdata = VMSFile.GetDataFromHTML(file);
			if (hdata != null)
			{
				// Looks like it's Slot 1 only
				Array.Copy(hdata, 68, chaodata_s1, 0, 512);
				chaoDataList.Clear();
				chaoDataList.Add(new VMS_Chao(chaodata_s1));
				RefreshChaoList();
				listBoxDataSlots.SelectedIndex = 0;
				RefreshLabels();
				currentFilename = filename;
				toolStripStatusLabelFilename.Text = Path.GetFileName(currentFilename);
				return;
			}
			MessageBox.Show("Chao data not found.");
		}

        private void openAVMSFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog od = new OpenFileDialog() { DefaultExt = "vms", Filter = "VMS Files|*.vms|All Files|*.*" })
                if (od.ShowDialog(this) == DialogResult.OK)
                    LoadChaoVMSFile(od.FileName);
        }

        private void trackBarHappy_ValueChanged(object sender, EventArgs e)
        {
            labelHappyValue.Text = trackBarHappy.Value.ToString();
        }

        private void trackBarRunPower_ValueChanged(object sender, EventArgs e)
        {
            labelRunPowerValue.Text = ((float)trackBarRunPower.Value / 100.0f).ToString();
        }

        private void trackBarSwimFly_ValueChanged(object sender, EventArgs e)
        {
            labelSwimFlyValue.Text = ((float)trackBarSwimFly.Value / 100.0f).ToString();
        }

        private void trackBarMagnitude_ValueChanged(object sender, EventArgs e)
        {
            labelMagnitudeValue.Text = ((float)trackBarMagnitude.Value / 100.0f).ToString();
        }

        private void ReadChaoMemories()
        {
            if (listBoxDataSlots.SelectedIndex == -1)
                return;
            VMS_Chao chaoData = chaoDataList[listBoxDataSlots.SelectedIndex];

            int page = (int)numericUpDownMemoriesPage.Value - 1;

            groupBoxMemories_0.Text = "Memory of Chao " + (page * 8).ToString();
            groupBoxMemories_1.Text = "Memory of Chao " + (page * 8 + 1).ToString();
            groupBoxMemories_2.Text = "Memory of Chao " + (page * 8 + 2).ToString();
            groupBoxMemories_3.Text = "Memory of Chao " + (page * 8 + 3).ToString();
            groupBoxMemories_4.Text = "Memory of Chao " + (page * 8 + 4).ToString();
            groupBoxMemories_5.Text = "Memory of Chao " + (page * 8 + 5).ToString();
            groupBoxMemories_6.Text = "Memory of Chao " + (page * 8 + 6).ToString();
            groupBoxMemories_7.Text = "Memory of Chao " + (page * 8 + 7).ToString();

            numericUpDownMemoriesID_0.Value = chaoData.Memories.chao[page * 8].id;
            numericUpDownMemoriesID_1.Value = chaoData.Memories.chao[page * 8 + 1].id;
            numericUpDownMemoriesID_2.Value = chaoData.Memories.chao[page * 8 + 2].id;
            numericUpDownMemoriesID_3.Value = chaoData.Memories.chao[page * 8 + 3].id;
            numericUpDownMemoriesID_4.Value = chaoData.Memories.chao[page * 8 + 4].id;
            numericUpDownMemoriesID_5.Value = chaoData.Memories.chao[page * 8 + 5].id;
            numericUpDownMemoriesID_6.Value = chaoData.Memories.chao[page * 8 + 6].id;
            numericUpDownMemoriesID_7.Value = chaoData.Memories.chao[page * 8 + 7].id;

            numericUpDownMemoriesLike_0.Value = chaoData.Memories.chao[page * 8].like;
            numericUpDownMemoriesLike_1.Value = chaoData.Memories.chao[page * 8 + 1].like;
            numericUpDownMemoriesLike_2.Value = chaoData.Memories.chao[page * 8 + 2].like;
            numericUpDownMemoriesLike_3.Value = chaoData.Memories.chao[page * 8 + 3].like;
            numericUpDownMemoriesLike_4.Value = chaoData.Memories.chao[page * 8 + 4].like;
            numericUpDownMemoriesLike_5.Value = chaoData.Memories.chao[page * 8 + 5].like;
            numericUpDownMemoriesLike_6.Value = chaoData.Memories.chao[page * 8 + 6].like;
            numericUpDownMemoriesLike_7.Value = chaoData.Memories.chao[page * 8 + 7].like;

            numericUpDownMemoriesMeet_0.Value = chaoData.Memories.chao[page * 8].meet;
            numericUpDownMemoriesMeet_1.Value = chaoData.Memories.chao[page * 8 + 1].meet;
            numericUpDownMemoriesMeet_2.Value = chaoData.Memories.chao[page * 8 + 2].meet;
            numericUpDownMemoriesMeet_3.Value = chaoData.Memories.chao[page * 8 + 3].meet;
            numericUpDownMemoriesMeet_4.Value = chaoData.Memories.chao[page * 8 + 4].meet;
            numericUpDownMemoriesMeet_5.Value = chaoData.Memories.chao[page * 8 + 5].meet;
            numericUpDownMemoriesMeet_6.Value = chaoData.Memories.chao[page * 8 + 6].meet;
            numericUpDownMemoriesMeet_7.Value = chaoData.Memories.chao[page * 8 + 7].meet;
        }

        private void WriteChaoMemories()
        {
            if (listBoxDataSlots.SelectedIndex == -1)
                return;
            VMS_Chao chaoData = chaoDataList[listBoxDataSlots.SelectedIndex];

            int page = (int)numericUpDownMemoriesPage.Value - 1;

            chaoData.Memories.chao[page * 8].id = (uint)numericUpDownMemoriesID_0.Value;
            chaoData.Memories.chao[page * 8 + 1].id = (uint)numericUpDownMemoriesID_1.Value;
            chaoData.Memories.chao[page * 8 + 2].id = (uint)numericUpDownMemoriesID_2.Value;
            chaoData.Memories.chao[page * 8 + 3].id = (uint)numericUpDownMemoriesID_3.Value;
            chaoData.Memories.chao[page * 8 + 4].id = (uint)numericUpDownMemoriesID_4.Value;
            chaoData.Memories.chao[page * 8 + 5].id = (uint)numericUpDownMemoriesID_5.Value;
            chaoData.Memories.chao[page * 8 + 6].id = (uint)numericUpDownMemoriesID_6.Value;
            chaoData.Memories.chao[page * 8 + 7].id = (uint)numericUpDownMemoriesID_7.Value;

            chaoData.Memories.chao[page * 8].like = (byte)numericUpDownMemoriesLike_0.Value;
            chaoData.Memories.chao[page * 8 + 1].like = (byte)numericUpDownMemoriesLike_1.Value;
            chaoData.Memories.chao[page * 8 + 2].like = (byte)numericUpDownMemoriesLike_2.Value;
            chaoData.Memories.chao[page * 8 + 3].like = (byte)numericUpDownMemoriesLike_3.Value;
            chaoData.Memories.chao[page * 8 + 4].like = (byte)numericUpDownMemoriesLike_4.Value;
            chaoData.Memories.chao[page * 8 + 5].like = (byte)numericUpDownMemoriesLike_5.Value;
            chaoData.Memories.chao[page * 8 + 6].like = (byte)numericUpDownMemoriesLike_6.Value;
            chaoData.Memories.chao[page * 8 + 7].like = (byte)numericUpDownMemoriesLike_7.Value;

            chaoData.Memories.chao[page * 8].meet = (byte)numericUpDownMemoriesMeet_0.Value;
            chaoData.Memories.chao[page * 8 + 1].meet = (byte)numericUpDownMemoriesMeet_1.Value;
            chaoData.Memories.chao[page * 8 + 2].meet = (byte)numericUpDownMemoriesMeet_2.Value;
            chaoData.Memories.chao[page * 8 + 3].meet = (byte)numericUpDownMemoriesMeet_3.Value;
            chaoData.Memories.chao[page * 8 + 4].meet = (byte)numericUpDownMemoriesMeet_4.Value;
            chaoData.Memories.chao[page * 8 + 5].meet = (byte)numericUpDownMemoriesMeet_5.Value;
            chaoData.Memories.chao[page * 8 + 6].meet = (byte)numericUpDownMemoriesMeet_6.Value;
            chaoData.Memories.chao[page * 8 + 7].meet = (byte)numericUpDownMemoriesMeet_7.Value;
        }

        private void numericUpDownMemoriesPage_ValueChanged(object sender, EventArgs e)
        {
            ReadChaoMemories();
        }

        private void trackBarAffection_ValueChanged(object sender, EventArgs e)
        {
            labelAffectionValue.Text = (trackBarAffection.Value / 10).ToString() + " %";
        }

        private void trackBarBondSonic_ValueChanged(object sender, EventArgs e)
        {
            labelBondSonic.Text = trackBarBondSonic.Value.ToString();
            if (Math.Abs(trackBarBondSonic.Value) > 100)
                labelBondSonic.ForeColor = Color.Red;
            else
                labelBondSonic.ForeColor = Color.Black;
        }

        private void trackBarBondTails_ValueChanged(object sender, EventArgs e)
        {
            labelBondTails.Text = trackBarBondTails.Value.ToString();
            if (Math.Abs(trackBarBondTails.Value) > 100)
                labelBondTails.ForeColor = Color.Red;
            else
                labelBondTails.ForeColor = Color.Black;
        }

        private void trackBarBondKnuckles_ValueChanged(object sender, EventArgs e)
        {
            labelBondKnuckles.Text = trackBarBondKnuckles.Value.ToString();
            if (Math.Abs(trackBarBondKnuckles.Value) > 100)
                labelBondKnuckles.ForeColor = Color.Red;
            else
                labelBondKnuckles.ForeColor = Color.Black;
        }

        private void trackBarBondAmy_ValueChanged(object sender, EventArgs e)
        {
            labelBondAmy.Text = trackBarBondAmy.Value.ToString();
            if (Math.Abs(trackBarBondAmy.Value) > 100)
                labelBondAmy.ForeColor = Color.Red;
            else
                labelBondAmy.ForeColor = Color.Black;
        }

        private void trackBarBondBig_ValueChanged(object sender, EventArgs e)
        {
            labelBondBig.Text = trackBarBondBig.Value.ToString();
            if (Math.Abs(trackBarBondBig.Value) > 100)
                labelBondBig.ForeColor = Color.Red;
            else
                labelBondBig.ForeColor = Color.Black;
        }

        private void trackBarBondGamma_ValueChanged(object sender, EventArgs e)
        {
            labelBondGamma.Text = trackBarBondGamma.Value.ToString();
            if (Math.Abs(trackBarBondGamma.Value) > 100)
                labelBondGamma.ForeColor = Color.Red;
            else
                labelBondGamma.ForeColor = Color.Black;
        }

        public static string ToStringEnums(Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return en.ToString();
        }

        private void listBoxDataSlots_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (previousChaoIndex != listBoxDataSlots.SelectedIndex)
            {
                RefreshData(previousChaoIndex);
                previousChaoIndex = listBoxDataSlots.SelectedIndex;
            }
            RefreshLabels();
        }

        private Bitmap GetAnimalPartPicture(int part)
        {
            switch (part)
            {
                case (byte)AnimalPartsSA1.Deer:
                    return Resources.icon_banb;
                case (byte)AnimalPartsSA1.Elephant:
                    return Resources.icon_zou;
                case (byte)AnimalPartsSA1.Gorilla:
                    return Resources.icon_gori;
                case (byte)AnimalPartsSA1.Koala:
                    return Resources.icon_koa;
                case (byte)AnimalPartsSA1.Lion:
                    return Resources.icon_lion;
                case (byte)AnimalPartsSA1.Mole:
                    return Resources.icon_mog;
                case (byte)AnimalPartsSA1.Otter:
                    return Resources.icon_rak;
                case (byte)AnimalPartsSA1.Parrot:
                    return Resources.icon_oum;
                case (byte)AnimalPartsSA1.Peacock:
                    return Resources.icon_kuj;
                case (byte)AnimalPartsSA1.Penguin:
                    return Resources.icon_pen;
                case (byte)AnimalPartsSA1.Rabbit:
                    return Resources.icon_usa;
                case (byte)AnimalPartsSA1.Seal:
                    return Resources.icon_goma;
                case (byte)AnimalPartsSA1.Skunk:
                    return Resources.icon_suka;
                case (byte)AnimalPartsSA1.Swallow:
                    return Resources.icon_tuba;
                case (byte)AnimalPartsSA1.Wallaby:
                    return Resources.icon_wara;
                case (byte)AnimalPartsSA1.None:
                default:
                    return new Bitmap(32, 32);
            }
        }

        private void RefreshAnimalParts(object sender, EventArgs e)
        {
            pictureBoxHeadFront.Image = GetAnimalPartPicture(comboBoxAnimalHeadFront.SelectedIndex);
            pictureBoxHeadBack.Image = GetAnimalPartPicture(comboBoxAnimalHeadBack.SelectedIndex);
            pictureBoxEars.Image = GetAnimalPartPicture(comboBoxAnimalEars.SelectedIndex);
            pictureBoxWings.Image = GetAnimalPartPicture(comboBoxAnimalWings.SelectedIndex);
            pictureBoxArms.Image = GetAnimalPartPicture(comboBoxAnimalArms.SelectedIndex);
            pictureBoxFeet.Image = GetAnimalPartPicture(comboBoxAnimalFeet.SelectedIndex);
            pictureBoxTail.Image = GetAnimalPartPicture(comboBoxAnimalTail.SelectedIndex);
        }

        private void EditorChao_FormClosing(object sender, FormClosingEventArgs e)
        {
			if (ProgramModeSelectorForm != null)
				ProgramModeSelectorForm.Show();
			else
				Application.Exit();
		}

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
			Close();
        }

        private void CreateVMI(string filename, byte[] data)
        {
            VMIFile vmi = new VMIFile();
            if (Path.GetFileNameWithoutExtension(filename).Length > 8)
                System.Windows.Forms.MessageBox.Show("For the VMI file to work correctly, the VMS filename should be 8 characters or less.", "Chao Editor Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            vmi.Description = "Chao Download";
            vmi.Copyright = "VMSEditor";
            vmi.Year = (ushort)DateTime.Now.Year;
            vmi.Month = (byte)DateTime.Now.Month;
            vmi.Day = (byte)DateTime.Now.Day;
            vmi.Hour = (byte)DateTime.Now.Hour;
            vmi.Minute = (byte)DateTime.Now.Minute;
            vmi.Second = (byte)DateTime.Now.Second;
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    vmi.Weekday = 0;
                    break;
                case DayOfWeek.Monday:
                    vmi.Weekday = 1;
                    break;
                case DayOfWeek.Tuesday:
                    vmi.Weekday = 2;
                    break;
                case DayOfWeek.Wednesday:
                    vmi.Weekday = 3;
                    break;
                case DayOfWeek.Thursday:
                    vmi.Weekday = 4;
                    break;
                case DayOfWeek.Friday:
                    vmi.Weekday = 5;
                    break;
                case DayOfWeek.Saturday:
                    vmi.Weekday = 6;
                    break;
            }
            vmi.FileID = 1;
            vmi.ResourceName = Path.GetFileNameWithoutExtension(filename);
            vmi.FileName = "SONICADV_VM";
            vmi.Size = (uint)data.Length;
            vmi.Flags |= VMIFile.VMIFlags.Game;
            File.WriteAllBytes(Path.ChangeExtension(filename, ".VMI"), vmi.GetBytes());
        }

        private void CreateVMS(string filename, ChaoSaveMode mode)
        {
            RefreshData(listBoxDataSlots.SelectedIndex);
            MemoryStream output;
            VMS_Chao chao1;
            VMS_Chao chao2;
            bool hasEgg = false;
            switch (mode)
            {
                // Download data and Chao Adventure can only store one Chao and one Egg
                case ChaoSaveMode.DownloadData:
                case ChaoSaveMode.ChaoAdventure:
                    switch (chaoDataList.Count)
                    {
                        // Nothing
                        case 0:
                            return;
                        // Just Chao
                        case 1:
                            chao1 = chaoDataList[0];
                            chao2 = new VMS_Chao(new byte[512]);
                            break;
                        // Chao + Egg
                        case 2:
                            chao1 = chaoDataList[0];
                            chao2 = chaoDataList[1];
                            hasEgg = true;
                            break;
                        // More than 2 slots
                        default:
                            using (EditorChaoSelectChao selectChao = new EditorChaoSelectChao(chaoDataList))
                            {
                                selectChao.ShowDialog();
                                if (selectChao.resultChao == -2) return;
                                chao1 = selectChao.resultChao != -1 ? chaoDataList[selectChao.resultChao] : new VMS_Chao(new byte[512]);
                                if (selectChao.resultEgg != -1)
                                {
                                    chao2 = chaoDataList[selectChao.resultEgg];
                                    hasEgg = true;
                                }
                                else
                                    chao2 = new VMS_Chao(new byte[512]);
                            }
                            break;
                    }
                    break;
                // TODO
                case ChaoSaveMode.GardenFile:
                case ChaoSaveMode.UploadData:
                default:
                    return;
            }
            if (hasEgg && chao2.Type != ChaoTypeSA1.ChaoEgg)
            {
                DialogResult save = MessageBox.Show(this, "The egg slot contains a " + ToStringEnums(chao2.Type) + "." + " Saving a Chao in this slot will cause glitches in the game.\nWould you still like to enable the second slot?", "Chao Editor Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (save == DialogResult.No)
                {
                    hasEgg = false;
                    chao2 = new VMS_Chao(new byte[512]);
                }
            }
            switch (mode)
            {
                case ChaoSaveMode.DownloadData:
                    output = new MemoryStream(4096);
                    output.Write(japaneseHeaderToolStripMenuItem.Checked ? Properties.Resources.download_jp : Properties.Resources.download_us, 0, 3072);
                    output.Seek(0xC00, SeekOrigin.Begin);
                    output.Write(chao1.GetBytes(), 0, 512);
                    output.Seek(0xE00, SeekOrigin.Begin);
                    output.Write(chao2.GetBytes(), 0, 512);
                    output.Seek(0x1EC, SeekOrigin.Begin);
                    output.Write(BitConverter.GetBytes((ushort)0xC00), 0, 2);
                    if (hasEgg)
                    {
                        output.Seek(0x1EE, SeekOrigin.Begin);
                        output.Write(BitConverter.GetBytes((ushort)0xE00), 0, 2);
                    }
                    break;
                case ChaoSaveMode.ChaoAdventure:
                    output = new MemoryStream(65536);
                    output.Write(japaneseHeaderToolStripMenuItem.Checked ? Properties.Resources.chaoadv_jp : Properties.Resources.chaoadv_us, 0, 65536);
                    output.Seek(0x3000, SeekOrigin.Begin);
                    output.Write(chao1.GetBytes(), 0, 512);
                    output.Seek(0x3200, SeekOrigin.Begin);
                    output.Write(chao2.GetBytes(), 0, 512);
                    output.Seek(0x1EC, SeekOrigin.Begin);
                    output.Write(BitConverter.GetBytes((ushort)0x3000), 0, 2);
                    if (hasEgg)
                    {
                        output.Seek(0x1EE, SeekOrigin.Begin);
                        output.Write(BitConverter.GetBytes((ushort)0x3200), 0, 2);
                    }
                    break;
                case ChaoSaveMode.GardenFile:
                case ChaoSaveMode.UploadData:
                default:
                    return;
            }
            byte[] data = output.ToArray();
            File.WriteAllBytes(filename, data);
            currentFilename = filename;
            toolStripStatusLabelFilename.Text = Path.GetFileName(currentFilename);
            if (generateAVMIFileToolStripMenuItem.Checked)
                CreateVMI(filename, data);
        }

        private void chaoDownloadDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sv = new SaveFileDialog() { FileName = "SONICADV__VM.VMS", Title = "Save VMS File", Filter = "VMS Files|*.vms|All Files|*.*", DefaultExt = "vms" })
            {
                if (sv.ShowDialog() == DialogResult.OK)
                    CreateVMS(sv.FileName, ChaoSaveMode.DownloadData);
            }
        }

        private void chaoAdventureDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sv = new SaveFileDialog() { FileName = "SONICADV__VM.VMS", Title = "Save VMS File", Filter = "VMS Files|*.vms|All Files|*.*", DefaultExt = "vms" })
            {
                if (sv.ShowDialog() == DialogResult.OK)
                    CreateVMS(sv.FileName, ChaoSaveMode.ChaoAdventure);
            }
        }

        private void checkBoxColorFlagJewel_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxJewelColor.Enabled = checkBoxColorFlagJewel.Checked;
        }

		private void numericUpDownMemoriesMeet_ValueChanged(object sender, EventArgs e)
		{
			WriteChaoMemories();
		}

		private void numericUpDownMemoriesID_0_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownMemoriesID_0.Value < 0)
				numericUpDownMemoriesID_0.Value = unchecked((uint)int.Parse(numericUpDownMemoriesID_0.Value.ToString()));
			WriteChaoMemories();
		}

		private void numericUpDownMemoriesID_1_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownMemoriesID_1.Value < 0)
				numericUpDownMemoriesID_1.Value = unchecked((uint)int.Parse(numericUpDownMemoriesID_1.Value.ToString()));
			WriteChaoMemories();
		}

		private void numericUpDownMemoriesID_2_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownMemoriesID_2.Value < 0)
				numericUpDownMemoriesID_2.Value = unchecked((uint)int.Parse(numericUpDownMemoriesID_2.Value.ToString()));
			WriteChaoMemories();
		}

		private void numericUpDownMemoriesID_3_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownMemoriesID_3.Value < 0)
				numericUpDownMemoriesID_3.Value = unchecked((uint)int.Parse(numericUpDownMemoriesID_3.Value.ToString()));
			WriteChaoMemories();
		}

		private void numericUpDownMemoriesID_4_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownMemoriesID_4.Value < 0)
				numericUpDownMemoriesID_4.Value = unchecked((uint)int.Parse(numericUpDownMemoriesID_4.Value.ToString()));
			WriteChaoMemories();
		}

		private void numericUpDownMemoriesID_5_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownMemoriesID_5.Value < 0)
				numericUpDownMemoriesID_5.Value = unchecked((uint)int.Parse(numericUpDownMemoriesID_5.Value.ToString()));
			WriteChaoMemories();
		}

		private void numericUpDownMemoriesID_6_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownMemoriesID_6.Value < 0)
				numericUpDownMemoriesID_6.Value = unchecked((uint)int.Parse(numericUpDownMemoriesID_6.Value.ToString()));
			WriteChaoMemories();
		}

		private void numericUpDownMemoriesID_7_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownMemoriesID_7.Value < 0)
				numericUpDownMemoriesID_7.Value = unchecked((uint)int.Parse(numericUpDownMemoriesID_7.Value.ToString()));
			WriteChaoMemories();
		}

		private void checkBoxColorFlag0x1_CheckedChanged(object sender, EventArgs e)
        {
            ChaoColorFlagsSA1 colorFlags = 0;
            comboBoxJewelColor.Enabled = false;
            if (checkBoxColorFlag0x1.Checked)
                colorFlags |= ChaoColorFlagsSA1.Flag1;
            if (checkBoxColorFlag0x10.Checked)
                colorFlags |= ChaoColorFlagsSA1.Flag10;
            if (checkBoxColorFlag0x20.Checked)
                colorFlags |= ChaoColorFlagsSA1.Flag20;
            if (checkBoxColorFlag0x80.Checked)
                colorFlags |= ChaoColorFlagsSA1.Flag80;
            if (checkBoxColorFlagBlack.Checked)
                colorFlags |= ChaoColorFlagsSA1.Black;
            if (checkBoxColorFlagSilver.Checked)
                colorFlags |= ChaoColorFlagsSA1.Silver;
            if (checkBoxColorFlagGold.Checked)
                colorFlags |= ChaoColorFlagsSA1.Gold;
            if (checkBoxColorFlagJewel.Checked)
            {
                colorFlags |= ChaoColorFlagsSA1.Jewel;
                comboBoxJewelColor.Enabled = true;
            }
            numericUpDown_ColorFlags.Value = (ushort)colorFlags;

        }

        private void DrawChaoEvolution()
        {
            pictureBoxEvolution.Image = new Bitmap(150, 150);
            using (Graphics gfx = Graphics.FromImage(pictureBoxEvolution.Image))
            {
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                Rectangle rect_big = new Rectangle(0, 0, 150, 150);
                gfx.DrawRectangle(new Pen(Color.Black, 1.0f), rect_big);
                gfx.FillRectangle(new SolidBrush(Color.Black), rect_big);
                Rectangle rect_smol = new Rectangle(25, 25, 100, 100);
                gfx.DrawRectangle(new Pen(new SolidBrush(Color.DimGray)), rect_smol);
                gfx.FillRectangle(new System.Drawing.SolidBrush(Color.DimGray), rect_smol);
                gfx.DrawLine(new Pen(Color.FromArgb(0, 255, 0), 2.0f), 75, 0, 75, 150);
                gfx.DrawLine(new Pen(Color.FromArgb(0, 255, 0), 2.0f), 0, 75, 150, 75);
                Color pencolor = Color.White;
                if (Math.Abs(trackBarRunPower.Value) > 100 || Math.Abs(trackBarSwimFly.Value) > 100)
                    pencolor = Color.Red;
                gfx.DrawRectangle(new Pen(pencolor, 4.0f), new Rectangle(new Point((trackBarRunPower.Value + 150) / 2 - 2, (trackBarSwimFly.Value + 150) / 2 - 2), new Size(4, 4)));
            }
        }

        private void trackBarSwimFly_Scroll(object sender, EventArgs e)
        {
            trackBarSwimFly_Click(sender, e);
            DrawChaoEvolution();
        }

        private void trackBarRunPower_Scroll(object sender, EventArgs e)
        {
            trackBarRunPower_Click(sender, e);
            DrawChaoEvolution();
        }

        private Bitmap GetFruitPreview(ChaoFruitsSA1 fruit)
        {
            switch (fruit)
            {
                case ChaoFruitsSA1.Chaonut:
                    return Properties.Resources.fruit_chaonut;
                case ChaoFruitsSA1.Cherry:
                    return Properties.Resources.fruit_cherry;
                case ChaoFruitsSA1.Coconut:
                    return Properties.Resources.fruit_coconut;
                case ChaoFruitsSA1.Grape:
                    return Properties.Resources.fruit_grape;
                case ChaoFruitsSA1.Hastenut:
                    return Properties.Resources.fruit_hastnut;
                case ChaoFruitsSA1.Lazynut:
                    return Properties.Resources.fruit_lazynut;
                case ChaoFruitsSA1.Lemon:
                    return Properties.Resources.fruit_lemon;
                case ChaoFruitsSA1.Lifenut:
                    return Properties.Resources.fruit_lifenut;
                case ChaoFruitsSA1.Plum:
                    return Properties.Resources.fruit_plum;
                case ChaoFruitsSA1.Starnut:
                    return Properties.Resources.fruit_starnut;
                case ChaoFruitsSA1.None:
                default:
                    return new Bitmap(75, 75);
            }
        }

        private void comboBoxFruit0_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBoxFruit0.Image = GetFruitPreview((ChaoFruitsSA1)comboBoxFruit0.SelectedIndex);
            pictureBoxFruit1.Image = GetFruitPreview((ChaoFruitsSA1)comboBoxFruit1.SelectedIndex);
            pictureBoxFruit2.Image = GetFruitPreview((ChaoFruitsSA1)comboBoxFruit2.SelectedIndex);
            pictureBoxFruit3.Image = GetFruitPreview((ChaoFruitsSA1)comboBoxFruit3.SelectedIndex);
            pictureBoxFruit4.Image = GetFruitPreview((ChaoFruitsSA1)comboBoxFruit4.SelectedIndex);
            pictureBoxFruit5.Image = GetFruitPreview((ChaoFruitsSA1)comboBoxFruit5.SelectedIndex);
            pictureBoxFruit6.Image = GetFruitPreview((ChaoFruitsSA1)comboBoxFruit6.SelectedIndex);
            pictureBoxFruit7.Image = GetFruitPreview((ChaoFruitsSA1)comboBoxFruit7.SelectedIndex);
        }

        private void comboBoxGarden_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((ChaoLocationSA1)comboBoxGarden.SelectedIndex)
            {
                case ChaoLocationSA1.EggCarrier:
                    pictureBoxGarden.Image = Properties.Resources.ec_panel;
                    break;
                case ChaoLocationSA1.MysticRuins:
                    pictureBoxGarden.Image = Properties.Resources.ml_panel;
                    break;
                case ChaoLocationSA1.StationSquare:
                default:
                    pictureBoxGarden.Image = Properties.Resources.ss_panel;
                    break;
            }
        }

        private int GetFaceValue(int src)
        {
            if (src >= -35)
                if (src >= 35) return 2; else return 1;
            else return 0;
        }

        byte[,,] mouse_default_num_0 = {
            { { 0x5, 0, 0x2 }, { 0x5, 0x5, 0x2 }, { 0x5, 0, 0x1 } },
            { { 0, 0x4, 0x4 }, { 0, 0, 0 }, { 0, 0x1, 0x1 } },
            { { 0, 0x4, 0x4 }, { 0, 0x2, 0x2 }, { 0x1, 0x1, 0x1 } } };

        byte[,,] eye_default_num_0 = {
            { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } },
            { { 0, 0, 0 }, { 0, 0, 0x2 }, { 0, 0, 0x2 } },
            { { 0, 0, 0 }, { 0, 0, 0x2 }, { 0, 0x2, 0x2 } }
        };

        private Bitmap GetMouthPart(int part)
        {
            switch (part)
            {
                case 0:
                default:
                    return Properties.Resources.al_kuchi00;
                case 1:
                    return Properties.Resources.al_kuchi01;
                case 2:
                    return Properties.Resources.al_kuchi02;
                case 3:
                    return Properties.Resources.al_kuchi03;
                case 4:
                    return Properties.Resources.al_kuchi04;
                case 5:
                    return Properties.Resources.al_kuchi05;
                case 6:
                    return Properties.Resources.al_kuchi06;
                case 7:
                    return Properties.Resources.al_kuchi07;
                case 8:
                    return Properties.Resources.al_kuchi08;
            }
        }

        private Bitmap ComposeMouthTexture(int m1, int m2)
        {
            Bitmap result = new Bitmap(96, 32);
            using (Graphics gfx = Graphics.FromImage(result))
            {
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                Image mouth_right = GetMouthPart(m1);
                mouth_right.RotateFlip(RotateFlipType.RotateNoneFlipX);
                gfx.DrawImage(GetMouthPart(m1), 0, 0);
                gfx.DrawImage(GetMouthPart(m2), 32, 0);
                gfx.DrawImage(mouth_right, 64, 0);
            }
            return result;
        }

        private Image GetEyeTexture(int idx)
        {
            switch (idx)
            {
                case 0:
                default:
                    return Properties.Resources.al_eye00;
                case 1:
                    return Properties.Resources.al_eye01;
                case 2:
                    return Properties.Resources.al_eye02;
                case 3:
                    return Properties.Resources.al_eye03;
                case 4:
                    return Properties.Resources.al_eye04;
                case 5:
                    return Properties.Resources.al_eye05;
                case 6:
                    return Properties.Resources.al_eye06;
                case 7:
                    return Properties.Resources.al_eye07;
            }
        }

        private Image GetMouthPreview(int index)
        {
            int a = 0;
            int b = 0;
            switch (index)
            {
                case 0:
                default:
                    break;
                case 1:
                    b = 1;
                    break;
                case 2:
                    b = 4;
                    break;
                case 3:
                    b = 5;
                    break;
                case 4:
                    a = 2;
                    b = 3;
                    break;
                case 5:
                    a = 6;
                    b = 7;
                    break;
                case 6:
                    b = 8;
                    break;
            }
            return ComposeMouthTexture(a, b);
        }

        private void CreateFacePreview()
        {
            int kindness = GetFaceValue((int)numericUpDownKindness.Value);
            int aggressive = GetFaceValue((int)numericUpDownAggressive.Value);
            int curious = GetFaceValue((int)numericUpDownCurious.Value);
            pictureBoxEyeLeft.Image = pictureBoxEyeRight.Image = GetEyeTexture(eye_default_num_0[aggressive, kindness, curious]);
            pictureBoxMouth.Image = GetMouthPreview(mouse_default_num_0[aggressive, kindness, curious]);
        }

        private void numericUpDownKindness_ValueChanged(object sender, EventArgs e)
        {
            CreateFacePreview();
        }

        private void listBoxDataSlots_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "List of all Chao in the currently open file.";
        }

        private void textBoxName_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "A Chao's name (up to 8 characters).";
        }

        private void comboBoxType_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Type of Chao.";
        }

        private void numericUpDownID_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Unique identifier used to tell whether Chao data is a copy or not.";
        }

        private void numericUpDownLifeSpan_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Counts upwards when the Chao is a baby, then counts downwards when it is an adult. When it reaches 0, the Chao dies.";
        }

        private void numericUpDownAgingFactor_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Controls how fast Lifespan decreases.";
        }

        private void numericUpDownReincarnations_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Number of times the Chao has reincarnated.";
        }

        private void numericUpDownCocoonTimer_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Unknown.";
        }

        private void numericUpDownExists_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Unknown.";
        }

        private void numericUpDownAge_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Chao age as reported by Chao Doctor.";
        }

        private void trackBarHappy_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Chao need at least 67 Happiness to reincarnate, and 91 Happiness in order to be able to evolve into a Chaos Chao.";
        }

        private void trackBarAffection_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Affection level as reported by Chao Doctor.";
        }

        private void trackBarSwimFly_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "This slider affects the Chao's evolution towards the Swim or Fly type.";
        }

        private void trackBarRunPower_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "This slider affects the Chao's evolution towards the Run or Power type.";
        }

        private void trackBarMagnitude_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "This slider affects the Chao's evolution progress.";
        }


        private void comboBoxGarden_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Location of the Chao.";
        }

        private void numericUpDownX_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Location of the Chao.";
        }

        private void numericUpDownKey1_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Unknown. Possibly used for a download integrity check.";
        }

        private void checkBoxColorFlag0x1_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Probably unused.";
        }

        private void checkBoxColorFlagBlack_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Sets the Chao color. Multiple color flags can be mixed.";
        }

        private void comboBoxJewelColor_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Sets the jewel color for Jewel Chao.";
        }

        private void numericUpDownKindness_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "The Chao's eye and mouth textures are determined by these values.";
        }

        private void comboBoxAnimalHeadFront_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Select Chao animal body parts.";
        }

        private void numericUpDownSwim_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Swim skill (0-999) multiplied by 10.";
        }

        private void numericUpDownFly_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Fly skill (0-999) multiplied by 10.";
        }

        private void numericUpDownRun_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Run skill (0-999) multiplied by 10.";
        }

        private void numericUpDownPower_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Power skill (0-999) multiplied by 10.";
        }

        private void numericUpDownPointsSwim_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "This value controls the increase of the Swim stat when Magnitude increases.";
        }

        private void numericUpDownHP_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Current amount of the Chao's health.";
        }

        private void numericUpDownMaxHP_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Total amount of the Chao's health.";
        }

        private void numericUpDownPointsFly_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "This value controls the increase of the Fly stat when Magnitude increases.";
        }

        private void numericUpDownPointsRun_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "This value controls the increase of the Run stat when Magnitude increases.";
        }

        private void numericUpDownPointsPower_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "This value controls the increase of the Power stat when Magnitude increases.";
        }

        private void trackBarMagnitude_Scroll(object sender, EventArgs e)
        {
            trackBarMagnitude_Click(sender, e);
        }

        private void numericUpDownCharm_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Unknown.";
        }

        private void numericUpDownHorny_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Controls the Chao's desire to mate.";
        }

        private void numericUpDownSleepy_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Controls how likely the Chao is to fall asleep. A sleeping Chao will wake up when this reaches 0.";
        }

        private void numericUpDownHungry_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Controls the Chao's hunger level.";
        }

        private void numericUpDownBored_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Controls how likely the Chao will use animal abilities.";
        }

        private void numericUpDownTired_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "The Chao's tiredness level, increases during Chao Race.";
        }

        private void numericUpDownStressed_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Controls how likely the Chao is to stop or trip during the race.";
        }

        private void numericUpDownNarrow_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Unknown.";
        }

        private void numericUpDownJoyful_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "When set to a high value, the Chao will show a heart emoticon.";
        }

        private void numericUpDownAngry_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Unknown. Increased by giving the Chao a Starnut or attacking the Chao if it has positive aggression.";
        }

        private void numericUpDownSad_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Controls how likely the Chao is to cry. Increased by giving the Chao a Lazynut.";
        }

        private void numericUpDownFearful_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Unknown. Increased by giving the Chao a Lazynut or by attacking the Chao if it has negative aggression.";
        }

        private void numericUpDownLonely_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Unknown.";
        }

        private void comboBoxFruit0_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "A Chao can have up to 8 Fruit in its inventory.";
        }

        private void checkBoxSealDance_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Chao can perform a shake dance.";
        }

        private void checkBoxOtterSwim_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Chao can backstroke swim.";
        }

        private void checkBoxPenguinSkate_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Chao can snuggle up to the character.";
        }

        private void checkBoxPeacockPose_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Chao can lie down on a side.";
        }

        private void checkBoxParrotSing_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Chao can sing.";
        }

        private void checkBoxSwallowTwirl_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Chao can perform the spin dance.";
        }

        private void checkBoxMoleDig_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Chao can use the shovel.";
        }

        private void checkBoxKoalaTrumpet_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Chao can play the trumpet.";
        }

        private void checkBoxSkunkDraw_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Chao can draw its favorite character.";
        }

        private void checkBoxDeerBow_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Chao can bow.";
        }

        private void checkBoxRabbitSomersault_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Chao can do a somersault.";
        }

        private void checkBoxWallabyPunch_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Chao can box.";
        }

        private void checkBoxGorillaChest_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Chao can play drums.";
        }

        private void checkBoxLionWash_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Chao can wash its face.";
        }

        private void checkBoxElephantSumo_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Chao can wrestle.";
        }

        private void numericUpDownMeetSonic_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Unknown.";
        }

        private void trackBarBondSonic_Scroll(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "When this value is high, the Chao will approach the character on its own. When it is low, the Chao will try to run away.";
        }

        private void numericUpDownRace0_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Unknown.";
        }

        private void checkBoxPearl_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Controls which jewels the Chao has won in Chao Race.";
        }

        private void numericUpDownRaceTrack_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Unknown.";
        }

        private void numericUpDownMemoriesID_0_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "Likely unused.";
        }

        private void numericUpDownMemoriesPage_Click(object sender, EventArgs e)
        {
            toolStripStatusLabelHint.Text = "There are 32 records of Chao memories, and 8 of them can be displayed at once.";
        }

        private void reportABugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start https://github.com/X-Hax/sa_tools/issues") { CreateNoWindow = true });
        }

        private void chaoEditorHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start https://github.com/X-Hax/sa_tools/wiki/Chao-Editor") { CreateNoWindow = true });
        }

		private void NumericUpDownID_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDownID.Value < 0)
				numericUpDownID.Value = unchecked((uint)int.Parse(numericUpDownID.Value.ToString()));
		}

		private void NumericUpDown_ColorFlags_ValueChanged(object sender, EventArgs e)
		{
			if (numericUpDown_ColorFlags.Value < 0)
				numericUpDown_ColorFlags.Value = unchecked((uint)int.Parse(numericUpDown_ColorFlags.Value.ToString()));
		}
	}
}