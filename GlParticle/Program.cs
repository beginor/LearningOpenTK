using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text;
using Examples.Shapes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using GlPixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace GlParticle {

	class Program : GameWindow {

		uint[] _textures = new uint[10];

		const int ParticleCount = 500;

		class Particle {

			public Vector3 Pos;

			public Vector3 Mov;

			public Color Color;

			public double Direction;
			public double Acceleration;
			public double Deceleration;

			public double ScaleZ;

			public bool Visible;
		}

		Particle[] _particles = new Particle[ParticleCount];

		void Square() {
			GL.BindTexture(TextureTarget.Texture2D, this._textures[0]);
			GL.Begin(BeginMode.Quads);
			GL.TexCoord2(0.0, 0.0);
			GL.Vertex2(-1.0, -1.0);
			GL.TexCoord2(1.0, 0.0);
			GL.Vertex2(1.0, -1.0);
			GL.TexCoord2(1.0, 1.0);
			GL.Vertex2(-1.0, 1.0);
			GL.End();
		}

		void CreateParticles() {
			var rand = new Random();
			for (int i = 0; i < ParticleCount; i++) {
				this._particles[i] = new Particle();
				this._particles[i].Pos = new Vector3();
				this._particles[i].Pos.X = 0;
				this._particles[i].Pos.Y = -5;
				this._particles[i].Pos.Z = -5;
				this._particles[i].Mov = new Vector3();
				this._particles[i].Mov.X = (float)((((((((2 - 1 + 1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.005) - (((((((2 - 1 + 1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.005));
				this._particles[i].Mov.Z = (float)((((((((2 - 1 + 1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.005) - (((((((2 - 1 + 1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.005));
				this._particles[i].Color = Color.White;
				this._particles[i].ScaleZ = 0.25;
				this._particles[i].Direction = 0;
				this._particles[i].Acceleration = ((((((8 - 5 + 2) * rand.NextDouble() % 11) + 5) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.02;
				this._particles[i].Deceleration = 0.0025;
			}
		}

		void UpdateParticles() {
			var rand = new Random();
			for (int i = 0; i < ParticleCount; i++) {
				GL.Color3(this._particles[i].Color);
				this._particles[i].Pos.Y = (float)(this._particles[i].Pos.Y + this._particles[i].Acceleration - this._particles[i].Deceleration);
				
				this._particles[i].Pos.X += this._particles[i].Mov.X * 4;
				this._particles[i].Pos.Z += this._particles[i].Mov.Z * 4;

				this._particles[i].Direction += ((((((int)(0.5 - 0.1 + 0.1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1);
				this._particles[i].Deceleration += 0.0025;

				if (this._particles[i].Pos.Y < -5) {
					this._particles[i].Pos.X = 0;
					this._particles[i].Pos.Y = -5;
					this._particles[i].Pos.Z = -5;
					this._particles[i].Mov.X = (float)((((((((2 - 1 + 1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.005) - (((((((2 - 1 + 1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.005));
					this._particles[i].Mov.Z = (float)((((((((2 - 1 + 1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.005) - (((((((2 - 1 + 1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.005));
					this._particles[i].Color = Color.White;
					this._particles[i].ScaleZ = 0.25;
					this._particles[i].Direction = 0;
					this._particles[i].Acceleration = ((((((8 - 5 + 2) * rand.NextDouble() % 11) + 5) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.02;
					this._particles[i].Deceleration = 0.0025;
				}
			}
		}

		void DrawParticles() {
			for (int i = 0; i < ParticleCount; i++) {
				GL.PushMatrix();
				GL.Translate(this._particles[i].Pos);
				GL.Rotate((float)(this._particles[i].Direction - 90), Vector3.UnitZ);
				GL.Scale(this._particles[i].ScaleZ, this._particles[i].ScaleZ, this._particles[i].ScaleZ);

				GL.Disable(EnableCap.DepthTest);

				GL.Enable(EnableCap.Blend);
				GL.BlendFunc(BlendingFactorSrc.DstColor, BlendingFactorDest.Zero);
				GL.BindTexture(TextureTarget.Texture2D, this._textures[0]);

				GL.Begin(BeginMode.Quads);
				GL.TexCoord2(0, 0);
				GL.Vertex3(-1, -1, 0);
				GL.TexCoord2(1, 0);
				GL.Vertex3(1, -1, 0);
				GL.TexCoord2(1, 1);
				GL.Vertex3(1, 1, 0);
				GL.TexCoord2(0, 1);
				GL.Vertex3(-1, 1, 0);
				GL.End();

				GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
				GL.BindTexture(TextureTarget.Texture2D, this._textures[1]);

				GL.Begin(BeginMode.Quads);
				GL.TexCoord2(0, 0);
				GL.Vertex3(-1, -1, 0);
				GL.TexCoord2(1, 0);
				GL.Vertex3(1, -1, 0);
				GL.TexCoord2(1, 1);
				GL.Vertex3(1, 1, 0);
				GL.TexCoord2(0, 1);
				GL.Vertex3(-1, 1, 0);
				GL.End();

				GL.Enable(EnableCap.DepthTest);

				GL.PopMatrix();
			}
		}

		void Init() {
			GL.Enable(EnableCap.Texture2D);
			GL.Enable(EnableCap.DepthTest);
			this.CreateParticles();

			this._textures[0] = LoadTexture("particle_mask.bmp");
			this._textures[1] = LoadTexture("particle.bmp");
		}

		void Reshape() {
			GL.Viewport(this.ClientRectangle);
			GL.MatrixMode(MatrixMode.Projection);
			Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, this.ClientRectangle.Width / (float)this.ClientRectangle.Height, 1.0F, 100F);
			GL.LoadMatrix(ref proj);
		}

		void Update() {
			//this.UpdateParticles();
		}

		void Display() {
			GL.ClearColor(Color.Black);
			GL.Clear(ClearBufferMask.ColorBufferBit);
			Matrix4 modelView = Matrix4.LookAt(new Vector3(15.0F, 0.0F, 15.0F), Vector3.Zero, Vector3.UnitY);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref modelView);
			this.UpdateParticles();
			this.DrawParticles();
			this.SwapBuffers();
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

		static void FreeTexture(uint texture) {
			GL.DeleteTexture((int)texture);
		}
	
		static void Main(string[] args) {
			using (var prog = new Program()) {
				prog.Run();
			}
		}


	}
}
