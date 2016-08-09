using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SamplerStateTest
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private SamplerState[] _samplerStates;
        private int _samplerStateIndex;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;

        private Effect _effect;
        private Texture2D _tex;

        private KeyboardState _prevKeyboard = Keyboard.GetState();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Content.Load<SpriteFont>("font");

            _tex = new Texture2D(GraphicsDevice, 16, 1, false, SurfaceFormat.Single);
            var data = new float[16];
            for (var x = 0; x < 16; x++)
                data[x] = x/16f;
            _tex.SetData(data);

            _effect = Content.Load<Effect>("CompareFunc");
            _effect.Parameters["CompareValue"].SetValue(0.5f);

            InitSamplerStates();
        }

        public void InitSamplerStates()
        {
            var compares = new[]
            {
                CompareFunction.Always,
                CompareFunction.Equal, 
                CompareFunction.Greater, 
                CompareFunction.GreaterEqual,
                CompareFunction.Less, 
                CompareFunction.LessEqual, 
                CompareFunction.Never, 
                CompareFunction.NotEqual
            };

            _samplerStates = new SamplerState[compares.Length];
            for (var i = 0; i < compares.Length; i++)
                _samplerStates[i] = new SamplerState
                {
                    AddressU = TextureAddressMode.Clamp,
                    AddressV = TextureAddressMode.Clamp,
                    AddressW = TextureAddressMode.Clamp,
                    ComparisonFunction = compares[i]
                };
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var keyboardState = Keyboard.GetState();

            var max = _samplerStates.Length;

            if (keyboardState.IsKeyDown(Keys.Left) && _prevKeyboard.IsKeyUp(Keys.Left))
                _samplerStateIndex = (_samplerStateIndex - 1 + max)%max;

            if (keyboardState.IsKeyDown(Keys.Right) && _prevKeyboard.IsKeyUp(Keys.Right))
                _samplerStateIndex = (_samplerStateIndex + 1)%max;

            _prevKeyboard = keyboardState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: _samplerStates[_samplerStateIndex], effect: _effect);

            _spriteBatch.Draw(_tex, new Rectangle(20, 20, 400, 400), Color.White);

            _spriteBatch.End();


            _spriteBatch.Begin();

            _spriteBatch.DrawString(_spriteFont, "Comparison Function: " + _samplerStates[_samplerStateIndex].ComparisonFunction, 
                new Vector2(480, 100), Color.Black);
            _spriteBatch.DrawString(_spriteFont, "Original texture:",
                new Vector2(480, 150), Color.Black);
            _spriteBatch.Draw(_tex, new Rectangle(480, 180, 200, 200), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
