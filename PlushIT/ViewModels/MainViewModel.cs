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
                Object3D obj = Object3D.Read(ofd.FileName);

                int innerPositionIndex = 0;
                int outerPositionIndex = 0;
                int innerNormalIndex = 0;
                int outerNormalIndex = 0;

                foreach (Triangle3D triangle in obj.Triangles)
                {
                    if (triangle.InnerPoint1.InnerPositionNumber == -1)
                    {
                        triangle.InnerPoint1.InnerPositionNumber = innerPositionIndex++;
                        geometryTriangles.Positions.Add(triangle.InnerPoint1.Point);

                        triangle.InnerPoint1.OuterPositionNumber = outerPositionIndex++;
                        geometryLines.Positions.Add(triangle.InnerPoint1.Point);
                    }
                    if (triangle.InnerPoint2.InnerPositionNumber == -1)
                    {
                        triangle.InnerPoint2.InnerPositionNumber = innerPositionIndex++;
                        geometryTriangles.Positions.Add(triangle.InnerPoint2.Point);

                        triangle.InnerPoint2.OuterPositionNumber = outerPositionIndex++;
                        geometryLines.Positions.Add(triangle.InnerPoint2.Point);
                    }
                    if (triangle.InnerPoint3.InnerPositionNumber == -1)
                    {
                        triangle.InnerPoint3.InnerPositionNumber = innerPositionIndex++;
                        geometryTriangles.Positions.Add(triangle.InnerPoint3.Point);

                        triangle.InnerPoint3.OuterPositionNumber = outerPositionIndex++;
                        geometryLines.Positions.Add(triangle.InnerPoint3.Point);
                    }
                    if (triangle.OuterPoint1.OuterPositionNumber == -1)
                    {
                        triangle.OuterPoint1.OuterPositionNumber = outerPositionIndex++;
                        geometryLines.Positions.Add(triangle.InnerPoint1.Point);
                    }
                    if (triangle.OuterPoint2.OuterPositionNumber == -1)
                    {
                        triangle.OuterPoint2.OuterPositionNumber = outerPositionIndex++;
                        geometryLines.Positions.Add(triangle.InnerPoint2.Point);
                    }
                    if (triangle.OuterPoint3.OuterPositionNumber == -1)
                    {
                        triangle.OuterPoint3.OuterPositionNumber = outerPositionIndex++;
                        geometryLines.Positions.Add(triangle.InnerPoint3.Point);
                    }

                    geometryTriangles.TriangleIndices.Add(triangle.InnerPoint1.InnerPositionNumber);
                    geometryTriangles.TriangleIndices.Add(triangle.InnerPoint2.InnerPositionNumber);
                    geometryTriangles.TriangleIndices.Add(triangle.InnerPoint3.InnerPositionNumber);
                                                                           
                    geometryLines.TriangleIndices.Add(triangle.OuterPoint1.OuterPositionNumber);
                    geometryLines.TriangleIndices.Add(triangle.InnerPoint1.OuterPositionNumber);
                    geometryLines.TriangleIndices.Add(triangle.InnerPoint2.OuterPositionNumber);
                                                                           
                    geometryLines.TriangleIndices.Add(triangle.OuterPoint1.OuterPositionNumber);
                    geometryLines.TriangleIndices.Add(triangle.InnerPoint2.OuterPositionNumber);
                    geometryLines.TriangleIndices.Add(triangle.OuterPoint2.OuterPositionNumber);
                                                                           
                    geometryLines.TriangleIndices.Add(triangle.OuterPoint2.OuterPositionNumber);
                    geometryLines.TriangleIndices.Add(triangle.InnerPoint2.OuterPositionNumber);
                    geometryLines.TriangleIndices.Add(triangle.InnerPoint3.OuterPositionNumber);
                                                                           
                    geometryLines.TriangleIndices.Add(triangle.OuterPoint2.OuterPositionNumber);
                    geometryLines.TriangleIndices.Add(triangle.InnerPoint3.OuterPositionNumber);
                    geometryLines.TriangleIndices.Add(triangle.OuterPoint3.OuterPositionNumber);
                                                                           
                    geometryLines.TriangleIndices.Add(triangle.OuterPoint3.OuterPositionNumber);
                    geometryLines.TriangleIndices.Add(triangle.InnerPoint3.OuterPositionNumber);
                    geometryLines.TriangleIndices.Add(triangle.InnerPoint1.OuterPositionNumber);
                                                                           
                    geometryLines.TriangleIndices.Add(triangle.OuterPoint3.OuterPositionNumber);
                    geometryLines.TriangleIndices.Add(triangle.InnerPoint1.OuterPositionNumber);
                    geometryLines.TriangleIndices.Add(triangle.OuterPoint1.OuterPositionNumber);
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
