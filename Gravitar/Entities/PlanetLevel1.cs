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
    public class PlanetLevel1 : VectorModel
    {
        #region Fields
        Camera cameraRef;

        #endregion
        #region Properties

        #endregion
        #region Constructor
        public PlanetLevel1(Game game, Camera camera) : base(game, camera)
        {
            cameraRef = camera;

        }
        #endregion
        #region Initialize-Load-BeginRun
        public override void Initialize()
        {
            base.Initialize();
            Main.instance.ThePlayer.Gravity = 0.5666f;

        }

        public new void LoadContent()
        {
            base.LoadContent();

            LoadVectorModel("WorldLevel1", new Color(0, 255, 0), 10.0f);
        }

        public void BeginRun()
        {
            Y = -Core.ScreenHeight + 1;

        }
        #endregion
        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

        }
        #endregion
        #region Public Methods
        #endregion
        #region Private Methods
        #endregion
    }
}
