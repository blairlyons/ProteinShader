using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model.Behaviors;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Markers
{
    /// <summary>
    /// Sensor of an <see cref="Entity"/>.
    /// Can be used to detect other <see cref="Entity"/>s that are near by.
    /// This information can be used e.g. in <see cref="EntityBehavior"/> to initiate bindings or induce conformation changes.
    /// </summary>
    public class Sensor : Marker
    {
        public Sensor(Entity owner, SensorClass sensorClass, LocRot localLocRot)
            : base(owner, sensorClass, localLocRot) { }

        public SensorClass SensorClass { get { return (SensorClass)this.Class; } }

        private Cone GetCone()
        {
            var lr = this.LocRot;
            var basementCircleCenter = lr.Location + lr.Rotation.Rotate(Sensor.SenseVector * this.SensorClass.Range);
            var cone = new Cone(lr.Location, basementCircleCenter, this.SensorClass.ApertureAngle);

            return cone;
        }

        public IEnumerable<TBehavior> FindEntitiesWithBehavior<TBehavior>(Func<TBehavior, bool> filter)
            where TBehavior : EntityBehavior
        {
            var cone = GetCone();

            return
                from b in
                    (from e in this.Owner.Environment.Entities
                     where (e.Behavior is TBehavior) && e != this.Owner && cone.IsLyingInCone(e.LocRot.Location)
                     select (TBehavior)e.Behavior)
                where
                    filter(b)
                select b;
        }

        public IEnumerable<Entity> FindEntitiesOfClass(string entityClassId, Func<Entity, bool> filter)
        {
            var cone = GetCone();

            return
                (from e in this.Owner.Environment.Entities
                 where e != this.Owner && e.Class.Id == entityClassId && cone.IsLyingInCone(e.LocRot.Location) && filter(e)
                 select e);
        }

        public bool FindNearestEntityOfClass(string entityClassId, Func<Entity, bool> filter, out Entity nearest)
        {
            return Nearest(this.LocRot.Location, FindEntitiesOfClass(entityClassId, filter), e => e.LocRot.Location, out nearest);
        }

        public bool FindNearestEntityOfClassWithFreeSite(string entityClassId, string freeBindingSite, out Entity nearest)
        {
            return FindNearestEntityOfClass(entityClassId, e => e.BindingSiteById(freeBindingSite).IsFree, out nearest);
        }

        public static bool Nearest<T>(Vector referenceLocation, IEnumerable<T> collection, Func<T, Vector> locationFunc, out T nearest)
        {
            // find nearest to sensor

            nearest = default(T);
            float nearestDistance = float.MaxValue;
            bool found = false;

            foreach (var item in collection)
            {
                var itemLocation = locationFunc(item);
                var distance = Vector.Distance(itemLocation, referenceLocation);

                if (distance < nearestDistance)
                {
                    nearest = item;
                    nearestDistance = distance;

                    found = true;
                }
            }

            return found;
        }

        public bool FindNearestWithBehavior<TBehavior>(Func<TBehavior, bool> filter, out TBehavior nearest)
            where TBehavior : EntityBehavior
        {
            return Nearest(this.LocRot.Location, FindEntitiesWithBehavior<TBehavior>(filter), b => b.Owner.LocRot.Location, out nearest);
        }

        public IEnumerable<Entity> FindEntities()
        {
            var cone = GetCone();

            return
                from e in this.Owner.Environment.Entities
                where cone.IsLyingInCone(e.LocRot.Location)
                select e;
        }

        public static readonly Func<Entity, bool> NoFilter = (entity) => true;

        // Geometric Info

        public static readonly Vector SenseVector = Vector.AxisY;
    }
}
