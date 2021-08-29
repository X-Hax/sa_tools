using System;
using System.Collections.Generic;
using System.IO;
using SplitTools;

// This tool was made to facilitate label transfer between the X360 and 2004 PC versions of SADX. Code is very rough and not meant for regular use.

namespace LabelTool
{
	partial class Program
	{
        static void Main(string[] args)
        {
            switch (args[args.Length - 1])
            {
                // Generate labels
                case "-g":
                    LabelGen_Main(args);
                    return;
                // Create match list
                case "-cm":
                    LabelMatchList(args);
                    return;
                // Create an address range list
                case "-ad":
                    LabelAddressList(args);
                    return;
                // Find duplicate addresses in split INI files
                case "-di":
                    LabelDuplicateINI(args);
                    return;
            }
        }

        // Builds a label database by comparing two folders containing labelled and unlabelled assets
        static void LabelMatchList(string[] args)
        {
            string folder1 = args[0];
            string folder2 = args[1];
            string[] files = Directory.GetFiles(Path.GetFullPath(folder1), "*.sa*", SearchOption.AllDirectories);
            TextWriter writer_split = File.CreateText("transfer_split.txt");
            TextWriter writer_ida = File.CreateText("transfer_ida.txt");
            Dictionary<string, int> labelindex = IniSerializer.Deserialize<Dictionary<string, int>>(Path.Combine(Environment.CurrentDirectory, "labels", "index.txt"));
            for (int i = 0; i < files.Length; i++)
            {
                string filePath = MakeRelativePath(Path.GetFullPath(folder1), files[i]);
                Console.WriteLine("Processing file {0} of {1}: {2}", i + 1, files.Length, filePath);
                string fileMatch = Path.Combine(Path.GetFullPath(folder2), filePath);
                if (File.Exists(fileMatch))
                    TransferLabels(files[i], fileMatch, writer_split, writer_ida, labelindex);
                else
                    Console.WriteLine("File not found: {0}", fileMatch);
                Console.WriteLine();
            }
            writer_split.Flush();
            writer_split.Close();
            writer_ida.Flush();
            writer_ida.Close();
        }

        // Scans a folder for split INI files and prints duplicate addresses
        static void LabelDuplicateINI(string[] args)
        {
            Dictionary<int, string> addresses = new Dictionary<int, string>();
            string[] files = Directory.GetFiles(Path.GetFullPath(args[0]), "*.ini", SearchOption.TopDirectoryOnly);
            for (int u = 0; u < files.Length; u++)
            {
                if (files[u].Contains("sadxlvl") || files[u].Contains("DLL") || files[u].Contains("CHRMODELS") || files[u].Contains("objdefs") || files[u].Contains("garden"))
                    continue;
                //Console.WriteLine("Checking file {0}", files[u]);
                IniData inifile = IniSerializer.Deserialize<IniData>(files[u]);
                foreach (var split in inifile.Files)
                {
                    if (addresses.ContainsKey(split.Value.Address))
                    {
                        if (!split.Value.Filename.Contains(".dum"))
                            Console.WriteLine("{0}: {1} in {2} already exists as {3}", split.Value.Address.ToString("X"), split.Value.Filename, files[u], addresses[split.Value.Address]);
                    }
                    else
                    {
                        if (!split.Value.Filename.Contains(".dum"))
                            addresses.Add(split.Value.Address, split.Value.Filename);
                    }
                }
            }
        }

        // Scans a folder with unlabelled (address-based) assets and outputs a list of address ranges occupied by each asset
        static void LabelAddressList(string[] args)
        {
            string folder = args[0];
            string[] files = Directory.GetFiles(Path.GetFullPath(folder), "*.sa*", SearchOption.AllDirectories);
            TextWriter writer_list = File.CreateText("transfer_address.txt");
            for (int i = 0; i < files.Length; i++)
            {
                string filePath_rel = MakeRelativePath(Path.GetFullPath(folder), files[i]);
                Console.WriteLine("Processing file {0} of {1}: {2}", i + 1, files.Length, filePath_rel);
                AddressesFromLabels(files[i], filePath_rel, writer_list);
            }
            writer_list.Flush();
            writer_list.Close();
        }
    }
}