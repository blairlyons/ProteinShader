using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.Trajectories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Markers
{
    /// <summary>
    /// A <see cref="BindingSite"/> of an <see cref="Entity"/>.
    /// <see cref="BindingSite"/>s can bind to other <see cref="BindingSite"/>s of other <see cref="Entity"/>s.
    /// </summary>
    public class BindingSite : Marker
    {
        public BindingSite(Entity owner, BindingSiteClass bindingSiteClass, LocRot localLocRot) : base(owner, bindingSiteClass, localLocRot)
        {
            this.State = BindingState.Free;
        }

        public BindingState State { get; private set; }

        public bool IsBound { get { return (State == BindingState.Bound); } }
        public bool IsBinding { get { return (State == BindingState.Binding); } }
        public bool IsFree { get { return (State == BindingState.Free); } }

        /// <summary>
        /// Is true, if this <see cref="BindingSite"/> initiated the bond.
        /// </summary>
        public bool IsInitiator { get; private set; }

        /// <summary>
        /// The <see cref="BindingSite"/> this <see cref="BindingSite"/> is bound to.
        /// </summary>
        public BindingSite OtherSite { get; private set; }

        /// <summary>
        /// The <see cref="Entity"/> which belongs the <see cref="BindingSite"/> to which this <see cref="BindingSite"/> is bound to.
        /// </summary>
        public Entity OtherEntity
        {
            get { return OtherSite?.Owner; }
        }

        /// <summary>
        /// Marks this <see cref="BindingSite"/> as <see cref="BindingState.Binding"/>.
        /// Previous bonds are released.
        /// When the two <see cref="BindingSite"/>s are finally bound together, <see cref="InstantBind(BindingSite, bool)"/> must be called.
        /// </summary>
        /// <param name="otherSite">Target</param>
        /// <param name="isInitiator">true, if this instance initiates the binding</param>
        public void InitiateBinding(BindingSite otherSite, bool isInitiator = true)
        {
            if (otherSite == null) { throw new ArgumentNullException(nameof(otherSite)); }

            // Prepare

            var state = this.State;

            if (state == BindingState.Free)
            {
                // nothing to do here
            }
            else if (state == BindingState.Binding)
            {
                if (this.OtherSite != otherSite) { ReleaseBond(); }
            }
            else if (state == BindingState.Bound)
            {
                ReleaseBond();
            }
            else { throw new Exception("unknown BindingState " + state); }

            // Initiate Binding

            this.OtherSite = otherSite;
            this.State = BindingState.Binding;
            this.IsInitiator = isInitiator;
            if (isInitiator)
            {
                otherSite.InitiateBinding(this, false);
            }
        }

        /// <summary>
        /// Instantly binds to another <see cref="BindingSite"/>.
        /// Previous bonds are released.
        /// </summary>
        /// <param name="otherSite">target</param>
        /// <param name="isInitiator">true, if this instance initiates the binding</param>
        public void InstantBind(BindingSite otherSite, bool isInitiator = true)
        {
            if (otherSite == null) { throw new ArgumentNullException(nameof(otherSite)); }

            // Prepare

            var state = this.State;

            if (state == BindingState.Free)
            {
                // nothing to do here
            }
            else if (state == BindingState.Binding)
            {
                if (this.OtherSite != otherSite) { ReleaseBond(); }
            }
            else if (state == BindingState.Bound)
            {
                if (this.OtherSite != otherSite)
                { ReleaseBond(); }
                else
                {
                    return; // already bound to this site -> return
                }
            }
            else { throw new Exception("unknown BindingState " + state); }

            // Bind

            this.OtherSite = otherSite;
            this.State = BindingState.Bound;
            this.IsInitiator = isInitiator;
            if (isInitiator)
            {
                otherSite.InstantBind(this, false);
            }

            if (isInitiator)
            {
                this.Owner.Compound.Merge(otherSite.Owner.Compound);
            }
        }

        /// <summary>
        /// Release the current bond
        /// </summary>
        public void ReleaseBond()
        {
            var ejectTrajectory = new TrajectoryBuilder().Float().Create();
            ReleaseBond(ejectTrajectory);
        }

        private void SetStateReleased()
        {
            this.State = BindingState.Free;
            this.OtherSite = null;
            this.IsInitiator = false;
        }

        /// <summary>
        /// Release the current bond
        /// </summary>
        /// <param name="ejectTrajectory">trajectory of the split away compound</param>
        public void ReleaseBond(Trajectory ejectTrajectory)
        {
            var state = this.State;
            if (state == BindingState.Free)
            {
                // nothing to do here
            }
            else if (state == BindingState.Binding)
            {
                var oldOtherSide = this.OtherSite;

                // release
                SetStateReleased();

                // inform the old other site to stop binding
                oldOtherSide.SetStateReleased();
            }
            else if (state == BindingState.Bound)
            {
                var oldOtherSide = this.OtherSite;

                // release
                SetStateReleased();

                // inform other site to release bond
                oldOtherSide.SetStateReleased();

                // Split compound if necessary
                this.Owner.Compound.Split(ejectTrajectory);
            }
            else { throw new Exception("unknown BindingState " + state); }
        }

        // Geometric Info

        /// <summary>
        /// Virtual vector of the <see cref="BindingSite"/>.
        /// When two <see cref="BindingSite"/>s are bound together, their <see cref="BindingVector"/>s point at each other.
        /// This vector indicates the direction when the <see cref="BindingSite"/> is not rotated.
        /// </summary>
        public static readonly Vector BindingVector = Vector.AxisY;

        /// <summary>
        /// Rotation that is applied to determine the location of a bound <see cref="BindingSite"/>.
        /// </summary>
        public static readonly Quaternion BindingQuaternion = Quaternion.FromYawPitchRoll(0, 0, (float)Math.PI);

        /// <summary>
        /// <see cref="LocRot"/> for a <see cref="BindingSite"/> at location <see cref="Vector.Zero"/> and rotation <see cref="BindingSite.BindingQuaternion"/>.
        /// </summary>
        public static readonly LocRot DirectBindingLocRot = new LocRotStatic(Vector.Zero, BindingSite.BindingQuaternion);
    }
}
