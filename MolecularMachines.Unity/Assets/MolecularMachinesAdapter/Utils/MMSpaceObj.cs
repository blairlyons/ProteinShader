using UnityEngine;
using System.Collections;
using Assets.MolecularMachinesAdapter.Adapters;
using MMG = MolecularMachines.Framework.Geometry;
using Assets.MolecularMachinesAdapter;
using MolecularMachines.Framework.Model;

public class MMSpaceObj : MonoBehaviour
{

    public float x, y, z, rotationYaw, rotationPitch, rotationRoll;

    // Use this for initialization
    void Start()
    {

    }

    private LocRot LocRot
    {
        get
        {
            return new MolecularMachines.Framework.Model.LocRotStatic(
                new MMG.Vector(x, y, z),
                MMG.Quaternion.FromYawPitchRoll(rotationYaw, rotationPitch, rotationRoll)
            );
        }
    }

    // Update is called once per frame
    void Update()
    {
        var locRot = this.LocRot;

        gameObject.transform.position = MMEnvironmentUnity.TransformToUnity(locRot.Location);
        gameObject.transform.rotation = locRot.Rotation.ToUnity();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.magenta;

        var locRot = this.LocRot;

        var from = MMEnvironmentUnity.TransformToUnity(locRot.Location);
        var to =
            MMEnvironmentUnity.TransformToUnity(
                locRot.Location +
                locRot.Rotation.Rotate(
                    MolecularMachines.Framework.Geometry.Vector.AxisY * 3f
                )
            );

        Gizmos.DrawLine(from, to);

        Gizmos.DrawSphere(
            from,
            0.1f
        );
        Gizmos.DrawSphere(
            to,
            0.04f
        );

        // Distance to next compartment
        /*
        {
            var nearestCompartment = MMM.Environment.NearestCompartment(locRot.Location);
            var sp = nearestCompartment.GetNearestSurfacePoint(locRot.Location);

            Gizmos.DrawCube(MMEnvironmentUnity.TransformToUnity(sp), new Vector3(1, 1, 1) * 0.05f);
            Gizmos.DrawLine(
                MMEnvironmentUnity.TransformToUnity(locRot.Location),
                MMEnvironmentUnity.TransformToUnity(sp)
            );
            Debug.Log("Distance: " + nearestCompartment.Distance(locRot.Location));
        }
        */
    }
}
