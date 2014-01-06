using System;
using System.Drawing;
using Examples.Shapes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GlRatation {

    public class MainWindow : GameWindow {
    
        private readonly Shape _cube = new Cube();
        private float _angle;

        private void Cube() {
            //GL.Rotate(this._angle * 0.18F / (float)Math.PI, Vector3.UnitX);
            //GL.Rotate(this._angle * 0.18F / (float)Math.PI, Vector3.UnitY);
            //GL.Rotate(this._angle * 0.18F / (float)Math.PI, Vector3.UnitZ);
            GL.Rotate(_angle * 0.18F / (float)Math.PI, Vector3.One);
            GL.Color4(Color.FromArgb(100, Color.Red));
            GL.Begin(PrimitiveType.Triangles);
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

        protected override void OnLoad(EventArgs e) {
            GL.ClearColor(Color.MidnightBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            Reshape();
        }

        protected override void OnResize(EventArgs e) {
            Reshape();
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            if (Keyboard[Key.Escape]) {
                Exit();
            }
            if (Keyboard[Key.Enter]) {
                if (WindowState == WindowState.Fullscreen) {
                    WindowState = WindowState.Normal;
                }
                else {
                    WindowState = WindowState.Fullscreen;
                }
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            Display();
        }
    }
}
