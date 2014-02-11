using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK;

namespace PongGL.Entity
{
    [StructLayout(LayoutKind.Sequential)]
    struct Sprite
    {
        public Vector2[] Vertices;

        public Sprite(int vertexNumber)
        {
            Vertices= new Vector2[vertexNumber];
        }
    }
}
