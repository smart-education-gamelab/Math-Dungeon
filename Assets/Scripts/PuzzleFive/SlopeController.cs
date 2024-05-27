using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class SlopeController : NetworkBehaviour
{
    public Slider slopeSlider;
    public LineRenderer lineRenderer;
    public GameObject door1; // Reference to the door object
    public GameObject door2;
    public Vector3 doorOpenPosition1; // The position to move the door to when it opens
    public Vector3 doorOpenPosition2;

    // Networked variable to sync the slope value
    private NetworkVariable<float> networkedSlope = new NetworkVariable<float>(0.5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        // Ensure the lineRenderer, slopeSlider, and door are assigned
        if (lineRenderer == null || slopeSlider == null || door1 == null || door2 == null)
        {
            Debug.LogError("SlopeController: Missing references to LineRenderer, Slider, or Door.");
            return;
        }

        if (IsOwner)
        {
            slopeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        // Initial update
        UpdateLineRenderer(networkedSlope.Value);

        // Listen for changes in the networked variable
        networkedSlope.OnValueChanged += OnNetworkedSlopeChanged;
    }

    private void OnDestroy()
    {
        // Cleanup event listeners
        if (IsOwner && slopeSlider != null)
        {
            slopeSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }

        networkedSlope.OnValueChanged -= OnNetworkedSlopeChanged;
    }

    private void OnSliderValueChanged(float value)
    {
        if (IsOwner)
        {
            networkedSlope.Value = value;

            // Check if the slider value is 1 and open the door if it is
            if (value == 1f)
            {
                OpenDoor();
            }
        }
    }

    private void OnNetworkedSlopeChanged(float oldValue, float newValue)
    {
        UpdateLineRenderer(newValue);

        // Check if the slider value is 1 and open the door if it is
        if (newValue == 1f)
        {
            OpenDoor();
        }
    }

    private void UpdateLineRenderer(float slope)
    {
        // Define the fixed length of the line
        float length = 1.0f;

        // Calculate the z component of the end point based on the fixed length and slope
        float deltaZ = length / Mathf.Sqrt(1 + slope * slope);

        Vector3[] positions = new Vector3[2];
        positions[0] = new Vector3(6.5f, 1.5f, 0); // Start point
        positions[1] = new Vector3(6.5f, 1.5f + (slope * deltaZ), 0 + deltaZ); // End point

        lineRenderer.SetPositions(positions);
    }

    private void OpenDoor()
    {
        // Implement the logic to open the door
        door1.transform.position = doorOpenPosition1;
        door2.transform.position = doorOpenPosition2;
        Debug.Log("Door opened!");
    }
}
