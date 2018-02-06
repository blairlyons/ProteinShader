using MolecularMachines.Framework.Model.Trajectories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.MolecularMachinesAdapter.Adapters.Trajectories
{
    class AttractionUnity : TrajectorySectionUnity<Attraction>
    {
        public AttractionUnity(TrajectoryUnity trajectoryUnity, TrajectorySection section) : base(trajectoryUnity, section)
        {
        }

        public override void Update()
        {
            if (CurrentSectionTime >= info.Timeout || !info.Destination.IsAlive)
            {
                Next();
            }
            else
            {
                var at = info.ApplyAt.LocRot;
                var destination = info.Destination.LocRot;

                var direction = (destination.Location - at.Location).ToUnity();
                if (direction.magnitude < info.UntilDistanceLessThan)
                {
                    Next();
                }
                else
                {
                    var force = direction.normalized * 50;
                    var position = at.Location.ToUnity();

                    Compound.Rigidbody.AddForceAtPosition(
                        force,
                        position,
                        ForceMode.Force
                    );
                }
            }
        }

    }
}
