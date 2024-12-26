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
        private Tool selectedTool = Tool.Pen;
        private readonly Dictionary<int, Line3D> selectedLines = [];
        private readonly List<IndexPoint3D> pointsCurrentlyBeingSelected = [];
        private readonly Dictionary<int, Line3D> linesCurrentlyBeingSelected = [];

        private readonly GeometryModel3D triangleContent = new();
        private readonly GeometryModel3D lineContent = new();
        private readonly GeometryModel3D hoverEdgeContent = new();
        private readonly GeometryModel3D hoverPointContent = new();
        private readonly GeometryModel3D selectedLinesContent = new();
        private readonly GeometryModel3D multiSelectedLinesContent = new();
        private readonly GeometryModel3D multiSelectedPointsContent = new();

        private readonly MeshGeometry3D geometryTriangles = new();
        private readonly MeshGeometry3D geometryLines = new();
        private readonly MeshGeometry3D geometryEdgeHover = new();
        private readonly MeshGeometry3D geometryPointHover = new();
        private readonly MeshGeometry3D geometrySelectedLines = new();
        private readonly MeshGeometry3D geometryMultiSelectedLines = new();
        private readonly MeshGeometry3D geometryMultiSelectedPoints = new();

        private Line3D? lastEdgeSelected = null;

        public Point3D CameraPosition { get => cameraPosition; set { cameraPosition = value; OnPropertyChanged(nameof(CameraPosition)); } }
        public Tool SelectedTool { get => selectedTool; set { selectedTool = value; OnPropertyChanged(nameof(SelectedTool)); } }

        public Model3DGroup MVGroup { get; set; } = new();
        public OBJModel3D? Model { get; set; }


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

                selectedLinesContent.BackMaterial = new DiffuseMaterial(Brushes.LightGreen);
                selectedLinesContent.Material = new DiffuseMaterial(Brushes.LightGreen);
                selectedLinesContent.Geometry = geometrySelectedLines;

                multiSelectedLinesContent.Geometry = geometryMultiSelectedLines;
                multiSelectedPointsContent.Geometry = geometryMultiSelectedPoints;

                hoverEdgeContent.Geometry = geometryEdgeHover;
                hoverPointContent.Geometry = geometryPointHover;

                MVGroup.Children.Add(lineContent);
                MVGroup.Children.Add(triangleContent);
                MVGroup.Children.Add(hoverEdgeContent);
                MVGroup.Children.Add(hoverPointContent);
                MVGroup.Children.Add(selectedLinesContent);
                MVGroup.Children.Add(multiSelectedLinesContent);
                MVGroup.Children.Add(multiSelectedPointsContent);
            }
        }

        public void MouseLeftDown(RayMeshGeometry3DHitTestResult hitTestResult, MouseEventArgs e)
        {
            if (SelectedTool == Tool.SingleEdge)
            {
                ApplyFunctionToNearestEdge(hitTestResult, SelectEdge);
            }
            else if (SelectedTool == Tool.Pen)
            {
                multiSelectedLinesContent.BackMaterial = new DiffuseMaterial(Brushes.Aqua);
                multiSelectedLinesContent.Material = new DiffuseMaterial(Brushes.Aqua);
                ApplyFunctionToNearestPoint(hitTestResult, SelectPoint);
            }
        }

        public void MouseLeftUp(RayMeshGeometry3DHitTestResult hitTestResult, MouseEventArgs e)
        {
            bool isCtrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            if (SelectedTool == Tool.MultiEdge)
            {
                foreach (Line3D lineToDraw in linesCurrentlyBeingSelected.Values)
                {
                    if (isCtrlPressed)
                    {
                        selectedLines.Remove(lineToDraw.LineIndex);
                        lineToDraw.UnRender(geometrySelectedLines);
                    }
                    else
                    {
                        if (selectedLines.TryAdd(lineToDraw.LineIndex, lineToDraw))
                        {
                            lineToDraw.Render(geometrySelectedLines);
                        }
                    }
                    linesCurrentlyBeingSelected.Remove(lineToDraw.LineIndex);
                    lineToDraw.UnRender(geometryMultiSelectedLines);
                }
            }
        }

        public void MouseMove(RayMeshGeometry3DHitTestResult hitTestResult, MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                geometryEdgeHover.Positions.Clear();
                geometryEdgeHover.TriangleIndices.Clear();

                ApplyFunctionToNearestEdge(hitTestResult, MouseMoveWhileLeftButtonDown);
            }
            else
            {   
                if (SelectedTool == Tool.Pen)
                {
                    ApplyFunctionToNearestPoint(hitTestResult, RenderPenHoverPoint);
                }
                else
                {
                    ApplyFunctionToNearestEdge(hitTestResult, (edge) => RenderHoverLine(selectedLines.ContainsKey(edge.LineIndex), edge));
                }
            }
        }

        private void MouseMoveWhileLeftButtonDown(Line3D line)
        {
            if (SelectedTool == Tool.MultiEdge)
            {
                bool isCtrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

                multiSelectedLinesContent.BackMaterial = new DiffuseMaterial(isCtrlPressed ? Brushes.Red : Brushes.Aqua);
                multiSelectedLinesContent.Material = new DiffuseMaterial(isCtrlPressed ? Brushes.Red : Brushes.Aqua);

                if (lastEdgeSelected is null || lastEdgeSelected.LineIndex != line.LineIndex)
                {
                    if (linesCurrentlyBeingSelected.TryAdd(line.LineIndex, line))
                    {
                        line.Render(geometryMultiSelectedLines);
                    }
                    else
                    {
                        linesCurrentlyBeingSelected.Remove(line.LineIndex);
                        line.UnRender(geometryMultiSelectedLines);
                    }
                }
                lastEdgeSelected = line;
            }
        }


        private void ApplyFunctionToNearestEdge(RayMeshGeometry3DHitTestResult hitTestResult, Action<Line3D> act)
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
                    Surface3D triangle;
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

        private void ApplyFunctionToNearestPoint(RayMeshGeometry3DHitTestResult hitTestResult, Action<IndexPoint3D> act)
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

                if (points.Count == 1)
                {
                    double pt1Dist = ThirdDimensionalCalculations.DistanceBetweenPoints(points[0].Point, hitTestResult.PointHit);
                    foreach (Line3D line in points[0].ConnectedLines)
                    {
                        if (line.StartPoint.OuterPositionNumber == points[0].OuterPositionNumber)
                        {
                            if (pt1Dist > ThirdDimensionalCalculations.DistanceBetweenPoints(hitTestResult.PointHit, line.EndPoint.Point))
                            {
                                act(line.EndPoint);
                                return;
                            }
                        }
                        else
                        {
                            if (pt1Dist > ThirdDimensionalCalculations.DistanceBetweenPoints(hitTestResult.PointHit, line.StartPoint.Point))
                            {
                                act(line.StartPoint);
                                return;
                            }
                        }
                    }

                    act(points[0]);
                    return;
                }
                else if (points.Count == 2)
                {
                    if (ThirdDimensionalCalculations.DistanceBetweenPoints(points[0].Point, hitTestResult.PointHit) < 
                        ThirdDimensionalCalculations.DistanceBetweenPoints(points[1].Point, hitTestResult.PointHit))
                    {
                        act(points[0]);
                    }
                    else
                    {
                        act(points[1]);
                    }
                    return;
                }
                else if (points.Count == 3)
                {
                    double dist1 = ThirdDimensionalCalculations.DistanceBetweenPoints(points[0].Point, hitTestResult.PointHit);
                    double dist2 = ThirdDimensionalCalculations.DistanceBetweenPoints(points[1].Point, hitTestResult.PointHit);
                    double dist3 = ThirdDimensionalCalculations.DistanceBetweenPoints(points[2].Point, hitTestResult.PointHit);

                    if (dist1 < dist2 && dist1 < dist3)
                    {
                        act(points[0]);
                        return;
                    }
                    else if (dist2 < dist3 && dist2 < dist1)
                    {
                        act(points[1]);
                        return;
                    }
                    else
                    {
                        act(points[2]);
                        return;
                    }
                }
            }
        }

        private void SelectPoint(IndexPoint3D point)
        {
            if (pointsCurrentlyBeingSelected.Count > 1 && pointsCurrentlyBeingSelected.First().OuterPositionNumber == point.OuterPositionNumber)
            {
                List<Line3D> path = FindShortestPathBetweenTwoPoints(pointsCurrentlyBeingSelected.Last(), point);

                foreach (Line3D line in path)
                {
                    linesCurrentlyBeingSelected.Add(line.LineIndex, line);
                    line.Render(geometryMultiSelectedLines);
                }

                foreach (Line3D line in linesCurrentlyBeingSelected.Values)
                {
                    line.UnRender(geometryMultiSelectedLines);
                    if (selectedLines.TryAdd(line.LineIndex, line))
                    {
                        line.Render(geometrySelectedLines);
                    }
                }

                linesCurrentlyBeingSelected.Clear();
                pointsCurrentlyBeingSelected.Clear();
                geometryMultiSelectedPoints.Positions.Clear();
                geometryMultiSelectedPoints.TextureCoordinates.Clear();
                RenderHoverPoint(false, point);
            }
            else if (pointsCurrentlyBeingSelected.Count > 0 && pointsCurrentlyBeingSelected.Last().OuterPositionNumber == point.OuterPositionNumber)
            {
                pointsCurrentlyBeingSelected.Remove(point);
                point.UnRender(geometryMultiSelectedPoints);
                RenderHoverPoint(false, point);
            }
            else
            {
                if (pointsCurrentlyBeingSelected.Count > 0)
                {
                    if (!point.ConnectedLines.Any(x => linesCurrentlyBeingSelected.ContainsKey(x.LineIndex)))
                    {
                        List<Line3D> path = FindShortestPathBetweenTwoPoints(pointsCurrentlyBeingSelected.Last(), point);

                        foreach (Line3D line in path)
                        {
                            linesCurrentlyBeingSelected.Add(line.LineIndex, line);
                            line.Render(geometryMultiSelectedLines);
                        }
                    }
                }
                pointsCurrentlyBeingSelected.Add(point);
                point.Render(geometryMultiSelectedPoints);
                RenderHoverPoint(true, point);
            }
        }

        private List<Line3D> FindShortestPathBetweenTwoPoints(IndexPoint3D start, IndexPoint3D end)
        {
            int i = 0;
            List<(double, Line3D, IndexPoint3D, int)> linesAndWeights = [];
            IndexPoint3D candidatePoint;
            foreach (Line3D line in start.ConnectedLines)
            {
                if (linesCurrentlyBeingSelected.ContainsKey(line.LineIndex))
                {
                    continue;
                }

                candidatePoint = line.StartPoint.OuterPositionNumber == start.OuterPositionNumber ? line.EndPoint : line.StartPoint;

                double distance = ThirdDimensionalCalculations.DistanceBetweenPoints(candidatePoint.Point, end.Point);

                if (distance == 0D)
                {
                    return [line];
                }

                if (!candidatePoint.ConnectedLines.Any(x => linesCurrentlyBeingSelected.ContainsKey(x.LineIndex)))
                {
                    linesAndWeights.Add(new(distance, line, candidatePoint, i++));
                }
            }

            (double, List<Line3D>, Dictionary<int, Line3D>)[] ret = new (double, List<Line3D>, Dictionary<int, Line3D>)[linesAndWeights.Count];
            Dictionary<int, Line3D>[] arrDict = new Dictionary<int, Line3D>[ret.Length]; 
            for (i = 0; i < ret.Length; i++)
            {
                arrDict[i] = new(linesCurrentlyBeingSelected);
            }

            Parallel.ForEach(linesAndWeights, tuple =>
            {
                ret[tuple.Item4] = FindShortestPathBetweenTwoPoints(tuple.Item3, end, (tuple.Item1, [tuple.Item2], arrDict[tuple.Item4]), new(-1D, [], []));
            });

            return ret.Where(x => x.Item1 > 0).MinBy(x => x.Item1).Item2;
        }

        private static (double, List<Line3D>, Dictionary<int, Line3D>) FindShortestPathBetweenTwoPoints(IndexPoint3D start, IndexPoint3D end, (double, List<Line3D>, Dictionary<int, Line3D>) bag, (double, List<Line3D>, Dictionary<int, Line3D>) currentRecord)
        {
            if (currentRecord.Item1 != -1D && currentRecord.Item1 <= bag.Item1)
            {
                return currentRecord;
            }

            if (start.ConnectedLines.Count == 1)
            {
                return currentRecord;
            }

            List<(double, Line3D, IndexPoint3D)> linesAndWeights = [];
            IndexPoint3D candidatePoint;
            foreach (Line3D line in start.ConnectedLines)
            {
                if (bag.Item2.Contains(line) || bag.Item3.ContainsKey(line.LineIndex))
                {
                    continue;
                }

                candidatePoint = line.StartPoint.OuterPositionNumber == start.OuterPositionNumber ? line.EndPoint : line.StartPoint;

                double distance = ThirdDimensionalCalculations.DistanceBetweenPoints(candidatePoint.Point, end.Point);

                if (distance == 0D)
                {
                    bag.Item1 += line.Length;
                    bag.Item2.Add(line);
                    return bag;
                }

                if (!candidatePoint.ConnectedLines.Any(x => bag.Item3.ContainsKey(x.LineIndex)))
                {
                    linesAndWeights.Add(new(distance, line, candidatePoint));
                }
            }

            (double, List<Line3D>, Dictionary<int, Line3D>) ret = currentRecord;
            (double, List<Line3D>, Dictionary<int, Line3D>) compare;
            foreach ((double, Line3D, IndexPoint3D) tuple in linesAndWeights.OrderBy(x => x.Item1))
            {
                bag.Item1 += tuple.Item2.Length;
                bag.Item2 = new(bag.Item2) { tuple.Item2 };

                compare = FindShortestPathBetweenTwoPoints(tuple.Item3, end, bag, ret);

                if (ret.Item1 == -1D)
                {
                    ret = compare;
                }
                else
                {
                    if (ret.Item1 > compare.Item1)
                    {
                        ret = compare;
                    }
                    else
                    {
                        return ret;
                    }
                }
            }

            return ret;
        }

        private void RenderPenHoverPoint(IndexPoint3D point)
        {
            if ((pointsCurrentlyBeingSelected.Count > 0 && pointsCurrentlyBeingSelected.First().OuterPositionNumber == point.OuterPositionNumber) || 
                !point.ConnectedLines.Any(x => linesCurrentlyBeingSelected.ContainsKey(x.LineIndex)))
            {
                RenderHoverPoint(pointsCurrentlyBeingSelected.Contains(point), point);
            }
        }

        private void SelectEdge(Line3D edge)
        {
            if (selectedLines.TryAdd(edge.LineIndex, edge))
            {
                edge.Render(geometrySelectedLines);
                RenderHoverLine(true, edge);
            }
            else
            {
                selectedLines.Remove(edge.LineIndex);
                edge.UnRender(geometrySelectedLines);
                RenderHoverLine(false, edge);
            }
        }

        private void RenderHoverLine(bool isSelected, Line3D edge)
        {
            hoverEdgeContent.BackMaterial = new DiffuseMaterial(isSelected ? Brushes.Red : Brushes.Yellow);
            hoverEdgeContent.Material = new DiffuseMaterial(isSelected ? Brushes.Red : Brushes.Yellow);
            edge.RenderHover(geometryEdgeHover);
        }

        private void RenderHoverPoint(bool isSelected, IndexPoint3D point)
        {
            bool hasCompletedARing = pointsCurrentlyBeingSelected.Count > 1 && point.OuterPositionNumber == pointsCurrentlyBeingSelected[0].OuterPositionNumber;
            Brush pointsCurrentlySelectedBrush = hasCompletedARing ? Brushes.LightGreen : Brushes.Aqua;
            Brush hoverBrush = isSelected ? (hasCompletedARing ? Brushes.LightGreen : Brushes.Red) : Brushes.Yellow;

            multiSelectedPointsContent.BackMaterial = new DiffuseMaterial(pointsCurrentlySelectedBrush);
            multiSelectedPointsContent.Material = new DiffuseMaterial(pointsCurrentlySelectedBrush);

            hoverPointContent.BackMaterial = new DiffuseMaterial(hoverBrush);
            hoverPointContent.Material = new DiffuseMaterial(hoverBrush);
            point.RenderHover(geometryPointHover);
        }

        public void ChangeCameraXY(double x, double y)
        {
            CameraPosition = new(cameraPosition.X + (float)x, cameraPosition.Y + (float)y, cameraPosition.Z);
        }
    }
}
