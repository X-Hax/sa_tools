using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IniFile;
using SonicRetro.SAModel;
using ModelObject = SonicRetro.SAModel.Object;

namespace replaceMDL
{
	static class Program
	{
		static readonly IniCollectionSettings inisettings = new IniCollectionSettings(IniCollectionMode.IndexOnly);

		static void Main(string[] args)
		{
			Queue<string> argq = new Queue<string>(args);
			string inifilename;
			if (argq.Count > 0)
			{
				inifilename = argq.Dequeue();
				Console.WriteLine("INI File: {0}", inifilename);
			}
			else
			{
				Console.Write("INI File: ");
				inifilename = Console.ReadLine();
			}
			Dictionary<int, string> modelnames = IniSerializer.Deserialize<Dictionary<int, string>>(inifilename, inisettings);
			string mdlfilename;
			if (argq.Count > 0)
			{
				mdlfilename = argq.Dequeue();
				Console.WriteLine("Model File: {0}", mdlfilename);
			}
			else
			{
				Console.Write("Model File: ");
				mdlfilename = Console.ReadLine();
			}
			ModelFile model = new ModelFile(mdlfilename);
			ModelObject[] objects = model.Model.GetObjects();
			string repmdlfilename;
			if (argq.Count > 0)
			{
				repmdlfilename = argq.Dequeue();
				Console.WriteLine("Replacement Model File: {0}", repmdlfilename);
			}
			else
			{
				Console.Write("Replacement Model File: ");
				repmdlfilename = Console.ReadLine();
			}
			ModelFile repmodel = new ModelFile(repmdlfilename);
			ModelObject[] repobjects = repmodel.Model.GetObjects();
			if (objects.Length != repobjects.Length)
				Console.WriteLine("Models have different structures, the game may crash.");
			foreach (KeyValuePair<int, string> item in modelnames.ToList())
				if (objects.Any((obj) => obj.Name == item.Value))
					modelnames[item.Key] = repobjects[Array.IndexOf(objects, objects.Single((o) => o.Name == item.Value))].Name;
			ModelFile.CreateFile(mdlfilename, repmodel.Model, null, null, repmodel.Author, repmodel.Description, "replaceMDL", repmodel.Metadata, repmodel.Format);
			IniSerializer.Serialize(modelnames, inisettings, inifilename);
		}
	}
}