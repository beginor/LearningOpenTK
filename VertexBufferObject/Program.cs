using System;
using System.Drawing;
using System.Windows.Forms;
using Examples.Shapes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace VertexBufferObject {

    public class T08_VBO : GameWindow {
    
        #region --- Private Fields ---
        public static readonly int order = 8;
        private readonly Shape cube = new Plane(10, 10, 1, 1);

        private readonly Vbo[] vbo = new Vbo[2];

        private struct Vbo {
            public int EboID, NumElements;
            public int VboID;
        }
        #endregion

        #region --- Constructor ---
        public T08_VBO() : base(800, 600) {
        }
        #endregion

        #region OnLoad override
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            string version = GL.GetString(StringName.Version);
            int major = version[0];
            int minor = version[2];
            if (major <= 1 && minor < 5) {
                MessageBox.Show("You need at least OpenGL 1.5 to run this example. Aborting.", "VBOs not supported",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Exit();
            }

            GL.ClearColor(0.1f, 0.1f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);

            // Create the Vertex Buffer Object:
            // 1) Generate the buffer handles.
            // 2) Bind the Vertex Buffer and upload your vertex buffer. Check that the buffer was uploaded correctly.
            // 3) Bind the Index Buffer and upload your index buffer. Check that the buffer was uploaded correctly.

            vbo[0] = LoadVBO(cube.Vertices, cube.Indices);
            vbo[1] = LoadVBO(cube.Vertices, cube.Indices);
        }
        #endregion

        #region OnResize override
        protected override void OnResize(EventArgs e) {
            GL.Viewport(0, 0, Width, Height);

            float aspect_ratio = Width / (float)Height;
            Matrix4 perpective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perpective);
        }
        #endregion

        #region OnUpdateFrame override
        /// <summary>
        ///     Prepares the next frame for rendering.
        /// </summary>
        /// <remarks>
        ///     Place your control logic here. This is the place to respond to user input,
        ///     update object positions etc.
        /// </remarks>
        protected override void OnUpdateFrame(FrameEventArgs e) {
            if (Keyboard[Key.Escape]) {
                Exit();
            }
        }
        #endregion

        #region OnRenderFrame
        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 lookat = Matrix4.LookAt(0, 5, 5, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            GL.Color4(Color.Black);
            Draw(vbo[0]);

            SwapBuffers();
        }
        #endregion

        private Vbo LoadVBO(Vector3[] vertices, uint[] indices) {
            var handle = new Vbo();
            int size;

            GL.GenBuffers(1, out handle.VboID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, handle.VboID);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Vector3.SizeInBytes), vertices,
                BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (vertices.Length * Vector3.SizeInBytes != size) {
                throw new ApplicationException("Vertex array not uploaded correctly");
            }
            //GL.BindBuffer(Version15.ArrayBuffer, 0);

            GL.GenBuffers(1, out handle.EboID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle.EboID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices,
                BufferUsageHint.StaticDraw);
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (indices.Length * sizeof(int) != size) {
                throw new ApplicationException("Element array not uploaded correctly");
            }
            //GL.BindBuffer(Version15.ElementArrayBuffer, 0);

            handle.NumElements = indices.Length;
            return handle;
        }

        private void Draw(Vbo handle) {
            //GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);

            //GL.EnableClientState(EnableCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.VertexArray);

            GL.BindBuffer(BufferTarget.ArrayBuffer, handle.VboID);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, handle.EboID);

            //GL.TexCoordPointer(2, TexCoordPointerType.Float, vector2_size, (IntPtr)vector2_size);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);

            GL.DrawElements(PrimitiveType.Triangles, handle.NumElements, DrawElementsType.UnsignedInt, IntPtr.Zero);
            //GL.DrawArrays(BeginMode.LineLoop, 0, vbo.element_count);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            GL.DisableClientState(ArrayCap.VertexArray);
            //GL.DisableClientState(EnableCap.TextureCoordArray);

            //GL.PopClientAttrib();
        }

        #region public static void Main()
        /// <summary>
        ///     Entry point of this example.
        /// </summary>
        [STAThread]
        public static void Main() {
            using (var example = new T08_VBO()) {
                // Get the title and category  of this example using reflection.
                //ExampleAttribute info = ((ExampleAttribute)example.GetType().GetCustomAttributes(false)[0]);
                //example.Title = String.Format("OpenTK | {0} {1}: {2}", info.Category, info.Difficulty, info.Title);
                example.Run(30.0, 0.0);
            }
        }
        #endregion
    }
}
