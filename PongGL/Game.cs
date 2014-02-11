using System;
using System.Timers;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using PongGL.Entity;
using QuickFont;

namespace PongGL
{
    public class Game : GameWindow
    {
        #region [ Fields ]
        private readonly Sprite[] _sprites;
        private QFont _heading1;
        private int _scorePlayer1, _scorePlayer2;
        private readonly float _y;
        private float _dx, _dy, _ballCenterX, _ballCenterY;
        private readonly Timer _timer = new Timer(TimeSpan.FromSeconds(2).TotalMilliseconds);
        private Matrix4 _projection;
        private readonly Random _randomClass = new Random();
        #endregion

        #region [ Constants ]
        private const float R = 1 / 20 + 0.02f;
        private const float PaddleSpeed = 0.02f;
        private const int WindowWidth = 800;
        private const int WindowHeight = 600;
        private const float BallSpeedX = 0.012f;
        private const float BallSpeedY = 0.005f;
        private const int PlayerOne = 0;
        private const int PlayerTwo = 1;
        private const int Bottom = 0;
        private const int Top = 1;
        #endregion

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

            _sprites = new Sprite[2];

            _sprites[PlayerOne] = new Sprite(2);
            _sprites[PlayerTwo] = new Sprite(2);


            _sprites[PlayerOne].Vertices[Bottom].X = -0.025f + 0.8f;
            _sprites[PlayerOne].Vertices[Bottom].Y = -0.15f;
            _sprites[PlayerOne].Vertices[Top].X = 0.025f + 0.8f;
            _sprites[PlayerOne].Vertices[Top].Y = 0.15f;

            _sprites[PlayerTwo].Vertices[Bottom].X = -0.025f - 0.8f;
            _sprites[PlayerTwo].Vertices[Bottom].Y = -0.15f;
            _sprites[PlayerTwo].Vertices[Top].X = 0.025f - 0.8f;
            _sprites[PlayerTwo].Vertices[Top].Y = 0.15f;

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
                if (_sprites[PlayerOne].Vertices[Top].Y <= 1f)
                {
                    _sprites[PlayerOne].Vertices[Bottom].Y += PaddleSpeed;
                    _sprites[PlayerOne].Vertices[Top].Y += PaddleSpeed;
                }
            }
            if (Keyboard[Key.Up])
            {
                if (_sprites[PlayerTwo].Vertices[Top].Y <= 1f)
                {
                    _sprites[PlayerTwo].Vertices[Bottom].Y += PaddleSpeed;

                    _sprites[PlayerTwo].Vertices[Top].Y += PaddleSpeed;
                }
            }

            if (Keyboard[Key.Down])
            {
                if (_sprites[PlayerTwo].Vertices[Bottom].Y >= -1f)
                {
                    _sprites[PlayerTwo].Vertices[Bottom].Y -= PaddleSpeed;
                    _sprites[PlayerTwo].Vertices[Top].Y -= PaddleSpeed;
                }
            }

            if (Keyboard[Key.S])
            {
                if (_sprites[PlayerOne].Vertices[Bottom].Y >= -1f)
                {
                    _sprites[PlayerOne].Vertices[Bottom].Y -= PaddleSpeed;
                    _sprites[PlayerOne].Vertices[Top].Y -= PaddleSpeed;

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
                for (float j = 0; j < 360; j += 90)
                {
                    var xx = _ballCenterX + R * (float)Math.Cos(j * Math.PI / 180);
                    var yy = _ballCenterY + R * (float)Math.Sin(j * Math.PI / 180);
                    if (!(xx >= _sprites[i].Vertices[Bottom].X) || !(xx <= _sprites[i].Vertices[Top].X) ||
                       !(yy >= _sprites[i].Vertices[Bottom].Y) || !(yy <= _sprites[i].Vertices[Top].Y))
                        continue;

                    SetBallSpeed(_sprites[i]);

                    //throw the ball away
                    _ballCenterX += (_ballCenterX - xx + 0.005f);

                    return;

                }
            }

            if (_ballCenterX <= -1)
            {
                _scorePlayer1++;//score 1
                _ballCenterX = 0;
                _ballCenterY = 0;
                _dx = BallSpeedX * -1;
            }
            if (_ballCenterX >= 1)
            {
                _scorePlayer2++;//score 2
                _ballCenterX = 0;
                _ballCenterY = 0;
                _dy = BallSpeedY * -1;
            }

            if (_ballCenterY <= -1 || _ballCenterY >= 1)
                _dy = -_dy;
        }

