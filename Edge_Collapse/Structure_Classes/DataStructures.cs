using System;

namespace Edge_Collapse
{
    public class Face 
    {
        public Point faceNormal;
        private Plane plane;
        public bool deleted;
        public bool dirty;
        public int attr;
        public double[,] Q;
        public int[] vertices = new int[3];
        public double[] qErrors = new double[4];
        public Point[] uvs = new Point[3];

        public Face(int point1, int point2, int point3)
        {
            vertices[0] = point1;
            vertices[1] = point2;
            vertices[2] = point3;
            plane = new Plane();
            //plane.SetPlane()
        }

        public Face() { }

        public void SetPlane(Point coordinate)
        {
            plane.SetPlane(coordinate, faceNormal);
            quadric();
        }
        private void quadric()
        {
            double A, B, C, D;
            plane.GetPlane(out A, out B, out C, out D);

            Q = new double[4, 4]
                {
                    {A * A, A * B, A * C, A * D },
                    {A * B, B * B, B * C, B * D },
                    {A * C, B * C, C * C, C * D },
                    {A * D, B * D, C * D, D * D }
                };
        }
    };
}
