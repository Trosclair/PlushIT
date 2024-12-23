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

        public Point3D CameraPosition { get => cameraPosition; set { cameraPosition = value; OnPropertyChanged(nameof(CameraPosition)); } }

        public Model3DGroup MVGroup { get; set; } = new();
        public Model3DGroup LinesGroup { get; set; } = new();
        public MainViewModel()
        {
            OpenFileDialog ofd = new();

            if (ofd.ShowDialog() is bool b && b)
            {
                Object3D obj = Object3D.Read(ofd.FileName);

                GeometryModel3D triangleContent = new();
                MeshGeometry3D geometryTriangles = new();

                GeometryModel3D lineContent = new();
                MeshGeometry3D geometryLines = new();

                int i = 0;
                int j = 0;

                foreach (Triangle3D triangle in obj.Triangles)
                {
                    geometryTriangles.Positions.Add(triangle.InnerPoint1!.Point);
                    geometryTriangles.Positions.Add(triangle.InnerPoint2!.Point);
                    geometryTriangles.Positions.Add(triangle.InnerPoint3!.Point);

                    geometryTriangles.Normals.Add(triangle.OuterPoint1.Normal);
                    geometryTriangles.Normals.Add(triangle.OuterPoint2.Normal);
                    geometryTriangles.Normals.Add(triangle.OuterPoint3.Normal);

                    geometryTriangles.TriangleIndices.Add(i);
                    geometryTriangles.TriangleIndices.Add(i + 1);
                    geometryTriangles.TriangleIndices.Add(i + 2);
                    i += 3;

                    geometryLines.Positions.Add(triangle.OuterPoint1.Point);
                    geometryLines.Positions.Add(triangle.InnerPoint1.Point);
                    geometryLines.Positions.Add(triangle.InnerPoint2.Point);

                    geometryLines.Positions.Add(triangle.OuterPoint1.Point);
                    geometryLines.Positions.Add(triangle.InnerPoint2.Point);
                    geometryLines.Positions.Add(triangle.OuterPoint2.Point);

                    geometryLines.Positions.Add(triangle.OuterPoint2.Point);
                    geometryLines.Positions.Add(triangle.InnerPoint2.Point);
                    geometryLines.Positions.Add(triangle.InnerPoint3.Point);

                    geometryLines.Positions.Add(triangle.OuterPoint2.Point);
                    geometryLines.Positions.Add(triangle.InnerPoint3.Point);
                    geometryLines.Positions.Add(triangle.OuterPoint3.Point);

                    geometryLines.Positions.Add(triangle.OuterPoint3.Point);
                    geometryLines.Positions.Add(triangle.InnerPoint3.Point);
                    geometryLines.Positions.Add(triangle.InnerPoint1.Point);

                    geometryLines.Positions.Add(triangle.OuterPoint3.Point);
                    geometryLines.Positions.Add(triangle.InnerPoint1.Point);
                    geometryLines.Positions.Add(triangle.OuterPoint1.Point);
                    
                    geometryLines.Normals.Add(triangle.OuterPoint1.Normal);
                    geometryLines.Normals.Add(triangle.InnerPoint1.Normal);
                    geometryLines.Normals.Add(triangle.InnerPoint2.Normal);
                                                            
                    geometryLines.Normals.Add(triangle.OuterPoint1.Normal);
                    geometryLines.Normals.Add(triangle.InnerPoint2.Normal);
                    geometryLines.Normals.Add(triangle.OuterPoint2.Normal);
                                                            
                    geometryLines.Normals.Add(triangle.OuterPoint2.Normal);
                    geometryLines.Normals.Add(triangle.InnerPoint2.Normal);
                    geometryLines.Normals.Add(triangle.InnerPoint3.Normal);
                                                           
                    geometryLines.Normals.Add(triangle.OuterPoint2.Normal);
                    geometryLines.Normals.Add(triangle.InnerPoint3.Normal);
                    geometryLines.Normals.Add(triangle.OuterPoint3.Normal);
                                                            
                    geometryLines.Normals.Add(triangle.OuterPoint3.Normal);
                    geometryLines.Normals.Add(triangle.InnerPoint3.Normal);
                    geometryLines.Normals.Add(triangle.InnerPoint1.Normal);
                                                            
                    geometryLines.Normals.Add(triangle.OuterPoint3.Normal);
                    geometryLines.Normals.Add(triangle.InnerPoint1.Normal);
                    geometryLines.Normals.Add(triangle.OuterPoint1.Normal);

                    for (int k = 0; k < 18; k++)
                    {
                        geometryLines.TriangleIndices.Add(k + j);
                    }
                    j += 18;
                }


                triangleContent.BackMaterial = new DiffuseMaterial(Brushes.Gray);
                triangleContent.Material = new DiffuseMaterial(Brushes.Gray);
                triangleContent.Geometry = geometryTriangles;

                lineContent.BackMaterial = new DiffuseMaterial(Brushes.Black);
                lineContent.Material = new DiffuseMaterial(Brushes.Black);
                lineContent.Geometry = geometryLines;

                MVGroup.Children.Add(triangleContent);
                LinesGroup.Children.Add(lineContent);
            }
        }

        public void ChangeCameraXY(double x, double y)
        {
            CameraPosition = new(cameraPosition.X + (float)x, cameraPosition.Y + (float)y, cameraPosition.Z);
        }

    }
}
