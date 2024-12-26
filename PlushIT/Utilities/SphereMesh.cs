using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace PlushIT.Utilities
{

    public class SphereMesh(List<Vector3D> normals, List<Point3D> positions, List<int> triangleIndices)
    {
        public IEnumerable<Vector3D> Normals { get; } = normals;
        public IEnumerable<Point3D> Positions { get; } = positions;
        public IEnumerable<int> TriangleIndices { get; } = triangleIndices;

        public static SphereMesh GenerateSphere(Point3D center, double radius, int slices = 16, int stacks = 8)
        {
            List<Vector3D> normals = [];
            List<Point3D> positions = [];
            List<int> triangleIndices = [];

            for (int stack = 0; stack <= stacks; stack++)
            {
                double phi = Math.PI / 2 - stack * Math.PI / stacks; 
                double y = radius * Math.Sin(phi); 
                double scale = -radius * Math.Cos(phi);

                for (int slice = 0; slice <= slices; slice++)
                {
                    double theta = slice * 2 * Math.PI / slices; 
                    double x = scale * Math.Sin(theta);
                    double z = scale * Math.Cos(theta);

                    Vector3D normal = new(x, y, z);
                    normals.Add(normal);
                    positions.Add(normal + center);
                }
            }

            for (int stack = 0; stack <= stacks; stack++)
            {
                int top = stack * (slices + 1);
                int bot = (stack + 1) * (slices + 1);

                for (int slice = 0; slice < slices; slice++)
                {
                    if (stack != 0)
                    {
                        triangleIndices.Add(top + slice);
                        triangleIndices.Add(bot + slice);
                        triangleIndices.Add(top + slice + 1);
                    }

                    if (stack != stacks - 1)
                    {
                        triangleIndices.Add(top + slice + 1);
                        triangleIndices.Add(bot + slice);
                        triangleIndices.Add(bot + slice + 1);
                    }
                }
            }

            return new(normals, positions, triangleIndices);
        }
    }
}
