using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace GLControlTest {

	public class MyControl : GLControl {

		private bool _loaded;
		private int _x;
		private float _rotation;
		private readonly Stopwatch _sw = new Stopwatch();

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			this._loaded = true;
			GL.ClearColor(Color.MidnightBlue);
			this.SetupViewport();
			Application.Idle += this.Application_Idle;
			this._sw.Start();
		}

		private void Application_Idle(object sender, EventArgs e) {
			while (this.IsIdle) {
				double milliseconds = this.ComputeTimeSlice();
				this.Animate(milliseconds);
				this.Render();
			}
		}

		private void Animate(double milliseconds) {
			float deltaRotation = (float)milliseconds / 20.0f;
			this._rotation += deltaRotation;
		}

		private double ComputeTimeSlice() {
			this._sw.Stop();
			double timeSlice = this._sw.Elapsed.TotalMilliseconds;
			this._sw.Reset();
			this._sw.Start();
			return timeSlice;
		}

		private void SetupViewport() {
			var w = this.Width;
			var h = this.Height;

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(0, w, 0, h, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
			GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if (!this._loaded) {
				return;
			}
			this.SetupViewport();
			this.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if (!this._loaded) {
				return;
			}
			this.Render();
		}

		private void Render() {
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();

			GL.Translate(this._x, 0, 0);

			if (this.Focused) {
				GL.Color3(Color.Yellow);
			}
			else {
				GL.Color3(Color.Blue);
			}

			GL.Rotate(this._rotation, Vector3.UnitZ);
			
			GL.Begin(PrimitiveType.Triangles);
			GL.Vertex2(10, 20);
			GL.Vertex2(100, 20);
			GL.Vertex2(100, 50);
			GL.End();

			this.SwapBuffers();
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if (e.KeyCode == Keys.Space) {
				this._x++;
				this.Invalidate();
			}
		}
	}
}
