﻿using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Maze_Accelerometer.Classes
{
    internal class Drawing
    {
        public class MazeGameDrawable : IDrawable
        {
            public Ball PlayerBall { get; private set; }
            public List<Wall> Walls { get; private set; } = new List<Wall>();
            public Goal GameGoal { get; private set; }

            public float CanvasWidth { get; private set; } //Edge left & rigt
            public float CanvasHeight { get; private set; } //Edge top & bottom

            public bool IsGameWon { get; private set; } = false;

            public Vector2 AccelerationInput { get; set; } = Vector2.Zero;

            private const float MoveSpeed = 40;
            private const float BallRadius = 10;


            public void InitializeGame(float canvasWidth, float canvasHeight)
            {
                CanvasWidth = canvasWidth;
                CanvasHeight = canvasHeight;
                IsGameWon = false;

                PlayerBall = new Ball(30, 30, BallRadius, Colors.DodgerBlue); //Player start position

                Walls.Clear();
                DefineMazeWalls();

                float goalSize = BallRadius * 2;
                GameGoal = new Goal(200, 600, goalSize, Colors.Gold);
            }

            private void DefineMazeWalls()
            {
                float wallThickness = 10;
                Color wallColor = Colors.DarkSlateGray;


                float wallX = wallThickness;
                float wallY = wallThickness;
                float wallYRange;
                float wallXRange;

                #region Zdi bludiště
                #region Hranice bludiště
                // 1. Horní zeď:
                //    Začíná za vstupní mezerou vlevo, končí u pravé vnější zdi.
                Walls.Add(new Wall(
                    wallX = 0, // Začátek X od levyho okraje (385)
                    wallY = 0,             // Začátek Y od horniho okraje (750)
                    wallXRange = 400, // Délka zdi (x range - 400, normal 10))
                    wallYRange = 10,             // Výška zdi (y range - 760, normal 10)
                    wallColor
                ));

                // 2. Levá zeď:
                //    Začíná od horního okraje, končí u dolního okraje.
                Walls.Add(new Wall(
                    wallX = 0, // Začátek X od levyho okraje
                    wallY = 0,             // Začátek Y od horniho okraje (750)
                    wallXRange = 10, // Délka zdi (x range - 400)
                    wallYRange = 770,             // Výška zdi (y range - 760)
                    wallColor
                ));

                // 4. Pravá zeď:
                Walls.Add(new Wall(
                    wallX + 385, // Začátek X od levyho okraje
                    wallY,             // Začátek Y od horniho okraje (750)
                    wallXRange = 10, // Délka zdi (x range - 400)
                    wallYRange = 760,             // Výška zdi (y range - 760)
                    wallColor
                ));

                // 4. Dolní zeď:
                Walls.Add(new Wall(
                    wallX, // Začátek X od levyho okraje
                    wallY + 760,             // Začátek Y od horniho okraje (750)
                    wallXRange = 400, // Délka zdi (x range - 400)
                    wallYRange = 10,             // Výška zdi (y range - 760)
                    wallColor
                ));
                #endregion
                // POZNÁMKA: Startovní pozice kuličky (např. X=30, Y=30)

                //ZNEVIDITELNIT SASKU
                // V1.1    První neviditelna zed
                Walls.Add(new Wall(
                    wallX + 70, // Začátek X od levyho okraje
                    wallY,             // Začátek Y od horniho okraje (750)
                    wallXRange = 10, // Délka zdi (x range - 400)
                    wallYRange = 80,             // Výška zdi (y range - 760)
                    wallColor,
                    wallType.Invisible
                ));



                // H2.1   Začíná za vstupní mezerou vlevo, konci u horni prostupni
                Walls.Add(new Wall(
                    wallX + 70, // Začátek X od levyho okraje
                    wallY + 80,             // Začátek Y od horniho okraje (750)
                    wallXRange = 160, // Délka zdi (x range - 400)
                    wallYRange = 10,             // Výška zdi (y range - 760)
                    wallColor
                ));

                //PRVNI PROSTUPNA
                // 2.2   Začíná na predchodi zdi, konci mezerou od startu pravy steny chodby do cile
                Walls.Add(new Wall(
                    wallX + 230, // Začátek X od levyho okraje
                    wallY + 80,             // Začátek Y od horniho okraje (750)
                    wallXRange = 50, // Délka zdi (x range - 400)
                    wallYRange = 10,             // Výška zdi (y range - 760)
                    wallColor,
                    wallType.OneWaySolidFromBottom
                ));

                // V2.3   Začíná na prvni prostupny, konci u 3.2
                Walls.Add(new Wall(
                    wallX + 280, // Začátek X od levyho okraje
                    wallY + 80,             // Začátek Y od horniho okraje (750)
                    wallXRange = 10, // Délka zdi (x range - 400)
                    wallYRange = 80,             // Výška zdi (y range - 760)
                    wallColor
                ));

                // V2.4.1  Začíná mezerou od horni zdi, konci na druhy prostupny
                Walls.Add(new Wall(
                    wallX + 325, // Začátek X od levyho okraje
                    wallY + 80,             // Začátek Y od horniho okraje (750)
                    wallXRange = 10, // Délka zdi (x range - 400)
                    wallYRange = 130,             // Výška zdi (y range - 760)
                    wallColor
                ));

                // DRUHA PROSTUPNA 
                // V2.4.2   Začíná mezerou od horni zdi, konci mezerou od dolni zdi
                Walls.Add(new Wall(
                    wallX + 325, // Začátek X od levyho okraje
                    wallY + 210,             // Začátek Y od horniho okraje (750)
                    wallXRange = 10, // Délka zdi (x range - 400)
                    wallYRange = 50,             // Výška zdi (y range - 760)
                    wallColor,
                    wallType.OneWaySolidFromRight
                ));

                // V2.4.3   Začíná mezerou od horni zdi, konci mezerou od dolni zdi
                Walls.Add(new Wall(
                    wallX + 325, // Začátek X od levyho okraje
                    wallY + 260,             // Začátek Y od horniho okraje (750)
                    wallXRange = 10, // Délka zdi (x range - 400)
                    wallYRange = 430,             // Výška zdi (y range - 760)
                    wallColor
                ));



                // H3.1   Začíná na levý zdi, končí u první rozdvojky
                Walls.Add(new Wall(
                    wallX, // Začátek X od levyho okraje
                    wallY + 150,             // Začátek Y od horniho okraje (750)
                    wallXRange = 120, // Délka zdi (x range - 400)
                    wallYRange = 10,             // Výška zdi (y range - 760)
                    wallColor
                ));

                // H3.2   Začíná za první rozdvojkou, končí u chodby do cíle
                Walls.Add(new Wall(
                    wallX + 180, // Začátek X od levyho okraje
                    wallY + 150,             // Začátek Y od horniho okraje (750)
                    wallXRange = 100, // Délka zdi (x range - 400)
                    wallYRange = 10,             // Výška zdi (y range - 760)
                    wallColor
                ));

                // V3.3   Začíná na zacatku 3.2, konci na 5.1
                Walls.Add(new Wall(
                    wallX + 180, // Začátek X od levyho okraje
                    wallY + 160,             // Začátek Y od horniho okraje (750)
                    wallXRange = 10, // Délka zdi (x range - 400)
                    wallYRange = 100,             // Výška zdi (y range - 760)
                    wallColor
                ));



                // H4.1   Začíná mezerou od 3.3 (vpravo vstupni chodby), konci na 2.4 (vlevo vedle pravy zdi)
                Walls.Add(new Wall(
                    wallX + 230, // Začátek X od levyho okraje
                    wallY + 200,             // Začátek Y od horniho okraje (750)
                    wallXRange = 95, // Délka zdi (x range - 400)
                    wallYRange = 10,             // Výška zdi (y range - 760)
                    wallColor
                ));



                // H5.1 Začíná mezerou od levy zdi, konci u konce vitezny chodby   
                Walls.Add(new Wall(
                    wallX + 70, // Začátek X od levyho okraje
                    wallY + 260,             // Začátek Y od horniho okraje (750)
                    wallXRange = 180, // Délka zdi (x range - 400)
                    wallYRange = 10,             // Výška zdi (y range - 760)
                    wallColor
                ));

                // H5.2   Začíná na konci vitezny chodby, konci na 2.4 (nepravejsi zdi)
                Walls.Add(new Wall(
                    wallX + 280, // Začátek X od levyho okraje
                    wallY + 260,             // Začátek Y od horniho okraje (750)
                    wallXRange = 45, // Délka zdi (x range - 400)
                    wallYRange = 10,             // Výška zdi (y range - 760)
                    wallColor
                ));



                // V6.1   Začíná na 5.1, konci pred vchodem do fake cilovy chodby
                Walls.Add(new Wall(
                    wallX + 70, // Začátek X od levyho okraje
                    wallY + 260,             // Začátek Y od horniho okraje (750)
                    wallXRange = 10, // Délka zdi (x range - 400)
                    wallYRange = 340,             // Výška zdi (y range - 760)
                    wallColor
                ));

                // DRUHA NEVIDITELNA ZED SASKU
                // V6.2.1   Začíná mezerou od horni zdi, konci mezerou od dolni zdi
                Walls.Add(new Wall(
                    wallX + 120, // Začátek X od levyho okraje
                    wallY + 260,             // Začátek Y od horniho okraje (750)
                    wallXRange = 10, // Délka zdi (x range - 400)
                    wallYRange = 70,             // Výška zdi (y range - 760)
                    wallColor,
                    wallType.Invisible
                ));

                // V6.2.2   Začíná na konci 6.2.1, konci na nejspodnejsi zdi
                Walls.Add(new Wall(
                    wallX + 120, // Začátek X od levyho okraje
                    wallY + 330,             // Začátek Y od horniho okraje (750)
                    wallXRange = 10, // Délka zdi (x range - 400)
                    wallYRange = 360,             // Výška zdi (y range - 760)
                    wallColor
                ));



                // H7.1 Začíná mezerou od levy zdi, konci zacatkem treti prostupne zdi
                Walls.Add(new Wall(
                    wallX + 70, // Začátek X od levyho okraje
                    wallY + 680,             // Začátek Y od horniho okraje (750)
                    wallXRange = 70, // Délka zdi (x range - 400)
                    wallYRange = 10,             // Výška zdi (y range - 760)
                    wallColor
                ));

                //TRETI PROSTUPNA ZED
                // H7.2.1   Začíná koncem 7.1, konci 7.2.2
                Walls.Add(new Wall(
                    wallX + 130, // Začátek X od levyho okraje
                    wallY + 680,             // Začátek Y od horniho okraje (750)
                    wallXRange = 195, // Délka zdi (x range - 400)
                    wallYRange = 10,             // Výška zdi (y range - 760)
                    wallColor,
                    wallType.OneWaySolidFromBottom
                ));
                #endregion
            }


            public void Update(float deltaTime)
            {
                if (IsGameWon || PlayerBall == null)
                {
                    return;
                }

                var previousPosition = PlayerBall.Position;
                Vector2 rawAccel = AccelerationInput;
                Vector2 movementForce = new Vector2(rawAccel.X, -rawAccel.Y) * MoveSpeed * deltaTime;

                // Future position
                PointF newPosition = new PointF(
                    PlayerBall.Position.X + movementForce.X,
                    PlayerBall.Position.Y + movementForce.Y
                );

                PointF newPosX = new PointF(newPosition.X, PlayerBall.Position.Y); //future position
                RectF boundsX = new RectF(newPosX.X - PlayerBall.Radius, newPosX.Y - PlayerBall.Radius, PlayerBall.Radius * 2, PlayerBall.Radius * 2); //x box colliders
                bool collidedX = false;

                foreach (var wall in Walls)
                {
                    if (boundsX.IntersectsWith(wall.Bounds))
                    {
                        bool block = wall.Type 
                        switch
                        {
                            wallType.Normal => true,
                            wallType.Invisible => true,
                            wallType.OneWaySolidFromRight =>
                                movementForce.X < 0 && boundsX.Right > wall.Bounds.Left && PlayerBall.Bounds.Left < wall.Bounds.Right,
                            _ => true //can go from left to right but not the other way
                        };

                        if (block)
                        {
                            collidedX = true;
                            break;
                        }
                    }
                }

                if (!collidedX)
                    PlayerBall.Position = new PointF(newPosX.X, PlayerBall.Position.Y);
                else
                    movementForce.X = 0;


                PointF newPosY = new PointF(PlayerBall.Position.X, newPosition.Y);
                RectF boundsY = new RectF(newPosY.X - PlayerBall.Radius, newPosY.Y - PlayerBall.Radius, PlayerBall.Radius * 2, PlayerBall.Radius * 2); //y box colliders
                bool collidedY = false;

                foreach (var wall in Walls)
                {
                    if (boundsY.IntersectsWith(wall.Bounds))
                    {
                        bool block = wall.Type 
                        switch
                        {
                            wallType.Normal => true,
                            wallType.Invisible => true,
                            wallType.OneWaySolidFromBottom =>
                                movementForce.Y < 0 && boundsY.Top < wall.Bounds.Bottom && PlayerBall.Bounds.Bottom > wall.Bounds.Bottom,
                            _ => true //
                        };

                        if (block)
                        {
                            collidedY = true;
                            break;
                        }
                    }
                }

                if (!collidedY)
                    PlayerBall.Position = new PointF(PlayerBall.Position.X, newPosY.Y);
                else
                    movementForce.Y = 0;

                // player collision check
                PlayerBall.Position = new PointF(
                    Math.Clamp(PlayerBall.Position.X, PlayerBall.Radius, CanvasWidth - PlayerBall.Radius),
                    Math.Clamp(PlayerBall.Position.Y, PlayerBall.Radius, CanvasHeight - PlayerBall.Radius)
                );

                // check game won
                if (GameGoal != null && PlayerBall.Bounds.IntersectsWith(GameGoal.Bounds))
                {
                    IsGameWon = true;
                }

                PlayerBall.Velocity = movementForce / deltaTime;
            }


            public void Draw(ICanvas canvas, RectF dirtyRect)
            { 
                //walls
                foreach (var wall in Walls)
                {
                    if (wall.Type == wallType.Invisible) // invisible walls
                    {
                        continue;
                    }

                    canvas.FillColor = wall.FillColor;

                    if (wall.Type != wallType.Normal) // special walls
                    {
                        canvas.FillColor = Colors.Brown;
                    }
                    canvas.FillRectangle(wall.Bounds);
                }

                // goal
                if (GameGoal != null)
                {
                    canvas.FillColor = GameGoal.FillColor;
                    canvas.FillRectangle(GameGoal.Bounds);
                }

                // player
                if (PlayerBall != null)
                {
                    canvas.FillColor = PlayerBall.FillColor;
                    canvas.FillCircle(PlayerBall.Position.X, PlayerBall.Position.Y, PlayerBall.Radius);
                }

                // game won
                if (IsGameWon)
                {
                    canvas.FontSize = 30;
                    canvas.FontColor = Colors.Green;
                    canvas.DrawString("YOU WIN!", CanvasWidth / 2, CanvasHeight / 2 - 15, HorizontalAlignment.Center);
                }

                if (!Start)
                {
                    canvas.FontSize = 30;
                    canvas.FontColor = Colors.Green;
                    canvas.DrawString("START", CanvasWidth / 2, CanvasHeight / 2 - 15, HorizontalAlignment.Center);
                }
            }
        }
    }
}