using MolecularMachines.Framework.Model;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class AtomMapper
    {
        public JSONNode ToJson(Atom atom)
        {
            var node = new JSONClass();

            node["symbol"] = atom.Element.Symbol;

            return node;
        }

        public Atom ToModel(JSONNode node)
        {
            return new Atom(
                Element.BySymbol(node["symbol"])
            );
        }
    }
}
