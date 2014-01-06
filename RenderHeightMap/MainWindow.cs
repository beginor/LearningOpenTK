using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace RenderHeightMap {

	public class MainWindow : GameWindow {

		private Size _size;
		private byte[,] _heightTable;
		private PrimitiveType _terrainRenderStyle = PrimitiveType.Quads;

		private int _renderSteps = 10;

		public int RenderSteps {
			get {
				return this._renderSteps;
			}
			set {
				if (value < 1) {
					this._renderSteps = 1;
				}
				else if (value > 5000) {
					this._renderSteps = 5000;
				}
				else {
					this._renderSteps = value;
				}
			}
		}

		private float _cameraPositionX = 250.0F;
		private float _cameraPositionY = 200.0F;
		private float _cameraPositionZ = 1000.0F;

		private string _tutTxt = "Page Up/Down\t: Camera height\n" +
						"Arrow keys\t: Camera position\n" +
						"1\t: Points rendering\n" +
						"2\t: Lines rendering\n" +
						"3\t: Quads rendering\n" +
						"+\t: Increase terrain quality\n" +
						"-\t: Decrease terrain quality\n" +
						"ESC\t: Quit applicaton\n";

		private Stopwatch _stopwatch = new Stopwatch();
		private uint _diagVertices = 0;

		public MainWindow()
			: base(800, 600, GraphicsMode.Default, "RenderHeightMap") {
			base.VSync = VSyncMode.On;
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			GL.ClearColor(Color.MidnightBlue);
			GL.PointSize(3.0F);
			GL.Enable(EnableCap.DepthTest);

			this.Keyboard.KeyUp += this.Keyboard_KeyUp;
			this.Mouse.ButtonUp += this.Mouse_ButtonUp;
			this._stopwatch.Start();

			LoadHeightMap("heightmap.png");
		}

		private void LoadHeightMap(string path) {
			if (!File.Exists(path)) {
				Console.WriteLine("Height map file {0} does not exists!");
				return;
			}
			Bitmap bitmap = new Bitmap(path);
			this._size = new Size(bitmap.Width, bitmap.Height);
			this._heightTable = new byte[this._size.Width, this._size.Height];

			for (int x = 0; x < this._size.Width; x++) {
				for (int y = 0; y < this._size.Height; y++) {
					this._heightTable[x, y] = bitmap.GetPixel(x, y).R;
				}
			}
		}

		private void Mouse_ButtonUp(object sender, MouseButtonEventArgs e) {
			if (e.Button == MouseButton.Right) {
				Console.Write(this._tutTxt);
			}
		}

		private void Keyboard_KeyUp(object sender, KeyboardKeyEventArgs e) {
			switch (e.Key) {
				case Key.Escape:
					this.Exit();
					break;
				case Key.Number1:
					this._terrainRenderStyle = PrimitiveType.Points;
					break;
				case Key.Number2:
					this._terrainRenderStyle = PrimitiveType.Lines;
					break;
				case Key.Number3:
					this._terrainRenderStyle = PrimitiveType.Quads;
					break;
				case Key.Plus:
					this.RenderSteps -= 1;
					break;
				case Key.Minus:
					this.RenderSteps += 1;
					break;
			}
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			GL.Viewport(this.ClientRectangle);

			Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(
				(float)Math.PI / 4,
				this.ClientRectangle.Width / (float)this.ClientRectangle.Height,
				1.0f,
				6400.0f
			);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref proj);
		}

		protected override void OnUpdateFrame(FrameEventArgs e) {
			base.OnUpdateFrame(e);
			if (Keyboard[Key.PageUp])
				_cameraPositionY += 100 * (float)e.Time;
			if (Keyboard[Key.PageDown])
				_cameraPositionY -= 100 * (float)e.Time;
			if (Keyboard[OpenTK.Input.Key.Left])
				_cameraPositionX -= 100 * (float)e.Time;
			if (Keyboard[OpenTK.Input.Key.Right])
				_cameraPositionX += 100 * (float)e.Time;
			if (Keyboard[OpenTK.Input.Key.Up])
				_cameraPositionZ -= 100 * (float)e.Time;
			if (Keyboard[OpenTK.Input.Key.Down])
				_cameraPositionZ += 100 * (float)e.Time;
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			base.OnRenderFrame(e);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			Matrix4 modelview = Matrix4.LookAt(
				new Vector3(this._cameraPositionX, this._cameraPositionY, this._cameraPositionZ),
				Vector3.UnitZ,
				Vector3.UnitY
			);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref modelview);

			this.RenderHeightmap();

			if (this._stopwatch.ElapsedMilliseconds > 1000) {
				this._stopwatch.Reset();
				this._stopwatch.Start();
			}

			this.SwapBuffers();
		}

		private void RenderHeightmap() {
			this._diagVertices = 0;
			GL.Begin(this._terrainRenderStyle);
			for (int x = 0; x < this._size.Width - this.RenderSteps; x++) {
				for (int z = 0; z < this._size.Height - this.RenderSteps; z++) {
					GL.Color3(this._heightTable[x, z], this._heightTable[x, z], this._heightTable[x, z]);
					if (this._terrainRenderStyle == PrimitiveType.Points) {
						GL.Vertex3(x, this._heightTable[x, z], z);
						this._diagVertices++;
					}
					else if (this._terrainRenderStyle == PrimitiveType.Lines) {
						// 0, 0 -> 1, 0
						GL.Vertex3(x, this._heightTable[x, z], z);
						GL.Vertex3(x + RenderSteps, this._heightTable[x + RenderSteps, z], z);
						// 1, 0 -> 1, 1 
						GL.Vertex3(x + RenderSteps, this._heightTable[x + RenderSteps, z], z);
						GL.Vertex3(x + RenderSteps, this._heightTable[x + RenderSteps, z + RenderSteps], z + RenderSteps);
						// 1, 1 -> 0, 1
						GL.Vertex3(x + RenderSteps, this._heightTable[x + RenderSteps, z + RenderSteps], z + RenderSteps);
						GL.Vertex3(x, this._heightTable[x, z + RenderSteps], z + RenderSteps);
						// 0, 1 -> 0, 0
						GL.Vertex3(x, this._heightTable[x, z], z);
						GL.Vertex3(x, this._heightTable[x, z + RenderSteps], z + RenderSteps);

						this._diagVertices += 8;
					}
					else if (this._terrainRenderStyle == PrimitiveType.Quads) {
						// 0,0
						GL.Vertex3(x, this._heightTable[x, z], z);
						// 1,0
						GL.Vertex3(x + RenderSteps, this._heightTable[x + RenderSteps, z], z);
						// 1,1
						GL.Vertex3(x + RenderSteps, this._heightTable[x + RenderSteps, z + RenderSteps], z + RenderSteps);
						// 0,1
						GL.Vertex3(x, this._heightTable[x, z + RenderSteps], z + RenderSteps);

						this._diagVertices += 4;
					}
				}
			}
			GL.End();
		}
	}
}
