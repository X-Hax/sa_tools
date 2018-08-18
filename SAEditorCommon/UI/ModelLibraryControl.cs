using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SonicRetro.SAModel.Direct3D;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace SonicRetro.SAModel.SAEditorCommon.UI
{
	/// <summary>
	/// Manages a list of attach objects, as well as provides a visual interface for allowing users to select them.
	/// </summary>
	public partial class ModelLibraryControl : UserControl
	{
		#region Events
		public delegate void LibrarySelectHandler(ModelLibraryControl sender, int selectionIndex, Attach selectedModel);
		public event LibrarySelectHandler SelectionChanged;
		#endregion

		#region Model Tracking Variables
		private List<KeyValuePair<int, Attach>> modelList; // we keep track of these so that NJS_OBJECTS can have their models swapped easily. This also lets users import their own meshes easily as well.
		private NJS_OBJECT njs_object; // we use this as a simple container for our attach
		private bool suppressingListUpateEvents = false;
		private bool listChangedWhileSuppressed = false;
		#endregion

		// state tracking variables
		private int selectedModelIndex = -1;
		public Attach SelectedModel
        {
            get { return (selectedModelIndex >= 0) ? modelList[selectedModelIndex].Value : null; }
            set 
		    {
                if (!IsDesignMode())
                {
                    selectedModelIndex = -1;
                    selectedModelIndex = modelList.FindIndex(item => item.Key == value.GetHashCode());

                    if (selectedModelIndex >= 0)
                    {
                        modelListView.SelectedItems.Clear();
                        modelListView.Items[selectedModelIndex].Selected = true;
                        modelListView.Select();
                    }
                    else
                    {
                        modelListView.SelectedItems.Clear();
                    }
                }
		    }
		}

		#region Rendering Variables
		private Bitmap renderFailureBitmap;
		private List<KeyValuePair<int, Bitmap>> attachListRenders; // list of meshes rendered to texture so that users can select them as buttons
		internal Device d3dDevice;
		//private Microsoft.DirectX.Direct3D.Font onscreenFont;
		private int panSpeed = 0x01;
		//private Sprite textSprite;
		private Point screenCenter;
		private Texture screenRenderTexture;
		private Surface defaultRenderTarget;
		private FillMode renderFillMode = FillMode.Solid;
		private Cull renderCullMode = Cull.None;
		private float renderDrawDistance = 3500f;
		private EditorCamera defaultCam; // the default camera is used for rendering the textures
		private EditorCamera panelCam; // the panel cam is user-controllable and can show an object from any angle.
		private List<Mesh> meshes;
		#endregion

		#region Initialization / Construction Methods
		public ModelLibraryControl()
		{
			InitializeComponent();

            if (!IsDesignMode())
            {

                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
                modelList = new List<KeyValuePair<int, Attach>>();
                attachListRenders = new List<KeyValuePair<int, Bitmap>>();
                meshes = new List<Mesh>();
                njs_object = new NJS_OBJECT();

                modelListView.LargeImageList = new ImageList();
                modelListView.HideSelection = false;

                panelCam = new EditorCamera(1000) { mode = 1, MoveSpeed = EditorCamera.DefaultMoveSpeed };
                defaultCam = new EditorCamera(1000) { mode = 1, MoveSpeed = EditorCamera.DefaultMoveSpeed };

                splitContainer1.Panel2.MouseWheel += Panel2_MouseWheel;
            }
		}

		public void InitRenderer()
		{
			d3dDevice = new Device(0, DeviceType.Hardware, splitContainer1.Panel2.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters[] { new PresentParameters() { Windowed = true, SwapEffect = SwapEffect.Discard, EnableAutoDepthStencil = true, AutoDepthStencilFormat = DepthFormat.D24X8 } });
			defaultRenderTarget = d3dDevice.GetRenderTarget(0);
			screenCenter = new Point(splitContainer1.Panel2.Width / 2, splitContainer1.Panel2.Height / 2);
			//textSprite = new Sprite(d3dDevice);

			renderFailureBitmap = new Bitmap(2, 2, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			for (int h = 0; h < 2; h++ )
			{
				for (int v = 0; v < 2; v++) renderFailureBitmap.SetPixel(h, v, Color.Purple);
			}

			#region Key Light
				d3dDevice.Lights[0].Type = LightType.Directional;
			d3dDevice.Lights[0].DiffuseColor = new ColorValue(180, 172, 172, 255);
			d3dDevice.Lights[0].Diffuse = Color.FromArgb(255, 180, 172, 172);
			d3dDevice.Lights[0].Ambient = Color.Black;
			d3dDevice.Lights[0].Specular = Color.White;
			d3dDevice.Lights[0].Range = 0;
			d3dDevice.Lights[0].Direction = Vector3.Normalize(new Vector3(-0.245f, -1, 0.125f));
			d3dDevice.Lights[0].Enabled = true;
			#endregion

			#region Fill Light
			d3dDevice.Lights[1].Type = LightType.Directional;
			d3dDevice.Lights[1].DiffuseColor = new ColorValue(132, 132, 132, 255);
			d3dDevice.Lights[1].Diffuse = Color.FromArgb(255, 132, 132, 132);
			d3dDevice.Lights[1].Ambient = Color.Black;
			d3dDevice.Lights[1].Specular = Color.Gray;
			d3dDevice.Lights[1].Range = 0;
			d3dDevice.Lights[1].Direction = Vector3.Normalize(new Vector3(0.245f, -0.4f, -0.125f));
			d3dDevice.Lights[1].Enabled = true;
			#endregion

			#region Back Light
			d3dDevice.Lights[2].Type = LightType.Directional;
			d3dDevice.Lights[2].DiffuseColor = new ColorValue(100, 100, 100, 255);
			d3dDevice.Lights[2].Diffuse = Color.FromArgb(255, 130, 142, 130);
			d3dDevice.Lights[2].Ambient = Color.Black;
			d3dDevice.Lights[2].Specular = Color.Gray;
			d3dDevice.Lights[2].Range = 0;
			d3dDevice.Lights[2].Direction = Vector3.Normalize(new Vector3(-0.45f, 1f, 0.25f));
			d3dDevice.Lights[2].Enabled = true;
			#endregion

			#region Setting Renderer States
			d3dDevice.SamplerState[0].MinFilter = TextureFilter.Anisotropic;
			d3dDevice.SamplerState[0].MagFilter = TextureFilter.Point;
			d3dDevice.SamplerState[0].MipFilter = TextureFilter.None;
			d3dDevice.RenderState.Lighting = true;
			d3dDevice.RenderState.SpecularEnable = true;
			d3dDevice.RenderState.Ambient = Color.Black;
			d3dDevice.RenderState.AlphaBlendEnable = false;
			d3dDevice.RenderState.BlendOperation = BlendOperation.Add;
			d3dDevice.RenderState.DestinationBlend = Blend.InvSourceAlpha;
			d3dDevice.RenderState.SourceBlend = Blend.SourceAlpha;
			d3dDevice.RenderState.AlphaTestEnable = true;
			d3dDevice.RenderState.AlphaFunction = Compare.Greater;
			d3dDevice.RenderState.AmbientMaterialSource = ColorSource.Material;
			d3dDevice.RenderState.DiffuseMaterialSource = ColorSource.Material;
			d3dDevice.RenderState.SpecularMaterialSource = ColorSource.Material;
			d3dDevice.TextureState[0].AlphaOperation = TextureOperation.BlendDiffuseAlpha;
			d3dDevice.RenderState.ColorVertex = true;
			d3dDevice.RenderState.ZBufferEnable = true;
			#endregion

			#region Setup Font
			//onscreenFont = new Microsoft.DirectX.Direct3D.Font(d3dDevice, 14, 14, FontWeight.DoNotCare, 0, false, CharacterSet.Oem, Precision.Default, FontQuality.Default, PitchAndFamily.FamilyDoNotCare, "Verdana");
			#endregion
		}
        #endregion

        #region Cleanup / Disposal Methods
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if(disposing)
            {
                if(renderFailureBitmap != null) renderFailureBitmap.Dispose();
                //if(textSprite != null) textSprite.Dispose();
                if(screenRenderTexture != null) screenRenderTexture.Dispose();
                if(defaultRenderTarget != null) defaultRenderTarget.Dispose();
                //if(onscreenFont != null) onscreenFont.Dispose();
                if(d3dDevice != null) d3dDevice.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion

        #region State Management Methods
        public void Clear()
		{
			modelList.Clear();
			attachListRenders.Clear();

			modelListView.Clear();
			selectedModelIndex = -1;
			RenderModel(selectedModelIndex, false);
		}

		/// <summary>
		/// Call this if you wish to add multiple models to the library in a short period of time. It will suppress events that could cause re-rendering or other heavy loads.
		/// </summary>
		public void BeginUpdate()
		{
			suppressingListUpateEvents = true;
			listChangedWhileSuppressed = false;
		}

		public void Add(Attach model)
		{
			bool foundMatch = false;
			foundMatch = modelList.Exists(item => item.Key == model.GetHashCode());

			if (!foundMatch)
			{
				modelList.Add(new KeyValuePair<int, Attach>(model.GetHashCode(), model));
				attachListRenders.Add(new KeyValuePair<int, Bitmap>(model.GetHashCode(), renderFailureBitmap));
				meshes.Add(model.CreateD3DMesh(d3dDevice));

				if (Visible) RenderModel(modelList.Count - 1, true); // todo: I think these Add calls are getting called while a separate thread is open, and thus causing locking issues
				// perhaps we could find a way to defer all of this until after the file load operation is complete

				if (!suppressingListUpateEvents)
				{
					if (Visible) PopulateListView();
				}
				else
				{
					listChangedWhileSuppressed = true;
				}
			}
		}

		private void PopulateListView()
		{
			modelListView.Clear();

			modelListView.LargeImageList.ImageSize = new System.Drawing.Size(128, 128);

			for (int i = 0; i < modelList.Count; i++)
			{
				int renderIndex = -1;
				renderIndex = attachListRenders.FindIndex(item => item.Key == modelList[i].Key);
				modelListView.LargeImageList.Images.Add((renderIndex >= 0) ? attachListRenders[renderIndex].Value : renderFailureBitmap);
				modelListView.Items.Add(new ListViewItem { Name = modelList[i].Value.Name, ImageIndex = i, Text = modelList[i].Value.Name, });
			}
		}

		/// <summary>
		/// Call this once you're done adding a mass of models to the library. It will immediately send an itemlistchanged event if the list changed during the update, however it will
		/// only send the message once instead of once for each item added. That's the entire idea behind the message suppression.
		/// </summary>
		public void EndUpdate()
		{
			if (listChangedWhileSuppressed)
			{
				if (Visible) PopulateListView();
			}
			suppressingListUpateEvents = false;
		}

        private bool IsDesignMode()
        {
            //return LicenseManager.UsageMode == LicenseUsageMode.Designtime;

            if (Application.ExecutablePath.IndexOf("devenv.exe", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return true;
            }
            return false;
        }
		#endregion

		#region Rendering Methods
		private void RenderModel(int modelToRender, bool renderToTexture = false)
		{
			EditorCamera camera = ((renderToTexture) ? defaultCam : panelCam);

			if (renderToTexture && (modelToRender <= 0)) // we're using our default camera, set it to a position that will show the entire model
			{
				camera.Position = new Vector3(0, 0, -modelList[modelToRender].Value.Bounds.Radius * 1.45f);
			}

			d3dDevice.SetTransform(TransformType.Projection, Matrix.PerspectiveFovRH((float)(Math.PI / 4), 1, 1, camera.DrawDistance));
			d3dDevice.SetTransform(TransformType.View, camera.ToMatrix());
			UpdateTitleBar(camera);
			d3dDevice.RenderState.FillMode = EditorOptions.RenderFillMode;
			d3dDevice.RenderState.CullMode = EditorOptions.RenderCullMode;
			d3dDevice.Material = new Material { Ambient = Color.White };
			d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Gray.ToArgb(), 1, 0);
			d3dDevice.RenderState.ZBufferEnable = true;
			if (renderToTexture)
			{
				if (screenRenderTexture == null) // we can't render onto a null surface
				{
					screenRenderTexture = new Texture(d3dDevice, 256, 256, 0, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
				}

				d3dDevice.SetRenderTarget(0, screenRenderTexture.GetSurfaceLevel(0));
			}
			else d3dDevice.SetRenderTarget(0, defaultRenderTarget);
			d3dDevice.BeginScene();
			//all drawings after this line

			if (modelToRender >= 0)
			{
				MatrixStack transform = new MatrixStack();
				njs_object.Attach = modelList[modelToRender].Value;
				RenderInfo.Draw(njs_object.DrawModel(d3dDevice, transform, null, meshes[modelToRender], true), d3dDevice, camera);
			}
			else // invalid selection, show a message telling the user to select something
			{
                // see if we can hide the panel's picture and/or draw an actual label here
				//onscreenFont.DrawText(textSprite, "No model selected.", screenCenter, Color.Black);
			}

			d3dDevice.EndScene(); //all drawings before this line
			d3dDevice.Present();

			if(renderToTexture)
			{
				int renderIndex = -1;
				renderIndex = attachListRenders.FindIndex(item => item.Key == modelList[modelToRender].Key);

				// convert our texture into a bitmap, add it to the rendertextures list
				Surface surface = screenRenderTexture.GetSurfaceLevel(0);
				GraphicsStream gs = SurfaceLoader.SaveToStream(ImageFileFormat.Bmp, surface);
				attachListRenders[renderIndex] = new KeyValuePair<int, Bitmap>(modelList[modelToRender].Key, new Bitmap(gs));
			}
		}

		private void RenderAllModels()
		{
			for (int i = 0; i < modelList.Count; i++) RenderModel(i, true);
		}
		#endregion

		private void UpdateTitleBar(EditorCamera camera)
		{
			Text = "Model Library:" + "X=" + camera.Position.X + " Y=" + camera.Position.Y + " Z=" + camera.Position.Z + " Pitch=" + camera.Pitch.ToString("X") + " Yaw=" + camera.Yaw.ToString("X") + " Speed=" + camera.MoveSpeed.ToString();
		}

		private void SetPanelCam()
		{
			if(selectedModelIndex >= 0) panelCam.Position = new Vector3(0, 0, -modelList[selectedModelIndex].Value.Bounds.Radius * 1.54f);
		}

		private void modelListView_SelectedIndexChanged(object sender, EventArgs e)
		{
            selectedModelIndex = (modelListView.SelectedIndices.Count > 0) ? modelListView.SelectedIndices[0] : -1;

			SetPanelCam();
			RenderModel(selectedModelIndex, false);

			if (SelectionChanged != null) SelectionChanged(this, selectedModelIndex, SelectedModel);
		}

        private void ModelLibraryControl_VisibleChanged(object sender, EventArgs e)
        {
            if (!IsDesignMode())
            {

            }
        }

        public void FullReRender()
        {
            RenderAllModels();
            PopulateListView();
            RenderModel(selectedModelIndex, false);
        }

        private void ModelLibraryControl_Resize(object sender, EventArgs e)
        {
            if (!IsDesignMode())
            {
                if(d3dDevice != null) RenderModel(selectedModelIndex, false);
            }
        }

        #region Input Event Methods
        bool zoomKeyDown = false, lookKeyDown = false;
		Point mouseLast;
		private void splitContainer1_Panel2_MouseMove(object sender, MouseEventArgs e)
		{
			Point mouseEvent = e.Location;
			if (mouseLast == Point.Empty)
			{
				mouseLast = mouseEvent;
				return;
			}

			Point mouseDelta = mouseEvent - (Size)mouseLast;

			if (panelCam.mode == 1)
			{
				if (zoomKeyDown)
				{
					panelCam.Distance += (mouseDelta.Y * panelCam.MoveSpeed) * 3;
				}
				else if (lookKeyDown)
				{
					panelCam.Yaw = unchecked((ushort)(panelCam.Yaw - mouseDelta.X * panSpeed));
					panelCam.Pitch = unchecked((ushort)(panelCam.Pitch - mouseDelta.Y * panSpeed));
				}
			}

			RenderModel(selectedModelIndex, false);
		}

		private void splitContainer1_Panel2_MouseDown(object sender, MouseEventArgs e)
		{
			splitContainer1.Panel2.Focus();
			if (e.Button == System.Windows.Forms.MouseButtons.Left) lookKeyDown = true;
		}

		void Panel2_MouseWheel(object sender, MouseEventArgs e)
		{
			float detentValue = 4;

			if (e.Delta > 0) detentValue *= -1;

			if (panelCam.mode == 0)
				panelCam.Position += panelCam.Look * (detentValue * panelCam.MoveSpeed);
			else if (panelCam.mode == 1)
				panelCam.Distance += (detentValue * panelCam.MoveSpeed);

			RenderModel(selectedModelIndex, false);
		}

        private void splitContainer1_Panel2_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left) lookKeyDown = false;
		}
		#endregion
	}
}
