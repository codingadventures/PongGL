using OpenTK;

namespace PongGL.Entity
{ 
    public class Sprite
    {
        public Vector2[] Vertices;

        public Sprite(int vertexNumber)
        {
            Vertices = new Vector2[vertexNumber];
        }
    }
}
