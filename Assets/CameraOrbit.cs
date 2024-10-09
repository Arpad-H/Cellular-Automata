using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public float distance = 10.0f; 
    public float orbitSpeed = 50.0f; 
    public float height = 5.0f; 
    public int gridSize;
    private Vector3 adjustment;
    private float angle;

    void Start()
    {
        gridSize = GameObject.Find("Controller").GetComponent<GameOfLifeSystem>().gridSize;
        adjustment = gridSize/2 * Vector3.one;
        
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
        Vector3 offset = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * distance, height,
            Mathf.Cos(angle * Mathf.Deg2Rad) * distance);
        transform.position = adjustment + offset;
        
        transform.LookAt(target.position + adjustment);
    }
}