using MolecularMachines.Framework.Model.Entities;
using MolecularMachines.Framework.Model.SpatialLinks;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MolecularMachines.Framework.Model.Compounds;
using MolecularMachines.Framework.Model.Markers;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class BondMapper
    {
        public bool IsSaveWorthy(BindingSite bindingSite)
        {
            return (bindingSite.IsBound && bindingSite.IsInitiator);
        }

        public JSONNode ToJson(BindingSite bindingSite, IdItemStore<Entity> entityIdStore)
        {
            var node = new JSONClass();

            node["active"] = BindingSiteInfoToJson(bindingSite, entityIdStore);
            node["passive"] = BindingSiteInfoToJson(bindingSite.OtherSite, entityIdStore);

            return node;
        }

        private JSONNode BindingSiteInfoToJson(BindingSite bindingSite, IdItemStore<Entity> entityIdStore)
        {
            var node = new JSONClass();

            node["entity"] = entityIdStore.GetId(bindingSite.Owner);
            node["bindingSite"] = bindingSite.Id;

            return node;
        }

        public void ToModel(JSONNode node, IdItemStore<Entity> entityIdStore)
        {
            var activeSite = BindingSiteInfoToModel(node["active"], entityIdStore);
            var passiveSite = BindingSiteInfoToModel(node["passive"], entityIdStore);

            activeSite.InstantBind(passiveSite, true);
        }

        private BindingSite BindingSiteInfoToModel(JSONNode node, IdItemStore<Entity> entityIdStore)
        {
            var entity = entityIdStore.ById(node["entity"].AsString);
            return entity.BindingSiteById(node["bindingSite"].AsString);
        }
    }
}
