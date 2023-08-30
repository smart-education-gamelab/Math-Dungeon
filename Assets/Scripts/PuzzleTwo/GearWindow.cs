using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GearWindow : MonoBehaviour {

    public int startingValue = 2; // Beginwaarde voor het getal op het raam
    public int currentValue; // Huidige waarde op het raam

    public List<GameObject> passedGears; // Lijst van gears die al zijn doorgegeven
    //private
    private List<TextMeshProUGUI> tmps;

    // Start is called before the first frame update
    void Start() {
        currentValue = startingValue;
        passedGears = new List<GameObject>();

        tmps = new List<TextMeshProUGUI>();

        // Voeg de TextMeshProUGUI-componenten van de children toe aan de lijst tmps
        TextMeshProUGUI[] textComponents = GetComponentsInChildren<TextMeshProUGUI>();
        tmps.AddRange(textComponents);

        UpdateWindowValue();
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnTriggerEnter(Collider other) {
        // Controleer of het object dat het raam raakt een gear is
        GrabableGear gear = other.GetComponent<GrabableGear>();
        if(gear != null) {
            // Controleer of de gear al eerder is doorgegeven
            if(!passedGears.Contains(gear.gameObject)) {
                // Verlaag de waarde op het raam met 1
                currentValue -= 1;

                // Update de tekst op het raam met de nieuwe waarde
                UpdateWindowValue();

                // Voeg de gear toe aan de lijst van doorgegeven gears
                passedGears.Add(gear.gameObject);

                // Controleer of de waarde op het raam nul is
                if(currentValue == 0) {
                    // Reset de doorgegeven gears en de waarde op het raam
                    ResetPuzzle();
                }
            }
        }
    }

    private void UpdateWindowValue() {
        // TODO: Implementeer de logica om de tekst op het raam bij te werken
        foreach(TextMeshProUGUI tmp in tmps) {
            // Pas de tekst van de TextMeshProUGUI-componenten aan met de nieuwe waarde
            tmp.text = currentValue.ToString();
        }
    }

    private void ResetPuzzle() {
        Loader.LoadNetwork(Loader.Scene.PuzzleTwoGears);
    }
}
