﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Linq;
using System;
using Panther;

namespace Gravitar.Entities
{
    public class Bunker : VectorModel
    {
        #region Fields
        Camera cameraRef;
        List<Shot> shotList = new List<Shot>();
        Vector3[] shotVerticies;
        VectorModel gun;
        Timer gunBlinkTimer;
        Timer fireTimer;
        #endregion
        #region Properties
        public VectorModel Gun { get => gun; }
        #endregion
        #region Constructor
        public Bunker(Game game, Camera camera) : base(game, camera)
        {
            cameraRef = camera;
            gun = new VectorModel(game, camera);
            gunBlinkTimer = new Timer(game, 0.25f);
            fireTimer = new Timer(game, 3);
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
            LoadVectorModel("BunkerBase", Color.Red);
            gun.LoadVectorModel("BunkerGun", Color.Red);

            FileIO fileIO = new FileIO();
            shotVerticies = fileIO.ReadVectorModelFile("Dot");
        }

        public void BeginRun()
        {
            gun.AddAsChildOf(this, true);
        }
        #endregion
        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (fireTimer.Elapsed)
            {
                fireTimer.Reset(Core.RandomMinMax(0.25f, 5f));
                Fire();
            }

            if (!gun.Enabled)
            {
                if (gunBlinkTimer.Elapsed)
                {
                    gun.Enabled = true;
                }
            }
        }
        #endregion
        #region Public Methods
        #endregion
        #region Private Methods
        void Fire()
        {
            bool spawnNew = true;
            int shotNumber = shotList.Count;

            for (int i = 0; i < shotNumber; i++)
            {
                if (!shotList[i].Enabled)
                {
                    spawnNew = false;
                    shotNumber = i;
                    break;
                }
            }

            if (spawnNew)
            {
                shotList.Add(new Shot(Game, cameraRef));
                shotList.Last().InitializePoints(shotVerticies);
            }

            float shotD = 0;

            if (Core.RandomMinMax(1, 6) > 3)
            {
                shotD = FireAimed();
            }
            else
            {
                shotD = FireRandom();
            }

            Vector3 shotV = Core.VelocityFromAngleZ(shotD, 15);

            shotList[shotNumber].Spawn(Position, shotV, 1.25f);
            gun.Enabled = false;
            gunBlinkTimer.Reset();
        }

        float FireRandom()
        {
            return Core.RandomMinMax(-MathHelper.PiOver4, MathHelper.PiOver4) + MathHelper.PiOver2;            
        }

        float FireAimed()
        {
            return PO.AngleFromVectorsZ(Main.instance.ThePlayer.Position);
        }
        #endregion
    }
}
