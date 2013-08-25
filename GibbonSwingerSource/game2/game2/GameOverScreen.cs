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
    class GameOverScreen
    {
         
        GibbonSwinger game; 
        KeyboardState lastState;
        GamePadState lastPadState;

        public int exitCase;
        public bool soundPlayed = false;

       

        public GameOverScreen(GibbonSwinger game)
        {
            this.game = game;
            exitCase = 1;
         
           
        } 

        public void Update()
        {
            
            
            
            KeyboardState currentState = Keyboard.GetState();
            GamePadState currentPadState = GamePad.GetState(PlayerIndex.One);

            if (!soundPlayed)
            {
                game.gameOverSound.Play();
                soundPlayed = true;
            }

            if (game.keyJustPressed(lastState,currentState,Keys.Escape)||game.padJustPressed(lastPadState,currentPadState,Buttons.Back))
            {
                game.Exit();
            }

            
            if (game.keyJustReleased(lastState,currentState,Keys.Enter)||game.padJustReleased(lastPadState,currentPadState,Buttons.A))
            {

                if (game.needName)
                    game.setMode(game.ENTERNAME);
                else
                    game.setMode(game.LEADERBOARD);

            }

            lastState = currentState;
            lastPadState = currentPadState;

        }

        public void Draw(SpriteBatch spriteBatch)
        {
                String text = " ";
                if (exitCase == 1)
                    text = "Out of energy.";

                if (exitCase == 2)
                    text = "Gibbon out of bounds.";

                if (exitCase == 3)
                    text = "Out of time.";

                spriteBatch.Begin();

                spriteBatch.Draw(game.staticBG, Vector2.Zero, Color.White);
                game.DrawText(new Vector2(game.screenCenter.X, 100f), "Game over!", game.quartz_big, true,Color.Black);
                game.DrawText(game.screenCenter, text, game.quartz_big, true,Color.Black);
                game.DrawText(new Vector2(game.screenCenter.X, 800f), "Your distance: " + game.getScore().ToString() + " m", game.quartz_big, true,Color.Black);


                spriteBatch.End();
            
        }

    }
}
