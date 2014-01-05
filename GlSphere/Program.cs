using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Examples;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GlPixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using Examples.Shapes;

namespace GlSphere {

	class Program : GameWindow {

		uint[] _textures = new uint[1];

		UvSphere _sphere;

		float _angle;

		Examples.Camera _camera;

		void DisplaySphere(double r, uint texture) {
			//GL.PushMatrix();
			//GL.LoadIdentity();
			GL.BindTexture(TextureTarget.Texture2D, texture);
			GL.Begin(BeginMode.Quads);
			foreach (var index in _sphere.Indices) {
				GL.TexCoord2(_sphere.Texcoords[index]);
				GL.Vertex3(_sphere.Vertices[index]);
			}
			GL.End();
			//GL.PopMatrix();
		}

		void CreateSphere(double r, double h, double k, double z) {
			this._sphere = new UvSphere((float)r);
		}

		void DisplayXyz() {
			GL.LineWidth(4.0F);
			GL.Begin(BeginMode.Lines);
			GL.Color3(Color.Red);
			GL.Vertex3(-1000, 0, 0);
			GL.Vertex3(1000, 0, 0);
			GL.Color3(Color.Green);
			GL.Vertex3(0, -1000, 0);
			GL.Vertex3(0, 1000, 0);
			GL.Color3(Color.Blue);
			GL.Vertex3(0, 0, -1000);
			GL.Vertex3(0, 0, 1000);
			GL.End();
			GL.Color4(Color.Transparent);
			GL.LineWidth(0.0F);
		}

		void Display() {
			//GL.ClearDepth(1);
			GL.ClearColor(Color.MidnightBlue);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.MatrixMode(MatrixMode.Modelview);
			var viewMatrix = _camera.ViewMatrix4;
			GL.LoadMatrix(ref viewMatrix);

			GL.Rotate(_angle / 5, Vector3.UnitY);
			GL.Rotate(_angle / 3, Vector3.UnitX);
			//this.DisplayXyz();
			this.DisplaySphere(5, this._textures[0]);

			this.SwapBuffers();
			_angle++;
		}

		void Init() {
			GL.Enable(EnableCap.Texture2D);
			GL.Enable(EnableCap.DepthTest);
			GL.DepthFunc(DepthFunction.Lequal);
			GL.Enable(EnableCap.CullFace);
			GL.CullFace(CullFaceMode.Back);
			GL.FrontFace(FrontFaceDirection.Cw);
			_textures[0] = LoadTexture("earth.bmp");
			GL.Enable(EnableCap.Fog);
			GL.Fog(FogParameter.FogMode, (int)FogMode.Exp2);
			GL.Fog(FogParameter.FogDensity, 0.022F);
			GL.Fog(FogParameter.FogColor, new[] { 0.5F, 0.5F, 0.5F, 1.0F });
			this.CreateSphere(60, 0, 0, 0);
		}

		void Reshape() {
			if (_camera == null) {
				_camera = new Camera {
					FarPlane = 1000.0F,
					NearPlane = 1.0F,
					FieldOfView = MathHelper.PiOver2,
					Target = Vector3.Zero,
					Location = new Vector3(90, 0, 0)
				};
			}

			GL.Viewport(this.ClientRectangle);
			_camera.AspectRatio = this.ClientRectangle.Width / (float)this.ClientRectangle.Height;

			GL.MatrixMode(MatrixMode.Projection);
			Matrix4 projMatrix = _camera.ProjectionMatrix4;
			GL.LoadMatrix(ref projMatrix);
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

		static void FreeTexture(uint texture) {
			GL.DeleteTextures(1, ref texture);
		}

		public override void OnUnload(EventArgs e) {
			base.OnUnload(e);
			FreeTexture(this._textures[0]);
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

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

			return texture;
		}


		static void Main(string[] args) {
			using (var prog = new Program()) {
				prog.Run(30);
			}
		}
	}
}
