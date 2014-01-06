using OpenTK;

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

        public float AspectRatio {
            get {
                return _aspectRatio;
            }
            set {
                if (_aspectRatio != value) {
                    _aspectRatio = value;
                    _projectionDirty = true;
                }
            }
        }

        public float FarPlane {
            get {
                return _farPlane;
            }
            set {
                if (_farPlane != value) {
                    _farPlane = value;
                    _projectionDirty = true;
                }
            }
        }

        public float FieldOfView {
            get {
                return _fieldOfView;
            }
            set {
                if (_fieldOfView != value) {
                    _fieldOfView = value;
                    _projectionDirty = true;
                }
            }
        }

        public Vector3 Location {
            get {
                return _location;
            }
            set {
                if (_location != value) {
                    _location = value;
                    _viewDirty = true;
                }
            }
        }

        public float NearPlane {
            get {
                return _nearPlane;
            }
            set {
                if (_nearPlane != value) {
                    _nearPlane = value;
                    _projectionDirty = true;
                }
            }
        }

        public Matrix4 ProjectionMatrix4 {
            get {
                if (_projectionDirty) {
                    RebuildProjectionMatrix4();
                }
                return _projectionMatrix4;
            }
        }

        public Vector3 Target {
            get {
                return _target;
            }
            set {
                if (_target != value) {
                    _target = value;
                    _viewDirty = true;
                }
            }
        }

        public Matrix4 ViewMatrix4 {
            get {
                if (_viewDirty) {
                    RebuildViewMatrix4();
                }
                return _viewMatrix4;
            }
        }

        protected virtual void RebuildProjectionMatrix4() {
            _projectionMatrix4 = Matrix4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane);
            _projectionDirty = false;
        }

        protected virtual void RebuildViewMatrix4() {
            _viewMatrix4 = Matrix4.LookAt(Location, Target, Vector3.UnitY);
            _viewDirty = false;
        }
    }
}
