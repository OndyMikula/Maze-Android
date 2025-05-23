using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;

namespace Maze_Accelerometer.Classes
{
    public class Ball
    {
        public PointF Position { get; set; }
        public float Radius { get; set; }
        public Color FillColor { get; set; }
        // Přidáme Velocity pro případ, že bychom ji později chtěli pro plynulejší pohyb nebo efekty
        public Vector2 Velocity { get; set; }


        public Ball(float x, float y, float radius, Color color)
        {
            Position = new PointF(x, y);
            Radius = radius;
            FillColor = color;
            Velocity = Vector2.Zero;
        }

        // Bounds pro detekci kolizí
        public RectF Bounds => new RectF(Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);
    }

    public class Wall
    {
        public RectF Bounds { get; set; }
        public Color FillColor { get; set; }

        public Wall(float x, float y, float width, float height, Color color)
        {
            Bounds = new RectF(x, y, width, height);
            FillColor = color;
        }
    }

    public class Goal
    {
        public RectF Bounds { get; set; }
        public Color FillColor { get; set; }

        public Goal(float x, float y, float size, Color color)
        {
            Bounds = new RectF(x, y, size, size);
            FillColor = color;
        }
    }
}
