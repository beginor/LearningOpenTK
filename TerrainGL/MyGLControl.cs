using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using SysKeyPressEventArgs = System.Windows.Forms.KeyPressEventArgs;

namespace TerrainGL {

	/// <summary>
	/// 创建一个自定义GLControl,继承自OpenTK.GLControl;
	/// </summary>
	public class MyGLControl : GLControl {

		/// <summary>
		/// 标记是否已经加载
		/// </summary>
		private bool _loaded;

		//private SwiftHeightField _heightField;
		private TerrainTile _terrain;
		private float xpos = 851.078F, ypos = 351.594F, zpos = 381.033F, xrot = 758F, yrot = 238F, angle = 0.0F;
		float lastx, lasty;
		float bounce;
		float cScale = 4.0F;

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			this._loaded = true;
			Application.Idle += this.Application_Idle;

			this.SetupViewport();
			
			GL.ClearColor(Color.Black);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.CullFace);

			this._terrain = new TerrainTile();
			
			this._watch.Start();
		}

		/// <summary>
		/// 创建OpenGL视图
		/// </summary>
		private void SetupViewport() {
			GL.Viewport(this.ClientRectangle);
			GL.MatrixMode(MatrixMode.Projection);
			var projMatrix = Matrix4.CreatePerspectiveFieldOfView(
				MathHelper.PiOver4,
				this.AspectRatio,
				1.0F,
				3000F
			);
			GL.LoadMatrix(ref projMatrix);
		}

		private void Application_Idle(object sender, EventArgs e) {
			while (this.IsIdle) {
				this.Render();
			}
		}

		private System.Diagnostics.Stopwatch _watch = new Stopwatch();
		private int _frameCount = 0;

		/// <summary>
		/// OpenGL 渲染
		/// </summary>
		private void Render() {
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			this.SetupCamera();
			this._terrain.Render();
			this.SwapBuffers();
			this._frameCount++;
			if (this._watch.ElapsedMilliseconds > 1000) {
				this._watch.Reset();
				this._watch.Start();
				this.ParentForm.Text = string.Format("FrameCount: {0}", this._frameCount);
				this._frameCount = 0;
			}
		}

		private void SetupCamera() {
			GL.MatrixMode(MatrixMode.Modelview);
			var eye = new Vector3d(this.xpos, this.ypos, this.zpos);
			var matrix = Matrix4d.LookAt(eye, new Vector3d(64, 0, 64), Vector3d.UnitY);
			GL.LoadMatrix(ref matrix);
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if (!this._loaded) {
				return;
			}
			this.SetupViewport();
			this.Invalidate();
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if (e.KeyCode == Keys.W) {
				float yrotrad = (this.yrot / 180 * MathHelper.Pi);
				float xrotrad = (this.xrot / 180 * MathHelper.Pi);
				xpos += (float)Math.Sin(yrotrad) * cScale;
				zpos -= (float)Math.Cos(yrotrad) * cScale;
				ypos -= (float)Math.Sin(xrotrad);
				bounce += 0.04F;
			}
			if (e.KeyCode == Keys.S) {
				float xrotrad, yrotrad;
				yrotrad = (yrot / 180 * 3.141592654f);
				xrotrad = (xrot / 180 * 3.141592654f);
				xpos -= (float)(Math.Sin(yrotrad)) * cScale;
				zpos += (float)(Math.Cos(yrotrad)) * cScale;
				ypos += (float)(Math.Sin(xrotrad));
				bounce += 0.04F;
			}
			if (e.KeyCode == Keys.D) {
				float yrotrad;
				yrotrad = (yrot / 180 * 3.141592654f);
				xpos += (float)(Math.Cos(yrotrad)) * cScale;
				zpos += (float)(Math.Sin(yrotrad)) * cScale;
			}
			if (e.KeyCode == Keys.A) {
				float yrotrad;
				yrotrad = (yrot / 180 * 3.141592654f);
				xpos -= (float)(Math.Cos(yrotrad)) * cScale;
				zpos -= (float)(Math.Sin(yrotrad)) * cScale;
			}
		}

		private bool _draging;

		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			_draging = true;
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			_draging = false;
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if (!this._loaded) {
				return;
			}
			this.Render();
		}

		protected override void Dispose(bool disposing) {
			this._terrain.Dispose();
			base.Dispose(disposing);
		}
	}
}
