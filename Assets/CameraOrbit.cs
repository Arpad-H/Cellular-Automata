using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; // The target to orbit around
    public float distance = 10.0f; // Distance from the target
    public float orbitSpeed = 50.0f; // Speed of orbiting
    public float height = 5.0f; // Height above the target
    public int gridSize;
    private Vector3 adjustment;
    private float angle;

    void Start()
    {
        gridSize = GameObject.Find("Controller").GetComponent<GameOfLifeSystem>().gridSize;
        adjustment = gridSize/2 * Vector3.one;
        // Initialize the camera position
        if (target != null)
        {
            angle = transform.eulerAngles.y; // Get the initial angle
            UpdateCameraPosition();
        }
    }

    void LateUpdate()
    {
        angle += orbitSpeed * Time.deltaTime; // Update the angle based on speed
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        // Calculate the new position
        Vector3 offset = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * distance, height,
            Mathf.Cos(angle * Mathf.Deg2Rad) * distance);
        transform.position = adjustment + offset;

        // Look at the target
        transform.LookAt(target.position + adjustment);
    }
}