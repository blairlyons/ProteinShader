using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assets.MolecularMachinesAdapter.Import
{
    class EnvironmentSource
    {
        public EnvironmentSource(string path, JsonImport import)
        {
            this.path = path;
            this.import = import;

            this.Name = new DirectoryInfo(path).Name;
        }

        public string Name { get; private set; }

        private string path;
        private JsonImport import;

        public void Load()
        {
            this.import.Folder(this.path);
        }
    }
}
