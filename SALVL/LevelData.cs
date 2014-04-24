using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using Microsoft.DirectX.Direct3D;

using PuyoTools;
using VrSharp.PvrTexture;

using SonicRetro.SAModel.Direct3D.TextureSystem;

namespace SonicRetro.SAModel.SALVL
{
    internal static class LevelData
    {
        public static MainForm MainForm;
        public static LandTable geo;
        public static string leveltexs;
        public static Dictionary<string, BMPInfo[]> TextureBitmaps;
        public static Dictionary<string, Texture[]> Textures;
        public static List<LevelItem> LevelItems;
    }
}