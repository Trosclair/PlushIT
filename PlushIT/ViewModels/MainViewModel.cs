using Microsoft.Win32;
using PlushIT.Models;
using PlushIT.Utilities;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace PlushIT.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private Point3D cameraPosition = new(-20, 100, 50);

        private readonly GeometryModel3D triangleContent = new();
        private readonly GeometryModel3D lineContent = new();
        private readonly GeometryModel3D hoverContent = new();
        private readonly GeometryModel3D selectedLinesContent = new();

        private readonly MeshGeometry3D geometryTriangles = new();
        private readonly MeshGeometry3D geometryLines = new();
        private readonly MeshGeometry3D geometryHover = new();
        private readonly MeshGeometry3D geometrySelectedLines = new();

        public Point3D CameraPosition { get => cameraPosition; set { cameraPosition = value; OnPropertyChanged(nameof(CameraPosition)); } }

        public Model3DGroup MVGroup { get; set; } = new();
        public Model3DGroup LinesGroup { get; set; } = new();
        public OBJModel3D? Model { get; set; }

        public Dictionary<int, Line3D> SelectedLines { get; } = [];

        public MainViewModel()
        {
            OpenFileDialog ofd = new();

            if (ofd.ShowDialog() is bool b && b)
            {
                Model = OBJModel3D.Read(ofd.FileName);
                foreach (IndexPoint3D pt in Model.OuterTrianglePoints)
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

                selectedLinesContent.BackMaterial = new DiffuseMaterial(Brushes.Green);
                selectedLinesContent.Material = new DiffuseMaterial(Brushes.Green);
                selectedLinesContent.Geometry = geometrySelectedLines;

                hoverContent.Geometry = geometryHover;

                MVGroup.Children.Add(lineContent);
                MVGroup.Children.Add(triangleContent);
                MVGroup.Children.Add(hoverContent);
                MVGroup.Children.Add(selectedLinesContent);
            }
        }

        public void SelectVertexFromHitTest(RayMeshGeometry3DHitTestResult hitTestResult)
        {
            ApplyFunctionToNearestEdge(hitTestResult, SelectVertex);
        }

        public void HighlightVertexFromHitTest(RayMeshGeometry3DHitTestResult hitTestResult)
        {
            ApplyFunctionToNearestEdge(hitTestResult, (edge) => RenderHoverLine(SelectedLines.ContainsKey(edge.LineIndex), edge));
        }

        public void ApplyFunctionToNearestEdge(RayMeshGeometry3DHitTestResult hitTestResult, Action<Line3D> act)
        {
            if (Model is not null)
            {
                Point3D[] pts =
                    [hitTestResult.MeshHit.Positions[hitTestResult.VertexIndex1],
                hitTestResult.MeshHit.Positions[hitTestResult.VertexIndex2],
                hitTestResult.MeshHit.Positions[hitTestResult.VertexIndex3]];

                List<IndexPoint3D> points = [];

                foreach (Point3D pos in pts)
                {
                    if (Model.AllPoints.TryGetValue(pos, out IndexPoint3D? pt) && pt is not null)
                    {
                        points.Add(pt);
                    }
                }

                //IEnumerable<Line3D> totalPointsWithDuplicates = points.SelectMany(x => x.ConnectedLines);
                //IEnumerable<IGrouping<int, Line3D>> commonLinesBetweenThePoints = totalPointsWithDuplicates.GroupBy(x => x.LineIndex);
                //int maxOccurencesOfALine = commonLinesBetweenThePoints.Max(x => x.Count());
                //IEnumerable<IGrouping<int, Line3D>> filteredGroupsOfLines = commonLinesBetweenThePoints.Where(x => x.Count() == maxOccurencesOfALine);
                //IEnumerable<Line3D> filteredLines = filteredGroupsOfLines.Select(x => x.FirstOrDefault()).OfType<Line3D>();

                //IEnumerable<Line3D> filteredLinesButUnreadable = points
                //    .SelectMany(x => x.ConnectedLines)
                //    .GroupBy(x => x.LineIndex)
                //    .Where(x => x.Count() == points.SelectMany(y => y.ConnectedLines).GroupBy(y => y.LineIndex).Max(y => y.Count()))
                //    .Select(x => x.FirstOrDefault())
                //    .OfType<Line3D>();

                //IEnumerable<Surface3D> totalSurfacesWithDuplicates = points.SelectMany(x => x.ConnectedSurfaces);
                //IEnumerable<IGrouping<int, Surface3D>> groupsOfTheSameSurface = totalSurfacesWithDuplicates.GroupBy(x => x.SurfaceIndex);
                //int maxOccurencesOfASurface = groupsOfTheSameSurface.Max(x => x.Count());
                //IEnumerable<IGrouping<int, Surface3D>> filteredGroupsOfSurfaces = groupsOfTheSameSurface.Where(x => x.Count() == maxOccurencesOfASurface);
                //IEnumerable<Surface3D> filteredSurfaces = filteredGroupsOfSurfaces.Select(x => x.FirstOrDefault()).OfType<Surface3D>();
                //IEnumerable<Line3D?[]> totalPointsWithNullsAndDuplicates = filteredSurfaces.Select(x => new Line3D?[] { x.Edge12, x.Edge23, x.Edge31 });
                //IEnumerable<Line3D> totalPointsWithDuplicates = totalPointsWithNullsAndDuplicates.SelectMany(x => x).OfType<Line3D>();
                //IEnumerable<IGrouping<int, Line3D>> commonLinesBetweenThePoints = totalPointsWithDuplicates.GroupBy(x => x.LineIndex);
                //int maxOccurencesOfALine = commonLinesBetweenThePoints.Max(x => x.Count());
                //IEnumerable<IGrouping<int, Line3D>> filteredGroupsOfLines = commonLinesBetweenThePoints.Where(x => x.Count() == maxOccurencesOfALine);
                //IEnumerable<Line3D> filteredLines = filteredGroupsOfLines.Select(x => x.FirstOrDefault()).OfType<Line3D>();

                Line3D? closestLine = null;

                if (points.Count == 1) // User moused over the worst part of a line... Greaaaaat Now I have to do math...
                {
                    closestLine = points[0].ConnectedLines.MinBy(x => 
                        ThirdDimensionalCalculations.DistanceFromPoint1ToLineBetweenPoints2And3(hitTestResult.PointHit, x.StartPoint.Point, x.EndPoint.Point));
                }
                else if (points.Count == 2) // Best case senario... User has basically given us the line for free by mousing over the correct part of the 'line'.
                {
                    closestLine = points[0].ConnectedLines
                        .IntersectBy(points[1].ConnectedLines.Select(x => x.LineIndex), x => x.LineIndex)
                        .SingleOrDefault(); // If you get more than one line here then we have broken some fundamental law of geometry since a line can't have more than one edge.                      
                }
                else if (points.Count == 3) // User is over the inside of the triangle, so now I get to do math again. Worst case senario.
                {
                    Surface3D? triangle = points
                        .SelectMany(x => x.ConnectedSurfaces)   // Flatten the list lists of triangles that these points are connected to... We are counting on there being duplicates here.
                        .GroupBy(x => x.SurfaceIndex)           // Group em by their unique index value assigned at birth.
                        .Where(x => x.Count() == 3)             // If a triangle shows up 3 times then it is because it is connected to all 3 points. Ladies and gentlemen... we got em. maybe.
                        .FirstOrDefault()?                      // There should only be one group of three if there is one.
                        .FirstOrDefault();                      // All 3 triangles are the same reference, so just snag the first one. You should be able to use just .Single, but I don't trust my own logic.

                    if (triangle is not null &&         
                        triangle.Edge12 is not null &&  // I gotta null check the edges because they are constructed after the containing triangle because I thought it'd be
                        triangle.Edge23 is not null &&  // helpful to have bordering triangle references in the edge themselves, but I just checked and I don't even use em.
                        triangle.Edge31 is not null)
                    {
                        // use the location of where the cursor touched the triangle and get the distances from each point.
                        // The function that calculates the distances does a projection of the cursor point onto the line.
                        // I found this sucks because if there is not a perpendicular line between pt1 and the line then you
                        // have to take the nearest of the two points. The computation is somewhat expensive and annoying to debug.
                        double distToEdge1 = ThirdDimensionalCalculations.DistanceFromPoint1ToLineBetweenPoints2And3(hitTestResult.PointHit, triangle.Edge12.StartPoint.Point, triangle.Edge12.EndPoint.Point);
                        double distToEdge2 = ThirdDimensionalCalculations.DistanceFromPoint1ToLineBetweenPoints2And3(hitTestResult.PointHit, triangle.Edge23.StartPoint.Point, triangle.Edge23.EndPoint.Point);
                        double distToEdge3 = ThirdDimensionalCalculations.DistanceFromPoint1ToLineBetweenPoints2And3(hitTestResult.PointHit, triangle.Edge31.StartPoint.Point, triangle.Edge31.EndPoint.Point);

                        //Find the closest edge.
                        if (distToEdge1 < distToEdge2 && distToEdge1 < distToEdge3)
                        {
                            closestLine = triangle.Edge12;
                        }
                        else if (distToEdge2 < distToEdge3 && distToEdge2 < distToEdge1)
                        {
                            closestLine = triangle.Edge23;
                        }
                        else
                        {
                            closestLine = triangle.Edge31;
                        }
                    }
                }

                if (closestLine is not null)
                {
                    act(closestLine);
                }
            }
        }

        private void SelectVertex(Line3D edge)
        {
            if (SelectedLines.TryAdd(edge.LineIndex, edge))
            {
                edge.Render(geometrySelectedLines);
                RenderHoverLine(true, edge);
            }
            else
            {
                SelectedLines.Remove(edge.LineIndex);
                edge.UnRender(geometrySelectedLines);
                RenderHoverLine(false, edge);
            }
        }

        private void RenderHoverLine(bool isSelected, Line3D edge)
        {
            hoverContent.BackMaterial = new DiffuseMaterial(isSelected ? Brushes.Red : Brushes.Yellow);
            hoverContent.Material = new DiffuseMaterial(isSelected ? Brushes.Red : Brushes.Yellow);
            edge.RenderHover(geometryHover);
        }

        public void ChangeCameraXY(double x, double y)
        {
            CameraPosition = new(cameraPosition.X + (float)x, cameraPosition.Y + (float)y, cameraPosition.Z);
        }

    }
}
