using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (HumanPatrol))]
public class FieldOfViewEditor : Editor {

    void OnSceneGUI() {
        Vector3 offset = new Vector3(0, 1.0f, 0);
        HumanPatrol fow = (HumanPatrol)target;
        Handles.color = Color.white;
        Handles.DrawWireArc (fow.transform.position + offset, Vector3.up, Vector3.forward, 360, fow.viewRadius);
        Vector3 viewAngleA = fow.DirFromAngle (-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle (fow.viewAngle / 2, false);

        Handles.DrawLine (fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine (fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        Handles.color = Color.red;
        foreach (Transform visibleTarget in fow.visibleTargets) {
            Handles.DrawLine (fow.transform.position, visibleTarget.position);
        }
    }

}