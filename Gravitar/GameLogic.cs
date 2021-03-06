﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Linq;
using System;
using Panther;
using Gravitar.Entities;
using Gravitar.Managers;
using System.ComponentModel;

public enum GameState
{
    PlayerHit,
    Over,
    InPlayOnPlanet,
    InPlayInSpace,
    Pause,
    HighScore,
    MainMenu
};

public struct HighScore
{
    public uint score;
    public string name;
}

namespace Gravitar
{
    public class GameLogic : GameComponent
    {
        #region Fields
        Camera _camera;
        Player player;
        PlayerShipsDisplay playerShipDisplay;
        PlanetLevel1 planet1;
        Enemies enemies;
        VectorModel cross;
        Timer highScoreListTimer;
        FileIO fileIO;
        List<VectorModel> playerShipModels = new List<VectorModel>();
        HighScore[] highScoreArray = new HighScore[10];
        SpriteFont hyper20Font;
        SpriteFont hyper16Font;
        SpriteFont hyper8Font;
        SoundEffect bonusSound;
        Vector3[] dotVerts;
        Vector2 scorePosition = new Vector2();
        Vector2 fuelPosition = new Vector2();
        Vector2 bonusPosition = new Vector2();
        Vector2 missionCompletePosition = new Vector2();
        Vector2 highScoreListPosition = new Vector2();
        Vector2 highScoreInstructionsPosition = new Vector2();
        Vector2 highScoreLettersPosition = new Vector2();
        Vector2 scoreDisplayPosition = new Vector2();
        Vector2 fuelDisplayPosition = new Vector2();
        Vector2 bonusDisplayPosition = new Vector2();
        Vector2 copyPosition = new Vector2();
        Vector2 gameoverPosition = new Vector2();
        GameState _gameMode = GameState.InPlayOnPlanet;
        string scoreText;
        string fuelText;
        string bonusText;
        string newHighScoreEntryText = "";
        string scoreDisplayText = "Score";
        string fuelDisplayText = "Fuel";
        string bonusDisplayText = "Bonus";
        string highScoresText = "High Scores";
        string missionCompleteText = "Mission complete";
        string[] highScoreInstructions = new string[4];
        string copyRightText = "(c) 1982 Atari inc"; //©
        string gameOverText = "Game Over";
        string fileNameHighScoreList = "HighScoreList.sav";
        char[] highScoreSelectedLetters = new char[3];
        uint score = 0;
        uint highScore = 0;
        uint bonus = 0;
        uint bonusLifeAmount = 10000;
        uint bonusLifeScore = 0;
        uint wave = 0;
        uint baseFuel = 10000;
        int fuel = 0;
        int lives = 0;
        int highScoreSelectedSpace;
        int newHighScorePosition;
        bool displayHighScoreList = true;
        bool missionComplete;
        #endregion
        #region Properties
        public GameState CurrentMode { get => _gameMode; set => _gameMode = value; }
        public Player ThePlayer { get => player; }
        public PlanetLevel1 Planet1 { get => planet1; }
        public Enemies TheEnemies { get => enemies; }
        public uint Score { get => score; }
        public uint Wave { get => wave; set => wave = value; }
        public int Lives { get => lives; }
        public int Fuel { get => fuel;}
        #endregion
        #region Constructor
        public GameLogic(Game game) : base(game)
        {
            // Screen resolution is 1200 X 900.
            // Y positive is Up.
            // (X) positive is to the right when camera is at rotation zero.
            // Z positive is towards the camera when at rotation zero.
            // Rotation on object rotates CCW. Zero has front facing X positive. Pi/2 on Y faces Z negative.
            _camera = new Camera(game, new Vector3(0, 0, 50), new Vector3(0, MathHelper.Pi, 0),
                Core.Graphics.Viewport.AspectRatio, 1f, 1000);

            cross = new VectorModel(Game, _camera);

            player = new Player(game, _camera);
            playerShipDisplay = new PlayerShipsDisplay(game, _camera);
            planet1 = new PlanetLevel1(game, _camera);
            enemies = new Enemies(game, _camera);

            highScoreListTimer = new Timer(game);
            fileIO = new FileIO();

            game.Components.Add(this);
        }
        #endregion
        #region Public Methods
        public override void Initialize()
        {
            base.Initialize();

            highScoreInstructions[0] = "Your score is one of the ten best";
            highScoreInstructions[1] = "Please enter your initials";
            highScoreInstructions[2] = "Push rotate to select letter";
            highScoreInstructions[3] = "Push hyperspace when letter is correct";
            float crossSize = 0.5f;
            Vector3[] crossVertex = { new Vector3(crossSize, 0, 0), new Vector3(-crossSize, 0, 0),
                new Vector3(0, crossSize, 0), new Vector3(0, -crossSize, 0) };
            cross.InitializePoints(crossVertex, "Cross");

            // The X: 27.63705 Y: -20.711943 for X and Y at Z zero with camera at 50 and still be visible.
            Core.ScreenWidth = 27.63705f;
            Core.ScreenHeight = 20.711943f;
        }

