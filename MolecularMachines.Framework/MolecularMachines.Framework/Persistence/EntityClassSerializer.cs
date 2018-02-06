using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Persistence.JsonMappers;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence
{
    public class EntityClassSerializer
    {
        private EntityClassMapper mapper = new EntityClassMapper();

        public void Serialize(EntityClass entityClass, string filename)
        {
            var node = this.mapper.ToJson(entityClass);
            var data = node.ToString();
            File.WriteAllText(filename, data);
        }

        public EntityClass Deserialize(string filename)
        {
            var data = File.ReadAllText(filename);
            var node = JSON.Parse(data);
            var entityClass = this.mapper.ToModel(node);
            return entityClass;
        }
    }
}
