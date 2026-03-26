using System.Collections.Generic;
using SAModel.Direct3D;
using SAModel.SAEditorCommon.DataTypes;
using System;
using SharpDX.Direct3D9;
using Mesh = SAModel.Direct3D.Mesh;
using SharpDX;
using SplitTools;
using SAModel.Direct3D.TextureSystem;

namespace SAModel.SAEditorCommon.SETEditing
{
	/// <summary>
	/// Simple object definition that doesn't use custom definition .cs files.
	/// </summary>
	public class DefaultObjectDefinition : ObjectDefinition
	{
		public static ObjectDefinition DefaultInstance { get; private set; }

		static DefaultObjectDefinition()
		{
			DefaultInstance = new DefaultObjectDefinition();
		}

		private string name = "Unknown";
		private NJS_OBJECT model;
		private Mesh[] meshes;
		private string texture;
		private List<string> texturesmulti;
		private float? addxpos, addypos, addzpos, defxscl, defyscl, defzscl, gnddst, addxscl, addyscl, addzscl;
		private ushort? defxrot, defyrot, defzrot, addxrot, addyrot, addzrot;
		private RotationOrder rottype;
		private ScaleOrder scltype;
		private Texture[] texs;
		private NJS_TEXLIST texnames;

		/// <summary>
		/// Loads the simple (codeless) object definition
		/// </summary>
		/// <param name="data">ObjectData instance (loaded from object defition INI file).</param>
		/// <param name="name">Object name.</param>
		public override void Init(ObjectData data, string name)
		{
			// Set name
			this.name = data.Name ?? name;
			// Set model and texlist
			if (!string.IsNullOrEmpty(data.Model))
			{
				model = ObjectHelper.LoadModel(data.Model);
				if (data.IgnorePos)
					model.Position = new Vertex(0, 0, 0);
				meshes = ObjectHelper.GetMeshes(model);
				if (!string.IsNullOrEmpty(data.Texlist))
					texnames = NJS_TEXLIST.Load(data.Texlist);
			}
			// Set texture
			texture = data.Texture;
			// Set multi-texture
			texturesmulti = data.TexturePacks;
			// Set default rotation
			defxrot = data.DefXRot; defyrot = data.DefYRot; defzrot = data.DefZRot;
			// Set default scale
			defxscl = data.DefXScl; defyscl = data.DefYScl; defzscl = data.DefZScl;
			// Set position offset
			addxpos = data.AddXPos; addypos = data.AddYPos; addzpos = data.AddZPos;
			// Set rotation offset
			addxrot = data.AddXRot; addyrot = data.AddYRot;	addzrot = data.AddZRot;
			// Set scale offset
			addxscl = data.AddXScl; addyscl = data.AddYScl;	addzscl = data.AddZScl;
			// Set ground distance
			gnddst = data.GndDst;
			// Set rotation order
			rottype = data.RotType;
			// Set scale order
			scltype = data.SclType;
		}

		public override HitResult CheckHit(SETItem item, Vector3 Near, Vector3 Far, Viewport Viewport, Matrix Projection, Matrix View, MatrixStack transform)
		{
			HitResult result;
			transform.Push();
			// Position
			transform.NJTranslate(item.Position.X + (addxpos ?? 0), item.Position.Y + (addypos ?? 0), item.Position.Z + (addzpos ?? 0));
			// Rotation
			Rotation addrot = new Rotation(addxrot ?? 0, addyrot ?? 0, addzrot ?? 0);
			ObjectHelper.RotateObject(transform, item, addrot, rottype);
			// If there is no model, use the question mark box
			if (model == null)
				// Result
				result = ObjectHelper.CheckQuestionBoxHit(Near, Far, Viewport, Projection, View, transform);
			// Otherwise check the model
			else
			{
				// Scale
				Vector3 addscl = new Vector3(addxscl ?? 0, addyscl ?? 0, addzscl ?? 0);
				Vector3 scl = ObjectHelper.GetScale(item, addscl, scltype);
				transform.NJScale(scl);
				// Result
				result = model.CheckHit(Near, Far, Viewport, Projection, View, transform, meshes);
			}
			transform.Pop();
			return result;
		}

