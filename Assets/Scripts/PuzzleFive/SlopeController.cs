using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;

public class SlopeController : NetworkBehaviour
{
    [Header("Slider references")]
    [SerializeField] private Slider slopeSlider1;
    [SerializeField] private Slider slopeSlider2;
    [SerializeField] private Slider slopeSlider3;

    [Header("LineRenderer references")]
    [SerializeField] private LineRenderer lineRenderer1;
    [SerializeField] private LineRenderer lineRenderer2;
    [SerializeField] private LineRenderer lineRenderer3;

    [Header("Door gameobject references")]
    [SerializeField] private GameObject door1; // Reference to door 1
    [SerializeField] private GameObject door2; // Reference to door 2

    [Header("Door values")]
    [SerializeField] private Vector3 doorOpenPosition1; // The position to move door 1 to when it opens
    [SerializeField] private Vector3 doorOpenPosition2; // The position to move door 2 to when it opens
    [SerializeField] private float doorOpenSpeed = 2f; // Speed at which the door opens

    [Header("Answer values")]
    [SerializeField]
    private float answerOne = 2f;
    [SerializeField]
    private float answerTwo = -1f;
    [SerializeField]
    private float answerThree = -2f;

    // Networked variables to sync the slope values
    private NetworkVariable<float> networkedSlope1 = new NetworkVariable<float>(0.5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> networkedSlope2 = new NetworkVariable<float>(0.5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> networkedSlope3 = new NetworkVariable<float>(0.5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        // Ensure the lineRenderers, slopeSliders, and doors are assigned
        if (lineRenderer1 == null || lineRenderer2 == null || lineRenderer3 == null ||
            slopeSlider1 == null || slopeSlider2 == null || slopeSlider3 == null ||
            door1 == null || door2 == null)
        {
            Debug.LogError("SlopeController: Missing references to LineRenderers, Sliders, or Doors.");
            return;
        }

        if (IsOwner)
        {
            slopeSlider1.onValueChanged.AddListener(value => OnSliderValueChanged(1, value));
            slopeSlider2.onValueChanged.AddListener(value => OnSliderValueChanged(2, value));
            slopeSlider3.onValueChanged.AddListener(value => OnSliderValueChanged(3, value));
        }

        // Initial update
        UpdateLineRenderer(lineRenderer1, networkedSlope1.Value, 1);
        UpdateLineRenderer(lineRenderer2, networkedSlope2.Value, 2);
        UpdateLineRenderer(lineRenderer3, networkedSlope3.Value, 3);

        // Listen for changes in the networked variables
        networkedSlope1.OnValueChanged += (oldValue, newValue) => OnNetworkedSlopeChanged(1, newValue);
        networkedSlope2.OnValueChanged += (oldValue, newValue) => OnNetworkedSlopeChanged(2, newValue);
        networkedSlope3.OnValueChanged += (oldValue, newValue) => OnNetworkedSlopeChanged(3, newValue);
    }

    private void OnDestroy()
    {
        // Cleanup event listeners
        if (IsOwner)
        {
            slopeSlider1.onValueChanged.RemoveAllListeners();
            slopeSlider2.onValueChanged.RemoveAllListeners();
            slopeSlider3.onValueChanged.RemoveAllListeners();
        }

        networkedSlope1.OnValueChanged -= (oldValue, newValue) => OnNetworkedSlopeChanged(1, newValue);
        networkedSlope2.OnValueChanged -= (oldValue, newValue) => OnNetworkedSlopeChanged(2, newValue);
        networkedSlope3.OnValueChanged -= (oldValue, newValue) => OnNetworkedSlopeChanged(3, newValue);
    }

    private void OnSliderValueChanged(int sliderIndex, float value)
    {
        if (IsOwner)
        {
            switch (sliderIndex)
            {
                case 1:
                    networkedSlope1.Value = value;
                    break;
                case 2:
                    networkedSlope2.Value = value;
                    break;
                case 3:
                    networkedSlope3.Value = value;
                    break;
            }
        }
    }

    private void OnNetworkedSlopeChanged(int sliderIndex, float newValue)
    {
        switch (sliderIndex)
        {
            case 1:
                UpdateLineRenderer(lineRenderer1, newValue, 1);
                break;
            case 2:
                UpdateLineRenderer(lineRenderer2, newValue, 2);
                break;
            case 3:
                UpdateLineRenderer(lineRenderer3, newValue, 3);
                break;
        }

        if (AllSlidersSetToTarget())
        {
            RequestOpenDoorsServerRpc();
        }
    }

    private bool AllSlidersSetToTarget()
    {
        return networkedSlope1.Value == answerOne && networkedSlope2.Value == answerTwo && networkedSlope3.Value == answerThree;
    }

    private void UpdateLineRenderer(LineRenderer lineRenderer, float slope, int lineIndex)
    {
        float length = 1.0f;
        float deltaZ, deltaY;
        Vector3[] positions = new Vector3[2];

        switch (lineIndex)
        {
            case 1:
                deltaZ = length / Mathf.Sqrt(1 + slope * slope);
                positions[0] = new Vector3(6.5f, 1.5f, 0); // Start point
                positions[1] = new Vector3(6.5f, 1.5f + (slope * deltaZ), 0 + deltaZ); // End point
                break;
            case 2:
                deltaY = length / Mathf.Sqrt(1 + slope * slope);
                positions[0] = new Vector3(8.8f, 1.1f, -5.65f); // Start point
                positions[1] = new Vector3(8.8f + deltaY, 1.1f + (slope * deltaY), -5.65f); // End point
                break;
            case 3:
                deltaZ = length / Mathf.Sqrt(1 + slope * slope);
                positions[0] = new Vector3(6.5f, 1.5f, 0); // Start point
                positions[1] = new Vector3(6.5f, 1.5f + (slope * deltaZ), 0 + deltaZ); // End point
                break;
        }

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
