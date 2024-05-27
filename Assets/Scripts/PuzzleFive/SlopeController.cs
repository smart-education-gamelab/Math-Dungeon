using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SliderLineRendererPair
{
    public Slider slider;
    public LineRenderer lineRenderer;
    public float targetValue; // Specifieke doelwaarde voor elke slider
}

public class SlopeController : NetworkBehaviour
{
    public List<SliderLineRendererPair> sliderLineRendererPairs; // Lijst van slider-LineRenderer paren
    public GameObject door1; // Referentie naar deur 1
    public GameObject door2; // Referentie naar deur 2
    public Vector3 doorOpenPosition1; // De positie waarnaar deur 1 moet bewegen als deze opent
    public Vector3 doorOpenPosition2; // De positie waarnaar deur 2 moet bewegen als deze opent
    public float doorOpenSpeed = 2f; // Snelheid waarmee de deur opent

    private List<NetworkVariable<float>> networkedSlopes = new List<NetworkVariable<float>>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Zorg ervoor dat de lineRenderers, slopeSliders en deuren zijn toegewezen
        if (sliderLineRendererPairs.Count == 0 || door1 == null || door2 == null)
        {
            Debug.LogError("SlopeController: Ontbrekende referenties naar LineRenderers, Sliders, of Deuren.");
            return;
        }

        for (int i = 0; i < sliderLineRendererPairs.Count; i++)
        {
            NetworkVariable<float> networkedSlope = new NetworkVariable<float>(0.5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
            networkedSlopes.Add(networkedSlope);

            if (IsOwner)
            {
                int index = i; // Index vastleggen voor gebruik in de lambda
                sliderLineRendererPairs[i].slider.onValueChanged.AddListener(value => OnSliderValueChanged(index, value));
            }

            // Initiële update
            UpdateLineRenderer(sliderLineRendererPairs[i].lineRenderer, networkedSlopes[i].Value);

            // Luister naar veranderingen in de genetwerkte variabele
            int capturedIndex = i; // Capture index for use in lambda
            networkedSlopes[i].OnValueChanged += (oldValue, newValue) => OnNetworkedSlopeChanged(capturedIndex, newValue);
        }
    }

    private void OnDestroy()
    {
        // Opruimen van event listeners
        if (IsOwner)
        {
            for (int i = 0; i < sliderLineRendererPairs.Count; i++)
            {
                sliderLineRendererPairs[i].slider.onValueChanged.RemoveAllListeners();
            }
        }

        for (int i = 0; i < networkedSlopes.Count; i++)
        {
            int index = i; // Index vastleggen voor gebruik in de lambda
            networkedSlopes[i].OnValueChanged -= (oldValue, newValue) => OnNetworkedSlopeChanged(index, newValue);
        }
    }

    private void OnSliderValueChanged(int index, float value)
    {
        if (IsOwner)
        {
            if (index >= 0 && index < networkedSlopes.Count)
            {
                networkedSlopes[index].Value = value;

                // Controleer of alle sliders op hun doelwaarden staan en vraag om de deuren te openen als dat zo is
                if (AllSlidersAtTargetValue())
                {
                    RequestOpenDoorsServerRpc();
                }
            }
            else
            {
                Debug.LogError("Index out of range in OnSliderValueChanged: " + index);
            }
        }
    }

    private void OnNetworkedSlopeChanged(int index, float newValue)
    {
        if (index >= 0 && index < sliderLineRendererPairs.Count)
        {
            UpdateLineRenderer(sliderLineRendererPairs[index].lineRenderer, newValue);

            // Controleer of alle sliders op hun doelwaarden staan en open de deuren als dat zo is
            if (AllSlidersAtTargetValue())
            {
                OpenDoors();
            }
        }
        else
        {
            Debug.LogError("Index out of range in OnNetworkedSlopeChanged: " + index);
        }
    }

    private bool AllSlidersAtTargetValue()
    {
        foreach (var pair in sliderLineRendererPairs)
        {
            if (Mathf.Abs(pair.slider.value - pair.targetValue) > 0.01f) // Gebruik een kleine drempel om rekening te houden met floating-point precisie
            {
                return false;
            }
        }
        return true;
    }

    private void UpdateLineRenderer(LineRenderer renderer, float slope)
    {
        // Definieer de vaste lengte van de lijn
        float length = 1.0f;

        // Bereken de z-component van het eindpunt op basis van de vaste lengte en helling
        float deltaZ = length / Mathf.Sqrt(1 + slope * slope);

        Vector3[] positions = new Vector3[2];
        positions[0] = new Vector3(6.5f, 1.5f, 0); // Startpunt
        positions[1] = new Vector3(6.5f, 1.5f + (slope * deltaZ), 0 + deltaZ); // Eindpunt

        renderer.SetPositions(positions);
    }

    [ServerRpc]
    private void RequestOpenDoorsServerRpc()
    {
        // Open de deuren op de server en verwittig alle clients
        OpenDoorsClientRpc();
    }

    [ClientRpc]
    private void OpenDoorsClientRpc()
    {
        // Implementeer de logica om beide deuren te openen
        OpenDoors();
    }

    private void OpenDoors()
    {
        // Start coroutines om beide deuren soepel te openen
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
        }

        door.transform.localPosition = targetPosition;
    }
}
