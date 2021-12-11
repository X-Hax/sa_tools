using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using SAModel.Direct3D.TextureSystem;
using SAModel.SAEditorCommon.Import;
using SAModel.SAEditorCommon.SETEditing;
using SAModel.SAEditorCommon.UI;

namespace SAModel.SAEditorCommon.DataTypes
{
	public static class LevelData
	{
		#region Events
		public static event Action StateChanged; // this one should allow us to tell the editor to re-render without needing an actual reference to MainForm
		public static event Action PointOperation = delegate { };
		public static event Action CharacterChanged = delegate { };
		#endregion

		private static Stack<string> changes = new Stack<string>();
		public static string GetRecentChange()
		{
			return changes.Peek();
		}

		private static bool suppressEvents = false;
		public static bool SuppressEvents { get { return suppressEvents; } set { suppressEvents = value; } }

		public static LandTable geo;
		public static string leveltexs;
		public static Dictionary<string, BMPInfo[]> TextureBitmaps;
		public static Dictionary<string, Texture[]> Textures;
		public static readonly string[] Characters = { "Sonic", "Tails", "Knuckles", "Amy", "Gamma", "Big" };
		public static readonly string[] SETChars = { "S", "M", "K", "A", "E", "B" };
		private static int character;
		public static int Character
		{
			get { return character; }
			set
			{
				int oldCharacter = character;
				character = value;
				if (oldCharacter != character) CharacterChanged.Invoke();
			}
		}
		public static string LevelName;
		public static string SETName;
		public static List<ObjectDefinition> ObjDefs;
		public static List<ObjectDefinition> MisnObjDefs;
		public static LevelDefinition leveleff;
		public static StartPosItem[] StartPositions;

		#region Editable Objects
		// level geometry items
		private static List<LevelItem> levelItems = new List<LevelItem>();

		public static int LevelItemCount { get { return levelItems.Count; } }
		public static LevelItem GetLevelitemAtIndex(int index)
		{
			return levelItems[index];
		}
		public static IEnumerable<LevelItem> LevelItems { get { return levelItems; } }
		public static int GetIndexOfItem(LevelItem item)
		{
			if (levelItems.Contains(item))
			{
				return levelItems.IndexOf(item);
			}
			else return -1;
		}

		// level animations
		private static List<LevelAnim> levelAnims = new List<LevelAnim>();

		public static int LevelAnimCount { get { return levelAnims.Count; } }
		public static LevelAnim GetLevelAnimAtIndex(int index)
		{
			return levelAnims[index];
		}
		public static IEnumerable<LevelAnim> LevelAnims { get { return levelAnims; } }
		public static int GetIndexOfItem(LevelAnim item)
		{
			if (levelAnims.Contains(item))
			{
				return levelAnims.IndexOf(item);
			}
			else return -1;
		}

		// set items
		public static void InitSETItems()
		{
			setItems = new List<SETItem>[LevelData.SETChars.Length];
		}

		public static void NullifySETItems()
		{
			setItems = null;
		}

		public static bool CharHasSETItems(int characterID)
		{
			return setItems[characterID] != null && setItems[characterID].Count > 0;
		}

		public static bool SETItemsIsNull()
		{
			return setItems == null;
		}

		public static void AssignSetList(int characterID, List<SETItem> list)
		{
			setItems[characterID] = list;
		}

		private static List<SETItem>[] setItems;

		public static int GetSetItemCount(int characterID)
		{
			return setItems[characterID].Count;
		}

		public static SETItem GetSetItemAtIndex(int characterID, int itemIndex)
		{
			int setItemCount = GetSetItemCount(characterID);

			if (itemIndex < setItemCount)
			{
				return setItems[characterID][itemIndex];
			}
			else return null;
		}

		public static IEnumerable<SETItem> SETItems(int characterID)
		{
			return setItems[characterID];
		}

		public static int GetIndexOfSETItem(int characterID, SETItem item)
		{
			if (characterID < setItems.Length && setItems[characterID].Contains(item))
			{
				return setItems[characterID].IndexOf(item);
			}
			else return -1;
		}

