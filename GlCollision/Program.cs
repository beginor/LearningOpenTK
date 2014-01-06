using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GlCollision {

    internal class Program : GameWindow {
    
        private const int radius1 = 1;
        private const int radius2 = 1;
        private float d;
        private Vector3 p1;
        private Vector3 p2;

        private void Collision() {
            float x = p2.X - p1.X;
            float y = p2.Y - p1.Y;
            float z = p2.Z - p1.Z;
            d = (float)Math.Sqrt(x * x + y * y + z * z);
        }

        private void Pointz() {
            GL.PushMatrix();
            GL.Begin(BeginMode.Points);
            if (d <= radius1 + radius2) {
                GL.Color3(Color.Red);
            }
            else {
                GL.Color3(Color.Blue);
            }
            GL.Vertex3(p1);
            GL.End();
            GL.PopMatrix();

            GL.PushMatrix();
            GL.Begin(BeginMode.Points);
            GL.Color3(Color.Green);
            GL.Vertex3(p2);
            GL.End();
            GL.PopMatrix();
        }

        private void Reshape() {
            GL.Viewport(ClientRectangle);
            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 projMatrix = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.PiOver2,
                ClientRectangle.Width / (float)ClientRectangle.Height,
                1.0F,
                100.0F
                );
            GL.LoadMatrix(ref projMatrix);
        }

        private void Display() {
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 viewProj = Matrix4.LookAt(new Vector3(5, 5, 5), Vector3.Zero, Vector3.UnitY);
            GL.LoadMatrix(ref viewProj);
            GL.PointSize(5);
            Collision();
            Pointz();
            SwapBuffers();
        }

        private void Init() {
            //GL.Disable(EnableCap.DepthTest);
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

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
            if (Keyboard[Key.Escape]) {
                Exit();
            }
            if (Keyboard[Key.Q]) {
                p1.Z -= 0.1F;
            }
            if (Keyboard[Key.Z]) {
                p1.Z += 0.1F;
            }
            if (Keyboard[Key.W]) {
                p1.Y -= 0.1F;
            }
            if (Keyboard[Key.S]) {
                p1.Y += 0.1F;
            }
            if (Keyboard[Key.A]) {
                p1.X -= 0.1F;
            }
            if (Keyboard[Key.D]) {
                p1.X += 0.1F;
            }
        }

        private static void Main(string[] args) {
            using (var prog = new Program()) {
                prog.Run(30);
            }
        }
    }
}
