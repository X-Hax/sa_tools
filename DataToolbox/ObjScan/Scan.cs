using SAModel;
using System;
using System.Collections.Generic;
using System.IO;
using ByteConverter = SAModel.ByteConverter;

namespace SAModel.DataToolbox
{
	public partial class ObjScan
	{
		// Scan for models
		static void ScanModel(ModelFormat modelfmt)
		{
			CurrentStep++;
			CurrentScanData = "Models " + modelfmt.ToString();
			uint scan_end = (EndAddress == 0) ? EndAddress : (uint)datafile.Length - 52; // 52 for NJS_OBJECT
			ByteConverter.BigEndian = BigEndian;
			Console.WriteLine("Step {0}: Scanning for {1} models", CurrentStep, modelfmt);
			string model_extension = ".sa1mdl";
			string model_dir = "basicmodels";
			string model_type = "NJS_OBJECT";
			int count = 0;
			switch (modelfmt)
			{
				case ModelFormat.Basic:
				default:
					model_extension = ".sa1mdl";
					model_dir = "basicmodels";
					model_type = "NJS_OBJECT_OLD";
					break;
				case ModelFormat.BasicDX:
					model_extension = ".sa1mdl";
					model_dir = "basicmodels";
					model_type = "NJS_OBJECT";
					break;
				case ModelFormat.Chunk:
					model_extension = ".sa2mdl";
					model_dir = "chunkmodels";
					model_type = "NJS_CNK_OBJECT";
					break;
				case ModelFormat.GC:
					model_extension = ".sa2bmdl";
					model_dir = "gcmodels";
					model_type = "NJS_GC_OBJECT";
					break;
			}
			if (!SingleOutputFolder)
				Directory.CreateDirectory(Path.Combine(OutputFolder, model_dir));
			for (uint address = StartAddress; address < scan_end; address += 1)
			{
				if (CancelScan)
				{
					break;
				}
				if (ConsoleMode && address % 1000 == 0) 
					Console.Write("\r{0} ", address.ToString("X8"));
				CurrentAddress = address;
				string fileOutputPath = Path.Combine(OutputFolder, model_dir, address.ToString("X8"));
				if (SingleOutputFolder)
					fileOutputPath = Path.Combine(OutputFolder, address.ToString("X8"));
				try
				{
					if (!CheckModel(address, 0, modelfmt))
					{
						//Console.WriteLine("Not found: {0}", address.ToString("X"));
						continue;
					}
					//else Console.WriteLine("found: {0}", address.ToString("X"));
					NJS_OBJECT mdl = new NJS_OBJECT(datafile, (int)address, ImageBase, modelfmt, new Dictionary<int, Attach>());
					// Additional checks to prevent false positives with empty nodes
					if (CheckForModelData(mdl))
					{
						ModelFile.CreateFile(fileOutputPath + model_extension, mdl, null, null, null, null, modelfmt, NoMeta);
						count++;
						switch (modelfmt)
						{
							case ModelFormat.Basic:
							case ModelFormat.BasicDX:
								FoundBasicModels++;
								break;
							case ModelFormat.Chunk:
								FoundChunkModels++;
								break;
							case ModelFormat.GC:
								FoundGCModels++;
								break;
							default:
								break;
						}
						addresslist.Add(address, model_type);
						if (!KeepChildModels)
							DeleteChildModels(mdl, model_dir, model_extension);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("\rError adding model at {0}: {1}", address.ToString("X"), ex.Message.ToString());
					continue;
				}
			}
			Console.WriteLine("\r{0} models found", count);
		}

		// Scan for Motions
		static void ScanMotions()
		{
			CurrentStep++;
			CurrentScanData = "Motions" + (ModelParts > 0 ? " with " + ModelParts.ToString() + " or more nodes" : "");
			Console.WriteLine("Step {0}: Scanning for motions with at least {1} model parts... ", CurrentStep, ModelParts);
			if (ShortRot) 
				Console.WriteLine("Using short rotations");
			ByteConverter.BigEndian = BigEndian;
			if (!SingleOutputFolder)
				Directory.CreateDirectory(Path.Combine(OutputFolder, "actions"));
			for (uint address = StartAddress; address < EndAddress; address += 1)
			{
				if (CancelScan)
				{
					break;
				}
				if (ConsoleMode && address % 1000 == 0) 
					Console.Write("\r{0} ", address.ToString("X8"));
				CurrentAddress = address;
				// Check for a valid MDATA pointer
				uint mdatap = ByteConverter.ToUInt32(datafile, (int)address);
				if (mdatap < ImageBase || mdatap >= datafile.Length - 12 + ImageBase || mdatap == 0)
				{
					//Console.WriteLine("Mdatap {0} fail", mdatap.ToString("X8"));
					continue;
				}
				uint frames = ByteConverter.ToUInt32(datafile, (int)address + 4);
				if (frames > 2000 || frames < 1)
				{
					//Console.WriteLine("Frames {0} fail", frames.ToString());
					continue;
				}
				AnimFlags animtype = (AnimFlags)ByteConverter.ToUInt16(datafile, (int)address + 8);
				if (animtype == 0) continue;
				int mdata = 0;
				//Console.WriteLine("Flags: {0}", animtype.ToString());
				if (animtype.HasFlag(AnimFlags.Position)) mdata++;
				if (animtype.HasFlag(AnimFlags.Rotation)) mdata++;
				if (animtype.HasFlag(AnimFlags.Scale)) mdata++;
				if (animtype.HasFlag(AnimFlags.Vector)) mdata++;
				if (animtype.HasFlag(AnimFlags.Vertex)) mdata++;
				if (animtype.HasFlag(AnimFlags.Normal)) mdata++;
				if (animtype.HasFlag(AnimFlags.Color)) continue;
				if (animtype.HasFlag(AnimFlags.Intensity)) continue;
				if (animtype.HasFlag(AnimFlags.Target)) continue;
				if (animtype.HasFlag(AnimFlags.Spot)) continue;
				if (animtype.HasFlag(AnimFlags.Point)) continue;
				if (animtype.HasFlag(AnimFlags.Roll)) continue;
				int mdatasize = 0;
				bool lost = false;
				switch (mdata)
				{
					case 1:
					case 2:
						mdatasize = 16;
						break;
					case 3:
						mdatasize = 24;
						break;
					case 4:
						mdatasize = 32;
						break;
					case 5:
						mdatasize = 40;
						break;
					default:
						lost = true;
						break;
				}
				if (lost) continue;
				// Check MKEY pointers
				int mdatas = 0;
				for (int u = 0; u < 255; u++)
				{
					for (int m = 0; m < mdata; m++)
					{
						if (lost) continue;
						uint pointer = ByteConverter.ToUInt32(datafile, (int)(mdatap - ImageBase) + mdatasize * u + 4 * m);
						if (pointer < ImageBase || pointer >= datafile.Length - 8 + ImageBase)
						{
							if (pointer != 0)
							{
								lost = true;
								//Console.WriteLine("Mkey pointer {0} lost", pointer.ToString("X8"));
							}
						}
						if (!lost)
						{
							// Read frame count
							int framecount = ByteConverter.ToInt32(datafile, (int)(mdatap - ImageBase) + mdatasize * u + 4 * mdata + 4 * m);
							if (framecount < 0 || framecount > 2000)
							{
								//Console.WriteLine("Framecount lost: {0}", framecount.ToString("X8"));
								lost = true;
							}
							if (pointer == 0 && framecount != 0)
							{
								//Console.WriteLine("Framecount non zero");
								lost = true;
							}
							//if (!lost) Console.WriteLine("Mdata size: {0}, MkeyP: {1}, Frames: {2}", mdatasize, pointer.ToString("X8"), framecount.ToString());
						}
					}
					if (!lost)
					{
						mdatas++;
						//Console.WriteLine("Mdata {0}, total {1}", u, mdatas);
					}
				}
				if (mdatas > 0 && mdatas >= ModelParts)
				{
					try
					{
						Console.WriteLine("\rAdding motion at {0}: {1} nodes", address.ToString("X8"), mdatas);
						//Console.WriteLine("trying Address: {0}, MdataP: {1}, mdatas: {2}", address.ToString("X8"), mdatap.ToString("X8"), mdata);
						NJS_MOTION mot = NJS_MOTION.ReadDirect(datafile, mdatas, (int)address, ImageBase, new Dictionary<int, Attach>(), ShortRot);
						if (mot.ModelParts <= 0) continue;
						if (mot.Frames <= 0) continue;
						if (mot.Models.Count == 0) continue;
						string fileOutputPath = Path.Combine(OutputFolder, "actions", address.ToString("X8") + ".saanim");
						if (SingleOutputFolder)
							fileOutputPath = Path.Combine(OutputFolder, address.ToString("X8") + ".saanim");
						mot.Save(fileOutputPath, NoMeta);
						FoundMotions++;
						addresslist.Add(address, "NJS_MOTION");
						uint[] arr = new uint[2];
						arr[0] = address;
						arr[1] = (uint)mot.ModelParts;
						actionlist.Add(address, arr);
					}
					catch (Exception ex)
					{
						Console.WriteLine("\rError adding motion at {0}: {1}", address.ToString("X8"), ex.Message);
					}
				}
			}
			Console.WriteLine("\rFound {0} motions", FoundMotions);
		}

		// Scan for Actions in a specific range
		static int ScanActions(uint addr, uint nummdl, ModelFormat modelfmt)
		{
			int count = 0;
			ByteConverter.BigEndian = BigEndian;
			if (nummdl == 0)
				return 0;
			for (uint address = addr; address < datafile.Length - 8; address += 1)
			{
				CurrentAddress = address;
				if (CancelScan)
				{
					break;
				}
				if (ByteConverter.ToUInt32(datafile, (int)address) != addr + ImageBase) 
					continue;
				uint motaddr = ByteConverter.ToUInt32(datafile, (int)address + 4);
				if (motaddr < ImageBase) 
					continue;
				try
				{
					NJS_MOTION mot = new NJS_MOTION(datafile, (int)(motaddr - ImageBase), ImageBase, (int)nummdl, null, false);
					if (mot.Models.Count == 0) 
						continue;
					addresslist.Add(motaddr - ImageBase, "NJS_MOTION");
					Console.WriteLine("\rMotion found for model {0} at address {1}", addr.ToString("X8"), (motaddr - ImageBase).ToString("X"));
					string fileOutputPath = Path.Combine(OutputFolder, "actions", (motaddr - ImageBase).ToString("X8"));
					if (SingleOutputFolder)
						fileOutputPath = Path.Combine(OutputFolder, (motaddr - ImageBase).ToString("X8"));
					mot.Save(fileOutputPath + ".saanim", NoMeta);
					uint[] arr = new uint[2];
					arr[0] = addr;
					arr[1] = nummdl;
					actionlist.Add(motaddr - ImageBase, arr);
					AddAction(addr, motaddr - ImageBase);
					address += 7;
					count++;
				}
				catch (Exception)
				{
					continue;
				}
			}
			return count;
		}

		// Scan for Actions for the models that have been found
		static void ScanAnimations(ModelFormat modelfmt)
		{
			CurrentStep++;
			CurrentScanData = "Source data for Actions";
			ByteConverter.BigEndian = BigEndian;
			string modelstring = "basicmodels";
			string modelext = ".sa1mdl";
			List<uint> modeladdr = new List<uint>();
			if (addresslist.Count == 0) 
				return;
			if (!SingleOutputFolder)
				Directory.CreateDirectory(Path.Combine(OutputFolder, "actions"));
			switch (modelfmt)
			{
				case ModelFormat.Basic:
				case ModelFormat.BasicDX:
					modelstring = "basicmodels";
					modelext = ".sa1mdl";
					break;
				case ModelFormat.Chunk:
					modelstring = "chunkmodels";
					modelext = ".sa2mdl";
					break;
				case ModelFormat.GC:
					modelstring = "gcmodels";
					modelext = ".sa2bmdl";
					break;
			}
			foreach (var entry in addresslist)
			{
				switch (entry.Value)
				{
					case "NJS_OBJECT":
					case "NJS_OBJECT_OLD":
					case "NJS_CNK_OBJECT":
					case "NJS_GC_OBJECT":
						modeladdr.Add(entry.Key);
						break;
					default:
						break;
				}
			}
			foreach (uint maddr in modeladdr)
			{
				if (CancelScan)
				{
					break;
				}
				string modelpath;
				if (!SingleOutputFolder)
					modelpath = Path.Combine(OutputFolder, modelstring, maddr.ToString("X8") + modelext);
				else
					modelpath = Path.Combine(OutputFolder, maddr.ToString("X8") + modelext);
				if (File.Exists(modelpath))
				{
					try
					{
						if (ConsoleMode)
							Console.Write("\rScanning for actions (model {0} of {1})", modeladdr.IndexOf(maddr) + 1, modeladdr.Count);
						CurrentScanData = "Actions (model " + (modeladdr.IndexOf(maddr) + 1).ToString() + " of " + modeladdr.Count.ToString()+ ")";
						ModelFile mdlfile = new ModelFile(modelpath);
						FoundActions += ScanActions(maddr, (uint)mdlfile.Model.CountAnimated(), modelfmt);
					}
					catch (Exception ex)
					{
						Console.WriteLine("\rError adding actions for model at {0}: {1}", maddr.ToString("X"), ex.Message.ToString());
					}
				}
			}
			Console.WriteLine("\n{0} animations found", FoundActions);
		}

		// Scan for Landtables
		static void ScanLandtable(LandTableFormat landfmt)
		{
			CurrentStep++;
			CurrentScanData = "Landtables " + landfmt.ToString();
			ByteConverter.BigEndian = BigEndian;
			Console.WriteLine("Step {0}: Scanning for {1} landtables", CurrentStep, landfmt.ToString());
			string landtable_extension = ".sa1lvl";
			int count = 0;
			switch (landfmt)
			{
				case LandTableFormat.SA1:
					landtable_extension = ".sa1lvl";
					break;
				case LandTableFormat.SADX:
				default:
					landtable_extension = ".sa1lvl";
					break;
				case LandTableFormat.SA2:
					landtable_extension = ".sa2lvl";
					break;
				case LandTableFormat.SA2B:
					landtable_extension = ".sa2blvl";
					break;
			}
			if (!SingleOutputFolder)
				Directory.CreateDirectory(Path.Combine(OutputFolder, "levels"));
			for (uint address = StartAddress; address < EndAddress; address += 1)
			{
				if (CancelScan)
				{
					break;
				}
				if (ConsoleMode && address % 1000 == 0) 
					Console.Write("\r{0} ", address.ToString("X8"));
				CurrentAddress = address;
				string fileOutputPath = Path.Combine(OutputFolder, "levels", address.ToString("X8"));
				if (SingleOutputFolder)
					fileOutputPath = Path.Combine(OutputFolder, address.ToString("X8"));
				if (!CheckLandTable(address, landfmt)) continue;
				try
				{
					//Console.WriteLine("Try {0}", address.ToString("X"));
					LandTable land = new LandTable(datafile, (int)address, ImageBase, landfmt);
					if (land.COL.Count > 3)
					{
						land.SaveToFile(fileOutputPath + landtable_extension, landfmt, NoMeta);
						count++;
						switch (landfmt)
						{
							case LandTableFormat.SA1:
								FoundSA1Landtables++;
								break;
							case LandTableFormat.SADX:
							default:
								FoundSADXLandtables++;
								break;
							case LandTableFormat.SA2:
								FoundSA2Landtables++;
								break;
							case LandTableFormat.SA2B:
								FoundSA2BLandtables++;
								break;
						}
						landtablelist.Add(address);
						Console.WriteLine("\rLandtable {0} at {1}", landfmt.ToString(), address.ToString("X8"));
						addresslist.Add(address, "landtable_" + landfmt.ToString());
						address += (uint)LandTable.Size(landfmt) - 1;
					}
				}
				catch (Exception)
				{
					continue;
				}
			}
			Console.WriteLine("\r{0} landtables found", count);
		}

		static void AddAction(uint objectaddr, uint motionaddr)
		{
			string strpath = Path.Combine(OutputFolder, "basicmodels", objectaddr.ToString("X8") + ".action");
			if (SingleOutputFolder)
				strpath = Path.Combine(OutputFolder, objectaddr.ToString("X8") + ".action");
			using (FileStream str = new FileStream(strpath, FileMode.Append, FileAccess.Write))
			using (StreamWriter tw = new StreamWriter(str))
			{
				if (!SingleOutputFolder)
					tw.WriteLine("../actions/" + motionaddr.ToString("X8") + ".saanim");
				else
					tw.WriteLine(motionaddr.ToString("X8") + ".saanim");
				tw.Flush();
				tw.Close();
			}
		}
	}
}
