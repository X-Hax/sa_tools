using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Buffer = SharpDX.Direct3D11.Buffer;
using Color = SharpDX.Color;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Resource = SharpDX.Direct3D11.Resource;

namespace SonicRetro.SAModel.Direct3D
{
	public partial class Renderer : IDisposable
	{
		static readonly NJS_MATERIAL nullMaterial = new NJS_MATERIAL();

		static readonly BlendOption[] blendModes =
		{
			BlendOption.Zero,
			BlendOption.One,
			BlendOption.SourceColor,
			BlendOption.InverseSourceColor,
			BlendOption.SourceAlpha,
			BlendOption.InverseSourceAlpha,
			BlendOption.DestinationAlpha,
			BlendOption.InverseDestinationAlpha,
		};

		public FlowControl FlowControl;

		CullMode defaultCullMode = CullMode.None;

		public CullMode DefaultCullMode
		{
			get => defaultCullMode;
			set
			{
				defaultCullMode = value;
				ClearDisplayStates();
			}
		}

		public FillMode FillMode { get; set; } // TODO

		readonly Device              device;
		readonly SwapChain           swapChain;
		RenderTargetView             backBuffer;
		Viewport                     viewPort;
		Texture2D                    depthTexture;
		DepthStencilStateDescription depthDesc;
		DepthStencilState            depthStateRW;
		DepthStencilState            depthStateRO;
		DepthStencilView             depthView;
		RasterizerState              rasterizerState;
		RasterizerStateDescription   rasterizerDescription;

		readonly Buffer debugHelperVertexBuffer;
		readonly Buffer perSceneBuffer;
		readonly Buffer perModelBuffer;
		readonly Buffer materialBuffer;

		readonly PerSceneBuffer perSceneData = new PerSceneBuffer();
		readonly PerModelBuffer perModelData = new PerModelBuffer();

		bool zWrite = true;
		bool lastZwrite;

		//readonly MeshsetQueue                       meshQueue     = new MeshsetQueue();
		readonly Dictionary<NJD_FLAG, DisplayState> displayStates = new Dictionary<NJD_FLAG, DisplayState>();

		readonly List<DebugLine>     debugLines     = new List<DebugLine>();
		readonly List<DebugWireCube> debugWireCubes = new List<DebugWireCube>();

		ShaderMaterial lastMaterial;
		VertexShader   vertexShader;
		PixelShader    pixelShader;
		InputLayout    inputLayout;

		VertexShader debugVertexShader;
		PixelShader  debugPixelShader;
		InputLayout  debugInputLayout;

		Buffer          lastVertexBuffer;
		BlendState      lastBlend;
		RasterizerState lastRasterizerState;
		SamplerState    lastSamplerState;
		SceneTexture    lastTexture;

		public Renderer(int w, int h, IntPtr sceneHandle)
		{
			FlowControl.Reset();

			var desc = new SwapChainDescription
			{
				BufferCount       = 1,
				ModeDescription   = new ModeDescription(w, h, new Rational(1000, 60), Format.R8G8B8A8_UNorm),
				Usage             = Usage.RenderTargetOutput,
				OutputHandle      = sceneHandle,
				IsWindowed        = true,
				SampleDescription = new SampleDescription(1, 0)
			};

			var levels = new FeatureLevel[]
			{
				FeatureLevel.Level_11_1,
				FeatureLevel.Level_11_0,
				FeatureLevel.Level_10_1,
				FeatureLevel.Level_10_0,
				FeatureLevel.Level_9_3,
				FeatureLevel.Level_9_2,
				FeatureLevel.Level_9_1,
			};

#if DEBUG
			const DeviceCreationFlags flag = DeviceCreationFlags.Debug;
#else
			const DeviceCreationFlags flag = DeviceCreationFlags.None;
#endif

			Device.CreateWithSwapChain(DriverType.Hardware, flag, levels, desc, out device, out swapChain);

			if (device.FeatureLevel < FeatureLevel.Level_9_1)
			{
				throw new InsufficientFeatureLevelException(device.FeatureLevel, FeatureLevel.Level_9_1);
			}

			var bufferDesc = new BufferDescription(PerSceneBuffer.SizeInBytes.RoundToMultiple(16),
				ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, PerSceneBuffer.SizeInBytes);

			perSceneBuffer = new Buffer(device, bufferDesc);

			bufferDesc = new BufferDescription(PerModelBuffer.SizeInBytes.RoundToMultiple(16),
				ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, PerModelBuffer.SizeInBytes);

			perModelBuffer = new Buffer(device, bufferDesc);

			// Size must be divisible by 16, so this is just padding.
			int size   = Math.Max(ShaderMaterial.SizeInBytes, 80);
			int stride = ShaderMaterial.SizeInBytes + sizeof(uint);

			bufferDesc = new BufferDescription(size,
				ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, stride);

			materialBuffer = new Buffer(device, bufferDesc);

			LoadShaders();
			LoadDebugShaders();

			device.ImmediateContext.VertexShader.SetConstantBuffer(0, perSceneBuffer);
			device.ImmediateContext.PixelShader.SetConstantBuffer(0, perSceneBuffer);

			device.ImmediateContext.VertexShader.SetConstantBuffer(1, materialBuffer);
			device.ImmediateContext.PixelShader.SetConstantBuffer(1, materialBuffer);

			device.ImmediateContext.VertexShader.SetConstantBuffer(2, perModelBuffer);
			device.ImmediateContext.PixelShader.SetConstantBuffer(2, perModelBuffer);


			device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

			int helperBufferSize = DebugWireCube.SizeInBytes.RoundToMultiple(16);

			var debugHelperDescription = new BufferDescription(helperBufferSize, BindFlags.VertexBuffer, ResourceUsage.Dynamic)
			{
				CpuAccessFlags      = CpuAccessFlags.Write,
				StructureByteStride = DebugPoint.SizeInBytes
			};

			debugHelperVertexBuffer = new Buffer(device, debugHelperDescription);

			RefreshDevice(w, h);
		}

