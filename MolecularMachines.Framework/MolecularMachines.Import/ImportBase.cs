using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Builders;
using MolecularMachines.Framework.Model.Compartments;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Persistence;
using MolecularMachines.Import.Utils;
using PdbXReader.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import
{
    abstract class ImportBase
    {
        public ImportBase()
        {
            this.SetEnvironment(new MMEnvironment());
        }

        public void SetEnvironment(MMEnvironment newEnvironment)
        {
            this.Environment = newEnvironment;
        }

        public void Import(string pdbsPath)
        {
            SetRootPath(pdbsPath);
            Import();
        }

        public abstract void Import();

        public MMEnvironment Environment { get; private set; }

        public string RootPath { get; private set; }
        public void SetRootPath(string path)
        {
            this.RootPath = path;
        }

        protected Entry LoadCif(string cifFile)
        {
            Console.WriteLine("loading " + cifFile);
            return ImportUtils.ReadPdbXCif(Path.Combine(this.RootPath, cifFile));
        }

        protected void Assert(bool condition)
        {
            if (!condition)
            {
                throw new Exception("Assert failed");
            }
        }

        protected void AssertEqual(object a, object b)
        {
            Assert(object.Equals(a, b));
        }

        protected void AssertEqualItems<T>(IEnumerable<T> enumerable1, IEnumerable<T> enumerable2)
        {
            var array1 = enumerable1.ToArray();
            var array2 = enumerable2.ToArray();

            AssertEqual(array1.Length, array2.Length);

            for (int i = 0; i < array1.Length; i++)
            {
                AssertEqual(array1[i], array2[i]);
            }
        }

        protected static readonly float pi = GeometryUtils.pi;

        protected IEnumerable<AtomSite> FilterAsymetricUnit(Entry entry, string id)
        {
            return
                from atomSite in entry.AtomSites
                where atomSite.AsymmetricUnit.Id == id
                select atomSite;
        }

        protected IEnumerable<AtomSite> FilterEntityId(Entry entry, string entityId)
        {
            return FilterEntityId(entry, new string[] { entityId });
        }

        protected IEnumerable<AtomSite> FilterEntityId(Entry entry, string[] entityIds)
        {
            return
                from atomSite in entry.AtomSites
                where entityIds.Contains(atomSite.Entity.Id)
                select atomSite;
        }

        protected IEnumerable<AtomSite> FilterAsymmetricUnit(Entry entry, string asymmetricUnitId)
        {
            return
                from atomSite in entry.AtomSites
                where atomSite.AsymmetricUnit.Id == asymmetricUnitId
                select atomSite;
        }

        protected void CreateEntityAtZero(EntityClassBuilder b)
        {
            var e = CreateEntity(b);
            e.Compound.Fix(LocRot.Zero);
        }

        protected Framework.Model.Entities.Entity CreateEntityFloating(EntityClassBuilder b, Compartment c)
        {
            var e = CreateEntity(b);
            e.Compound.Float(new LocRotStatic(GeometryUtils.RandomVectorInsideCompartment(c), GeometryUtils.RandomQuaternion()));
            return e;
        }

        protected Framework.Model.Entities.Entity CreateEntity(EntityClassBuilder b)
        {
            EntityClass c = this.Environment.EntityClassByIdOrNull(b.Id);
            if (c == null)
            {
                c = b.Create();
                this.Environment.AddEntityClass(c);
            }

            var e = this.Environment.AddEntity(c);
            return e;
        }

        public void Save(string display, string subDir)
        {
            var root = Path.Combine(display, subDir);
            Directory.CreateDirectory(root); 
            
            foreach (var ec in this.Environment.EntityClasses)
            {
                var ecSerializer = new EntityClassSerializer();
                ecSerializer.Serialize(ec, Path.Combine(root, ec.Id + ".mmec.json"));
            }

            {
                var eSerializer = new EnvironmentSerializer(this.Environment);
                eSerializer.Serialize(Path.Combine(root, "environment.mmenv.json"));
            }
        }
    }
}
