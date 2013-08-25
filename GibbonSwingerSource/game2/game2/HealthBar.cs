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
    class HealthBar
    {
        GibbonSwinger game;

        Texture2D healthBarFill;
        Texture2D healthBarBorder;

        const float maxHealth = 100.0f;
        float currentHealth;

        public HealthBar(GibbonSwinger aGame)
        {
            game = aGame;
            healthBarFill = game.Content.Load<Texture2D>("HealthBarFill");
            healthBarBorder = game.Content.Load<Texture2D>("HealthBarBorder");

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float perct_health = currentHealth / maxHealth;

            Vector2 healthBarPosition = new Vector2(30f, 30f);

            game.DrawText(healthBarPosition + new Vector2(0f, -25f), "Energy", game.quartz_small);

            //Draw the box around the health bar
            spriteBatch.Draw(healthBarBorder, healthBarPosition, null, Color.Gray);

            //Draw the current health level based on the current Health
            spriteBatch.Draw(healthBarFill, new Rectangle((int)healthBarPosition.X + 1, (int)healthBarPosition.Y, (int)(healthBarFill.Width * perct_health), 43), null, Color.Red);

        }


        public void Restart()
        {
            currentHealth = maxHealth;
        }

        public void changeHealth(float dH)
        {
            currentHealth += dH;

            if (currentHealth > 100)
                currentHealth = 100;

            if (currentHealth < 0)
            {
                currentHealth = 0;

                Console.Write("Out of energy!\n");
                game.endGame(1);

            }

        }


    }
}
