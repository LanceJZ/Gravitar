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
    public class PlanetEnemy : VectorModel
    {
        #region Fields
        Camera cameraRef;

        #endregion
        #region Properties

        #endregion
        #region Constructor
        public PlanetEnemy(Game game, Camera camera) : base(game, camera)
        {
            cameraRef = camera;

        }
        #endregion
        #region Initialize-Load-BeginRun
        public override void Initialize()
        {
            base.Initialize();
            Enabled = false;
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

        }
        #endregion
        #region Public Methods
        #endregion
        #region Private Methods
        #endregion
    }
}