		public override List<RenderInfo> Render(SETItem item, Device dev, EditorCamera camera, MatrixStack transform)
		{
			List<RenderInfo> result = new List<RenderInfo>();
			transform.Push();
			// Position
			transform.NJTranslate(item.Position.X + (addxpos ?? 0), item.Position.Y + (addypos ?? 0), item.Position.Z + (addzpos ?? 0));
			// Rotation
			Rotation addrot = new Rotation(addxrot ?? 0, addyrot ?? 0, addzrot ?? 0);
			ObjectHelper.RotateObject(transform, item, addrot, rottype);
			if (model == null)
				result.AddRange(ObjectHelper.RenderQuestionBox(dev, transform, item.Position.ToVector3(), item.Selected));
			else
			{
				// Get textures
				if (texs == null)
				{
					// SA2 multi-textured
					if (texturesmulti.Count > 0)
						texs = ObjectHelper.GetTextures(texturesmulti, texnames, dev);
					// Regular
					texs = ObjectHelper.GetTextures(texture, texnames, dev);
				}
				// Scale
				Vector3 addscl = new Vector3(addxscl ?? 0, addyscl ?? 0, addzscl ?? 0);
				Vector3 scl = ObjectHelper.GetScale(item, addscl, scltype);
				transform.NJScale(scl);
				// Render
				result.AddRange(model.DrawModelTree(dev.GetRenderState<FillMode>(RenderState.FillMode), transform, texs, meshes, EditorOptions.IgnoreMaterialColors, EditorOptions.OverrideLighting));
				// Render selected
				if (item.Selected)
					result.AddRange(model.DrawModelTreeInvert(transform, meshes));
			}
			transform.Pop();
			return result;
		}

		public override List<ModelTransform> GetModels(SETItem item, MatrixStack transform)
		{
			if (model == null) return new List<ModelTransform>();
			List<ModelTransform> result = new List<ModelTransform>();
			transform.Push();
			transform.NJTranslate(item.Position.X + (addxpos ?? 0), item.Position.Y + (addypos ?? 0), item.Position.Z + (addzpos ?? 0));
			Rotation addrot = new Rotation(addxrot ?? 0, addyrot ?? 0, addzrot ?? 0);
			ObjectHelper.RotateObject(transform, item, addrot, rottype);
			Vector3 addscl = new Vector3(addxscl ?? 0, addyscl ?? 0, addzscl ?? 0);
			Vector3 scl = ObjectHelper.GetScale(item, addscl, scltype);
			transform.NJScale(scl);
			result.Add(new ModelTransform(model, transform.Top));
			transform.Pop();
			return result;
		}

		public override BoundingSphere GetBounds(SETItem item)
		{
			MatrixStack transform = new MatrixStack();
			transform.NJTranslate(item.Position.X + (addxpos ?? 0), item.Position.Y + (addypos ?? 0), item.Position.Z + (addzpos ?? 0));
			Rotation addrot = new Rotation(addxrot ?? 0, addyrot ?? 0, addzrot ?? 0);
			ObjectHelper.RotateObject(transform, item, addrot, rottype);
			if (model == null)
				return ObjectHelper.GetQuestionBoxBounds(transform, 1.0f);
			else
			{
				Vector3 addscl = new Vector3(addxscl ?? 0, addyscl ?? 0, addzscl ?? 0);
				Vector3 scl = ObjectHelper.GetScale(item, addscl, scltype);
				transform.NJScale(scl);
				return ObjectHelper.GetModelBounds(model, transform, Math.Max(Math.Max(scl.X, scl.Y), scl.Z));
			}
		}

		public override Matrix GetHandleMatrix(SETItem item)
		{
			Matrix matrix = Matrix.Identity;
			MatrixFunctions.Translate(ref matrix, item.Position);
			MatrixFunctions.RotateObject(ref matrix, item.Rotation);
			//MatrixFunctions.Scale(ref matrix, item.Scale);
			return matrix;
		}

		public override BMPInfo[] ExportTextures(SETItem item)
		{
			return ObjectHelper.GetTextureBmpInfos(texture);
		}

		public override string Name { get { return name; } }
		public override ushort DefaultXRotation { get { return defxrot ?? base.DefaultXRotation; } }
		public override ushort DefaultYRotation { get { return defyrot ?? base.DefaultYRotation; } }
		public override ushort DefaultZRotation { get { return defzrot ?? base.DefaultZRotation; } }
		public override float DefaultXScale { get { return defxscl ?? base.DefaultXScale; } }
		public override float DefaultYScale { get { return defyscl ?? base.DefaultYScale; } }
		public override float DefaultZScale { get { return defzscl ?? base.DefaultZScale; } }
		public override float DistanceFromGround { get { return gnddst ?? base.DistanceFromGround; } }
		public override ushort AddXRotation { get { return addxrot ?? base.AddXRotation; } }
		public override ushort AddYRotation { get { return addyrot ?? base.AddYRotation; } }
		public override ushort AddZRotation { get { return addzrot ?? base.AddZRotation; } }
		public override float AddXScale { get { return addxscl ?? base.AddXScale; } }
		public override float AddYScale { get { return addyscl ?? base.AddYScale; } }
		public override float AddZScale { get { return addzscl ?? base.AddZScale; } }
		public override float AddXPosition { get { return addxpos ?? base.AddXPosition; } }
		public override float AddYPosition { get { return addypos ?? base.AddYPosition; } }
		public override float AddZPosition { get { return addzpos ?? base.AddZPosition; } }
	}
}