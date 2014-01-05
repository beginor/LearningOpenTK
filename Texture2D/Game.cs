using System;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing.Imaging;
using GlPixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using SysPixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Texture2D {

	public class Game : OpenTK.GameWindow {

		private Bitmap _bitmap = new Bitmap("logo.jpg");
		private int _texture;

		/// <summary>Creates a 800x600 window with the specified title.</summary>
		public Game()
			: base(800, 600/*, GraphicsMode.Default, "Texture 2D Sample"*/) {
			//VSync = VSyncMode.On;
		}

		/// <summary>Load resources here.</summary>
		/// <param name="e">Not used.</param>
		public override void OnLoad(EventArgs e) {
			GL.ClearColor(Color.MidnightBlue);
			//GL.Enable(EnableCap.DepthTest);

			GL.Enable(EnableCap.Texture2D);
			GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

			GL.GenTextures(1, out _texture);
			GL.BindTexture(TextureTarget.Texture2D, _texture);

			BitmapData bitmapData = _bitmap.LockBits(
				new Rectangle(0, 0, _bitmap.Width, _bitmap.Height),
				ImageLockMode.ReadOnly,
				SysPixelFormat.Format32bppArgb
			);

			GL.TexImage2D(
				TextureTarget.Texture2D,
				0,
				PixelInternalFormat.Rgba,
				bitmapData.Width,
				bitmapData.Height,
				0,
				GlPixelFormat.Bgra,
				PixelType.UnsignedByte,
				bitmapData.Scan0
			);

			_bitmap.UnlockBits(bitmapData);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
		}

		public override void OnUnload(EventArgs e) {
			//base.OnUnload(e);
			GL.DeleteTextures(1, ref _texture);
		}

		/// <summary>
		/// Called when your window is resized. Set your viewport here. It is also
		/// a good place to set up your projection matrix (which probably changes
		/// along when the aspect ratio of your window).
		/// </summary>
		/// <param name="e">Contains information on the new Width and Size of the GameWindow.</param>
		protected override void OnResize(EventArgs e) {
			GL.Viewport(ClientRectangle);

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();

			GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
		}

		/// <summary>
		/// Called when it is time to setup the next frame. Add you game logic here.
		/// </summary>
		/// <param name="e">Contains timing information for framerate independent logic.</param>
		protected override void OnUpdateFrame(FrameEventArgs e) {
			if (Keyboard[Key.Escape])
				Exit();
		}

		/// <summary>
		/// Called when it is time to render the next frame. Add your rendering code here.
		/// </summary>
		/// <param name="e">Contains timing information.</param>
		protected override void OnRenderFrame(FrameEventArgs e) {
			GL.Clear(ClearBufferMask.ColorBufferBit);

			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			GL.BindTexture(TextureTarget.Texture2D, _texture);

			GL.Begin(BeginMode.Quads);

			GL.TexCoord2(0.0f, 1.0f);
			GL.Vertex2(-0.6f, -0.4f);
			GL.TexCoord2(1.0f, 1.0f);
			GL.Vertex2(0.6f, -0.4f);
			GL.TexCoord2(1.0f, 0.0f);
			GL.Vertex2(0.6f, 0.4f);
			GL.TexCoord2(0.0f, 0.0f);
			GL.Vertex2(-0.6f, 0.4f);

			GL.End();

			SwapBuffers();
		}

		[STAThread]
		static void Main(string[] args) {
			// The 'using' idiom guarantees proper resource cleanup.
			// We request 30 UpdateFrame events per second, and unlimited
			// RenderFrame events (as fast as the computer can handle).
			using (Game game = new Game()) {
				game.Run(30.0, 0.0);
			}
		}
	}
}
