using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections.Generic;

[System.Serializable]
public class LineRendererData
{
    public Slider slider;
    public LineRenderer lineRenderer;
}

public class SlopeController : NetworkBehaviour
{
    public List<LineRendererData> lineRendererDataList; // List of LineRendererData, each containing a Slider and a LineRenderer
    public GameObject door1; // Reference to door 1
    public GameObject door2; // Reference to door 2
    public Vector3 doorOpenPosition1; // The position to move door 1 to when it opens
    public Vector3 doorOpenPosition2; // The position to move door 2 to when it opens
    public float doorOpenSpeed = 2f; // Speed at which the door opens

    private List<NetworkVariable<float>> sliderValues;

    private void Start()
    {
        // Ensure at least one LineRendererData is assigned
        if (lineRendererDataList.Count == 0)
        {
            Debug.LogError("SlopeController: No LineRendererData assigned.");
            return;
        }

        // Initialize NetworkVariables for each LineRendererData
        sliderValues = new List<NetworkVariable<float>>();
        for (int i = 0; i < lineRendererDataList.Count; i++)
        {
            var sliderValue = new NetworkVariable<float>(lineRendererDataList[i].slider.value);
            sliderValues.Add(sliderValue);

            int index = i; // Capture the current index for the closure
            sliderValue.OnValueChanged += (oldValue, newValue) => OnSliderValueChanged(index, newValue);

            if (lineRendererDataList[i].slider != null && lineRendererDataList[i].lineRenderer != null)
            {
                lineRendererDataList[i].slider.onValueChanged.AddListener(delegate { OnLocalSliderValueChanged(index); });
                UpdateLineRenderer(lineRendererDataList[i].lineRenderer, lineRendererDataList[i].slider.value);
            }
            else
            {
                Debug.LogError("SlopeController: LineRendererData is missing Slider or LineRenderer.");
            }
        }
    }

    private void OnDestroy()
    {
        // Cleanup event listeners
        foreach (LineRendererData data in lineRendererDataList)
        {
            if (data.slider != null)
            {
                data.slider.onValueChanged.RemoveAllListeners();
            }
        }
    }

    private void OnLocalSliderValueChanged(int index)
    {
        // Update the NetworkVariable with the new Slider value
        if (IsServer)
        {
            sliderValues[index].Value = lineRendererDataList[index].slider.value;
        }
        else
        {
            SubmitSliderValueServerRpc(index, lineRendererDataList[index].slider.value);
        }
    }

    [ServerRpc]
    private void SubmitSliderValueServerRpc(int index, float value)
    {
        sliderValues[index].Value = value;
    }

    private void OnSliderValueChanged(int index, float newValue)
    {
        // Update the LineRenderer associated with the Slider
        UpdateLineRenderer(lineRendererDataList[index].lineRenderer, newValue);
    }

    private void UpdateLineRenderer(LineRenderer renderer, float slope)
    {
        // Define the fixed length of the line
        float length = 1.0f;

        // Calculate the z component of the end point based on the fixed length and slope
        float deltaZ = length / Mathf.Sqrt(1 + slope * slope);

        Vector3[] positions = new Vector3[2];
        positions[0] = new Vector3(6.5f, 1.5f, 0); // Start point
        positions[1] = new Vector3(6.5f, 1.5f + (slope * deltaZ), 0 + deltaZ); // End point

        renderer.SetPositions(positions);
    }
}