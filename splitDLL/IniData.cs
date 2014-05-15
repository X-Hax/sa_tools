using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using SA_Tools;

namespace splitDLL
{
    public class IniData
    {
        [IniName("game")]
        [DefaultValue(Game.SADX)]
        public Game Game { get; set; }
		[IniName("modulename")]
		public string ModuleName { get; set; }
        [IniCollection]
        public Dictionary<string, FileInfo> Files { get; set; }
    }

    public enum Game
    {
        SADX,
        SA2B
    }

    public class FileInfo
    {
        [IniName("type")]
        public string Type { get; set; }
		[IniName("length")]
		public int Length { get; set; }
        [IniName("filename")]
        public string Filename { get; set; }
    }
}