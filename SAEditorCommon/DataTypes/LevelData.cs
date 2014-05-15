using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using Microsoft.DirectX.Direct3D;

using PuyoTools;
using VrSharp.PvrTexture;

using SonicRetro.SAModel;
using SonicRetro.SAModel.SAEditorCommon.DataTypes;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using SonicRetro.SAModel.Direct3D.TextureSystem;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
{
    public static class LevelData
    {
        #region Events
        public delegate void LevelStateChangeHandler();
        public static event LevelStateChangeHandler StateChanged; // this one should allow us to tell the editor to re-render without needing an actual reference to MainForm
        #endregion

        public static LandTable geo;
        public static string leveltexs;
        public static Dictionary<string, BMPInfo[]> TextureBitmaps;
        public static Dictionary<string, Texture[]> Textures;
        public static List<LevelItem> LevelItems;
        public static readonly string[] Characters = { "sonic", "tails", "knuckles", "amy", "gamma", "big" };
        public static readonly string[] SETChars = { "S", "M", "K", "A", "E", "B" };
        public static int Character;
        public static StartPosItem[] StartPositions;
        public static string LevelName;
        public static string SETName;
        public static List<ObjectDefinition> ObjDefs;
        public static List<SETItem>[] SETItems;
        public static List<DeathZoneItem> DeathZones;
        public static LevelDefinition leveleff;

        /// <summary>
        /// This invokes the StateChanged event. Call this any time an outside form or control modifies the level data.
        /// </summary>
        public static void InvalidateRenderState()
        {
            if (StateChanged != null)
            {
                StateChanged();
            }
        }

        /// <summary>
        /// This will clear the level's geometry, letting the user start fresh.
        /// </summary>
        public static void ClearLevelGeometry()
        {
            LevelItems.Clear();
            geo.COL.Clear();
            if(StateChanged != null) StateChanged();
        }

        /// <summary>
        /// This will clear all animated level models.
        /// </summary>
        public static void ClearLevelGeoAnims()
        {
            geo.Anim.Clear();
            if (StateChanged != null) StateChanged();
        }

        public static List<Item> ImportFromFile(string filePath, Device d3ddevice, SAModel.Direct3D.Camera camera, out bool errorFlag, out string errorMsg)
        {
            List<Item> createdItems = new List<Item>();

            if (!File.Exists(filePath))
            {
                errorFlag = true;
                errorMsg = "File does not exist!";
                return null;
            }

            DirectoryInfo filePathInfo = new DirectoryInfo(filePath);

            bool importError = false;
            string importErrorMsg = "";

            switch (filePathInfo.Extension)
            {
                case (".obj"):
                    Microsoft.DirectX.Vector3 pos = camera.Position + (-20 * camera.Look);
                    LevelItem item = new LevelItem(d3ddevice, filePath, new Vertex(pos.X, pos.Y, pos.Z), new Rotation());

                    item.Visible = true;
                    createdItems.Add(item);
                    break;

                case (".txt"):
                    SAEditorCommon.Import.NodeTable.ImportFromFile(d3ddevice, filePath, out importError, out importErrorMsg);
                    break;

                default:
                    errorFlag = true;
                    errorMsg = "Invalid file format!";
                    return null;
            }

            StateChanged();

            errorFlag = importError;
            errorMsg = importErrorMsg;

            return createdItems;
        }
    }
}