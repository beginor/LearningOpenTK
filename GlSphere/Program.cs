using System;
using System.Drawing;
using System.Drawing.Imaging;
using Examples;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GlPixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace GlSphere {

    internal class Program : GameWindow {
    
        private readonly uint[] _textures = new uint[1];

        private float _angle;

        private Camera _camera;
        private UvSphere _sphere;

        private void DisplaySphere(double r, uint texture) {
            //GL.PushMatrix();
            //GL.LoadIdentity();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.Begin(PrimitiveType.Quads);
            foreach (uint index in _sphere.Indices) {
                GL.TexCoord2(_sphere.Texcoords[index]);
                GL.Vertex3(_sphere.Vertices[index]);
            }
            GL.End();
            //GL.PopMatrix();
        }

        private void CreateSphere(double r, double h, double k, double z) {
            _sphere = new UvSphere((float)r);
        }

        private void DisplayXyz() {
            GL.LineWidth(4.0F);
            GL.Begin(PrimitiveType.Lines);
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

        private void Display() {
            //GL.ClearDepth(1);
            GL.ClearColor(Color.MidnightBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 viewMatrix = _camera.ViewMatrix4;
            GL.LoadMatrix(ref viewMatrix);

            GL.Rotate(_angle / 5, Vector3.UnitY);
            GL.Rotate(_angle / 3, Vector3.UnitX);
            //this.DisplayXyz();
            DisplaySphere(5, _textures[0]);

            SwapBuffers();
            _angle++;
        }

        private void Init() {
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
            GL.Fog(FogParameter.FogColor, new[] {0.5F, 0.5F, 0.5F, 1.0F});
            CreateSphere(60, 0, 0, 0);
        }

        private void Reshape() {
            if (_camera == null) {
                _camera = new Camera {
                                         FarPlane = 1000.0F,
                                         NearPlane = 1.0F,
                                         FieldOfView = MathHelper.PiOver2,
                                         Target = Vector3.Zero,
                                         Location = new Vector3(90, 0, 0)
                                     };
            }

            GL.Viewport(ClientRectangle);
            _camera.AspectRatio = ClientRectangle.Width / (float)ClientRectangle.Height;

            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 projMatrix = _camera.ProjectionMatrix4;
            GL.LoadMatrix(ref projMatrix);
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

        private static void FreeTexture(uint texture) {
            GL.DeleteTextures(1, ref texture);
        }

        protected override void OnUnload(EventArgs e) {
            base.OnUnload(e);
            FreeTexture(_textures[0]);
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

        private static void Main(string[] args) {
            using (var prog = new Program()) {
                prog.Run(30);
            }
        }
    }
}
