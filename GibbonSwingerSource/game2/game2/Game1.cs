
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using FarseerPhysics.Controllers;
using FarseerPhysics.Common;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;




namespace game2
{
    /// <summary>
    /// Gibbon Swinger V1.0
    /// 
    /// Author: Andrew Kessler
    /// March 2012
    /// Laboratory for Intelligent Mechanical Systems (LIMS)
    /// Northwestern University
    /// 
    /// </summary>
    public class GibbonSwinger : Microsoft.Xna.Framework.Game
    {

        #region Game Objects
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState oldKeyState;
        GamePadState oldGamePad;

        GameOverScreen gameOverScreen;
        GameStartScreen gameStartScreen;
        EnterTextScreen enterTextScreen;
        Leaderboard leaderboardEasy;
        Leaderboard leaderboardHard;

        Gibbon gibbon;
        ScrollingBackground scrollingBG;
        HealthBar healthBar;
        #endregion

        #region Misc.
        int mode = 0;
        public bool needName;
        public int PREGAME = -1;
        public int PLAY = 0;
        public int GAMEOVER = 1;
        public int ENTERNAME = 2;
        public int LEADERBOARD = 3;
        const int easy = 1;
        const int hard = 2;
        #endregion

        #region Phyiscs Engine Stuff
        public World world;
        #endregion

        #region Colors, Fonts, etc.
        Color bgColor = Color.CornflowerBlue;

        public Texture2D bananaSprite;
        public Texture2D staticBG;
        Texture2D circleSprite;


        public SoundEffect gameOverSound;
        SoundEffect eatFruitSound;
        public Song bgMusic;

        public SpriteFont quartz_small;
        public SpriteFont quartz_big;
        public SpriteFont Andy;
        public SpriteFont Arial;
        public SpriteFont Andy_big;
        #endregion

        #region Gameplay values

        public bool needToResetTimer;
   
        float current_time;
        const float max_time = 200;

        Vector2 bananaPosition = Vector2.Zero;
        bool bananaActive;


        #endregion

        #region Camera Stuff
        Matrix view;
        public Vector2 cameraPosition;
        public Vector2 screenCenter;

        Random rand = new Random();


        public float Meter2Pixel = 64f; // 64 pixels 
        public float Pixel2Meter = 1 / 64f;

        public Vector2 r;
        #endregion

      
        public GibbonSwinger()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";



            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

           // graphics.PreferredBackBufferWidth = 1920;
           // graphics.PreferredBackBufferHeight = 1050;
    
            //This defines whether the game runs in Fullscreen
           graphics.IsFullScreen = false;
         
            world = new World(new Vector2(0, 20));

