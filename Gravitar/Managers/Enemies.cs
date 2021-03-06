﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Linq;
using System;
using Panther;
using Gravitar.Entities;

namespace Gravitar.Managers
{
    public class Enemies : GameComponent
    {
        #region Fields
        Camera cameraRef;
        #endregion
        #region Properties

        #endregion
        #region Constructor
        public Enemies(Game game, Camera camera) : base(game)
        {
            cameraRef = camera;


            game.Components.Add(this);
        }
        #endregion
        #region Initialize-Load-BeginRun
        public override void Initialize()
        {
            base.Initialize();

        }

        public void LoadContent()
        {

        }

        public void BeginRun()
        {

        }
        #endregion
        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

        }
        #endregion
        #region Public Methods
        public void SpawnPlanetEnemies() //For when player enters planet.
        {

        }
        #endregion
        #region Private Methods
        #endregion
    }
}
