using Microsoft.Win32;
using PlushIT.Enums;
using PlushIT.Models;
using PlushIT.Utilities;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace PlushIT.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private Point3D cameraPosition = new(-20, 100, 50);
        private Tool selectedTool = Tool.MultiEdge;
        private readonly Dictionary<int, Line3D> multiselectedLines = [];
        private Line3D lastEdgeSelected = null;

        private readonly GeometryModel3D triangleContent = new();
        private readonly GeometryModel3D lineContent = new();
        private readonly GeometryModel3D hoverContent = new();
        private readonly GeometryModel3D selectedLinesContent = new();
        private readonly GeometryModel3D multiselectedLinesContent = new();

        private readonly MeshGeometry3D geometryTriangles = new();
        private readonly MeshGeometry3D geometryLines = new();
        private readonly MeshGeometry3D geometryHover = new();
        private readonly MeshGeometry3D geometrySelectedLines = new();
        private readonly MeshGeometry3D geometryMultiSelectedLines = new();

        public Point3D CameraPosition { get => cameraPosition; set { cameraPosition = value; OnPropertyChanged(nameof(CameraPosition)); } }
        public Tool SelectedTool { get => selectedTool; set { selectedTool = value; OnPropertyChanged(nameof(SelectedTool)); } }

        public Model3DGroup MVGroup { get; set; } = new();
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

                multiselectedLinesContent.Geometry = geometryMultiSelectedLines;

                hoverContent.Geometry = geometryHover;

                MVGroup.Children.Add(lineContent);
                MVGroup.Children.Add(triangleContent);
                MVGroup.Children.Add(hoverContent);
                MVGroup.Children.Add(selectedLinesContent);
                MVGroup.Children.Add(multiselectedLinesContent);
            }
        }

        public void MouseLeftDown(RayMeshGeometry3DHitTestResult hitTestResult, MouseEventArgs e)
        {
            if (SelectedTool == Tool.SingleEdge)
            {
                ApplyFunctionToNearestEdge(hitTestResult, SelectVertex);
            }
        }

        public void MouseLeftUp(RayMeshGeometry3DHitTestResult hitTestResult, MouseEventArgs e)
        {
            if (SelectedTool == Tool.MultiEdge)
            {
                bool isCtrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

                foreach (Line3D lineToDraw in multiselectedLines.Values)
                {
                    if (isCtrlPressed)
                    {
                        SelectedLines.Remove(lineToDraw.LineIndex);
                        lineToDraw.UnRender(geometrySelectedLines);
                    }
                    else
                    {
                        if (SelectedLines.TryAdd(lineToDraw.LineIndex, lineToDraw))
                        {
                            lineToDraw.Render(geometrySelectedLines);
                        }
                    }
                    multiselectedLines.Remove(lineToDraw.LineIndex);
                    lineToDraw.UnRender(geometryMultiSelectedLines);
                }
            }
        }

        public void MouseMove(RayMeshGeometry3DHitTestResult hitTestResult, MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                geometryHover.Positions.Clear();
                geometryHover.TriangleIndices.Clear();

                ApplyFunctionToNearestEdge(hitTestResult, MouseMoveWhileLeftButtonDown);
            }
            else
            {
                ApplyFunctionToNearestEdge(hitTestResult, (edge) => RenderHoverLine(SelectedLines.ContainsKey(edge.LineIndex), edge));
            }
        }

        private void MouseMoveWhileLeftButtonDown(Line3D line)
        {
            if (SelectedTool == Tool.MultiEdge)
            {
                bool isCtrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

                multiselectedLinesContent.BackMaterial = new DiffuseMaterial(isCtrlPressed ? Brushes.Red : Brushes.Aqua);
                multiselectedLinesContent.Material = new DiffuseMaterial(isCtrlPressed ? Brushes.Red : Brushes.Aqua);

                if (lastEdgeSelected is null || lastEdgeSelected.LineIndex != line.LineIndex)
                {
                    if (multiselectedLines.TryAdd(line.LineIndex, line))
                    {
                        line.Render(geometryMultiSelectedLines);
                    }
                    else
                    {
                        multiselectedLines.Remove(line.LineIndex);
                        line.UnRender(geometryMultiSelectedLines);
                    }
                }
                lastEdgeSelected = line;
            }
        }


        public void ApplyFunctionToNearestEdge(RayMeshGeometry3DHitTestResult hitTestResult, Action<Line3D> act)
        {
            if (Model is not null && 
                hitTestResult.MeshHit.Positions.Count > hitTestResult.VertexIndex1 && 
                hitTestResult.MeshHit.Positions.Count > hitTestResult.VertexIndex2 &&
                hitTestResult.MeshHit.Positions.Count > hitTestResult.VertexIndex3)
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

                if (points.Count == 1) // User moused over the worst part of a line... Greaaaaat Now I have to do math...
                {
                    Line3D? line = points[0].ConnectedLines.MinBy(x => 
                        ThirdDimensionalCalculations.DistanceFromPointToLineSegment(hitTestResult.PointHit, x.StartPoint.Point, x.EndPoint.Point));

                    if (line is not null)
                    {
                        act(line);
                    }
                }
                else if (points.Count == 2) // Best case senario... User has basically given us the line for free by mousing over the correct part of the 'line'.
                {
                    for (int i = 0; i < points[0].ConnectedLines.Count; i++)
                    {
                        for (int j = 0; j < points[1].ConnectedLines.Count; j++)
                        {
                            if (points[0].ConnectedLines[i].LineIndex == points[1].ConnectedLines[j].LineIndex)
                            {
                                act(points[0].ConnectedLines[i]);
                                return;
                            }
                        }
                    }                     
                }
                else if (points.Count == 3) // User is over the inside of the triangle, so now I get to do math again. Worst case senario.
                {
                    Surface3D? triangle = null;
                    Dictionary<int, int> surfaceIndexesToOccurences = [];
                    int dictIndex, i;

                    for (i = 0; i < points[0].ConnectedSurfaces.Count; i++)
                    {
                        surfaceIndexesToOccurences.Add(points[0].ConnectedSurfaces[i].SurfaceIndex, 1);
                    }

                    for (i = 0; i < points[1].ConnectedSurfaces.Count; i++)
                    {
                        dictIndex = points[1].ConnectedSurfaces[i].SurfaceIndex;
                        if (surfaceIndexesToOccurences.ContainsKey(dictIndex))
                        {
                            surfaceIndexesToOccurences[dictIndex]++;
                        }
                    }

                    for (i = 0; i < points[2].ConnectedSurfaces.Count; i++)
                    {
                        if (surfaceIndexesToOccurences.TryGetValue(points[2].ConnectedSurfaces[i].SurfaceIndex, out int si) && si == 2)
                        {
                            triangle = points[2].ConnectedSurfaces[i];
                            if (triangle.Edge12 is not null &&  // I gotta null check the edges because they are constructed after the containing triangle because I thought it'd be
                                triangle.Edge23 is not null &&  // helpful to have bordering triangle references in the edge themselves, but I just checked and I don't even use em.
                                triangle.Edge31 is not null)
                            {
                                // use the location of where the cursor touched the triangle and get the distances from each point.
                                // The function that calculates the distances does a projection of the cursor point onto the line.
                                // I found this sucks because if there is not a perpendicular line between pt1 and the line then you
                                // have to take the nearest of the two points. The computation is somewhat expensive and annoying to debug.
                                double distToEdge1 = ThirdDimensionalCalculations.DistanceFromPointToLine(hitTestResult.PointHit, triangle.Edge12.StartPoint.Point, triangle.Edge12.EndPoint.Point);
                                double distToEdge2 = ThirdDimensionalCalculations.DistanceFromPointToLine(hitTestResult.PointHit, triangle.Edge23.StartPoint.Point, triangle.Edge23.EndPoint.Point);
                                double distToEdge3 = ThirdDimensionalCalculations.DistanceFromPointToLine(hitTestResult.PointHit, triangle.Edge31.StartPoint.Point, triangle.Edge31.EndPoint.Point);

                                //Find the closest edge.
                                if (distToEdge1 < distToEdge2 && distToEdge1 < distToEdge3)
                                {
                                    act(triangle.Edge12);
                                }
                                else if (distToEdge2 < distToEdge3 && distToEdge2 < distToEdge1)
                                {
                                    act(triangle.Edge23);
                                }
                                else
                                {
                                    act(triangle.Edge31);
                                }
                            }
                            return;
                        }
                    }
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
