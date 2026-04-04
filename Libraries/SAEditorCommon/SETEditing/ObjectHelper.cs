using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D9;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using SAModel.SAEditorCommon.Properties;
using Color = System.Drawing.Color;
using Mesh = SAModel.Direct3D.Mesh;
using System.Collections.Concurrent;
using TextureLib;

namespace SAModel.SAEditorCommon.SETEditing
{
	/// <summary>
	/// Helper class for object definitions. Contains methods to retrieve and cache textures, meshes etc.
	/// </summary>
	public static class ObjectHelper
	{
		/// <summary>Flat "sprite" model.</summary>
		private static NJS_OBJECT FlatSquareModel;
		/// <summary>Question mark model for the default definition.</summary>
		private static NJS_OBJECT QuestionBoxModel;
		/// <summary>Question mark meshes for the default definition.</summary>
		private static Mesh QuestionBoxMesh;
		/// <summary>Question mark texture.</summary>
		internal static Texture QuestionMarkTexture;
		/// <summary>Cache of loaded models.</summary>
		private static ConcurrentDictionary<string, NJS_OBJECT> ModelCache;
		/// <summary>Cache of loaded meshes.</summary>
		private static ConcurrentDictionary<NJS_OBJECT, Mesh[]> MeshCache;
		/// <summary>Cache of loaded textures per texlist (by NJS_TEXLIST name). Same Texture items may be included in both LevelData.Textures and TexlistCache.</summary>
		private static ConcurrentDictionary<string, Texture[]> TexlistCache;
		/// <summary>Cache of loaded motions.</summary>
		private static ConcurrentDictionary<string, NJS_MOTION> MotionCache;

		/// <summary>
		/// Initializes the cache and default meshes and textures for the specified Device.
		/// </summary>
		/// <param name="device">Direct3D device.</param>
		public static void Init(Device device)
		{		
			ModelCache = new(StringComparer.OrdinalIgnoreCase);
			MotionCache = new(StringComparer.OrdinalIgnoreCase);
			TexlistCache = new(StringComparer.OrdinalIgnoreCase);
			MeshCache = [];
			QuestionBoxModel = new ModelFile(Resources.questionmark).Model;
			FlatSquareModel = new ModelFile(Resources.flatSquare).Model;
			QuestionBoxMesh = GetMeshes(QuestionBoxModel).First();
			QuestionMarkTexture = Resources.questionmark_t.ToTexture(device);
		}

		/// <summary>
		/// Loads a model from a file.
		/// </summary>
		/// <param name="file">Path to the model file to load.</param>
		/// <returns>NJS_OBJECT of: a cached entry (if available), a newly loaded model or the question mark box.</returns>
		public static NJS_OBJECT LoadModel(string file)
		{
			if (!System.IO.File.Exists(file))
				return QuestionBoxModel;
			if (ModelCache.TryGetValue(file, out NJS_OBJECT value))
				return value;
			NJS_OBJECT model = new ModelFile(file).Model;
			ModelCache.TryAdd(file, model);
			return model;
		}

		/// <summary>
		/// Loads a motion from a file.
		/// </summary>
		/// <param name="file">Path to the motion file to load.</param>
		/// <returns>NJS_MOTION of: a cached entry (if available), a newly loaded motion or null.</returns>
		public static NJS_MOTION LoadMotion(string file)
		{
			if (!System.IO.File.Exists(file))
				return null;
			if (MotionCache.TryGetValue(file, out NJS_MOTION value))
				return value;
			NJS_MOTION motion = NJS_MOTION.Load(file);
			MotionCache.TryAdd(file, motion);
			return motion;
		}

		/// <summary>
		/// Creates SAModel Meshes for the specified NJS_OBJECT.
		/// </summary>
		/// <param name="model">Model to generate meshes.</param>
		/// <returns>An array of Mesh[] for the specified NJS_OBJECT (a cached one if the object has had meshes generated previously).</returns>
		public static Mesh[] GetMeshes(NJS_OBJECT model)
		{
			if (MeshCache.TryGetValue(model, out Mesh[] value))
				return value;
			model.ProcessVertexData();
			NJS_OBJECT[] models = model.GetObjects();
			Mesh[] Meshes = new Mesh[models.Length];
			for (int i = 0; i < models.Length; i++)
				if (models[i].Attach != null)
					Meshes[i] = models[i].Attach.CreateD3DMesh();
			MeshCache.TryAdd(model, Meshes);
			return Meshes;
		}

