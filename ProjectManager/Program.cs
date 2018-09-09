using System;
using System.Windows.Forms;
using SonicRetro.SAModel.SAEditorCommon;

namespace ProjectManager
{
	static class Program
	{
		private static ProjectManagerSettings settings;
		public static ProjectManagerSettings Settings { get { return settings; } } 

		private static void PrintHelp()
		{
			Console.WriteLine("Argument #1: Path to file to be split apart into data chunks.");
			Console.WriteLine("Argument #2: Path to data mapping file. This is usually an INI file.");
			Console.WriteLine("Argument #3: Project Directory. All files will be output to this directory.");
			//Console.WriteLine("Argument #3 must be an absolute path.");
		}

		public static bool AnyGamesConfigured()
		{
			// sadx validity check
			string sadxpcfailreason = "";
			bool sadxPCIsValid = GamePathChecker.CheckSADXPCValid(settings.SADXPCPath, out sadxpcfailreason);

			// sa2 valididty check next
			string sa2pcFailReason = "";
			bool sa2PCIsValid = GamePathChecker.CheckSA2PCValid(settings.SA2PCPath, out sa2pcFailReason);

			return sadxPCIsValid || sa2PCIsValid;
		}

		[STAThread]
		static int Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			ProjectManager projectSelect;

			//Properties.Settings.Default.Upgrade();
			settings = ProjectManagerSettings.Load();

			if(args != null && args.Length > 0  && args[0] == "build")
			{
				// not yet implemented because not enough info to know which game is being targeted
				// args[1] should be mod name
				// args[2] should be game
				throw new System.NotImplementedException();
				
			}
			else
			{
				// first check to see if we're configured properly.
				if(!AnyGamesConfigured())
				{
					GameConfig gameConfig = new GameConfig();
					DialogResult configResult = gameConfig.ShowDialog();

					if (configResult == DialogResult.Abort) return (int)SA_Tools.Split.SplitERRORVALUE.InvalidConfig;
					gameConfig.Dispose();
				}

				// todo: catch unhandled exceptions
				projectSelect = new ProjectManager();
				Application.ThreadException += Application_ThreadException;
				Application.Run(projectSelect);
			}

			#region Split Old Code
			//#region Getting Input Arguments
			//string datafilename, inifilename, projectFolderName;
			//if (args.Length > 0)
			//{
			//	datafilename = args[0];
			//	Console.WriteLine("File: {0}", datafilename);
			//}
			//else
			//{
			//	Console.WriteLine("No source file supplied. Aborting.");
			//	PrintHelp();
			//	Console.WriteLine("Press any key to exit.");
			//	Console.ReadLine();
			//	return (int)ERRORVALUE.NoSourceFile;
			//}
			//if (args.Length > 1)
			//{
			//	inifilename = args[1];
			//	Console.WriteLine("INI File: {0}", inifilename);
			//}
			//else
			//{
			//	Console.WriteLine("No data mapping file supplied (expected ini). Aborting.");
			//	Console.WriteLine("Press any key to exit.");
			//	Console.ReadLine();
			//	return (int)ERRORVALUE.NoDataMapping;
			//}
			//if (args.Length > 2)
			//{
			//	projectFolderName = args[2];
			//	Console.WriteLine("Project Folder: {0}", projectFolderName);
			//}
			//else
			//{
			//	Console.WriteLine("No project folder supplied. Aborting.");
			//	Console.WriteLine("Press any key to exit.");
			//	Console.ReadLine();
			//	return (int)ERRORVALUE.NoProject;
			//}
			//#endregion
			#endregion

			#region SplitMDL Old Command Line Ingestion Code
			//		string dir = Environment.CurrentDirectory;
			//		try
			//		{
			//			// Bigh endian check
			//			Queue<string> argq = new Queue<string>(args);
			//			if (argq.Count > 0 && argq.Peek().Equals("/be", StringComparison.OrdinalIgnoreCase))
			//			{
			//				ByteConverter.BigEndian = true;
			//				argq.Dequeue();
			//			}

			//			string outputFolder = "";

			//			if(argq.Count > 0)
			//			{
			//				string outputCandidate = argq.Peek();

			//				string[] outputCandidateSplit = outputCandidate.Split('=');
			//				if(outputCandidateSplit.Length == 2 && outputCandidateSplit[0].Equals("output"))
			//				{
			//					outputFolder = outputCandidateSplit[1];

			//					argq.Dequeue();
			//				}					
			//			}

			//			// get file name, read it from the console if nothing
			//			string mdlfilename;
			//			if (argq.Count > 0)
			//			{
			//				mdlfilename = argq.Dequeue();
			//				Console.WriteLine("File: {0}", mdlfilename);
			//			}
			//			else
			//			{
			//				Console.Write("File: ");
			//				mdlfilename = Console.ReadLine();
			//			}
			//			mdlfilename = Path.GetFullPath(mdlfilename);

