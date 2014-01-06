using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Examples;
using Examples.Shapes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GlRatation {

	public class MainWindow : GameWindow {

		private float _angle = 0.0f;
		private Shape _cube = new Cube();

		void Cube() {
			//GL.Rotate(this._angle * 0.18F / (float)Math.PI, Vector3.UnitX);
			//GL.Rotate(this._angle * 0.18F / (float)Math.PI, Vector3.UnitY);
			//GL.Rotate(this._angle * 0.18F / (float)Math.PI, Vector3.UnitZ);
			GL.Rotate(this._angle*0.18F/(float) Math.PI, Vector3.One);
			GL.Color4(Color.FromArgb(100, Color.Red));
			GL.Begin(PrimitiveType.Triangles);
			foreach (var index in _cube.Indices) {
				GL.Vertex3(this._cube.Vertices[index]);
			}
			GL.End();
		}

		void Display() {
			GL.ClearColor(Color.Black);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			
			Matrix4 modelView = Matrix4.LookAt(new Vector3(0.0F, 0.0F, 5.0F), Vector3.Zero, Vector3.UnitY);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref modelView);
			Cube();
			this._angle++;
			this.SwapBuffers();
		}

		void Reshape() {
			GL.Viewport(this.ClientRectangle);
			GL.MatrixMode(MatrixMode.Projection);
			Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, this.ClientRectangle.Width / (float)this.ClientRectangle.Height, 1.0F, 100F);
			GL.LoadMatrix(ref proj);
		}

		protected override void OnLoad(EventArgs e) {
			GL.ClearColor(System.Drawing.Color.MidnightBlue);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			this.Reshape();
		}

		protected override void OnResize(EventArgs e) {
			this.Reshape();
		}

		protected override void OnUpdateFrame(FrameEventArgs e) {
			if (Keyboard[Key.Escape]) {
				Exit();
			}
			if (this.Keyboard[Key.Enter]) {
				if (this.WindowState == WindowState.Fullscreen) {
					this.WindowState = WindowState.Normal;
				}
				else {
					this.WindowState = WindowState.Fullscreen;
				}
			}

		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			this.Display();
		}

	}
}
