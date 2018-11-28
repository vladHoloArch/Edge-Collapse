using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Edge_Collapse
{
    public static class EdgeCollapser
    {


        /* [Vx, Vy, Vz, 1] Q |Vx|
                             |Vy|
                             |Vz|
                             |1 |*/
        private static double VertexError(double[,] i_Q, double i_X, double i_Y, double i_Z)
        {
            double[,] resultA = MultiplyMatrix(new double[1, 4] { { i_X, i_Y, i_Z, 1f } }, i_Q);
            return MultiplyMatrix(resultA, new double[4, 1] { { i_X }, { i_Y }, { i_Z }, { 1f } })[0, 0];
        }

        /* |q11 q12 q13 q14| -1 |0|
         * |q12 q22 q23 q24|    |0|
         * |q13 q23 q33 q34|    |0| 
         * | 0   0   0   1 |    |1|*/
        private static double CombinedError(int i_Vertex1, int i_Vertex2, ref Point o_NewPoint)
        {
            bool isBorder = Globals.VERTICES[i_Vertex1].border & Globals.VERTICES[i_Vertex2].border;
            double result;
            double[][] vertex = null;
            double[,] qCombined = Matrix.AddMatrices(Globals.VERTICES[i_Vertex1].Q, Globals.VERTICES[i_Vertex2].Q);

            if (!isBorder)
            {
                try
                {
                    vertex = Matrix.MatrixProduct(Matrix.MatrixInverse(yuck(qCombined)), new double[][] { new double[] { 0 },
                                                                                                          new double[] { 0 },
                                                                                                          new double[] { 0 },
                                                                                                          new double[] { 1 } });
                    o_NewPoint.X = vertex[0][0];
                    o_NewPoint.Y = vertex[1][0];
                    o_NewPoint.Z = vertex[2][0];

                    result = VertexError(qCombined, o_NewPoint.X, o_NewPoint.Y, o_NewPoint.Z);
                    //double arch = VertexError(Globals.VERTICES[i_Vertex1].Q, Globals.VERTICES[i_Vertex1].location.X, Globals.VERTICES[i_Vertex1].location.Y, Globals.VERTICES[i_Vertex1].location.Z);
                    //double arch2 = VertexError(Globals.VERTICES[i_Vertex2].Q, Globals.VERTICES[i_Vertex2].location.X, Globals.VERTICES[i_Vertex2].location.Y, Globals.VERTICES[i_Vertex2].location.Z);
                    //result = arch + arch2;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Point p1 = Globals.VERTICES[i_Vertex1].location;
                    Point p2 = Globals.VERTICES[i_Vertex2].location;
                    Point p3 = Globals.VERTICES[i_Vertex1].location + Globals.VERTICES[i_Vertex2].location / 2;
                    double p1Error = VertexError(qCombined, p1.X, p1.Y, p1.Z);
                    double p2Error = VertexError(qCombined, p2.X, p2.Y, p2.Z);
                    double p3Error = VertexError(qCombined, p3.X, p3.Y, p3.Z);

                    result = Math.Min(p1Error, Math.Min(p2Error, p3Error));

                    if (result == p1Error)
                    {
                        o_NewPoint = p1;
                    }
                    else if (result == p2Error)
                    {
                        o_NewPoint = p2;
                    }
                    else
                    {
                        o_NewPoint = p3;
                    }
                }
            }
            else
            {
                Point p1 = Globals.VERTICES[i_Vertex1].location;
                Point p2 = Globals.VERTICES[i_Vertex2].location;
                Point p3 = Globals.VERTICES[i_Vertex1].location + Globals.VERTICES[i_Vertex2].location / 2;
                double p1Error = VertexError(qCombined, p1.X, p1.Y, p1.Z);
                double p2Error = VertexError(qCombined, p2.X, p2.Y, p2.Z);
                double p3Error = VertexError(qCombined, p3.X, p3.Y, p3.Z);

                result = Math.Min(p1Error, Math.Min(p2Error, p3Error));

                if (result == p1Error)
                {
                    o_NewPoint = p1;
                }
                else if (result == p2Error)
                {
                    o_NewPoint = p2;
                }
                else
                {
                    o_NewPoint = p3;
                }
            }

            return result;
        }

        private static void trimTriangles(int i_Index, ref Vertex i_Vertex, ref List<bool> o_Deleted, ref int o_NumberOfDeletedTriangles)
        {
            Point p = new Point();

            for (int i = 0; i < i_Vertex.numberOfTriangles; i++)
            {
                Reference reference = Globals.REFERENCES[i_Vertex.trianglesStartPosition + i];
                Face face = Globals.FACES[reference.triangleId];

                if (face.deleted)
                {
                    continue;
                }
                if (o_Deleted[i] == true)
                {
                    face.deleted = true;
                    o_NumberOfDeletedTriangles++;
                    continue;
                }

                face.dirty = true;

                // update collapsed in vertex
                face.vertices[reference.triangleVertex] = i_Index;
                face.qErrors[0] = CombinedError(face.vertices[0], face.vertices[1], ref p);
                face.qErrors[1] = CombinedError(face.vertices[1], face.vertices[2], ref p);
                face.qErrors[2] = CombinedError(face.vertices[2], face.vertices[0], ref p);
                face.qErrors[3] = Math.Min(face.qErrors[0], Math.Min(face.qErrors[1], face.qErrors[2]));
                Globals.REFERENCES.Add(reference);
            }
        }

        private static void updateMesh(int i_NumberOfIteration)
        {
            if (i_NumberOfIteration > 0)
            {
                int counter = 0;

                for (int i = 0; i < Globals.FACES.Count; i++)
                {
                    if (!Globals.FACES[i].deleted)
                    {
                        Globals.FACES[counter++] = Globals.FACES[i];
                    }

                }

                Resize(ref Globals.FACES, counter);

            }

            if (i_NumberOfIteration == 0)
            {
                for (int i = 0; i < Globals.VERTICES.Count; i++)
                {
                    Globals.VERTICES[i].Q = new double[4, 4];
                }

                for (int i = 0; i < Globals.FACES.Count; i++)
                {
                    Face face = Globals.FACES[i];
                    Point normal = new Point();
                    Point[] points = new Point[3];

                    for (int j = 0; j < 3; j++)
                    {
                        points[j] = Globals.VERTICES[face.vertices[j]].location;
                    }

                    normal.cross(points[1] - points[0], points[2] - points[0]);
                    normal.Normalize();
                    face.faceNormal = normal;
                    face.SetPlane(points[0]);

                    for (int j = 0; j < 3; j++)
                    {
                        Globals.VERTICES[face.vertices[j]].Q = Matrix.AddMatrices(face.Q, Globals.VERTICES[face.vertices[j]].Q);
                    }
                }

                for (int i = 0; i < Globals.FACES.Count; i++)
                {
                    Face face = Globals.FACES[i];
                    Point point = new Point(); ;

                    for (int j = 0; j < 3; j++)
                    {
                        face.qErrors[j] = CombinedError(face.vertices[j], face.vertices[(j + 1) % 3], ref point);
                    }

                    face.qErrors[3] = Math.Min(face.qErrors[0], Math.Min(face.qErrors[1], face.qErrors[2]));
                }
            }

            for (int i = 0; i < Globals.VERTICES.Count; i++)
            {
                Globals.VERTICES[i].trianglesStartPosition = 0;
                Globals.VERTICES[i].numberOfTriangles = 0;
            }

            for (int i = 0; i < Globals.FACES.Count; i++)
            {
                Face face = Globals.FACES[i];

                for (int j = 0; j < 3; j++)
                {
                    Globals.VERTICES[face.vertices[j]].numberOfTriangles++;
                }
            }

            int countStart = 0;

            for (int i = 0; i < Globals.VERTICES.Count; i++)
            {
                Vertex vertex = Globals.VERTICES[i];
                vertex.trianglesStartPosition = countStart;
                countStart += vertex.numberOfTriangles;
                vertex.numberOfTriangles = 0;
            }

            Resize(ref Globals.REFERENCES, Globals.FACES.Count * 3);

            for (int i = 0; i < Globals.FACES.Count; i++)
            {
                Face face = Globals.FACES[i];

                for (int j = 0; j < 3; j++)
                {
                    Vertex v = Globals.VERTICES[face.vertices[j]];
                    Globals.REFERENCES[v.trianglesStartPosition + v.numberOfTriangles].triangleId = i;
                    Globals.REFERENCES[v.trianglesStartPosition + v.numberOfTriangles].triangleVertex = j;

                    v.numberOfTriangles++;
                }
            }

            if (i_NumberOfIteration == 0)
            {
                List<int> vertexCounter = new List<int>();
                List<int> vertexIds = new List<int>();

                for (int i = 0; i < Globals.VERTICES.Count; i++)
                {
                    Globals.VERTICES[i].border = false;
                }

                for (int i = 0; i < Globals.VERTICES.Count; i++)
                {
                    Vertex vertex = Globals.VERTICES[i];
                    vertexCounter.Clear();
                    vertexIds.Clear();

                    for (int j = 0; j < vertex.numberOfTriangles; j++)
                    {
                        int tmp = Globals.REFERENCES[vertex.trianglesStartPosition + j].triangleId;
                        Face face = Globals.FACES[tmp];

                        for (int k = 0; k < 3; k++)
                        {
                            int offset = 0;
                            int id = face.vertices[k];

                            while (offset < vertexCounter.Count)
                            {
                                if (vertexIds[offset] == id)
                                {
                                    break;
                                }

                                offset++;
                            }

                            if (offset == vertexCounter.Count)
                            {
                                vertexCounter.Add(1);
                                vertexIds.Add(id);
                            }
                            else
                            {
                                vertexCounter[offset]++;
                            }
                        }
                    }

                    for (int j = 0; j < vertexCounter.Count; j++)
                    {
                        if (vertexCounter[j] == 1)
                        {
                            Globals.VERTICES[vertexIds[j]].border = true;
                        }
                    }
                }
            }
        }

        #region not using uvs
        private static void updateUVS(int i_Id, ref Vertex i_Vertex, ref Point i_Point, List<bool> i_Deleted)
        {
            for (int i = 0; i < i_Vertex.numberOfTriangles; i++)
            {
                Reference reference = Globals.REFERENCES[i_Vertex.trianglesStartPosition + 1];
                Face face = Globals.FACES[reference.triangleId];

                if (face.deleted)
                {
                    continue;
                }

                if (i_Deleted[i])
                {
                    continue;
                }

                Point point1, point2, point3;
                point1 = Globals.VERTICES[face.vertices[0]].location;
                point2 = Globals.VERTICES[face.vertices[1]].location;
                point3 = Globals.VERTICES[face.vertices[2]].location;
                face.uvs[reference.triangleVertex] = interpolate(i_Point, point1, point2, point3, face.uvs);
            }
        }

        private static Point barycentric(Point i_Point, Point i_A, Point i_B, Point i_C)
        {
            Point v0 = i_B - i_A;
            Point v1 = i_C - i_A;
            Point v2 = i_Point - i_A;
            double d00 = v0.dot(v0);
            double d01 = v0.dot(v1);
            double d11 = v1.dot(v1);
            double d20 = v2.dot(v0);
            double d21 = v2.dot(v1);
            double denom = d00 * d11 - d01 * d01;
            double v = (d11 * d20 - d01 * d21) / denom;
            double w = (d00 * d21 - d01 * d20) / denom;
            double u = 1.0 - v - w;
            return new Point() { X = u, Y = v, Z = w };
        }

        private static Point interpolate(Point i_Point, Point i_Point1, Point i_Point2, Point i_Point3, Point[] attributes)
        {
            Point b = barycentric(i_Point, i_Point1, i_Point2, i_Point3);
            Point output = new Point() { X = 0, Y = 0, Z = 0 };
            output = output + attributes[0] * b.X;
            output = output + attributes[1] * b.Y;
            output = output + attributes[2] * b.Z;

            return output;
        }
        #endregion

        private static bool isFlipped(Point i_Point, int i_Id0, int i_Id1, Vertex i_Vertex0, Vertex i_Vertex1, List<bool> i_Deleted)
        {

            for (int i = 0; i < i_Vertex0.numberOfTriangles; i++)
            {
                Face face = Globals.FACES[Globals.REFERENCES[i_Vertex0.trianglesStartPosition + i].triangleId];

                if (face.deleted)
                {
                    continue;
                }

                int start = Globals.REFERENCES[i_Vertex0.trianglesStartPosition + i].triangleVertex;
                int id1 = face.vertices[(start + 1) % 3];
                int id2 = face.vertices[(start + 2) % 3];

                if (id1 == i_Id1 || id2 == i_Id1)
                {
                    i_Deleted[i] = true;
                    continue;
                }

                Point d1 = Globals.VERTICES[id1].location - i_Point;
                d1.Normalize();
                Point d2 = Globals.VERTICES[id2].location - i_Point;
                d2.Normalize();

                if (Math.Abs(d1.dot(d2)) > 0.999)
                {
                    return true;
                }

                Point normal = new Point();
                normal.cross(d1, d2);
                normal.Normalize();
                i_Deleted[i] = false;

                if (normal.dot(face.faceNormal) < 0.2)
                {
                    return true;
                }
            }

            return false;
        }

        private static void compactMesh()
        {
            int destination = 0;

            for (int i = 0; i < Globals.VERTICES.Count; i++)
            {
                Globals.VERTICES[i].numberOfTriangles = 0;
            }

            for (int i = 0; i < Globals.FACES.Count; i++)
            {
                if (!Globals.FACES[i].deleted)
                {
                    Face face = Globals.FACES[i];
                    Globals.FACES[destination++] = face;

                    for (int j = 0; j < 3; j++)
                    {
                        Globals.VERTICES[face.vertices[j]].numberOfTriangles = 1;
                    }
                }
            }

            Resize(ref Globals.FACES, destination);
            destination = 0;

            for (int i = 0; i < Globals.VERTICES.Count; i++)
            {
                if (Globals.VERTICES[i].numberOfTriangles != 0)
                {
                    Globals.VERTICES[i].trianglesStartPosition = destination;
                    Globals.VERTICES[destination].location = Globals.VERTICES[i].location;
                    destination++;
                }
            }

            for (int i = 0; i < Globals.FACES.Count; i++)
            {
                Face face = Globals.FACES[i];

                for (int j = 0; j < 3; j++)
                {
                    face.vertices[j] = Globals.VERTICES[face.vertices[j]].trianglesStartPosition;
                }
            }

            Resize(ref Globals.VERTICES, destination);
        }

        public static void Resize<T>(ref List<T> list, int sz) where T : new()
        {
            int cur = list.Count;

            if (sz < cur)
                list.RemoveRange(sz, cur - sz);
            else if (sz > cur)
            {
                if (sz > list.Capacity)//this bit is purely an optimisation, to avoid multiple automatic capacity changes.
                {
                    list.Capacity = sz;
                }

                T[] array = new T[sz - cur];

                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = new T();
                }

                list.AddRange(array);
            }
        }

     
        private static double[][] yuck(double[,] toYuck)
        {
            int rows = toYuck.GetLength(0);
            int cols = toYuck.GetLength(1);
            int i = 0;
            double[][] result = new double[rows][];

            for (i = 0; i < rows; i++)
            {
                result[i] = new double[cols];
            }

            for (i = 0; i < rows - 1; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i][j] = toYuck[i, j];
                }
            }

            result[i] = new double[] { 0, 0, 0, 1 };

            return result;
        }

        private static double[,] MultiplyMatrix(double[,] A, double[,] B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);
            int rB = B.GetLength(0);
            int cB = B.GetLength(1);
            double temp = 0f;
            double[,] result = new double[rA, cB];

            if (cA != rB)
            {
                Console.WriteLine("matrix can't be multiplied");
                result = null;
            }
            else
            {
                for (int i = 0; i < rA; i++)
                {
                    for (int j = 0; j < cB; j++)
                    {
                        temp = 0;
                        for (int k = 0; k < cA; k++)
                        {
                            temp += A[i, k] * B[k, j];
                        }
                        result[i, j] = temp;
                    }
                }
            }

            return result;
        }

        public static void Run(int i_TargetCount, double agressiveness = 7)
        {
            for (int i = 0; i < Globals.FACES.Count; i++)
            {
                Globals.FACES[i].deleted = false;
            }

            int deletedTrianglesCounter = 0;
            List<bool> deleted1 = new List<bool>();
            List<bool> deleted2 = new List<bool>();
            int numberOfFaces = Globals.FACES.Count;

            for (int iteration = 0; iteration < 100; iteration++)
            {
                if (numberOfFaces - deletedTrianglesCounter <= i_TargetCount)
                {
                    break;
                }

                if (iteration % 5 == 0)
                {
                    updateMesh(iteration);
                }

                for (int j = 0; j < Globals.FACES.Count; j++)
                {
                    Globals.FACES[j].dirty = false;
                }

                double threshold = 0.000000001 * Math.Pow((iteration + 3), agressiveness);

                for (int j = 0; j < Globals.FACES.Count; j++)
                {
                    Face face = Globals.FACES[j];

                    if (face.qErrors[3] > threshold)
                    {
                        continue;
                    }

                    if (face.deleted)
                    {
                        continue;
                    }

                    if (face.dirty)
                    {
                        continue;
                    }

                    for (int k = 0; k < 3; k++)
                    {
                        if (face.qErrors[k] < threshold)
                        {
                            int pos0 = face.vertices[k];
                            Vertex v0 = Globals.VERTICES[pos0];
                            int pos1 = face.vertices[(k + 1) % 3];
                            Vertex v1 = Globals.VERTICES[pos1];

                            if (v0.border != v1.border)
                            {
                                continue;
                            }

                            Point point = new Point();
                            CombinedError(pos0, pos1, ref point);
                            Resize(ref deleted1, v0.numberOfTriangles);
                            Resize(ref deleted2, v1.numberOfTriangles);

                            if (isFlipped(point, pos0, pos1, v0, v1, deleted1) == true)
                            {
                                continue;
                            }

                            if (isFlipped(point, pos1, pos0, v1, v0, deleted2) == true)
                            {
                                continue;
                            }

                            v0.location = point;
                            v0.Q = Matrix.AddMatrices(v1.Q, v0.Q);
                            int faceStart = Globals.REFERENCES.Count;

                            trimTriangles(pos0, ref v0, ref deleted1, ref deletedTrianglesCounter);
                            trimTriangles(pos1, ref v1, ref deleted2, ref deletedTrianglesCounter);

                            int faceCount = Globals.REFERENCES.Count - faceStart;

                            if (faceCount <= v0.numberOfTriangles)
                            {
                                if (faceCount > 0)
                                {
                                    for (int ainKoah = 0; ainKoah < faceCount; ainKoah++)
                                    {
                                        Globals.REFERENCES[v0.trianglesStartPosition + ainKoah] = Globals.REFERENCES[faceStart + ainKoah].Clone() as Reference;
                                    }

                                    Globals.REFERENCES.RemoveRange(Globals.REFERENCES.Count - faceCount, faceCount);
                                }
                            }
                            else
                            {
                                v0.trianglesStartPosition = faceStart;
                            }

                            v0.numberOfTriangles = faceCount;

                            break;
                        }

                        if (numberOfFaces - deletedTrianglesCounter <= i_TargetCount)
                        {
                            break;
                        }
                    }
                }
            }

            compactMesh();
        }

        private static byte[] ObjectToByteArray(List<Reference> obj)
        {
            if (obj == null)
                return null;
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }
    enum Attributes
    {
        NONE,
        NORMAL = 2,
        TEXCOORD = 4,
        COLOR = 8
    };
}
