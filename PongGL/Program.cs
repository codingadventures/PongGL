// Released to the public domain. Use, modify and relicense at will.

using System;
using System.Runtime.InteropServices;
using System.Timers;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using QuickFont;

namespace PongGL
{
    public class Game : GameWindow
    {
        private readonly Sprite[] _sprites;
        private QFont _heading1;
        private int _scorePlayer1, _scorePlayer2;

        private readonly float _y;
        private float _dx, _dy, _ballCenterX, _ballCenterY;

        private const float R = 1 / 20 + 0.02f;
        private const float PaddleSpeed = 0.02f;
        private const int WindowWidth = 800;
        private const int WindowHeight = 600;
        private const float BallSpeedX = 0.012f;
        private const float BallSpeedY = 0.005f;

        private readonly Timer _timer = new Timer(TimeSpan.FromSeconds(4).TotalMilliseconds);

        private Matrix4 _projection;


        struct Sprite
        {
            public Vertex[] Vertices;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vertex
        {
            public Vector2 Position;
        }

        readonly Random _randomClass = new Random();

        /// <summary>Creates a 800x600 window with the specified title.</summary>
        public Game()
            : base(WindowWidth, WindowHeight, GraphicsMode.Default, "Funny PongGL")
        {
            //Increase speed as time goes by
            _timer.Elapsed += (sender, args) =>
            {
                _dx += 0.002f * Math.Sign(_dx);
            };

            VSync = VSyncMode.On;

            _ballCenterX = (float)_randomClass.NextDouble();
            _ballCenterY = (float)_randomClass.NextDouble();
            _dx = BallSpeedX;
            _dy = BallSpeedY;

            _sprites = new Sprite[3];
            _sprites[0].Vertices = new Vertex[2];
            _sprites[1].Vertices = new Vertex[2];

            _sprites[0].Vertices[0].Position.X = -0.025f + 0.8f;
            _sprites[0].Vertices[0].Position.Y = -0.15f;
            _sprites[0].Vertices[1].Position.X = 0.025f + 0.8f;
            _sprites[0].Vertices[1].Position.Y = 0.15f;

            _sprites[1].Vertices[0].Position.X = -0.025f - 0.8f;
            _sprites[1].Vertices[0].Position.Y = -0.15f;
            _sprites[1].Vertices[1].Position.X = 0.025f - 0.8f;
            _sprites[1].Vertices[1].Position.Y = 0.15f;

            _sprites[2].Vertices = new Vertex[4];
            _timer.Start();
        }

        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _heading1 = QFont.FromQFontFile("woodenFont.qfont", 1.0f, new QFontLoaderConfiguration(true));

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

            _projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref _projection);
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

            if (Keyboard[Key.W])
            {
                if (_sprites[0].Vertices[1].Position.Y <= 1f)
                {
                    _sprites[0].Vertices[0].Position.Y += PaddleSpeed;
                    _sprites[0].Vertices[1].Position.Y += PaddleSpeed;
                }
            }
            if (Keyboard[Key.Up])
            {
                if (_sprites[1].Vertices[1].Position.Y <= 1f)
                {
                    _sprites[1].Vertices[0].Position.Y += PaddleSpeed;

                    _sprites[1].Vertices[1].Position.Y += PaddleSpeed;
                }
            }

            if (Keyboard[Key.Down])
            {
                if (_sprites[1].Vertices[0].Position.Y >= -1f)
                {
                    _sprites[1].Vertices[0].Position.Y -= PaddleSpeed;
                    _sprites[1].Vertices[1].Position.Y -= PaddleSpeed;
                }
            }

            if (Keyboard[Key.S])
            {
                if (_sprites[0].Vertices[0].Position.Y >= -1f)
                {
                    _sprites[0].Vertices[0].Position.Y -= PaddleSpeed;
                    _sprites[0].Vertices[1].Position.Y -= PaddleSpeed;

                }
            }

            CollisionDetection();
            //if (_ballSpeedX >= 0.1f || _ballSpeedY >= 0.1f)
            //{
            //    this.p;
            //}
            //   AI();
        }

