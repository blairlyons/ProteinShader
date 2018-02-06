using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MolecularMachines.Framework.Geometry;

namespace MolecularMachines.Framework.Model.Builders
{
    /// <summary>
    /// Helper class for managing a list of <see cref="LocRot"/>s.
    /// Used by <see cref="EntityClassBuilder"/> to manage <see cref="Markers.Marker"/> <see cref="LocRot"/>s.
    /// </summary>
    public class LocRotListBuilder
    {
        private List<LocRot> items = new List<LocRot>();

        /// <summary>
        /// Add new <see cref="LocRot"/>
        /// </summary>
        /// <param name="item">value</param>
        public void Add(LocRot item)
        {
            items.Add(item);
        }

        /// <summary>
        /// Add new <see cref="LocRot"/> at a specific index.
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="item">value</param>
        public void Add(int index, LocRot item)
        {
            while (items.Count <= index)
            {
                items.Add(null);
            }

            items[index] = item;
        }

        /// <summary>
        /// Get all items.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LocRot> GetLocRots()
        {
            return items;
        }

        /// <summary>
        /// Get <see cref="LocRot"/> at specific index.
        /// </summary>
        /// <param name="index">index</param>
        /// <returns></returns>
        public LocRot GetLocRotAt(int index)
        {
            return items[index];
        }

        /// <summary>
        /// Move atoms by setting a new origin
        /// </summary>
        /// <param name="origin">new origin</param>
        public void SetOrigin(Vector origin)
        {
            for (int i = 0; i < this.items.Count; i++)
            {
                var item = this.items[i];

                this.items[i] = new LocRotStatic(
                    item.Location - origin,
                    item.Rotation
                );
            }
        }

        /// <summary>
        /// Clear all items and copy them from another <see cref="LocRotListBuilder"/> instance.
        /// </summary>
        /// <param name="markers"></param>
        public void CopyFrom(LocRotListBuilder markers)
        {
            this.items.Clear();
            this.items.AddRange(markers.items);
        }
    }
}
