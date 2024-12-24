using System.IO;
using System.Windows.Media.Media3D;

namespace PlushIT.Models
{
    public class Model3D
    {
        public List<Surface3D> Surfaces { get; } = [];
        public Dictionary<int, IndexPoint3D> OuterTrianglePoints { get; } = [];
        public Dictionary<Point3D, IndexPoint3D> AllPoints { get; } = [];

        private Model3D() { }

        public static Model3D Read(string filename)
        {
            Model3D obj = new();
            string[] arr = File.ReadAllLines(filename);
            IndexPoint3D[] trianglePoints = new IndexPoint3D[3];
            int pNumber = 1;

            foreach (string dataLine in arr)
            {
                string[] data = dataLine.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (data.Length > 0)
                {
                    if (data[0] == "v")
                    {
                        IndexPoint3D pt = new(new(Convert.ToDouble(data[1]), Convert.ToDouble(data[2]), Convert.ToDouble(data[3])));
                        obj.OuterTrianglePoints.Add(pNumber, pt);
                        pNumber++;
                    }
                    else if (data[0] == "f")
                    {
                        for (int i = 1; i < data.Length; i++)
                        {
                            string[] fields = data[i].Split("/");

                            trianglePoints[i - 1] = obj.OuterTrianglePoints[Convert.ToInt32(fields[0])];
                        }

                        Surface3D surface = new(trianglePoints[0], trianglePoints[1], trianglePoints[2]);

                        obj.AllPoints.TryAdd(surface.OuterTriangle.Point1.Point, surface.OuterTriangle.Point1);
                        obj.AllPoints.TryAdd(surface.OuterTriangle.Point2.Point, surface.OuterTriangle.Point2);
                        obj.AllPoints.TryAdd(surface.OuterTriangle.Point3.Point, surface.OuterTriangle.Point3);

                        if (surface.InnerTriangle is not null)
                        {
                            obj.AllPoints.Add(surface.InnerTriangle.Point1.Point, surface.InnerTriangle.Point1);
                            obj.AllPoints.Add(surface.InnerTriangle.Point2.Point, surface.InnerTriangle.Point2);
                            obj.AllPoints.Add(surface.InnerTriangle.Point3.Point, surface.InnerTriangle.Point3);
                        }

                        obj.Surfaces.Add(surface);
                    }
                }
            }
            return obj;
        }
    }
}
