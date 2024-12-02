using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    private Vector3 target = Vector3.zero;
    public int gridSize = 32;
    public float distance = 10.0f; 
    public float orbitSpeed = 50.0f; 
    public float height = 5.0f; 
    private Vector3 adjustment;
    private float angle;

    void Start()
    {
       
        if (target != null)
        {
            angle = transform.eulerAngles.y;
            UpdateCameraPosition();
        }
    }

    void LateUpdate()
    {
        angle += orbitSpeed * Time.deltaTime; 
        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        adjustment = gridSize/2 * Vector3.one;

        Vector3 offset = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * distance, height,
            Mathf.Cos(angle * Mathf.Deg2Rad) * distance);
        transform.position = adjustment + offset;
        
        transform.LookAt(target + adjustment);
    }
}