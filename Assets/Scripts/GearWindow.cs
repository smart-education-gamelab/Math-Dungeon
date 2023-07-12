using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearWindow : MonoBehaviour {

    public int startingValue = 7; // Beginwaarde voor het getal op het raam
    private int currentValue; // Huidige waarde op het raam

    private List<GameObject> passedGears; // Lijst van gears die al zijn doorgegeven

    // Start is called before the first frame update
    void Start() {
        currentValue = startingValue;
        passedGears = new List<GameObject>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnTriggerEnter(Collider other) {
        // Controleer of het object dat het raam raakt een gear is
        Gear gear = other.GetComponent<Gear>();
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
                    ResetPassing();
                }
            }
        }
    }

    private void UpdateWindowValue() {
        // TODO: Implementeer de logica om de tekst op het raam bij te werken
    }

    private void ResetPassing() {
        // TODO: Implementeer de logica om de doorgegeven gears te resetten en de waarde op het raam terug te zetten naar de startwaarde
    }
}
