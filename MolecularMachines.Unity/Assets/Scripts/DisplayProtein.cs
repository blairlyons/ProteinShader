using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayProtein : MonoBehaviour
{
    public string pdbID;
    public float randomStepSize = 1f;
    public float randomRotation = 5f;

	void Start ()
    {
        SceneManager.Instance.AddIngredient(pdbID, new Bounds(transform.position, 5f * Vector3.one), PdbLoader.LoadAtomSpheres(pdbID), Color.blue);
        SceneManager.Instance.AddIngredientInstance(pdbID, transform.position, transform.rotation);
        SceneManager.Instance.UploadAllData();
    }

	void Update ()
    {
        transform.position += randomStepSize * Random.onUnitSphere;
        transform.rotation *= Quaternion.Euler(randomRotation * Random.onUnitSphere);

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        SceneManager.Instance.UpdateProteinInstanceLocRot(0, transform.position, transform.rotation);
        SceneManager.Instance.UploadAllData();
    }
}
