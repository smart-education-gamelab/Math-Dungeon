using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class SlopeController : NetworkBehaviour
{
    public Slider slopeSlider;
    public LineRenderer lineRenderer;

    // Networked variable to sync the slope value
    private NetworkVariable<float> networkedSlope = new NetworkVariable<float>(0.5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        // Ensure the lineRenderer and slopeSlider are assigned
        if (lineRenderer == null || slopeSlider == null)
        {
            Debug.LogError("SlopeController: Missing references to LineRenderer or Slider.");
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
        }
    }

    private void OnNetworkedSlopeChanged(float oldValue, float newValue)
    {
        UpdateLineRenderer(newValue);
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
}