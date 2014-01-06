using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GLControlTest {

    public class MyControl : GLControl {
    
        private readonly Stopwatch _sw = new Stopwatch();
        private bool _loaded;
        private float _rotation;
        private int _x;

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            _loaded = true;
            GL.ClearColor(Color.MidnightBlue);
            SetupViewport();
            Application.Idle += Application_Idle;
            _sw.Start();
        }

        private void Application_Idle(object sender, EventArgs e) {
            while (IsIdle) {
                double milliseconds = ComputeTimeSlice();
                Animate(milliseconds);
                Render();
            }
        }

        private void Animate(double milliseconds) {
            float deltaRotation = (float)milliseconds / 20.0f;
            _rotation += deltaRotation;
        }

        private double ComputeTimeSlice() {
            _sw.Stop();
            double timeSlice = _sw.Elapsed.TotalMilliseconds;
            _sw.Reset();
            _sw.Start();
            return timeSlice;
        }

        private void SetupViewport() {
            int w = Width;
            int h = Height;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if (!_loaded) {
                return;
            }
            SetupViewport();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (!_loaded) {
                return;
            }
            Render();
        }

        private void Render() {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Translate(_x, 0, 0);

            if (Focused) {
                GL.Color3(Color.Yellow);
            }
            else {
                GL.Color3(Color.Blue);
            }

            GL.Rotate(_rotation, Vector3.UnitZ);

            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex2(10, 20);
            GL.Vertex2(100, 20);
            GL.Vertex2(100, 50);
            GL.End();

            SwapBuffers();
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Space) {
                _x++;
                Invalidate();
            }
        }
    }
}
