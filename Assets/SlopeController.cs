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
        if (IsOwner)
        {
            slopeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        // Initial update
        UpdateLineRenderer(networkedSlope.Value);

        // Listen for changes in the networked variable
        networkedSlope.OnValueChanged += OnNetworkedSlopeChanged;
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
        // Assuming the line starts at (0,0) and ends at (1, slope)
        Vector3[] positions = new Vector3[2];
        positions[0] = new Vector3(6.5f, 1, 0);
        positions[1] = new Vector3(6.5f, 2, slope);
        lineRenderer.SetPositions(positions);
    }
}