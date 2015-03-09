using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using Microsoft.DirectX.Direct3D;

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
		public static string CAMName;
		public static List<ObjectDefinition> ObjDefs;
		public static List<SETItem>[] SETItems;
		public static List<CAMItem>[] CAMItems;
		public static List<DeathZoneItem> DeathZones;
		public static LevelDefinition leveleff;
		public static List<SplineData> LevelSplines;

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
			InvalidateRenderState();
		}

		/// <summary>
		/// This will clear all animated level models.
		/// </summary>
		public static void ClearLevelGeoAnims()
		{
			geo.Anim.Clear();
			InvalidateRenderState();
		}

		/// <summary>
		/// Clears SET Items for all characters.
		/// </summary>
		public static void ClearSETItems()
		{
			if (LevelData.SETItems == null)
				return;

			for (uint i = 0; i < SETChars.Length; i++)
				LevelData.SETItems[i] = new List<SETItem>();

			InvalidateRenderState();
		}

		/// <summary>
		/// Clears SET Items for the specified character.
		/// </summary>
		/// <param name="character">The ID of the character whose layout you want to clear.</param>
		public static void ClearSETItems(int character)
		{
			if (LevelData.SETItems == null)
				return;

			LevelData.SETItems[character] = new List<SETItem>();
			InvalidateRenderState();
		}

		/// <summary>
		/// Clears CAM Items for all characters.
		/// </summary>
		public static void ClearCAMItems()
		{
			if (LevelData.CAMItems == null)
				return;

			for (uint i = 0; i < SETChars.Length; i++)
				LevelData.CAMItems[i] = new List<CAMItem>();

			InvalidateRenderState();
		}

		/// <summary>
		/// Clears CAM Items for the specified character.
		/// </summary>
		/// <param name="character">The ID of the character whose layout you want to clear.</param>
		public static void ClearCAMItems(int character)
		{
			if (LevelData.CAMItems == null)
				return;

			LevelData.CAMItems[character] = new List<CAMItem>();
			InvalidateRenderState();
		}
		
		public static string GetStats()
		{
			int landtableItems = LevelData.geo.COL.Count;
			int textureArcCount = LevelData.Textures.Count;
			int setItems = 0;
			int animatedItems = LevelData.geo.Anim.Count;
			int cameraItems = 0;

			if (LevelData.SETItems != null) setItems = LevelData.SETItems[LevelData.Character].Count;
			if (LevelData.CAMItems != null) cameraItems = LevelData.CAMItems[LevelData.Character].Count;

			return String.Format("Landtable items: {0}\nTexture Archives: {1}\nAnimated Level Models:{2}\nSET Items: {3}\nCamera Zones/Items:{4}", landtableItems, textureArcCount, animatedItems, setItems, cameraItems);
		}

		public static void DuplicateSelection(Device d3ddevice, ref List<Item> SelectedItems, out bool errorFlag, out string errorMsg)
		{
			if (SelectedItems == null)
			{
				SelectedItems = null;
				errorFlag = false;
				errorMsg = "";
				return;
			}

			if (SelectedItems.Count < 0) { errorFlag = true; errorMsg = "Negative selection count... what did you do?!?"; return; }

			if (SelectedItems.Count == 1)
			{
				// duplicate goes here
				if (SelectedItems[0] is SETItem)
				{
					SETItem originalItem = (SETItem)SelectedItems[0];
					SETItem newItem = new SETItem(originalItem.GetBytes(), 0);

					LevelData.SETItems[LevelData.Character].Add(newItem);
					SelectedItems = new List<Item>() { newItem };
					InvalidateRenderState();
				}
				else if (SelectedItems[0] is LevelItem)
				{
					LevelItem originalItem = (LevelItem)SelectedItems[0];
					LevelItem newItem = new LevelItem(d3ddevice, originalItem.CollisionData.Model.Attach, originalItem.Position, originalItem.Rotation, LevelItems.Count);

					newItem.CollisionData.SurfaceFlags = originalItem.CollisionData.SurfaceFlags;
					SelectedItems.Clear();
					SelectedItems = new List<Item>() { newItem };
					InvalidateRenderState();
				}
				else if (SelectedItems[0] is CAMItem)
				{
					CAMItem originalItem = (CAMItem)SelectedItems[0];
					CAMItem newItem = new CAMItem(originalItem.GetBytes(), 0);

					LevelData.CAMItems[LevelData.Character].Add(newItem);
					SelectedItems = new List<Item>() { newItem };
					InvalidateRenderState();
				}
			}
			else
			{
				List<Item> newItems = new List<Item>();

				// duplicate goes here
				for (int i = 0; i < SelectedItems.Count; i++)
				{
					if (SelectedItems[i] is SETItem)
					{
						SETItem originalItem = (SETItem)SelectedItems[i];
						SETItem newItem = new SETItem(originalItem.GetBytes(), 0);

						LevelData.SETItems[LevelData.Character].Add(newItem);
						newItems.Add(newItem);
					}
					else if (SelectedItems[i] is LevelItem)
					{
						LevelItem originalItem = (LevelItem)SelectedItems[0];
						LevelItem newItem = new LevelItem(d3ddevice, originalItem.CollisionData.Model.Attach, originalItem.Position, originalItem.Rotation, LevelItems.Count);

						newItem.CollisionData.SurfaceFlags = originalItem.CollisionData.SurfaceFlags;
						newItems.Add(newItem);
					}
					else if (SelectedItems[i] is CAMItem)
					{
						CAMItem originalItem = (CAMItem)SelectedItems[i];
						CAMItem newItem = new CAMItem(originalItem.GetBytes(), 0);

						LevelData.CAMItems[LevelData.Character].Add(newItem);
						newItems.Add(newItem);
					}
				}

				SelectedItems.Clear();
				SelectedItems = newItems;

				InvalidateRenderState();
			}

			errorFlag = false;
			errorMsg = "";
		}

		public static List<Item> ImportFromFile(string filePath, Device d3ddevice, SAModel.Direct3D.EditorCamera camera, out bool errorFlag, out string errorMsg)
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

			if ((filePathInfo.Extension == ".obj") || (filePathInfo.Extension == ".objf"))
			{
				Microsoft.DirectX.Vector3 pos = camera.Position + (-20 * camera.Look);
				LevelItem item = new LevelItem(d3ddevice, filePath, new Vertex(pos.X, pos.Y, pos.Z), new Rotation(), LevelItems.Count);

				item.Visible = true;
				createdItems.Add(item);
			}
			else if (filePathInfo.Extension == ".txt")
			{
				SAEditorCommon.Import.NodeTable.ImportFromFile(d3ddevice, filePath, out importError, out importErrorMsg);
			}
			else
			{
				errorFlag = true;
				errorMsg = "Invalid file format!";
				return null;
			}

			/*switch (filePathInfo.Extension)
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
			}*/

			StateChanged();

			errorFlag = importError;
			errorMsg = importErrorMsg;

			return createdItems;
		}
	}
}