        public void LoadContent()
        {
            player.LoadContent();
            planet1.LoadContent();
            enemies.LoadContent();

            hyper20Font = Game.Content.Load<SpriteFont>("Hyperspace20");
            hyper16Font = Game.Content.Load<SpriteFont>("Hyperspace16");
            hyper8Font = Game.Content.Load<SpriteFont>("Hyperspace8");
        }

        public void BeginRun()
        {
            player.BeginRun();
            planet1.BeginRun();
            enemies.BeginRun();
            playerShipDisplay.BeginRun();

            cross.Enabled = false;
            LoadHighScore();
            HighScoreChanged();
            copyPosition = new Vector2(Core.WindowWidth / 2 - hyper8Font.MeasureString(copyRightText).X / 2,
                Core.WindowHeight - 20);
            gameoverPosition = new Vector2(Core.WindowWidth / 2 - hyper20Font.MeasureString(gameOverText).X / 2,
                Core.WindowHeight / 1.25f);
            highScoreListPosition = new Vector2(Core.WindowWidth / 2.75f, Core.WindowHeight / 4.25f);
            highScoreInstructionsPosition = new Vector2(50, Core.WindowHeight / 4);
            highScoreLettersPosition = new Vector2(Core.WindowWidth / 2.25f, Core.WindowHeight / 1.25f);
            scoreDisplayPosition.X = Core.WindowWidth / 2 - hyper20Font.MeasureString(scoreDisplayText).X / 2;
            fuelDisplayPosition.X = Core.WindowWidth / 2 - hyper20Font.MeasureString(fuelDisplayText).X / 2;
            missionCompletePosition.X = Core.WindowWidth / 2 - hyper20Font.MeasureString(missionCompleteText).X / 2;
            missionCompletePosition.Y = fuelDisplayPosition.Y + 
                hyper20Font.MeasureString(missionCompleteText).Y + 50;
            bonusDisplayPosition.X = Core.WindowWidth - hyper20Font.MeasureString(bonusDisplayText).X - 20;
            bonusDisplayPosition.Y = 50;

            ResetGame();
        }

        public void UnloadContent()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            GetKeys();

            if (_gameMode == GameState.InPlayOnPlanet)
            {
                _camera.X = player.X;
                _camera.UpdateLookAt();
            }

            playerShipDisplay.X = _camera.X;
            playerShipDisplay.Y = _camera.Y;
            playerShipDisplay.Z = _camera.Z - 50;
        }

        public void Draw()
        {
            Core.SpriteBatch.Begin();

            if (score > 0)
            {
                Core.SpriteBatch.DrawString(hyper20Font, scoreText, scorePosition, Color.Green);
            }

            Core.SpriteBatch.DrawString(hyper20Font, fuelText, fuelPosition, new Color(0, 255, 0));
            Core.SpriteBatch.DrawString(hyper20Font, bonusText, bonusPosition, Color.Green);
            Core.SpriteBatch.DrawString(hyper20Font, scoreDisplayText, scoreDisplayPosition,
                Color.Aqua);
            Core.SpriteBatch.DrawString(hyper20Font, fuelDisplayText, fuelDisplayPosition,
                Color.Aqua);
            Core.SpriteBatch.DrawString(hyper20Font, bonusDisplayText, bonusDisplayPosition,
                Color.Aqua);

            if (missionComplete)
            {
                Core.SpriteBatch.DrawString(hyper20Font, missionCompleteText, missionCompletePosition,
                    Color.Aqua);
            }

            if (_gameMode == GameState.Over)
            {
                if (highScoreListTimer.Elapsed)
                {
                    displayHighScoreList = !displayHighScoreList;
                    highScoreListTimer.Reset(15);
                }

                if (displayHighScoreList)
                {

                }

                Core.SpriteBatch.DrawString(hyper8Font, copyRightText, copyPosition, Color.White);
                Core.SpriteBatch.DrawString(hyper20Font, gameOverText, gameoverPosition, Color.White);
            }

            if (_gameMode == GameState.HighScore)
            {

            }

            Core.SpriteBatch.End();
        }