		public void LoadShaders()
		{
			using (var includeMan = new DefaultIncludeHandler())
			{
				vertexShader?.Dispose();
				pixelShader?.Dispose();
				inputLayout?.Dispose();

				CompilationResult vs_result = ShaderBytecode.CompileFromFile("Shaders\\scene_vs.hlsl", "main", "vs_4_0", include: includeMan);

				if (vs_result.HasErrors || !string.IsNullOrEmpty(vs_result.Message))
				{
					throw new Exception(vs_result.Message);
				}

				vertexShader = new VertexShader(device, vs_result.Bytecode);

				CompilationResult ps_result = ShaderBytecode.CompileFromFile("Shaders\\scene_ps.hlsl", "main", "ps_4_0", include: includeMan);

				if (ps_result.HasErrors || !string.IsNullOrEmpty(ps_result.Message))
				{
					throw new Exception(ps_result.Message);
				}

				pixelShader = new PixelShader(device, ps_result.Bytecode);

				var layout = new InputElement[]
				{
					new InputElement("POSITION", 0, Format.R32G32B32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
					new InputElement("NORMAL",   0, Format.R32G32B32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
					new InputElement("COLOR",    0, Format.R8G8B8A8_UNorm,  InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
					new InputElement("TEXCOORD", 0, Format.R32G32_Float,    InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0)
				};

				inputLayout = new InputLayout(device, vs_result.Bytecode, layout);

				device.ImmediateContext.VertexShader.Set(vertexShader);
				device.ImmediateContext.PixelShader.Set(pixelShader);
				device.ImmediateContext.InputAssembler.InputLayout = inputLayout;
			}
		}

		public void LoadDebugShaders()
		{
			using (var includeMan = new DefaultIncludeHandler())
			{
				debugVertexShader?.Dispose();
				debugPixelShader?.Dispose();
				debugInputLayout?.Dispose();

				CompilationResult vs_result = ShaderBytecode.CompileFromFile("Shaders\\debug_vs.hlsl", "main", "vs_4_0", include: includeMan);

				if (vs_result.HasErrors || !string.IsNullOrEmpty(vs_result.Message))
				{
					throw new Exception(vs_result.Message);
				}

				debugVertexShader = new VertexShader(device, vs_result.Bytecode);

				CompilationResult ps_result = ShaderBytecode.CompileFromFile("Shaders\\debug_ps.hlsl", "main", "ps_4_0", include: includeMan);

				if (ps_result.HasErrors || !string.IsNullOrEmpty(ps_result.Message))
				{
					throw new Exception(ps_result.Message);
				}

				debugPixelShader = new PixelShader(device, ps_result.Bytecode);

				var layout = new InputElement[]
				{
					new InputElement("POSITION", 0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
					new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0)
				};

				debugInputLayout = new InputLayout(device, vs_result.Bytecode, layout);
			}
		}

		public void Clear()
		{
			if (device == null)
			{
				return;
			}

			meshQueue.Clear();

			device.ImmediateContext.Rasterizer.State = rasterizerState;
			device.ImmediateContext.ClearRenderTargetView(backBuffer, new RawColor4(0.0f, 1.0f, 1.0f, 1.0f));

#if REVERSE_Z
			device.ImmediateContext.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth, 0.0f, 0);
#else
			device.ImmediateContext.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
#endif
		}

		public void Draw(Camera camera, NJS_OBJECT @object)
		{
			while (!(@object is null))
			{
				MatrixStack.Push();
				@object.PushTransform();
				Matrix m = MatrixStack.Peek();
				SetTransform(TransformState.World, in m);

				if (!@object.SkipDraw && @object.Model != null)
				{
					Draw(camera, @object, @object.Model);
				}

				if (!@object.SkipChildren)
				{
					Draw(camera, @object.Child);
				}

				MatrixStack.Pop();
				@object = @object.Sibling;
			}
		}

		public void Draw(Camera camera, NJS_OBJECT @object, NJS_MODEL model)
		{
			foreach (NJS_MESHSET set in model.meshsets)
			{
				meshQueue.Enqueue(this, camera, @object, model, set);
			}
		}

		void DrawSet(NJS_MODEL parent, NJS_MESHSET set)
		{
			List<NJS_MATERIAL> mats = parent.mats;

			ushort       materialId = set.MaterialId;
			NJS_MATERIAL njMaterial = mats.Count > 0 && materialId < mats.Count ? mats[materialId] : nullMaterial;

			FlowControl flowControl = FlowControl;

			if (texturePool.Count < 1)
			{
				if (!FlowControl.UseMaterialFlags)
				{
					FlowControl.Reset();
					FlowControl.UseMaterialFlags = true;
				}

				FlowControl.Set(FlowControl.AndFlags & ~NJD_FLAG.UseTexture, FlowControl.OrFlags);
			}

			ShaderMaterial shaderMaterial = NJS_MODEL.GetSADXMaterial(this, njMaterial);
			SetShaderMaterial(in shaderMaterial);

			DisplayState state = GetSADXDisplayState(njMaterial);

			FlowControl = flowControl;

			if (state.Blend != lastBlend)
			{
				device.ImmediateContext.OutputMerger.SetBlendState(state.Blend);
				lastBlend = state.Blend;
			}

			if (state.Sampler != lastSamplerState)
			{
				device.ImmediateContext.PixelShader.SetSampler(0, state.Sampler);
				lastSamplerState = state.Sampler;
			}

			if (state.Raster != lastRasterizerState)
			{
				device.ImmediateContext.Rasterizer.State = state.Raster;
				lastRasterizerState                      = state.Raster;
			}

			CommitPerModelData();

			if (parent.VertexBuffer != lastVertexBuffer)
			{
				var binding = new VertexBufferBinding(parent.VertexBuffer, Vertex.SizeInBytes, 0);
				device.ImmediateContext.InputAssembler.SetVertexBuffers(0, binding);
				lastVertexBuffer = parent.VertexBuffer;
			}

			device.ImmediateContext.InputAssembler.SetIndexBuffer(set.IndexBuffer, Format.R16_UInt, 0);
			device.ImmediateContext.DrawIndexed(set.IndexCount, 0, 0);
		}

		void CommitPerModelData()
		{
			if (!perModelData.Modified)
			{
				return;
			}

			device.ImmediateContext.MapSubresource(perModelBuffer, MapMode.WriteDiscard,
				MapFlags.None, out DataStream stream);

			using (stream)
			{
				Matrix wvMatrixInvT = perModelData.World.Value * perSceneData.View.Value;
				Matrix.Invert(ref wvMatrixInvT, out wvMatrixInvT);

				perModelData.wvMatrixInvT.Value = wvMatrixInvT;

				stream.Write(perModelData.World.Value);
				stream.Write(wvMatrixInvT);

				Debug.Assert(stream.RemainingLength == 0);
			}

			device.ImmediateContext.UnmapSubresource(perModelBuffer, 0);
			perModelData.Clear();
		}

		void CommitPerSceneData()
		{
			if (!perSceneData.Modified)
			{
				return;
			}

			device.ImmediateContext.MapSubresource(perSceneBuffer, MapMode.WriteDiscard,
				MapFlags.None, out DataStream stream);

			using (stream)
			{
				stream.Write(perSceneData.View.Value);
				stream.Write(perSceneData.Projection.Value);
				stream.Write(perSceneData.CameraPosition.Value);

				//Debug.Assert(stream.RemainingLength == 0);
			}

			perSceneData.Clear();
			device.ImmediateContext.UnmapSubresource(perSceneBuffer, 0);
		}

		public void Present(Camera camera)
		{
			var visibleCount = 0;
			zWrite = true;

			perSceneData.CameraPosition.Value = camera.Position;
			CommitPerSceneData();

			//meshTree.SortOpaque();

			foreach (MeshsetQueueElement e in meshQueue.OpaqueSets)
			{
				++visibleCount;
				DrawMeshsetQueueElement(e);
			}

			if (EnableAlpha && meshQueue.AlphaSets.Count > 1)
			{
				meshQueue.SortAlpha();

				// First draw with depth writes enabled & alpha threshold (in shader)
				foreach (MeshsetQueueElement e in meshQueue.AlphaSets)
				{
					DrawMeshsetQueueElement(e);
				}

				// Now draw with depth writes disabled
				device.ImmediateContext.OutputMerger.SetDepthStencilState(depthStateRO);
				zWrite = false;

				foreach (MeshsetQueueElement e in meshQueue.AlphaSets)
				{
					++visibleCount;
					DrawMeshsetQueueElement(e);
				}

				device.ImmediateContext.OutputMerger.SetDepthStencilState(depthStateRW);
				zWrite = true;
			}

			DrawDebugHelpers();
			meshQueue.Clear();
			swapChain.Present(0, 0);

			if (!MatrixStack.Empty)
			{
				throw new Exception("Matrix stack still contains data");
			}

			if (visibleCount != lastVisibleCount)
			{
				lastVisibleCount = visibleCount;
				Debug.WriteLine(visibleCount);
			}

			lastVertexBuffer = null;
		}

		static void WriteToStream(in DebugPoint point, DataStream stream)
		{
			stream.Write(point.Point);

			Color4 color = point.Color;

			stream.Write(color.Red);
			stream.Write(color.Green);
			stream.Write(color.Blue);
			stream.Write(1.0f);
		}

		static void WriteToStream(in DebugLine line, DataStream stream)
		{
			WriteToStream(in line.PointA, stream);
			WriteToStream(in line.PointB, stream);
		}

		void WriteToStream(in DebugWireCube cube, DataStream stream)
		{
			foreach (var line in cube.Lines)
			{
				WriteToStream(in line, stream);
			}
		}

		void DrawDebugHelpers()
		{
			if (debugLines.Count == 0 && debugWireCubes.Count == 0)
			{
				return;
			}

			CommitPerSceneData();

			device.ImmediateContext.VertexShader.Set(debugVertexShader);
			device.ImmediateContext.PixelShader.Set(debugPixelShader);
			device.ImmediateContext.InputAssembler.InputLayout = debugInputLayout;

			var binding = new VertexBufferBinding(debugHelperVertexBuffer, DebugPoint.SizeInBytes, 0);

			device.ImmediateContext.InputAssembler.SetVertexBuffers(0, binding);
			device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;

			// TODO: make debug z-writes (and tests) configurable
			device.ImmediateContext.OutputMerger.SetDepthStencilState(depthStateRW);
			zWrite = true;

			// using these variables we're able to batch lines into
			// the cube-sized vertex buffer to reduce draw calls.
			int lineCount = debugLines.Count;
			int lineIndex = 0;

			while (lineCount > 0)
			{
				int n = Math.Min(lineCount, 12);
				device.ImmediateContext.MapSubresource(debugHelperVertexBuffer, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);

				using (stream)
				{
					for (int i = 0; i < n; i++)
					{
						DebugLine line = debugLines[lineIndex++];
						WriteToStream(in line, stream);
						--lineCount;
					}
				}

				device.ImmediateContext.UnmapSubresource(debugHelperVertexBuffer, 0);
				device.ImmediateContext.Draw(2 * n, 0);
			}

			foreach (DebugWireCube cube in debugWireCubes)
			{
				device.ImmediateContext.MapSubresource(debugHelperVertexBuffer, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);

				using (stream)
				{
					WriteToStream(in cube, stream);
				}

				device.ImmediateContext.UnmapSubresource(debugHelperVertexBuffer, 0);
				device.ImmediateContext.Draw(24, 0);
			}

			debugWireCubes.Clear();
			debugLines.Clear();

			// restore default shaders, input layout and topology
			device.ImmediateContext.VertexShader.Set(vertexShader);
			device.ImmediateContext.PixelShader.Set(pixelShader);
			device.ImmediateContext.InputAssembler.InputLayout       = inputLayout;
			device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
		}

		void DrawMeshsetQueueElement(MeshsetQueueElement e)
		{
			FlowControl = e.FlowControl;

			Matrix m = e.Transform;
			SetTransform(TransformState.World, in m);

			if (!e.Object.SkipDraw)
			{
				DrawSet(e.Model, e.Set);
			}
		}

		void CreateRenderTarget()
		{
			using (var pBackBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0))
			{
				backBuffer?.Dispose();
				backBuffer = new RenderTargetView(device, pBackBuffer);
			}

			device.ImmediateContext.OutputMerger.SetRenderTargets(backBuffer);
		}

		void SetViewPort(int x, int y, int width, int height)
		{
			viewPort.MinDepth = 0f;
			viewPort.MaxDepth = 1f;

			Viewport vp = viewPort;

			vp.X      = x;
			vp.Y      = y;
			vp.Width  = width;
			vp.Height = height;

			if (vp == viewPort)
			{
				return;
			}

			viewPort = vp;
			device.ImmediateContext.Rasterizer.SetViewport(viewPort);
		}

		public void Draw(IEnumerable<MeshsetQueueElementBase> visible, Camera camera)
		{
			foreach (MeshsetQueueElement e in visible.Select(x => new MeshsetQueueElement(x, camera)))
			{
				meshQueue.Enqueue(e);
			}
		}

		public void RefreshDevice(int w, int h)
		{
			backBuffer?.Dispose();
			swapChain?.ResizeBuffers(1, w, h, Format.Unknown, 0);
			SetViewPort(0, 0, w, h);

			CreateRenderTarget();
			CreateRasterizerState();
			CreateDepthStencil(w, h);
		}

		void CreateDepthStencil(int w, int h)
		{
			// TODO: shader resource?

			var depthBufferDesc = new Texture2DDescription
			{
				Width             = w,
				Height            = h,
				MipLevels         = 1,
				ArraySize         = 1,
				Format            = Format.D24_UNorm_S8_UInt,
				SampleDescription = new SampleDescription(1, 0),
				Usage             = ResourceUsage.Default,
				BindFlags         = BindFlags.DepthStencil,
				CpuAccessFlags    = CpuAccessFlags.None,
				OptionFlags       = ResourceOptionFlags.None
			};

			depthTexture?.Dispose();
			depthTexture = new Texture2D(device, depthBufferDesc);

			depthDesc = new DepthStencilStateDescription
			{
				IsDepthEnabled = true,
				DepthWriteMask = DepthWriteMask.All,
#if REVERSE_Z
				DepthComparison = Comparison.Greater,
#else
				DepthComparison = Comparison.Less,
#endif

				FrontFace = new DepthStencilOperationDescription
				{
					FailOperation = StencilOperation.Keep,
					PassOperation = StencilOperation.Keep,
					Comparison    = Comparison.Always
				},

				BackFace = new DepthStencilOperationDescription
				{
					FailOperation = StencilOperation.Keep,
					PassOperation = StencilOperation.Keep,
					Comparison    = Comparison.Always
				}
			};

			depthStateRW?.Dispose();
			depthStateRW = new DepthStencilState(device, depthDesc);

			depthDesc.DepthWriteMask = DepthWriteMask.Zero;
			depthStateRO?.Dispose();
			depthStateRO = new DepthStencilState(device, depthDesc);

			var depthViewDesc = new DepthStencilViewDescription
			{
				Format    = Format.D24_UNorm_S8_UInt,
				Dimension = DepthStencilViewDimension.Texture2D,
				Texture2D = new DepthStencilViewDescription.Texture2DResource
				{
					MipSlice = 0
				}
			};

			depthView?.Dispose();
			depthView = new DepthStencilView(device, depthTexture, depthViewDesc);

			device?.ImmediateContext.OutputMerger.SetTargets(depthView, backBuffer);
			device?.ImmediateContext.OutputMerger.SetDepthStencilState(depthStateRW);
		}

		void CreateRasterizerState()
		{
			rasterizerDescription = new RasterizerStateDescription
			{
				IsAntialiasedLineEnabled = false,
				CullMode                 = DefaultCullMode,
				DepthBias                = 0,
				DepthBiasClamp           = 0.0f,
				IsDepthClipEnabled       = true,
				FillMode                 = FillMode.Solid,
				IsFrontCounterClockwise  = false,
				IsMultisampleEnabled     = false,
				IsScissorEnabled         = false,
				SlopeScaledDepthBias     = 0.0f
			};

			rasterizerState?.Dispose();
			rasterizerState = new RasterizerState(device, rasterizerDescription);

			device.ImmediateContext.Rasterizer.State = rasterizerState;
		}

		void CopyToTexture(Texture2D texture, Bitmap bitmap, int level)
		{
			BitmapData bmpData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			var        buffer  = new byte[bmpData.Stride * bitmap.Height];
			Marshal.Copy(bmpData.Scan0, buffer, 0, buffer.Length);
			bitmap.UnlockBits(bmpData);

			device.ImmediateContext.UpdateSubresource(buffer, texture, level, 4 * bitmap.Width, 4 * bitmap.Height);
		}

		public void CreateTextureFromBitMap(Bitmap bitmap, Bitmap[] mipmaps, int levels)
		{
			var texDesc = new Texture2DDescription
			{
				ArraySize         = 1,
				BindFlags         = BindFlags.ShaderResource,
				CpuAccessFlags    = CpuAccessFlags.Write,
				Format            = Format.B8G8R8A8_UNorm,
				Width             = bitmap.Width,
				Height            = bitmap.Height,
				MipLevels         = levels,
				Usage             = ResourceUsage.Default,
				SampleDescription = new SampleDescription(1, 0)
			};

			var texture = new Texture2D(device, texDesc);

			if (mipmaps?.Length > 0)
			{
				for (var i = 0; i < levels; i++)
				{
					CopyToTexture(texture, mipmaps[i], i);
				}
			}
			else
			{
				CopyToTexture(texture, bitmap, 0);
			}

			var pair = new SceneTexture(texture, new ShaderResourceView(device, texture));
			texturePool.Add(pair);
		}

		// TODO: generate mipmaps mod loader style
		public void CreateTextureFromBitmapSource(BitmapSource bitmapSource)
		{
			int stride = bitmapSource.Size.Width * 4;
			using (var buffer = new DataStream(bitmapSource.Size.Height * stride, true, true))
			{
				bitmapSource.CopyPixels(stride, buffer);

				var texture = new Texture2D(device, new Texture2DDescription
				{
					Width             = bitmapSource.Size.Width,
					Height            = bitmapSource.Size.Height,
					ArraySize         = 1,
					BindFlags         = BindFlags.ShaderResource,
					Usage             = ResourceUsage.Immutable,
					CpuAccessFlags    = CpuAccessFlags.None,
					Format            = Format.R8G8B8A8_UNorm,
					MipLevels         = 1,
					OptionFlags       = ResourceOptionFlags.None,
					SampleDescription = new SampleDescription(1, 0)
				}, new DataRectangle(buffer.DataPointer, stride));

				texturePool.Add(new SceneTexture(texture, new ShaderResourceView(device, texture)));
			}
		}

		public void ClearTexturePool()
		{
			foreach (SceneTexture texture in texturePool)
			{
				texture.Dispose();
			}

			texturePool.Clear();
			SetTexture(0, -1);
		}

		public Buffer CreateVertexBuffer(IReadOnlyCollection<Vertex> vertices)
		{
			int vertexSize = vertices.Count * Vertex.SizeInBytes;

			var desc = new BufferDescription(vertexSize, BindFlags.VertexBuffer, ResourceUsage.Immutable);

			using (var stream = new DataStream(vertexSize, true, true))
			{
				foreach (Vertex v in vertices)
				{
					stream.Write(v.Position.X);
					stream.Write(v.Position.Y);
					stream.Write(v.Position.Z);

					stream.Write(v.Normal.X);
					stream.Write(v.Normal.Y);
					stream.Write(v.Normal.Z);

					RawColorBGRA color = v.Diffuse ?? Color.White;

					stream.Write(color.B);
					stream.Write(color.G);
					stream.Write(color.R);
					stream.Write(color.A);

					RawVector2 uv = v.UV ?? (RawVector2)Vector2.Zero;

					stream.Write(uv.X);
					stream.Write(uv.Y);
				}

				if (stream.RemainingLength != 0)
				{
					throw new Exception("Failed to fill vertex buffer.");
				}

				stream.Position = 0;
				return new Buffer(device, stream, desc);
			}
		}

		public Buffer CreateIndexBuffer(IEnumerable<short> indices, int sizeInBytes)
		{
			var desc = new BufferDescription(sizeInBytes, BindFlags.IndexBuffer, ResourceUsage.Immutable);

			using (var stream = new DataStream(sizeInBytes, true, true))
			{
				foreach (short i in indices)
				{
					stream.Write(i);
				}

				if (stream.RemainingLength != 0)
				{
					throw new Exception("Failed to fill index buffer.");
				}

				stream.Position = 0;
				return new Buffer(device, stream, desc);
			}
		}

		public void SetTransform(TransformState state, in Matrix m)
		{
			switch (state)
			{
				case TransformState.World:
					perModelData.World.Value = m;
					break;
				case TransformState.View:
					perSceneData.View.Value = m;
					break;
				case TransformState.Projection:
#if REVERSE_Z
					var a = new Matrix(
						1f, 0f,  0f, 0f,
						0f, 1f,  0f, 0f,
						0f, 0f, -1f, 0f,
						0f, 0f,  1f, 1f
					);

					perSceneData.Projection.Value = rawMatrix * a;
#else
					perSceneData.Projection.Value = m;
#endif
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(state), state, null);
			}
		}

