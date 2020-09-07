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
    public class Player : VectorModel
    {
        #region Fields
        Camera cameraRef;
        VectorModel flame;
        Color color = new Color(0, 0, 255);
        Color flameColor = new Color (255, 0, 0);
        float thrustAmount = 12.666f;
        float deceleration = 0.2666f;
        float maxVelocity = 42.666f;
        float modelScale = 0.35f;
        float gravity = 0;
        #endregion
        #region Properties
        public Color Color { get => color; }
        public float ModelScale { get => modelScale; }
        public float Gravity { set => gravity = value; }
        #endregion
        #region Constructor
        public Player(Game game, Camera camera) : base(game, camera)
        {
            cameraRef = camera;
            flame = new VectorModel(game, camera);

        }
        #endregion
        #region Initialize-Load-BeginRun
        public override void Initialize()
        {
            base.Initialize();

        }

        public new void LoadContent()
        {
            base.LoadContent();

            LoadVectorModel("Gravitar Player Ship", color, modelScale);
            flame.LoadVectorModel("Gravitar Player Flame", flameColor, modelScale);
        }

        public void BeginRun()
        {
            //Enabled = false;
            flame.Visible = false;
            flame.PO.AddAsChildOf(PO);
        }
        #endregion
        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            GetKeys();

            PO.Acceleration.Y -= gravity;

            if (Y < -Core.ScreenHeight + PO.Radius)
            {
                PO.Acceleration.Y = 0;
                PO.Velocity.Y = 0;
                PO.Position.Y = -Core.ScreenHeight + PO.Radius;
            }
        }
        #endregion
        #region Public Methods
        public new void Hit()
        {

        }
        #endregion
        #region Private Methods
        void GetKeys()
        {
            float rotationAmound = MathHelper.Pi;

            if (Core.KeyDown(Keys.W) || Core.KeyDown(Keys.Up))
            {
                ThrustOn();
            }
            else
            {
                ThrustOff();
            }

            if (Core.KeyDown(Keys.A) || Core.KeyDown(Keys.Left))
            {
                PO.RotationVelocity.Z = rotationAmound;
            }
            else if (Core.KeyDown(Keys.D) || Core.KeyDown(Keys.Right))
            {
                PO.RotationVelocity.Z = -rotationAmound;
            }
            else
            {
                PO.RotationVelocity.Z = 0;
            }

            if (Core.KeyPressed(Keys.Down))
            {
                ShieldFuel();
            }

            if (Core.KeyPressed(Keys.LeftControl) || Core.KeyPressed(Keys.Space))
            {
                Fire();
            }
        }

        void ThrustOn()
        {
            flame.Visible = true;

            if (Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) < maxVelocity)
            {

                Acceleration = Core.VelocityFromAngleZ(Rotation.Z, thrustAmount);
            }
            else
            {
                ThrustOff();
            }
        }

        void ThrustOff()
        {
            flame.Visible = false;
            Acceleration = -Velocity * deceleration;
        }

        void ShieldFuel()
        {

        }

        void Fire()
        {

        }
        #endregion
    }
}
