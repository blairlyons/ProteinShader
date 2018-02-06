using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.MolecularMachinesAdapter
{
    class InfoTextLog
    {
        private static InfoTextLog instance = null;
        public static InfoTextLog Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InfoTextLog();
                }

                return instance;
            }
        }

        private StringBuilder log = new StringBuilder();

        public void Reset()
        {
            log.Remove(0, log.Length);
        }

        public void AppendLine(string s)
        {
            log.AppendLine(s);
        }

        public string GetText()
        {
            return log.ToString();
        }
    }
}
