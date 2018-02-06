using Assets.MolecularMachinesAdapter.Adapters;
using MolecularMachines.Framework.Geometry;
using MolecularMachines.Framework.Model;
using MolecularMachines.Framework.Model.Builders;
using MolecularMachines.Framework.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.MolecularMachinesAdapter.Import
{
    class JsonImport
    {
        public JsonImport(MMEnvironmentUnity environment)
        {
            this.environment = environment;
        }

        private MMEnvironmentUnity environment;

        public void Folder(string path)
        {
            UnityEngine.Debug.Log("Start import...");
            var start = DateTime.Now;

            this.environment.Reset();

            foreach (var filename in Directory.GetFiles(path, "*.mmec.json"))
            {
                Debug.Log("import ec " + filename);
                var ecSerialier = new EntityClassSerializer();
                var ec = ecSerialier.Deserialize(filename);
                environment.AddEntityClass(ec);
            }

            foreach (var filename in Directory.GetFiles(path, "*.mmenv.json"))
            {
                Debug.Log("import env " + filename);

                var eSerializer = new EnvironmentSerializer(environment);
                eSerializer.Deserialize(filename);
            }

            var startUpload = DateTime.Now;
            environment.Reupload();
            var end = DateTime.Now;
            UnityEngine.Debug.Log("Imported \"" + environment.Name + "\" in " + (end - start).TotalMilliseconds + "ms. Upload took " + (end - startUpload).TotalMilliseconds + "ms");
        }

        private float DegToRad(double v)
        {
            return (float)(v * Math.PI / 180.0);
        }
    }
}
