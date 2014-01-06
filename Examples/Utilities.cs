using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GlPixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using SysPixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Examples {

    public static class Utilities {
    
        public static int ColorToRgba32(Color c) {
            return (c.A << 24) | (c.B << 16) | (c.G << 8) | c.R;
        }

        public static int LoadTexture(string textureFile) {
            GL.Enable(EnableCap.Texture2D);
            int texture = GL.GenTexture();
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            using (var bmp = new Bitmap(textureFile)) {
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, bmp.Width, bmp.Height, 0, GlPixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
                bmp.UnlockBits(data);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

            return texture;
        }

        public static Vector3 ComputeNormal(Vector3 v1, Vector3 v2, Vector3 v3) {
            float v1x, v1y, v1z, v2x, v2y, v2z;
            float nx, ny, nz;
            float vLen;

            var result = new Vector3();

            // Calculate vectors
            v1x = v1.X - v2.X;
            v1y = v1.Y - v2.Y;
            v1z = v1.Z - v2.Z;

            v2x = v2.X - v3.X;
            v2y = v2.Y - v3.Y;
            v2z = v2.Z - v3.Z;

            // Get cross product of vectors
            nx = (v1y * v2z) - (v1z * v2y);
            ny = (v1z * v2x) - (v1x * v2z);
            nz = (v1x * v2y) - (v1y * v2x);

            // Normalise final vector
            vLen = (float)Math.Sqrt((nx * nx) + (ny * ny) + (nz * nz));

            result.X = nx / vLen;
            result.Y = ny / vLen;
            result.Z = nz / vLen;

            return result;
        }
    }
}
