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
        public Dictionary<int, NormalPoint3D> OuterPoints { get; } = [];
        public Dictionary<Point3D, Triangle3D> InnerPointsToTriangle { get; } = [];
        public Dictionary<int, Vector3D> Normals { get; } = [];

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
                        NormalPoint3D pt = new(new(Convert.ToDouble(data[1]), Convert.ToDouble(data[2]), Convert.ToDouble(data[3])));
                        obj.OuterPoints.Add(pNumber, pt);
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

                            trianglePoints[i - 1] = obj.OuterPoints[Convert.ToInt32(fields[0])];

                            if (fields.Length > 2)
                            {
                                trianglePoints[i - 1].Normal = obj.Normals[fields.Length > 2 && fields[2].Length == 0 ? 0 : Convert.ToInt32(fields[2])];
                            }
                        }

                        Triangle3D triangle = new(trianglePoints[0], trianglePoints[1], trianglePoints[2]);

                        //obj.InnerPointsToTriangle.Add(triangle.InnerPoint1.Point, triangle);
                        //obj.InnerPointsToTriangle.Add(triangle.InnerPoint2.Point, triangle);
                        //obj.InnerPointsToTriangle.Add(triangle.InnerPoint3.Point, triangle);

                        obj.Triangles.Add(triangle);
                    }
                }
            }
            return obj;
        }
    }
}
