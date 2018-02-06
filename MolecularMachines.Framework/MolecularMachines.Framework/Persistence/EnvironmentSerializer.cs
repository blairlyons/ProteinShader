using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Persistence.JsonMappers;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence
{
    public class EnvironmentSerializer
    {
        public EnvironmentSerializer(MMEnvironment environment)
        {
            this.environment = environment;
        }

        private MMEnvironment environment;

        private EnvironmentMapper environmentMapper = new EnvironmentMapper();

        public void Serialize(string filename)
        {
            var node = this.environmentMapper.ToJson(this.environment);
            var data = node.ToString();
            File.WriteAllText(filename, data);
        }
        
        public void Deserialize(string filename)
        {
            var data = File.ReadAllText(filename);
            var node = JSON.Parse(data);

            this.environmentMapper.ToModel(this.environment, node);
        }
    }
}
