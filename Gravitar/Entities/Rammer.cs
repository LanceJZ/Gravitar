using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Linq;
using System;
using Panther;

namespace Gravitar.Entities
{
    public class Rammer : VectorModel
    {
        #region Fields
        Camera cameraRef;
        Timer changeVectorTimer;
        #endregion
        #region Properties

        #endregion
        #region Constructor
        public Rammer(Game game, Camera camera) : base(game, camera)
        {
            cameraRef = camera;
            changeVectorTimer = new Timer(game, 0.25f);
        }
        #endregion
        #region Initialize-Load-BeginRun
        public override void Initialize()
        {
            base.Initialize();
            Enabled = false;
            ModelScale = 0.5f;
        }

        public new void LoadContent()
        {
            base.LoadContent();
            LoadVectorModel("PlanetEnemy", Color.Red);
        }

        public void BeginRun()
        {

        }
        #endregion
        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (changeVectorTimer.Elapsed)
            {
                changeVectorTimer.Reset();
                HuntPlayer();
            }

            CheckCollision();
        }
        #endregion
        #region Public Methods
        #endregion
        #region Private Methods
        void HuntPlayer()
        {
            if (Main.instance.ThePlayer.Y > Y)
            {
                PO.Velocity.Y = 2;
            }
            else
            {
                PO.Velocity.Y = -2;
            }

            if (Main.instance.ThePlayer.X > X)
            {
                PO.Velocity.X = 2;
            }
            else
            {
                PO.Velocity.X = -2;
            }
        }

        void CheckCollision()
        {
            if (PO.CirclesIntersect(Main.instance.ThePlayer.PO))
            {
                Main.instance.PlayerHit();
                Explode();
            }

            foreach (Shot shot in Main.instance.ThePlayer.Shots)
            {
                if (CirclesIntersect(shot))
                {
                    Explode();
                    shot.Enabled = false;
                }
            }
        }

        void Explode()
        {
            Enabled = false;
        }
        #endregion
    }
}
