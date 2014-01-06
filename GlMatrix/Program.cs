using System;
using System.Drawing;
using Examples.Shapes;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GlMatrix {

    internal class Program : GameWindow {
    
        private readonly Cube _cube1 = new Cube();
        private readonly Cube _cube2 = new Cube();
        private float _angle1;
        private float _angle2;

        private void Cube1() {
            GL.PushMatrix();
            GL.Translate(1, 0, 0);
            GL.Rotate(_angle1, Vector3.UnitX);
            GL.Rotate(_angle1, Vector3.UnitY);
            GL.Rotate(_angle1, Vector3.UnitZ);
            DisplayCube(_cube1);
            GL.PopMatrix();
        }

        private void Cube2() {
            GL.PushMatrix();
            GL.Translate(Math.Sin(_angle2 * Math.PI / 180), Math.Cos(_angle2 * Math.PI / 180), 0);
            GL.Rotate(_angle2, Vector3.UnitX);
            GL.Rotate(_angle2, Vector3.UnitY);
            GL.Rotate(_angle2, Vector3.UnitZ);
            DisplayCube(_cube2);
            GL.PopMatrix();
        }

        private void Display() {
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Matrix4 modelView = Matrix4.LookAt(new Vector3(0.0F, 0.0F, 15.0F), Vector3.Zero, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelView);
            Cube1();
            Cube2();
            SwapBuffers();
            _angle1 += 1.0F;
            _angle2 += 2.0F;
        }

        private void Reshape() {
            GL.Viewport(ClientRectangle);
            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, ClientRectangle.Width / (float)ClientRectangle.Height, 1.0F, 100F);
            GL.LoadMatrix(ref proj);
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            Reshape();
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            Display();
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            //GL.Enable(EnableCap.DepthTest);
            //GL.DepthFunc(DepthFunction.Lequal);
            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Back);
            //GL.FrontFace(FrontFaceDirection.Cw);
        }

        private static void DisplayCube(Shape cube) {
            GL.Begin(PrimitiveType.Triangles);
            foreach (uint index in cube.Indices) {
                GL.Color3(cube.Colors[index]);
                GL.Vertex3(cube.Vertices[index]);
            }
            GL.End();
        }

        private static void Main(string[] args) {
            using (var prog = new Program()) {
                prog.Run();
            }
        }
    }
}
