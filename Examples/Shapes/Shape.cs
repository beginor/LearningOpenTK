using System.Drawing;
using OpenTK;

namespace Examples.Shapes {

    /// <summary>
    ///     一个抽象的形状
    /// </summary>
    public abstract class Shape {
    
        public Vector3[] Vertices { get; protected set; }

        public Vector3[] Normals { get; protected set; }

        public Vector2[] Texcoords { get; protected set; }

        public uint[] Indices { get; protected set; }

        public Color[] Colors { get; protected set; }
    }
}