            r = new Vector2(64f * Pixel2Meter, 0);
        }
        protected override void Initialize()
        {
           
           

            base.Initialize();

  

        }
        protected override void LoadContent()
        {

            gibbon = new Gibbon(this);
            gameStartScreen = new GameStartScreen(this);
            gameOverScreen = new GameOverScreen(this);
            enterTextScreen = new EnterTextScreen(this);
            leaderboardEasy = new Leaderboard(this,"Easy",600);
            leaderboardHard = new Leaderboard(this,"Hard",1200);

            scrollingBG = new ScrollingBackground(this);
            healthBar = new HealthBar(this);

            int v_w = graphics.GraphicsDevice.Viewport.Width;
            int v_h = graphics.GraphicsDevice.Viewport.Height;
            screenCenter = new Vector2(v_w / 2f, v_h / 2f);

            Restart();
      
            //Load textures to sprites
            circleSprite = Content.Load<Texture2D>("circleSprite");
            staticBG = Content.Load<Texture2D>("staticbg");
            bananaSprite = Content.Load<Texture2D>("bananas");
            gameOverSound = Content.Load<SoundEffect>("pacman_death");
            eatFruitSound = Content.Load<SoundEffect>("pacman_eatfruit");
            bgMusic = Content.Load<Song>("DonkeyKong");

            //Load any fonts
            quartz_small = Content.Load<SpriteFont>("SpriteFont1");
            quartz_big = Content.Load<SpriteFont>("SpriteFont2");
            Andy = Content.Load<SpriteFont>("SpriteFont4");
            Andy_big = Content.Load<SpriteFont>("SpriteFont5");
            Arial = Content.Load<SpriteFont>("SpriteFont3");   

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        public void Restart()
        {
      
            current_time = 0;

            setMode(PREGAME);
            gibbon.Restart(gameStartScreen.gameDiff);
            scrollingBG.Restart();

            gameOverScreen.soundPlayed = false;

            healthBar.Restart();

            //Setup camera controls
            view = Matrix.Identity;
            cameraPosition = Vector2.Zero;
            
            bananaActive = false;
            bananaPosition = (screenCenter * Pixel2Meter) + new Vector2(3.0f, -1.5f);

          
           
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void gamePlayUpdate(GameTime gameTime)
        {
                
            current_time += gameTime.ElapsedGameTime.Milliseconds;
            HandleKeyboard();

            if (bananaActive)
                    checkBananaCollision();
    
            pushCameraFoward();
 
            gibbon.Update();
                
            scrollingBG.Update();
 
            if (gibbon.jointSpinning)
                    healthBar.changeHealth(-0.1f);
               
            checkGameTime(gameTime);

            if (!bananaActive || isLeftofScreen(bananaPosition))
                generateBanana();
               
            /* Multiple physics steps per game loop.*/
            for (int i = 0; i < 16; i++)  
                world.Step(0.001f);

            }

        protected override void Update(GameTime gameTime)
        {

            if (mode == PREGAME)
            {
                gameStartScreen.Update();
            }
            else if (mode == PLAY)
            {
                gamePlayUpdate(gameTime);
            }
            else if (mode == GAMEOVER)
            {
                gameOverScreen.Update();
            }
            else if (mode == ENTERNAME)
            {
                enterTextScreen.Update();
            }
            else if (mode == LEADERBOARD)
            {
                leaderboardEasy.Update();
                leaderboardHard.Update();
            }
               
            base.Update(gameTime);
        }

        #region Handle input
        public bool keyJustPressed(KeyboardState oldState, KeyboardState newState, Keys thisKey)
        {
            return (newState.IsKeyDown(thisKey) && (!oldState.IsKeyDown(thisKey)));
        }

        public bool keyJustReleased(KeyboardState oldState, KeyboardState newState, Keys thisKey)
        {
            return (newState.IsKeyUp(thisKey) && oldState.IsKeyDown(thisKey));
        }

        public bool padJustReleased(GamePadState oldState, GamePadState newState, Buttons thisKey)
        {
            return (newState.IsButtonUp(thisKey) && oldState.IsButtonDown(thisKey));
        }

        private void HandleKeyboard()
        {
            KeyboardState newKeyState = Keyboard.GetState();
            GamePadState newPadState = GamePad.GetState(PlayerIndex.One);
            bool connected = newPadState.IsConnected;

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();


                if (keyJustPressed(oldKeyState,newKeyState,Keys.S)||padJustPressed(oldGamePad,newPadState,Buttons.RightTrigger))
                {
                    if (!gibbon.inFreeFlight())
                    {
                        gibbon.applyTorque(-1f);
                        
                    }
                }

                if (keyJustPressed(oldKeyState,newKeyState,Keys.A)||padJustPressed(oldGamePad,newPadState,Buttons.LeftTrigger))
                {
                    if (!gibbon.inFreeFlight())
                    {
                        gibbon.applyTorque(1f);
                    }

                }

                if (keyJustReleased(oldKeyState, newKeyState, Keys.A) || keyJustReleased(oldKeyState,newKeyState,Keys.S))
                {
                    gibbon.endTorque();
                }

                if (padJustReleased(oldGamePad, newPadState, Buttons.LeftTrigger) || padJustReleased(oldGamePad, newPadState, Buttons.RightTrigger))
               {
                   gibbon.endTorque();
               }
                

                if (keyJustPressed(oldKeyState, newKeyState, Keys.W)||padJustPressed(oldGamePad,newPadState,Buttons.B))
                {
                    if (gibbon.inFreeFlight())
                        gibbon.switchClamped();
                    else
                        gibbon.releaseClamp();
                }
                  

                if (keyJustPressed(oldKeyState, newKeyState, Keys.Q)||padJustPressed(oldGamePad,newPadState,Buttons.A))
                   gibbon.switchClamped();

             

                if (newKeyState.IsKeyDown(Keys.Escape)||padPressed(newPadState,Buttons.Back))
                    Exit();

            oldKeyState = newKeyState;
            oldGamePad = newPadState;
        }

        public bool padPressed(GamePadState state, Buttons button)
        {
            if (state.IsConnected)
            {
                return state.IsButtonDown(button);
            }

            else
                return false;


        }

        public bool padJustPressed(GamePadState oldState, GamePadState state, Buttons button)
        {
            if (state.IsConnected)
            {
                return (state.IsButtonDown(button) && oldState.IsButtonUp(button));
            }

            else
                return false;


        }
        #endregion

        #region Drawing Functions
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(bgColor);
           
            switch (mode)
            {

                case -1:
                    gameStartScreen.Draw(spriteBatch);
                    break;
                case 0:
                    gamePlayDraw(gameTime, spriteBatch);
                    break;
                case 1:
                    gameOverScreen.Draw(spriteBatch);
                    break;
                case 2:
                    enterTextScreen.Draw(spriteBatch);
                    break;
                case 3:
                    spriteBatch.Begin();
                    spriteBatch.Draw(staticBG, Vector2.Zero, Color.White);
                    DrawText(new Vector2(screenCenter.X, 100), "High Scores", quartz_big,true,Color.Black);
                    spriteBatch.End();

                    leaderboardEasy.Draw(spriteBatch);
                    leaderboardHard.Draw(spriteBatch);
                    break;
            }

            base.Draw(gameTime);
        }
        private void gamePlayDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            /* This space is for drawing things that do not move with the camera, i.e. have a position in physical space. */
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, view);


