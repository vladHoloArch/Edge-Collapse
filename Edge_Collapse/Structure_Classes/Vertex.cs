using System.Collections.Generic;

namespace Edge_Collapse
{
    public class Vertex 
    {
        public int index;
        public int trianglesStartPosition;
        public int numberOfTriangles;
        public bool border;
        public double[,] Q;
        public float combinedQerrorSize;
        public List<Vertex> edges;
        public Point location;

        public Vertex(double i_X, double i_Y, double i_Z)
        {
            Q = new double[4, 4];
            location = new Point() { X = i_X, Y = i_Y, Z = i_Z};
        }

        public Vertex()
        {

        }

        private float[][] yuck(float[,] toYuck)
        {
            int rows = toYuck.GetLength(0);
            int cols = toYuck.GetLength(1);
            int i = 0;
            float[][] result = new float[rows][];

            for (i = 0; i < rows; i++)
            {
                result[i] = new float[cols];
            }

            for (i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i][j] = toYuck[i, j];
                }
            }

            return result;
        }

        private bool checkIfAlreadyThere(List<Vertex> i_List, Vertex i_VertexToAdd)
        {
            bool result = false;

            foreach (Vertex v in i_List)
            {
                if (v.index == i_VertexToAdd.index)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}
