using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Builders;
using MolecularMachines.Framework.Model.Compartments;
using MolecularMachines.Framework.Model.ConcentrationControllers;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Import;
using MolecularMachines.Import.Utils;
using PdbXReader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.Hemoglobin
{
    class ImportHemoglobin : ImportBase
    {
        private Entry entryOxy;
        private Entry entryDeoxy;

        private Compartment compartment;

        private EntityClassBuilder hemoglobin;
        private EntityClassBuilder o2;
        private EntityClassBuilder co;

        private Vector HemoSphereCenter = new Vector(-225, 150, -350);
        private float HemoSphereRadius = 400;

        public override void Import()
        {

            this.Environment.Name = "Hemoglobin";

            this.entryOxy = LoadCif(@"1hho_oxy.cif");
            this.entryDeoxy = LoadCif(@"2hhb_deoxy.cif");

            ImportO2();
            ImportCo();

            ImportHemoglobin_();
        }

        public void CreateSingleHemoglobin()
        {
            hemoglobin.BehaviorId = "test.overview";
            CreateHb(LocRot.Zero);
        }

        public void CreateEntityOverview()
        {
            this.compartment = new Compartment("main", new Vector(-150, -150, -150), new Vector(150, 150, 150));
            hemoglobin.BehaviorId = "test.conformation.increment";

            for (int i = 0; i < 5; i++)
            {
                var lr = new LocRotStatic(
                    new Vector(0, 50, -150 + 75 * i),
                    Quaternion.Identity
                );
                CreateHb(lr);
            }
            CreateEntity(o2).Compound.Fix(new LocRotStatic(new Vector(0, 0, -50), Quaternion.Identity));
            CreateEntity(co).Compound.Fix(new LocRotStatic(new Vector(0, 0, -100), Quaternion.Identity));
        }

        public void CreateTestEnvironment()
        {
            this.compartment = new Compartment("main", new Vector(-150, -150, -150), new Vector(150, 150, 150));
            this.Environment.AddCompartment(compartment);

            for (int i = 0; i < 10; i++)
            {
                CreateEntity(hemoglobin).Compound.Float(
                    new LocRotStatic(
                        GeometryUtils.RandomVectorInsideCompartment(compartment)
                        ,
                        GeometryUtils.RandomQuaternion()
                    )
                );
            }

            // o2
            var o2Class = this.o2.Create();
            this.Environment.AddEntityClass(o2Class);

            for (int i = 0; i < 0; i++)
            {
                var o2 = this.Environment.AddEntity(o2Class);

                o2.Compound.Float(
                    new LocRotStatic(
                        GeometryUtils.RandomVectorInsideCompartment(compartment)
                        ,
                        GeometryUtils.RandomQuaternion()
                    )
                );

                /*proton.Compound.Float(
                    new LocRotStatic(
                        GeometryUtils.RandomVectorInsideCompartment(compartment),
                        GeometryUtils.RandomQuaternion()
                    )
                );*/
                //hb.BindingSites[i].InstantBind(o2.BindingSites[0]);
            }

            // co
            var coClass = this.co.Create();
            this.Environment.AddEntityClass(coClass);

            for (int i = 0; i < 0; i++)
            {
                var proton = this.Environment.AddEntity(coClass);
                proton.Compound.Float(
                    new LocRotStatic(
                        GeometryUtils.RandomVectorInsideCompartment(compartment),
                        GeometryUtils.RandomQuaternion()
                    )
                );
            }

            //
            // ConcentrationControllers
            //

            this.Environment.AddConcentrationController(new ConcentrationController(this.Environment, "hemoglobin.ef.freeO2", "free O2", 0, 2000));
            this.Environment.AddConcentrationController(new ConcentrationController(this.Environment, "hemoglobin.ef.freeCo", "free CO", 0, 2000));
            this.Environment.AddConcentrationController(new ConcentrationController(this.Environment, "hemoglobin.ef.hemoglobin", "Hemoglobin", 0, 15));
        }

        public void CreatePoster()
        {
            // hemoglobin
            var hemoglobinClass = this.hemoglobin.Create();
            this.Environment.AddEntityClass(hemoglobinClass);

            var hb = CreateHb(HemoSphereCenter);
            CreateHb(HemoSphereCenter - new Vector(100, 80, 135));
            CreateHb(HemoSphereCenter - new Vector(44, 20, -150));
            CreateHb(HemoSphereCenter - new Vector(52, -30, 100));
            CreateHb(HemoSphereCenter + new Vector(53, 43, 80));
            CreateHb(HemoSphereCenter + new Vector(0, 66, 180));


            // o2
            var o2Class = this.o2.Create();
            this.Environment.AddEntityClass(o2Class);

            for (int i = 0; i < 300; i++)
            {
                var o2 = this.Environment.AddEntity(o2Class);

                o2.Compound.Fix(
                    new LocRotStatic(
                        GeometryUtils.RandomVectorInPropabilitySphere(HemoSphereCenter, HemoSphereRadius)
                        ,
                        GeometryUtils.RandomQuaternion()
                    )
                );

                /*proton.Compound.Float(
                    new LocRotStatic(
                        GeometryUtils.RandomVectorInsideCompartment(compartment),
                        GeometryUtils.RandomQuaternion()
                    )
                );*/
                //hb.BindingSites[i].InstantBind(o2.BindingSites[0]);
            }

            // co
            var coClass = this.co.Create();
            this.Environment.AddEntityClass(coClass);

            for (int i = 0; i < 0; i++)
            {
                var proton = this.Environment.AddEntity(coClass);
                proton.Compound.Float(
                    new LocRotStatic(
                        GeometryUtils.RandomVectorInsideCompartment(compartment),
                        GeometryUtils.RandomQuaternion()
                    )
                );
            }
        }

        private Framework.Model.Entities.Entity CreateHb(LocRot locRot)
        {
            var hb = this.CreateEntity(hemoglobin);
            hb.Compound.Fix(
                locRot
            );

            return hb;
        }

        private Framework.Model.Entities.Entity CreateHb(Vector v)
        {
            var hb = CreateHb(new LocRotStatic(v, GeometryUtils.RandomQuaternion()));
            return
                hb;
        }

        private void ImportO2()
        {
            this.o2 = new EntityClassBuilder("o2");
            this.o2.DefaultColor = new Color(1f, 0f, 0f);

            this.o2.AddAtoms(FilterAsymmetricUnit(this.entryOxy, "E"), atomSite => atomSite.Symbol, AtomSiteToVector); // E is the ID of an oxygen molecule
            this.o2.ReCenter();

            this.o2.BindingSite("site", BindingSite.DirectBindingLocRot);
        }

        private void ImportCo()
        {
            this.co = new EntityClassBuilder("co");
            this.co.DefaultColor = new Color(0f, 0f, 0f);

            this.co.AddAtom("C", new Vector(-1, 0, 0));
            this.co.AddAtom("O", new Vector(1, 0, 0));

            this.co.BindingSite("site", BindingSite.DirectBindingLocRot);
        }

        private Vector AtomSiteToVector(AtomSite atomSite)
        {
            return new Vector(atomSite.X, atomSite.Y, atomSite.Z);
        }

        private Vector Mirror(Vector v)
        {
            return new Vector(
                v.Y,
                v.X,
                -v.Z
            );
        }

        private void ImportHemoglobin_()
        {
            this.hemoglobin = new EntityClassBuilder("hemoglobin");
            this.hemoglobin.DefaultColor = new Color(1f, 1f, 1f);
            this.hemoglobin.BehaviorId = "hemoglobin.hemoglobin";

            this.hemoglobin.SetConformation("deoxy");
            //this.hemoglobin.CurrentConformation.Color = new Color(1f, 1f, 1f);
            this.hemoglobin.CurrentConformation.Color = new Color(114f/255, 152f/255, 1f);
            this.hemoglobin.AddAtoms(FilterAsymmetricUnit(this.entryDeoxy, "A"), atomSite => atomSite.Symbol, AtomSiteToVector);
            this.hemoglobin.AddAtoms(FilterAsymmetricUnit(this.entryDeoxy, "B"), atomSite => atomSite.Symbol, AtomSiteToVector);
            this.hemoglobin.AddAtoms(FilterAsymmetricUnit(this.entryDeoxy, "A"), atomSite => atomSite.Symbol, a => Mirror(AtomSiteToVector(a)));
            this.hemoglobin.AddAtoms(FilterAsymmetricUnit(this.entryDeoxy, "B"), atomSite => atomSite.Symbol, a => Mirror(AtomSiteToVector(a)));

            this.hemoglobin.SetConformation("100% oxy");
            //this.hemoglobin.CurrentConformation.Color = new Color(1f, 0f, 0f);
            this.hemoglobin.CurrentConformation.Color = new Color(1f, 0.5f, 0.5f);
            this.hemoglobin.AddAtoms(FilterAsymmetricUnit(this.entryOxy, "A"), atomSite => atomSite.Symbol, AtomSiteToVector);
            this.hemoglobin.AddAtoms(FilterAsymmetricUnit(this.entryOxy, "B"), atomSite => atomSite.Symbol, AtomSiteToVector);
            this.hemoglobin.AddAtoms(FilterAsymmetricUnit(this.entryOxy, "A"), atomSite => atomSite.Symbol, a => Mirror(AtomSiteToVector(a)));
            this.hemoglobin.AddAtoms(FilterAsymmetricUnit(this.entryOxy, "B"), atomSite => atomSite.Symbol, a => Mirror(AtomSiteToVector(a)));

            this.hemoglobin.InterpolateConformation(1, "25% oxy", "deoxy", "100% oxy", 0.25f);
            this.hemoglobin.InterpolateConformation(2, "50% oxy", "deoxy", "100% oxy", 0.50f);
            this.hemoglobin.InterpolateConformation(3, "75% oxy", "deoxy", "100% oxy", 0.75f);

            //
            // binding sites
            //

            var site1 = ImportUtils.CalcCenter(this.FilterAsymmetricUnit(entryOxy, "E").Select(AtomSiteToVector));
            var site2 = ImportUtils.CalcCenter(this.FilterAsymmetricUnit(entryOxy, "G").Select(AtomSiteToVector));
            var site3 = Mirror(site1);
            var site4 = Mirror(site2);

            this.hemoglobin.BindingSite("site1", new LocRotStatic(site1, Quaternion.Identity));
            this.hemoglobin.BindingSite("site2", new LocRotStatic(site2, Quaternion.Identity));
            this.hemoglobin.BindingSite("site3", new LocRotStatic(site3, Quaternion.Identity));
            this.hemoglobin.BindingSite("site4", new LocRotStatic(site4, Quaternion.Identity));

            //
            // sensors
            //
            float range = 60;
            float apertureAngle = 2.25f;

            this.hemoglobin.Sensor("sensor1", range, apertureAngle, SensorLocRotFromSite(site1));
            this.hemoglobin.Sensor("sensor2", range, apertureAngle, SensorLocRotFromSite(site2));
            this.hemoglobin.Sensor("sensor3", range, apertureAngle, SensorLocRotFromSite(site3));
            this.hemoglobin.Sensor("sensor4", range, apertureAngle, SensorLocRotFromSite(site4));
        }

        private LocRot SensorLocRotFromSite(Vector site)
        {
            return new LocRotStatic(
                site,
                Quaternion.FromVectorRotation(Sensor.SenseVector, site)
            );
        }
    }
}