		public static List<CAMItem>[] CAMItems;
		public static List<MissionSETItem>[] MissionSETItems;
		public static List<DeathZoneItem> DeathZones;
		public static List<SplineData> LevelSplines;
		#endregion

		/// <summary>
		/// This invokes the StateChanged event. Call this any time an outside form or control modifies the level data.
		/// </summary>
		public static void InvalidateRenderState()
		{
			if (!suppressEvents) StateChanged?.Invoke();
		}

		public static void BeginPointOperation()
		{
			PointOperation();
		}

		#region Level Geometry Items
		/// <summary>
		/// This will clear the level's geometry, letting the user start fresh.
		/// </summary>
		public static void ClearLevelGeometry()
		{
			if (LevelItems != null)
				levelItems.Clear();
			if (geo != null && geo.COL != null)
				geo.COL.Clear();

			changes.Push("Clear Level Geometry");
			InvalidateRenderState();
		}

		public static void ClearLevelItems()
		{
			if (levelItems != null)
			{
				levelItems.Clear();
			}
			else levelItems = new List<LevelItem>();

			changes.Push("Clear Level Items");
		}

		public static void RemoveLevelItem(LevelItem item)
		{
			LevelData.levelItems.Remove(item);

			changes.Push("Remove level item");
			InvalidateRenderState();
		}

		public static void AddLevelItem(LevelItem item)
		{
			LevelData.levelItems.Add(item);

			changes.Push("Add level item");
			InvalidateRenderState();
		}
		#endregion

		#region Level Animations
		public static void ClearLevelAnims()
		{
			if (levelAnims != null)
			{
				levelAnims.Clear();
			}
			else levelAnims = new List<LevelAnim>();

			changes.Push("Clear Level Animations");
		}

		public static void RemoveLevelAnim(LevelAnim item)
		{
			LevelData.levelAnims.Remove(item);

			changes.Push("Remove level animation");
			InvalidateRenderState();
		}

		public static void AddLevelAnim(LevelAnim item)
		{
			LevelData.levelAnims.Add(item);

			changes.Push("Add level animation");
			InvalidateRenderState();
		}

		/// <summary>
		/// This will clear all animated level models.
		/// </summary>
		public static void ClearLevelGeoAnims()
		{
			if (levelAnims != null)
				levelAnims.Clear();
			if (geo != null && geo.Anim != null)
				geo.Anim.Clear();

			changes.Push("Clear Level Animations");
			InvalidateRenderState();
		}
		#endregion

		#region SET Items
		public static void AddSETItem(int characterID, SETItem item)
		{
			setItems[characterID].Add(item);
			changes.Push("Add SET Item");
			InvalidateRenderState();
		}

		public static void RemoveSETItem(int characterID, SETItem item)
		{
			setItems[characterID].Remove(item);
			changes.Push("Remove SET Item");
			InvalidateRenderState();
		}

		/// <summary>
		/// Clears SET Items for all characters.
		/// </summary>
		public static void ClearSETItems()
		{
			if (setItems == null)
				return;

			for (uint i = 0; i < SETChars.Length; i++)
				setItems[i] = new List<SETItem>();

			changes.Push("Clear SET Items");

			InvalidateRenderState();
		}

		/// <summary>
		/// Clears SET Items for the specified character.
		/// </summary>
		/// <param name="character">The ID of the character whose layout you want to clear.</param>
		public static void ClearSETItems(int character)
		{
			if (setItems == null)
				return;

			setItems[character] = new List<SETItem>();
			changes.Push("Clear SET Items");
			InvalidateRenderState();
		}

		#endregion

		/// <summary>
		/// Clears CAM Items for all characters.
		/// </summary>
		public static void ClearCAMItems()
		{
			if (CAMItems == null)
				return;

			for (uint i = 0; i < SETChars.Length; i++)
				CAMItems[i] = new List<CAMItem>();

			changes.Push("Clear CAM Items");
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
			changes.Push("Clear CAM Items");
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

			changes.Push("Clear MI SET Items");
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
			changes.Push("Clear MI SET Items");
			InvalidateRenderState();
		}

