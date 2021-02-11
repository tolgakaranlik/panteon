using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDisplayer : MonoBehaviour
{
    Transform[] childGizmos;

    public float Radius = 0.3f;
    public Color GizmoColor = Color.white;

    private void OnDrawGizmos()
    {
        childGizmos = GetComponentsInChildren<Transform>();
        if(childGizmos.Length == 0)
        {
            return;
        }

        Gizmos.DrawSphere(childGizmos[0].position, Radius);

        for (int i = 1; i < childGizmos.Length; i++)
        {
            Gizmos.color = GizmoColor;
            Gizmos.DrawLine(childGizmos[i - 1].position, childGizmos[i].position);
            Gizmos.DrawSphere(childGizmos[i].position,Radius);
        }
    }
}
