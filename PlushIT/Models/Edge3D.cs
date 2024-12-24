using PlushIT.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PlushIT.Models
{
    public class Edge3D
    {
        public IndexPoint3D StartPoint { get; set; }
        public IndexPoint3D EndPoint { get; set; }
        public IndexPoint3D Point1 { get; set; }
        public IndexPoint3D Point2 { get; set; }

        public double Length { get; set; }

        public Edge3D(IndexPoint3D pt1, IndexPoint3D pt2, IndexPoint3D pt3, IndexPoint3D pt4)
        {
            StartPoint = pt1;
            Point1 = pt2;
            Point2 = pt3;
            EndPoint = pt4;

            Length = ThirdDimensionalCalculations.DistanceBetweenPoints(StartPoint.Point, EndPoint.Point);
        }

        public bool IsEdgeShared(Edge3D? edge)
        {
            return edge is not null && edge.Length == Length &&
                ((edge.StartPoint == StartPoint && edge.EndPoint == EndPoint) ||
                (edge.EndPoint == StartPoint && edge.StartPoint == EndPoint));

        }
    }
}
