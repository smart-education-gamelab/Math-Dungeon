using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;

public class SlopeController : NetworkBehaviour
{
    public Slider slopeSlider;
    public LineRenderer lineRenderer;
    public GameObject door1; // Reference to door 1
    public GameObject door2; // Reference to door 2
    public Vector3 doorOpenPosition1; // The position to move door 1 to when it opens
    public Vector3 doorOpenPosition2; // The position to move door 2 to when it opens
    public float doorOpenSpeed = 2f; // Speed at which the door opens

    // Networked variable to sync the slope value
    private NetworkVariable<float> networkedSlope = new NetworkVariable<float>(0.5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        // Ensure the lineRenderer, slopeSlider, and doors are assigned
        if (lineRenderer == null || slopeSlider == null || door1 == null || door2 == null)
        {
            Debug.LogError("SlopeController: Missing references to LineRenderer, Slider, or Doors.");
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

            // Check if the slider value is exactly 1 and request to open the doors if it is
            if (value == 1f)
            {
                RequestOpenDoorsServerRpc();
            }
        }
    }

    private void OnNetworkedSlopeChanged(float oldValue, float newValue)
    {
        UpdateLineRenderer(newValue);

        // Check if the slider value is exactly 1 and open the doors if it is
        if (newValue == 1f)
        {
            OpenDoors();
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

    [ServerRpc]
    private void RequestOpenDoorsServerRpc()
    {
        // Open the doors on the server and notify all clients
        OpenDoorsClientRpc();
    }

    [ClientRpc]
    private void OpenDoorsClientRpc()
    {
        // Implement the logic to open both doors
        StartCoroutine(OpenDoorSmoothly(door1, doorOpenPosition1));
        StartCoroutine(OpenDoorSmoothly(door2, doorOpenPosition2));
    }

    private void OpenDoors()
    {
        // Start coroutines to open both doors smoothly
        StartCoroutine(OpenDoorSmoothly(door1, doorOpenPosition1));
        StartCoroutine(OpenDoorSmoothly(door2, doorOpenPosition2));
    }

    private IEnumerator OpenDoorSmoothly(GameObject door, Vector3 targetPosition)
    {
        float timeElapsed = 0f;
        Vector3 startPosition = door.transform.position;

        while (timeElapsed < doorOpenSpeed)
        {
            door.transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / doorOpenSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        door.transform.position = targetPosition;
    }
}
