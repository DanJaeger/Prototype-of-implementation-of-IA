using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GA { 
[CustomEditor(typeof(FieldOfView))]
[CanEditMultipleObjects]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);

        Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle, false);

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);

        Handles.color = Color.red;

        if(fov.closestTarget != null)
            Handles.DrawLine(fov.transform.position, fov.closestTarget.transform.position);
        
    }
}
}