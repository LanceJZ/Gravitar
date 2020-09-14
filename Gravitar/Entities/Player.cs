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
        VectorModel shield;
        VectorModel fuelBeamL;
        VectorModel fuelBeamR;
        Shot[] shotList = new Shot[4];
        float thrustAmount = 6.666f;
        float deceleration = 0.2666f;
        float maxVelocity = 40.666f;
        float gravity = 0;
        #endregion
        #region Properties
        public float Gravity { set => gravity = value; }
        public VectorModel[] Shots { get => shotList; }
        #endregion
        #region Constructor
        public Player(Game game, Camera camera) : base(game, camera)
        {
            cameraRef = camera;
            flame = new VectorModel(game, camera);
            shield = new VectorModel(game, camera);
            fuelBeamL = new VectorModel(game, camera);
            fuelBeamR = new VectorModel(game, camera);

            for (int i = 0; i < 4; i++)
            {
                shotList[i] = new Shot(game, camera);
            }
        }
        #endregion
        #region Initialize-Load-BeginRun
        public override void Initialize()
        {
            base.Initialize();
            ModelScale = 0.25f;
            Color = new Color(0, 0, 255);
            flame.Color = new Color(255, 0, 0);
            fuelBeamL.Color = Color.Green;
            fuelBeamR.Color = Color.Green;
        }

        public new void LoadContent()
        {
            base.LoadContent();

            LoadVectorModel("Gravitar Player Ship");
            shield.LoadVectorModel("Gravitar Player Shield", Color, ModelScale);
            flame.LoadVectorModel("Gravitar Player Flame", ModelScale);
            fuelBeamL.LoadVectorModel("FuelBeamLeft", ModelScale);
            fuelBeamR.LoadVectorModel("FuelBeamRight", ModelScale);

            foreach (Shot shot in shotList)
            {
                shot.LoadContent();
            }
        }

        public void BeginRun()
        {
            //Enabled = false;
            flame.AddAsChildOf(this, true);
            shield.AddAsChildOf(this, true, false);
            fuelBeamL.AddAsChildOf(this, true, false);
            fuelBeamR.AddAsChildOf(this, true, false);
            Reset();

            foreach(Shot shot in shotList)
            {
                shot.BeginRun();
            }
        }
        #endregion
        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            GetKeys();
            CheckCollision();

            PO.Acceleration.Y += -gravity;

            if (Y < -Core.ScreenHeight + PO.Radius)
            {
                PO.Acceleration.Y = 0;
                PO.Velocity.Y = 0;
                PO.Position.Y = -Core.ScreenHeight + PO.Radius;
            }

            if (X < -Main.instance.Planet1.PO.Radius)
            {
                X = Main.instance.Planet1.PO.Radius;
            }
            else if (X > Main.instance.Planet1.PO.Radius)
            {
                X = -Main.instance.Planet1.PO.Radius;
            }

        }
        #endregion
        #region Public Methods
        public new void Hit()
        {

        }

        public void Reset()
        {
            //Y = Core.ScreenHeight / 1.25f;
            X = 0;
            PO.Rotation.Z = -MathHelper.PiOver2;
            Acceleration = Vector3.Zero;
            Velocity = Vector3.Zero;
        }
        #endregion
        #region Private Methods
        void GetKeys()
        {
            float rotationAmound = MathHelper.Pi;

            if (Core.KeyDown(Keys.Up)) //Core.KeyDown(Keys.W) || 
            {
                ThrustOn();
            }
            else
            {
                ThrustOff();
            }

            if (Core.KeyDown(Keys.Left)) //Core.KeyDown(Keys.A) || 
            {
                PO.RotationVelocity.Z = rotationAmound;
            }
            else if (Core.KeyDown(Keys.Right)) //Core.KeyDown(Keys.D) || 
            {
                PO.RotationVelocity.Z = -rotationAmound;
            }
            else
            {
                PO.RotationVelocity.Z = 0;
            }

            if (Core.KeyDown(Keys.Down))
            {
                ShieldFuelOn();
            }
            else
            {
                ShieldFuelOff();
            }

            if (Core.KeyPressed(Keys.LeftControl) || Core.KeyPressed(Keys.Space))
            {
                Fire();
            }
        }

        void ThrustOn()
        {
            if (Main.instance.Fuel < 1)
                return;

            flame.Visible = true;

            if (Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) < maxVelocity)
            {
                Main.instance.PlayerFuel(-1);
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

        void ShieldFuelOn()
        {
            shield.Visible = true;
            fuelBeamL.Visible = true;
            fuelBeamR.Visible = true;

            foreach(FuelDepot fuel in Main.instance.Planet1.FuelDepots)
            {
                if (fuel.Enabled)
                {
                    if (fuelBeamL.PO.CirclesIntersect(fuel.PO) && fuelBeamR.PO.CirclesIntersect(fuel.PO))
                    {
                        Main.instance.PlayerFuel(2500);
                        fuel.Enabled = false;
                    }
                }
            }
        }

        void ShieldFuelOff()
        {
            shield.Visible = false;
            fuelBeamL.Visible = false;
            fuelBeamR.Visible = false;

        }

        void Fire()
        {
            Vector3 dir = Core.VelocityFromAngleZ(Rotation.Z, 26.66f);
            Vector3 offset = Core.VelocityFromAngleZ(Rotation.Z, PO.Radius);

            foreach (Shot shot in shotList)
            {
                if (!shot.Enabled)
                {

                    shot.Spawn(Position + offset, dir + (Velocity * 0.75f), 1.25f);
                    break;
                }
            }
        }

        void CheckCollision()
        {
            
        }
        #endregion
    }
}
