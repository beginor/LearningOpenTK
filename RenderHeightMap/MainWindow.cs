using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace RenderHeightMap {

    public class MainWindow : GameWindow {
    
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private float _cameraPositionX = 250.0F;
        private float _cameraPositionY = 200.0F;
        private float _cameraPositionZ = 1000.0F;

        private uint _diagVertices;
        private byte[,] _heightTable;
        private int _renderSteps = 10;
        private Size _size;
        private PrimitiveType _terrainRenderStyle = PrimitiveType.Quads;

        private string _tutTxt = "Page Up/Down\t: Camera height\n" +
                                 "Arrow keys\t: Camera position\n" +
                                 "1\t: Points rendering\n" +
                                 "2\t: Lines rendering\n" +
                                 "3\t: Quads rendering\n" +
                                 "+\t: Increase terrain quality\n" +
                                 "-\t: Decrease terrain quality\n" +
                                 "ESC\t: Quit applicaton\n";

        public MainWindow()
            : base(800, 600, GraphicsMode.Default, "RenderHeightMap") {
            base.VSync = VSyncMode.On;
        }

        public int RenderSteps {
            get {
                return _renderSteps;
            }
            set {
                if (value < 1) {
                    _renderSteps = 1;
                }
                else if (value > 5000) {
                    _renderSteps = 5000;
                }
                else {
                    _renderSteps = value;
                }
            }
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            GL.ClearColor(Color.MidnightBlue);
            GL.PointSize(3.0F);
            GL.Enable(EnableCap.DepthTest);

            Keyboard.KeyUp += Keyboard_KeyUp;
            Mouse.ButtonUp += Mouse_ButtonUp;
            _stopwatch.Start();

            LoadHeightMap("heightmap.png");
        }

        private void LoadHeightMap(string path) {
            if (!File.Exists(path)) {
                Console.WriteLine("Height map file {0} does not exists!");
                return;
            }
            var bitmap = new Bitmap(path);
            _size = new Size(bitmap.Width, bitmap.Height);
            _heightTable = new byte[_size.Width, _size.Height];

            for (int x = 0; x < _size.Width; x++) {
                for (int y = 0; y < _size.Height; y++) {
                    _heightTable[x, y] = bitmap.GetPixel(x, y).R;
                }
            }
        }

        private void Mouse_ButtonUp(object sender, MouseButtonEventArgs e) {
            if (e.Button == MouseButton.Right) {
                Console.Write(_tutTxt);
            }
        }

        private void Keyboard_KeyUp(object sender, KeyboardKeyEventArgs e) {
            switch (e.Key) {
                case Key.Escape:
                    Exit();
                    break;
                case Key.Number1:
                    _terrainRenderStyle = PrimitiveType.Points;
                    break;
                case Key.Number2:
                    _terrainRenderStyle = PrimitiveType.Lines;
                    break;
                case Key.Number3:
                    _terrainRenderStyle = PrimitiveType.Quads;
                    break;
                case Key.Plus:
                    RenderSteps -= 1;
                    break;
                case Key.Minus:
                    RenderSteps += 1;
                    break;
            }
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            GL.Viewport(ClientRectangle);

            Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(
                (float)Math.PI / 4,
                ClientRectangle.Width / (float)ClientRectangle.Height,
                1.0f,
                6400.0f
                );
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref proj);
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
            if (Keyboard[Key.PageUp]) {
                _cameraPositionY += 100 * (float)e.Time;
            }
            if (Keyboard[Key.PageDown]) {
                _cameraPositionY -= 100 * (float)e.Time;
            }
            if (Keyboard[Key.Left]) {
                _cameraPositionX -= 100 * (float)e.Time;
            }
            if (Keyboard[Key.Right]) {
                _cameraPositionX += 100 * (float)e.Time;
            }
            if (Keyboard[Key.Up]) {
                _cameraPositionZ -= 100 * (float)e.Time;
            }
            if (Keyboard[Key.Down]) {
                _cameraPositionZ += 100 * (float)e.Time;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = Matrix4.LookAt(
                new Vector3(_cameraPositionX, _cameraPositionY, _cameraPositionZ),
                Vector3.UnitZ,
                Vector3.UnitY
                );
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            RenderHeightmap();

            if (_stopwatch.ElapsedMilliseconds > 1000) {
                _stopwatch.Reset();
                _stopwatch.Start();
            }

            SwapBuffers();
        }

        private void RenderHeightmap() {
            _diagVertices = 0;
            GL.Begin(_terrainRenderStyle);
            for (int x = 0; x < _size.Width - RenderSteps; x++) {
                for (int z = 0; z < _size.Height - RenderSteps; z++) {
                    GL.Color3(_heightTable[x, z], _heightTable[x, z], _heightTable[x, z]);
                    if (_terrainRenderStyle == PrimitiveType.Points) {
                        GL.Vertex3(x, _heightTable[x, z], z);
                        _diagVertices++;
                    }
                    else if (_terrainRenderStyle == PrimitiveType.Lines) {
                        // 0, 0 -> 1, 0
                        GL.Vertex3(x, _heightTable[x, z], z);
                        GL.Vertex3(x + RenderSteps, _heightTable[x + RenderSteps, z], z);
                        // 1, 0 -> 1, 1 
                        GL.Vertex3(x + RenderSteps, _heightTable[x + RenderSteps, z], z);
                        GL.Vertex3(x + RenderSteps, _heightTable[x + RenderSteps, z + RenderSteps], z + RenderSteps);
                        // 1, 1 -> 0, 1
                        GL.Vertex3(x + RenderSteps, _heightTable[x + RenderSteps, z + RenderSteps], z + RenderSteps);
                        GL.Vertex3(x, _heightTable[x, z + RenderSteps], z + RenderSteps);
                        // 0, 1 -> 0, 0
                        GL.Vertex3(x, _heightTable[x, z], z);
                        GL.Vertex3(x, _heightTable[x, z + RenderSteps], z + RenderSteps);

                        _diagVertices += 8;
                    }
                    else if (_terrainRenderStyle == PrimitiveType.Quads) {
                        // 0,0
                        GL.Vertex3(x, _heightTable[x, z], z);
                        // 1,0
                        GL.Vertex3(x + RenderSteps, _heightTable[x + RenderSteps, z], z);
                        // 1,1
                        GL.Vertex3(x + RenderSteps, _heightTable[x + RenderSteps, z + RenderSteps], z + RenderSteps);
                        // 0,1
                        GL.Vertex3(x, _heightTable[x, z + RenderSteps], z + RenderSteps);

                        _diagVertices += 4;
                    }
                }
            }
            GL.End();
        }
    }
}
