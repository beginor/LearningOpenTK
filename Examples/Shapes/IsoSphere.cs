using System;
using OpenTK;

namespace Examples.Shapes {

    public class IsoSphere : Shape {
    
        private const double DoublePi = Math.PI * 2.0;

        public IsoSphere(int sSteps, int tSteps, float xScale, float yScale, float zScale) {
            int count = 4 * sSteps * tSteps;

            Vertices = new Vector3[count];
            Normals = new Vector3[count];
            Texcoords = new Vector2[count];
            Indices = new uint[6 * count / 4];

            int i = 0;
            for (double t = -Math.PI; (float)t < (float)Math.PI - Single.Epsilon; t += Math.PI / (double)tSteps) {
                for (double s = 0.0; (float)s < (float)DoublePi; s += Math.PI / (double)sSteps) {
                    Vertices[i].X = xScale * (float)(Math.Cos(s) * Math.Sin(t));
                    Vertices[i].Y = yScale * (float)(Math.Sin(s) * Math.Sin(t));
                    Vertices[i].Z = zScale * (float)Math.Cos(t);
                    //vertices[i] = vertices[i].Scale(xScale, yScale, zScale);
                    Normals[i] = Vector3.Normalize(Vertices[i]);

                    ++i;
                }
            }

            for (i = 0; i < 6 * count / 4; i += 6) {
                Indices[i] = (uint)i;
                Indices[i + 1] = (uint)(i + 1);
                Indices[i + 2] = (uint)(i + 2 * sSteps + 1);
                Indices[i + 3] = (uint)(i + 2 * sSteps);
                Indices[i + 4] = (uint)i;
                Indices[i + 5] = (uint)(i + 2 * sSteps + 1);
            }
        }
    }
}
