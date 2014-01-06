using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Examples.Shapes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GlLighting {

	public class MainWindow : GameWindow {

		private float _angle = 0.0f;
		private Shape _cube = new Cube();

		// 设置物体的材质
		private float[] _redDiffuseMaterial = new float[] { 1, 0, 0 };
		private float[] _whiteSpecularMaterial = new float[] { 1, 1, 1 };
		private float[] _greenEmissiveMaterial = new float[] { 0, 1, 0 };

		// 设置灯光的材质
		private float[] _whiteSpacularLight = new float[] { 1, 1, 1 };
		private float[] _blackAmbientLight = new float[] { 0, 0, 0 };
		private float[] _whiteDiffuseLight = new float[] { 1, 1, 1 };

		private float[] _blankMaterial = new float[] { 0, 0, 0 };
		private float[] _mShininess = new float[] {128};

		bool _diffuse = false;
		bool _emissive = false;
		bool _specular = false;

		void Cube() {
			GL.Rotate(this._angle * 0.18F / (float)Math.PI, Vector3.UnitX);
			GL.Rotate(this._angle * 0.18F / (float)Math.PI, Vector3.UnitY);
			GL.Rotate(this._angle * 0.18F / (float)Math.PI, Vector3.UnitZ);
			//GL.Rotate(this._angle * 0.18F / (float)Math.PI, Vector3.One);
			GL.Color4(Color.FromArgb(100, Color.Red));
			GL.Begin(BeginMode.Triangles);
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

		void Init() {
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Lighting);
			GL.Enable(EnableCap.Light0);
			GL.Light(LightName.Light0, LightParameter.Specular, this._whiteSpacularLight);
			GL.Light(LightName.Light0, LightParameter.Ambient, this._blackAmbientLight);
			GL.Light(LightName.Light0, LightParameter.Diffuse, this._whiteDiffuseLight);
			GL.ShadeModel(ShadingModel.Smooth);
			GL.Enable(EnableCap.Fog);
			GL.Fog(FogParameter.FogMode, (int) FogMode.Exp2);
			GL.Fog(FogParameter.FogDensity, 0.3F);
			GL.Fog(FogParameter.FogColor, new[] {0.5F, 0.5F, 0.5F, 1.0F});
			GL.Hint(HintTarget.FogHint, HintMode.Nicest);
		}

		protected override void OnLoad(EventArgs e) {
			this.Init();
		}

		protected override void OnResize(EventArgs e) {
			this.Reshape();
		}

		protected override void OnUpdateFrame(FrameEventArgs e) {
			if (Keyboard[Key.Escape]) {
				Exit();
			}
			if (this.Keyboard[Key.Enter]) {
				this.WindowState = this.WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;
			}
			if (this.Keyboard[Key.S]) {
				this._specular = !this._specular;
				GL.Material(
					MaterialFace.FrontAndBack,
					MaterialParameter.Specular,
					this._specular ? this._whiteSpecularMaterial : this._blankMaterial
				);
				GL.Material(
					MaterialFace.FrontAndBack,
					MaterialParameter.Shininess,
					this._specular ? this._mShininess : this._blankMaterial
				);
			}
			if (this.Keyboard[Key.D]) {
				this._diffuse = !this._diffuse;
				GL.Material(
					MaterialFace.FrontAndBack,
					MaterialParameter.Diffuse,
					this._diffuse ? this._redDiffuseMaterial : this._blankMaterial
				);
			}
			if (this.Keyboard[Key.E]) {
				this._emissive = !this._emissive;
				GL.Material(
					MaterialFace.FrontAndBack,
					MaterialParameter.Emission,
					this._emissive ? this._greenEmissiveMaterial : this._blankMaterial
				);
			}
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			this.Display();
		}

	}

	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			using (var mainForm = new MainWindow()) {
				mainForm.Run();
			}
		}
	}
}
