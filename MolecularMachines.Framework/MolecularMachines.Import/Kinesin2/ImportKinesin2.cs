using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Builders;
using MolecularMachines.Framework.Model.Colliders;
using MolecularMachines.Framework.Model.Compartments;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Import;
using MolecularMachines.Import.Utils;
using PdbXReader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.Kinesin2
{
    class ImportKinesin2 : ImportBase
    {
        private Compartment compartment;

        private Entry entryTubulin;
        private Entry entryKinesin;

        private EntityClassBuilder tubulinLoop;
        private EntityClassBuilder tubulinAlpha;
        private EntityClassBuilder tubulinBeta;

        private EntityClassBuilder kinesin;
        private EntityClassBuilder kinesinHead1;
        private EntityClassBuilder kinesinHead2;
        private EntityClassBuilder kinesinNeckLinker1;
        private EntityClassBuilder kinesinNeckLinker2;

        public override void Import()
        {
            var pdbsPath = this.RootPath;

            this.Environment.Name = "Kinesin";

            this.compartment = new Compartment("main", new Vector(-10000, -10000, -10000), new Vector(10000, 10000, 10000));
            this.Environment.AddCompartment(compartment);

            ImportTubulin();
            ImportKinesin_();

            //
            // Instances
            //

            // tubulinLoop
            var tubulinLoopClass = this.tubulinLoop.Create();
            this.Environment.AddEntityClass(tubulinLoopClass);

            // tubulinAlpha
            var tubulinAlphaClass = this.tubulinAlpha.Create();
            this.Environment.AddEntityClass(tubulinAlphaClass);

            // tubulinBeta
            var tubulinBetaClass = this.tubulinBeta.Create();
            this.Environment.AddEntityClass(tubulinBetaClass);

            // Entities


        }

        public void CreatePoster()
        {
            var loops = CreateMicrotubule(
                new LocRotStatic(
                    new Vector(-900, -200, -1000),
                    Quaternion.FromYawPitchRollDeg(0.484f, 39.2f, 0)
                 ),
                30
             );

            var kinesinEntity = CreateKinesin();
            kinesinEntity.Compound.Float(new LocRotStatic(new Vector(-10, 35, 10), Quaternion.Identity));
            loops[13].BindingSiteById("tubulin4_beta").OtherEntity.BindingSiteById("dock").InstantBind(kinesinEntity.BindingSiteById("neckLinker1").OtherEntity.BindingSiteById("head").OtherEntity.BindingSiteById("tubulin"));
        }

        public void CreateTestEnvironment()
        {
            var loops = CreateMicrotubule(
                new LocRotStatic(
                    new Vector(0, -200, -450),
                    Quaternion.FromYawPitchRollDeg(0, 0, 0)
                 ),
                30
             );

            var kinesinEntity1 = CreateKinesin();
            kinesinEntity1.Compound.Float(new LocRotStatic(new Vector(-10, 40, 10), Quaternion.Identity));
            //loops[5].BindingSiteById("tubulin4_beta").OtherEntity.BindingSiteById("dock").InstantBind(kinesinEntity1.BindingSiteById("neckLinker1").OtherEntity.BindingSiteById("head").OtherEntity.BindingSiteById("tubulin"));

            /*var kinesinEntity2 = CreateKinesin();
            kinesinEntity2.Compound.Float(new LocRotStatic(new Vector(-10, 35, 10), Quaternion.Identity));
            loops[7].BindingSiteById("tubulin3_beta").OtherEntity.BindingSiteById("dock").InstantBind(kinesinEntity2.BindingSiteById("neckLinker1").OtherEntity.BindingSiteById("head").OtherEntity.BindingSiteById("tubulin"));
            */
        }

        private List<Framework.Model.Entities.Entity> CreateMicrotubule(LocRot start, int loopCount)
        {
            var loops = new List<Framework.Model.Entities.Entity>();

            if (loopCount > 0)
            {
                var firstTubulinLoop = CreateTubulinLoop();
                firstTubulinLoop.Compound.Fix(
                    start
                );

                loops.Add(firstTubulinLoop);

                var lastTubulinLoop = firstTubulinLoop;

                for (int i = 0; i < loopCount - 1; i++)
                {
                    var tl = CreateTubulinLoop();
                    lastTubulinLoop.BindingSiteById("nextLoop").InstantBind(tl.BindingSiteById("previousLoop"));

                    loops.Add(tl);

                    lastTubulinLoop = tl;
                }
            }

            return loops;
        }

        private Vector kinesinImportCenter = new Vector(-26.8f, 26.4f, 108.1f);
        private Quaternion kinesinImportRotation = Quaternion.FromYawPitchRoll(-51 * pi / 180f, 0, pi);

        private Vector KinesinAtomSiteToVector(AtomSite atomSite)
        {
            var v = AtomSiteToVector(atomSite);
            v = v - kinesinImportCenter;
            v = kinesinImportRotation.Rotate(v);
            return v;
        }

        private void ImportKinesin_()
        {
            this.entryKinesin = LoadCif(@"3kin_kinesin.cif");

            this.kinesinHead1 = new EntityClassBuilder("kinesinHead1");
            this.kinesinNeckLinker1 = new EntityClassBuilder("kinesinNeckLinker1");
            this.kinesinHead2 = new EntityClassBuilder("kinesinHead2");
            this.kinesinNeckLinker2 = new EntityClassBuilder("kinesinNeckLinker2");

            ImportKinesinMonomer(this.kinesinHead1, this.kinesinNeckLinker1, "A", "B", 500, Quaternion.FromYawPitchRoll(-pi, 0, 0), Quaternion.Identity, Quaternion.Identity);
            ImportKinesinMonomer(this.kinesinHead2, this.kinesinNeckLinker2, "C", "D", 500, Quaternion.FromYawPitchRoll(pi, 0, 0), Quaternion.Identity, Quaternion.FromYawPitchRoll(pi, 0, 0)); //2.4f

            this.kinesin = new EntityClassBuilder("kinesin");
            this.kinesin.BehaviorId = "kinesin2.kinesin";
            this.kinesin.BindingSite(
                "neckLinker1",
                new LocRotStatic(
                    new Vector(0, 0, 0),
                    Quaternion.Identity
                )
            );
            this.kinesin.BindingSite(
                "neckLinker2",
                new LocRotStatic(
                    new Vector(0, 0, 0), //new Vector(1.2f, 3.2f, -16.7f),
                    Quaternion.Identity
                )
            );
        }

        private Framework.Model.Entities.Entity CreateKinesin()
        {
            var kinesinHead1Entity = CreateEntity(this.kinesinHead1);
            var kinesinNeckLinker1Entity = CreateEntity(this.kinesinNeckLinker1);
            var kinesinHead2Entity = CreateEntity(this.kinesinHead2);
            var kinesinNeckLinker2Entity = CreateEntity(this.kinesinNeckLinker2);
            var kinesinEntity = CreateEntity(this.kinesin);

            kinesinEntity.BindingSiteById("neckLinker1").InstantBind(kinesinNeckLinker1Entity.BindingSiteById("hip"));
            kinesinEntity.BindingSiteById("neckLinker2").InstantBind(kinesinNeckLinker2Entity.BindingSiteById("hip"));

            kinesinNeckLinker1Entity.BindingSiteById("head").InstantBind(kinesinHead1Entity.BindingSiteById("neckLinker"));
            kinesinNeckLinker2Entity.BindingSiteById("head").InstantBind(kinesinHead2Entity.BindingSiteById("neckLinker"));

            kinesinEntity.Compound.Fix(
                LocRot.Zero
            );

            return kinesinEntity;
        }

        private void ImportKinesinMonomer(EntityClassBuilder kinesinHead, EntityClassBuilder kinesinNeckLinker, string asymmetricUnit1, string asymmetricUnit2, int asymmetricUnit2Cutoff, Quaternion defaultRotation, Quaternion zipperTriggeredQuaternion, Quaternion tubulinQuaternion)
        {
            kinesinHead.BehaviorId = "kinesin2.head";
            kinesinHead.SetConformation("boundTight");

            kinesinHead.AddAtoms(
                FilterAsymetricUnit(this.entryKinesin, asymmetricUnit1),
                atomSite => atomSite.Symbol,
                KinesinAtomSiteToVector
            );

            var atomIndex = 0;
            Vector lastAtomPosition = Vector.Zero;
            foreach (var atomSite in this.entryKinesin.AtomSites)
            {
                if (atomSite.AsymmetricUnit.Id == asymmetricUnit2)
                {
                    var atomPosition = KinesinAtomSiteToVector(atomSite);

                    if (atomIndex < asymmetricUnit2Cutoff)
                    {
                        kinesinHead.AddAtom(atomSite.Symbol, atomPosition);
                    }
                    else
                    {
                        kinesinNeckLinker.AddAtom(atomSite.Symbol, atomPosition);
                    }

                    if (atomIndex == asymmetricUnit2Cutoff)
                    {
                        kinesinHead.BindingSite("neckLinker",
                            new LocRotStatic(
                                atomPosition,
                                defaultRotation
                            )
                        );

                        kinesinNeckLinker.BindingSite("head",
                            new LocRotStatic(
                                atomPosition,
                                BindingSite.BindingQuaternion
                            )
                        );
                    }

                    lastAtomPosition = atomPosition;
                    atomIndex++;
                }
            }

            kinesinNeckLinker.BindingSite("hip",
                new LocRotStatic(
                    lastAtomPosition,
                    BindingSite.BindingQuaternion
                )
            );

            // center
            kinesinHead.ReCenter();
            kinesinNeckLinker.ReCenter();

            // add tubulin binding stuff

            kinesinHead.BindingSite("tubulin",
                new LocRotStatic(
                    new Vector(0, 0, 0),
                    tubulinQuaternion
                )
            );

            kinesinHead.Sensor("tubulinSensor",
                70f,
                1.8f,
                new LocRotStatic(
                    new Vector(0, 0, 0),
                    Quaternion.FromVectorRotation(Sensor.SenseVector, new Vector(0, -1, 0))
                )
            );

            // add conformaitons
            kinesinHead.CopyCurrentConformation("boundWeak");

            kinesinHead.BindingSite("tubulin",
                new LocRotStatic(
                    new Vector(0, -10, 0),
                    tubulinQuaternion
                )
            );

            kinesinHead.SetConformation("boundTight");
            kinesinHead.CopyCurrentConformation("neckZipperTriggered");
            kinesinHead.CurrentConformation.Color = new Color(1, 1, 0.5f);
            kinesinHead.BindingSite("neckLinker",
                new LocRotStatic(
                    kinesinHead.GetMarkerLocRot("neckLinker").Location,
                    zipperTriggeredQuaternion
                )
            );

            kinesinHead.CopyCurrentConformation("boundWeakNeckZipperTriggered");
            kinesinHead.CurrentConformation.Color = kinesinHead.DefaultColor;
            kinesinHead.BindingSite("tubulin",
                new LocRotStatic(
                    new Vector(0, -10, 0),
                    tubulinQuaternion
                )
            );

            kinesinHead.SetConformation("boundWeak");
            kinesinHead.CopyCurrentConformation("free");
        }

        private Framework.Model.Entities.Entity CreateTubulinLoop()
        {
            var tubulinLoop = CreateEntity(this.tubulinLoop);

            foreach (var bindingSite in tubulinLoop.BindingSites)
            {
                if (bindingSite.Id.Contains("alpha"))
                {
                    var tubulinAlpha = CreateEntity(this.tubulinAlpha);
                    bindingSite.InstantBind(tubulinAlpha.BindingSiteById("tubulinCenter"));
                }
                else if (bindingSite.Id.Contains("beta"))
                {
                    var tubulinBeta = CreateEntity(this.tubulinBeta);
                    bindingSite.InstantBind(tubulinBeta.BindingSiteById("tubulinCenter"));
                }
            }

            return tubulinLoop;
        }

        private Vector AtomSiteToVector(AtomSite atomSite)
        {
            return new Vector(atomSite.X, atomSite.Y, atomSite.Z);
        }

        private void ImportTubulin()
        {
            this.entryTubulin = LoadCif(@"5syc_microtubule.cif");

            var tubulinCenter = ImportUtils.CalcCenter(this.entryTubulin.AtomSites.Select(AtomSiteToVector));

            var subBindingQuaternion = BindingSite.BindingQuaternion;
            // Alpha
            this.tubulinAlpha = new EntityClassBuilder("tubulinAlpha");
            this.tubulinAlpha.DefaultColor = new Color(0.80f, 0.35f, 0.25f);
            this.tubulinAlpha.CurrentConformation.Collider = Collider.Empty;
            this.tubulinAlpha.AddAtoms(FilterEntityId(this.entryTubulin, "1"), atomSite => atomSite.Symbol, AtomSiteToVector);
            var tubulinAlphaCenter = this.tubulinAlpha.CurrentConformation.Atoms.CalcCenter();
            this.tubulinAlpha.SetOrigin(tubulinAlphaCenter);
            tubulinAlpha.BindingSite(
                "tubulinCenter",
                new LocRotStatic(
                    tubulinCenter - tubulinAlphaCenter,
                    subBindingQuaternion
                )
            );

            // Beta
            this.tubulinBeta = new EntityClassBuilder("tubulinBeta");
            this.tubulinBeta.DefaultColor = new Color(0.95f, 0.75f, 0.35f);
            this.tubulinBeta.CurrentConformation.Collider = Collider.Empty;
            this.tubulinBeta.AddAtoms(FilterEntityId(this.entryTubulin, "2"), atomSite => atomSite.Symbol, AtomSiteToVector);
            var tubulinBetaCenter = this.tubulinBeta.CurrentConformation.Atoms.CalcCenter();
            this.tubulinBeta.SetOrigin(tubulinBetaCenter);
            tubulinBeta.BindingSite(
                "tubulinCenter",
                new LocRotStatic(
                    tubulinCenter - tubulinBetaCenter,
                    subBindingQuaternion
                )
            );

            tubulinBeta.BehaviorId = "kinesin2.tubulinBeta";
            tubulinBeta.BindingSite(
                "dock",
                new LocRotStatic(
                    //new Vector(27.6f, -21.4f, 20.5f),
                    new Vector(27.6f, -27.8f, 11.6f),
                    Quaternion.FromYawPitchRoll(0, 0, 1f)
                )
            );

            // Loop
            this.tubulinLoop = new EntityClassBuilder("tubulinLoop");
            this.tubulinLoop.CurrentConformation.Collider = Collider.Empty;
            int loopCount = 13;
            float radius = 100f;
            float loopDepth = 50f;

            var angleDelta = 2 * pi / loopCount;
            var rotationDelta = Quaternion.FromYawPitchRoll(0, 0, -angleDelta).GetInverted();
            var rotation = Quaternion.FromYawPitchRoll(0, 0, 0); // initial rotation

            this.tubulinLoop.BindingSite($"previousLoop", LocRot.Zero);
            this.tubulinLoop.BindingSite($"nextLoop", new LocRotStatic(
                new Vector(0, 0, 90),
                BindingSite.BindingQuaternion
            ));

            for (int i = 0; i < loopCount; i++)
            {
                rotation = Quaternion.CombineRotation(rotation, rotationDelta);

                var rotX = (float)Math.Cos(angleDelta * i) * radius;
                var rotY = (float)Math.Sin(angleDelta * i) * radius;
                var depth = (float)(loopDepth / loopCount * i);

                var tubulinLocRot = new LocRotStatic(
                    new Vector(rotX, rotY, depth),
                    rotation
                );

                this.tubulinLoop.BindingSite($"tubulin{i}_alpha", tubulinLocRot);
                this.tubulinLoop.BindingSite($"tubulin{i}_beta", tubulinLocRot);
            }
        }

        private void CreateReference(Entry entry, LocRot center)
        {
            var entryCenter = ImportUtils.CalcCenter(entry.AtomSites.Select(AtomSiteToVector));

            var mainBuilder = new EntityClassBuilder("reference_" + entry.Id);

            foreach (var pdbAsymUnit in entry.AsymetricUnits)
            {
                var builder = new EntityClassBuilder("reference_" + entry.Id + "_" + pdbAsymUnit.Id);

                builder.AddAtoms(FilterAsymetricUnit(entry, pdbAsymUnit.Id), atomSite => atomSite.Symbol, atomSite => AtomSiteToVector(atomSite) - entryCenter);

                var mmClass = builder.Create();
                this.Environment.AddEntityClass(mmClass);
                var entity = this.Environment.AddEntity(mmClass);
                entity.Compound.Fix(center);
            }
        }
    }
}
