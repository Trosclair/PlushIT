using HelixToolkit.Wpf;
using Microsoft.Win32;
using PlushIT.Models;
using PlushIT.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
        public MainViewModel()
        {
            OpenFileDialog ofd = new();

            if (ofd.ShowDialog() is bool b && b)
            {
                Models.Model3D obj = Models.Model3D.Read(ofd.FileName);

                int innerPositionIndex = 0;
                int outerPositionIndex = 0;

                foreach (Surface3D triangle in obj.Surfaces)
                {
                    if (triangle.InnerTriangle is not null)
                    {
                        if (triangle.InnerTriangle.Point1.InnerPositionNumber == -1)
                        {
                            triangle.InnerTriangle.Point1.InnerPositionNumber = innerPositionIndex++;
                            geometryTriangles.Positions.Add(triangle.InnerTriangle.Point1.Point);

                            triangle.InnerTriangle.Point1.OuterPositionNumber = outerPositionIndex++;
                            geometryLines.Positions.Add(triangle.InnerTriangle.Point1.Point);
                        }
                        if (triangle.InnerTriangle.Point2.InnerPositionNumber == -1)
                        {
                            triangle.InnerTriangle.Point2.InnerPositionNumber = innerPositionIndex++;
                            geometryTriangles.Positions.Add(triangle.InnerTriangle.Point2.Point);

                            triangle.InnerTriangle.Point2.OuterPositionNumber = outerPositionIndex++;
                            geometryLines.Positions.Add(triangle.InnerTriangle.Point2.Point);
                        }
                        if (triangle.InnerTriangle.Point3.InnerPositionNumber == -1)
                        {
                            triangle.InnerTriangle.Point3.InnerPositionNumber = innerPositionIndex++;
                            geometryTriangles.Positions.Add(triangle.InnerTriangle.Point3.Point);

                            triangle.InnerTriangle.Point3.OuterPositionNumber = outerPositionIndex++;
                            geometryLines.Positions.Add(triangle.InnerTriangle.Point3.Point);
                        }
                        if (triangle.OuterTriangle.Point1.OuterPositionNumber == -1)
                        {
                            triangle.OuterTriangle.Point1.OuterPositionNumber = outerPositionIndex++;
                            geometryLines.Positions.Add(triangle.OuterTriangle.Point1.Point);
                        }
                        if (triangle.OuterTriangle.Point2.OuterPositionNumber == -1)
                        {
                            triangle.OuterTriangle.Point2.OuterPositionNumber = outerPositionIndex++;
                            geometryLines.Positions.Add(triangle.OuterTriangle.Point2.Point);
                        }
                        if (triangle.OuterTriangle.Point3.OuterPositionNumber == -1)
                        {
                            triangle.OuterTriangle.Point3.OuterPositionNumber = outerPositionIndex++;
                            geometryLines.Positions.Add(triangle.OuterTriangle.Point3.Point);
                        }

                        geometryTriangles.TriangleIndices.Add(triangle.InnerTriangle.Point1.InnerPositionNumber);
                        geometryTriangles.TriangleIndices.Add(triangle.InnerTriangle.Point2.InnerPositionNumber);
                        geometryTriangles.TriangleIndices.Add(triangle.InnerTriangle.Point3.InnerPositionNumber);

                        geometryLines.TriangleIndices.Add(triangle.OuterTriangle.Point1.OuterPositionNumber);
                        geometryLines.TriangleIndices.Add(triangle.InnerTriangle.Point1.OuterPositionNumber);
                        geometryLines.TriangleIndices.Add(triangle.InnerTriangle.Point2.OuterPositionNumber);

                        geometryLines.TriangleIndices.Add(triangle.OuterTriangle.Point1.OuterPositionNumber);
                        geometryLines.TriangleIndices.Add(triangle.InnerTriangle.Point2.OuterPositionNumber);
                        geometryLines.TriangleIndices.Add(triangle.OuterTriangle.Point2.OuterPositionNumber);

                        geometryLines.TriangleIndices.Add(triangle.OuterTriangle.Point2.OuterPositionNumber);
                        geometryLines.TriangleIndices.Add(triangle.InnerTriangle.Point2.OuterPositionNumber);
                        geometryLines.TriangleIndices.Add(triangle.InnerTriangle.Point3.OuterPositionNumber);

                        geometryLines.TriangleIndices.Add(triangle.OuterTriangle.Point2.OuterPositionNumber);
                        geometryLines.TriangleIndices.Add(triangle.InnerTriangle.Point3.OuterPositionNumber);
                        geometryLines.TriangleIndices.Add(triangle.OuterTriangle.Point3.OuterPositionNumber);

                        geometryLines.TriangleIndices.Add(triangle.OuterTriangle.Point3.OuterPositionNumber);
                        geometryLines.TriangleIndices.Add(triangle.InnerTriangle.Point3.OuterPositionNumber);
                        geometryLines.TriangleIndices.Add(triangle.InnerTriangle.Point1.OuterPositionNumber);

                        geometryLines.TriangleIndices.Add(triangle.OuterTriangle.Point3.OuterPositionNumber);
                        geometryLines.TriangleIndices.Add(triangle.InnerTriangle.Point1.OuterPositionNumber);
                        geometryLines.TriangleIndices.Add(triangle.OuterTriangle.Point1.OuterPositionNumber);
                    }
                }

                triangleContent.BackMaterial = new DiffuseMaterial(Brushes.Gray);
                triangleContent.Material = new DiffuseMaterial(Brushes.Gray);
                triangleContent.Geometry = geometryTriangles;

                lineContent.BackMaterial = new DiffuseMaterial(Brushes.Black);
                lineContent.Material = new DiffuseMaterial(Brushes.Black);
                lineContent.Geometry = geometryLines;

                MVGroup.Children.Add(triangleContent);
                MVGroup.Children.Add(lineContent);
            }
        }

        public void HighlightVertexFromHitTest(RayMeshGeometry3DHitTestResult hitTestResult)
        {
            if (hitTestResult.MeshHit.Positions.Count == geometryLines.Positions.Count)
            {

            }
            else if (hitTestResult.MeshHit.Positions.Count == geometryTriangles.Positions.Count)
            {

            }
            else if (hitTestResult.MeshHit.Positions.Count == geometryHover.Positions.Count)
            {

            }
        }

        public void ChangeCameraXY(double x, double y)
        {
            CameraPosition = new(cameraPosition.X + (float)x, cameraPosition.Y + (float)y, cameraPosition.Z);
        }

    }
}
