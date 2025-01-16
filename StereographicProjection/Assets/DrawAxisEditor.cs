using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DrawAxis))]
public class DrawAxisLinesEditor : Editor
{
    private void OnSceneGUI()
    {
        // Get the target object and its position
        DrawAxis script = (DrawAxis)target;
        Transform targetTransform = script.transform;
        Vector3 origin = targetTransform.position;

        // Define the colors
        Color xColor = Color.red;     // X-axis (Red)
        Color yColor = Color.green;  // Y-axis (Green)
        Color zColor = Color.blue;   // Z-axis (Blue)

        // Store the original handle color
        Color originalColor = Handles.color;

        // Draw the X-axis line
        Handles.color = xColor;
        Handles.DrawLine(origin, origin + Vector3.right * script.lineLength);

        // Draw the Y-axis line
        Handles.color = yColor;
        Handles.DrawLine(origin, origin + Vector3.up * script.lineLength);

        // Draw the Z-axis line
        Handles.color = zColor;
        Handles.DrawLine(origin, origin + Vector3.forward * script.lineLength);

        // Restore the original handle color
        Handles.color = originalColor;
    }
}
