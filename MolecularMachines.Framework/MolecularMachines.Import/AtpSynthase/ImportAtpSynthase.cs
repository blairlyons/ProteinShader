using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Builders;
using MolecularMachines.Framework.Model.Compartments;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Import.Utils;
using PdbXReader.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.AtpSynthase
{
    class ImportAtpSynthase : ImportBase
    {
        private Vector importOffset;
        private Quaternion importRotation;

        private Entry entry1;
        private Entry entry2;
        private Entry entry3;

        private Compartment extraCellularCompartment;
        private Compartment intraCellularCompartment;

        private EntityClassBuilder proton;
        private EntityClassBuilder rotor;
        private EntityClassBuilder f0c;
        private EntityClassBuilder atpSynthase;
        private EntityClassBuilder stator;

        private EntityClassBuilder f1a_1;
        private EntityClassBuilder f1a_2;
        private EntityClassBuilder f1a_3;

        private EntityClassBuilder f1b_1;
        private EntityClassBuilder f1b_2;
        private EntityClassBuilder f1b_3;

        private EntityClassBuilder adp;
        /// <summary>
        /// phosphate residue
        /// </summary>
        private EntityClassBuilder pr;


        private string[] F0cIds = new string[] { "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V" };
        private string[] AxleIds = new string[] { "G", "H" };
        private string[] StatorIds = new string[] { "I", "J", "K", "L" };
        private string[] F1aIds = new string[] { "A", "B", "C" };
        private string[] F1bIds = new string[] { "D", "E", "F" };


        public override void Import()
        {

            // Load cif files
            this.entry1 = LoadCif(@"01_5t4o.cif");
            this.entry2 = LoadCif(@"02_5t4p.cif");
            this.entry3 = LoadCif(@"03_5t4q.cif");

            // calculate offset
            CalcImportTransformation();

            // import proton
            ImportProton();

            // import Rotor
            ImportRotor();

            // import F0c
            ImportF0c();

            // import Stator
            ImportStator();

            // import F1a
            var f1aColor = new Color(0f, 1f, 0f);
            var f1aBehaviorId = "atpSynthase.f1a";
            this.f1a_1 = this.ImportF1Unit("f1a_1", this.F1aIds[0], f1aColor, f1aBehaviorId, "W"); // A
            this.f1a_2 = this.ImportF1Unit("f1a_2", this.F1aIds[1], f1aColor, f1aBehaviorId, "Z"); // B // TODO "Z" is ADP (not ATP)
            this.f1a_3 = this.ImportF1Unit("f1a_3", this.F1aIds[2], f1aColor, f1aBehaviorId, "Y"); // C

            // import F1b
            var f1bColor = new Color(1f, 1f, 0.3f);
            var f1bBehaviorId = "atpSynthase.f1b";
            this.f1b_1 = this.ImportF1Unit("f1b_1", this.F1bIds[0], f1bColor, f1bBehaviorId); // D
            this.f1b_2 = this.ImportF1Unit("f1b_2", this.F1bIds[1], f1bColor, f1bBehaviorId); // E
            this.f1b_3 = this.ImportF1Unit("f1b_3", this.F1bIds[2], f1bColor, f1bBehaviorId); // F

            // import ADP/ATP
            ImportAdpPr();

            // import AtpSynthase
            ImportAtpSynthase_();

            // import AtpSynthaseReference
            //ImportAtpSynthaseReference();


        }

        public void CreateTestEnvironment()
        {
            // basic
            this.Environment.Name = "ATP Synthase";

            extraCellularCompartment = new Compartment("extra", new Vector(-200, -200, -250), new Vector(200, 200, -35));
            intraCellularCompartment = new Compartment("intra", new Vector(-200, -200, 50), new Vector(200, 200, 400));

            this.Environment.AddCompartment(extraCellularCompartment);
            this.Environment.AddCompartment(intraCellularCompartment);

            //
            // create and add classes and entities
            //

            // protons
            var protonClass = proton.Create();
            this.Environment.AddEntityClass(protonClass);


            for (int i = 0; i < 300; i++)
            {
                var proton = this.Environment.AddEntity(protonClass);
                proton.Compound.Float(
                    new LocRotStatic(
                        GeometryUtils.RandomVectorInsideCompartment(extraCellularCompartment),
                        Quaternion.Identity
                    )
                );
            }

            for (int i = 0; i < 40; i++)
            {
                var proton = this.Environment.AddEntity(protonClass);
                proton.Compound.Float(
                    new LocRotStatic(
                        GeometryUtils.RandomVectorInsideCompartment(intraCellularCompartment),
                        Quaternion.Identity
                    )
                );
            }

            // atp synthase
            AddAtpSynthaseEntity(
                new LocRotStatic(
                    new Vector(0, 0, 0),
                    Quaternion.Identity
                )
            );

            // adp and prs
            var adpClass = this.adp.Create();
            this.Environment.AddEntityClass(adpClass);

            var prClass = this.pr.Create();
            this.Environment.AddEntityClass(prClass);

            for (int i = 0; i < 100; i++)
            {
                var adpEntity = this.Environment.AddEntity(adpClass);
                var prEntity = this.Environment.AddEntity(prClass);

                adpEntity.Compound.Float(
                    new LocRotStatic(
                        GeometryUtils.RandomVectorInsideCompartment(intraCellularCompartment),
                        GeometryUtils.RandomQuaternion()
                    )
                );

                prEntity.Compound.Float(
                    new LocRotStatic(
                        GeometryUtils.RandomVectorInsideCompartment(intraCellularCompartment),
                        GeometryUtils.RandomQuaternion()
                    )
                );

                //adpEntity.BindingSiteById("prSite").InstantBind(prEntity.BindingSiteById("adpSite"));
            }
        }

        public void CreatePoster()
        {
            // basic
            this.Environment.Name = "ATP Synthase";

            extraCellularCompartment = new Compartment("extra", new Vector(-200, -100, -80), new Vector(0, 300, 100));
            intraCellularCompartment = new Compartment("intra", new Vector(-200, -100, 130), new Vector(0, 300, 450));

            this.Environment.AddCompartment(extraCellularCompartment);
            this.Environment.AddCompartment(intraCellularCompartment);

            //
            // create and add classes and entities
            //

            // protons
            var protonClass = proton.Create();
            this.Environment.AddEntityClass(protonClass);


            for (int i = 0; i < 80; i++)
            {
                var proton = this.Environment.AddEntity(protonClass);
                proton.Compound.Float(
                    new LocRotStatic(
                        GeometryUtils.RandomVectorInsideCompartment(extraCellularCompartment),
                        Quaternion.Identity
                    )
                );
            }

            for (int i = 0; i < 10; i++)
            {
                var proton = this.Environment.AddEntity(protonClass);
                proton.Compound.Float(
                    new LocRotStatic(
                        GeometryUtils.RandomVectorInsideCompartment(intraCellularCompartment),
                        Quaternion.Identity
                    )
                );
            }

            // atp synthase
            AddAtpSynthaseEntity(
                new LocRotStatic(
                    new Vector(-26.7f, 80, 115),
                    Quaternion.FromYawPitchRollDeg(-9.7f, 0.84f, 0)
                )
            );
            AddAtpSynthaseEntity(
                new LocRotStatic(
                    new Vector(-72, -63.7f, 137.4f),
                    Quaternion.FromYawPitchRollDeg(1.4f, 0.84f, 56.2f)
                )
            );

            // adp and prs
            var adpClass = this.adp.Create();
            this.Environment.AddEntityClass(adpClass);

            var prClass = this.pr.Create();
            this.Environment.AddEntityClass(prClass);

            for (int i = 0; i < 75; i++)
            {
                var adpEntity = this.Environment.AddEntity(adpClass);
                var prEntity = this.Environment.AddEntity(prClass);

                adpEntity.Compound.Fix(
                    new LocRotStatic(
                        GeometryUtils.RandomVectorInsideCompartment(intraCellularCompartment),
                        GeometryUtils.RandomQuaternion()
                    )
                );

                prEntity.Compound.Fix(
                    new LocRotStatic(
                        GeometryUtils.RandomVectorInsideCompartment(intraCellularCompartment),
                        GeometryUtils.RandomQuaternion()
                    )
                );

                //adpEntity.BindingSiteById("prSite").InstantBind(prEntity.BindingSiteById("adpSite"));
            }
        }

        private void AddAtpSynthaseEntity(LocRot locRot)
        {
            var atpSynthaseEntity = this.CreateEntity(atpSynthase); ;
            atpSynthaseEntity.Compound.Fix(locRot);

            // rotor
            var rotorEntity = this.CreateEntity(rotor);

            atpSynthaseEntity.BindingSiteById("rotorSite").InstantBind(rotorEntity.BindingSiteById("atpSynthaseSite"));

            // f0cs
            foreach (var bindingSite in rotorEntity.BindingSites)
            {
                if (bindingSite.Id.Contains("f0c"))
                {
                    var f0cEntity = CreateEntity(f0c);

                    bindingSite.InstantBind(f0cEntity.BindingSiteById("rotorSite"));
                }
            }

            // stator
            var statorEntity = this.CreateEntity(stator);

            atpSynthaseEntity.BindingSiteById("statorSite").InstantBind(statorEntity.BindingSiteById("atpSynthaseSite"));

            // add F1 entites
            AddF1Entity(this.f1a_1, atpSynthaseEntity.BindingSiteById("f1a_1"));
            AddF1Entity(this.f1a_2, atpSynthaseEntity.BindingSiteById("f1a_2"));
            AddF1Entity(this.f1a_3, atpSynthaseEntity.BindingSiteById("f1a_3"));

            AddF1Entity(this.f1b_1, atpSynthaseEntity.BindingSiteById("f1b_1"));
            AddF1Entity(this.f1b_2, atpSynthaseEntity.BindingSiteById("f1b_2"));
            AddF1Entity(this.f1b_3, atpSynthaseEntity.BindingSiteById("f1b_3"));
        }

        private void ImportAdpPr()
        {
            var atpAtomSites = FilterAsymetricUnit(entry1, "W").ToArray();
            var atpAtomPositions = atpAtomSites.Select(ImportAtomSiteVector).ToArray();
            var atpAtomSymbols = entry1.AtomSites.Select(atomSite => atomSite.Symbol).ToArray();

            this.pr = new EntityClassBuilder("pr");
            this.pr.BehaviorId = "atpSynthase.pr";

            this.adp = new EntityClassBuilder("adp");
            this.adp.BehaviorId = "atpSynthase.adp";

            for (int i = 0; i < atpAtomSites.Length; i++)
            {
                if (i < 4)
                {
                    // cut off first 4 atoms (POOO) --> this is phosphate residue

                    this.pr.AddAtom(atpAtomSymbols[i], atpAtomPositions[i]);
                }
                else
                {
                    // the rest ist ADP

                    var atomPosition = atpAtomPositions[i];
                    this.adp.AddAtom(atpAtomSymbols[i], atomPosition);
                }
            }

            // binding location where adp + pr bind (between 3 and 4) (which is also the new center of the entities)
            var bindingLocation = (atpAtomPositions[3] + atpAtomPositions[4]) / 2f;

            this.pr.SetOrigin(bindingLocation);
            this.adp.SetOrigin(bindingLocation);

            this.pr.BindingSite("adpSite", BindingSite.DirectBindingLocRot);
            this.adp.BindingSite("prSite", new LocRotStatic(Vector.Zero, Quaternion.Identity));
            this.adp.BindingSite("mainSite", new LocRotStatic(Vector.Zero, Quaternion.Identity));
        }

        private void AddF1Entity(EntityClassBuilder builder, BindingSite atpSynthaseBindingSite)
        {
            var entityClass = builder.Create();
            this.Environment.AddEntityClass(entityClass);
            var entity = this.Environment.AddEntity(entityClass);

            atpSynthaseBindingSite.InstantBind(entity.BindingSiteById("atpSynthaseSite"));
        }

        private void ImportProton()
        {
            this.proton = new EntityClassBuilder("H+");
            this.proton.DefaultColor = new Color(1f, 1f, 0.3f); // bright yellow
            this.proton.AddAtom("H", Vector.Zero);
            this.proton.BindingSite("BindingSite", LocRot.Zero);
            this.proton.BehaviorId = "present.empty";
        }

        private Vector AtomSiteToVectorRaw(AtomSite atomSite)
        {
            return new Vector(atomSite.X, atomSite.Y, atomSite.Z);
        }

        private Vector ImportAtomSiteVector(AtomSite atomSite)
        {
            var v = new Vector(atomSite.X, atomSite.Y, atomSite.Z);
            v = v - this.importOffset;
            v = this.importRotation.Rotate(v);
            return v;
        }

        private Atom AtomSiteToAtom(AtomSite atomSite)
        {
            return new Atom(Element.BySymbol(atomSite.Symbol));
        }

        private void CalcImportTransformation()
        {
            // use center of all F0c entites as origin (= offset)
            // Asymetric Unit Ids of F0c parts
            var f0cCenters =
                F0cIds
                    .Select(id => FilterAsymetricUnit(entry1, id)) // get AtomSites from Asymetric Unit
                    .Select(atomSites => ImportUtils.CalcCenter(atomSites.Select(AtomSiteToVectorRaw))); // get center from that AtomSites

            var center = ImportUtils.CalcCenter(f0cCenters);

            // set offset
            this.importOffset = center;

            // Optimize Rotation so that all F0c lay on Z-plane
            Console.WriteLine("Optimizing Rotation...");

            var centeredF0cCenters = f0cCenters.Select(v => v - this.importOffset).ToArray();
            var optimizeRotation = new SimpleOptimizer(
                new double[] { -Math.PI / 4 }, //, -Math.PI / 4, -Math.PI / 4 },
                new double[] { Math.PI / 4 }, //, Math.PI / 4, Math.PI / 4 },
                50000,
                (sample) =>
                {
                    var q = Quaternion.FromYawPitchRoll(0, (float)sample[0], 0);
                    var rotatedF0cCenters = centeredF0cCenters.Select(v => q.Rotate(v));
                    var cost = rotatedF0cCenters.Select(v => Math.Abs(v.Z)).Sum();
                    return cost;
                }
            );

            var optimizeStart = DateTime.Now;
            optimizeRotation.Optimize();
            var optimizeEnd = DateTime.Now;
            Console.WriteLine("Optimization took: " + (optimizeEnd - optimizeStart).TotalMilliseconds + "ms");


            // Output
            {
                var sample = optimizeRotation.BestSample;

                Console.WriteLine("Optimized Rotation: yaw=0; pitch=" + RadToDegStr(sample[0]) + "; roll=0");

                var q = Quaternion.FromYawPitchRoll(0, (float)sample[0], 0);
                this.importRotation = q;

                var rotatedF0cCenters = centeredF0cCenters.Select(v => q.Rotate(v)).ToArray();
                var cost = rotatedF0cCenters.Select(v => Math.Abs(v.Z)).Sum();

                var zAxesStr = rotatedF0cCenters.Select(v => v.Z.ToString("0.000")).Aggregate((a, b) => a + "; " + b);

                Console.WriteLine("Z-Axes of F0c centers are now (should be near 0): " + zAxesStr);
            }
        }

        private string RadToDegStr(double rad)
        {
            return (rad * 180 / pi).ToString("0.0") + "°";
        }

        private IEnumerable<AtomSite> JoinAsymetricUnits(Entry entry, IEnumerable<string> ids)
        {
            return ids.SelectMany(id => FilterAsymetricUnit(entry, id));
        }

        private void ImportRotor()
        {
            Console.WriteLine("import Rotor...");

            this.rotor = new EntityClassBuilder("rotor");
            this.rotor.BehaviorId = "atpSynthase.rotor";

            // Asymetric Unit Ids of F0c parts

            //<< Export Table
            /*
            var t = new Table();
            for (int i = 0; i < f0cIds.Length; i++)
            {
                var id = f0cIds[i];

                t[0, i] = id;

                var sites = FilterAsymetricUnit(entry1, id).ToArray();

                for (int l = 0; l < sites.Length; l++)
                {
                    var s = sites[l];

                    var value = s.Symbol + " " + s.SequenceId + " " + s.Component.Id;

                    t[l + 1, i] = value;
                }
            }

            t.SaveCsv(@"c:\temp\f0cs.csv");
            */
            //>>


            IEnumerable<AtomSite> axleAtoms = new AtomSite[0];

            foreach (var id in AxleIds)
            {
                axleAtoms = axleAtoms.Concat(FilterAsymetricUnit(entry1, id));
            }

            this.rotor.AddAtoms(axleAtoms, atomSite => atomSite.Symbol, ImportAtomSiteVector);

            // Binding Site for ATP Synthase
            this.rotor.BindingSite("atpSynthaseSite", BindingSite.DirectBindingLocRot);

            // Binding Sites for F0cs

            var f0cCenters =
                F0cIds
                    .Select(id => FilterAsymetricUnit(entry1, id)) // get AtomSites from Asymetric Unit
                    .Select(atomSites => ImportUtils.CalcCenter(atomSites.Select(ImportAtomSiteVector))) // get center from that AtomSites
                    .ToArray(); // as array

            var angleDelta = 2 * pi / f0cCenters.Length;
            var rotationDelta = Quaternion.FromYawPitchRoll(0, 0, angleDelta).GetInverted();

            int counter = 1;
            var rotation = Quaternion.FromYawPitchRoll(0, 0, 39f * pi / 180f); // start with 39° offset since the data are not 100% aligned

            foreach (var f0cCenter in f0cCenters)
            {
                var id = "f0c_" + (counter++).ToString();
                rotation = Quaternion.CombineRotation(rotation, rotationDelta);

                this.rotor.BindingSite(
                    id,
                    new LocRotStatic(
                        f0cCenter,
                        rotation
                    )
                );
            }
        }

        private IEnumerable<Vector> CenteredVectorsOfAsymetricUnit(Entry entry, string asymetricUnitId)
        {
            return CenterVectors(FilterAsymetricUnit(entry, asymetricUnitId).Select(ImportAtomSiteVector));
        }

        private IEnumerable<Vector> CenterVectors(IEnumerable<Vector> vectors)
        {
            var center = ImportUtils.CalcCenter(vectors);
            return vectors.Select(v => v - center);
        }

        private void ImportF0c()
        {
            Console.WriteLine("import F0c...");

            this.f0c = new EntityClassBuilder("f0c");
            this.f0c.BehaviorId = "atpSynthase.f0c";
            this.f0c.DefaultColor = new Color(0.3f, 0.3f, 0.7f);

            // Asymetric Unit Id of F0c at Stator (at entry1)
            string f0cId = "M";
            string f0cIdOtherSide = "R";

            // get both
            var f0cAtStator = CenteredVectorsOfAsymetricUnit(entry1, f0cId).ToArray();
            var f0cOtherSide = CenteredVectorsOfAsymetricUnit(entry1, f0cIdOtherSide).ToArray();
            var f0cOtherSideRotation = Quaternion.FromYawPitchRoll(0, 0, pi);

            // get atoms
            var f0cAtoms = FilterAsymetricUnit(entry1, f0cId).Select(AtomSiteToAtom).ToArray();

            // just to be sure
            Assert(f0cAtoms.Length == f0cAtStator.Length);
            Assert(f0cAtoms.Length == f0cOtherSide.Length);
            AssertEqualItems(
                FilterAsymetricUnit(entry1, f0cId).Select(atomSite => atomSite.Symbol),
                FilterAsymetricUnit(entry2, f0cId).Select(atomSite => atomSite.Symbol)
            );

            // create conformations

            for (int i = 0; i < f0cAtoms.Length; i++)
            {
                var symbol = f0cAtoms[i].Element.Symbol;

                this.f0c.SetConformation("tensed");
                this.f0c.CurrentConformation.Color = new Color(0.4f, 0.4f, 0.6f);

                this.f0c.AddAtom(
                    symbol,
                    f0cAtStator[i]
                );

                this.f0c.SetConformation("relaxed");
                this.f0c.CurrentConformation.Color = new Color(0.3f, 0.3f, 0.7f);

                this.f0c.AddAtom(
                    symbol,
                    f0cOtherSideRotation.Rotate(f0cOtherSide[i])
                );
            }

            var rotorSiteLocRot = BindingSite.DirectBindingLocRot;

            var protonSiteLocRot = new LocRotStatic(
                new Vector(0, 7, 0), // << just an estimation
                Quaternion.Identity
            );

            this.f0c.SetConformation("tensed");
            this.f0c.BindingSite("rotorSite", rotorSiteLocRot);
            this.f0c.BindingSite("protonSite", protonSiteLocRot);
            this.f0c.SetConformation("relaxed");
            this.f0c.BindingSite("rotorSite", rotorSiteLocRot);
            this.f0c.BindingSite("protonSite", protonSiteLocRot);

        }

        private void ImportStator()
        {
            this.stator = new EntityClassBuilder("stator");
            this.stator.BehaviorId = "atpSynthase.stator";
            this.stator.DefaultColor = new Color(1, 0, 0);

            var atomSites1 = JoinAsymetricUnits(entry1, StatorIds).ToArray();
            var atomSites2 = JoinAsymetricUnits(entry2, StatorIds).ToArray();
            var atomSites3 = JoinAsymetricUnits(entry3, StatorIds).ToArray();

            // just to be sure
            AssertEqual(atomSites1.Length, atomSites2.Length);
            AssertEqual(atomSites1.Length, atomSites3.Length);
            AssertEqualItems(atomSites1.Select(a => a.Symbol), atomSites2.Select(a => a.Symbol));
            AssertEqualItems(atomSites1.Select(a => a.Symbol), atomSites3.Select(a => a.Symbol));

            // create conformations

            Action<string, IEnumerable<AtomSite>> addConformation = (name, atomSites) =>
            {
                this.stator.SetConformation(name);
                this.stator.AddAtoms(atomSites, a => a.Symbol, ImportAtomSiteVector);
                this.stator.BindingSite("atpSynthaseSite", BindingSite.DirectBindingLocRot);
            };

            addConformation("c0", atomSites1);
            addConformation("c4", atomSites2);
            addConformation("c7", atomSites3);

            // interpolate missing conformaitons
            this.stator.InterpolateConformation(1, "c1", "c0", "c4", 0.25f);
            this.stator.InterpolateConformation(2, "c2", "c0", "c4", 0.50f);
            this.stator.InterpolateConformation(3, "c3", "c0", "c4", 0.75f);

            this.stator.InterpolateConformation(5, "c5", "c4", "c7", 0.33f);
            this.stator.InterpolateConformation(6, "c6", "c4", "c7", 0.67f);

            this.stator.InterpolateConformation(8, "c8", "c7", "c0", 0.33f);
            this.stator.InterpolateConformation(9, "c9", "c7", "c0", 0.67f);
        }

        private EntityClassBuilder ImportF1Unit(string entityClassId, string asymmetricUnitId, Color color, string behaviorId, string atpAsymmetricId = null)
        {
            EntityClassBuilder builder = new EntityClassBuilder(entityClassId);
            builder.DefaultColor = color;
            builder.BehaviorId = behaviorId;

            var atomSites1 = FilterAsymetricUnit(entry1, asymmetricUnitId);
            var atomSites2 = FilterAsymetricUnit(entry2, asymmetricUnitId);
            var atomSites3 = FilterAsymetricUnit(entry3, asymmetricUnitId);

            var center = ImportUtils.CalcCenter(atomSites1.Select(ImportAtomSiteVector));

            // add conformations
            Action<string, IEnumerable<AtomSite>> addConformation = (name, atomSites) =>
            {
                builder.SetConformation(name);
                builder.AddAtoms(atomSites, a => a.Symbol, v => ImportAtomSiteVector(v) - center);
                builder.BindingSite("atpSynthaseSite", BindingSite.DirectBindingLocRot);

                if (atpAsymmetricId != null)
                {
                    ImportF1aAdpBindingSite(builder, atpAsymmetricId, center);
                }
            };

            addConformation("c0", atomSites1);
            addConformation("c4", atomSites2);
            addConformation("c7", atomSites3);

            // interpolate missing conformaitons
            builder.InterpolateConformation(1, "c1", "c0", "c4", 0.25f);
            builder.InterpolateConformation(2, "c2", "c0", "c4", 0.50f);
            builder.InterpolateConformation(3, "c3", "c0", "c4", 0.75f);

            builder.InterpolateConformation(5, "c5", "c4", "c7", 0.33f);
            builder.InterpolateConformation(6, "c6", "c4", "c7", 0.67f);

            builder.InterpolateConformation(8, "c8", "c7", "c0", 0.33f);
            builder.InterpolateConformation(9, "c9", "c7", "c0", 0.67f);

            return builder;
        }

        private void ImportF1aAdpBindingSite(EntityClassBuilder f1a, string atpAsymmetricId, Vector f1aUnitCenter)
        {
            var atpAtomSites = FilterAsymetricUnit(entry1, atpAsymmetricId);

            var position = ImportAtomSiteVector(atpAtomSites.ElementAt(4)) - f1aUnitCenter; // at Element 4 is the BindingSite

            // TODO consider different conformations
            // TODO consider rotation

            f1a.BindingSite("adpSite", new LocRotStatic(position, Quaternion.Identity));
            f1a.BindingSite("prSite", new LocRotStatic(position, Quaternion.Identity));

            var q = Quaternion.FromVectorRotation(Sensor.SenseVector, new Vector(f1aUnitCenter.X, f1aUnitCenter.Y, 0));
            f1a.Sensor("sensor", 80, 2f, new LocRotStatic(position, q));

            f1a.Marker("releaseMarker",
                new LocRotStatic(
                    position + q.Rotate(Sensor.SenseVector * 60),
                    q
                )
            );
        }

        private void ImportAtpSynthase_()
        {
            this.atpSynthase = new EntityClassBuilder("atpSynthase");
            this.atpSynthase.BehaviorId = "atpSynthase.atpSynthase";

            this.atpSynthase.BindingSite("rotorSite", LocRot.Zero);
            this.atpSynthase.BindingSite("statorSite", LocRot.Zero);

            int counter;
            // F1a Binding Sites
            counter = 1;
            foreach (var id in this.F1aIds)
            {
                var position = ImportUtils.CalcCenter(FilterAsymetricUnit(entry1, id).Select(ImportAtomSiteVector));
                var bindingSiteId = "f1a_" + (counter++).ToString();

                this.atpSynthase.BindingSite(bindingSiteId, new LocRotStatic(position, Quaternion.Identity));
            }

            // F1b Binding Sites
            counter = 1;
            foreach (var id in this.F1bIds)
            {
                var position = ImportUtils.CalcCenter(FilterAsymetricUnit(entry1, id).Select(ImportAtomSiteVector));
                var bindingSiteId = "f1b_" + (counter++).ToString();

                this.atpSynthase.BindingSite(bindingSiteId, new LocRotStatic(position, Quaternion.Identity));
            }


            this.atpSynthase.Marker("intraCellularEjectMarker", new LocRotStatic(new Vector(0, 180, 200), Quaternion.Identity));

            this.atpSynthase.Sensor("extraCellularSensor", 100, 2,
                new LocRotStatic(
                    new Vector(0, 30, -15),
                    Quaternion.FromYawPitchRoll(0, -pi / 2, 0)
                )
            );
        }

        private void ImportAtpSynthaseReference()
        {
            Func<AtomSite, Vector> importFunc =
                //atomSite => AtomSiteToVectorRaw(atomSite); // original
                //atomSite => AtomSiteToVectorRaw(atomSite) - this.importOffset; // centered, without rotation
                atomSite => ImportAtomSiteVector(atomSite); // centered and rotated



            var builder = new EntityClassBuilder("atpSynthaseReference");
            builder.AddAtoms(entry1.AtomSites, atomSite => atomSite.Symbol, importFunc);

            var entityClass = builder.Create();
            this.Environment.AddEntityClass(entityClass);
            var entity = this.Environment.AddEntity(entityClass, false);
            entity.Compound.Fix(LocRot.Zero);
        }
    }
}
