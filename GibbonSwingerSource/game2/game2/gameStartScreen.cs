using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace game2
{
    class GameStartScreen
    {
        private GibbonSwinger game;
        private KeyboardState lastState;
        private GamePadState lastPadState;
        bool musicPlaying = false;
        public int gameDiff;
        const int easy = 1;
        const int hard = 2;

        Texture2D startbg;


        public GameStartScreen(GibbonSwinger game)
        {
            this.game = game;
            lastState = Keyboard.GetState();
            lastPadState = GamePad.GetState(PlayerIndex.One);
            gameDiff = easy;

            startbg = game.Content.Load<Texture2D>("startscreen");
    

        }

        public void Update()
        {
            KeyboardState currentState = Keyboard.GetState();
            GamePadState currentPadState = GamePad.GetState(PlayerIndex.One);

            if (!musicPlaying)
            {
                MediaPlayer.Play(game.bgMusic);
                MediaPlayer.IsRepeating = true;
                musicPlaying = true;
            }


            if (game.keyJustPressed(lastState, currentState, Keys.Escape)||game.padPressed(currentPadState,Buttons.Back))
            {
                game.Exit();
            }


            if (game.keyJustPressed(lastState, currentState, Keys.Enter)||game.padJustPressed(lastPadState,currentPadState,Buttons.Start))
            {
                game.setMode(game.PLAY);
                game.ResetElapsedTime();
            }


            if (game.keyJustPressed(lastState, currentState, Keys.Right) || game.padJustPressed(lastPadState, currentPadState, Buttons.DPadRight))
            {
                gameDiff = hard;
            }

            if (game.keyJustPressed(lastState, currentState, Keys.Left) || game.padJustPressed(lastPadState, currentPadState, Buttons.DPadLeft))
            {
                gameDiff = easy;
            }

            lastState = currentState;
            lastPadState = currentPadState;

        }

        public void Draw(SpriteBatch spriteBatch)
        {

  
            spriteBatch.Begin();
            //spriteBatch.Draw(startbg, Vector2.Zero, null, Color.White, 0, Vector2.Zero, startbg.Width / game.GraphicsDevice.Viewport.Width, SpriteEffects.None, 0f);
            spriteBatch.Draw(startbg,Vector2.Zero, null, Color.White);
    
            Vector2 bananaPos;
            if (gameDiff == easy)
                bananaPos = new Vector2(game.screenCenter.X - 240f, 630f);
            else
                bananaPos = new Vector2(game.screenCenter.X + 390f, 630f);

            spriteBatch.Draw(game.bananaSprite, bananaPos, null, Color.White, 0, new Vector2(game.bananaSprite.Width / 2f, game.bananaSprite.Height / 2f), .17f, SpriteEffects.None, 0f);

        
            spriteBatch.End();
        }

    }
}
