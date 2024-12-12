using UnityEngine;

public class Look : MonoBehaviour
{
    public Transform target; // The object to look around
    public float distance = 5.0f; // Distance from the target
    public float sensitivity = 2.0f; // Mouse sensitivity
    public float minY = -20f; // Minimum vertical angle
    public float maxY = 80f; // Maximum vertical angle
    public float zoomSpeed = 2.0f; // Speed of zooming
    public float minDistance = 2.0f; // Minimum zoom distance
    public float maxDistance = 10.0f; // Maximum zoom distance

    private float currentX = 0f; // Current horizontal angle
    private float currentY = 0f; // Current vertical angle

    void Start()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get mouse input for rotation
        currentX += .5f;
        currentY = 30;

        // Clamp the vertical angle
        currentY = Mathf.Clamp(currentY, minY, maxY);

        // Get mouse scroll input for zooming
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        distance -= scrollInput * zoomSpeed; // Adjust distance based on scroll input
        distance = Mathf.Clamp(distance, minDistance, maxDistance); // Clamp the distance

        // Calculate the average position of all children
        Vector3 averagePosition = CalculateAveragePosition(target);

        // Calculate the new position and rotation
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = averagePosition + rotation * direction;
        transform.LookAt(averagePosition);
    }

    private Vector3 CalculateAveragePosition(Transform target)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        // Iterate through all children of the target
        foreach (Transform child in target)
        {
            sum += child.position;
            count++;
        }

        // Return the average position
        return count > 0 ? sum / count : target.position; // Fallback to target position if no children
    }
}