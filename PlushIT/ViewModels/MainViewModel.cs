using Microsoft.Win32;
using PlushIT.Models;
using PlushIT.Utilities;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PlushIT.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private Point3D cameraPosition = new(-20, 100, 50);
        private GeometryModel3D triangleContent = new();
        private GeometryModel3D lineContent = new();
        private GeometryModel3D hoverContent = new();

        private MeshGeometry3D geometryTriangles = new();
        private MeshGeometry3D geometryLines = new();
        private MeshGeometry3D geometryHover = new();

        public Point3D CameraPosition { get => cameraPosition; set { cameraPosition = value; OnPropertyChanged(nameof(CameraPosition)); } }

        public Model3DGroup MVGroup { get; set; } = new();
        public Model3DGroup LinesGroup { get; set; } = new();
        public OBJModel3D? Model { get; set; }
        public MainViewModel()
        {
            OpenFileDialog ofd = new();

            if (ofd.ShowDialog() is bool b && b)
            {
                Model = OBJModel3D.Read(ofd.FileName);

                foreach (IndexPoint3D pt in Model.OuterTrianglePoints.Values.OrderBy(x => x.OuterPositionNumber))
                {
                    geometryLines.Positions.Add(pt.Point);
                }

                foreach (Surface3D surface in Model.Surfaces)
                {
                    if (surface.InnerUpperTriangle is not null && surface.InnerLowerTriangle is not null)
                    {
                        geometryTriangles.Positions.Add(surface.InnerUpperTriangle.Point1.Point);
                        geometryTriangles.Positions.Add(surface.InnerUpperTriangle.Point2.Point);
                        geometryTriangles.Positions.Add(surface.InnerUpperTriangle.Point3.Point);

                        geometryTriangles.Positions.Add(surface.InnerLowerTriangle.Point1.Point);
                        geometryTriangles.Positions.Add(surface.InnerLowerTriangle.Point2.Point);
                        geometryTriangles.Positions.Add(surface.InnerLowerTriangle.Point3.Point);

                        geometryTriangles.TriangleIndices.Add(surface.InnerUpperTriangle.Point1.InnerPositionNumber);
                        geometryTriangles.TriangleIndices.Add(surface.InnerUpperTriangle.Point2.InnerPositionNumber);
                        geometryTriangles.TriangleIndices.Add(surface.InnerUpperTriangle.Point3.InnerPositionNumber);

                        geometryTriangles.TriangleIndices.Add(surface.InnerLowerTriangle.Point1.InnerPositionNumber);
                        geometryTriangles.TriangleIndices.Add(surface.InnerLowerTriangle.Point2.InnerPositionNumber);
                        geometryTriangles.TriangleIndices.Add(surface.InnerLowerTriangle.Point3.InnerPositionNumber);
                    }
                    else if (surface.InnerTriangle is not null)
                    {
                        geometryTriangles.Positions.Add(surface.InnerTriangle.Point1.Point);
                        geometryTriangles.Positions.Add(surface.InnerTriangle.Point2.Point);
                        geometryTriangles.Positions.Add(surface.InnerTriangle.Point3.Point);

                        geometryTriangles.TriangleIndices.Add(surface.InnerTriangle.Point1.InnerPositionNumber);
                        geometryTriangles.TriangleIndices.Add(surface.InnerTriangle.Point2.InnerPositionNumber);
                        geometryTriangles.TriangleIndices.Add(surface.InnerTriangle.Point3.InnerPositionNumber);
                    }

                    geometryLines.TriangleIndices.Add(surface.OuterTriangle.Point1.OuterPositionNumber);
                    geometryLines.TriangleIndices.Add(surface.OuterTriangle.Point2.OuterPositionNumber);
                    geometryLines.TriangleIndices.Add(surface.OuterTriangle.Point3.OuterPositionNumber);
                }

                triangleContent.BackMaterial = new DiffuseMaterial(Brushes.Gray);
                triangleContent.Material = new DiffuseMaterial(Brushes.Gray);
                triangleContent.Geometry = geometryTriangles;

                lineContent.BackMaterial = new DiffuseMaterial(Brushes.Black);
                lineContent.Material = new DiffuseMaterial(Brushes.Black);
                lineContent.Geometry = geometryLines;

                hoverContent.BackMaterial = new DiffuseMaterial(Brushes.Yellow);
                hoverContent.Material = new DiffuseMaterial(Brushes.Yellow);
                hoverContent.Geometry = geometryHover;
                geometryHover.TriangleIndices = [0, 1, 2, 0, 2, 3, 3, 4, 5, 3, 5, 0];

                MVGroup.Children.Add(lineContent);
                MVGroup.Children.Add(triangleContent);
                LinesGroup.Children.Add(hoverContent);
            }
        }

        public void HighlightVertexFromHitTest(RayMeshGeometry3DHitTestResult hitTestResult)
        {
            if (hitTestResult.MeshHit.Positions.Count == geometryLines.Positions.Count || 
                hitTestResult.MeshHit.Positions.Count == geometryTriangles.Positions.Count)
            {
                Point3D pos1 = hitTestResult.MeshHit.Positions[hitTestResult.VertexIndex1];
                Point3D pos2 = hitTestResult.MeshHit.Positions[hitTestResult.VertexIndex2];
                Point3D pos3 = hitTestResult.MeshHit.Positions[hitTestResult.VertexIndex3];

                if (Model is not null)
                {
                    Surface3D? surface = null;
                    if (Model.AllPoints.TryGetValue(pos1, out IndexPoint3D? pt) && pt is not null)
                    {
                        surface = pt.ConnectedSurfaces.FirstOrDefault(x => x.IsSuppliedPointAVertice(pos2) && x.IsSuppliedPointAVertice(pos3));
                    }

                    if (surface?.FindClosestEdge(hitTestResult.PointHit) is Edge3D edge1)
                    {
                        Surface3D? otherSurface = edge1.StartPoint.ConnectedSurfaces.Intersect(edge1.EndPoint.ConnectedSurfaces).SingleOrDefault(x => x.SurfaceIndex != surface.SurfaceIndex);

                        if (otherSurface?.GetSharedEdge(edge1) is Edge3D edge2)
                        {
                            geometryHover.Positions = [edge1.StartPoint.Point, edge1.Point1.Point, edge1.Point2.Point, edge1.EndPoint.Point, edge2.Point1.Point, edge2.Point2.Point];
                        }
                    }
                }
            }
        }

        public void ChangeCameraXY(double x, double y)
        {
            CameraPosition = new(cameraPosition.X + (float)x, cameraPosition.Y + (float)y, cameraPosition.Z);
        }

    }
}
