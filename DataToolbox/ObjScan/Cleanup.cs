using SAModel;
using System;
using System.Globalization;
using System.IO;

namespace SAModel.DataToolbox
{
	public partial class ObjScan
	{
		static void DeleteChildModels(NJS_OBJECT mdl, string model_dir, string model_extension)
		{
			if (mdl.Children != null && mdl.Children.Count > 0)
			{
				foreach (NJS_OBJECT child in mdl.Children)
				{
					uint itemaddr = uint.Parse(child.Name.Substring(7, child.Name.Length - 7), NumberStyles.AllowHexSpecifier);
					string cpath = Path.Combine(OutputFolder, model_dir, itemaddr.ToString("X8") + model_extension);
					if (SingleOutputFolder)
						cpath = Path.Combine(OutputFolder, itemaddr.ToString("X8") + model_extension);
					Console.WriteLine("Deleting child object {0}", cpath);
					File.Delete(cpath);
					if (addresslist.ContainsKey(itemaddr))
						addresslist.Remove(itemaddr);
                    DeleteChildModels(child, model_dir, model_extension);
				}
			}
			if (mdl.Sibling != null)
			{
				uint itemaddr = uint.Parse(mdl.Sibling.Name.Substring(7, mdl.Sibling.Name.Length - 7), NumberStyles.AllowHexSpecifier);
				string spath = Path.Combine(OutputFolder, model_dir, itemaddr.ToString("X8") + model_extension);
				if (SingleOutputFolder)
					spath = Path.Combine(OutputFolder, itemaddr.ToString("X8") + model_extension);
				Console.WriteLine("Deleting sibling object {0}", spath);
				File.Delete(spath);
				if (addresslist.ContainsKey(itemaddr))
					addresslist.Remove(itemaddr);
                DeleteChildModels(mdl.Sibling, model_dir, model_extension);
			}
		}

        static void CleanUpLandtable()
        {
            bool delete_basic = false;
            bool delete_chunk = false;
            bool delete_gc = false;
            ByteConverter.BigEndian = BigEndian;
            string model_extension = ".sa1mdl";
            string model_dir = "basicmodels";
            LandTableFormat landfmt = LandTableFormat.SA1;
            foreach (uint landaddr in landtablelist)
            {
                switch (addresslist[landaddr])
                {
                    case "landtable_SA1":
                        landfmt = LandTableFormat.SA1;
                        delete_basic = true;
                        break;
                    case "landtable_SADX":
                        landfmt = LandTableFormat.SADX;
                        delete_basic = true;
                        break;
                    case "landtable_SA2":
                        landfmt = LandTableFormat.SA2;
                        delete_basic = true;
                        delete_chunk = true;
                        break;
                    case "landtable_SA2B":
                        landfmt = LandTableFormat.SA2B;
                        delete_basic = true;
                        delete_gc = true;
                        delete_chunk = true;
                        break;
                }
                //Console.WriteLine("Landtable {0}, {1}, {2}", landaddr.ToString("X"), ImageBase.ToString("X"), landfmt.ToString());
                LandTable land = new LandTable(datafile, (int)landaddr, ImageBase, landfmt);
                string landdir = Path.Combine(OutputFolder, "levels", land.Name);
                Directory.CreateDirectory(landdir);
                if (land.COL.Count > 0)
                {
                    foreach (COL col in land.COL)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (i == 0 && delete_basic)
                            {
                                model_dir = "basicmodels";
                                model_extension = ".sa1mdl";
                            }
                            else if (i == 1 && delete_chunk)
                            {
                                model_dir = "chunkmodels";
                                model_extension = ".sa2mdl";
                            }
                            else if (i == 2 && delete_gc)
                            {
                                model_dir = "gcmodels";
                                model_extension = ".sa2bmdl";
                            }
                            string col_filename = Path.Combine(OutputFolder, model_dir, col.Model.Name.Substring(7, col.Model.Name.Length - 7) + model_extension);
                            if (SingleOutputFolder)
                                col_filename = Path.Combine(OutputFolder, col.Model.Name.Substring(7, col.Model.Name.Length - 7) + model_extension);
                            if (File.Exists(col_filename))
                            {
                                uint itemaddr = uint.Parse(col.Model.Name.Substring(7, col.Model.Name.Length - 7), NumberStyles.AllowHexSpecifier);
                                File.Move(col_filename, Path.Combine(landdir, Path.GetFileName(col_filename)));
                                Console.WriteLine("Moving landtable object {0}", Path.GetFileName(col_filename));
                                if (!KeepLandtableModels)
                                    if (addresslist.ContainsKey(itemaddr))
                                        addresslist.Remove(itemaddr);
                            }
                        }
                    }
                    foreach (GeoAnimData anim in land.Anim)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (i == 0 && delete_basic)
                            {
                                model_dir = "basicmodels";
                                model_extension = ".sa1mdl";
                            }
                            else if (i == 1 && delete_chunk)
                            {
                                model_dir = "chunkmodels";
                                model_extension = ".sa2mdl";
                            }
                            else if (i == 2 && delete_gc)
                            {
                                model_dir = "gcmodels";
                                model_extension = ".sa2bmdl";
                            }
                            string anim_filename = Path.Combine(OutputFolder, model_dir, anim.Model.Name.Substring(7, anim.Model.Name.Length - 7) + model_extension);
                            if (SingleOutputFolder)
                                anim_filename = Path.Combine(OutputFolder, anim.Model.Name.Substring(7, anim.Model.Name.Length - 7) + model_extension);
                            if (File.Exists(anim_filename))
                            {
                                uint itemaddr = uint.Parse(anim.Model.Name.Substring(7, anim.Model.Name.Length - 7), NumberStyles.AllowHexSpecifier);
                                Console.WriteLine("Moving landtable GeoAnim object {0}", anim_filename);
                                File.Move(anim_filename, Path.Combine(landdir, Path.GetFileName(anim_filename)));
                                if (File.Exists(Path.ChangeExtension(anim_filename, ".action")))
                                {
                                    File.Move(Path.ChangeExtension(anim_filename, ".action"), Path.Combine(landdir, Path.GetFileNameWithoutExtension(anim_filename) + ".action"));
                                    // TODO: Insert a reference to the parent folder in the action file so that the animation runs correctly
                                }
                                if (!KeepLandtableModels)
                                    if (addresslist.ContainsKey(itemaddr))
                                        addresslist.Remove(itemaddr);
                            }
                        }
                    }
                }
            }
        }
    }
}
