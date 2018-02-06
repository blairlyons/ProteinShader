using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MolecularMachines.Framework.Model.Entities;
using SimpleJSON;

namespace MolecularMachines.Framework.Persistence.JsonMappers
{
    public class IdItemStore<T>
    {
        private Dictionary<string, T> idItemMap = new Dictionary<string, T>();
        private Dictionary<T, string> itemIdMap = new Dictionary<T, string>();


        private int idCounter = 0;

        public void Add(T item, string id)
        {
            this.idItemMap.Add(id, item);
            this.itemIdMap.Add(item, id);
        }

        public string Add(T item)
        {
            // generate a non existing id
            string id;
            do
            {
                id = (idCounter++).ToString();
            }
            while (idItemMap.ContainsKey(id));

            // add with generated id
            Add(item, id);

            // return
            return id;
        }

        public T ById(string id)
        {
            T item;
            if (this.idItemMap.TryGetValue(id, out item))
            {
                return item;
            }
            else
            {
                throw new Exception("id \""+id+"\" not found");
            }
        }

        public string GetId(T item)
        {
            string id;
            if (this.itemIdMap.TryGetValue(item, out id))
            {
                return id;
            }
            else
            {
                throw new Exception("item not found");
            }
        }
    }
}
