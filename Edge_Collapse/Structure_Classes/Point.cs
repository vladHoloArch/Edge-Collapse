using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edge_Collapse
{

    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public static Point operator +(Point b, Point c)
        {
            Point point = new Point();
            point.X = b.X + c.X;
            point.Y = b.Y + c.Y;
            point.Z = b.Z + c.Z;

            return point;
        }

        public static Point operator /(Point b, int c)
        {
            Point point = new Point();
            point.X = b.X / c;
            point.Y = b.Y / c;
            point.Z = b.Z / c;

            return point;
        }

        public void cross(Point a, Point b)
        {
            X = a.Y * b.Z - a.Z * b.Y;
            Y = a.Z * b.X - a.X * b.Z;
            Z = a.X * b.Y - a.Y * b.X;
        }

        public static Point operator -(Point a, Point b)
        {
            Point point = new Point();
            point.X = a.X - b.X;
            point.Y = a.Y - b.Y;
            point.Z = a.Z - b.Z;

            return point;
        }

        public static Point operator *(Point a, double b)
        {
            return new Point() { X = a.X * b, Y = a.Y * b, Z = a.Z * b };
        }

        public void Normalize()
        {
            double sqrt = Math.Sqrt(X * X + Y * Y + Z * Z);

            X /= sqrt;
            Y /= sqrt;
            Z /= sqrt;
        }
        public double dot(Point a)
        {
            double result;
            result = X * a.X + Y * a.Y + Z * a.Z;
            return result;
        }
    }

}
