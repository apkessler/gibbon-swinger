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
    class EnterTextScreen
    {

  
        GibbonSwinger game;

        GamePadState lastPadState;
        KeyboardState lastKeyState;

        int currentLetter = 0;
        int[] currentIndices;

        public String output;

        public EnterTextScreen(GibbonSwinger game)
        {
            this.game = game;

            currentIndices = new int[3];
            currentIndices[0] = (int)'A';
            currentIndices[1] = (int)'A';
            currentIndices[2] = (int)'A';

            updateOutput();
        }

        private void updateOutput()
        {
            output = ((char)currentIndices[0]).ToString() + ((char)currentIndices[1]).ToString() + ((char)currentIndices[2]).ToString();
        }

        public void Update()
        {
            GamePadState newPadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState newKeyState = Keyboard.GetState();

            if (game.keyJustPressed(lastKeyState,newKeyState,Keys.Up)||game.padJustPressed(lastPadState, newPadState, Buttons.DPadUp))
            {
                currentIndices[currentLetter]++;
                if (currentIndices[currentLetter] > (int)'Z')
                    currentIndices[currentLetter] = (int)'A';

            }

            if (game.keyJustPressed(lastKeyState, newKeyState, Keys.Down) || game.padJustPressed(lastPadState, newPadState, Buttons.DPadDown))
            {
                
                currentIndices[currentLetter]--;
                if (currentIndices[currentLetter] < (int)'A')
                    currentIndices[currentLetter] = (int)'Z';              
            }

            if (game.keyJustPressed(lastKeyState, newKeyState, Keys.Right) || game.padJustPressed(lastPadState, newPadState, Buttons.DPadRight))
            {
                
                currentLetter++;
                if (currentLetter > (currentIndices.Length - 1))
                    currentLetter = 0;              
            }

            if (game.keyJustPressed(lastKeyState, newKeyState, Keys.Left) || game.padJustPressed(lastPadState, newPadState, Buttons.DPadLeft))
            {

                currentLetter--;
                if (currentLetter < 0)
                    currentLetter = (currentIndices.Length - 1);
            }

            if (game.keyJustReleased(lastKeyState,newKeyState,Keys.Enter)||game.padJustReleased(lastPadState, newPadState, Buttons.A))
            {
                game.appendLeaderboard();
                game.setMode(game.LEADERBOARD);
            }

            lastPadState = newPadState;
            lastKeyState = newKeyState;

            updateOutput();
           




        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(game.staticBG, Vector2.Zero, Color.White);
            game.DrawText(new Vector2(game.screenCenter.X - 100, 300), "_", game.quartz_big, true);
            game.DrawText(new Vector2(game.screenCenter.X, 300), "_", game.quartz_big, true);
            game.DrawText(new Vector2(game.screenCenter.X + 100, 300), "_", game.quartz_big, true);


           game.DrawText(new Vector2(game.screenCenter.X- 100, 300), ((char) currentIndices[0]).ToString(), game.quartz_big, true);
           game.DrawText(new Vector2(game.screenCenter.X, 300), ((char)currentIndices[1]).ToString(), game.quartz_big, true);
           game.DrawText(new Vector2(game.screenCenter.X + 100, 300), ((char)currentIndices[2]).ToString(), game.quartz_big, true);

           game.DrawText(new Vector2(game.screenCenter.X, 100), "New high score!", game.quartz_big, true);
           game.DrawText(new Vector2(game.screenCenter.X, 200), "Please enter your initials, using the DPAD or arrow keys. Press Start or Return when done.", game.quartz_small, true);

           Vector2 bananaPos = new Vector2(game.screenCenter.X - 100 + 100 * currentLetter, 430);
           Vector2 bananaOrigin = new Vector2(game.bananaSprite.Width / 2f, game.bananaSprite.Height / 2f);
           spriteBatch.Draw(game.bananaSprite, bananaPos, null, Color.White, 0, bananaOrigin, 0.1f, SpriteEffects.None, 0f);


            spriteBatch.End();

        }


        public String getIntials()
        {

            return output;

        }


    }
}





    

