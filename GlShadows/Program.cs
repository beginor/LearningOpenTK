﻿using System;
using System.Drawing.Imaging;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GlPixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace GlShadows {

	class Program : GameWindow {

		float _angle;

		readonly uint[] _texture = new uint[2];

		static void FreeTexture(uint texture) {
			GL.DeleteTextures(1, ref texture);
		}

		static uint LoadTexture(string filename) {
			uint texture;
			GL.GenTextures(1, out texture);
			GL.BindTexture(TextureTarget.Texture2D, texture);
			GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);

			using (var bmp = new Bitmap(filename)) {
				var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, bmp.Width, bmp.Height, 0, GlPixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
				bmp.UnlockBits(data);
			}

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

			return texture;
		}

		void Square() {
			GL.PushMatrix();
			GL.BindTexture(TextureTarget.Texture2D, _texture[0]);
			GL.Translate(0, 2.5F, 0);
			GL.Scale(2, 2, 2);
			GL.Begin(BeginMode.Quads);
			GL.TexCoord2(1, 0);
			GL.Vertex3(-1, -1, 0);
			GL.TexCoord2(1, 1);
			GL.Vertex3(-1, 1, 0);
			GL.TexCoord2(0, 1);
			GL.Vertex3(1, 1, 0);
			GL.TexCoord2(0, 0);
			GL.Vertex3(1, -1, 0);
			GL.End();
			GL.PopMatrix();
		}

		void Bench () {
			GL.PushMatrix();
			GL.Color4(1,1,1,0.7);
			GL.BindTexture(TextureTarget.Texture2D, _texture[1]);
			GL.Translate(0,-2.5,0);
			GL.Scale(4,2,4);
			GL.Begin(BeginMode.Quads);
			GL.TexCoord2(1,0);
			GL.Vertex3(-1,-1,1);
			GL.TexCoord2(1,1);
			GL.Vertex3(-1,1,-0.5);
			GL.TexCoord2(0,1);
			GL.Vertex3(1,1,-0.5);
			GL.TexCoord2(0,0);
			GL.Vertex3(1,-1,1);
			GL.End();
			GL.PopMatrix();
		}

		void Display() {
			GL.ClearStencil(0);
			GL.ClearDepth(1);
			GL.ClearColor(1.0F, 1.0F, 1.0F, 1.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

			GL.LoadIdentity();
			GL.Translate(0, 0, -10);
			
			//start
			//disable the color mask
			GL.ColorMask(false, false, false, false);
			//disable the depth mask
			GL.DepthMask(false);
			//enable the stencil testing
			GL.Enable(EnableCap.StencilTest);

			//set the stencil buffer to replace our next lot of data
			GL.StencilFunc(StencilFunction.Always, 1, 0xFFFFFFFF);
			GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);
			//set the data plane to be replaced
			Bench();

			//enable the color mask
			GL.ColorMask(true, true, true, true);
			//enable the depth mask
			GL.DepthMask(true);

			//set the stencil buffer to keep our next lot of data
			GL.StencilFunc(StencilFunction.Equal, 1, 0xFFFFFFFF);
			GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);

			//disable texturing of the shadow
			GL.Disable(EnableCap.Texture2D);
			//disable depth testing of the shadow
			GL.Disable(EnableCap.DepthTest);
			GL.PushMatrix();
			//flip the shadow vertically
			GL.Scale(1.0F, -1.0F, 1.0f);
			//translate the shadow onto our drawing plane
			GL.Translate(0, 2, 0);
			//rotate the shadow accordingly
			GL.Rotate(_angle, 0, 1, 0);
			//color the shadow black
			GL.Color4(0, 0, 0, 1);
			//draw our square as the shadow
			Square();
			GL.PopMatrix();
			//enable depth testing
			GL.Enable(EnableCap.Texture2D);
			//enable texturing
			GL.Enable(EnableCap.DepthTest);
			//disable the stencil testing
			GL.Disable(EnableCap.StencilTest);
			//end

			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			this.Bench();
			GL.Disable(EnableCap.Blend);
			GL.Rotate(_angle, 0, 1, 0);
			this.Square();

			this.SwapBuffers();
			this._angle++;
		}

		void Init() {
			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Lequal);
			GL.ShadeModel(ShadingModel.Smooth);
			GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
			GL.Enable(EnableCap.Texture2D);
			this._texture[0] = LoadTexture("brick33.jpg");
			this._texture[1] = LoadTexture("water06.jpg");
		}

		void Reshape() {
			GL.Viewport(this.ClientRectangle);
			GL.MatrixMode(MatrixMode.Projection);
			Matrix4 projMatrix = Matrix4.CreatePerspectiveFieldOfView(
				MathHelper.PiOver2,
				this.ClientRectangle.Width/(float)this.ClientRectangle.Height,
				1.0F,
				60F
			);
			GL.LoadMatrix(ref projMatrix);
			GL.MatrixMode(MatrixMode.Modelview);
		}

		public override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			this.Init();
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			this.Reshape();
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			base.OnRenderFrame(e);
			this.Display();
		}

		public override void OnUnload(EventArgs e) {
			base.OnUnload(e);
			FreeTexture(this._texture[0]);
			FreeTexture(this._texture[1]);
		}

		static void Main() {
			using (var prog = new Program()) {
				prog.Run(30);
			}
		}
	}
}