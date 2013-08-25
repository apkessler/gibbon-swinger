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
    class ScrollingBackground
    {
        Texture2D bgSprite;
        GibbonSwinger game;

        Queue<Vector2> positions;

        public ScrollingBackground(GibbonSwinger aGame)
        {
            game = aGame;
            
            bgSprite = game.Content.Load<Texture2D>("scrollbg");
            this.Restart();
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            
            spriteBatch.Draw(bgSprite, positions.ElementAt(0), Color.White);
            spriteBatch.Draw(bgSprite, positions.ElementAt(1), Color.White);
      
        }


        public void Update()
        {
            if (-game.cameraPosition.X > (positions.Peek().X + bgSprite.Width))
            {
                positions.Dequeue();
                positions.Enqueue(new Vector2(positions.Peek().X + bgSprite.Width, 0));
            }

        }


        public void Restart()
        {
            positions = new Queue<Vector2>();
      
            positions.Enqueue(new Vector2(-bgSprite.Width, 0));
            positions.Enqueue(new Vector2(0, 0));
       
        }

    }
}
