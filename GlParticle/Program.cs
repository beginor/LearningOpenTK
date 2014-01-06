using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GlPixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace GlParticle {

    internal class Program : GameWindow {
    
        private const int ParticleCount = 500;

        private readonly Particle[] _particles = new Particle[ParticleCount];
        private readonly uint[] _textures = new uint[10];

        private void Square() {
            GL.BindTexture(TextureTarget.Texture2D, _textures[0]);
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0.0, 0.0);
            GL.Vertex2(-1.0, -1.0);
            GL.TexCoord2(1.0, 0.0);
            GL.Vertex2(1.0, -1.0);
            GL.TexCoord2(1.0, 1.0);
            GL.Vertex2(-1.0, 1.0);
            GL.End();
        }

        private void CreateParticles() {
            var rand = new Random();
            for (int i = 0; i < ParticleCount; i++) {
                _particles[i] = new Particle();
                _particles[i].Pos = new Vector3();
                _particles[i].Pos.X = 0;
                _particles[i].Pos.Y = -5;
                _particles[i].Pos.Z = -5;
                _particles[i].Mov = new Vector3();
                _particles[i].Mov.X = (float)((((((((2 - 1 + 1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.005) - (((((((2 - 1 + 1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.005));
                _particles[i].Mov.Z = (float)((((((((2 - 1 + 1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.005) - (((((((2 - 1 + 1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.005));
                _particles[i].Color = Color.White;
                _particles[i].ScaleZ = 0.25;
                _particles[i].Direction = 0;
                _particles[i].Acceleration = ((((((8 - 5 + 2) * rand.NextDouble() % 11) + 5) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.02;
                _particles[i].Deceleration = 0.0025;
            }
        }

        private void UpdateParticles() {
            var rand = new Random();
            for (int i = 0; i < ParticleCount; i++) {
                GL.Color3(_particles[i].Color);
                _particles[i].Pos.Y = (float)(_particles[i].Pos.Y + _particles[i].Acceleration - _particles[i].Deceleration);

                _particles[i].Pos.X += _particles[i].Mov.X * 4;
                _particles[i].Pos.Z += _particles[i].Mov.Z * 4;

                _particles[i].Direction += ((((((int)(0.5 - 0.1 + 0.1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1);
                _particles[i].Deceleration += 0.0025;

                if (_particles[i].Pos.Y < -5) {
                    _particles[i].Pos.X = 0;
                    _particles[i].Pos.Y = -5;
                    _particles[i].Pos.Z = -5;
                    _particles[i].Mov.X = (float)((((((((2 - 1 + 1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.005) - (((((((2 - 1 + 1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.005));
                    _particles[i].Mov.Z = (float)((((((((2 - 1 + 1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.005) - (((((((2 - 1 + 1) * rand.NextDouble() % 11) + 1) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.005));
                    _particles[i].Color = Color.White;
                    _particles[i].ScaleZ = 0.25;
                    _particles[i].Direction = 0;
                    _particles[i].Acceleration = ((((((8 - 5 + 2) * rand.NextDouble() % 11) + 5) - 1 + 1) * rand.NextDouble() % 11) + 1) * 0.02;
                    _particles[i].Deceleration = 0.0025;
                }
            }
        }

        private void DrawParticles() {
            for (int i = 0; i < ParticleCount; i++) {
                GL.PushMatrix();
                GL.Translate(_particles[i].Pos);
                GL.Rotate((float)(_particles[i].Direction - 90), Vector3.UnitZ);
                GL.Scale(_particles[i].ScaleZ, _particles[i].ScaleZ, _particles[i].ScaleZ);

                GL.Disable(EnableCap.DepthTest);

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.DstColor, BlendingFactorDest.Zero);
                GL.BindTexture(TextureTarget.Texture2D, _textures[0]);

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
                GL.BindTexture(TextureTarget.Texture2D, _textures[1]);

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

        private void Init() {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
            CreateParticles();

            _textures[0] = LoadTexture("particle_mask.bmp");
            _textures[1] = LoadTexture("particle.bmp");
        }

        private void Reshape() {
            GL.Viewport(ClientRectangle);
            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, ClientRectangle.Width / (float)ClientRectangle.Height, 1.0F, 100F);
            GL.LoadMatrix(ref proj);
        }

        private void Update() {
            //this.UpdateParticles();
        }

        private void Display() {
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Matrix4 modelView = Matrix4.LookAt(new Vector3(15.0F, 0.0F, 15.0F), Vector3.Zero, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelView);
            UpdateParticles();
            DrawParticles();
            SwapBuffers();
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            Init();
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            Reshape();
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            Display();
        }

        private static uint LoadTexture(string filename) {
            uint texture;
            GL.GenTextures(1, out texture);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);

            using (var bmp = new Bitmap(filename)) {
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, bmp.Width, bmp.Height, 0, GlPixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
                bmp.UnlockBits(data);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

            return texture;
        }

        private static void FreeTexture(uint texture) {
            GL.DeleteTexture((int)texture);
        }

        private static void Main(string[] args) {
            using (var prog = new Program()) {
                prog.Run();
            }
        }

        private class Particle {
            public double Acceleration;
            public Color Color;
            public double Deceleration;
            public double Direction;
            public Vector3 Mov;
            public Vector3 Pos;

            public double ScaleZ;

            public bool Visible;
        }
    }
}
