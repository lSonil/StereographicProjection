using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Public variables to define positions and angles for the camera
    public Vector3 position1 = new Vector3(0, 5, -10); // Position 1
    public Vector3 angle1 = new Vector3(30, 45, 0); // Angle for position 1
    public Vector3 position234 = new Vector3(5, 5, -5); // Position 2
    public Vector3 angle2 = new Vector3(30, 45, 0); // Angle for position 2
    public Vector3 angle3 = new Vector3(30, -45, 0); // Angle for position 3
    public Vector3 angle4 = new Vector3(30, 60, 0); // Angle for position 4

    public float orbitSpeed = 30f; // Speed for orbiting around Y-axis

    private void Update()
    {
        // Listen for key presses to switch camera positions
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetCameraPositionAndAngle(position1, angle1, false); // Default position
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetCameraPositionAndAngle(position234, angle2, false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetCameraPositionAndAngle(position234, angle3, false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetCameraPositionAndAngle(position234, angle4, false);
        }
        // Handle mouse scroll input for rotation around the Y-axis
        HandleMouseScrollRotation();
    }

    private void SetCameraPositionAndAngle(Vector3 position, Vector3 angles, bool orbit)
    {
        // Set the camera's position and rotation
        transform.position = position;
        transform.rotation = Quaternion.Euler(angles);
        transform.parent.eulerAngles = Vector3.zero;

        // Set orbit flag if needed
    }

    private void HandleMouseScrollRotation()
    {
        // Get mouse scroll wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            print(scrollInput);
            float rotationAmount = scrollInput * orbitSpeed;
            transform.parent.Rotate(0, rotationAmount * Time.deltaTime * 100, 0);
        }
    }

}
