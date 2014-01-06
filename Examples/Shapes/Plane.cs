using OpenTK;

namespace Examples.Shapes {

    public class Plane : Shape {
    
        public Plane(int xRes, int yRes, float xScale, float yScale) {
            Vertices = new Vector3[xRes * yRes];
            Normals = new Vector3[xRes * yRes];
            Indices = new uint[6 * xRes * yRes];
            Texcoords = new Vector2[xRes * yRes];

            int i = 0;
            for (int y = -yRes / 2; y < yRes / 2; y++) {
                for (int x = -xRes / 2; x < xRes / 2; x++) {
                    Vertices[i].X = xScale * x / xRes;
                    Vertices[i].Y = yScale * y / yRes;
                    Vertices[i].Z = 0;
                    Normals[i].X = Normals[i].Y = 0;
                    Normals[i].Z = 1;
                    i++;
                }
            }

            i = 0;
            for (int y = 0; y < yRes - 1; y++) {
                for (int x = 0; x < xRes - 1; x++) {
                    Indices[i++] = (uint)((y + 0) * xRes + x);
                    Indices[i++] = (uint)((y + 1) * xRes + x);
                    Indices[i++] = (uint)((y + 0) * xRes + x + 1);

                    Indices[i++] = (uint)((y + 0) * xRes + x + 1);
                    Indices[i++] = (uint)((y + 1) * xRes + x);
                    Indices[i++] = (uint)((y + 1) * xRes + x + 1);
                }
            }
        }
    }
}