		/// <summary>
		/// Clears the entire stage except textures.
		/// </summary>
		public static void Clear()
		{
			ClearMissionSETItems();
			ClearCAMItems();
			ClearSETItems();
			ClearLevelGeoAnims();
			ClearLevelGeometry();

			changes.Push("Clear Stage");
		}
		/// <summary>
		/// Clears loaded textures.
		/// </summary>
		public static void ClearTextures()
		{
			if (Textures != null)
				foreach (KeyValuePair<string, Texture[]> dicc in Textures)
					for (int u = 0; u < dicc.Value.Length; u++)
						dicc.Value[u].Dispose();
			if (TextureBitmaps != null)
				foreach (KeyValuePair<string, BMPInfo[]> dicc2 in TextureBitmaps)
					for (int u = 0; u < dicc2.Value.Length; u++)
						dicc2.Value[u].Image.Dispose();
			TextureBitmaps = null;
			Textures = null;
		}
		public static string GetStats()
		{
			int landtableItems = 0;
			int textureArcCount = 0;
			int setItems = 0;
			int animatedItems = geo.Anim.Count;
			int cameraItems = 0;

			if (Textures != null) textureArcCount = Textures.Count;
			if (geo != null) landtableItems = geo.COL.Count;
			if (LevelData.setItems != null) setItems = LevelData.GetSetItemCount(LevelData.character);
			if (CAMItems != null) cameraItems = CAMItems[Character].Count;

			return String.Format("Landtable items: {0}\nTexture Archives: {1}\nAnimated Level Models: {2}\nSET Items: {3}\nCamera Zones/Items: {4}", landtableItems, textureArcCount, animatedItems, setItems, cameraItems);
		}

		public static void DuplicateSelection(EditorItemSelection selection, out bool errorFlag, out string errorMsg)
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

					//SETItems[Character].Add(newItem);
					AddSETItem(Character, newItem);
					newItems.Add(newItem);
				}
				else if (currentItems[i] is LevelItem)
				{
					LevelItem originalItem = (LevelItem)currentItems[i];
					LevelItem newItem = new LevelItem(originalItem.CollisionData.Model.Attach, originalItem.Position, originalItem.Rotation, levelItems.Count, selection);

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
			InvalidateRenderState();
			selection.Add(newItems);

			changes.Push("Duplicate Item");

			errorFlag = false;
			errorMsg = "";
		}

		internal static List<Item> ImportFromHierarchy(NJS_OBJECT objm, EditorItemSelection selectionManager, OnScreenDisplay osd, bool multiple = false)
		{
			List<Item> createdItems = new List<Item>();
			if (objm.Attach != null)
			{
				objm.Attach.ProcessVertexData();
				LevelItem lvlitem = new LevelItem(objm.Attach, new Vertex(objm.Position.X, objm.Position.Y, objm.Position.Z), objm.Rotation, levelItems.Count, selectionManager)
				{
					Visible = true
				};
				createdItems.Add(lvlitem);
			}
			if (multiple)
			{
				if (objm.Children != null && objm.Children.Count > 0)
				{
					foreach (NJS_OBJECT child in objm.Children)
					{
						createdItems.AddRange(ImportFromHierarchy(child, selectionManager, osd, true));
					}
				}
			}
			return createdItems;
		}

		public static LevelAnim ImportLevelAnimation(NJS_OBJECT objm, NJS_MOTION mot, EditorItemSelection selectionManager)
		{
			objm.ProcessVertexData();
			StateChanged();
			return new LevelAnim(new GeoAnimData(objm, mot), levelAnims.Count, selectionManager);
		}

		public static List<Item> ImportFromFile(string filePath, EditorCamera camera, out bool errorFlag, out string errorMsg, EditorItemSelection selectionManager, OnScreenDisplay osd, bool multiple = false)
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
			Vector3 pos = camera.Position + (-20 * camera.Look);

