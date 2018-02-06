using MolecularMachines.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.Compounds;
using UnityEngine;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Framework.Model.Colliders;

namespace Assets.MolecularMachinesAdapter.Adapters
{
    public class MMEnvironmentUnity : MMEnvironment
    {
        protected override Compound OnCreateCompound(Entity rootEntity)
        {
            return new CompoundUnity(this, rootEntity);
        }

        protected override Entity OnCreateEntity(EntityClass entityClass)
        {
            RequireReupload();
            return new EntityUnity(this, entityClass);
        }

        protected override void OnRemoveEntity(Entity entity)
        {
            base.OnRemoveEntity(entity);
            RequireReupload();
        }

        private bool requireReupload = true;
        private void RequireReupload()
        {
            this.requireReupload = true;
        }

        //
        // CellView stuff
        //

        protected override void OnReset()
        {
            base.OnReset();

            SceneManager.Instance.ClearScene();
        }

        public void Reupload()
        {
            this.requireReupload = false;

            DateTime start = DateTime.Now;

            SceneManager.Instance.ClearScene();

            foreach (var structure in this.EntityStructures)
            {
                var structureClass = structure.Class;

                SceneManager.Instance.AddIngredient(
                    structure.Id,
                    ComputeBounds(structureClass),
                    GetVector4FromAtomPositions(structure).ToList(),
                    structure.CurrentColor.ToUnity()
                );
            }

            foreach (var entity in this.Entities)
            {
                SceneManager.Instance.AddIngredientInstance(
                    entity.Structure.Id,
                    entity.LocRot.Location.ToUnity(),
                    entity.LocRot.Rotation.ToUnity()
                );
            }

            SceneManager.Instance.UploadAllData();

            DateTime end = DateTime.Now;
            Debug.Log("Reupload: uploaded " + this.EntityStructures.Count() + " structures and " + this.Entities.Count() + " entities. took " + (end - start).TotalMilliseconds + "ms");
        }

        private IEnumerable<Vector4> GetVector4FromAtomPositions(EntityStructure structure)
        {
            var count = structure.Class.Atoms.Count;
            var result = new List<Vector4>(count);

            if (count != structure.CurrentAtomPositions.Length)
            {
                throw new Exception("StructureClass.Atoms.Count and Structure.CurrentAtomPositions.Length mismatch (" + count + " vs. " + structure.CurrentAtomPositions.Length + ")");
            }

            for (int i = 0; i < count; i++)
            {
                var atom = structure.Class.Atoms[i];
                var position = structure.CurrentAtomPositions[i];

                yield return
                    new Vector4(
                        position.X,
                        position.Y,
                        position.Z,
                        atom.Element.VdWRadius * 10f // nm to Angstroms
                    );
            }
        }

        private Bounds ComputeBounds(EntityStructureClass structureClass)
        {
            var bbMin = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            var bbMax = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

            foreach (var conformation in structureClass.Conformations)
            {
                foreach (var atomPosition in conformation.AtomPositions)
                {
                    var p = atomPosition.ToUnity();
                    bbMin = Vector3.Min(bbMin, p);
                    bbMax = Vector3.Max(bbMax, p);
                }
            }

            var bbSize = bbMax - bbMin;
            var bbCenter = bbMin + bbSize * 0.5f;

            return new Bounds(bbCenter, bbSize);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (this.requireReupload)
            {
                this.requireReupload = false;
                this.Reupload();
            }

            var screenManager = SceneManager.Instance;
            bool uploadAtomPositions = false;

            var entities = this.Entities.ToArray(); // TODO this is only a temporary solution

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];

                // update LocRot
                var locRot = entity.LocRot;
                screenManager.UpdateProteinInstanceLocRot(i, locRot.Location.ToUnity(), locRot.Rotation.ToUnity());
            }

            var entityStructures = this.EntityStructures.ToArray();
            for (int i = 0; i < entityStructures.Length; i++)
            {
                var structure = entityStructures[i];

                // update Structure
                structure.ExecuteIfDirty(
                    () =>
                    {
                        // some bodies do not have atoms. Do only update positions and retransmit
                        // if structure has atoms
                        if (structure.CurrentAtomPositions.Length > 0)
                        {
                            screenManager.UpdateProteinAtoms(i, GetVector4FromAtomPositions(structure));
                            screenManager.UpdateProteinColor(i, structure.CurrentColor.ToUnity());
                            uploadAtomPositions = true;
                        }
                    }
                );
            }

