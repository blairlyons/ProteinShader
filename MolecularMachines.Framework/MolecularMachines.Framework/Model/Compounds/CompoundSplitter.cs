using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.SpatialLinks;
using MolecularMachines.Framework.Model.Trajectories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Model.Compounds
{
    /// <summary>
    /// Helper class that helps to split a <see cref="Compound"/>, when some <see cref="Entities.Entity"/>s are not bound together anymore.
    /// </summary>
    public class CompoundSplitter
    {
        /// <summary>
        /// New instance for a specific compound.
        /// </summary>
        /// <param name="compound">compound</param>
        public CompoundSplitter(Compound compound)
        {
            this.compound = compound;
            this.environment = compound.Environment;
        }

        private Compound compound;
        private MMEnvironment environment;

        /// <summary>
        /// All entities from the original compund that are not yet assigned to a new compound yet
        /// </summary>
        private HashSet<Entity> unassignedEntities;

        /// <summary>
        /// Analyze <see cref="Compound"/> and create new <see cref="Compound"/>s for split away <see cref="Entity"/>s with a split <see cref="Trajectory"/>
        /// </summary>
        /// <param name="trajectory">split <see cref="Trajectory"/></param>
        public void Split(Trajectory trajectory)
        {
            this.unassignedEntities = new HashSet<Entity>(compound.Entities);

            // Start with original RootEntity and determine which entities remain in the existing Compound
            var entitiesConnectedToRoot = FindConnectedEntities(this.compound.RootEntity);
            this.compound.RemoveEntities(unassignedEntities); // remove entities that are still in unassignedEntities, because only the ones which are not connected to the RootEntity are still in there

            // Find connected entities in the remaining (unassigned) entities and
            // create a new Compound for each connected graph
            while (unassignedEntities.Count > 0)
            {
                var entity = unassignedEntities.First();
                var entitiesConnected = FindConnectedEntities(entity);

                var newCompound = this.environment.CreateCompound(entity);
                foreach (var connectedEntity in entitiesConnected)
                {
                    newCompound.AddEntity(connectedEntity);
                }
                newCompound.Trajectory(trajectory);
            }
        }

        /// <summary>
        /// returns a list with all entities which are reachable via bindings and are still in <see cref="unassignedEntities"/>. The start node will not be contained in the list.
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        private List<Entity> FindConnectedEntities(Entity start)
        {
            List<Entity> connectedEntities = new List<Entity>();
            var removed = this.unassignedEntities.Remove(start);

            // just a check to be sure
            if (!removed) { throw new Exception("something went wrong: start entity is not even in unassigned-list"); }

            FindConnectedEntities(start, connectedEntities);

            return connectedEntities;
        }

        private void FindConnectedEntities(Entity entity, List<Entity> connectedEntities)
        {
            foreach (var bindingSite in entity.BindingSites)
            {
                if (bindingSite.IsBound)
                {
                    var otherEntity = bindingSite.OtherSite.Owner;
                    if (unassignedEntities.Remove(otherEntity))
                    {
                        // otherEntity is not assigned to a connected group yet

                        connectedEntities.Add(otherEntity);

                        // recursively search for other neighbors
                        FindConnectedEntities(otherEntity, connectedEntities);
                    }
                }
            }
        }
    }
}
