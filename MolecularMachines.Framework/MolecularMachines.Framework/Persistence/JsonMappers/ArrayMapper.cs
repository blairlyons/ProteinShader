using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    class ArrayMapper
    {
        public JSONNode ToJson<T>(IEnumerable<T> items, Func<T, JSONNode> mapper)
        {
            var jsonArray = new JSONArray();

            foreach (var item in items)
            {
                jsonArray.Add(mapper(item));
            }

            return jsonArray;
        }

        public IEnumerable<T> ToModel<T>(JSONNode node, Func<JSONNode, T> mapper)
        {
            foreach (JSONNode item in node.AsArray)
            {
                yield return mapper(item);
            }            
        }
    }
}
