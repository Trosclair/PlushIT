using PlushIT.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace PlushIT.Models
{
    public class Hex3D
    {
        private Triangle3D? side1, side2;

        public static readonly int[] TriangleIndicies = [1, 3, 4, 2, 5, 6];

        public NormalPoint3D Point1 { get; set; }
        public NormalPoint3D Point2 { get; set; }
        public NormalPoint3D? Point3 { get; set; }
        public NormalPoint3D? Point4 { get; set; }
        public NormalPoint3D? Point5 { get; set; }
        public NormalPoint3D? Point6 { get; set; }

        public Triangle3D? Side1 { get => side1; set { side1 = value; } }
        public Triangle3D? Side2 { get => side2; set { side2 = value; } }

        public double Thickness { get; set; }
        public double Length { get; set; }

        public Hex3D(NormalPoint3D p1, NormalPoint3D p2, double thickness)
        {
            Point1 = p1;
            Point2 = p2;
            Thickness = thickness;
            Length = ThirdDimensionalCalculations.DistanceBetweenPoints(Point1.Point, Point2.Point);
        }

        public NormalPoint3D[] GetPoints()
        {
            return [Point1, Point3!, Point4!, Point2, Point5!, Point6!];
        }

        public void Refresh()
        {
            OnSideOneChanged();
            OnSideTwoChanged();
        }

        private void OnSideOneChanged()
        {
            double hyp1lengthScaleLength;
            double hyp2lengthScaleLength;
            double point1Angle;
            double point2Angle;
            NormalPoint3D side1ThirdPoint;

            if (Side1 is not null)
            {
                if (!Side1.Point1.Equals(Point1) && !Side1.Point1.Equals(Point2))
                {
                    side1ThirdPoint = Side1.Point1;
                    point1Angle = Side1.Point3Angle;
                    point2Angle = Side1.Point2Angle;
                }
                else if (!Side1.Point2.Equals(Point1) && !Side1.Point2.Equals(Point2))
                {
                    side1ThirdPoint = Side1.Point2;
                    point1Angle = Side1.Point1Angle;
                    point2Angle = Side1.Point3Angle;
                }
                else
                {
                    side1ThirdPoint = Side1.Point3;
                    point1Angle = Side1.Point2Angle;
                    point2Angle = Side1.Point1Angle;
                }
                hyp1lengthScaleLength = Thickness / Math.Sin(point1Angle * (Math.PI / 180D));
                hyp2lengthScaleLength = Thickness / Math.Sin(point2Angle * (Math.PI / 180D));

                double hyp1Length = ThirdDimensionalCalculations.DistanceBetweenPoints(side1ThirdPoint.Point, Point1.Point);
                double hyp2Length = ThirdDimensionalCalculations.DistanceBetweenPoints(side1ThirdPoint.Point, Point2.Point);

                double h1Scale = hyp1lengthScaleLength / hyp1Length;
                double h2Scale = hyp2lengthScaleLength / hyp2Length;

                h1Scale = Math.Min(1D, h1Scale);
                h2Scale = Math.Min(1D, h2Scale);

                Vector3D pt3Vector = ThirdDimensionalCalculations.VectorFromPoints(Point1.Point, side1ThirdPoint.Point) * h1Scale;
                Vector3D pt4Vector = ThirdDimensionalCalculations.VectorFromPoints(Point2.Point, side1ThirdPoint.Point) * h2Scale;

                Point3 = new(new(Point1.Point.X + pt3Vector.X, Point1.Point.Y + pt3Vector.Y, Point1.Point.Z + pt3Vector.Z)) { Normal = Point1.Normal };
                Point4 = new(new(Point2.Point.X + pt4Vector.X, Point2.Point.Y + pt4Vector.Y, Point2.Point.Z + pt4Vector.Z)) { Normal = Point2.Normal };

            }
        }

        private void OnSideTwoChanged()
        {
            double hyp1lengthScaleLength;
            double hyp2lengthScaleLength;
            double point1Angle;
            double point2Angle;
            NormalPoint3D side2ThirdPoint;

            if (Side2 is not null)
            {
                if (!Side2.Point1.Equals(Point1) && !Side2.Point1.Equals(Point2))
                {
                    side2ThirdPoint = Side2.Point1;
                    point1Angle = Side2.Point3Angle;
                    point2Angle = Side2.Point2Angle;
                }
                else if (!Side2.Point2.Equals(Point1) && !Side2.Point2.Equals(Point2))
                {
                    side2ThirdPoint = Side2.Point2;
                    point1Angle = Side2.Point1Angle;
                    point2Angle = Side2.Point3Angle;
                }
                else
                {
                    side2ThirdPoint = Side2.Point3;
                    point1Angle = Side2.Point2Angle;
                    point2Angle = Side2.Point1Angle;
                }

                hyp1lengthScaleLength = Thickness / Math.Sin(point1Angle * (Math.PI / 180D));
                hyp2lengthScaleLength = Thickness / Math.Sin(point2Angle * (Math.PI / 180D));

                double hyp1Length = ThirdDimensionalCalculations.DistanceBetweenPoints(side2ThirdPoint.Point, Point1.Point);
                double hyp2Length = ThirdDimensionalCalculations.DistanceBetweenPoints(side2ThirdPoint.Point, Point2.Point);

                double h1Scale = hyp1lengthScaleLength / hyp1Length;
                double h2Scale = hyp2lengthScaleLength / hyp2Length;

                h1Scale = Math.Min(1D, h1Scale);
                h2Scale = Math.Min(1D, h2Scale);

                Vector3D pt5Vector = ThirdDimensionalCalculations.VectorFromPoints(Point1.Point, side2ThirdPoint.Point) * h1Scale;
                Vector3D pt6Vector = ThirdDimensionalCalculations.VectorFromPoints(Point2.Point, side2ThirdPoint.Point) * h2Scale;

                Point5 = new(new(Point1.Point.X + pt5Vector.X, Point1.Point.Y + pt5Vector.Y, Point1.Point.Z + pt5Vector.Z)) { Normal = Point1.Normal };
                Point6 = new(new(Point2.Point.X + pt6Vector.X, Point2.Point.Y + pt6Vector.Y, Point2.Point.Z + pt6Vector.Z)) { Normal = Point2.Normal };
            }
        }

        public bool Contains(NormalPoint3D p)
        {
            return Point1.PointNumber == p.PointNumber || Point2.PointNumber == p.PointNumber;
        }
    }
}
