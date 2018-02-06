using MolecularMachines.Framework.Model.Markers;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class BindingSiteClassMapper
    {
        public void ToJson(JSONNode node, BindingSiteClass bindingSiteClass)
        {
            // nothing to do here
        }

        public BindingSiteClass ToModel(JSONNode node, string id)
        {
            return new BindingSiteClass(id);
        }
    }
}
