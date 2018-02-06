using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.Utils
{
    class ExampleStructureCreator
    {
        public ExampleStructureCreator()
        {
            this.Element = Element.C;
            this.AtomDistance = this.Element.VdWRadius * 10f;
        }

        public Element Element { get; set; }
        public float AtomDistance { get; set; }

        public void Cuboid(EntityClassBuilder builder, Vector size)
        {
            int countX = (int)Math.Ceiling(size.X / this.AtomDistance);
            int countY = (int)Math.Ceiling(size.Y / this.AtomDistance);
            int countZ = (int)Math.Ceiling(size.Z / this.AtomDistance);

            int lastX = countX - 1;
            int lastY = countY - 1;
            int lastZ = countZ - 1;

            Vector start = new Vector(
                -size.X / 2f,
                -size.Y / 2f,
                -size.Z / 2f
            );

            for (int z = 0; z < countZ; z++)
            {
                for (int y = 0; y < countY; y++)
                {
                    for (int x = 0; x < countX; x++)
                    {

                        if (x == 0 || x == lastX ||
                            y == 0 || y == lastY ||
                            z == 0 || z == lastZ)
                        {

                            var position = new Vector(
                                start.X + this.AtomDistance * x,
                                start.Y + this.AtomDistance * y,
                                start.Z + this.AtomDistance * z
                            );

                            builder.AddAtom(this.Element, position);

                        }

                    }
                }
            }
        }
    }
}
