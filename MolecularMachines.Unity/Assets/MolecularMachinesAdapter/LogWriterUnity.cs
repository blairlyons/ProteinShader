using System;
using MolecularMachines.Framework.Logging;
using UnityEngine;

namespace Assets.MolecularMachinesAdapter
{
    internal class LogWriterUnity : ILogWriter
    {
        private int counter = 0;

        public void Error(string message)
        {
            Debug.LogError(message);
        }

        public void Info(string message)
        {
            Debug.Log(message);
        }

        public void Warn(string message)
        {
            Debug.LogWarning(message);
        }
    }
}