using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Linq;
using System;
using Panther;
using CollisionBuddy;

namespace Gravitar.Entities
{
    public class PlanetLevel1 : VectorModel
    {
        #region Fields
        Camera cameraRef;
        VectorModel otherSide;
        Bunker[] bunkerList = new Bunker[2];
        Rammer[] planetEnemies = new Rammer[2];
        FuelDepot[] fuelDepots;
        bool zoomStartDone;
        #endregion
        #region Properties
        public FuelDepot[] FuelDepots { get => fuelDepots; }
        #endregion
        #region Constructor
        public PlanetLevel1(Game game, Camera camera) : base(game, camera)
        {
            cameraRef = camera;
            otherSide = new VectorModel(game, camera);
            fuelDepots = new FuelDepot[2];

            for (int i = 0; i < 2; i++)
            {
                bunkerList[i] = new Bunker(game, camera);
                fuelDepots[i] = new FuelDepot(game, camera);
                planetEnemies[i] = new Rammer(game, camera);
            }
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

            LoadVectorModel("WorldLevel1", Color.Green * 2, 10.0f);
            otherSide.InitializePoints(VertexArray, Color, "Planet Other Side");

            for(int i = 0; i < 2; i++)
            {
                bunkerList[i].LoadContent();
                fuelDepots[i].LoadContent();
                planetEnemies[i].LoadContent();
            }
        }

        public void BeginRun()
        {
            Vector3 camPos = new Vector3(0, 0, 900);
            cameraRef.MoveTo(Position + camPos);
            Main.instance.ThePlayer.Y = Core.ScreenHeight * 1.5f;
            //Main.instance.ThePlayer.Y = Core.ScreenHeight / 1.25f; //Zooms in all the way.

            Y = -Core.ScreenHeight + 1;
            otherSide.Y = Y;
            otherSide.X = otherSide.PO.Radius * 2;
            otherSide.UpdateMatrix();

            bunkerList[0].Position = new Vector3(-4.31f, -15.21f, 0);
            bunkerList[0].PO.Rotation.Z = -MathHelper.PiOver4 + 0.1f;
            bunkerList[1].Position = new Vector3(23.28f, -11.12f, 0);
            bunkerList[1].PO.Rotation.Z = MathHelper.PiOver4 - 0.135f;
            fuelDepots[0].Position = new Vector3(3.3f, Y - 0.175f, 0);
            fuelDepots[1].Position = new Vector3(37.588f, -14.415f, 0);

            for(int i = 0; i < 2; i++)
            {
                planetEnemies[i].Y = Core.ScreenHeight / 1.25f;
                planetEnemies[i].X = -PO.Radius + (i * (PO.Radius * 2));
                bunkerList[i].BeginRun();
                fuelDepots[i].BeginRun();
                planetEnemies[i].BeginRun();
            }

            otherSide.Enabled = false;
        }
        #endregion
        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Main.instance.ThePlayer.Y < -8.75f)
            {
                if (CheckPlanetCollision())
                {
                    Core.DebugConsole("Player hit ground as: " + Main.instance.ThePlayer.Position.ToString());
                    Main.instance.PlayerHit();
                }
            }

            if (!zoomStartDone)
            {
                cameraRef.MoveTo(new Vector3(cameraRef.X, cameraRef.Y, cameraRef.Z - 10));

                if (cameraRef.Z < 95)
                {
                    cameraRef.Z = 95;
                    cameraRef.Velocity.Z = 0;
                    Main.instance.ThePlayer.Gravity = 0.666f;
                    zoomStartDone = true;
                    otherSide.Enabled = true;
                    otherSide.UpdateMatrix();

                    for (int i = 0; i < 2; i++)
                    {
                        planetEnemies[i].Enabled = true;
                        planetEnemies[i].UpdateMatrix();
                    }
                }
            }
            else
            {
                if (Main.instance.ThePlayer.X > 0)
                {
                    otherSide.X = otherSide.PO.Radius * 2;
                }
                else if (Main.instance.ThePlayer.X < 0)
                {
                    otherSide.X = -otherSide.PO.Radius * 2;
                }

                if (Main.instance.ThePlayer.Y > Core.ScreenHeight / 1.25f)
                {
                    cameraRef.MoveTo(new Vector3(cameraRef.X, cameraRef.Y, 95));
                }
                else
                {
                    cameraRef.MoveTo(new Vector3(cameraRef.X, cameraRef.Y, 50));
                }
            }
        }
        #endregion
        #region Public Methods
        public void PlayerReset()
        {
            Main.instance.ThePlayer.Y = Core.ScreenHeight * 1.25f;
            Main.instance.ThePlayer.X = 0;
        }
        #endregion
        #region Private Methods
        bool CheckPlanetCollision() // Spelled Collision
        {
            Vector2 playerPos = new Vector2(Main.instance.ThePlayer.X, Main.instance.ThePlayer.Y);
            Circle playerCir = new Circle(playerPos, Main.instance.ThePlayer.PO.Radius);
            Vector2 overlap = Vector2.Zero;
            Vector2 collisionPoint = Vector2.Zero;

            // Start at -8.75 world.
            for (int i = 0; i < VertexArray.Length -1; i++)
            {
                Vector2 groundStart = new Vector2(VertexArray[i].X, VertexArray[i].Y - Core.ScreenHeight + 1);
                Vector2 groundEnd = new Vector2(VertexArray[i + 1].X, VertexArray[i + 1].Y - Core.ScreenHeight + 1);
                Line groundLine = new Line(groundStart, groundEnd);

                if (CollisionCheck.CircleLineCollision(playerCir, groundLine, ref collisionPoint, ref overlap))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
