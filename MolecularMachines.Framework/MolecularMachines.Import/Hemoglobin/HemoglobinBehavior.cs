using MolecularMachines.Framework.Logging;
using MolecularMachines.Framework.Model.Behaviors;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Framework.Model.Trajectories;
using MolecularMachines.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.Hemoglobin
{
    [EntityBehaviorId("hemoglobin.hemoglobin")]
    public class HemoglobinBehavior : EntityBehavior
    {
        [BindingSite("site1")]
        public BindingSite Site1 { get; set; }

        [BindingSite("site2")]
        public BindingSite Site2 { get; set; }

        [BindingSite("site3")]
        public BindingSite Site3 { get; set; }

        [BindingSite("site4")]
        public BindingSite Site4 { get; set; }

        [Sensor("sensor1")]
        public Sensor Sensor1 { get; set; }

        [Sensor("sensor2")]
        public Sensor Sensor2 { get; set; }

        [Sensor("sensor3")]
        public Sensor Sensor3 { get; set; }

        [Sensor("sensor4")]
        public Sensor Sensor4 { get; set; }

        private static TimeSpan avgTimeSpan = TimeSpan.FromSeconds(2.0);
        private static int avgCapacity = 120;
        private AverageOverTime sensor1O2Avg = new AverageOverTime(avgTimeSpan, avgCapacity);
        private AverageOverTime sensor1COAvg = new AverageOverTime(avgTimeSpan, avgCapacity);
        private AverageOverTime sensor2O2Avg = new AverageOverTime(avgTimeSpan, avgCapacity);
        private AverageOverTime sensor2COAvg = new AverageOverTime(avgTimeSpan, avgCapacity);
        private AverageOverTime sensor3O2Avg = new AverageOverTime(avgTimeSpan, avgCapacity);
        private AverageOverTime sensor3COAvg = new AverageOverTime(avgTimeSpan, avgCapacity);
        private AverageOverTime sensor4O2Avg = new AverageOverTime(avgTimeSpan, avgCapacity);
        private AverageOverTime sensor4COAvg = new AverageOverTime(avgTimeSpan, avgCapacity);

        private static float[] bindThresholdO2ByBoundCount = new float[]
        {
            2.50f, // 0% bound
            2.80f, // 25% bound
            3.20f, // 50% bound
            3.50f, // 75% bound
            3.00f // 100% bound (item not relevant)
        };

        private static float[] unbindThresholdO2ByBoundCount = new float[]
        {
            0.30f, // 0% bound (item not relevant)
            0.90f, // 25% bound
            1.40f, // 50% bound
            1.90f, // 75% bound
            2.00f // 100% bound
        };

        // Thresholds for CO is only 1/100 of O2

        private static float[] bindThresholdCoByBoundCount = new float[]
        {
            0.002f, // 0% bound
            0.003f, // 25% bound
            0.004f, // 50% bound
            0.006f, // 75% bound
            1.000f // 100% bound (item not relevant)
        };

        private static float[] unbindThresholdCoByBoundCount = new float[]
        {
            0.000f, // 0% bound (item not relevant)
            0.001f, // 25% bound
            0.002f, // 50% bound
            0.003f, // 75% bound
            0.005f // 100% bound
        };

        private int GetCountBound()
        {
            int count = 0;

            if (Site1.IsBound) { count++; }
            if (Site2.IsBound) { count++; }
            if (Site3.IsBound) { count++; }
            if (Site4.IsBound) { count++; }

            return count;
        }

        [State(InitialState = true)]
        public void Main()
        {
            var boundCount = GetCountBound();
            SetConformation(this.Owner.Class.StructureClass.Conformations[boundCount]);
            
            HandleSite(Site1, Sensor1, sensor1O2Avg, sensor1COAvg, boundCount);
            HandleSite(Site2, Sensor2, sensor2O2Avg, sensor2COAvg, boundCount);
            HandleSite(Site3, Sensor3, sensor3O2Avg, sensor3COAvg, boundCount);
            HandleSite(Site4, Sensor4, sensor4O2Avg, sensor4COAvg, boundCount);
        }

        public void HandleSite(BindingSite bindingSite, Sensor sensor, AverageOverTime o2Avg, AverageOverTime coAvg, int boundCount)
        {
            var o2 = sensor.FindEntitiesOfClass("o2", p => p.BindingSites[0].IsFree).ToArray();
            var co = sensor.FindEntitiesOfClass("co", p => p.BindingSites[0].IsFree).ToArray();

            o2Avg.Push(o2.Length);
            coAvg.Push(co.Length);
            
            if (bindingSite.IsFree)
            {
                var o2AvgValue = o2Avg.GetAvg();
                if (coAvg.GetAvg() >= bindThresholdCoByBoundCount[boundCount])
                {
                    InitiateBinding(co, bindingSite);
                }
                else if (o2AvgValue >= bindThresholdO2ByBoundCount[boundCount])
                {
                    Log.Info("o2Avg " + o2AvgValue + " > " + bindThresholdO2ByBoundCount[boundCount] + " -> bind");
                    InitiateBinding(o2, bindingSite);
                }
            }
            else
            {
                var boundClassId = bindingSite.OtherEntity.Class.Id;
                if (boundClassId == "co")
                {
                    // avg+1 because one is bound that is not in the scope of the sensor
                    if (coAvg.GetAvg() + 1 <= unbindThresholdCoByBoundCount[boundCount])
                    {
                        bindingSite.ReleaseBond(); // TODO trajectory?
                    }
                }
                else if (boundClassId == "o2")
                {
                    if (o2Avg.GetAvg() + 1 <= bindThresholdO2ByBoundCount[boundCount])
                    {
                        bindingSite.ReleaseBond(); // TODO trajectory?
                    }
                }
            }
        }

        private void InitiateBinding(Entity[] possibleEntities, BindingSite bindingSite)
        {
            // find nearest in possible entities
            Entity nearestEntity;
            if (Sensor.Nearest(bindingSite.LocRot.Location, possibleEntities, e => e.LocRot.Location, out nearestEntity))
            {
                bindingSite.InitiateBinding(nearestEntity.BindingSites[0]);

                var trajectory = new TrajectoryBuilder()
                    .Movement(bindingSite, TimeSpan.FromSeconds(0.5), false)
                    .Binding(bindingSite, nearestEntity.BindingSites[0])
                    .Create();

                nearestEntity.Compound.Trajectory(trajectory);
            }
        }
    }
}
