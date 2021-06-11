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

        public EditorChao()
        {
            InitializeComponent();
            if (Program.args.Length > 0)
                if (File.Exists(Program.args[0]))
                    LoadChaoVMSFile(Program.args[0]);
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
            trackBarBondBig.Value = chaoData.Memories.player[4].like;
            numericUpDownMeetBig.Value = chaoData.Memories.player[4].meet;
            trackBarBondGamma.Value = chaoData.Memories.player[5].like;
            numericUpDownMeetGamma.Value = chaoData.Memories.player[5].meet;
            // Chao memories
            RefreshChaoMemories();
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
        }

        private void RefreshData()
        {
            if (listBoxDataSlots.SelectedIndex == -1)
                return;
            VMS_Chao chaoData = chaoDataList[listBoxDataSlots.SelectedIndex];
            chaoData.Name = textBoxName.Text;
            chaoData.Type = (ChaoTypeSA1)comboBoxType.SelectedIndex;
            chaoData.Garden = (ChaoLocationSA1)comboBoxGarden.SelectedIndex;
            chaoData.Happiness = (sbyte)trackBarHappy.Value;
            chaoData.Key1 = (byte)numericUpDownKey1.Value;
            chaoData.Key2 = (byte)numericUpDownKey2.Value;
            chaoData.Key3 = (byte)numericUpDownKey3.Value;
            chaoData.Key4 = (byte)numericUpDownKey4.Value;
            //TODO here
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
            trackBarBondBig.Value = chaoData.Memories.player[4].like;
            numericUpDownMeetBig.Value = chaoData.Memories.player[4].meet;
            trackBarBondGamma.Value = chaoData.Memories.player[5].like;
            numericUpDownMeetGamma.Value = chaoData.Memories.player[5].meet;
            // Chao memories
            RefreshChaoMemories();
            // Other stuff
            numericUpDownReincarnations.Value = chaoData.Reincarnations;
            numericUpDownRaceTrack.Value = chaoData.Lane;
            numericUpDownExists.Value = chaoData.Exists;
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
                return;
            }
            // Upload Data
            for (int u = 0; u < file.Length - 15; u++)
            {
                if (System.Text.Encoding.GetEncoding(932).GetString(file, u, 15) == "NAME=\"chaodata\"")
                {
                    // Load Base64
                    StringBuilder b64str = new StringBuilder();
                    for (int k = u + 23; k < file.Length - 2; k++)
                    {
                        if (file[k] == 0x22 && file[k + 1] == 0x3E)
                            break;
                        else
                            b64str.Append((Convert.ToChar(file[k])).ToString());
                    }
                    // Decode Base64, decrypt
                    byte[] data = Convert.FromBase64String(b64str.ToString());
                    VMSFile.DecryptData(ref data);
                    // Looks like it's Slot 1 only
                    Array.Copy(data, 68, chaodata_s1, 0, 512);
                    chaoDataList.Clear();
                    chaoDataList.Add(new VMS_Chao(chaodata_s1));
                    RefreshChaoList();
                    listBoxDataSlots.SelectedIndex = 0;
                    RefreshLabels();
                    return;
                }
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

        private void RefreshChaoMemories()
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

        private void numericUpDownMemoriesPage_ValueChanged(object sender, EventArgs e)
        {
            RefreshChaoMemories();
        }

        private void trackBarAffection_ValueChanged(object sender, EventArgs e)
        {
            labelAffectionValue.Text = trackBarAffection.Value.ToString();
        }

        private void trackBarBondSonic_ValueChanged(object sender, EventArgs e)
        {
            labelBondSonic.Text = trackBarBondSonic.Value.ToString();
        }

        private void trackBarBondTails_ValueChanged(object sender, EventArgs e)
        {
            labelBondTails.Text = trackBarBondTails.Value.ToString();
        }

        private void trackBarBondKnuckles_ValueChanged(object sender, EventArgs e)
        {
            labelBondKnuckles.Text = trackBarBondKnuckles.Value.ToString();
        }

        private void trackBarBondAmy_ValueChanged(object sender, EventArgs e)
        {
            labelBondAmy.Text = trackBarBondAmy.Value.ToString();
        }

        private void trackBarBondBig_ValueChanged(object sender, EventArgs e)
        {
            labelBondBig.Text = trackBarBondBig.Value.ToString();
        }

        private void trackBarBondGamma_ValueChanged(object sender, EventArgs e)
        {
            labelBondGamma.Text = trackBarBondGamma.Value.ToString();
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
            Application.Exit();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CreateVMI(string filename, byte[] data)
        {
            VMIFile vmi = new VMIFile();
            if (Path.GetFileName(filename).Length > 8)
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
            vmi.ResourceName = vmi.FileName = Path.GetFileNameWithoutExtension(filename);
            vmi.Size = (uint)data.Length;
            vmi.Flags |= VMIFile.VMIFlags.Game;
            File.WriteAllBytes(Path.ChangeExtension(filename, ".VMI"), vmi.GetBytes());
        }

        private void CreateVMS(string filename, ChaoSaveMode mode)
        {
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
            if (generateAVMIFileToolStripMenuItem.Checked)
                CreateVMI(Path.GetFileNameWithoutExtension(filename), data);
        }
    
        private void chaoDownloadDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sv = new SaveFileDialog() { FileName = Path.ChangeExtension(Path.GetFileName(currentFilename), "vms"), Title = "Save VMS File", Filter = "VMS Files|*.vms|All Files|*.*", DefaultExt = "vms" })
            {
                if (sv.ShowDialog() == DialogResult.OK)
                    CreateVMS(sv.FileName, ChaoSaveMode.DownloadData);
            }
        }

        private void chaoAdventureDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sv = new SaveFileDialog() { FileName = Path.ChangeExtension(Path.GetFileName(currentFilename), "vms"), Title = "Save VMS File", Filter = "VMS Files|*.vms|All Files|*.*", DefaultExt = "vms" })
            {
                if (sv.ShowDialog() == DialogResult.OK)
                    CreateVMS(sv.FileName, ChaoSaveMode.ChaoAdventure);
            }
        }

        private void checkBoxColorFlagJewel_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxJewelColor.Enabled = checkBoxColorFlagJewel.Checked;
        }
    }
}