        private void Ai()
        {

            var paddleCenter = (_sprites[PlayerTwo].Vertices[Bottom].Y + _sprites[PlayerTwo].Vertices[Top].Y) / 2;

            if (_dx < 0)
            {
                if (paddleCenter - _ballCenterY <= 0)
                {
                    _sprites[PlayerTwo].Vertices[Bottom].Y += PaddleSpeed;
                    _sprites[PlayerTwo].Vertices[Top].Y += PaddleSpeed;
                }
                else
                {
                    _sprites[PlayerTwo].Vertices[Bottom].Y -= PaddleSpeed;
                    _sprites[PlayerTwo].Vertices[Top].Y -= PaddleSpeed;
                }
            }
            else
            {
                if (paddleCenter <= 0.1 && paddleCenter >= -0.1) return;

                if (paddleCenter > 0)
                {
                    _sprites[PlayerTwo].Vertices[Bottom].Y -= PaddleSpeed;
                    _sprites[PlayerTwo].Vertices[Top].Y -= PaddleSpeed;
                }
                else
                {
                    _sprites[PlayerTwo].Vertices[Bottom].Y += PaddleSpeed;
                    _sprites[PlayerTwo].Vertices[Top].Y += PaddleSpeed;
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
            GL.Vertex2(_sprites[PlayerOne].Vertices[Bottom].X, _sprites[PlayerOne].Vertices[Bottom].Y);
            GL.Vertex2(_sprites[PlayerOne].Vertices[Top].X, _sprites[PlayerOne].Vertices[Bottom].Y);
            GL.Vertex2(_sprites[PlayerOne].Vertices[Top].X, _sprites[PlayerOne].Vertices[Top].Y);
            GL.Vertex2(_sprites[PlayerOne].Vertices[Bottom].X, _sprites[PlayerOne].Vertices[Top].Y);
            GL.Vertex2(_sprites[PlayerOne].Vertices[Top]);
            GL.End();


            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Rect(_sprites[PlayerTwo].Vertices[Bottom].X
                 , _sprites[PlayerTwo].Vertices[Bottom].Y
                 , _sprites[PlayerTwo].Vertices[Top].X
                 , _sprites[PlayerTwo].Vertices[Top].Y);

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

        private void SetBallSpeed(Sprite player)
        {
            var currentVelocity = Math.Sqrt(_dx * _dx + _dy * _dy);

            var localHitLoc = _ballCenterY - player.Vertices[Bottom].Y + 0.15f;
            // Where did the ball hit, relative to the paddle's center?
            var angleMultiplier = Math.Abs(localHitLoc / 0.15f);
            // Express local hit loc as a percentage of half the paddle's height

            // Use the angle multiplier to determine the angle the ball should return at, from 0-65 degrees. Then use trig functions to determine new x/y velocities. Feel free to use a different angle limit if you think another one works better.
            var xVelocity = Math.Cos(85.0f * angleMultiplier * Math.PI / 180) * currentVelocity;
            var yVelocity = Math.Sin(85.0f * angleMultiplier * Math.PI / 180) * currentVelocity;

            // If the ball hit the paddle below the center, the yVelocity should be flipped so that the ball is returned at a downward angle
            if (localHitLoc < 0)
            {
                yVelocity = -yVelocity;
            }

            // If the ball came in at an xVelocity of more than 0, we know the ball was travelling right when it hit the paddle. It should now start going left.        
            if (_dx < 0)
            {
                xVelocity = -xVelocity;
            }

            _dx = (float)xVelocity;
            _dy = (float)yVelocity;
        }
    }
}