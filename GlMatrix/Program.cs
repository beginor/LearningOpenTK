using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Examples.Shapes;
using System.Drawing;

namespace GlMatrix {

	class Program : GameWindow {

		float _angle1;
		float _angle2;

		Cube _cube1 = new Cube();
		Cube _cube2 = new Cube();

		void Cube1() {
			GL.PushMatrix();
			GL.Translate(1, 0, 0);
			GL.Rotate(this._angle1, Vector3.UnitX);
			GL.Rotate(this._angle1, Vector3.UnitY);
			GL.Rotate(this._angle1, Vector3.UnitZ);
			DisplayCube(this._cube1);
			GL.PopMatrix();
		}

		void Cube2() {
			GL.PushMatrix();
			GL.Translate(Math.Sin(_angle2 * Math.PI / 180), Math.Cos(_angle2 * Math.PI / 180), 0);
			GL.Rotate(this._angle2, Vector3.UnitX);
			GL.Rotate(this._angle2, Vector3.UnitY);
			GL.Rotate(this._angle2, Vector3.UnitZ);
			DisplayCube(this._cube2);
			GL.PopMatrix();
		}

		void Display() {
			GL.ClearColor(Color.Black);
			GL.Clear(ClearBufferMask.ColorBufferBit);
			Matrix4 modelView = Matrix4.LookAt(new Vector3(0.0F, 0.0F, 15.0F), Vector3.Zero, Vector3.UnitY);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref modelView);
			this.Cube1();
			this.Cube2();
			this.SwapBuffers();
			this._angle1 += 1.0F;
			this._angle2 += 2.0F;
		}

		void Reshape() {
			GL.Viewport(this.ClientRectangle);
			GL.MatrixMode(MatrixMode.Projection);
			Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, this.ClientRectangle.Width / (float)this.ClientRectangle.Height, 1.0F, 100F);
			GL.LoadMatrix(ref proj);
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			this.Reshape();
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			base.OnRenderFrame(e);
			this.Display();
		}

		public override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			//GL.Enable(EnableCap.DepthTest);
			//GL.DepthFunc(DepthFunction.Lequal);
			//GL.Enable(EnableCap.CullFace);
			//GL.CullFace(CullFaceMode.Back);
			//GL.FrontFace(FrontFaceDirection.Cw);
		}

		static void DisplayCube(Shape cube) {
			GL.Begin(BeginMode.Triangles);
			foreach (var index in cube.Indices) {
				GL.Color3(cube.Colors[index]);
				GL.Vertex3(cube.Vertices[index]);
			}
			GL.End();
		}

		static void Main(string[] args) {
			using (var prog = new Program()) {
				prog.Run();
			}
		}
	}
}
