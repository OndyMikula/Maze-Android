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

            public float CanvasWidth { get; private set; }
            public float CanvasHeight { get; private set; }

            public bool IsGameWon { get; private set; } = false;

            public Vector2 AccelerationInput { get; set; } = Vector2.Zero;
            // Konstanta pro rychlost pohybu kuličky - TUTO BUDEŠ MOŽNÁ CHTÍT LADIT
            private const float MoveSpeed = 2.5f;
            private const float BallRadius = 8f; // Menší kulička pro snazší průchod bludištěm

            public MazeGameDrawable() { }

            public void InitializeGame(float canvasWidth, float canvasHeight)
            {
                CanvasWidth = canvasWidth;
                CanvasHeight = canvasHeight;
                IsGameWon = false;

                // Startovní pozice kuličky (uprav dle tvého bludiště)
                // Např. vlevo nahoře v první chodbě
                PlayerBall = new Ball(30f, 30f, BallRadius, Colors.DodgerBlue);

                Walls.Clear();
                DefineMazeWalls(); // Metoda pro definici zdí

                // Cíl (uprav dle tvého bludiště)
                // Např. ve středu
                float goalSize = BallRadius * 3;
                GameGoal = new Goal(CanvasWidth / 2 - goalSize / 2, CanvasHeight / 2 - goalSize / 2, goalSize, Colors.Gold);
            }

            private void DefineMazeWalls()
            {
                // Toto je příklad VELMI JEDNODUCHÉHO bludiště.
                // Budeš si muset vytvořit vlastní definici zdí pro spirálu.
                // (x, y, šířka, výška)
                float wallThickness = 10f;
                Color wallColor = Colors.DarkSlateGray;

                // Vnější ohraničení
                Walls.Add(new Wall(0, 0, CanvasWidth, wallThickness, wallColor)); // Horní
                Walls.Add(new Wall(0, CanvasHeight - wallThickness, CanvasWidth, wallThickness, wallColor)); // Dolní
                Walls.Add(new Wall(0, 0, wallThickness, CanvasHeight, wallColor)); // Levá
                Walls.Add(new Wall(CanvasWidth - wallThickness, 0, wallThickness, CanvasHeight, wallColor)); // Pravá

                // Příklad vnitřních zdí - vytvoř si zde vlastní spirálu!
                // Tloušťka = wallThickness
                // Chodba = např. 3 * BallRadius * 2 (pro průchod) ~ 48

                // Příklad pro začátek spirály:
                // Předpokládejme, že kulička startuje v (30,30)
                // Chodba je široká např. 50
                // 0,0 je levý horní roh
                float pathWidth = BallRadius * 2 + 20; // Šířka chodby, aby se kulička vešla + rezerva
                float currentX = wallThickness;
                float currentY = wallThickness;

                // 1. První horizontální zeď shora (nechává mezeru pro vstup)
                Walls.Add(new Wall(currentX + pathWidth + wallThickness, currentY, CanvasWidth - (currentX + pathWidth + wallThickness) - wallThickness, wallThickness, wallColor));
                // 2. První vertikální zeď vpravo
                Walls.Add(new Wall(CanvasWidth - wallThickness - pathWidth, currentY + wallThickness, wallThickness, CanvasHeight - 2 * wallThickness - pathWidth, wallColor));
                // 3. První horizontální zeď zespodu
                Walls.Add(new Wall(currentX + wallThickness, CanvasHeight - wallThickness - pathWidth, CanvasWidth - 2 * wallThickness - pathWidth - wallThickness, wallThickness, wallColor));
                // 4. První vertikální zeď vlevo (nechává mezeru pro další krok spirály)
                Walls.Add(new Wall(currentX + wallThickness, currentY + wallThickness + pathWidth, wallThickness, CanvasHeight - 2 * wallThickness - pathWidth - wallThickness, wallColor));

                // Pokračuj v tomto duchu, zmenšuj obdélník a nechávej mezery
                currentX += wallThickness + pathWidth;
                currentY += wallThickness + pathWidth;
                float innerCanvasWidth = CanvasWidth - 2 * (wallThickness + pathWidth);
                float innerCanvasHeight = CanvasHeight - 2 * (wallThickness + pathWidth);

                if (innerCanvasWidth > pathWidth * 2 && innerCanvasHeight > pathWidth * 2) // Aby mělo smysl pokračovat
                {
                    // 5. Druhá horizontální zeď shora
                    Walls.Add(new Wall(currentX + pathWidth + wallThickness, currentY, innerCanvasWidth - (pathWidth + wallThickness) - wallThickness, wallThickness, wallColor));
                    // A tak dále... toto je jen velmi hrubý nástřel, definice bludiště je piplačka
                }

                // Pamatuj: souřadnice (0,0) je levý horní roh.
                // X roste doprava, Y roste dolů.

                // Příklad jednoduché překážky uprostřed, abys viděl kolize:
                // Walls.Add(new Wall(CanvasWidth/2 - 50, CanvasHeight/2 - 5, 100, 10, wallColor));
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
                    canvas.FillColor = wall.FillColor;
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
