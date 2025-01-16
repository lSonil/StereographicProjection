using TMPro;
using UnityEngine;

public class DrawAxisLines : MonoBehaviour
{
    public float lineLength = 5f;
    public float lineWidth = 0.1f;

    public float height = 1.0f; // Sphere height along X-axis
    public Material wireframeMaterial; // Assign a wireframe material in the Inspector
    public Material wireframeMaterial1; // Assign a wireframe material in the Inspector
    public Material projectionframeMaterial1; // Assign a wireframe material in the Inspector
    public Material trailMaterial; // Assign a wireframe material in the Inspector

    public Vector2 smallSphereStartPosition = new Vector2(2.0f, 3.0f); // Initial X, Z for the small sphere
    public float smallSphereHeight = 1.0f; // Height of the small sphere
    public float intersectionSphereCalcHeight = 5.0f; // Height of the stretched sphere

    private GameObject projectionSphere;
    private GameObject bigSphere;
    private GameObject smallSphere;
    private GameObject intersectionSphereCalc;
    public GameObject center;
    private LineRenderer lineRenderer;

    public TextMeshProUGUI textW; // Reference to the first TextMeshProUGUI for 'w'
    public TextMeshProUGUI textP; // Reference to the second TextMeshProUGUI for 'P'
    public TextMeshProUGUI mode; // Reference to the second TextMeshProUGUI for 'P'
    public Transform pivot;
    public bool axisMode=true;
    private void Start()
    {
        center.transform.position = new Vector3(0,height/2,0);
        CreateLine(Vector3.right, Color.red);    // X-axis
        CreateLine(Vector3.left, Color.red);    // X-axis
        CreateLine(Vector3.up, Color.green);    // Y-axis
        CreateLine(Vector3.down, Color.green);    // Y-axis
        CreateLine(Vector3.forward, Color.blue); // Z-axis
        CreateLine(Vector3.back, Color.blue); // Z-axis

        bigSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectionSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        // Position the bottom of the sphere at 0.0
        bigSphere.transform.position = new Vector3(0.0f, height / 2.0f, 0.0f);
        projectionSphere.transform.position = Vector3.zero;

        // Scale the sphere to stretch along the X-axis
        bigSphere.transform.localScale = new Vector3(height, height, height);
        projectionSphere.transform.localScale = new Vector3(height, 0.1f, height);

        Renderer renderer = bigSphere.GetComponent<Renderer>();
        renderer.material = wireframeMaterial;
        renderer = projectionSphere.GetComponent<Renderer>();
        renderer.material = projectionframeMaterial1;


        // Create the small sphere
        smallSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        smallSphere.transform.position = new Vector3(smallSphereStartPosition.x, 0.0f, smallSphereStartPosition.y);
        smallSphere.transform.localScale = new Vector3(1, 0.1f, 1) * smallSphereHeight;

        // Create the intersection sphere
        intersectionSphereCalc = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        intersectionSphereCalc.transform.localScale = Vector3.one * intersectionSphereCalcHeight;

        TrailRenderer trail = intersectionSphereCalc.AddComponent<TrailRenderer>();

        // Set the trail's properties
        trail.time = 1.0f; // Duration the trail will persist
        trail.minVertexDistance = 0.1f; // Smoothness of the trail
        trail.startWidth = 0.25f; // Starting width of the trail
        trail.endWidth = 0.0f; // Ending width of the trail

        // Optionally assign a material to the TrailRenderer
        trail.material = new Material(Shader.Find("Sprites/Default")); // Basic material
        trail.material.color = Color.red; // Change trail color to red

        renderer = intersectionSphereCalc.GetComponent<Renderer>();
        renderer.material = wireframeMaterial1;


        // Add a LineRenderer to connect the spheres
        GameObject lineObject = new GameObject("ConnectingLine");
        lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Set LineRenderer properties
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = trailMaterial;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        intersectionSphereCalc.transform.SetParent(center.transform, false);

        // Initialize the line positions
        HandleKey();
    }