		public void SetTexture(int sampler, SceneTexture texture)
		{
			if (lastTexture == texture)
			{
				return;
			}

			device.ImmediateContext.PixelShader.SetShaderResource(sampler, texture?.ShaderResource);
			lastTexture = texture;
		}

		// TODO: generate in bulk
		public DisplayState GetSADXDisplayState(NJS_MATERIAL material)
		{
			// Not implemented in SADX:
			// - NJD_FLAG.Pick
			// - NJD_FLAG.UseAnisotropic
			// - NJD_FLAG.UseFlat

			const NJD_FLAG state_mask = NJD_FLAG.ClampU | NJD_FLAG.ClampV | NJD_FLAG.FlipU | NJD_FLAG.FlipV
			                            | NJD_FLAG.DoubleSide | NJD_FLAG.UseAlpha
			                            | (NJD_FLAG)0xFC000000 /* blend modes */;

			NJD_FLAG flags = FlowControl.Apply(material.attrflags) & state_mask;

			if (displayStates.TryGetValue(flags, out DisplayState state))
			{
				return state;
			}

			var samplerDesc = new SamplerStateDescription
			{
				AddressW           = TextureAddressMode.Wrap,
				Filter             = Filter.MinMagMipLinear,
				MinimumLod         = -float.MaxValue,
				MaximumLod         = float.MaxValue,
				MaximumAnisotropy  = 1,
				ComparisonFunction = Comparison.Never
			};

			if ((flags & (NJD_FLAG.ClampU | NJD_FLAG.FlipU)) == (NJD_FLAG.ClampU | NJD_FLAG.FlipU))
			{
				samplerDesc.AddressU = TextureAddressMode.MirrorOnce;
			}
			else if ((flags & NJD_FLAG.ClampU) != 0)
			{
				samplerDesc.AddressU = TextureAddressMode.Clamp;
			}
			else if ((flags & NJD_FLAG.FlipU) != 0)
			{
				samplerDesc.AddressU = TextureAddressMode.Mirror;
			}
			else
			{
				samplerDesc.AddressU = TextureAddressMode.Wrap;
			}

			if ((flags & (NJD_FLAG.ClampV | NJD_FLAG.FlipV)) == (NJD_FLAG.ClampV | NJD_FLAG.FlipV))
			{
				samplerDesc.AddressV = TextureAddressMode.MirrorOnce;
			}
			else if ((flags & NJD_FLAG.ClampV) != 0)
			{
				samplerDesc.AddressV = TextureAddressMode.Clamp;
			}
			else if ((flags & NJD_FLAG.FlipV) != 0)
			{
				samplerDesc.AddressV = TextureAddressMode.Mirror;
			}
			else
			{
				samplerDesc.AddressV = TextureAddressMode.Wrap;
			}

			var sampler = new SamplerState(device, samplerDesc) { DebugName = $"Sampler: {flags.ToString()}" };

			// Base it off of the default rasterizer state.
			RasterizerStateDescription rasterDesc = rasterizerDescription;

			rasterDesc.CullMode = (flags & NJD_FLAG.DoubleSide) != 0 ? CullMode.None : DefaultCullMode;

			var raster = new RasterizerState(device, rasterDesc) { DebugName = $"Rasterizer: {flags.ToString()}" };

			var                              blendDesc = new BlendStateDescription();
			ref RenderTargetBlendDescription rt        = ref blendDesc.RenderTarget[0];

			rt.IsBlendEnabled        = (flags & NJD_FLAG.UseAlpha) != 0;
			rt.SourceBlend           = blendModes[material.SourceBlend];
			rt.DestinationBlend      = blendModes[material.DestinationBlend];
			rt.BlendOperation        = BlendOperation.Add;
			rt.SourceAlphaBlend      = blendModes[material.SourceBlend];
			rt.DestinationAlphaBlend = blendModes[material.DestinationBlend];
			rt.AlphaBlendOperation   = BlendOperation.Add;
			rt.RenderTargetWriteMask = ColorWriteMaskFlags.All;

			var blend = new BlendState(device, blendDesc) { DebugName = $"Blend: {flags.ToString()}" };

			var result = new DisplayState(sampler, raster, blend);
			displayStates[flags] = result;

			return result;
		}

