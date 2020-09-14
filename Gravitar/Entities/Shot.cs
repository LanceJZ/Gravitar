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
    public class Shot : VectorModel
    {
        #region Fields
        Camera cameraRef;
        Timer life;

        #endregion
        #region Properties

        #endregion
        #region Constructor
        public Shot(Game game, Camera camera) : base(game, camera)
        {
            cameraRef = camera;
            life = new Timer(game);

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

            LoadVectorModel("Dot", Color.Yellow);
        }

        public void BeginRun()
        {

        }
        #endregion
        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (life.Elapsed)
            {
                Enabled = false;
            }
        }
        #endregion
        #region Public Methods
        public void Spawn(Vector3 position, Vector3 velocity, float timer)
        {
            Spawn(position, velocity);
            life.Reset(timer);
        }
        #endregion
        #region Private Methods
        #endregion
    }
}