    private void Update()
    {
        // Allow movement of the small sphere on X and Z using arrow keys
        bool update;

        if (axisMode)
        {
            update = AxisMode();
        }
        else
        {
            update = SphereMode();
        }

        // Handle mouse click
        if (Input.GetMouseButton(0) || !axisMode) // Left mouse button
        {
            HandleClick();
        }
        else
        if (update)
        {
            HandleKey();
        }
        if (Input.GetMouseButtonDown(2)) // Left mouse button
        {
            axisMode = !axisMode;
        }
        // Update the line and intersection dynamically
        UpdateLine();

        // Update the TMPro texts with the calculated values
        UpdateTextFields();
    }
    public bool SphereMode()
    {
        float moveSpeed = 5f * Time.deltaTime;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            center.transform.Rotate(moveSpeed * Time.deltaTime * 1000, 0, 0);
            return true;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            center.transform.Rotate(-moveSpeed * Time.deltaTime * 1000, 0, 0);
            return true;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            center.transform.Rotate(0, 0, moveSpeed * Time.deltaTime * 1000);
            return true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            center.transform.Rotate(0, 0, -moveSpeed * Time.deltaTime * 1000);
            return true;
        }
        if (Input.GetKey(KeyCode.Return))
        {
            center.transform.eulerAngles = Vector3.zero;
            intersectionSphereCalc.transform.position = Vector3.zero;
            return true;
        }
        return false;
    }

    public bool AxisMode()
    {
        float moveSpeed = 5f * Time.deltaTime;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            smallSphere.transform.position += Vector3.right * moveSpeed;
            return true;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            smallSphere.transform.position += Vector3.left * moveSpeed;
            return true;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            smallSphere.transform.position += Vector3.forward * moveSpeed;
            return true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            smallSphere.transform.position += Vector3.back * moveSpeed;
            return true;
        }
        if (Input.GetKey(KeyCode.Return))
        {
            smallSphere.transform.position = Vector3.zero;
            return true;
        }
        return false;
    }
    private void HandleClick()
    {
        // Create a ray from the camera to the mouse position
        if (Input.GetMouseButton(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits the bigSphere
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == bigSphere)
                {
                    // Move the intersectionSphereCalc to the hit point
                    intersectionSphereCalc.transform.position = hit.point;
                }
            }
        }
        Vector3 intersectionPoint2 = ReverseStereographicProjection(intersectionSphereCalc.transform.position);
        smallSphere.transform.position = intersectionPoint2;
    }

    private void HandleKey()
    {
        print("key");
        // Define the top positions of both spheres
        Vector3 smallSphereTop = smallSphere.transform.position;

        // Update the intersection sphere's position
        Vector3 intersectionPoint2 = StereographicProjection(smallSphereTop);
        intersectionSphereCalc.transform.position = intersectionPoint2;
    }

    private void UpdateLine()
    {
        // Update the line's positions
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, smallSphere.transform.position);  // Start at small sphere
        lineRenderer.SetPosition(1, bigSphere.transform.position + Vector3.up * (height / 2.0f));  // Move to top of big sphere
    }
    public Vector3 ReverseStereographicProjection(Vector3 end)
    {
        float x = end.x / (1.0f - (end.y / height));
        float z = end.z / (1.0f - (end.y / height));
        return new Vector3(x,0, z);
    }

    public Vector3 StereographicProjection(Vector3 end)
    {
        float x = (height*height * end.x) / (end.x * end.x + end.z * end.z + height * height);
        float y = height-(height * height *height/ (end.x * end.x + end.z * end.z + height * height));
        float z = (height * height * end.z) / (end.x * end.x + end.z * end.z + height * height);
        return new Vector3(x, y, z);
    }

    private void UpdateTextFields()
    {
        // Update the text elements
        textW.text = "w = " + smallSphere.transform.position.x.ToString("F2") + "+ i*"+smallSphere.transform.position.z.ToString("F2");
        textP.text = "P = " + intersectionSphereCalc.transform.position.x.ToString("F2") + "+ i*" + intersectionSphereCalc.transform.position.z.ToString("F2");
        mode.text = axisMode ? "XoY":"Rotation";
    }

    private void CreateLine(Vector3 direction, Color color)
    {
        // Create a new GameObject for the line
        GameObject line = new GameObject($"Line_{color}");
        LineRenderer lr = line.AddComponent<LineRenderer>();

        // Set LineRenderer properties
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 2;
        lr.material = new Material(Shader.Find("Sprites/Default")); // Basic material
        lr.startColor = color;
        lr.endColor = color;

        // Set line positions
        Vector3 origin = transform.position;
        lr.SetPosition(0, origin); // Start point
        lr.SetPosition(1, origin + direction * lineLength); // End point
        line.transform.SetParent(pivot, false);
    }
}