		/// <summary>
		/// Creates an array of SharpDX Textures for the specified list of PVM/GVM names and (optionally) using the specified texture list.
		/// </summary>
		/// <param name="pvmNames">List of texture archive names without extensions.</param>
		/// <param name="texlist">NJS_TEXLIST for objects that load textures from multiple archives. Can be null.</param>
		/// <param name="dev">Direct3D device. No textures will be generated if this is null.</param>
		/// <returns>An array of Texture, or null.</returns>
		public static Texture[] GetTextures(List<string> pvmNames, SplitTools.NJS_TEXLIST texlist = null, Device dev = null)
		{
			// No PVM names specified
			if (pvmNames == null || pvmNames.Count == 0)
				return null;
			// No textures loaded or textures disabled in the editor
			if (LevelData.Textures == null || LevelData.TextureBitmaps == null || EditorOptions.DisableTextures)
				return null;
			// If a texlist is not specified, check cache for PVMs
			if (texlist == null)
			{
				if (LevelData.Textures.TryGetValue(pvmNames[0], out Texture[] cacheSingle))
					return cacheSingle;
				// If that fails, it means the required textures are not loaded.
				else return null;
			}
			// If a texlist is specified
			else
			{
				// Check cache for texlists first
				if (TexlistCache.TryGetValue(texlist.Name, out Texture[] cache))
					return cache;
				// Retrieve individual textures
				List<Texture> resultMulti = new List<Texture>();
				// List of duplicate texture names
				List<string> dupnames = new List<string>();
				// Loop through the texlist's texture names
				for (int i = 0; i < texlist.TextureNames.Length; i++)
				{
					string textureName = texlist.TextureNames[i];
					// If the texture is already found, skip it
					if (dupnames.Contains(textureName))
						continue;
					// If not, try to find it
					bool found = false;
					// Loop through PVMs
					for (int pvmId = 0; pvmId < pvmNames.Count; pvmId++)
					{
						// Some SA2 stuff is like this?
						if (string.IsNullOrEmpty(pvmNames[pvmId]))
							continue;
						// Loop through the PVM's texture names
						for (int texId = 0; texId < LevelData.TextureBitmaps[pvmNames[pvmId]].Length; texId++)
						{
							// If a match is found, add it to the result
							if (string.Equals(LevelData.TextureBitmaps[pvmNames[pvmId]][texId].Name, textureName, StringComparison.OrdinalIgnoreCase))
							{
								// Add it from LevelData.Textures instead of creating a new Texture from TextureBitmaps
								resultMulti.Add(LevelData.Textures[pvmNames[pvmId]][texId]);
								// Mark the texture as found
								found = true;
								dupnames.Add(textureName);
								// Stop looping through this PVM
								break;
							}
						}
						// If the texture was found in the PVM, stop looping through PVMs and go to the next texture name
						if (found)
							break;
					}
				}
				return resultMulti.ToArray();
			}
		}

		/// <summary>This function is obsolete, you can use GetTextures with the same arguments instead.</summary>
		[Obsolete]
		public static Texture[] GetTexturesMultiSource(List<string> name, SplitTools.NJS_TEXLIST texnames = null, Device dev = null)
		{
			return GetTextures(name, texnames, dev);
		}

		/// <summary>
		/// Creates an array of SharpDX Textures for the specified PVM/GVM and (optionally) using the specified texture list.
		/// </summary>
		/// <param name="pvmName">Texture archive name without extension.</param>
		/// <param name="texlist">NJS_TEXLIST for objects that load textures from multiple archives. Can be null.</param>
		/// <param name="dev">Direct3D device. No textures will be generated if this is null.</param>
		/// <returns>An array of Texture, or null.</returns>
		public static Texture[] GetTextures(string pvmName, SplitTools.NJS_TEXLIST texlist = null, Device dev = null)
		{
			if (string.IsNullOrEmpty(pvmName))
				return null;
			return GetTextures([pvmName], texlist, dev);
		}

