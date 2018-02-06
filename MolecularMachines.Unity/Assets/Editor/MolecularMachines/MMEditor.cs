using UnityEngine;
using System.Collections;
using UnityEditor;
using Assets.MolecularMachinesAdapter;
using System.Collections.Generic;
using System.Linq;
using Assets.MolecularMachinesAdapter.Adapters.SpatialLinks;
using MolecularMachines.Framework.Model;

public class MMEditor : EditorWindow {

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

    [MenuItem("MM/Show Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MMEditor));
    }

    public int compoundIndex = 0;

    void OnGUI()
    {
        var environment = MMM.Environment;
        if (environment != null)
        {
            GUILayout.Label("This is experimental");

            var options = environment.Compounds.Select(c => c.ToString()).OrderByDescending(s => s.Length).ToArray();
            compoundIndex = EditorGUILayout.Popup(compoundIndex, options);
            GUILayout.Label("Compounds: "+options.Length);

            var compound = environment.Compounds.ElementAtOrDefault(compoundIndex);
            if (compound != null)
            {
                GUILayout.Label("Selected Compound: " + compound);
                GUILayout.Label("Spatial Link: " + compound.SpatialLink);

                var locRot = compound.SpatialLink.LocRot;

                if (compound.SpatialLink is FixedUnity)
                {
                    
                }
                else
                {
                    if (GUILayout.Button("Fix"))
                    {
                        compound.Fix(locRot);
                    }
                }
            }
        }
        else
        {
            GUILayout.Label("no MM Environment");
        }
    }
}
