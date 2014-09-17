using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Audio;
using SharpDX.Toolkit.Input;
using PokeSi.Screens;
using BlendState = SharpDX.Toolkit.Graphics.BlendState;
using SamplerState = SharpDX.Toolkit.Graphics.SamplerState;

namespace PokeSi
{
    public class PokeSiGame : Game
    {
        /// <summary>
        /// Give the rectangle corresponding to the Viexport (part of the screen we draw on)
        /// </summary>
        public Rectangle Viewport
        {
            get
            {
                return new Rectangle(0, 0, (int)this.GraphicsDevice.Presenter.BackBuffer.Width, (int)this.GraphicsDevice.Presenter.BackBuffer.Height);
            }
        }

        // Resources for drawing
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private BlendState blendState;
        private SamplerState samplerState;

        // Setup constantes
        private const int PreferredWidth = 1920;
        private const int PreferredHeight = 1080;
        private const bool IsFullScreen = false;
        private const int TargetFrameRate = 60;

        // Resources for input managing
        public KeyboardManager Keyboard;
        public MouseManager Mouse;

        // Screen resources
        private ScreenManager screenManager;

        public PokeSiGame()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = PreferredWidth;
            graphics.PreferredBackBufferHeight = PreferredHeight;
            graphics.IsFullScreen = IsFullScreen;

            Keyboard = new KeyboardManager(this);
            Mouse = new MouseManager(this);
            Input.Initilize(this);

            this.IsMouseVisible = true;

            Content.RootDirectory = "Content";

            // Framerate differs between platforms.
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);

            screenManager = new ScreenManager(this);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            blendState = BlendState.New(graphics.GraphicsDevice, BlendOption.SourceAlpha, BlendOption.InverseSourceAlpha, BlendOperation.Add, BlendOption.One, BlendOption.InverseSourceAlpha, BlendOperation.Add);
            samplerState = SamplerState.New(graphics.GraphicsDevice,
                new SamplerStateDescription()
                {
                    AddressU = TextureAddressMode.Wrap,
                    AddressV = TextureAddressMode.Wrap,
                    AddressW = TextureAddressMode.Wrap,
                    BorderColor = Color4.Black,
                    ComparisonFunction = Comparison.Always,
                    Filter = Filter.MinMagMipPoint,
                    MaximumAnisotropy = 4,
                    MaximumLod = 4,
                    MinimumLod = 0,
                    MipLodBias = 0
                });
            screenManager.CloseAllAndThenOpen(new WorldScreen(screenManager));
        }

        /// <summary>
        /// Update will be called once per game frame, it's the place to update
        /// all the logic.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            if (base.IsActive)
            {
                screenManager.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw will be called once per frame, it's the place to draw
        /// all the thing to see on screen.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.BackToFront, blendState, samplerState); // Start drawing operation

            screenManager.Draw(gameTime, spriteBatch);

            spriteBatch.End(); // End and flush drawing order

            base.Draw(gameTime);
        }
    }
}
