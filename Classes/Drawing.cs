using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using System.Numerics;

namespace Maze_Accelerometer.Classes
{
    internal class Drawing
    {
        public class MazeGameDrawable : IDrawable
        {
            public Ball PlayerBall { get; private set; }
            public List<Wall> Walls { get; private set; } = new List<Wall>();
            public Goal GameGoal { get; private set; }

            public float CanvasWidth { get; private set; } //Kraje prava a leva
            public float CanvasHeight { get; private set; } //Kraje horni a dolni sasku jeden

            public bool IsGameWon { get; private set; } = false;

            public Vector2 AccelerationInput { get; set; } = Vector2.Zero;
            // Konstanta pro rychlost pohybu kuličky
            private const float MoveSpeed = 2;
            private const float BallRadius = 10;


            public MazeGameDrawable() { }

            public void InitializeGame(float canvasWidth, float canvasHeight)
            {
                CanvasWidth = canvasWidth;
                CanvasHeight = canvasHeight;
                IsGameWon = false;

                // Startovní pozice kuličky (uprav dle tvého bludiště)
                // Např. vlevo nahoře v první chodbě
                PlayerBall = new Ball(30, 30, BallRadius, Colors.DodgerBlue);

                Walls.Clear();
                DefineMazeWalls(); // Metoda pro definici zdí

                // Cíl (uprav dle tvého bludiště)
                // Např. ve středu
                float goalSize = BallRadius * 2;
                GameGoal = new Goal(200, 400, goalSize, Colors.Gold);
            }

            private void DefineMazeWalls()
            {
                float wallThickness = 10;
                Color wallColor = Colors.DarkSlateGray;


                float wallX = wallThickness;
                float wallY = wallThickness;
                float wallYRange;
                float wallXRange;

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
            }


            public void Update(float deltaTime)
            {
                if (IsGameWon || PlayerBall == null) return;

                // Pohyb kuličky na základě akcelerometru
                // Převrácení Y osy je časté, protože akcelerometr dává kladné Y při naklonění "od sebe",
                // ale na plátně je Y kladné směrem dolů.
                Vector2 rawAccel = AccelerationInput;
                Vector2 movementForce = new Vector2(rawAccel.X, -rawAccel.Y) * MoveSpeed;

                // Uložíme si původní pozici pro případ, že bychom se museli vrátit
                PointF originalPosition = PlayerBall.Position;

                // 1. Pokus o pohyb v ose X
                float newX = PlayerBall.Position.X + movementForce.X;
                RectF testBoundsX = new RectF(newX - PlayerBall.Radius, PlayerBall.Position.Y - PlayerBall.Radius, PlayerBall.Radius * 2, PlayerBall.Radius * 2);
                bool collisionX = false;

                foreach (var wall in Walls)
                {
                    if (testBoundsX.IntersectsWith(wall.Bounds))
                    {
                        collisionX = true;
                        // Uprav pozici X tak, aby byla těsně u zdi
                        if (movementForce.X > 0) // Pohyb doprava
                            newX = wall.Bounds.Left - PlayerBall.Radius;
                        else if (movementForce.X < 0) // Pohyb doleva
                            newX = wall.Bounds.Right + PlayerBall.Radius;
                        break;
                    }
                }
                PlayerBall.Position = new PointF(newX, PlayerBall.Position.Y);


                // 2. Pokus o pohyb v ose Y (s již upravenou X pozicí)
                float newY = PlayerBall.Position.Y + movementForce.Y;
                RectF testBoundsY = new RectF(PlayerBall.Position.X - PlayerBall.Radius, newY - PlayerBall.Radius, PlayerBall.Radius * 2, PlayerBall.Radius * 2);
                bool collisionY = false;

                foreach (var wall in Walls)
                {
                    if (testBoundsY.IntersectsWith(wall.Bounds))
                    {
                        collisionY = true;
                        // Uprav pozici Y tak, aby byla těsně u zdi
                        if (movementForce.Y > 0) // Pohyb dolů
                            newY = wall.Bounds.Top - PlayerBall.Radius;
                        else if (movementForce.Y < 0) // Pohyb nahoru
                            newY = wall.Bounds.Bottom + PlayerBall.Radius;
                        break;
                    }
                }
                PlayerBall.Position = new PointF(PlayerBall.Position.X, newY);


                // Zajištění, aby kulička neopustila hranice plátna (i když by to měly řešit vnější zdi)
                PlayerBall.Position = new PointF(
                    Math.Clamp(PlayerBall.Position.X, PlayerBall.Radius, CanvasWidth - PlayerBall.Radius),
                    Math.Clamp(PlayerBall.Position.Y, PlayerBall.Radius, CanvasHeight - PlayerBall.Radius)
                );


                // Detekce cíle
                if (GameGoal != null && PlayerBall.Bounds.IntersectsWith(GameGoal.Bounds))
                {
                    IsGameWon = true;
                }
            }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                // Vykreslení pozadí (pokud není v XAML)
                // canvas.FillColor = Colors.LightSteelBlue;
                // canvas.FillRectangle(dirtyRect);

                // Vykreslení zdí
                foreach (var wall in Walls)
                {
                    if (wall.Type == wallType.Invisible) continue; // Neviditelné zdi nekreslíme

                    canvas.FillColor = wall.FillColor;
                    // Pro jednostranné zdi můžeme přidat vizuální indikaci, např. jinou barvu okraje nebo vzor
                    if (wall.Type != wallType.Normal) // Všechny "speciální" zdi (kromě Invisible)
                    {
                        canvas.FillColor = Colors.Brown; // Například oranžový okraj pro OneWay
                    }
                    canvas.FillRectangle(wall.Bounds);
                }

                // Vykreslení cíle
                if (GameGoal != null)
                {
                    canvas.FillColor = GameGoal.FillColor;
                    canvas.FillRectangle(GameGoal.Bounds);
                }

                // Vykreslení kuličky
                if (PlayerBall != null)
                {
                    canvas.FillColor = PlayerBall.FillColor;
                    canvas.FillCircle(PlayerBall.Position.X, PlayerBall.Position.Y, PlayerBall.Radius);
                }

                // Vykreslení zprávy o výhře (může být i v XAML, pokud se hra zastaví)
                if (IsGameWon)
                {
                    canvas.FontSize = 30;
                    canvas.FontColor = Colors.Green;
                    canvas.DrawString("YOU WIN!", CanvasWidth / 2, CanvasHeight / 2 - 15, HorizontalAlignment.Center);
                }
            }
        }
    }
}