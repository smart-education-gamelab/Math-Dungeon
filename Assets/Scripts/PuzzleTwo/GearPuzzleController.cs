using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Unity.Netcode;

public class GearPuzzleController : NetworkBehaviour {
    public enum PuzzleOptions {
        Option1,
        Blabla,
        Option55
    }

    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private PuzzleOptions selectedPuzzle;

	[SerializeField]
	private GameObject bigGearPrefab;

    [SerializeField]
    private List<Transform> smallGearSpawnPoints = new List<Transform>();

    private FormulaGenerator formulaGenerator;
    private Dictionary<string, float[]> formulasAndSolutionsControllerCopy;
    private List<GameObject> spawnedGears = new List<GameObject>();

    public PuzzleOptions GetSelectedPuzzle() {
        return selectedPuzzle;
    }

    // Start is called before the first frame update
    void Start() {
        Debug.Log("Selected option: " + selectedPuzzle);

        // Haal het FormulaGenerator-script op via GetComponent
        formulaGenerator = GetComponent<FormulaGenerator>();

        // Controleer of het FormulaGenerator-script is gevonden
        if(formulaGenerator != null) {
            // Roep GenerateFormulas aan via de referentie naar FormulaGenerator
            formulaGenerator.GenerateFormulas();
        } else {
            Debug.LogError("FormulaGenerator component not found!");
        }

        SpawnGears();
    }

    public void SpawnGears() {
        formulasAndSolutionsControllerCopy = GetComponent<FormulaGenerator>().GetFormulasAndSolutions();

        if(formulasAndSolutionsControllerCopy == null) {
            Debug.LogError("Formulas and solutions not generated!");
            return;
        }

        // Loop om het gewenste aantal tandwielen te spawnen
        for(int i = 0; i < 5; i++) {
            // Pas de positie van het bigGearPrefab aan naar de positie van het gameobject
            Vector3 spawnPosition = spawnPoint.position;

            // Maak een instantie van het bigGearPrefab op de aangepaste positie
            GameObject newBigGear = Instantiate(bigGearPrefab, spawnPosition, Quaternion.identity);
            newBigGear.GetComponent<NetworkObject>().Spawn();

            // Haal de TextMeshProUGUI-component op van newBigGear
            TextMeshProUGUI tmp = newBigGear.GetComponentInChildren<TextMeshProUGUI>();

            // Controleer of er een TMP-component is gevonden
            if(tmp != null) {
                // Controleer of het huidige indexnummer binnen de geldige bereik ligt
                if(i < formulasAndSolutionsControllerCopy.Count) {
                    // Haal de formule op uit de dictionary
                    KeyValuePair<string, float[]> formulaEntry = formulasAndSolutionsControllerCopy.ElementAt(i);
                    string formula = formulaEntry.Key;

                    // Pas de formule toe op de tekst van het TMP-object
                    tmp.text = formula;
                } else {
                    Debug.LogWarning("No formula found for big gear at index: " + i);
                }
            } else {
                Debug.LogError("TextMeshPro component not found on big gear!");
            }

            // Optioneel: Pas de positie en rotatie van newBigGear aan naar wens

            // Voeg newBigGear toe aan de lijst met gespawnede tandwielen
            spawnedGears.Add(newBigGear);
        }
    }
}