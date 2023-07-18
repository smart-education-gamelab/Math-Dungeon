using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GearPuzzleController : MonoBehaviour {
    public enum PuzzleOptions {
        Option1,
        Blabla,
        Option55
    }

    public PuzzleOptions selectedPuzzle;

    [SerializeField]
    private List<GameObject> smallGearList = new List<GameObject>();
    [SerializeField]
    private List<GameObject> bigGearList = new List<GameObject>();

	private Dictionary<string, float[]> formulasAndSolutionsControllerCopy;

	// Start is called before the first frame update
	void Start() {
        Debug.Log("Selected option: " + selectedPuzzle);
    }

	// Update is called once per frame
	void Update() {
		formulasAndSolutionsControllerCopy = this.gameObject.GetComponent<FormulaGenerator>().GetFormulasAndSolutions();

		if(formulasAndSolutionsControllerCopy == null) {
			Debug.LogError("Formulas and solutions not generated!");
			return;
		}

		// Loop door alle grote tandwielen
		for(int i = 0; i < bigGearList.Count; i++) {
			GameObject bigGear = bigGearList[i];
			TextMeshProUGUI tmp = bigGear.GetComponentInChildren<TextMeshProUGUI>();

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
		}
	}
}
