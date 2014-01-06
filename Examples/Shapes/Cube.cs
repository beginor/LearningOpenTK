using System.Drawing;
using OpenTK;

namespace Examples.Shapes {

    public class Cube : Shape {
    
        public Cube() {
            Vertices = new[] {
                                 new Vector3(-1.0f, -1.0f, 1.0f),
                                 new Vector3(1.0f, -1.0f, 1.0f),
                                 new Vector3(1.0f, 1.0f, 1.0f),
                                 new Vector3(-1.0f, 1.0f, 1.0f),
                                 new Vector3(-1.0f, -1.0f, -1.0f),
                                 new Vector3(1.0f, -1.0f, -1.0f),
                                 new Vector3(1.0f, 1.0f, -1.0f),
                                 new Vector3(-1.0f, 1.0f, -1.0f)
                             };

            Indices = new uint[] {
                                     // front face
                                     0, 1, 2, 2, 3, 0,
                                     // top face
                                     3, 2, 6, 6, 7, 3,
                                     // back face
                                     7, 6, 5, 5, 4, 7,
                                     // left face
                                     4, 0, 3, 3, 7, 4,
                                     // bottom face
                                     0, 1, 5, 5, 4, 0,
                                     // right face
                                     1, 5, 6, 6, 2, 1
                                 };

            Normals = new[] {
                                new Vector3(-1.0f, -1.0f, 1.0f),
                                new Vector3(1.0f, -1.0f, 1.0f),
                                new Vector3(1.0f, 1.0f, 1.0f),
                                new Vector3(-1.0f, 1.0f, 1.0f),
                                new Vector3(-1.0f, -1.0f, -1.0f),
                                new Vector3(1.0f, -1.0f, -1.0f),
                                new Vector3(1.0f, 1.0f, -1.0f),
                                new Vector3(-1.0f, 1.0f, -1.0f)
                            };

            Colors = new[] {
                               Color.DarkRed,
                               Color.DarkSalmon,
                               Color.Gainsboro,
                               Color.Goldenrod,
                               Color.DarkRed,
                               Color.DarkSeaGreen,
                               Color.Gold,
                               Color.Gray
                           };
        }
    }
}
