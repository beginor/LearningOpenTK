using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GlCollision {
	
	class Program  : GameWindow {

		Vector3 p1;
		Vector3 p2;

		float d;

		const int radius1 = 1;
		const int radius2 = 1;

		void Collision() {
			var x = p2.X - p1.X;
			var y = p2.Y - p1.Y;
			var z = p2.Z - p1.Z;
			d = (float) Math.Sqrt(x * x + y * y + z * z);
		}

		void Pointz() {
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

		void Reshape() {
			GL.Viewport(this.ClientRectangle);
			GL.MatrixMode(MatrixMode.Projection);
			Matrix4 projMatrix = Matrix4.CreatePerspectiveFieldOfView(
				MathHelper.PiOver2,
				this.ClientRectangle.Width / (float)this.ClientRectangle.Height,
				1.0F,
				100.0F
				);
			GL.LoadMatrix(ref projMatrix);
		}

		void Display() {
			GL.ClearColor(Color.Black);
			GL.Clear(ClearBufferMask.ColorBufferBit);
			GL.MatrixMode(MatrixMode.Modelview);
			var viewProj = Matrix4.LookAt(new Vector3(5, 5, 5), Vector3.Zero, Vector3.UnitY);
			GL.LoadMatrix(ref viewProj);
			GL.PointSize(5);
			Collision();
			Pointz();
			this.SwapBuffers();
		}

		void Init() {
			//GL.Disable(EnableCap.DepthTest);
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			this.Init();
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			this.Reshape();
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			base.OnRenderFrame(e);
			this.Display();
		}

		protected override void OnUpdateFrame(FrameEventArgs e) {
			base.OnUpdateFrame(e);
			if (this.Keyboard[Key.Escape]) {
				this.Exit();
			}
			if (this.Keyboard[Key.Q]) {
				p1.Z -= 0.1F;
			}
			if (this.Keyboard[Key.Z]) {
				p1.Z += 0.1F;
			}
			if (this.Keyboard[Key.W]) {
				p1.Y -= 0.1F;
			}
			if (this.Keyboard[Key.S]) {
				p1.Y += 0.1F;
			}
			if (this.Keyboard[Key.A]) {
				p1.X -= 0.1F;
			}
			if (this.Keyboard[Key.D]) {
				p1.X += 0.1F;
			}
		}

		static void Main(string[] args) {
			using (var prog = new Program()) {
				prog.Run(30);
			}
		}
	}
}
