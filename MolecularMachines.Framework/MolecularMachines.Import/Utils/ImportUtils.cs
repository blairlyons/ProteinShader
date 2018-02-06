using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model.Compartments;
using PdbXReader;
using PdbXReader.Model;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.Utils
{
    static class ImportUtils
    {
        public static Entry ReadPdbXCif(string path)
        {
            using (var reader = new ModelReader())
            {
                return reader.ReadEntry(System.IO.File.ReadAllText(path));
            }
        }

        public static void ClearFolder(string folder)
        {
            var directory = new DirectoryInfo(folder);

            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                subDirectory.Delete(true);
            }
        }

        public static Vector CalcCenter(IEnumerable<Vector> vectors)
        {
            var sum = Vector.Zero;
            var count = vectors.Count();

            foreach (var atomSite in vectors)
            {
                var v = new Vector(atomSite.X, atomSite.Y, atomSite.Z);

                sum += v;
            }

            var center = sum / count;
            return center;
        }
    }
}
