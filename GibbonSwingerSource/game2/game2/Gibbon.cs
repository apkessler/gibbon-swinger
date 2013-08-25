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
    class Gibbon
    {
        GibbonSwinger game;

        #region Textures & Sprites
        Texture2D gibbonSprite;
        Texture2D gibbonArm;
        Texture2D circleSprite;
        Texture2D rectSprite;
        #endregion
        #region Bodies
        public Body rectBody1;
        public Body rectBody2;
        Body currentClamped; //this either points to rectBody1 or rectBody2, depending on which leg is fixed
        Body notClamped;
        #endregion
        #region Joints
        FixedRevoluteJoint clampedJoint;
        //public DistanceJoint connectRectJoint;
        public RevoluteJoint connectRectJoint;
        #endregion
        #region Misc.
        bool freeFlight;
    
        int gameMode;
        Vector2 r;
        public float currentMaxPosition;
        public bool jointSpinning { private set; get; }
        #endregion

        const int easy = 1;
        const int hard = 2;

        int spin_direction;

        public Gibbon(GibbonSwinger aGame)
        {
            game = aGame;
            r = game.r;
            this.Restart(1);

            gibbonSprite = game.Content.Load<Texture2D>("gibbon_body");
            gibbonArm = game.Content.Load<Texture2D>("gibbon_arm2");
            circleSprite = game.Content.Load<Texture2D>("circleSprite");
            rectSprite = game.Content.Load<Texture2D>("rectSprite");
        }
        public void Restart(int mode)
        {
            World world = game.world;
            float Pixel2Meter = game.Pixel2Meter;
            Vector2 screenCenter = game.screenCenter;
            gameMode = mode;

            freeFlight = false;
            if (connectRectJoint != null)
                world.RemoveJoint(connectRectJoint);

            if (clampedJoint != null)
                world.RemoveJoint(clampedJoint);

            if (rectBody1 != null)
                world.RemoveBody(rectBody1);

            if (rectBody2 != null)
                world.RemoveBody(rectBody2);

            Vector2 rectPosition1 = (screenCenter * Pixel2Meter) + new Vector2(-2f, -2f);
            //create rectange fixture
            rectBody1 = BodyFactory.CreateRectangle(world, 128f * Pixel2Meter, 16f * Pixel2Meter, 1f, rectPosition1);
            rectBody1.BodyType = BodyType.Dynamic;
            rectBody1.Restitution = 1f;
            rectBody1.Friction = 0.0f;

            Vector2 rectPosition2 = rectPosition1;
            //create rectange fixture
            rectBody2 = BodyFactory.CreateRectangle(world, 128f * Pixel2Meter, 16f * Pixel2Meter, 1f, rectPosition2);
            rectBody2.BodyType = BodyType.Dynamic;
            rectBody2.Restitution = 1f;
            rectBody2.Friction = 0.0f;

            rectBody1.IgnoreCollisionWith(rectBody2);
            rectBody2.IgnoreCollisionWith(rectBody1);
            rectBody1.LinearDamping = 0;
            rectBody2.LinearDamping = 0;

            clampedJoint = JointFactory.CreateFixedRevoluteJoint(world, rectBody1, new Vector2(-64f * Pixel2Meter, 0), rectPosition1 + new Vector2(-64f * Pixel2Meter, 0));
            currentClamped = rectBody1;
            notClamped = rectBody2;

            setupClampedJoint();


            connectRectJoint = JointFactory.CreateRevoluteJoint(world,rectBody1, rectBody2, new Vector2(+64f * Pixel2Meter, 0));
            stopSpinning();


            currentMaxPosition = connectRectJoint.WorldAnchorA.X;

        }
        private void setupClampedJoint()
        {
            clampedJoint.MotorSpeed = 0.0f;
            clampedJoint.MaxMotorTorque = 2.0f;
            clampedJoint.MotorEnabled = true;

        }
        private void stopSpinning()
        {
            connectRectJoint.MotorSpeed = 0.0f;
            connectRectJoint.MotorEnabled = true;
            connectRectJoint.MaxMotorTorque = 2f;
            jointSpinning = false;

        }

        private void startSpinning(float dir)
        {
            jointSpinning = true;
            spin_direction = (int) dir;

            if (gameMode == hard)
            {
                connectRectJoint.MotorSpeed = dir * 5.0f;
                connectRectJoint.MaxMotorTorque = 100.0f;
            }

        }
        public void Update()
        {
            checkGibbonBounds();

           if (jointSpinning && gameMode == easy)
              currentClamped.ApplyTorque(spin_direction*300f);


        }
        public void Draw(SpriteBatch spriteBatch)
        {
            float Meter2Pixel = game.Meter2Pixel;
   
            /*Rect1 position and origin */
            Vector2 rectPos1 = rectBody1.Position * Meter2Pixel;
            float rectRot1 = rectBody1.Rotation;
            Vector2 rectOrigin1 = new Vector2(rectSprite.Width / 2f, rectSprite.Height / 2f);

            /*Rect1 position and origin */
            Vector2 rectPos2 = rectBody2.Position * Meter2Pixel;
            float rectRot2 = rectBody2.Rotation;
            Vector2 rectOrigin2 = new Vector2(rectSprite.Width / 2f, rectSprite.Height / 2f);

            //Align sprite center to body position
            Vector2 circleOrigin = new Vector2(circleSprite.Width / 2f, circleSprite.Height / 2f);

            //draw clamped point
            if (!inFreeFlight())
                spriteBatch.Draw(circleSprite, currentClamped.GetWorldPoint(-r) * Meter2Pixel, null, Color.White, 0, circleOrigin, 0.2f, SpriteEffects.None, 0f);

            //draw body
            //spriteBatch.Draw(rectSprite, rectPos1, null, Color.White, rectRot1, rectOrigin1, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(gibbonArm, connectRectJoint.WorldAnchorA*Meter2Pixel, null, Color.White, rectRot1, new Vector2(gibbonArm.Width/2f,gibbonArm.Height/2f), 0.5f, SpriteEffects.FlipVertically, 0f);
           
            spriteBatch.Draw(gibbonSprite, connectRectJoint.WorldAnchorA * Meter2Pixel, null, Color.White, 0, new Vector2(gibbonSprite.Width / 2f, gibbonSprite.Height / 2f + -100f), 0.4f, SpriteEffects.None, 0f);

            //draw rect1

            //spriteBatch.Draw(rectSprite, rectPos2, null, Color.Blue, rectRot2, rectOrigin2, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(gibbonArm, connectRectJoint.WorldAnchorA * Meter2Pixel, null, Color.White, rectRot2, new Vector2(gibbonArm.Width / 2f, gibbonArm.Height / 2f), 0.5f, SpriteEffects.FlipVertically, 0f);
         


        }
        private void checkGibbonBounds()
        {
            if (game.isOffscreen(connectRectJoint.WorldAnchorA) && game.isOffscreen(rectBody1.WorldCenter) && game.isOffscreen(rectBody2.WorldCenter) && game.isOffscreen(clampedJoint.WorldAnchorA))
            {
                Console.Write("Robot out of bounds.\n");
                game.endGame(2);
            }
        }
        public bool inFreeFlight()
        {
            return freeFlight;

        }
        public void switchClamped()
        {
            if (clampedJoint != null)
                game.world.RemoveJoint(clampedJoint);

            freeFlight = false;

            Body oldCurrentClamped = currentClamped;

            clampedJoint = JointFactory.CreateFixedRevoluteJoint(game.world, notClamped, -r, notClamped.GetWorldPoint(-r));
            setupClampedJoint();

            currentClamped = notClamped;
            notClamped = oldCurrentClamped;


        }
        public void releaseClamp()
        {
            game.world.RemoveJoint(clampedJoint);
            freeFlight = true;
        }
        public void applyTorque(float direction)
        {
            float vibAmount = 0.3f;
            GamePad.SetVibration(PlayerIndex.One, vibAmount, vibAmount);

            startSpinning(direction);

 

            
        }
        public void endTorque()
        {
            stopSpinning();
            GamePad.SetVibration(PlayerIndex.One, 0, 0);

        }
        public void updateMaxPosition()
        {
            if (connectRectJoint.WorldAnchorA.X > currentMaxPosition)
            {
                currentMaxPosition = connectRectJoint.WorldAnchorA.X;
            }

        }
        public float getDistance()
        {

            return (float) Math.Truncate(Math.Max(rectBody1.WorldCenter.X, rectBody2.WorldCenter.X)*10.0f)/10.0f;
        }


    }
}
