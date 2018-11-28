using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Edge_Collapse
{
    public class ObjHandler
    {
        public void LoadVertices(string i_FilePath)
        {
            Globals.FACES = new List<Face>();
            Globals.REFERENCES = new List<Reference>();
            string line = string.Empty;
            string[] coordinates, faces;
            double holder, locationX, locationY, locationZ;
            int position, index, pointIndex1, pointIndex2, pointIndex3, indexHolder;
            position = index = pointIndex1 = pointIndex2 = pointIndex3 = indexHolder = 0;
            locationX = locationY = locationZ = 0f;
            Globals.VERTICES = new Vertex[index].ToList();
            index = 0;

            using (StreamReader objReader = new StreamReader(i_FilePath))
            {
                while (!objReader.EndOfStream)
                {
                    line = objReader.ReadLine();

                    //////////////////////////////////////////////////////////////////////////////
                    if (line[0] == 'v')
                    {
                        coordinates = line.Split(' ');

                        foreach (string s in coordinates)
                        {
                            if (double.TryParse(s, out holder))
                            {
                                switch (position)
                                {
                                    case 0:
                                        locationX = holder;
                                        position++;
                                        break;
                                    case 1:
                                        locationY = holder;
                                        position++;
                                        break;
                                    case 2:
                                        locationZ = holder;
                                        break;
                                }
                            }
                        }

                        Globals.VERTICES.Add(new Vertex(locationX, locationY, locationZ));
                        position = 0;
                        index++;
                    }
                    ///////////////////////////////////////////////////////////////////////////////
                    else if (line[0] == 'f')
                    {
                        faces = line.Split(' ');

                        foreach (string s in faces)
                        {
                            if (Int32.TryParse(s, out indexHolder))
                            {
                                switch (position)
                                {
                                    case 0:
                                        pointIndex1 = indexHolder;
                                        position++;
                                        break;
                                    case 1:
                                        pointIndex2 = indexHolder;
                                        position++;
                                        break;
                                    case 2:
                                        pointIndex3 = indexHolder;
                                        break;
                                }
                            }
                        }

                        position = 0;
                        Face face = new Face(--pointIndex1, --pointIndex2, --pointIndex3);
                        face.attr = 0;
                        Globals.FACES.Add(face);
                    }
                }
            }
        }

        public void WriteObj()
        {
            using (StreamWriter writer = new StreamWriter("harta.obj"))
            {
                for (int i = 0; i <  Globals.VERTICES.Count; i++)
                {
                    Vertex v = Globals.VERTICES[i];

                    writer.WriteLine("v {0} {1} {2}", v.location.X, v.location.Y, v.location.Z);
                }

                for (int i = 0; i < Globals.FACES.Count; i++)
                {
                    Face f = Globals.FACES[i];

                    writer.WriteLine("f {0} {1} {2}", f.vertices[0] + 1, f.vertices[1] + 1, f.vertices[2] + 1);
                }
            }
        }
    }
}