        private void CollisionDetection()
        {
            //foreach sprite
            for (var i = 0; i < 2; i++)
            {
                //foreach vertex of the ball
                for (float j = 0; j < 360; j += 10)
                {
                    var xx = _ballCenterX + R * (float)Math.Cos(j * Math.PI / 180);
                    var yy = _ballCenterY + R * (float)Math.Sin(j * Math.PI / 180);
                    if (!(xx >= _sprites[i].Vertices[0].Position.X) || !(xx <= _sprites[i].Vertices[1].Position.X) ||
                        !(yy >= _sprites[i].Vertices[0].Position.Y) || !(yy <= _sprites[i].Vertices[1].Position.Y))
                        continue;
                    {
                        //collision
                        _dx = -_dx;

                        //throw the ball away
                        _ballCenterX += (_ballCenterX - xx + 0.01f);
                    }
                    return;
                }
            }

            if (_ballCenterX <= -1)
            {
                _scorePlayer1++;//score 1
                _ballCenterX = 0;
                _ballCenterY = 0;
                _dx *= -1;
            }
            if (_ballCenterX >= 1)
            {
                _scorePlayer2++;//score 2
                _ballCenterX = 0;
                _ballCenterY = 0;
                _dy *= -1;
            }

            if (_ballCenterY <= -1 || _ballCenterY >= 1)
                _dy = -_dy;
        }

        private void AI()
        {

            var paddleCenter = (_sprites[1].Vertices[0].Position.Y + _sprites[1].Vertices[1].Position.Y) / 2;

            if (_dx < 0)
            {
                if (paddleCenter - _ballCenterY <= 0)
                {
                    _sprites[1].Vertices[0].Position.Y += PaddleSpeed;
                    _sprites[1].Vertices[1].Position.Y += PaddleSpeed;
                }
                else
                {
                    _sprites[1].Vertices[0].Position.Y -= PaddleSpeed;
                    _sprites[1].Vertices[1].Position.Y -= PaddleSpeed;
                }
            }
            else
            {
                if (paddleCenter <= 0.1 && paddleCenter >= -0.1) return;

                if (paddleCenter > 0)
                {
                    _sprites[1].Vertices[0].Position.Y -= PaddleSpeed;
                    _sprites[1].Vertices[1].Position.Y -= PaddleSpeed;
                }
                else
                {
                    _sprites[1].Vertices[0].Position.Y += PaddleSpeed;
                    _sprites[1].Vertices[1].Position.Y += PaddleSpeed;
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

            var modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref modelview);


            QFont.Begin();

            GL.PushMatrix();
            GL.Translate(Width * 0.5f, 0, 0f);
            var score = string.Format("{0} - {1}", _scorePlayer1, _scorePlayer2);
            _heading1.Print(score, QFontAlignment.Centre);

            GL.PopMatrix();


            QFont.End();

            GL.Disable(EnableCap.Texture2D);
            GL.Begin(BeginMode.Quads);
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex2(_sprites[0].Vertices[0].Position.X, _sprites[0].Vertices[0].Position.Y);
            GL.Vertex2(_sprites[0].Vertices[1].Position.X, _sprites[0].Vertices[0].Position.Y);
            GL.Vertex2(_sprites[0].Vertices[1].Position.X, _sprites[0].Vertices[1].Position.Y);
            GL.Vertex2(_sprites[0].Vertices[0].Position.X, _sprites[0].Vertices[1].Position.Y);

            GL.Vertex2(_sprites[0].Vertices[1].Position);

            GL.End();


            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Rect(_sprites[1].Vertices[0].Position.X
                 , _sprites[1].Vertices[0].Position.Y
                 , _sprites[1].Vertices[1].Position.X
                 , _sprites[1].Vertices[1].Position.Y);


            GL.LoadIdentity();


            GL.Begin(BeginMode.Points);

            GL.Color3(1.0f, 1.0f, 1.0f);

            GL.Vertex2(_ballCenterX, _ballCenterY);


            GL.End();

            _ballCenterX += _dx;
            _ballCenterY += _dy;

            GL.Flush();
            SwapBuffers();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Game())
            {
                game.Run(60.0);
            }
        }
    }
}