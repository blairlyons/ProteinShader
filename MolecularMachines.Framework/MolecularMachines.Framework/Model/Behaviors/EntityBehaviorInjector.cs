using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Behaviors
{
    /// <summary>
    /// Injector for a <see cref="EntityBehavior"/> object.
    /// Recognizes and applies <see cref="BindingSiteAttribute"/>, <see cref="SensorAttribute"/>,
    /// <see cref="MarkerAttribute"/> and <see cref="BoundEntityAttribute"/>.
    /// Also makes sure, there is an initial state (<see cref="StateAttribute"/>) and calls 
    /// <see cref="EntityBehavior.SetState(StateMethod)"/> using it.
    /// Provides a method <see cref="UpdateValues"/> to update all <see cref="BoundEntityAttribute"/>
    /// annotated properties.
    /// </summary>
    class EntityBehaviorInjector
    {
        public EntityBehaviorInjector(EntityBehavior behavior, Entity owner)
        {
            var type = behavior.GetType();

            // Properties
            foreach (var property in type.GetProperties())
            {
                var attributes = property.GetCustomAttributes(true);

                foreach (var a in attributes)
                {
                    var bindingSiteAttribute = (a as BindingSiteAttribute);
                    if (bindingSiteAttribute != null)
                    {
                        property.SetValue(
                            behavior,
                            owner.BindingSiteById(bindingSiteAttribute.Id),
                            null
                        );
                    }

                    var sensorAttribute = (a as SensorAttribute);
                    if (sensorAttribute != null)
                    {
                        property.SetValue(
                            behavior,
                            owner.SensorById(sensorAttribute.Id),
                            null
                        );
                    }

                    var markerAttribute = (a as MarkerAttribute);
                    if (markerAttribute != null)
                    {
                        property.SetValue(
                            behavior,
                            owner.MarkerById(markerAttribute.Id),
                            null
                        );
                    }

                    var boundEntityAttribute = (a as BoundEntityAttribute);
                    if (boundEntityAttribute != null)
                    {
                        var link = new BoundEntityLink(behavior, property, owner.BindingSiteById(boundEntityAttribute.BindingSiteId));
                        boundEntityLinks.Add(link);
                    }
                }
            }

            // States
            StateMethod initialState = null;

            foreach (var method in type.GetMethods())
            {
                var attributes = method.GetCustomAttributes(typeof(StateAttribute), true);
                foreach (var a in attributes)
                {
                    var stateAttribute = (a as StateAttribute);
                    if (stateAttribute != null)
                    {
                        if (stateAttribute.InitialState)
                        {
                            initialState = (StateMethod)Delegate.CreateDelegate(typeof(StateMethod), behavior, method);
                        }
                    }
                }
            }

            if (initialState == null)
            {
                throw new Exception("no initial state defined for EntityBehavior: "+type.Name);
            }

            behavior.SetState(initialState);
        }

        private List<BoundEntityLink> boundEntityLinks = new List<BoundEntityLink>();

        /// <summary>
        /// Updates all properties with a <see cref="BoundEntityAttribute"/> attribute.
        /// </summary>
        public void UpdateValues()
        {
            foreach (var link in boundEntityLinks)
            {
                link.UpdateValue();
            }
        }
    }
}
