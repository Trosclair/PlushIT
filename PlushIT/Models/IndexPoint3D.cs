using PlushIT.Utilities;
using System.Windows.Media.Media3D;

namespace PlushIT.Models
{
    public class IndexPoint3D(Point3D point, int outerPositionNumber = -1)
    {
        public int InnerPositionNumber { get; set; } = -1;
        public int OuterPositionNumber { get; set; } = outerPositionNumber;
        public Point3D Point { get; set; } = point;

        public List<Surface3D> ConnectedSurfaces { get; set; } = [];
        public List<Line3D> ConnectedLines { get; set; } = [];

        public SphereMesh? SphereMesh { get; private set; } = null;

        public void UnRender(MeshGeometry3D geometry)
        {
            if (SphereMesh is not null)
            {
                int positionsLength = SphereMesh.Positions.Count();

                int lowestTriangleIndice = 0;
                for (int i = 0; i < geometry.Positions.Count - positionsLength + 1; i++)
                {
                    if (geometry.Positions[i] == SphereMesh.Positions.First())
                    {
                        bool isMatch = true;
                        for (int j = 1; j < positionsLength; j++)
                        {
                            if (geometry.Positions[j + i] != SphereMesh.Positions.ElementAt(j))
                            {
                                isMatch = false;
                                break;
                            }
                        }

                        if (isMatch)
                        {
                            lowestTriangleIndice = i;
                            for (int j = positionsLength + i - 1; j >= i; j--)
                            {
                                geometry.Positions.RemoveAt(j);
                            }
                            break;
                        }
                    }
                }

                for (int i = 0; i < geometry.TriangleIndices.Count; i++)
                {
                    if (geometry.TriangleIndices[i] == lowestTriangleIndice)
                    {
                        for (int j = 0; j < SphereMesh.TriangleIndices.Count(); j++)
                        {
                            geometry.TriangleIndices.RemoveAt(i);
                        }

                        for (int j = i; j < geometry.TriangleIndices.Count; j++)
                        {
                            geometry.TriangleIndices[j] -= SphereMesh.Positions.Count();
                        }

                        break;
                    }
                }
                SphereMesh = null;
            }
        }

        public void Render(MeshGeometry3D geometry)
        {
            if (SphereMesh is null)
            {
                int currentPosCount = geometry.Positions.Count;

                SphereMesh = SphereMesh.GenerateSphere(Point, .025);

                foreach (int i in SphereMesh.TriangleIndices.Select(x => x + currentPosCount))
                {
                    geometry.TriangleIndices.Add(i);
                }

                foreach (Point3D pt in SphereMesh.Positions)
                {
                    geometry.Positions.Add(pt);
                }
            }
        }

        public void RenderHover(MeshGeometry3D geometry)
        {
            geometry.Positions.Clear();
            geometry.TriangleIndices.Clear();

            SphereMesh sp = SphereMesh.GenerateSphere(Point, .03);

            foreach (Point3D pt in sp.Positions)
            {
                geometry.Positions.Add(pt);
            }

            foreach (int i in sp.TriangleIndices)
            {
                geometry.TriangleIndices.Add(i);
            }
        }
    }
}
