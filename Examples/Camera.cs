using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Examples {

	public class Camera {

		private float _aspectRatio;
		private float _farPlane;
		private float _fieldOfView;
		private Vector3 _location;
		private float _nearPlane;
		private bool _projectionDirty = true;
		private Matrix4 _projectionMatrix4;
		private Vector3 _target;
		private bool _viewDirty = true;
		private Matrix4 _viewMatrix4;

		protected virtual void RebuildProjectionMatrix4() {
			this._projectionMatrix4 = Matrix4.CreatePerspectiveFieldOfView(this.FieldOfView, this.AspectRatio, this.NearPlane, this.FarPlane);
			this._projectionDirty = false;
		}

		protected virtual void RebuildViewMatrix4() {
			this._viewMatrix4 = Matrix4.LookAt(this.Location, this.Target, Vector3.UnitY);
			this._viewDirty = false;
		}

		public float AspectRatio {
			get {
				return this._aspectRatio;
			}
			set {
				if (this._aspectRatio != value) {
					this._aspectRatio = value;
					this._projectionDirty = true;
				}
			}
		}

		public float FarPlane {
			get {
				return this._farPlane;
			}
			set {
				if (this._farPlane != value) {
					this._farPlane = value;
					this._projectionDirty = true;
				}
			}
		}

		public float FieldOfView {
			get {
				return this._fieldOfView;
			}
			set {
				if (this._fieldOfView != value) {
					this._fieldOfView = value;
					this._projectionDirty = true;
				}
			}
		}

		public Vector3 Location {
			get {
				return this._location;
			}
			set {
				if (this._location != value) {
					this._location = value;
					this._viewDirty = true;
				}
			}
		}

		public float NearPlane {
			get {
				return this._nearPlane;
			}
			set {
				if (this._nearPlane != value) {
					this._nearPlane = value;
					this._projectionDirty = true;
				}
			}
		}

		public Matrix4 ProjectionMatrix4 {
			get {
				if (this._projectionDirty) {
					this.RebuildProjectionMatrix4();
				}
				return this._projectionMatrix4;
			}
		}

		public Vector3 Target {
			get {
				return this._target;
			}
			set {
				if (this._target != value) {
					this._target = value;
					this._viewDirty = true;
				}
			}
		}

		public Matrix4 ViewMatrix4 {
			get {
				if (this._viewDirty) {
					this.RebuildViewMatrix4();
				}
				return this._viewMatrix4;
			}
		}
	}


}
