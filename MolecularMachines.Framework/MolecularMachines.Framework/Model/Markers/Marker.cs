using MolecularMachines.Framework.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Markers
{
    public class Marker : ISpaceInherent
    {
        public Marker(Entity owner, MarkerClass markerClass, LocRot localLocRot)
        {
            this.Id = markerClass.Id;
            this.Owner = owner;
            this.Class = markerClass;
            this.LocalLocRot = localLocRot;
        }

        public string Id { get; private set; }
        public Entity Owner { get; private set; }
        public MarkerClass Class { get; private set; }
        public LocRot LocalLocRot { get; private set; }

        public LocRot LocRot
        {
            get
            {
                return LocRot.Combine(this.Owner.LocRot, LocalLocRot);
            }
        }

        bool ISpaceInherent.IsAlive
        {
            get
            {
                return !this.Owner.IsDisposed;
            }
        }
    }
}
