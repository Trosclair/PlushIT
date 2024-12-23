using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace PlushIT.Utilities
{

    public class SphereMeshGenerator
    {
        private int currentTriangleIndicies = 0;
        private MeshGeometry3D mesh = new();

        public int Slices { get; set; } = 8;
        public int Stacks { get; set; } = 4;
        public MeshGeometry3D Mesh => mesh;

        public MeshGeometry3D AddMesh(Point3D center, double radius)
        {
            for (int stack = 0; stack <= Stacks; stack++)
            {
                double phi = Math.PI / 2 - stack * Math.PI / Stacks; // kut koji zamisljeni pravac povucen iz sredista koordinatnog sustava zatvara sa XZ ravninom. 
                double y = radius * Math.Sin(phi); // Odredi poziciju Y koordinate. 
                double scale = -radius * Math.Cos(phi);

                for (int slice = 0; slice <= Slices; slice++)
                {
                    double theta = slice * 2 * Math.PI / Slices; // Kada gledamo 2D koordinatni sustav osi X i Z... ovo je kut koji zatvara zamisljeni pravac povucen iz sredista koordinatnog sustava sa Z osi ( Z = Y ). 
                    double x = scale * Math.Sin(theta); // Odredi poziciju X koordinate. Uoči da je scale = -_radius * Math.Cos(phi)
                    double z = scale * Math.Cos(theta); // Odredi poziciju Z koordinate. Uoči da je scale = -_radius * Math.Cos(phi)

                    Vector3D normal = new Vector3D(x, y, z); // Normala je vektor koji je okomit na površinu. U ovom slučaju normala je vektor okomit na trokut plohu trokuta. 
                    mesh.Normals.Add(normal);
                    mesh.Positions.Add(normal + center);     // Positions dobiva vrhove trokuta. 
                }
            }

            for (int stack = 0; stack <= Stacks; stack++)
            {
                int top = (stack + 0) * (Slices + 1) + currentTriangleIndicies;
                int bot = (stack + 1) * (Slices + 1) + currentTriangleIndicies;

                for (int slice = 0; slice < Slices; slice++)
                {
                    if (stack != 0)
                    {
                        mesh.TriangleIndices.Add(top + slice);
                        mesh.TriangleIndices.Add(bot + slice);
                        mesh.TriangleIndices.Add(top + slice + 1);
                    }

                    if (stack != Stacks - 1)
                    {
                        mesh.TriangleIndices.Add(top + slice + 1);
                        mesh.TriangleIndices.Add(bot + slice);
                        mesh.TriangleIndices.Add(bot + slice + 1);
                    }
                }
            }

            currentTriangleIndicies += Stacks * Slices;

            return mesh;
        }
    }
}
