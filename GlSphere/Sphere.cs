using System;
using System.Collections.Generic;
using Examples.Shapes;
using OpenTK;

namespace GlSphere {

    internal class UvSphere : Shape {
    
        public UvSphere(float radius) {
            Radius = radius;

            const int step = 5;
            int xWidth = 360 / step + 1;
            int zHeight = 180 / step + 1;

            base.Vertices = new Vector3[xWidth * zHeight];
            base.Normals = new Vector3[xWidth * zHeight];
            base.Indices = new uint[6 * xWidth * zHeight];
            base.Texcoords = new Vector2[xWidth * zHeight];

            int i = 0;
            int halfZHeight = (zHeight - 1) / 2;
            int v = 0;
            for (int z = -halfZHeight; z <= halfZHeight; z++) {
                int d = 0;
                for (int x = 0; x < xWidth; x++) {
                    base.Vertices[i].X = (float)(radius * Math.Cos((x * step * Math.PI / 180)) * Math.Cos((z * step * Math.PI / 180)));
                    base.Vertices[i].Z = (float)(radius * Math.Sin((x * step * Math.PI / 180)) * Math.Cos((z * step * Math.PI / 180)));

                    base.Vertices[i].Y = (float)(radius * Math.Sin((z * step * Math.PI / 180)));

                    base.Texcoords[i].X = x / (float)(xWidth - 1);
                    base.Texcoords[i].Y = 1 - (z + halfZHeight) / (float)(halfZHeight * 2);

                    i++;
                }
            }

            i = 0;
            var indexList = new List<uint>();
            for (int z = 0; z < zHeight - 1; z++) {
                for (int x = 0; x < xWidth - 1; x++) {
                    indexList.Add((uint)(z * xWidth + x));
                    indexList.Add((uint)(z * xWidth + x + 1));
                    indexList.Add((uint)((z + 1) * xWidth + x + 1));
                    indexList.Add((uint)((z + 1) * xWidth + x));
                }
            }
            Indices = indexList.ToArray();
        }

        public float Radius { get; private set; }
    }
}
