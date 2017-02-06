using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using SonicRetro.SAModel.Direct3D;
using SonicRetro.SAModel.Direct3D.TextureSystem;
using SonicRetro.SAModel.SAEditorCommon.Import;
using SonicRetro.SAModel.SAEditorCommon.SETEditing;
using SonicRetro.SAModel.SAEditorCommon.UI;

namespace SonicRetro.SAModel.SAEditorCommon.DataTypes
{
	public static class LevelData
	{
		#region Events
		public static event Action StateChanged; // this one should allow us to tell the editor to re-render without needing an actual reference to MainForm
		public static event Action PointOperation = delegate { };
		#endregion

		public static LandTable geo;
		public static string leveltexs;
		public static Dictionary<string, BMPInfo[]> TextureBitmaps;
		public static Dictionary<string, Texture[]> Textures;
		public static List<LevelItem> LevelItems;
		public static readonly string[] Characters = { "Sonic", "Tails", "Knuckles", "Amy", "Gamma", "Big" };
		public static readonly string[] SETChars = { "S", "M", "K", "A", "E", "B" };
		public static int Character;
		public static StartPosItem[] StartPositions;
		public static string LevelName;
		public static string SETName;
		public static List<ObjectDefinition> ObjDefs;
		public static List<ObjectDefinition> MisnObjDefs;
		public static List<SETItem>[] SETItems;
		public static List<CAMItem>[] CAMItems;
		public static List<MissionSETItem>[] MissionSETItems;
		public static List<DeathZoneItem> DeathZones;
		public static LevelDefinition leveleff;
		public static List<SplineData> LevelSplines;

		/// <summary>
		/// This invokes the StateChanged event. Call this any time an outside form or control modifies the level data.
		/// </summary>
		public static void InvalidateRenderState()
		{
			StateChanged?.Invoke();
		}

		public static void BeginPointOperation()
		{
			PointOperation();
		}

		/// <summary>
		/// This will clear the level's geometry, letting the user start fresh.
		/// </summary>
		public static void ClearLevelGeometry()
		{
			if (LevelItems != null)
				LevelItems.Clear();
			if (geo != null && geo.COL != null)
				geo.COL.Clear();

			InvalidateRenderState();
		}

		/// <summary>
		/// This will clear all animated level models.
		/// </summary>
		public static void ClearLevelGeoAnims()
		{
			if (geo != null && geo.Anim != null)
			{
				geo.Anim.Clear();
				InvalidateRenderState();
			}
		}

		/// <summary>
		/// Clears SET Items for all characters.
		/// </summary>
		public static void ClearSETItems()
		{
			if (SETItems == null)
				return;

			for (uint i = 0; i < SETChars.Length; i++)
				SETItems[i] = new List<SETItem>();

			InvalidateRenderState();
		}

		/// <summary>
		/// Clears SET Items for the specified character.
		/// </summary>
		/// <param name="character">The ID of the character whose layout you want to clear.</param>
		public static void ClearSETItems(int character)
		{
			if (SETItems == null)
				return;

			SETItems[character] = new List<SETItem>();
			InvalidateRenderState();
		}

		/// <summary>
		/// Clears CAM Items for all characters.
		/// </summary>
		public static void ClearCAMItems()
		{
			if (CAMItems == null)
				return;

			for (uint i = 0; i < SETChars.Length; i++)
				CAMItems[i] = new List<CAMItem>();

			InvalidateRenderState();
		}

		/// <summary>
		/// Clears CAM Items for the specified character.
		/// </summary>
		/// <param name="character">The ID of the character whose layout you want to clear.</param>
		public static void ClearCAMItems(int character)
		{
			if (CAMItems == null)
				return;

			CAMItems[character] = new List<CAMItem>();
			InvalidateRenderState();
		}

		/// <summary>
		/// Clears Mission SET Items for all characters.
		/// </summary>
		public static void ClearMissionSETItems()
		{
			if (MissionSETItems == null)
				return;

			for (uint i = 0; i < SETChars.Length; i++)
				MissionSETItems[i] = new List<MissionSETItem>();

			InvalidateRenderState();
		}

