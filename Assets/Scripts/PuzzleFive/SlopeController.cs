using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;

public class SlopeController : NetworkBehaviour
{
    public Slider slopeSlider1;
    public Slider slopeSlider2;
    public Slider slopeSlider3;
    public LineRenderer lineRenderer1;
    public LineRenderer lineRenderer2;
    public LineRenderer lineRenderer3;
    public GameObject door1; // Reference to door 1
    public GameObject door2; // Reference to door 2
    public Vector3 doorOpenPosition1; // The position to move door 1 to when it opens
    public Vector3 doorOpenPosition2; // The position to move door 2 to when it opens
    public float doorOpenSpeed = 2f; // Speed at which the door opens

    // Networked variables to sync the slope values
    private NetworkVariable<float> networkedSlope1 = new NetworkVariable<float>(0.5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> networkedSlope2 = new NetworkVariable<float>(0.5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> networkedSlope3 = new NetworkVariable<float>(0.5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        // Ensure the lineRenderers, slopeSliders, and doors are assigned
        if (lineRenderer1 == null || lineRenderer2 == null || lineRenderer3 == null || slopeSlider1 == null || slopeSlider2 == null || slopeSlider3 == null || door1 == null || door2 == null)
        {
            Debug.LogError("SlopeController: Missing references to LineRenderers, Sliders, or Doors.");
            return;
        }

        if (IsOwner)
        {
            slopeSlider1.onValueChanged.AddListener(OnSlider1ValueChanged);
            slopeSlider2.onValueChanged.AddListener(OnSlider2ValueChanged);
            slopeSlider3.onValueChanged.AddListener(OnSlider3ValueChanged);
        }

        // Initial update
        UpdateLineRenderer(lineRenderer1, networkedSlope1.Value);
        UpdateLineRenderer(lineRenderer2, networkedSlope2.Value);
        UpdateLineRenderer(lineRenderer3, networkedSlope3.Value);

        // Listen for changes in the networked variables
        networkedSlope1.OnValueChanged += OnNetworkedSlope1Changed;
        networkedSlope2.OnValueChanged += OnNetworkedSlope2Changed;
        networkedSlope3.OnValueChanged += OnNetworkedSlope3Changed;
    }

    private void OnDestroy()
    {
        // Cleanup event listeners
        if (IsOwner)
        {
            if (slopeSlider1 != null) slopeSlider1.onValueChanged.RemoveListener(OnSlider1ValueChanged);
            if (slopeSlider2 != null) slopeSlider2.onValueChanged.RemoveListener(OnSlider2ValueChanged);
            if (slopeSlider3 != null) slopeSlider3.onValueChanged.RemoveListener(OnSlider3ValueChanged);
        }

        networkedSlope1.OnValueChanged -= OnNetworkedSlope1Changed;
        networkedSlope2.OnValueChanged -= OnNetworkedSlope2Changed;
        networkedSlope3.OnValueChanged -= OnNetworkedSlope3Changed;
    }

    private void OnSlider1ValueChanged(float value)
    {
        if (IsOwner)
        {
            networkedSlope1.Value = value;

            if (value == 2)
            {
                RequestOpenDoorsServerRpc();
            }
        }
    }

    private void OnSlider2ValueChanged(float value)
    {
        if (IsOwner)
        {
            networkedSlope2.Value = value;

            if (value == 2)
            {
                RequestOpenDoorsServerRpc();
            }
        }
    }

    private void OnSlider3ValueChanged(float value)
    {
        if (IsOwner)
        {
            networkedSlope3.Value = value;

            if (value == 2)
            {
                RequestOpenDoorsServerRpc();
            }
        }
    }

    private void OnNetworkedSlope1Changed(float oldValue, float newValue)
    {
        UpdateLineRenderer(lineRenderer1, newValue);

        if (newValue == 2)
        {
            OpenDoors();
        }
    }

    private void OnNetworkedSlope2Changed(float oldValue, float newValue)
    {
        UpdateLineRenderer(lineRenderer2, newValue);

        if (newValue == 2)
        {
            OpenDoors();
        }
    }

    private void OnNetworkedSlope3Changed(float oldValue, float newValue)
    {
        UpdateLineRenderer(lineRenderer3, newValue);

        if (newValue == 2)
        {
            OpenDoors();
        }
    }

    private void UpdateLineRenderer(LineRenderer lineRenderer, float slope)
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
        OpenDoors();
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
        Vector3 startPosition = door.transform.localPosition;

        while (timeElapsed < doorOpenSpeed)
        {
            door.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, timeElapsed / doorOpenSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;

            if (Vector3.Distance(door.transform.localPosition, targetPosition) < 0.01f)
            {
                break;
            }
        }

        door.transform.localPosition = targetPosition;
    }
}