			//			// look through the argumetns for animationfiles
			//			string[] anifilenames = new string[argq.Count];
			//			for (int j = 0; j < anifilenames.Length; j++)
			//			{
			//				Console.WriteLine("Animations: {0}", argq.Peek());
			//				anifilenames[j] = Path.GetFullPath(argq.Dequeue());
			//			}

			//			// load model file
			//			Environment.CurrentDirectory = (outputFolder.Length != 0) ? outputFolder : Path.GetDirectoryName(mdlfilename);
			//			byte[] mdlfile = File.ReadAllBytes(mdlfilename);
			//			if (Path.GetExtension(mdlfilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
			//				mdlfile = FraGag.Compression.Prs.Decompress(mdlfile);
			//			Directory.CreateDirectory(Path.GetFileNameWithoutExtension(mdlfilename));

			//			// getting model pointers
			//			int address = 0;
			//			int i = ByteConverter.ToInt32(mdlfile, address);
			//SortedDictionary<int, int> modeladdrs = new SortedDictionary<int, int>();
			//while (i != -1)
			//{
			//	modeladdrs[i] = ByteConverter.ToInt32(mdlfile, address + 4);
			//	address += 8;
			//	i = ByteConverter.ToInt32(mdlfile, address);
			//}

			//			// load models from pointer list
			//Dictionary<int, NJS_OBJECT> models = new Dictionary<int, NJS_OBJECT>();
			//Dictionary<int, string> modelnames = new Dictionary<int, string>();
			//List<string> partnames = new List<string>();
			//			foreach (KeyValuePair<int, int> item in modeladdrs)
			//			{
			//	NJS_OBJECT obj = new NJS_OBJECT(mdlfile, item.Value, 0, ModelFormat.Chunk);
			//	modelnames[item.Key] = obj.Name;
			//	if (!partnames.Contains(obj.Name))
			//	{
			//		List<string> names = new List<string>(obj.GetObjects().Select((o) => o.Name));
			//		foreach (int idx in modelnames.Where(a => names.Contains(a.Value)).Select(a => a.Key))
			//			models.Remove(idx);
			//		models[item.Key] = obj;
			//		partnames.AddRange(names);
			//	}
			//			}

			//			// load animations
			//			Dictionary<int, string> animfns = new Dictionary<int, string>();
			//			Dictionary<int, Animation> anims = new Dictionary<int, Animation>();
			//			foreach (string anifilename in anifilenames)
			//			{
			//				byte[] anifile = File.ReadAllBytes(anifilename);
			//				if (Path.GetExtension(anifilename).Equals(".prs", StringComparison.OrdinalIgnoreCase))
			//					anifile = FraGag.Compression.Prs.Decompress(anifile);
			//				Directory.CreateDirectory(Path.GetFileNameWithoutExtension(anifilename));
			//				address = 0;
			//				i = ByteConverter.ToInt16(anifile, address);
			//				while (i != -1)
			//				{
			//					anims[i] = new Animation(anifile, ByteConverter.ToInt32(anifile, address + 4), 0, ByteConverter.ToInt16(anifile, address + 2));
			//					animfns[i] = Path.Combine(Path.GetFileNameWithoutExtension(anifilename), i.ToString(NumberFormatInfo.InvariantInfo) + ".saanim");
			//					address += 8;
			//					i = ByteConverter.ToInt16(anifile, address);
			//				}
			//			}

			//			// save output model files
			//			foreach (KeyValuePair<int, NJS_OBJECT> model in models)
			//			{
			//				List<string> animlist = new List<string>();
			//				foreach (KeyValuePair<int, Animation> anim in anims)
			//					if (model.Value.CountAnimated() == anim.Value.ModelParts)
			//						animlist.Add("../" + animfns[anim.Key]);
			//				ModelFile.CreateFile(Path.Combine(Path.GetFileNameWithoutExtension(mdlfilename),
			//		model.Key.ToString(NumberFormatInfo.InvariantInfo) + ".sa2mdl"), model.Value, animlist.ToArray(),
			//		null, null, null, "splitMDL", null, ModelFormat.Chunk);
			//			}

			//			// save ini file
			//IniSerializer.Serialize(modelnames, new IniCollectionSettings(IniCollectionMode.IndexOnly),
			//	Path.Combine(Path.GetFileNameWithoutExtension(mdlfilename), Path.GetFileNameWithoutExtension(mdlfilename) + ".ini"));
			//			foreach (KeyValuePair<int, Animation> anim in anims)
			//				anim.Value.Save(animfns[anim.Key]);
			//		}
			//		finally
			//		{
			//			Environment.CurrentDirectory = dir;
			//		}
			#endregion

			return 0;
		}

		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			string error = string.Format("{0}\n{1}", e.Exception.Message, e.Exception.TargetSite);
			MessageBox.Show(error, "Unhandled exception");

				//if (primaryForm != null)
				//	using (ErrorDialog ed = new ErrorDialog((Exception)e.ExceptionObject, false))
				//		ed.ShowDialog(primaryForm);
				//else
				//{
				//	System.IO.File.WriteAllText("SADXLVL2.log", e.ExceptionObject.ToString());
				//	MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", "SADXLVL2 Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				//}
		}
	}
}