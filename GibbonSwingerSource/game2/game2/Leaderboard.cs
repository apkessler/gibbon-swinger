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

    class Leader
    {
        private String name;
        private float score;

        public Leader(String n, float s)
        {
            this.name = n;
            this.score = s;

        }

        public String getName()
        {
            return this.name;
        }

        public float getScore()
        {
            return this.score;
        }

        public override String ToString()
        {
            return (this.name + ": " + this.score.ToString());
        }

    }

    class Leaderboard
    {

        GibbonSwinger game;
        KeyboardState lastState;
        GamePadState lastPadState;



        //Queue<leader> leaderboard;
        List<Leader> leaders;


        const int num_to_follow = 10;
        String title;
        int drawCenter;

        public Leaderboard(GibbonSwinger game, String aTitle, int aDrawCenter)
        {
            this.game = game;
            this.title = aTitle;
            this.drawCenter = aDrawCenter;
            leaders = new List<Leader>(num_to_follow);


            for (int i = 0; i < num_to_follow; i++)
            {
                leaders.Add(new Leader("LIMS", (float)20 - 2 * i));
            }

        }

        public void Update()
        {
            KeyboardState currentState = Keyboard.GetState();
            GamePadState currentPadState = GamePad.GetState(PlayerIndex.One);

            if (game.keyJustPressed(lastState, currentState, Keys.Escape) || game.padJustPressed(lastPadState, currentPadState, Buttons.Back))
            {
                game.Exit();
            }


            if (game.keyJustReleased(lastState, currentState, Keys.Enter) || game.padJustReleased(lastPadState, currentPadState, Buttons.A))
            {
                game.Restart();
            }

            #region Replace bad names
            if (game.keyJustPressed(lastState, currentState, Keys.D0))
                {
                    leaders[0] = new Leader("LIMS", leaders[0].getScore());
                }

                if (game.keyJustPressed(lastState, currentState, Keys.D1))
                {
                    leaders[1] = new Leader("LIMS", leaders[1].getScore());
                }
                if (game.keyJustPressed(lastState, currentState, Keys.D2))
                {
                    leaders[2] = new Leader("LIMS", leaders[2].getScore());
                }
                if (game.keyJustPressed(lastState, currentState, Keys.D3))
                {
                    leaders[3] = new Leader("LIMS", leaders[3].getScore());
                }
                if (game.keyJustPressed(lastState, currentState, Keys.D4))
                {
                    leaders[4] = new Leader("LIMS", leaders[4].getScore());
                }
                if (game.keyJustPressed(lastState, currentState, Keys.D5))
                {
                    leaders[5] = new Leader("LIMS", leaders[5].getScore());
                }
                if (game.keyJustPressed(lastState, currentState, Keys.D6))
                {
                    leaders[6] = new Leader("LIMS", leaders[6].getScore());
                }
                if (game.keyJustPressed(lastState, currentState, Keys.D7))
                {
                    leaders[7] = new Leader("LIMS", leaders[7].getScore());
                }
                if (game.keyJustPressed(lastState, currentState, Keys.D8))
                {
                    leaders[8] = new Leader("LIMS", leaders[8].getScore());
                }
                if (game.keyJustPressed(lastState, currentState, Keys.D9))
                {
                    leaders[9] = new Leader("LIMS", leaders[9].getScore());
                }
            #endregion

            lastState = currentState;
            lastPadState = currentPadState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Begin();

           
            game.DrawText(new Vector2(drawCenter , 300), title, game.quartz_big, true,Color.Black);

            for (int i = 0; i < num_to_follow; i++)
            {
                game.DrawText(new Vector2(drawCenter-100, 400 + 50 * i), ((i + 1) + ". " + leaders[i].ToString()), game.quartz_small,Color.Black);
            }

            spriteBatch.End();


        }


        public void addLeader(Leader L)
        {


            int i = 0;

            while (i < num_to_follow && leaders[i].getScore() > L.getScore())
            {
                i++;
            }

            int new_index = i; //where this leader goes on the board

            if (new_index < num_to_follow)
            {
                leaders.Insert(new_index, L);
                leaders.RemoveAt(num_to_follow); //get rid of the last element

            }


        }

        public bool newLeader(float s)
        {
            int i = 0;

            while (i < num_to_follow && leaders[i].getScore() > s)
            {
                i++;
            }

            return (i < num_to_follow);

        }



    }

}