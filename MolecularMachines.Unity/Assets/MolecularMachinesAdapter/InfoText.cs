using MolecularMachines.Framework.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.MolecularMachinesAdapter
{
    class InfoText
    {
        public void Load()
        {
            var observedEntities = new string[]
            {
                //"atpSynthase",
                "f0c",
                "f1a_1",
                "f1a_2",
                "f1a_3",
                "hemoglobin"
            };

            foreach (var entity in MMM.Environment.Entities)
            {
                if (observedEntities.Contains(entity.Class.Id))
                {
                    this.entities.Add(entity);
                }
            }
        }

        private List<Entity> entities = new List<Entity>();

        public string GetText()
        {
            if (entities.Count == 0)
            {
                Load();
            }

            var s = new StringBuilder();

            foreach (var entity in this.entities)
            {
                s.AppendLine(entity.ToString());
            }

            return s.ToString();
        }
    }
}
