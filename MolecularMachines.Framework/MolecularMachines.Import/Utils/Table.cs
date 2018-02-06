using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MolecularMachines.Import.Utils
{
    class Table
    {
        public Table() { }

        private Dictionary<CellId, string> data = new Dictionary<CellId, string>();

        private struct CellId
        {
            public CellId(int y, int x)
            {
                this.y = y;
                this.x = x;
            }

            private int y;
            private int x;

            public int Y { get { return this.y; } }
            public int X { get { return this.x; } }
        }

        public string this[int y, int x]
        {
            get
            {
                string value;
                if (data.TryGetValue(new CellId(y, x), out value))
                {
                    return value;
                }
                else
                {
                    return null;
                }
            }

            set
            {
                data[new CellId(y, x)] = value;
            }
        }

        private CellId GetMaxId()
        {
            int x = -1;
            int y = -1;

            foreach (var key in this.data.Keys)
            {
                if (key.X > x) { x = key.X; }
                if (key.Y > y) { y = key.Y; }
            }

            return new CellId(y, x);
        }

        public void SaveCsv(string filename)
        {
            var max = GetMaxId();

            using (var w = new StreamWriter(File.Open(filename, FileMode.Create)))
            {
                for (int y = 0; y <= max.Y; y++)
                {
                    if (y != 0)
                    {
                        w.WriteLine();
                    }

                    for (int x = 0; x <= max.X; x++)
                    {
                        if (x != 0)
                        {
                            w.Write(",");
                        }

                        var value = this[y, x];

                        if (value != null)
                        {
                            value = value.Replace("\"", "\"\"");

                            w.Write("\"");
                            w.Write(value);
                            w.Write("\"");
                        }
                    }
                }
            }
        }
    }
}
