using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlushIT.Models
{
    public class Triangle3D(IndexPoint3D pt1, IndexPoint3D pt2, IndexPoint3D pt3)
    {
        public IndexPoint3D Point1 { get; set; } = pt1;
        public IndexPoint3D Point2 { get; set; } = pt2;
        public IndexPoint3D Point3 { get; set; } = pt3;
    }
}
