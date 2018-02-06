using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Builders;
using MolecularMachines.Framework.Model.Compartments;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Import;
using MolecularMachines.Import.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.KinesinConcept
{
    class ImportKinesinConcept : ImportBase
    {
        private Compartment compartment;

        private EntityClassBuilder head1;
        private EntityClassBuilder head2;
        private EntityClassBuilder neckLinker1;
        private EntityClassBuilder neckLinker2;
        private EntityClassBuilder kinesin;
        private EntityClassBuilder tubulin;

        public override void Import()
        {
            var pdbsPath = this.RootPath;

            this.Environment.Name = "Kinesin (Concept)";

            this.compartment = new Compartment("main", new Vector(-250, -250, -250), new Vector(250, 250, 250));
            this.Environment.AddCompartment(compartment);

            ImportTubulin();
            this.head1 = ImportHead(1);
            this.head2 = ImportHead(2);

            this.neckLinker1 = ImportNeckLinker(1);
            this.neckLinker2 = ImportNeckLinker(2);

            ImportKinesin_();

            //
            // Instances
            //

            // tubulin
            var tubulinClass = this.tubulin.Create();
            this.Environment.AddEntityClass(tubulinClass);

            var firstTubulin = this.Environment.AddEntity(tubulinClass);
            firstTubulin.Compound.Fix(
                new LocRotStatic(
                    Vector.Zero,
                    Quaternion.Identity
                )
            );

            var lastTubulin = firstTubulin;

            for (int i = 0; i < 15; i++)
            {
                var t = this.Environment.AddEntity(tubulinClass);
                lastTubulin.BindingSiteById("next").InstantBind(t.BindingSiteById("previous"));

                lastTubulin = t;
            }

            // head1
            var head1Class = this.head1.Create();
            this.Environment.AddEntityClass(head1Class);
            var head1Entity = this.Environment.AddEntity(head1Class);

            // head2
            var head2Class = this.head2.Create();
            this.Environment.AddEntityClass(head2Class);
            var head2Entity = this.Environment.AddEntity(head2Class);

            // neckLinker1
            var neckLinker1Class = this.neckLinker1.Create();
            this.Environment.AddEntityClass(neckLinker1Class);
            var neckLinker1Entity = this.Environment.AddEntity(neckLinker1Class);

            // neckLinker2
            var neckLinker2Class = this.neckLinker2.Create();
            this.Environment.AddEntityClass(neckLinker2Class);
            var neckLinker2Entity = this.Environment.AddEntity(neckLinker2Class);

            // kinesin
            var kinesinClass = this.kinesin.Create();
            this.Environment.AddEntityClass(kinesinClass);
            var kinesinEntity = this.Environment.AddEntity(kinesinClass);

            kinesinEntity.BindingSiteById("neckLinker1Site").InstantBind(neckLinker1Entity.BindingSiteById("hipSite"));
            kinesinEntity.BindingSiteById("neckLinker2Site").InstantBind(neckLinker2Entity.BindingSiteById("hipSite"));

            neckLinker1Entity.BindingSiteById("neckSite").InstantBind(head1Entity.BindingSiteById("neckSite"));
            neckLinker2Entity.BindingSiteById("neckSite").InstantBind(head2Entity.BindingSiteById("neckSite"));

            //firstTubulin.BindingSiteById("kinesinSite").InstantBind(head1Entity.BindingSiteById("tubulinSite"));

            kinesinEntity.Compound.Float(new LocRotStatic(new Vector(0, 50, 10), Quaternion.FromYawPitchRoll(0, 0, pi)));
        }

        private EntityClassBuilder ImportNeckLinker(int number)
        {
            var neckLinker = new EntityClassBuilder("neckLinker" + number);
            //this.tubulin.DefaultColor = new Color(0f, 0f, 1f);

            var example = new ExampleStructureCreator();
            var length = 15;
            example.Cuboid(neckLinker, new Vector(1, 1, length));

            neckLinker.BindingSite("hipSite", new LocRotStatic(new Vector(0, 0, -length / 2f), BindingSite.BindingQuaternion));
            neckLinker.BindingSite("neckSite", new LocRotStatic(new Vector(0, 0, length / 2f), Quaternion.Identity));

            return neckLinker;
        }

        private void ImportKinesin_()
        {
            this.kinesin = new EntityClassBuilder("kinesin");
            this.kinesin.BehaviorId = "kinesin.kinesin";

            this.kinesin.BindingSite("neckLinker1Site", new LocRotStatic(new Vector(-1, 0, 0), Quaternion.Identity));
            this.kinesin.BindingSite("neckLinker2Site", new LocRotStatic(new Vector(1, 0, 0), Quaternion.FromYawPitchRoll(0, pi, 0)));
        }

        private EntityClassBuilder ImportHead(int number)
        {
            var head = new EntityClassBuilder("head" + number);
            head.BehaviorId = "kinesin.head";
            head.DefaultColor = Color.Random;
            //this.tubulin.DefaultColor = new Color(0f, 0f, 1f);

            head.SetConformation("free");

            var example = new ExampleStructureCreator();
            example.Cuboid(head, new Vector(5, 7.5f, 15));

            float neckSiteX = (number == 1) ? 2.5f : -2.5f;
            var neckSitePosition = new Vector(neckSiteX, -3.75f, -7.5f);
            var neckSiteRotation = Quaternion.FromYawPitchRoll(0, 0, pi);

            head.BindingSite("neckSite", new LocRotStatic(neckSitePosition, neckSiteRotation));
            head.BindingSite("tubulinSite", new LocRotStatic(new Vector(0, 5, 0), BindingSite.BindingQuaternion));

            head.Sensor(
                "tubulinSensor",
                25,
                GeometryUtils.pi / 3f,
                new LocRotStatic(new Vector(0, 5, 0), Quaternion.FromVectorRotation(Sensor.SenseVector, new Vector(0, 1, 0)))
            );


            head.CopyCurrentConformation("weaklyBound");
            head.CurrentConformation.Color = new Color(0.3f, 0.3f, 0.5f);

            head.CopyCurrentConformation("stronglyBound");
            head.CurrentConformation.Color = new Color(1f, 1f, 0.6f);
            head.BindingSite("neckSite", new LocRotStatic(neckSitePosition, Quaternion.FromYawPitchRoll(0, -pi * 0.88f, pi)));

            return head;
        }

        private void ImportTubulin()
        {
            this.tubulin = new EntityClassBuilder("tubulin");
            //this.tubulin.DefaultColor = new Color(0f, 0f, 1f);
            this.tubulin.BehaviorId = "kinesin.tubulin";

            var example = new ExampleStructureCreator();
            example.Cuboid(this.tubulin, new Vector(10, 10, 15));

            this.tubulin.BindingSite("previous", new LocRotStatic(new Vector(0, 0, -8.5f), Quaternion.Identity));
            this.tubulin.BindingSite("next", new LocRotStatic(new Vector(0, 0, +8.5f), BindingSite.BindingQuaternion));

            this.tubulin.BindingSite("kinesinSite", new LocRotStatic(new Vector(0, 5, 0), BindingSite.BindingQuaternion));
        }
    }
}
