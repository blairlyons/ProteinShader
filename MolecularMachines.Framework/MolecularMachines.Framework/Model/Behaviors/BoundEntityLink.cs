using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MolecularMachines.Framework.Model.Behaviors
{
    /// <summary>
    /// Keeps the value of a property in a <see cref="EntityBehavior"/> class
    /// that is annotated with a <see cref="BoundEntityAttribute"/> uptodate.
    /// When the <see cref="UpdateValue"/> method is called, the property value
    /// is refreshed, if required.
    /// </summary>
    class BoundEntityLink
    {
        /// <summary>
        /// Create a new <see cref="BoundEntityLink"/>.
        /// </summary>
        /// <param name="behavior">Target object</param>
        /// <param name="property">Target property in target behavior object</param>
        /// <param name="bindingSite"><see cref="BindingSite"/> that is observed by the specified target property</param>
        public BoundEntityLink(EntityBehavior behavior, PropertyInfo property, BindingSite bindingSite)
        {
            this.behavior = behavior;
            this.property = property;
            this.bindingSite = bindingSite;
        }

        private EntityBehavior behavior;
        private PropertyInfo property;
        private BindingSite bindingSite;

        private BindingSite lastBoundSite = null;

        /// <summary>
        /// Refreshes the property value, if required.
        /// </summary>
        public void UpdateValue()
        {
            var propType = property.PropertyType;
            object newValue;

            // get value from entity bound to binding site
            var boundSite = (bindingSite.IsBound ? bindingSite.OtherSite : null);

            if (lastBoundSite == boundSite)
            {
                // nothing changed - nothing todo here
            }
            else
            {
                if (boundSite != null)
                {
                    var boundEntity = boundSite.Owner;

                    // choose new value dependent on property type
                    // either the bound entity itself, or its
                    // behavior is injected (if the type is appropriate)
                    if (propType.Equals(typeof(Entity)))
                    {
                        newValue = boundEntity;
                    }
                    else
                    {
                        var boundBehavior = boundEntity.Behavior;
                        if (propType.IsAssignableFrom(boundBehavior.GetType()))
                        {
                            newValue = boundBehavior;
                        }
                        else
                        {
                            newValue = null;
                        }
                    }
                }
                else
                {
                    newValue = null;
                }

                // update lastBoundSite
                lastBoundSite = boundSite;

                // assign
                property.SetValue(behavior, newValue, null);
            }
        }
    }
}