		/// <summary>
		/// Gets texture data for the specified texture archives and (optionally) a texlist. Used in model export.
		/// </summary>
		/// <param name="names">Texture archive names without extensions.</param>
		/// <param name="texlist">NJS_TEXLIST for objects that load textures from multiple archives. Can be null.</param>
		/// <returns>An array of GenericTexture, or null.</returns>
		public static GenericTexture[] GetTextureBmpInfos(List<string> names, SplitTools.NJS_TEXLIST texlist = null)
		{
			// PVM names not specified
			if (names == null || names.Count == 0)
				return null;
			// Textures not loaded or disabled in the editor
			if (LevelData.Textures == null || EditorOptions.DisableTextures)
				return null;
			List<GenericTexture> texInfos = new List<GenericTexture>();
			// Get textures for all PVMs and put them into a single List
			foreach (var name in names)
			{
				// Return null on failure
				if (!LevelData.TextureBitmaps.TryGetValue(name, out GenericTexture[] texturebmps))
					return null;
				texInfos.AddRange(texturebmps);
			}
			// Return the whole texture list if not using a texlist
			if (texlist == null)
				return texInfos.ToArray();
			// Process the texlist using one or multiple PVMs
			List<GenericTexture> result = new List<GenericTexture>();
			// Keep a list of duplicate texture names
			List<string> dupNames = new List<string>();
			// Loop through the list of names in the texlist
			for (int i = 0; i < texlist.TextureNames.Length; i++)
			{
				// Loop through the GenericTextures
				for (int b = 0; b < texInfos.Count; b++)
				{
					// If the texture's name matches the name in the texlist and hasn't been added already, add it to the result
					if (string.Equals(texInfos[b].Name, texlist.TextureNames[i], StringComparison.OrdinalIgnoreCase) && !dupNames.Contains(texlist.TextureNames[i]))
					{
						result.Add(texInfos[b]);
						dupNames.Add(texlist.TextureNames[i]);
						// Since the texture for the specified name has been found, the GenericTexture loop can be stopped
						break;
					}
				}
			}
			return result.ToArray();
		}

		/// <summary>
		/// Gets texture data for the specified texture archive and (optionally) using the specified texlist. Used in model export.
		/// </summary>
		/// <param name="name">Texture archive name without extension.</param>
		/// <param name="texlist">NJS_TEXLIST for objects that load textures from multiple archives. Can be null.</param>
		/// <returns>An array of GenericTexture, or null.</returns>
		public static GenericTexture[] GetTextureBmpInfos(string name, SplitTools.NJS_TEXLIST texlist = null)
		{
			return GetTextureBmpInfos([name], texlist);
		}

		/// <summary>
		/// Checks whether the question mark model has been selected (for default object definition).
		/// </summary>
		/// <param name="Near"></param>
		/// <param name="Far"></param>
		/// <param name="Viewport"></param>
		/// <param name="Projection"></param>
		/// <param name="View"></param>
		/// <param name="transform"></param>
		/// <returns>A HitResult.</returns>
		public static HitResult CheckQuestionBoxHit(Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			return QuestionBoxMesh.CheckHit(Near, Far, Viewport, Projection, View, transform);
		}

		/// <summary>
		/// Renders the question mark box (for default object definition).
		/// </summary>
		/// <param name="dev">Direct3D Device.</param>
		/// <param name="transform">Transform matrix.</param>
		/// <param name="center">Center location of the box (for bounds calculation).</param>
		/// <param name="selected">Whether to draw an inverted selection mesh or not.</param>
		/// <returns>Array of RenderInfo.</returns>
		public static RenderInfo[] RenderQuestionBox(Device dev, MatrixStack transform, Vector3 center, bool selected)
		{
			List<RenderInfo> result = [];
			NJS_MATERIAL mat = new() { DiffuseColor = Color.White };
			// Render the box
			result.Add(new RenderInfo(QuestionBoxMesh, 0, transform.Top, mat, EditorOptions.DisableTextures ? null : QuestionMarkTexture, dev.GetRenderState<FillMode>(RenderState.FillMode), new BoundingSphere(center.X, center.Y, center.Z, 8)));
			// Render the selection
			if (selected)
			{
				mat = new NJS_MATERIAL { DiffuseColor = Color.Yellow, UseAlpha = false };
				result.Add(new RenderInfo(QuestionBoxMesh, 0, transform.Top, mat, null, FillMode.Wireframe, new BoundingSphere(center.X, center.Y, center.Z, 8)));
			}
			return result.ToArray();
		}

		/// <summary>This function is obsolete, use RenderQuestionBox without the 'texture' argument instead.</summary>
		[Obsolete]
		public static RenderInfo[] RenderQuestionBox(Device dev, MatrixStack transform, Texture texture, Vector3 center, bool selected)
		{
			return RenderQuestionBox(dev, transform, center, selected);
		}