			switch (filePathInfo.Extension)
			{
				case ".sa1mdl":
					ModelFile mf = new ModelFile(filePath);
					NJS_OBJECT objm = mf.Model;
					osd.ClearMessageList();
					osd.AddMessage("Importing models, please wait...", 3000);
					osd.ClearMessageList();
					createdItems.AddRange(ImportFromHierarchy(objm, selectionManager, osd, multiple));
					osd.AddMessage("Stage import complete!", 100);
					break;
				case ".obj":
				case ".objf":
					LevelItem item = new LevelItem(filePath, new Vertex(pos.X, pos.Y, pos.Z), new Rotation(), levelItems.Count, selectionManager)
					{
						Visible = true
					};

					createdItems.Add(item);
					break;

				case ".txt":
					NodeTable.ImportFromFile(filePath, out importError, out importErrorMsg, selectionManager);
					break;

				case ".dae":
				case ".fbx":
					Assimp.AssimpContext context = new Assimp.AssimpContext();
					Assimp.Configs.FBXPreservePivotsConfig conf = new Assimp.Configs.FBXPreservePivotsConfig(false);
					context.SetConfig(conf);
					Assimp.Scene scene = context.ImportFile(filePath, Assimp.PostProcessSteps.Triangulate);
					for (int i = 0; i < scene.RootNode.ChildCount; i++)
					{
						osd.ClearMessageList();
						osd.AddMessage("Importing model " + i.ToString() + " of " + scene.RootNode.ChildCount.ToString() + "...", 3000);
						Assimp.Node child = scene.RootNode.Children[i];
						List<Assimp.Mesh> meshes = new List<Assimp.Mesh>();
						foreach (int j in child.MeshIndices)
							meshes.Add(scene.Meshes[j]);
						bool isVisible = true;
						for (int j = 0; j < child.MeshCount; j++)
						{
							if (scene.Materials[meshes[j].MaterialIndex].Name.Contains("Collision"))
							{
								isVisible = false;
								break;
							}
						}
						ModelFormat mfmt = ModelFormat.Basic;
						if (isVisible)
							switch (geo.Format)
							{
								case LandTableFormat.SA2:
									mfmt = ModelFormat.Chunk;
									break;
								case LandTableFormat.SA2B:
									mfmt = ModelFormat.GC;
									break;
							}
						NJS_OBJECT obj = AssimpStuff.AssimpImport(scene, child, mfmt, TextureBitmaps[leveltexs].Select(a => a.Name).ToArray(), !multiple);
						{
							//sa2 collision patch
							if (obj.Attach.GetType() == typeof(BasicAttach))
							{
								BasicAttach ba = obj.Attach as BasicAttach;
								foreach (NJS_MATERIAL mats in ba.Material)
									mats.DoubleSided = true;
							}
							//cant check for transparent texture so i gotta force alpha for now, temporary
							else if (obj.Attach.GetType() == typeof(ChunkAttach))
							{
								ChunkAttach ca = obj.Attach as ChunkAttach;
								foreach (PolyChunk polys in ca.Poly)
								{
									if (polys.GetType() == typeof(PolyChunkMaterial))
									{
										PolyChunkMaterial mat = polys as PolyChunkMaterial;
										mat.SourceAlpha = AlphaInstruction.SourceAlpha;
										mat.DestinationAlpha = AlphaInstruction.InverseSourceAlpha;
									}
									else if (polys.GetType() == typeof(PolyChunkStrip))
									{
										PolyChunkStrip str = polys as PolyChunkStrip;
										//str.UseAlpha = true;
									}
								}
							}
						}
						obj.Attach.ProcessVertexData();
						LevelItem newLevelItem = new LevelItem(obj.Attach, new Vertex(obj.Position.X + pos.X, obj.Position.Y + pos.Y, obj.Position.Z + pos.Z), obj.Rotation, levelItems.Count, selectionManager)
						{
							Visible = isVisible
						};
						createdItems.Add(newLevelItem);
					}
					osd.ClearMessageList();
					osd.AddMessage("Stage import complete!", 100);
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