            scrollingBG.Draw(spriteBatch);

            if (bananaActive)
                DrawBanana(gameTime);

            gibbon.Draw(spriteBatch);

            spriteBatch.End();


            /* This space is for drawing things that do move with camera, i.e. HUDs, static backgrounds, etc.) They draw directly to the screen, and ignore the camera completely.*/
            spriteBatch.Begin();

            healthBar.Draw(spriteBatch);
        
            DrawDistance();
            DrawTimer();

            spriteBatch.End();

        }   
        private void DrawDistance()
        {
            DrawText(new Vector2(300f, 5), "Distance: ", quartz_small);
            DrawText(new Vector2(410f, 5f), ((gibbon.getDistance() * 10.0f) / 10.0f).ToString(), quartz_small);
        }
        private void DrawBanana(GameTime gameTime)
        {

            Vector2 bananaPos = bananaPosition * Meter2Pixel;
            Vector2 bananaOrigin = new Vector2(bananaSprite.Width / 2f, bananaSprite.Height / 2f);

            float scale = 0.7f* (float) (Math.Abs(Math.Sin(current_time * .0050))) + 0.3f;
            spriteBatch.Draw(circleSprite, bananaPos, null, Color.Red,   0, new Vector2(circleSprite.Width / 2f, circleSprite.Height / 2f), scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(bananaSprite, bananaPos, null, Color.White, 0, bananaOrigin, 0.1f, SpriteEffects.None, 0f);

        }
        public  void DrawText(Vector2 position, String text, SpriteFont this_font)
        {
            spriteBatch.DrawString(this_font, text, position, Color.Black);
        }

        public void DrawText(Vector2 position, String text, SpriteFont this_font, Color textColor)
        {
            spriteBatch.DrawString(this_font, text, position, textColor);
        }
        public void DrawText(Vector2 position, String text, SpriteFont this_font, bool useCenter)
        {
            Vector2 textSize;
            if (useCenter)
                textSize = this_font.MeasureString(text);
            else
                textSize = Vector2.Zero;

            spriteBatch.DrawString(this_font, text, position - textSize / 2.0f, Color.Black);
        }

        public void DrawText(Vector2 position, String text, SpriteFont this_font, bool useCenter,Color textColor)
        {
            Vector2 textSize;
            if (useCenter)
                textSize = this_font.MeasureString(text);
            else
                textSize = Vector2.Zero;

            spriteBatch.DrawString(this_font, text, position - textSize / 2.0f, textColor);
        }
        private void DrawTimer()
        {
            DrawText(new Vector2(graphics.GraphicsDevice.Viewport.Width -300, 25), "Time Remaining: ", quartz_small);
            DrawText(new Vector2(graphics.GraphicsDevice.Viewport.Width - 140, 25), (max_time - current_time / 1000f).ToString(), quartz_small);


        }
        #endregion

        private void checkBananaCollision()
        {

            float bananaRadius = 0.7f;

            float arm1Dist = (gibbon.rectBody1.Position - bananaPosition).Length();
            float arm2Dist = (gibbon.rectBody2.Position - bananaPosition).Length();
            float jointDist = (gibbon.connectRectJoint.WorldAnchorA - bananaPosition).Length();

            if ((arm1Dist < bananaRadius) || (arm2Dist < bananaRadius) || (jointDist < bananaRadius))
            {
                healthBar.changeHealth(15.0f);
                bananaActive = false;
                eatFruitSound.Play();
            }


        }
        private void checkGameTime(GameTime gametime)
        {

            if ((current_time / 1000f) > max_time)
            {

                endGame(3);

            }

        }

        #region Camera Functions
        private void pushCameraFoward()
        {

            gibbon.updateMaxPosition();

            cameraPosition.X = -gibbon.currentMaxPosition * Meter2Pixel + screenCenter.X;

            view = Matrix.CreateTranslation(new Vector3(cameraPosition - screenCenter, 0f)) *
                Matrix.CreateTranslation(new Vector3(screenCenter, 0f));




        }
        public bool isOffscreen(Vector2 worldCoordinate)
        {
            worldCoordinate *= Meter2Pixel;

            //Console.Write("Pos: " + worldCoordinate.ToString() + " px \n");
            //Console.Write("Cam: " + cameraPosition.ToString() + " px \n");
           // worldCoordinate += screenCenter;

            bool xLeft = (worldCoordinate.X < -cameraPosition.X);
            bool yHigh = (worldCoordinate.Y < -cameraPosition.Y);

            bool xRight = (-cameraPosition.X + 2.0f * screenCenter.X) < worldCoordinate.X;
            bool yLow = (-cameraPosition.Y + 2.0f * screenCenter.Y) < worldCoordinate.Y;
            //bool yLow = (worldCoordinate.Y + 2.0f*screenCenter.Y) > -cameraPosition.Y;
           // bool yBad = Math.Abs(cameraPosition.Y - worldCoordinate.Y) > screenCenter.Y;

            return (xLeft || yHigh || xRight || yLow);

        
        }
        public bool isLeftofScreen(Vector2 testPoint)
        {
            testPoint *= Meter2Pixel;

            return (testPoint.X < -cameraPosition.X);
      
        }
        #endregion

        private void generateBanana()
        {

            bananaPosition = new Vector2(gibbon.connectRectJoint.WorldAnchorA.X,0.0f) + new Vector2((float) graphics.GraphicsDevice.Viewport.Width*Pixel2Meter, getBananaHeight());
            bananaActive = true;

        }
        private float getBananaHeight()
        {


            return (rand.Next(graphics.GraphicsDevice.Viewport.Height - 200) + 100f) * Pixel2Meter;
            
        }

        public float getScore()
        {

            return (((float) gibbon.getDistance() * 10.0f) / 10.0f);


        }

        public void endGame(int exitCase)
        {
            gameOverScreen.exitCase = exitCase;
            GamePad.SetVibration(PlayerIndex.One, 0, 0);
            
            if (gameStartScreen.gameDiff == easy)
                needName = leaderboardEasy.newLeader(getScore());
            else
                needName = leaderboardHard.newLeader(getScore());

            setMode(GAMEOVER);      
        }
        public void setMode(int m)
        {
            mode = m;
        }

        public void appendLeaderboard()
        {
            if (gameStartScreen.gameDiff == easy)
                leaderboardEasy.addLeader(new Leader(enterTextScreen.getIntials(), getScore()));
            else
                leaderboardHard.addLeader(new Leader(enterTextScreen.getIntials(), getScore()));


          
        }

        
    }
}
