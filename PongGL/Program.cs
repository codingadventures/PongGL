// Released to the public domain. Use, modify and relicense at will.

using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using QuickFont;

namespace StarterKit
{
    class Game : GameWindow
    {

        Sprite[] sprites;
        QFont heading1;
        int scorePlayer1, scorePlayer2;
        struct Sprite
        {
            public Vertex[] vertices;
            //public bool isCollidable;

        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vertex
        { // mimic InterleavedArrayFormat.T2fN3fV3f
            public Vector2 TexCoord;

            public Vector2 Position;
        }
        Random RandomClass = new Random();
        float error;
        /// <summary>Creates a 800x600 window with the specified title.</summary>
        public Game()
            : base(800, 600, GraphicsMode.Default, "OpenTK Quick Start Sample")
        {
            VSync = VSyncMode.On;

            cx = (float)RandomClass.NextDouble();
            cy = (float)RandomClass.NextDouble();
            dx = 0.01f;
            dy = 0.002f;

            sprites = new Sprite[3];
            sprites[0].vertices = new Vertex[2];
            sprites[1].vertices = new Vertex[2];
            //sprites[0].vertices[0].Normal = Vector3.UnitX;

            sprites[0].vertices[0].Position.X = -0.025f + 0.8f;
            sprites[0].vertices[0].Position.Y = -0.15f;
            sprites[0].vertices[1].Position.X = 0.025f + 0.8f;
            sprites[0].vertices[1].Position.Y = 0.15f;
            sprites[1].vertices[0].Position.X = -0.025f - 0.8f;
            sprites[1].vertices[0].Position.Y = -0.15f;
            sprites[1].vertices[1].Position.X = 0.025f - 0.8f;
            sprites[1].vertices[1].Position.Y = 0.15f;

            sprites[2].vertices = new Vertex[4];




        }

        private float x = 0.8f, y;
        private float dx, dy, cx, cy, r = 1 / 20 + 0.02f;

        private Matrix4 projection;
        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            heading1 = QFont.FromQFontFile("woodenFont.qfont", 1.0f, new QFontLoaderConfiguration(true));

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.PointSmooth);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.PointSize(10.0f);
        }

        /// <summary>
        /// Called when your window is resized. Set your viewport here. It is also
        /// a good place to set up your projection matrix (which probably changes
        /// along when the aspect ratio of your window).
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        /// <summary>
        /// Called when it is time to setup the next frame. Add you game logic here.
        /// </summary>
        /// <param name="e">Contains timing information for framerate independent logic.</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape])
                Exit();

            if (Keyboard[Key.Up])
            {
                if (y <= 1f)
                {
                    sprites[0].vertices[0].Position.Y += 0.02f;
                    sprites[0].vertices[1].Position.Y += 0.02f;
                }
            }
            if (Keyboard[Key.W])
            {
                sprites[1].vertices[0].Position.Y += 0.02f;
                sprites[1].vertices[1].Position.Y += 0.02f;
            }

            if (Keyboard[Key.S])
            {
                sprites[1].vertices[0].Position.Y -= 0.02f;
                sprites[1].vertices[1].Position.Y -= 0.02f;
            }


            if (Keyboard[Key.Down])
            {
                if (y >= -1f)
                {
                    sprites[0].vertices[0].Position.Y -= 0.02f;
                    sprites[0].vertices[1].Position.Y -= 0.02f;

                }
            }


            CollisionDetection();
            AI();
        }

        private void CollisionDetection()
        {
            //foreach sprite
            for (int i = 0; i < 2; i++)
            {

                //foreach vertex of the ball
                for (float j = 0; j < 360; j += 10)
                {
                    float xx = cx + r * (float)Math.Cos(j * Math.PI / 180);
                    float yy = cy + r * (float)Math.Sin(j * Math.PI / 180);
                    if (xx >= sprites[i].vertices[0].Position.X
                        && xx <= sprites[i].vertices[1].Position.X
                        && yy >= sprites[i].vertices[0].Position.Y
                        && yy <= sprites[i].vertices[1].Position.Y)
                    {
                        //collision
                        dx = -dx;
                        return;
                    }
                }
            }

            if (cx <= -1)
            {
                scorePlayer1++;
                cx = 0;
                cy = 0;
                dx *= -1;
            }
            if (cx >= 1)
            {
                scorePlayer2++;//score 2
                cx = 0;
                cy = 0;
                dy *= -1;
            }

            if (cy <= -1 || cy >= 1)
                dy = -dy;





        }

        private void AI()
        {

            //I have to follow the ball
            if (dx > 0)
            {

                if (sprites[1].vertices[0].Position.Y + 0.15f - 0.5f >= 0)
                {
                    sprites[1].vertices[0].Position.Y += 0.015f;
                    sprites[1].vertices[1].Position.Y += 0.015f;

                }
                else
                {
                    sprites[1].vertices[0].Position.Y -= 0.015f;
                    sprites[1].vertices[1].Position.Y -= 0.015f;
                }
            }
            else
            {
                if (cy - sprites[1].vertices[0].Position.Y >= 0)
                {
                    sprites[1].vertices[0].Position.Y += 0.015f;
                    sprites[1].vertices[1].Position.Y += 0.015f;

                }
                else
                {
                    sprites[1].vertices[0].Position.Y -= 0.015f;
                    sprites[1].vertices[1].Position.Y -= 0.015f;
                }


            }
        }

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref modelview);


            QFont.Begin();

            GL.PushMatrix();
            GL.Translate(Width * 0.5f, 0, 0f);
            string score = string.Format("{0} - {1}", scorePlayer1, scorePlayer2);
            heading1.Print(score, QFontAlignment.Centre);

            GL.PopMatrix();


            QFont.End();

            GL.Disable(EnableCap.Texture2D);
            GL.Begin(BeginMode.Quads);
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex2(sprites[0].vertices[0].Position.X, sprites[0].vertices[0].Position.Y);
            GL.Vertex2(sprites[0].vertices[1].Position.X, sprites[0].vertices[0].Position.Y);
            GL.Vertex2(sprites[0].vertices[1].Position.X, sprites[0].vertices[1].Position.Y);
            GL.Vertex2(sprites[0].vertices[0].Position.X, sprites[0].vertices[1].Position.Y);

            GL.Vertex2(sprites[0].vertices[1].Position);

            GL.End();


            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Rect(sprites[1].vertices[0].Position.X
                 , sprites[1].vertices[0].Position.Y
                 , sprites[1].vertices[1].Position.X
                 , sprites[1].vertices[1].Position.Y);


            GL.LoadIdentity();


            GL.Begin(BeginMode.Points);

            GL.Color3(1.0f, 1.0f, 1.0f);

            GL.Vertex2(cx, cy);


            GL.End();




            cx += dx;
            cy += dy;

            GL.Flush();
            SwapBuffers();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // The 'using' idiom guarantees proper resource cleanup.
            // We request 30 UpdateFrame events per second, and unlimited
            // RenderFrame events (as fast as the computer can handle).
            using (Game game = new Game())
            {
                game.Run(30.0);
            }
        }
    }
}