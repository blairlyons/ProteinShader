using Assets.MolecularMachinesAdapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class MMTest
    {
        [MenuItem("MM/Clear All")]
        public static void ClearAll()
        {
            SceneManager.Instance.ClearScene();
        }

        [MenuItem("MM/Init MMM")]
        public static void InitMmm()
        {
            MMM.Instance.Poke();
        }
    }
}
