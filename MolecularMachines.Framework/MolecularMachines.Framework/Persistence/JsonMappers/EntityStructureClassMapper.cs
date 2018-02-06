using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Entities;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class EntityStructureClassMapper
    {
        private AtomMapper atomMapper = new AtomMapper();
        private ArrayMapper arrayMapper = new ArrayMapper();
        private ConformationMapper conformationMapper = new ConformationMapper();
        
        public JSONNode ToJson(EntityStructureClass entityStructureClass)
        {
            var node = new JSONClass();

            node["id"] = entityStructureClass.Id;
            node["atoms"] = arrayMapper.ToJson(entityStructureClass.Atoms, atomMapper.ToJson);
            node["conformations"] = arrayMapper.ToJson(entityStructureClass.Conformations, conformationMapper.ToJson);
            
            return node;
        }

        public EntityStructureClass ToModel(JSONNode node)
        {
            return new EntityStructureClass(
                node["id"],
                arrayMapper.ToModel(node["atoms"], atomMapper.ToModel),
                arrayMapper.ToModel(node["conformations"], conformationMapper.ToModel)
            );
        }
    }
}
