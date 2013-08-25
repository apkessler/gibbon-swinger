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
    class Tree
    {
        Game1 game;
        Vector2 position;
        bool active;
        Random rand;
        int treeNum;

        public Tree(Game1 aGame, Vector2 camera_center)
        {
            this.game = aGame;
            position = camera_center + new Vector2(game.GraphicsDevice.Viewport.Width*1.3f, game.GraphicsDevice.Viewport.Height+100f);
            active = true;
            rand = new Random();
            treeNum = rand.Next(2);
  

        }

        public void Update()
        {
            active = game.isLeftofScreen(position * game.Pixel2Meter + new Vector2(-2.0f*game.GraphicsDevice.Viewport.Width,0));
            //Console.WriteLine("Tree: " + position.X);
  
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            if (game.tree1 == null)
            {
                Console.WriteLine("Tree texture is null...");
            }
            else
            {
                //spriteBatch.Draw(myTexture, position, Color.White);
                Texture2D t2D;

                if (treeNum == 0)
                    t2D = game.tree1;
                else
                    t2D = game.tree2;

                spriteBatch.Draw(t2D, position, null, Color.White, 0, new Vector2(t2D.Width / 2f, t2D.Height), 1f, SpriteEffects.None, 0f);
            }

        }
        public bool isActive()
        {
            return active;
        }
        


    }


    class Forest
    {

        List<Tree> treeList;
        Game1 game;
        Random rand;
        List<Tree> toChopDown;

        public Forest(Game1 aGame)
        {
            treeList = new List<Tree>();
            toChopDown = new List<Tree>();
            game = aGame;
            rand = new Random();
        }

        public void Update()
        {
            foreach (Tree t in treeList)
            {
                t.Update();
            }

            int x = rand.Next(1000);

            if (x < 20)
            { 
                addTree();
                chopDownTrees();
                Console.WriteLine(treeList.Count);
            }

           
            
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tree t in treeList)
                t.Draw(spriteBatch);
        }

        public void addTree()
        {
            treeList.Add(new Tree(this.game,-1 * this.game.cameraPosition));
            Console.WriteLine("Adding new tree.");
        }

        private void chopDownTrees()
        {
            Console.WriteLine("Chopping down trees.");
            //grow through the forest, and take note of each tree that's off screen (on the left)
            foreach (Tree t in treeList)
            {
                if (!t.isActive())
                    toChopDown.Add(t);
            }

            //now go through the list of trees to chop down, and remove them from the tree list
            foreach (Tree t in toChopDown)
            {
                treeList.Remove(t);
                Console.WriteLine("Removing tree.");
            }

            //clear the chop down list
            toChopDown = new List<Tree>();





        }



    }

}
