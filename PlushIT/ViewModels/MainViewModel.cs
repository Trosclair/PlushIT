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
                    geometryTriangles.Positions.Add(triangle.Point1.Point);
                    geometryTriangles.Positions.Add(triangle.Point2.Point);
                    geometryTriangles.Positions.Add(triangle.Point3.Point);

                    geometryTriangles.Normals.Add(triangle.Point1.Normal);
                    geometryTriangles.Normals.Add(triangle.Point2.Normal);
                    geometryTriangles.Normals.Add(triangle.Point3.Normal);

                    geometryTriangles.TriangleIndices.Add(i);
                    geometryTriangles.TriangleIndices.Add(i + 1);
                    geometryTriangles.TriangleIndices.Add(i + 2);
                    i += 3;

                    NormalPoint3D[] pts = triangle.Point1ToPoint2!.GetPoints();

                    geometryLines.Positions.Add(triangle.Point1ToPoint2!.Point1.Point);
                    geometryLines.Positions.Add(triangle.Point1ToPoint2!.Point3.Point);
                    geometryLines.Positions.Add(triangle.Point1ToPoint2!.Point4.Point);

                    geometryLines.Positions.Add(triangle.Point1ToPoint2!.Point2.Point);
                    geometryLines.Positions.Add(triangle.Point1ToPoint2!.Point4.Point);
                    geometryLines.Positions.Add(triangle.Point1ToPoint2!.Point1.Point);

                    geometryLines.Positions.Add(triangle.Point1ToPoint2!.Point2.Point);
                    geometryLines.Positions.Add(triangle.Point1ToPoint2!.Point5.Point);
                    geometryLines.Positions.Add(triangle.Point1ToPoint2!.Point6.Point);

                    geometryLines.Positions.Add(triangle.Point1ToPoint2!.Point1.Point);
                    geometryLines.Positions.Add(triangle.Point1ToPoint2!.Point2.Point);
                    geometryLines.Positions.Add(triangle.Point1ToPoint2!.Point5.Point);

                    geometryLines.Normals.Add(triangle.Point1ToPoint2!.Point1.Normal);
                    geometryLines.Normals.Add(triangle.Point1ToPoint2!.Point3.Normal);
                    geometryLines.Normals.Add(triangle.Point1ToPoint2!.Point4.Normal);

                    geometryLines.Normals.Add(triangle.Point1ToPoint2!.Point2.Normal);
                    geometryLines.Normals.Add(triangle.Point1ToPoint2!.Point4.Normal);
                    geometryLines.Normals.Add(triangle.Point1ToPoint2!.Point1.Normal);

                    geometryLines.Normals.Add(triangle.Point1ToPoint2!.Point2.Normal);
                    geometryLines.Normals.Add(triangle.Point1ToPoint2!.Point5.Normal);
                    geometryLines.Normals.Add(triangle.Point1ToPoint2!.Point6.Normal);

                    geometryLines.Normals.Add(triangle.Point1ToPoint2!.Point1.Normal);
                    geometryLines.Normals.Add(triangle.Point1ToPoint2!.Point2.Normal);
                    geometryLines.Normals.Add(triangle.Point1ToPoint2!.Point5.Normal);

                    for (int k = 0; k < 12; k++)
                    {
                        geometryLines.TriangleIndices.Add(k + j);
                    }
                    j += 12;

                    geometryLines.Positions.Add(triangle.Point2ToPoint3!.Point1.Point);
                    geometryLines.Positions.Add(triangle.Point2ToPoint3!.Point3.Point);
                    geometryLines.Positions.Add(triangle.Point2ToPoint3!.Point4.Point);

                    geometryLines.Positions.Add(triangle.Point2ToPoint3!.Point2.Point);
                    geometryLines.Positions.Add(triangle.Point2ToPoint3!.Point4.Point);
                    geometryLines.Positions.Add(triangle.Point2ToPoint3!.Point1.Point);

                    geometryLines.Positions.Add(triangle.Point2ToPoint3!.Point2.Point);
                    geometryLines.Positions.Add(triangle.Point2ToPoint3!.Point5.Point);
                    geometryLines.Positions.Add(triangle.Point2ToPoint3!.Point6.Point);

                    geometryLines.Positions.Add(triangle.Point2ToPoint3!.Point1.Point);
                    geometryLines.Positions.Add(triangle.Point2ToPoint3!.Point2.Point);
                    geometryLines.Positions.Add(triangle.Point2ToPoint3!.Point5.Point);

                    geometryLines.Normals.Add(triangle.Point2ToPoint3!.Point1.Normal);
                    geometryLines.Normals.Add(triangle.Point2ToPoint3!.Point3.Normal);
                    geometryLines.Normals.Add(triangle.Point2ToPoint3!.Point4.Normal);

                    geometryLines.Normals.Add(triangle.Point2ToPoint3!.Point2.Normal);
                    geometryLines.Normals.Add(triangle.Point2ToPoint3!.Point4.Normal);
                    geometryLines.Normals.Add(triangle.Point2ToPoint3!.Point1.Normal);

                    geometryLines.Normals.Add(triangle.Point2ToPoint3!.Point2.Normal);
                    geometryLines.Normals.Add(triangle.Point2ToPoint3!.Point5.Normal);
                    geometryLines.Normals.Add(triangle.Point2ToPoint3!.Point6.Normal);

                    geometryLines.Normals.Add(triangle.Point2ToPoint3!.Point1.Normal);
                    geometryLines.Normals.Add(triangle.Point2ToPoint3!.Point2.Normal);
                    geometryLines.Normals.Add(triangle.Point2ToPoint3!.Point5.Normal);

                    for (int k = 0; k < 12; k++)
                    {
                        geometryLines.TriangleIndices.Add(k + j);
                    }
                    j += 12;

                    geometryLines.Positions.Add(triangle.Point3ToPoint1!.Point1.Point);
                    geometryLines.Positions.Add(triangle.Point3ToPoint1!.Point3.Point);
                    geometryLines.Positions.Add(triangle.Point3ToPoint1!.Point4.Point);

                    geometryLines.Positions.Add(triangle.Point3ToPoint1!.Point2.Point);
                    geometryLines.Positions.Add(triangle.Point3ToPoint1!.Point4.Point);
                    geometryLines.Positions.Add(triangle.Point3ToPoint1!.Point1.Point);

                    geometryLines.Positions.Add(triangle.Point3ToPoint1!.Point2.Point);
                    geometryLines.Positions.Add(triangle.Point3ToPoint1!.Point6.Point);
                    geometryLines.Positions.Add(triangle.Point3ToPoint1!.Point5.Point);

                    geometryLines.Positions.Add(triangle.Point3ToPoint1!.Point1.Point);
                    geometryLines.Positions.Add(triangle.Point3ToPoint1!.Point2.Point);
                    geometryLines.Positions.Add(triangle.Point3ToPoint1!.Point6.Point);

                    geometryLines.Normals.Add(triangle.Point3ToPoint1!.Point1.Normal);
                    geometryLines.Normals.Add(triangle.Point3ToPoint1!.Point3.Normal);
                    geometryLines.Normals.Add(triangle.Point1ToPoint2!.Point4.Normal);

                    geometryLines.Normals.Add(triangle.Point3ToPoint1!.Point2.Normal);
                    geometryLines.Normals.Add(triangle.Point3ToPoint1!.Point4.Normal);
                    geometryLines.Normals.Add(triangle.Point3ToPoint1!.Point1.Normal);

                    geometryLines.Normals.Add(triangle.Point3ToPoint1!.Point2.Normal);
                    geometryLines.Normals.Add(triangle.Point3ToPoint1!.Point6.Normal);
                    geometryLines.Normals.Add(triangle.Point3ToPoint1!.Point5.Normal);

                    geometryLines.Normals.Add(triangle.Point3ToPoint1!.Point1.Normal);
                    geometryLines.Normals.Add(triangle.Point3ToPoint1!.Point2.Normal);
                    geometryLines.Normals.Add(triangle.Point3ToPoint1!.Point6.Normal);

                    for (int k = 0; k < 12; k++)
                    {
                        geometryLines.TriangleIndices.Add(k + j);
                    }
                    j += 12;

                    //triangle.Point2ToPoint3!.GetPoints();
                    //for (int k = 0; k < 6; k++)
                    //{
                    //    geometryLines.Positions.Add(pts[k].Point);
                    //    geometryLines.Normals.Add(pts[k].Normal);
                    //    geometryLines.TriangleIndices.Add(Hex3D.TriangleIndicies[k] + j);
                    //}
                    //j += 6;

                    //triangle.Point3ToPoint1!.GetPoints();
                    //for (int k = 0; k < 6; k++)
                    //{
                    //    geometryLines.Positions.Add(pts[k].Point);
                    //    geometryLines.Normals.Add(pts[k].Normal);
                    //    geometryLines.TriangleIndices.Add(Hex3D.TriangleIndicies[k] + j);
                    //}
                    //j += 6;
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
