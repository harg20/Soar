using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Ikfabric : MonoBehaviour
{
    public int chainlength = 2;
    public Transform target;
    public Transform pole;
    public int iterations = 10;
    public float delta = .001f;
    public float SnapBackStrength = 1f;

    

    protected float[] BonesLength;
    protected float CompleteLength;

    protected Transform[] Bones;
    protected Vector3[] Positions;

    protected Vector3[] StartDirectionSucc;
    protected Quaternion[] StartRotationBone;
    protected Quaternion StartRotationTarget;
    protected Quaternion StartRotationRoot;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Init()
    {
        Bones = new Transform[chainlength + 1];
        Positions = new Vector3[chainlength + 1];
        BonesLength = new float[chainlength];
        StartDirectionSucc = new Vector3[chainlength + 1];
        StartRotationBone = new Quaternion[chainlength + 1];
       
        StartRotationTarget = target.rotation;
        CompleteLength = 0;
      

        var current = transform;
        for(var i = Bones.Length - 1; i>= 0; i--)
        {
            Bones[i] = current;
            StartRotationBone[i] = current.rotation;

            if (i == Bones.Length - 1)
            {
                StartDirectionSucc[i] = target.position - current.position;
            }
            else
            {
                StartDirectionSucc[i] = Bones[i + 1].position - current.position;
                BonesLength[i] = (Bones[i + 1].position - current.position).magnitude;
                CompleteLength += BonesLength[i];
            }
            current = current.parent;
        }

    }

    private void LateUpdate()
    {
        ResolveIK();
    }

    void ResolveIK()
    {
        if (target == null)
        {
            return;
        }
        if (BonesLength.Length != chainlength)
        {
            Init();
        }
        for (int i = 0; i < Bones.Length; i++)
        {
            Positions[i] = Bones[i].position;
        }
        var RootRot = (Bones[0].parent != null) ? Bones[0].parent.rotation : Quaternion.identity;
        var RootRitDiff = RootRot * Quaternion.Inverse(StartRotationRoot);
        if((target.position - Bones[0].position).sqrMagnitude >= CompleteLength * CompleteLength)
        {
            var direction = (target.position - Positions[0]).normalized;

            for (int i = 1; i < Positions.Length; i++)
            {
                Positions[i] = Positions[i - 1] + direction * BonesLength[i - 1];
            }
        }
        else
        {
            for (int iteration = 0; iteration < iterations; iteration++){

                for (int i = Positions.Length - 1; i > 0; i--)
                {
                    if (i == Positions.Length - 1)
                    {
                        Positions[i] = target.position;
                    }
                    else
                    {
                        Positions[i] = Positions[i + 1] + (Positions[i] - Positions[i + 1]).normalized * BonesLength[i];
                    }
                }
                    for (int i = 1; i < Positions.Length; i++)
                        Positions[i] = Positions[i - 1] + (Positions[i] - Positions[i - 1]).normalized * BonesLength[i - 1]; 
                

                if ((Positions[Positions.Length - 1] - target.position).sqrMagnitude < delta * delta)
                {
                    break;
                }
            }
            if (pole != null)
            {
                for (int i = 1; i < Positions.Length - 1; i++)
                {
                    var plane = new Plane(Positions[i + 1] - Positions[i - 1], Positions[i - 1]);
                    var ProjectedPole = plane.ClosestPointOnPlane(pole.position);
                    var ProjectedBone = plane.ClosestPointOnPlane(Positions[i]);
                    var angle = Vector3.SignedAngle(ProjectedBone - Positions[i - 1], ProjectedPole - Positions[i - 1], plane.normal);
                    Positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (Positions[i] - Positions[i - 1]) + Positions[i - 1];
                }
            }
        }
        for (int i = 0; i < Positions.Length; i++)
        {
            if (i == Positions.Length - 1)
                Bones[i].rotation = target.rotation * Quaternion.Inverse(StartRotationTarget) * StartRotationBone[i];
            else
                Bones[i].rotation = Quaternion.FromToRotation(StartDirectionSucc[i], Positions[i + 1] - Positions[i]) * StartRotationBone[i];
            Bones[i].position = Positions[i] ;

        }

    }
    private void OnDrawGizmos()
    {
        var current = this.transform;
        for (int i = 0;  i < chainlength && current != null && current.parent != null; i++)
        {
            var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            //Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
           // Handles.color = Color.green;
           // Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
    }
}
