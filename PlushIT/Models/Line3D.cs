using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlushIT.Models
{
    public class Line3D
    {
        public Edge3D Edge1 { get; set; }
        public Edge3D Edge2 { get; set; }

        public Line3D(Edge3D edge1, Edge3D edge2)
        {
            Edge1 = edge1;
            Edge2 = edge2;
        }
    }
}
