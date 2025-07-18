﻿using System;
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
        public Vector2 Velocity { get; set; }


        public Ball(float x, float y, float radius, Color color)
        {
            Position = new PointF(x, y);
            Radius = radius;
            FillColor = color;
            Velocity = Vector2.Zero;
        }

        // collision checking
        public RectF Bounds => new RectF(Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);
    }

    public enum wallType
    {
        Normal,                 // Klasická pevná zeď
        Invisible,              // Pevná, ale nekreslí se
        OneWaySolidFromBottom,  // Pevná zespodu (nedá se projít nahoru), prostupná shora (dá se projít dolů)
        OneWaySolidFromRight    // Pevná zleva (nedá se projít doprava), prostupná zprava (dá se projít doprava)
    }

    public enum SceneType //PRIDAT LEVELY
    {
        Start,
        Level1,
        Level2,
        Level3,
        Win
    }

    public class Wall
    {
        public RectF Bounds { get; set; }
        public Color FillColor { get; set; }
        public wallType Type { get; set; }
        public string Tag { get; set; }

        public Wall(float x, float y, float width, float height, Color color, wallType type = wallType.Normal, string tag = null)
        {
            Bounds = new RectF(x, y, width, height);
            FillColor = color;
            Type = type;
            Tag = tag;
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