		/// <summary>
		/// Clears Mission SET Items for the specified character.
		/// </summary>
		/// <param name="character">The ID of the character whose layout you want to clear.</param>
		public static void ClearMissionSETItems(int character)
		{
			if (MissionSETItems == null)
				return;

			MissionSETItems[character] = new List<MissionSETItem>();
			InvalidateRenderState();
		}

		/// <summary>
		/// Clears the entire stage.
		/// </summary>
		public static void Clear()
		{
			ClearMissionSETItems();
			ClearCAMItems();
			ClearSETItems();
			ClearLevelGeoAnims();
			ClearLevelGeometry();
		}

		public static string GetStats()
		{
			int landtableItems = geo.COL.Count;
			int textureArcCount = Textures.Count;
			int setItems = 0;
			int animatedItems = geo.Anim.Count;
			int cameraItems = 0;

			if (SETItems != null) setItems = SETItems[Character].Count;
			if (CAMItems != null) cameraItems = CAMItems[Character].Count;

			return String.Format("Landtable items: {0}\nTexture Archives: {1}\nAnimated Level Models:{2}\nSET Items: {3}\nCamera Zones/Items:{4}", landtableItems, textureArcCount, animatedItems, setItems, cameraItems);
		}

		public static void DuplicateSelection(Device d3ddevice, EditorItemSelection selection, out bool errorFlag, out string errorMsg)
		{
			if (selection.ItemCount < 0) { errorFlag = true; errorMsg = "Negative selection count... what did you do?!?"; return; }

			List<Item> newItems = new List<Item>();
			List<Item> currentItems = selection.GetSelection();

			// duplicate goes here
			for (int i = 0; i < selection.ItemCount; i++)
			{
				if (currentItems[i] is MissionSETItem)
				{
					MissionSETItem originalItem = (MissionSETItem)currentItems[i];
					MissionSETItem newItem = new MissionSETItem(originalItem.GetBytes(), 0, originalItem.GetPRMBytes(), 0, selection);

					MissionSETItems[Character].Add(newItem);
					newItems.Add(newItem);
				}
				else if (currentItems[i] is SETItem)
				{
					SETItem originalItem = (SETItem)currentItems[i];
					SETItem newItem = new SETItem(originalItem.GetBytes(), 0, selection);

					SETItems[Character].Add(newItem);
					newItems.Add(newItem);
				}
				else if (currentItems[i] is LevelItem)
				{
					LevelItem originalItem = (LevelItem)currentItems[0];
					LevelItem newItem = new LevelItem(d3ddevice, originalItem.CollisionData.Model.Attach, originalItem.Position, originalItem.Rotation, LevelItems.Count, selection);

					newItem.CollisionData.SurfaceFlags = originalItem.CollisionData.SurfaceFlags;
					newItems.Add(newItem);
				}
				else if (currentItems[i] is CAMItem)
				{
					CAMItem originalItem = (CAMItem)currentItems[i];
					CAMItem newItem = new CAMItem(originalItem.GetBytes(), 0, selection);

					CAMItems[Character].Add(newItem);
					newItems.Add(newItem);
				}
			}

			selection.Clear();
			selection.Add(newItems);

			InvalidateRenderState();

			errorFlag = false;
			errorMsg = "";
		}

		public static List<Item> ImportFromFile(string filePath, Device d3ddevice, EditorCamera camera, out bool errorFlag, out string errorMsg, EditorItemSelection selectionManager)
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
				case ".obj":
				case ".objf":
					Vector3 pos = camera.Position + (-20 * camera.Look);
					LevelItem item = new LevelItem(d3ddevice, filePath, new Vertex(pos.X, pos.Y, pos.Z), new Rotation(), LevelItems.Count, selectionManager)
					{
						Visible = true
					};

					createdItems.Add(item);
					break;

				case ".txt":
					NodeTable.ImportFromFile(d3ddevice, filePath, out importError, out importErrorMsg, selectionManager);
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