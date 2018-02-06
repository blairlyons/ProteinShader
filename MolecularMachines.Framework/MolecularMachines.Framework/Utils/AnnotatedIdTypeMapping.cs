using MolecularMachines.Framework.Logging;
using MolecularMachines.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MolecularMachines.Framework.Utils
{
    /// <summary>
    /// Searches the <see cref="AppDomain"/> using reflection to find public classes that are annotated with ID-Attributes.
    /// </summary>
    /// <typeparam name="TAnnotation">Type if the ID-Attribute. Must implement <see cref="IStringId"/></typeparam>
    /// <typeparam name="TMappedType">Type of the annotated class</typeparam>
    class AnnotatedIdTypeMapping<TAnnotation, TMappedType>
        where TAnnotation : Attribute, IStringId
    {
        public AnnotatedIdTypeMapping(bool throwExcepionWhenNotFound, Type defaultType)
        {
            this.defaultType = defaultType;
            this.throwExcepionWhenNotFound = throwExcepionWhenNotFound;

            Scan();
        }

        private Type defaultType;
        private bool throwExcepionWhenNotFound;

        private void Scan()
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in a.GetExportedTypes())
                {
                    var attributes = t.GetCustomAttributes(typeof(TAnnotation), false);

                    foreach (IStringId attribute in attributes)
                    {
                        if (!typeof(TMappedType).IsAssignableFrom(t))
                        {
                            throw new Exception($"Type {t.FullName} has the {typeof(TAnnotation).Name}-Attribute applied, but does not inherit from {nameof(TMappedType)}");
                        }

                        idTypeMap.Add(attribute.Id, t);
                    }
                }
            }
        }

        private Dictionary<string, Type> idTypeMap = new Dictionary<string, Type>();

        public Type GetTypeById(string id)
        {
            Type result;
            if (idTypeMap.TryGetValue(id, out result))
            {
                return result;
            }

            if (throwExcepionWhenNotFound)
            {
                throw new Exception($"{typeof(TAnnotation).Name} not found in AppDomain: {id}");
            }
            else
            {
                Log.Warn($"{typeof(TAnnotation).Name} not found in AppDomain: " + id);
                return defaultType; 
            }
        }
    }
}
