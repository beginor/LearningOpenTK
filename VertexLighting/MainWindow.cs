using System;
using System.Drawing;
using Examples.Shapes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace VertexLighting {

    public class MainWindow : GameWindow {
    
        private readonly Shape shape = new Plane(32, 32, 4, 4);
        private float x_angle, zoom;

        #region OnLoad
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            GL.ClearColor(Color.MidnightBlue);
            GL.Enable(EnableCap.DepthTest);
            //GL.Enable(EnableCap.CullFace);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);

            GL.VertexPointer(3, VertexPointerType.Float, 0, shape.Vertices);
            GL.NormalPointer(NormalPointerType.Float, 0, shape.Normals);

            // Enable Light 0 and set its parameters.
            GL.Light(LightName.Light0, LightParameter.Position, new[] {1.0f, 1.0f, -0.5f});
            GL.Light(LightName.Light0, LightParameter.Ambient, new[] {0.3f, 0.3f, 0.3f, 1.0f});
            GL.Light(LightName.Light0, LightParameter.Diffuse, new[] {1.0f, 1.0f, 1.0f, 1.0f});
            GL.Light(LightName.Light0, LightParameter.Specular, new[] {1.0f, 1.0f, 1.0f, 1.0f});
            GL.Light(LightName.Light0, LightParameter.SpotExponent, new[] {1.0f, 1.0f, 1.0f, 1.0f});
            GL.LightModel(LightModelParameter.LightModelAmbient, new[] {0.2f, 0.2f, 0.2f, 1.0f});
            GL.LightModel(LightModelParameter.LightModelTwoSide, 1);
            GL.LightModel(LightModelParameter.LightModelLocalViewer, 1);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);

            // Use GL.Material to set your object's material parameters.
            GL.Material(MaterialFace.Front, MaterialParameter.Ambient, new[] {0.3f, 0.3f, 0.3f, 1.0f});
            GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, new[] {1.0f, 1.0f, 1.0f, 1.0f});
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, new[] {1.0f, 1.0f, 1.0f, 1.0f});
            GL.Material(MaterialFace.Front, MaterialParameter.Emission, new[] {0.0f, 0.0f, 0.0f, 1.0f});
        }
        #endregion

        #region OnResize
        /// <summary>
        ///     Called when the user resizes the window.
        /// </summary>
        /// <param name="e">Contains the new width/height of the window.</param>
        /// <remarks>
        ///     You want the OpenGL viewport to match the window. This is the place to do it!
        /// </remarks>
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);

            float aspect_ratio = Width / (float)Height;
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
        }
        #endregion

        #region OnUpdateFrame
        /// <summary>
        ///     Prepares the next frame for rendering.
        /// </summary>
        /// <remarks>
        ///     Place your control logic here. This is the place to respond to user input,
        ///     update object positions etc.
        /// </remarks>
        protected override void OnUpdateFrame(FrameEventArgs e) {
            if (Keyboard[Key.Escape]) {
                Exit();
                return;
            }

            if ((Keyboard[Key.AltLeft] || Keyboard[Key.AltRight]) &&
                Keyboard[Key.Enter]) {
                if (WindowState != WindowState.Fullscreen) {
                    WindowState = WindowState.Fullscreen;
                }
                else {
                    WindowState = WindowState.Normal;
                }
            }

            if (Mouse[MouseButton.Left]) {
                x_angle = Mouse.X;
            }
            else {
                x_angle += 0.5f;
            }

            zoom = Mouse.Wheel * 0.5f; // Mouse.Wheel is broken on both Linux and Windows.

            // Do not leave x_angle drift too far away, as this will cause inaccuracies.
            if (x_angle > 360.0f) {
                x_angle -= 360.0f;
            }
            else if (x_angle < -360.0f) {
                x_angle += 360.0f;
            }
        }
        #endregion

        #region OnRenderFrame
        /// <summary>
        ///     Place your rendering code here.
        /// </summary>
        protected override void OnRenderFrame(FrameEventArgs e) {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 lookat = Matrix4.LookAt(0, 0, -7.5f + zoom, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            GL.Rotate(x_angle, 0.0f, 1.0f, 0.0f);

            GL.Begin(PrimitiveType.Triangles);
            foreach (int index in shape.Indices) {
                GL.Normal3(shape.Normals[index]);
                GL.Vertex3(shape.Vertices[index]);
            }
            GL.End();

            SwapBuffers();
        }
        #endregion
    }
}