        public void PlayerFuel(int points)
        {
            fuel += points;

            fuelText = fuel.ToString();
            float ftextlength = hyper20Font.MeasureString(fuelText).X;
            fuelPosition.X = 500 - ftextlength;
            fuelPosition.Y = scoreDisplayPosition.Y + 40;
            fuelDisplayPosition.Y = fuelPosition.Y;
        }

        public void PlayerScore(uint points)
        {
            score += points;
            scoreText = score.ToString();
            float stextlength = hyper20Font.MeasureString(scoreText).X;
            scorePosition.X = 500 - stextlength;
            scoreDisplayPosition.Y = scorePosition.Y;

            if (score > bonusLifeScore)
            {
                lives++;
                bonusLifeScore += bonusLifeAmount;
                playerShipDisplay.ShipToDesplay(Lives);
            }
        }

        public void PlayerBonus(uint points)
        {
            bonus += points;
            bonusText = bonus.ToString();
            float btextlength = hyper20Font.MeasureString(bonusText).X;
            bonusPosition.X = (Core.WindowWidth - 20) - btextlength;
            bonusPosition.Y = bonusDisplayPosition.Y - 30;
        }

        public void PlayerHit()
        {
            ThePlayer.Hit();
            ThePlayer.Reset();
            lives--;

            if (lives < 0)
            {
                _gameMode = GameState.Over;

                if (score > highScore)
                {
                    highScore = score;
                    HighScoreChanged();
                    SaveHighScore();
                }

                CheckForNewHighScore();

                return;
            }

            playerShipDisplay.ShipToDesplay(Lives);
            //_gameMode = GameState.PlayerHit;
            planet1.PlayerReset();
        }

        public void GetKeys()
        {
            if (Core.KeyPressed(Keys.End))
            {
                cross.Enabled = !cross.Enabled;
                cross.Position = ThePlayer.Position;
            }

            if (Core.KeyPressed(Keys.Pause))
            {
                if (CurrentMode == GameState.InPlayOnPlanet)
                {
                    _gameMode = GameState.Pause;
                }
                else if (CurrentMode == GameState.Pause)
                {
                    _gameMode = GameState.InPlayOnPlanet;
                }
            }

            if (Core.KeyPressed(Keys.Enter) && _gameMode == GameState.Over)
            {
                ResetGame();
            }

            if (cross.Enabled)
            {
                if (Core.KeyPressed(Keys.Enter))
                {
                    Core.DebugConsole("X: " + cross.X.ToString() +
                        " " + "Y: " + cross.Y.ToString());
                }

                if (Core.KeyDown(Keys.W))
                {
                    cross.PO.Velocity.Y += 0.125f;
                }
                else if (Core.KeyDown(Keys.S))
                {
                    cross.PO.Velocity.Y -= 0.125f;
                }
                else
                {
                    cross.PO.Velocity.Y = 0;
                }

                if (Core.KeyDown(Keys.D))
                {
                    cross.PO.Velocity.X += 0.125f;
                }
                else if (Core.KeyDown(Keys.A))
                {
                    cross.PO.Velocity.X -= 0.125f;
                }
                else
                {
                    cross.PO.Velocity.X = 0;
                }
            }
        }
        #endregion
        #region Private Methods
        void NewHighScoreEntry()
        {
            if (Core.KeyPressed(Keys.Down))
            {
                highScoreSelectedSpace++;

                if (highScoreSelectedSpace > 2)
                {
                    highScoreArray[newHighScorePosition].name = newHighScoreEntryText;
                    highScoreArray[newHighScorePosition].score = score;
                    _gameMode = GameState.Over;
                    WriteHighScoreList();
                }
                else
                {
                    highScoreSelectedLetters[highScoreSelectedSpace] = (char)65;
                }
            }
            else if (Core.KeyPressed(Keys.Left))
            {
                highScoreSelectedLetters[highScoreSelectedSpace]--;

                if (highScoreSelectedLetters[highScoreSelectedSpace] < 65)
                {
                    highScoreSelectedLetters[highScoreSelectedSpace] = (char)90;
                }
            }
            else if (Core.KeyPressed(Keys.Right))
            {
                highScoreSelectedLetters[highScoreSelectedSpace]++;

                if (highScoreSelectedLetters[highScoreSelectedSpace] > 90)
                {
                    highScoreSelectedLetters[highScoreSelectedSpace] = (char)65;
                }
            }

            newHighScoreEntryText = "";

            foreach (char letter in highScoreSelectedLetters)
            {
                newHighScoreEntryText += letter;
            }
        }

