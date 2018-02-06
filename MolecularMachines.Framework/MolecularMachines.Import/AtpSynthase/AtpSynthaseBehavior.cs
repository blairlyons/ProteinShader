using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Behaviors;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.Markers;
using MolecularMachines.Framework.Model.Trajectories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.AtpSynthase
{
    [EntityBehaviorId("atpSynthase.atpSynthase")]
    public class AtpSynthaseBehavior : EntityBehavior
    {
        [BoundEntity("rotorSite")]
        public RotorBehavior Rotor { get; set; }

        [BoundEntity("statorSite")]
        public StatorBehavior Stator { get; set; }

        [BoundEntity("f1a_1")]
        public F1aBehavior F1a_1 { get; set; }

        [BoundEntity("f1a_2")]
        public F1aBehavior F1a_2 { get; set; }

        [BoundEntity("f1a_3")]
        public F1aBehavior F1a_3 { get; set; }

        [Sensor("extraCellularSensor")]
        public Sensor ExtraCellularSensor { get; set; }

        [Marker("intraCellularEjectMarker")]
        public Marker IntraCellularEjectMarker { get; set; }

        [State(InitialState = true)]
        public void Init()
        {
            if (Rotor.CurrentF0c != null)
            {
                SetState(Idle);
            }
        }

        public float RotationProgress
        {
            get { return this.Rotor.RotationProgress; }
        }

        [State]
        public void Idle()
        {


            if (Rotor.CurrentF0c.ProtonSite.IsBound)
            {
                // Eject Proton that is bound to F0c at stator
                var randomPosition =
                    new LocRotStatic(
                        GeometryUtils.RandomVectorInsideCompartment(this.Owner.Environment.CompartmentById("intra")),
                        Quaternion.Identity
                    );

                var ejectTrajectory = new TrajectoryBuilder()
                    .Movement(
                        randomPosition,//this.IntraCellularEjectMarker,
                        TimeSpan.FromSeconds(1.0),
                        false
                    )
                    .Float() // start floating at the end
                    .Create();

                Rotor.CurrentF0c.ProtonSite.ReleaseBond(ejectTrajectory);
            }

            Entity proton;
            if (ExtraCellularSensor.FindNearestEntityOfClass("H+", p => p.BindingSites[0].IsFree, out proton))
            {
                var trajectory = new TrajectoryBuilder()
                    .Movement(
                        Rotor.CurrentF0c.ProtonSite,
                        TimeSpan.FromSeconds(0.5),
                        false
                    )
                    //.Attract(proton.BindingSites[0], Rotor.CurrentF0c.ProtonSite, 10, TimeSpan.FromSeconds(10))
                    .Binding(Rotor.CurrentF0c.ProtonSite, proton.BindingSites[0])
                    .Create();

                Rotor.CurrentF0c.ProtonSite.InitiateBinding(proton.BindingSites[0]);

                proton.Compound.Trajectory(trajectory);

                SetState(BindingInProgress);
            }
        }

        [State]
        public void BindingInProgress()
        {
            if (Rotor.CurrentF0c.ProtonSite.IsBound)
            {
                if (F1Ready())
                {
                    Rotor.Rotate();
                    SetState(Rotating);
                }
            }
        }

        [State]
        public void Rotating()
        {
            if (!Rotor.IsRotating)
            {
                SetState(Idle);
            }
        }

        public bool F1Ready()
        {
            bool result = true;
            switch (Rotor.ActiveF0cIndex)
            {
                case 0:
                    F1a_1.BindAdpAndPr();

                    if (F1a_2.IsAdpAndPrBound)
                    { F1a_2.MakeAtp(); }
                    else if (F1a_2.IsAdpAndPrBinding)
                    { result = false; }

                    F1a_3.ReleaseAtp();
                    break;

                case 4:
                    F1a_3.BindAdpAndPr();

                    if (F1a_1.IsAdpAndPrBound)
                    { F1a_1.MakeAtp(); }
                    else if (F1a_1.IsAdpAndPrBinding)
                    { result = false; }

                    F1a_2.ReleaseAtp();
                    break;


                case 7:
                    F1a_2.BindAdpAndPr();

                    if (F1a_3.IsAdpAndPrBound)
                    { F1a_3.MakeAtp(); }
                    else if (F1a_3.IsAdpAndPrBinding)
                    { result = false; }

                    F1a_1.ReleaseAtp();
                    break;
            }

            return result;
        }
    }
}
