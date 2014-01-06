using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Examples.Shapes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GlPixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using SysPixelFormat = System.Drawing.Imaging.PixelFormat;

namespace TerrainGL {

    public class TerrainTile : Shape {
    
        private readonly uint[] _vboId = new uint[3];
        private int _texture;
        private bool fog;

        public TerrainTile() {
            const int xRes = 1024;
            const int zRes = 1024;
            XRes = xRes;
            ZRes = zRes;

            Vertices = new Vector3[xRes * zRes];
            Normals = new Vector3[xRes * zRes];
            Indices = new uint[6 * xRes * zRes];
            Texcoords = new Vector2[xRes * zRes];

            int i = 0;
            for (int z = -zRes / 2; z < zRes / 2; z++) {
                for (int x = -xRes / 2; x < xRes / 2; x++) {
                    Vertices[i] = new Vector3(x, 0, z);
                    Normals[i] = new Vector3(0, 1, 0);
                    i++;
                }
            }

            i = 0;
            for (int z = 0; z < zRes - 1; z++) {
                for (int x = 0; x < xRes - 1; x++) {
                    Indices[i++] = (uint)((z + 0) * xRes + x);
                    Indices[i++] = (uint)((z + 1) * xRes + x);
                    Indices[i++] = (uint)((z + 0) * xRes + x + 1);

                    Indices[i++] = (uint)((z + 0) * xRes + x + 1);
                    Indices[i++] = (uint)((z + 1) * xRes + x);
                    Indices[i++] = (uint)((z + 1) * xRes + x + 1);
                }
            }

            int zLength = zRes - 1;
            int xLength = xRes - 1;
            for (int x = 0; x < xRes; x++) {
                for (int z = 0; z < zRes; z++) {
                    //Texcoords[x * xRes + z] = new Vector2(z/(float)zLength,x/(float)xLength);
                    Texcoords[x * xRes + z].X = z / (float)zLength;
                    Texcoords[x * xRes + z].Y = x / (float)xLength;
                }
            }

            LoadTexture();
            LoadHeightFile();
            Init();
        }

        public int XRes { get; private set; }

        public int ZRes { get; private set; }

        public void Init() {
            GL.GenBuffers(3, _vboId);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboId[0]);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(Vertices.Length * 3 * sizeof(float)),
                Vertices,
                BufferUsageHint.StaticDraw
                );

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboId[1]);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(Texcoords.Length * 2 * sizeof(float)),
                Texcoords,
                BufferUsageHint.StaticDraw
                );

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _vboId[2]);
            GL.BufferData(
                BufferTarget.ElementArrayBuffer,
                (IntPtr)(Indices.Length * sizeof(uint)),
                Indices,
                BufferUsageHint.StaticDraw
                );

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        private void LoadTexture() {
            //_texture = Examples.Utilities.LoadTexture("texture.jpg");
            using (var bitmap = new Bitmap("texture.jpg")) {
                GL.Enable(EnableCap.Texture2D);
                GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

                GL.GenTextures(1, out _texture);
                GL.BindTexture(TextureTarget.Texture2D, _texture);

                BitmapData bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly,
                    bitmap.PixelFormat
                    );

                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgb,
                    bitmapData.Width,
                    bitmapData.Height,
                    0,
                    GlPixelFormat.Bgr,
                    PixelType.UnsignedByte,
                    bitmapData.Scan0
                    );

                bitmap.UnlockBits(bitmapData);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
        }

        private void LoadHeightFile() {
            int c = 1024 / XRes;
            using (var file = new FileStream("heightField.raw", FileMode.Open)) {
                var reader = new BinaryReader(file);
                for (int i = 0; i < Vertices.Length; i++) {
                    int b = 0;
                    for (int x = 0; x < c; x++) {
                        b += reader.ReadByte();
                    }
                    b = b / c;
                    Vertices[i].Y = b;
                }
                reader.Close();
                file.Close();
            }
        }

        public void Render() {
            if (!fog) {
                GL.Enable(EnableCap.Fog);
                GL.Fog(FogParameter.FogMode, (int)FogMode.Exp2);
                GL.Fog(FogParameter.FogDensity, 0.005F);
                GL.Fog(FogParameter.FogColor, new[] {0.5F, 0.5F, 0.5F, 1.0F});
                fog = true;
            }

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboId[0]);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);

            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboId[1]);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vector2.SizeInBytes, IntPtr.Zero);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _vboId[2]);

            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.VertexArray);

            if (fog) {
                GL.Disable(EnableCap.Fog);
            }
        }

        public void Dispose() {
            try {
                GL.DeleteTexture(_texture);
                GL.DeleteBuffers(3, _vboId);
            }
            catch (Exception ex) {
            }
        }
    }
}