            screenManager.UploadProteinInstancePositionsRotations();
            if (uploadAtomPositions)
            {
                screenManager.UploadProteinAtoms();
                screenManager.UploadProteinColors();
            }
        }

        public void DrawGizmos()
        {
            foreach (var compartment in this.Compartments)
            {
                Gizmos.color = UnityEngine.Color.gray;

                var c = TransformToUnity((compartment.CornerMin + compartment.CornerMax) / 2f);
                var d = TransformToUnity(compartment.CornerMax - compartment.CornerMin);

                Gizmos.DrawWireCube(
                    c,
                    d
                );

            }

            foreach (var entity in this.Entities)
            {
                // Sensors

                Gizmos.color = UnityEngine.Color.blue;
                foreach (var sensor in entity.Sensors)
                {
                    var locRot = sensor.LocRot;

                    var circleRadius = sensor.SensorClass.Range * Mathf.Tan(sensor.SensorClass.ApertureAngle / 2f);
                    var circle = new MolecularMachines.Framework.Geometry.Vector[10];
                    for (int i = 0; i < circle.Length; i++)
                    {
                        var angle = Mathf.PI * 2 * i / circle.Length;

                        circle[i] = new MolecularMachines.Framework.Geometry.Vector(
                            Mathf.Cos(angle) * circleRadius,
                            0,
                            Mathf.Sin(angle) * circleRadius
                        );
                    }

                    var from = TransformToUnity(locRot.Location);
                    var to =
                        TransformToUnity(
                            locRot.Location +
                            locRot.Rotation.Rotate(
                                Sensor.SenseVector * sensor.SensorClass.Range
                            )
                        );

                    Vector3? firstCircleVectorUnity = null;
                    Vector3? lastCircleVectorUnity = null;
                    Vector3 currentCircleVectorUnity = Vector3.zero;

                    foreach (var circleVector in circle)
                    {
                        currentCircleVectorUnity = TransformToUnity(
                            locRot.Location +
                            locRot.Rotation.Rotate(
                                Sensor.SenseVector * sensor.SensorClass.Range + circleVector
                            )
                        );

                        if (lastCircleVectorUnity != null)
                        {
                            Gizmos.DrawLine(lastCircleVectorUnity.Value, currentCircleVectorUnity);
                        }

                        Gizmos.DrawLine(from, currentCircleVectorUnity);

                        Gizmos.color = UnityEngine.Color.gray;
                        Gizmos.DrawLine(to, currentCircleVectorUnity);
                        Gizmos.color = UnityEngine.Color.blue;

                        if (firstCircleVectorUnity == null)
                        {
                            firstCircleVectorUnity = currentCircleVectorUnity;
                        }

                        lastCircleVectorUnity = currentCircleVectorUnity;
                    }

                    if (firstCircleVectorUnity != null)
                    {
                        Gizmos.DrawLine(firstCircleVectorUnity.Value, currentCircleVectorUnity);
                    }

                    Gizmos.DrawLine(
                        from,
                        to
                    );
                }

                // Binding Sites

                Gizmos.color = UnityEngine.Color.green;
                foreach (var bindingSite in entity.BindingSites)
                {
                    var locRot = bindingSite.LocRot;

                    var from = TransformToUnity(locRot.Location);
                    var to =
                        TransformToUnity(
                            locRot.Location +
                            locRot.Rotation.Rotate(
                                BindingSite.BindingVector * 3f
                            )
                        );

                    Gizmos.DrawLine(from, to);

                    Gizmos.DrawSphere(
                        from,
                        0.1f
                    );
                    Gizmos.DrawSphere(
                        to,
                        0.03f
                    );
                }

                // Other Markers

                Gizmos.color = UnityEngine.Color.magenta;
                foreach (var marker in entity.OtherMarkers)
                {
                    var locRot = marker.LocRot;

                    var from = TransformToUnity(locRot.Location);
                    var to =
                        TransformToUnity(
                            locRot.Location +
                            locRot.Rotation.Rotate(
                                MolecularMachines.Framework.Geometry.Vector.AxisY * 3f
                            )
                        );

                    Gizmos.DrawLine(from, to);

                    Gizmos.DrawSphere(
                        from,
                        0.1f
                    );
                    Gizmos.DrawSphere(
                        to,
                        0.03f
                    );
                }

                // Collider

                if (MMM.Instance.collidersActive)
                {
                    Gizmos.color = UnityEngine.Color.yellow;
                    var collider = entity.Structure.CurrentConformation.Collider;
                    if (collider != null)
                    {
                        foreach (ColliderGeometry g in collider)
                        {
                            var sp = (g as SphereColliderGeometry);
                            if (sp != null)
                            {
                                Gizmos.DrawWireSphere(
                                    TransformToUnity(
                                        sp.Center + entity.LocRot.Location
                                    ),
                                    TransformToUnity(sp.Radius)
                                );
                            }
                        }
                    }
                }
            }
        }

        private float TransformToUnity(float x)
        {
            return x * PersistantSettings.Instance.Scale;
        }

        public static Vector3 TransformToUnity(MolecularMachines.Framework.Geometry.Vector vector)
        {
            return (vector * PersistantSettings.Instance.Scale).ToUnity();
        }
    }
}
