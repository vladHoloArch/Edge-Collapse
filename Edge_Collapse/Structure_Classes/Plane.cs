using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edge_Collapse
{
    public class Plane
    {
        private Point xyz;
        private double d;
        public void SetPlane(Point coordinate, Point normal)
        {
            xyz = new Point() { X = normal.X, Y = normal.Y, Z = normal.Z };
            d = (normal.X * -(coordinate.X)) + (normal.Y * -(coordinate.Y)) + (normal.Z * -(coordinate.Z));
        }

        public void GetPlane(out double a, out double b, out double c, out double d)
        {
            a = xyz.X;
            b = xyz.Y;
            c = xyz.Z;
            d = this.d;
        }
    };
}
