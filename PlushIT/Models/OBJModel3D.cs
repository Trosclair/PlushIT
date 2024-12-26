using PlushIT.Utilities;
using System.IO;
using System.Windows.Media.Media3D;

namespace PlushIT.Models
{
    public class OBJModel3D
    {
        public List<Surface3D> Surfaces { get; } = [];
        public List<IndexPoint3D> OuterTrianglePoints { get; } = [];
        public Dictionary<Point3D, IndexPoint3D> AllPoints { get; } = [];

        private OBJModel3D() { }

        public static OBJModel3D Read(string filename)
        {
            OBJModel3D obj = new();
            string[] arr = File.ReadAllLines(filename);
            IndexPoint3D[] trianglePoints = new IndexPoint3D[3];
            int surfaceIndex = 0;
            int outerPositionIndex = 0;
            int innerPositionIndex = 0;

            foreach (string dataLine in arr)
            {
                string[] data = dataLine.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (data.Length > 0)
                {
                    if (data[0] == "v")
                    {
                        IndexPoint3D pt = new(new(Convert.ToDouble(data[1]), Convert.ToDouble(data[2]), Convert.ToDouble(data[3])), outerPositionIndex++);
                        obj.OuterTrianglePoints.Add(pt);
                    }
                    else if (data[0] == "f")
                    {
                        for (int i = 1; i < data.Length; i++)
                        {
                            string[] fields = data[i].Split("/");

                            trianglePoints[i - 1] = obj.OuterTrianglePoints[Convert.ToInt32(fields[0]) - 1];
                        }

                        Surface3D surface = new(trianglePoints[0], trianglePoints[1], trianglePoints[2], surfaceIndex);

                        obj.AllPoints.TryAdd(surface.OuterTriangle.Point1.Point, surface.OuterTriangle.Point1);
                        obj.AllPoints.TryAdd(surface.OuterTriangle.Point2.Point, surface.OuterTriangle.Point2);
                        obj.AllPoints.TryAdd(surface.OuterTriangle.Point3.Point, surface.OuterTriangle.Point3);

                        if (surface.InnerUpperTriangle is not null && surface.InnerLowerTriangle is not null)
                        {
                            surface.InnerUpperTriangle.Point1.InnerPositionNumber = innerPositionIndex++;
                            surface.InnerUpperTriangle.Point2.InnerPositionNumber = innerPositionIndex++;
                            surface.InnerUpperTriangle.Point3.InnerPositionNumber = innerPositionIndex++;

                            surface.InnerLowerTriangle.Point1.InnerPositionNumber = innerPositionIndex++;
                            surface.InnerLowerTriangle.Point2.InnerPositionNumber = innerPositionIndex++;
                            surface.InnerLowerTriangle.Point3.InnerPositionNumber = innerPositionIndex++;

                            obj.AllPoints.Add(surface.InnerUpperTriangle.Point1.Point, surface.OuterTriangle.Point1);
                            obj.AllPoints.Add(surface.InnerUpperTriangle.Point2.Point, surface.OuterTriangle.Point2);
                            obj.AllPoints.Add(surface.InnerUpperTriangle.Point3.Point, surface.OuterTriangle.Point3);
                                                                                               
                            obj.AllPoints.Add(surface.InnerLowerTriangle.Point1.Point, surface.OuterTriangle.Point1);
                            obj.AllPoints.Add(surface.InnerLowerTriangle.Point2.Point, surface.OuterTriangle.Point2);
                            obj.AllPoints.Add(surface.InnerLowerTriangle.Point3.Point, surface.OuterTriangle.Point3);
                        }
                        else
                        {
                            surface.InnerTriangle.Point1.InnerPositionNumber = innerPositionIndex++;
                            surface.InnerTriangle.Point2.InnerPositionNumber = innerPositionIndex++;
                            surface.InnerTriangle.Point3.InnerPositionNumber = innerPositionIndex++;

                            if (ThirdDimensionalCalculations.AreaOfTriangle(surface.InnerTriangle.Point1.Point, surface.InnerTriangle.Point2.Point, surface.InnerTriangle.Point3.Point) > 1E-10)
                            {
                                obj.AllPoints.Add(surface.InnerTriangle.Point1.Point, surface.InnerTriangle.Point1);
                                obj.AllPoints.Add(surface.InnerTriangle.Point2.Point, surface.InnerTriangle.Point2);
                                obj.AllPoints.Add(surface.InnerTriangle.Point3.Point, surface.InnerTriangle.Point3);
                            }
                        }

                        obj.Surfaces.Add(surface);
                        surfaceIndex++;
                    }
                }
            }

            int lineIndex = 0;
            foreach (IndexPoint3D pt in obj.OuterTrianglePoints)
            {
                List<Surface3D> unfinishedSurfaces = [.. pt.ConnectedSurfaces];
                for (int i = 0; i < unfinishedSurfaces.Count - 1; i++)
                {
                    for (int j = i + 1; j < unfinishedSurfaces.Count; j++)
                    {
                        if (Line3D.TryCreateLine(unfinishedSurfaces[i], unfinishedSurfaces[j], lineIndex) is Line3D line)
                        {
                            line.StartPoint.ConnectedLines.Add(line);
                            line.EndPoint.ConnectedLines.Add(line);
                            lineIndex++;
                        }
                    }
                }
            }
            
            return obj;
        }
    }
}
