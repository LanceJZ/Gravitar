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
    class PlayerShipsDisplay : PositionedObject
    {
        #region Fields
        Camera cameraRef;
        List<VectorModel> playerShipModels = new List<VectorModel>();
        Vector3[] shipModel;
        #endregion
        #region Properties
        public Vector3[] ShipModel { set => shipModel = value; }
        #endregion
        #region Constructor
        public PlayerShipsDisplay(Game game, Camera camera) : base(game)
        {
            cameraRef = camera;
        }
        #endregion
        #region Initialize-Load-Begin
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
        public void ShipToDesplay(int lives)
        {
            foreach (VectorModel ship in playerShipModels)
            {
                ship.Enabled = false;
            }

            for (int i = 0; i < lives; i++)
            {
                bool newShip = true;
                int thisShip = 0;

                for (int j = 0; j < playerShipModels.Count; j++)
                {
                    if (!playerShipModels[j].Enabled)
                    {
                        thisShip = j;
                        newShip = false;
                        playerShipModels[j].Enabled = true;
                        break;
                    }
                }

                if (newShip)
                {
                    playerShipModels.Add(new VectorModel(Game, cameraRef));
                    playerShipModels.Last().PO.AddAsChildOf(this);
                    playerShipModels.Last().InitializePoints(Main.instance.ThePlayer.VertexArray,
                        Main.instance.ThePlayer.Color, 1);
                    playerShipModels.Last().Scale = 1.25f;
                    playerShipModels.Last().PO.Rotation.Z = MathHelper.PiOver2;
                    playerShipModels.Last().UpdateMatrix();
                }
            }

            float column = -15;

            for (int i = 0; i < lives; i++)
            {
                if (playerShipModels[i].Enabled)
                {
                    playerShipModels[i].PO.Position = new 
                        Vector3(column - (i * (playerShipModels[0].PO.Radius * 1.75f)),
                                    Core.ScreenHeight - 1, 0);
                }
            }
        }
        #endregion
        #region Private Methods
        #endregion
    }
}
