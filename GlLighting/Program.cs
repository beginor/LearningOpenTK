using System;
using System.Drawing;
using System.Windows.Forms;
using Examples.Shapes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GlLighting {

    public class MainWindow : GameWindow {
    
        private readonly float[] _blackAmbientLight = {0, 0, 0};

        private readonly float[] _blankMaterial = {0, 0, 0};
        private readonly Shape _cube = new Cube();
        private readonly float[] _greenEmissiveMaterial = {0, 1, 0};
        private readonly float[] _mShininess = {128};
        private readonly float[] _redDiffuseMaterial = {1, 0, 0};
        private readonly float[] _whiteDiffuseLight = {1, 1, 1};
        private readonly float[] _whiteSpacularLight = {1, 1, 1};
        private readonly float[] _whiteSpecularMaterial = {1, 1, 1};
        private float _angle;

        private bool _diffuse;
        private bool _emissive;
        private bool _specular;

        private void Cube() {
            GL.Rotate(_angle * 0.18F / (float)Math.PI, Vector3.UnitX);
            GL.Rotate(_angle * 0.18F / (float)Math.PI, Vector3.UnitY);
            GL.Rotate(_angle * 0.18F / (float)Math.PI, Vector3.UnitZ);
            //GL.Rotate(this._angle * 0.18F / (float)Math.PI, Vector3.One);
            GL.Color4(Color.FromArgb(100, Color.Red));
            GL.Begin(BeginMode.Triangles);
            foreach (uint index in _cube.Indices) {
                GL.Vertex3(_cube.Vertices[index]);
            }
            GL.End();
        }

        private void Display() {
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelView = Matrix4.LookAt(new Vector3(0.0F, 0.0F, 5.0F), Vector3.Zero, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelView);
            Cube();
            _angle++;
            SwapBuffers();
        }

        private void Reshape() {
            GL.Viewport(ClientRectangle);
            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, ClientRectangle.Width / (float)ClientRectangle.Height, 1.0F, 100F);
            GL.LoadMatrix(ref proj);
        }

        private void Init() {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Light(LightName.Light0, LightParameter.Specular, _whiteSpacularLight);
            GL.Light(LightName.Light0, LightParameter.Ambient, _blackAmbientLight);
            GL.Light(LightName.Light0, LightParameter.Diffuse, _whiteDiffuseLight);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.Fog);
            GL.Fog(FogParameter.FogMode, (int)FogMode.Exp2);
            GL.Fog(FogParameter.FogDensity, 0.3F);
            GL.Fog(FogParameter.FogColor, new[] {0.5F, 0.5F, 0.5F, 1.0F});
            GL.Hint(HintTarget.FogHint, HintMode.Nicest);
        }

        protected override void OnLoad(EventArgs e) {
            Init();
        }

        protected override void OnResize(EventArgs e) {
            Reshape();
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            if (Keyboard[Key.Escape]) {
                Exit();
            }
            if (Keyboard[Key.Enter]) {
                WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;
            }
            if (Keyboard[Key.S]) {
                _specular = !_specular;
                GL.Material(
                    MaterialFace.FrontAndBack,
                    MaterialParameter.Specular,
                    _specular ? _whiteSpecularMaterial : _blankMaterial
                    );
                GL.Material(
                    MaterialFace.FrontAndBack,
                    MaterialParameter.Shininess,
                    _specular ? _mShininess : _blankMaterial
                    );
            }
            if (Keyboard[Key.D]) {
                _diffuse = !_diffuse;
                GL.Material(
                    MaterialFace.FrontAndBack,
                    MaterialParameter.Diffuse,
                    _diffuse ? _redDiffuseMaterial : _blankMaterial
                    );
            }
            if (Keyboard[Key.E]) {
                _emissive = !_emissive;
                GL.Material(
                    MaterialFace.FrontAndBack,
                    MaterialParameter.Emission,
                    _emissive ? _greenEmissiveMaterial : _blankMaterial
                    );
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            Display();
        }
    }

    internal static class Program {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var mainForm = new MainWindow()) {
                mainForm.Run();
            }
        }
    }
}
