using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;

public class SlopeController : NetworkBehaviour
{
    public Slider[] slopeSliders;
    public LineRenderer[] lineRenderers;
    public GameObject door1; // Reference to door 1
    public GameObject door2; // Reference to door 2
    public Vector3 doorOpenPosition1; // The position to move door 1 to when it opens
    public Vector3 doorOpenPosition2; // The position to move door 2 to when it opens
    public float doorOpenSpeed = 2f; // Speed at which the door opens

    private NetworkVariable<float>[] networkedSlopes;
    private float[] targetValues = { 1f, 1.5f, 2f }; // Target values for sliders

    private void Awake()
    {
        if (slopeSliders.Length != lineRenderers.Length)
        {
            Debug.LogError("The number of sliders must match the number of line renderers.");
            return;
        }

        networkedSlopes = new NetworkVariable<float>[slopeSliders.Length];
        for (int i = 0; i < slopeSliders.Length; i++)
        {
            networkedSlopes[i] = new NetworkVariable<float>(0.5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        }
    }

    private void Start()
    {
        if (lineRenderers.Length == 0 || slopeSliders.Length == 0 || door1 == null || door2 == null)
        {
            Debug.LogError("SlopeController: Missing references to LineRenderers, Sliders, or Doors.");
            return;
        }

        if (IsOwner)
        {
            for (int i = 0; i < slopeSliders.Length; i++)
            {
                int index = i; // Local copy of the loop variable for the lambda expression
                slopeSliders[i].onValueChanged.AddListener(value => OnSliderValueChanged(index, value));
            }
        }

        for (int i = 0; i < lineRenderers.Length; i++)
        {
            UpdateLineRenderer(lineRenderers[i], networkedSlopes[i].Value);
            int index = i; // Local copy for the lambda
            networkedSlopes[i].OnValueChanged += (oldValue, newValue) => OnNetworkedSlopeChanged(index, newValue);
        }
    }

    private void OnDestroy()
    {
        if (IsOwner)
        {
            for (int i = 0; i < slopeSliders.Length; i++)
            {
                slopeSliders[i].onValueChanged.RemoveAllListeners();
            }
        }

        for (int i = 0; i < networkedSlopes.Length; i++)
        {
            networkedSlopes[i].OnValueChanged -= (oldValue, newValue) => OnNetworkedSlopeChanged(i, newValue);
        }
    }

    private void OnSliderValueChanged(int index, float value)
    {
        if (IsOwner)
        {
            networkedSlopes[index].Value = value;

            CheckAndRequestOpenDoors();
        }
    }

    private void OnNetworkedSlopeChanged(int index, float newValue)
    {
        UpdateLineRenderer(lineRenderers[index], newValue);

        CheckAndRequestOpenDoors();
    }

    private void CheckAndRequestOpenDoors()
    {
        // Check if all sliders are at their target values
        for (int i = 0; i < networkedSlopes.Length; i++)
        {
            if (networkedSlopes[i].Value != targetValues[i])
            {
                return;
            }
        }

        // All sliders are at their target values
        RequestOpenDoorsServerRpc();
    }

    private void UpdateLineRenderer(LineRenderer lineRenderer, float slope)
    {
        float length = 1.0f;
        float deltaZ = length / Mathf.Sqrt(1 + slope * slope);

        Vector3[] positions = new Vector3[2];
        positions[0] = new Vector3(6.5f, 1.5f, 0); // Start point
        positions[1] = new Vector3(6.5f, 1.5f + (slope * deltaZ), 0 + deltaZ); // End point

        lineRenderer.SetPositions(positions);
    }

    [ServerRpc]
    private void RequestOpenDoorsServerRpc()
    {
        OpenDoorsClientRpc();
    }

    [ClientRpc]
    private void OpenDoorsClientRpc()
    {
        OpenDoors();
    }

    private void OpenDoors()
    {
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
