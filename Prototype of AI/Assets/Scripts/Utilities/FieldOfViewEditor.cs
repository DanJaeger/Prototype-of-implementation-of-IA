using UnityEngine;
using UnityEditor;

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

        if (fov.closestTarget != null)
            Handles.DrawWireArc(fov.closestTarget.transform.position, Vector3.up, Vector3.forward, 360, 1);
        
    }
}
