using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Logging;
using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Compounds
{
    /// <summary>
    /// Helper class that aligns <see cref="Entity"/>s starting from a root unsing their <see cref="BindingSite"/>s.
    /// </summary>
    public class CompoundEntityAligner
    {
        private HashSet<Entity> alreadyAligned = new HashSet<Entity>();

        /// <summary>
        /// Start align for a rootEntity.
        /// </summary>
        /// <param name="rootEntity"><see cref="Entity"/> to start with</param>
        /// <param name="locRot">RootEntity <see cref="LocRot"/></param>
        public void Align(Entity rootEntity, LocRot locRot)
        {
            SetEntityLocRot(rootEntity, locRot);
        }

        private void SetEntityLocRot(Entity entity, LocRot locRot)
        {
            entity.LocRot.Set(locRot);

            foreach (var bindingSite in entity.BindingSites)
            {
                if (bindingSite.IsBound)
                {
                    // Update bound entity position
                    var otherSite = bindingSite.OtherSite;
                    var otherEntity = otherSite.Owner;

                    if (alreadyAligned.Add(otherEntity))
                    {
                        var otherSiteLocRotLocal = otherSite.LocalLocRot;
                        var otherEntityLocRot = otherEntity.LocRot;

                        var bindingSiteLocRot = bindingSite.LocRot;


                        SetEntityLocRot(
                            otherEntity,
                            CalculateBoundEntityLocRot(bindingSiteLocRot, otherSiteLocRotLocal)
                        );
                    }
                    else
                    {
                        // already aligned -> nothing to do here
                        // this can happen on circular bindings
                    }
                }
            }
        }

        /// <summary>
        /// Calculate the absolute location and rotation of another entity, by the absolute <see cref="LocRot"/> of the root <see cref="BindingSite"/> and the local <see cref="LocRot"/> of the bound <see cref="BindingSite"/>.
        /// </summary>
        /// <param name="rootSite"></param>
        /// <param name="otherSiteLocal"></param>
        /// <returns>absolute location and rotation of other (bound) entity</returns>
        public static LocRot CalculateBoundEntityLocRot(LocRot rootSite, LocRot otherSiteLocal)
        {
            //
            // RSL... rootSite.Location
            // RSR... rootSite.Rotation
            // EL.... Entity Location
            // ER.... Entity Rotation
            // OSLL.. otherSiteLocal.Location
            // OSLR.. otherSiteLocal.Rotation
            //
            // The global rotation and position of otherSite is defined by:
            //  OSR = ER * OSLR
            //  OSL = EL + ER.Rotate(OSLL)
            //
            // In a bond, OSL = RSL
            //
            // Therefore:
            //  ER = OSR * Invert(OSLR)
            //  EL = OSL - ER.Rotate(OSLL)

            // rotate rootSite to get OSR
            var otherSiteRotation = Quaternion.CombineRotation(rootSite.Rotation, BindingSite.BindingQuaternion);

            var otherEntityRotation = Quaternion.CombineRotation(otherSiteRotation, otherSiteLocal.Rotation.GetInverted());
            var otherEntityLocation = rootSite.Location - otherEntityRotation.Rotate(otherSiteLocal.Location);

            return new LocRotStatic(
                otherEntityLocation,
                otherEntityRotation
            );
        }
    }
}