		public void SetShaderMaterial(in ShaderMaterial material)
		{
			if (material == lastMaterial && zWrite == lastZwrite)
			{
				return;
			}

			lastMaterial = material;
			lastZwrite   = zWrite;

			device.ImmediateContext.MapSubresource(materialBuffer, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);

			using (stream)
			{
				stream.Write(material.Diffuse);
				stream.Write(material.Specular);
				stream.Write(material.Exponent);
				stream.Write(material.UseLight ? 1 : 0);
				stream.Write(material.UseAlpha ? 1 : 0);
				stream.Write(material.UseEnv ? 1 : 0);
				stream.Write(material.UseTexture ? 1 : 0);
				stream.Write(material.UseSpecular ? 1 : 0);
				stream.Write(zWrite ? 1 : 0);
			}

			device.ImmediateContext.UnmapSubresource(materialBuffer, 0);
		}

		public void Dispose()
		{
			swapChain?.Dispose();
			backBuffer?.Dispose();
			depthTexture?.Dispose();
			depthStateRW?.Dispose();
			depthStateRO?.Dispose();
			depthView?.Dispose();
			rasterizerState?.Dispose();
			materialBuffer?.Dispose();
			lastVertexBuffer?.Dispose();
			lastBlend?.Dispose();
			lastRasterizerState?.Dispose();
			lastSamplerState?.Dispose();
			lastTexture?.Dispose();
			vertexShader?.Dispose();
			pixelShader?.Dispose();
			debugVertexShader?.Dispose();
			debugPixelShader?.Dispose();
			inputLayout?.Dispose();
			debugInputLayout?.Dispose();
			debugHelperVertexBuffer?.Dispose();

			perSceneBuffer?.Dispose();
			perModelBuffer?.Dispose();
			materialBuffer?.Dispose();

			ClearTexturePool();
			ClearDisplayStates();

#if DEBUG
			using (var debug = new DeviceDebug(device))
			{
				debug.ReportLiveDeviceObjects(ReportingLevel.Summary);
			}
#endif

			device?.Dispose();
		}

		void ClearDisplayStates()
		{
			foreach (KeyValuePair<NJD_FLAG, DisplayState> i in displayStates)
			{
				i.Value.Dispose();
			}

			displayStates.Clear();
		}

		public void DrawDebugLine(DebugPoint start, DebugPoint end)
		{
			DrawDebugLine(new DebugLine(start, end));
		}

		public void DrawDebugLine(DebugLine line)
		{
			debugLines.Add(line);
		}

		public void DrawBounds(in BoundingBox bounds, Color4 color)
		{
			debugWireCubes.Add(new DebugWireCube(in bounds, color));
		}

		public void SetMaterial(NJS_MATERIAL material)
		{
			var shaderMaterial = material?.ToShaderMaterial() ?? nullMaterial.ToShaderMaterial();
			SetShaderMaterial(shaderMaterial);
		}
	}

	class InsufficientFeatureLevelException : Exception
	{
		public readonly FeatureLevel SupportedLevel;
		public readonly FeatureLevel TargetLevel;

		public InsufficientFeatureLevelException(FeatureLevel supported, FeatureLevel target)
		{
			SupportedLevel = supported;
			TargetLevel    = target;
		}
	}
}