		/// <summary>
		/// Gets the bounding sphere of the question mark box (for default object definition).
		/// </summary>
		/// <param name="transform">Transform matrix to use.</param>
		/// <param name="scale">Scale of the box.</param>
		/// <returns>A Bounding Sphere.</returns>
		public static BoundingSphere GetQuestionBoxBounds(MatrixStack transform, float scale)
		{
			return GetModelBounds(QuestionBoxModel, transform, scale);
		}

		/// <summary>
		/// Returns a unique copy of the flat square model for use in object definitions.
		/// </summary>
		/// <returns>NJS_OBJECT that is a flat square.</returns>
		public static NJS_OBJECT GetFlatSquareModel()
		{
			return FlatSquareModel.Clone();
		}

		/// <summary>Converts BAMS rotation to Radians.</summary>
		public static float BAMSToRad(int BAMS)
		{
			return Direct3D.Extensions.BAMSToRad(BAMS);
		}

		/// <summary>Converts Radians to BAMS rotation.</summary>
		public static int RadToBAMS(float rad)
		{
			return (int)(rad * (65536 / (2 * Math.PI)));
		}

		/// <summary>Converts BAMS rotation to Degrees.</summary>
		public static float BAMSToDeg(int BAMS)
		{
			return (float)(BAMS / (65536 / 360.0));
		}

		/// <summary>Converts Degrees to BAMS rotation.</summary>
		public static int DegToBAMS(float deg)
		{
			return (int)(deg * (65536 / 360.0));
		}

		/// <summary>Calculates the sine of a BAMS rotation.</summary>
		public static float NJSin(int BAMS)
		{
			return Direct3D.Extensions.NJSin(BAMS);
		}

		/// <summary>Calculates the cosine of a BAMS rotation.</summary>
		public static float NJCos(int BAMS)
		{
			return Direct3D.Extensions.NJCos(BAMS);
		}

		/// <summary>
		/// Gets the bounding sphere of the specified model/transform with the default scale of 1.0.
		/// </summary>
		/// <param name="model">Model to use in calculation.</param>
		/// <param name="transform">Transform matrix to use.</param>
		/// <returns>A BoundingSphere.</returns>
		public static BoundingSphere GetModelBounds(NJS_OBJECT model, MatrixStack transform)
		{
			return GetModelBounds(model, transform, 1);
		}

		/// <summary>
		/// Gets the bounding sphere of the specified model/transform + scale.
		/// </summary>
		/// <param name="model">Model to use in calculation.</param>
		/// <param name="transform">Transform matrix to use.</param>
		/// <param name="scale">Model scale.</param>
		/// <returns>A BoundingSphere.</returns>
		public static BoundingSphere GetModelBounds(NJS_OBJECT model, MatrixStack transform, float scale)
		{
			return GetModelBounds(model, transform, scale, new BoundingSphere());
		}

		/// <summary>
		/// Gets the bounding sphere of the specified model/transform + scale and merbes it with the specified bounding sphere.
		/// </summary>
		/// <param name="model">Model to use in calculation.</param>
		/// <param name="transform">Transform matrix to use.</param>
		/// <param name="scale">Model scale.</param>
		/// <param name="bounds">Bounding sphere to merge.</param>
		/// <returns>A BoundingSphere.</returns>
		public static BoundingSphere GetModelBounds(NJS_OBJECT model, MatrixStack transform, float scale, BoundingSphere bounds)
		{
			transform.Push();
			model.ProcessTransforms(transform);
			scale *= Math.Max(Math.Max(model.Scale.X, model.Scale.Y), model.Scale.Z);
			if (model.Attach != null)
				bounds = Direct3D.Extensions.Merge(bounds, new BoundingSphere(Vector3.TransformCoordinate(model.Attach.Bounds.Center.ToVector3(), transform.Top).ToVertex(), model.Attach.Bounds.Radius * scale));
			foreach (NJS_OBJECT child in model.Children)
				bounds = GetModelBounds(child, transform, scale, bounds);
			transform.Pop();
			return bounds;
		}