        void CheckForNewHighScore()
        {
            for (int rank = 0; rank < 10; rank++)
            {
                if (score > highScoreArray[rank].score)
                {
                    if (rank < 9)
                    {
                        HighScore[] oldScores = new HighScore[10];

                        for (int oldRank = rank; oldRank < 10; oldRank++)
                        {
                            oldScores[oldRank].score = highScoreArray[oldRank].score;
                            oldScores[oldRank].name = highScoreArray[oldRank].name;
                        }

                        for (int newRank = rank; newRank < 9; newRank++)
                        {
                            highScoreArray[newRank + 1].score = oldScores[newRank].score;
                            highScoreArray[newRank + 1].name = oldScores[newRank].name;
                        }
                    }

                    highScoreArray[rank].score = score;
                    highScoreArray[rank].name = "AAA";
                    _gameMode = GameState.HighScore;
                    highScoreSelectedLetters = "___".ToCharArray();
                    highScoreSelectedSpace = 0;
                    newHighScorePosition = rank;
                    highScoreSelectedLetters[highScoreSelectedSpace] = (char)65;
                    break;
                }
            }
        }

        void HighScoreChanged()
        {

        }

        void ResetGame()
        {
            _gameMode = GameState.InPlayOnPlanet;
            lives = 4;
            score = 0;
            bonusLifeScore = bonusLifeAmount;
            ThePlayer.Reset();
            playerShipDisplay.ShipToDesplay(Lives);
            fuel = (int)baseFuel;
            PlayerScore(0);
            PlayerBonus(2000);
            PlayerFuel(0);
        }

        void SaveHighScore()
        {
            fileIO.WriteStringFile("Score.sav", highScore.ToString());
        }

        void LoadHighScore()
        {
            if (fileIO.DoesFileExist("Score.sav"))
            {
                highScore = uint.Parse(fileIO.ReadStringFile("Score.sav"));
            }

            if (fileIO.DoesFileExist(fileNameHighScoreList))
            {
                // Read High Score List into array.
                LoadandDecodeHighScores(fileNameHighScoreList);

                foreach(HighScore high in highScoreArray)
                {
                    if (highScore < high.score)
                    {
                        highScore = high.score;
                    }
                }
            }
            else
            {
                MakeNewHighScoreList();
                WriteHighScoreList();
            }
        }

        void MakeNewHighScoreList()
        {
            for(int i = 0; i < 10; i++)
            {
                highScoreArray[i].name = "AAA";
                highScoreArray[i].score = 1000;
            }
        }

        void WriteHighScoreList()
        {
            fileIO.OpenForWrite(fileNameHighScoreList);

            foreach(HighScore score in highScoreArray)
            {
                fileIO.WriteByteArray(fileIO.StringToByteArray(score.name));
                fileIO.WriteByteArray(fileIO.StringToByteArray(score.score.ToString()));
                fileIO.WriteByteArray(fileIO.StringToByteArray(":"));
            }

            fileIO.Close();
        }


        void LoadandDecodeHighScores(string fileName)
        {
            string scoreData = fileIO.ReadStringFile(fileName);

            int list = 0;
            int letter = 0;
            bool isLetter = true;
            string fromNumber = "";

            foreach (char character in scoreData)
            {
                if (character.ToString() == "\0")
                {
                    break;
                }

                if (isLetter)
                {
                    letter++;
                    highScoreArray[list].name += character;

                    if (letter == 3)
                        isLetter = false;
                }
                else
                {
                    if (character.ToString() == ":")
                    {
                        highScoreArray[list].score = uint.Parse(fromNumber);

                        list++;
                        letter = 0;
                        fromNumber = "";
                        isLetter = true;
                    }
                    else
                    {
                        fromNumber += character.ToString();
                    }
                }
            }
        }

        #endregion
    }
}
