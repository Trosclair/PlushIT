using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PlushIT.Models
{
    public class Object3D
    {
        public List<Triangle3D> Triangles { get; } = [];
        public Dictionary<int, NormalPoint3D> Points = [];
        public Dictionary<int, Vector3D> Normals = [];

        private Object3D() { }

        public static Object3D Read(string filename)
        {
            Object3D obj = new();
            string[] arr = File.ReadAllLines(filename);
            NormalPoint3D[] trianglePoints = new NormalPoint3D[3];
            int pNumber = 1, vNumber = 1;

            foreach (string dataLine in arr)
            {
                string[] data = dataLine.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (data.Length > 0)
                {
                    if (data[0] == "v")
                    {
                        NormalPoint3D pt = new(new(Convert.ToDouble(data[1]), Convert.ToDouble(data[2]), Convert.ToDouble(data[3])), pNumber);
                        obj.Points.Add(pNumber, pt);
                        pNumber++;
                    }
                    else if (data[0] == "vn")
                    {
                        obj.Normals.Add(vNumber, new(Convert.ToSingle(data[1]), Convert.ToSingle(data[2]), Convert.ToSingle(data[3])));
                        vNumber++;
                    }
                    else if (data[0] == "f")
                    {
                        for (int i = 1; i < data.Length; i++)
                        {
                            string[] fields = data[i].Split("/");

                            trianglePoints[i - 1] = obj.Points[Convert.ToInt32(fields[0])];

                            if (fields.Length > 2)
                            {
                                trianglePoints[i - 1].Normal = obj.Normals[fields.Length > 2 && fields[2].Length == 0 ? 0 : Convert.ToInt32(fields[2])];
                            }
                        }

                        Triangle3D triangle = new(trianglePoints[0], trianglePoints[1], trianglePoints[2]);

                        Hex3D? line1To2 = obj.Points[trianglePoints[0].PointNumber].ConnectedLineSegments.FirstOrDefault(x => x.Contains(trianglePoints[1]));
                        Hex3D? line2To3 = obj.Points[trianglePoints[1].PointNumber].ConnectedLineSegments.FirstOrDefault(x => x.Contains(trianglePoints[2]));
                        Hex3D? line3To1 = obj.Points[trianglePoints[2].PointNumber].ConnectedLineSegments.FirstOrDefault(x => x.Contains(trianglePoints[0]));

                        if (line1To2 is not null)
                        {
                            line1To2.Side2 = triangle;
                        }
                        else
                        {
                            line1To2 = new(trianglePoints[0], trianglePoints[1], 0.005D);
                            obj.Points[trianglePoints[0].PointNumber].ConnectedLineSegments.Add(line1To2);
                            obj.Points[trianglePoints[1].PointNumber].ConnectedLineSegments.Add(line1To2);
                            line1To2.Side1 = triangle;
                        }

                        if (line2To3 is not null)
                        {
                            line2To3.Side2 = triangle;
                        }
                        else
                        {
                            line2To3 = new(trianglePoints[1], trianglePoints[2], 0.005D);
                            obj.Points[trianglePoints[1].PointNumber].ConnectedLineSegments.Add(line2To3);
                            obj.Points[trianglePoints[2].PointNumber].ConnectedLineSegments.Add(line2To3);
                            line2To3.Side1 = triangle;
                        }

                        if (line3To1 is not null)
                        {
                            line3To1.Side2 = triangle;
                        }
                        else
                        {
                            line3To1 = new(trianglePoints[2], trianglePoints[0], 0.005D);
                            obj.Points[trianglePoints[2].PointNumber].ConnectedLineSegments.Add(line3To1);
                            obj.Points[trianglePoints[0].PointNumber].ConnectedLineSegments.Add(line3To1);
                            line3To1.Side1 = triangle;
                        }

                        triangle.SetLineSegments(line1To2, line2To3, line3To1);

                        obj.Triangles.Add(triangle);
                    }
                }
            }
            return obj;
        }
    }
}