		/// <summary>
		/// Applies rotation of the specified object on the specified matrix.
		/// </summary>
		/// <param name="transform">Transform matrix to use.</param>
		/// <param name="item">SET item instance to use.</param>
		/// <param name="addrot">Additional rotation, if applicable.</param>
		/// <param name="type">Rotation order such as XYZ, ZYX etc.</param>
		public static void RotateObject(MatrixStack transform, SETItem item, Rotation addrot, RotationOrder type = RotationOrder.XYZ)
		{
			if (addrot.X != 0 || addrot.Y != 0 || addrot.Z != 0)
				transform.NJRotateObject(addrot);

			switch (type)
			{
				case RotationOrder.X:
					transform.NJRotateX(item.Rotation.X);
					break;
				case RotationOrder.Y:
					transform.NJRotateY(item.Rotation.Y);
					break;
				case RotationOrder.Z:
					transform.NJRotateZ(item.Rotation.Z);
					break;
				case RotationOrder.XY:
					transform.NJRotateX(item.Rotation.X);
					transform.NJRotateY(item.Rotation.Y);
					break;
				case RotationOrder.XZ:
					transform.NJRotateX(item.Rotation.X);
					transform.NJRotateZ(item.Rotation.Z);
					break;
				case RotationOrder.YX:
					transform.NJRotateY(item.Rotation.Y);
					transform.NJRotateX(item.Rotation.X);
					break;
				case RotationOrder.YZ:
					transform.NJRotateY(item.Rotation.Y);
					transform.NJRotateZ(item.Rotation.Z);
					break;
				case RotationOrder.ZX:
					transform.NJRotateZ(item.Rotation.Z);
					transform.NJRotateX(item.Rotation.X);
					break;
				case RotationOrder.ZY:
					transform.NJRotateZ(item.Rotation.Z);
					transform.NJRotateY(item.Rotation.Y);
					break;
				case RotationOrder.XZY:
					transform.NJRotateX(item.Rotation.X);
					transform.NJRotateZ(item.Rotation.Z);
					transform.NJRotateY(item.Rotation.Y);
					break;
				case RotationOrder.YXZ:
					transform.NJRotateY(item.Rotation.Y);
					transform.NJRotateX(item.Rotation.X);
					transform.NJRotateZ(item.Rotation.Z);
					break;
				case RotationOrder.YZX:
					transform.NJRotateY(item.Rotation.Y);
					transform.NJRotateZ(item.Rotation.Z);
					transform.NJRotateX(item.Rotation.X);
					break;
				case RotationOrder.ZXY:
					transform.NJRotateZ(item.Rotation.Z);
					transform.NJRotateX(item.Rotation.X);
					transform.NJRotateY(item.Rotation.Y);
					break;
				case RotationOrder.ZYX:
					transform.NJRotateZYX(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
					break;
				case RotationOrder.None:
					break;
				case RotationOrder.XYZ:
				default:
					transform.NJRotateXYZ(item.Rotation.X, item.Rotation.Y, item.Rotation.Z);
					break;
			}
		}

		/// <summary>
		/// Calculates the scale of the specified object.
		/// </summary>
		/// <param name="item">SET Item instance.</param>
		/// <param name="addscl">Additional scale (XYZ vector), if applicable.</param>
		/// <param name="type">Scale order type, such as XYZ, X only etc.</param>
		/// <returns></returns>
		public static Vector3 GetScale(SETItem item, Vector3 addscl, ScaleOrder type = ScaleOrder.None)
		{
			float x = 1;
			float y = 1;
			float z = 1;

			switch (type)
			{
				case ScaleOrder.X:
					x = item.Scale.X;
					break;
				case ScaleOrder.Y:
					y = item.Scale.Y;
					break;
				case ScaleOrder.Z:
					z = item.Scale.Z;
					break;
				case ScaleOrder.XY:
					x = item.Scale.X;
					y = item.Scale.Y;
					break;
				case ScaleOrder.XZ:
					x = item.Scale.X;
					z = item.Scale.Z;
					break;
				case ScaleOrder.YZ:
					y = item.Scale.Y;
					z = item.Scale.Z;
					break;
				case ScaleOrder.XYZ:
					x = item.Scale.X;
					y = item.Scale.Y;
					z = item.Scale.Z;
					break;
				case ScaleOrder.AllX:
					x = item.Scale.X;
					y = item.Scale.X;
					z = item.Scale.X;
					break;
				case ScaleOrder.AllY:
					x = item.Scale.Y;
					y = item.Scale.Y;
					z = item.Scale.Y;
					break;
				case ScaleOrder.AllZ:
					x = item.Scale.Z;
					y = item.Scale.Z;
					z = item.Scale.Z;
					break;
				case ScaleOrder.None:
				default:
					break;
			}

			if (addscl.X != 0)
				x += addscl.X;
			if (addscl.Y != 0)
				y += addscl.Y;
			if (addscl.Z != 0)
				z += addscl.Z;

			return new Vector3(x, y, z);
		}
	}
}