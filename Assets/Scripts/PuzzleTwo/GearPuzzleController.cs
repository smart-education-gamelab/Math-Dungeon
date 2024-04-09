using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using Newtonsoft.Json;
using System.Collections;

public class GearPuzzleController : NetworkBehaviour {
    public enum PuzzleOptions {
        Option1,
        Blabla,
        Option55
    }

    [SerializeField]
    private int wantedSolutions;

    private int amountOfSolved;

    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
	private PuzzleOptions selectedPuzzle;

	[SerializeField]
    private GameObject smallGearPrefab;

	[SerializeField]
	private GameObject bigGearPrefab;

    [SerializeField]
    private List<Transform> smallGearSpawnPoints = new List<Transform>();

    private FormulaGenerator formulaGenerator;
    private List<GameObject> spawnedGears = new List<GameObject>();

    [SerializeField]
    private GameObject snapPointGear1;

    [SerializeField]
    private GameObject snapPointGear2;

    [SerializeField]
    private GameObject snapPointGear3;

    [SerializeField]
    private GameObject snapPointGear4;

    [SerializeField]
    private GameObject snapPointGear5;

    [SerializeField]
    private GameObject gearParent;

    // Dictionary om snap points te koppelen aan formules
    private Dictionary<GameObject, string> snapPointFormulas = new Dictionary<GameObject, string>();

    public PuzzleOptions GetSelectedPuzzle() {
        return selectedPuzzle;
    }

    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();

    public string TMPText;

    // Start is called before the first frame update
    private void Start() {
        amountOfSolved = 0;
        wantedSolutions = 5;
        Debug.Log("Selected option: " + selectedPuzzle);

        // Haal het FormulaGenerator-script op via GetComponent
        formulaGenerator = GetComponent<FormulaGenerator>();

        // Controleer of het FormulaGenerator-script is gevonden
        if(formulaGenerator != null && (IsServer || IsOwner)) {
            // Roep GenerateFormulas aan via de referentie naar FormulaGenerator
            formulaGenerator.GenerateFormulas();
            Dictionary<string, float[]> formulasAndSolutionsControllerCopy = GetComponent<FormulaGenerator>().GetFormulasAndSolutions();
            int[] solutionsCopy = GetComponent<FormulaGenerator>().GetSolutions();
            SpawnGears(formulasAndSolutionsControllerCopy, solutionsCopy);
        } else {
            Debug.LogError("FormulaGenerator component not found!");
        }

        

        //tmp = GetComponent<TextMeshProUGUI>();

        /*// Listen for changes in the TMP text value
        TMPText.OnValueChanged += OnTMPTextChanged;*/
    }

	private void Update() {
        if(amountOfSolved == wantedSolutions) {
            foreach(GameObject objectToActivate in objectsToActivate) {
                objectToActivate.GetComponent<InteractableMechanism>().Activate();
            }
            amountOfSolved = 0;
        }
	}

	// Deze methode controleert of een snap point de juiste formule heeft
	public bool CheckSnapPointFormula(GameObject snapPoint, string formula) {
        if(snapPointFormulas.ContainsKey(snapPoint)) {
            string correctFormula = snapPointFormulas[snapPoint];
            
            if(formula == correctFormula) {
                amountOfSolved++;
                Debug.Log("aantal opgelost: " + amountOfSolved);
                return true;
            } else {
                Debug.Log("aantal opgelost: " + amountOfSolved);
                return false;
            }
        }
        Debug.Log("aantal opgelost: " + amountOfSolved);
        return false;
    }

    public void AddSnapPointFormula(Dictionary<string, float[]> snapPointFormulasAndSolutions) {
        snapPointFormulas.Add(snapPointGear1, snapPointFormulasAndSolutions.ElementAt(0).Key);
        snapPointFormulas.Add(snapPointGear2, snapPointFormulasAndSolutions.ElementAt(1).Key);
        snapPointFormulas.Add(snapPointGear3, snapPointFormulasAndSolutions.ElementAt(2).Key);
        snapPointFormulas.Add(snapPointGear4, snapPointFormulasAndSolutions.ElementAt(3).Key);
        snapPointFormulas.Add(snapPointGear5, snapPointFormulasAndSolutions.ElementAt(4).Key);
    }

    public void SpawnGears(Dictionary<string, float[]> formulasAndSolutions, int[] solutions) {
        Debug.Log("Am I client? " + IsClient + "Am I host? " + IsHost + "Am I server? " + IsServer);

        AddSnapPointFormula(formulasAndSolutions);

        if(formulasAndSolutions == null) {
            Debug.LogError("Formulas and solutions not generated!");
            return;
        }

        // Loop om het gewenste aantal tandwielen te spawnen
        for(int i = 0; i < 5; i++) {
            // Pas de positie van het bigGearPrefab aan naar de positie van het gameobject
            Vector3 spawnPosition = spawnPoint.position;

            // Maak een instantie van het bigGearPrefab op de aangepaste positie
            GameObject newBigGear;

            newBigGear = Instantiate(bigGearPrefab, spawnPosition, Quaternion.identity);
            newBigGear.GetComponent<NetworkObject>().Spawn();
            newBigGear.transform.parent = gearParent.transform;

            Debug.Log(newBigGear.GetComponent<NetworkObject>().OwnerClientId);

            if(!newBigGear.GetComponent<NetworkObject>().IsSpawned) {
                Debug.Log("NetworkObject is not spawned or has been destroyed.");
            }

            setGearText(i, formulasAndSolutions);

            // Voeg newBigGear toe aan de lijst met gespawnede tandwielen
            spawnedGears.Add(newBigGear);
        }



        // Loop om het gewenste aantal tandwielen te spawnen
        for(int j = 0; j < 6; j++) {
            // Pas de positie van het smallGearPrefab aan naar de positie van het gameobject
            Vector3 spawnPosition = smallGearSpawnPoints[j].position;

            // Maak een instantie van het smallGearPrefab op de aangepaste positie
            GameObject newSmallGear = Instantiate(smallGearPrefab, spawnPosition, Quaternion.identity);
            newSmallGear.GetComponent<NetworkObject>().Spawn();

            // Pas de rotatie aan om de kleine gears verticaal te laten staan
            newSmallGear.transform.rotation = Quaternion.Euler(270f, 0f, 0f);

            // Haal de TextMeshProUGUI-component op van newSmallGear
            TextMeshProUGUI tmp = newSmallGear.GetComponentInChildren<TextMeshProUGUI>();

            // Controleer of er een TMP-component is gevonden
            if(tmp != null) {
                // Controleer of het huidige indexnummer binnen de geldige bereik ligt
                if(j < solutions.Length) {
                    // Haal de formule op uit de dictionary
                    int solutionOnGear = solutions[j];

                    // Pas de formule toe op de tekst van het TMP-object
                    tmp.text = solutionOnGear.ToString();
                } else {
                    Debug.LogWarning("No solution found for small gear at index: " + j);
                }
            } else {
                Debug.LogError("TextMeshPro component not found on big gear!");
            }
        }

        ArrayList jsonPayloadList = new ArrayList();
        jsonPayloadList.Add(formulasAndSolutions);
        jsonPayloadList.Add(solutions);
        string jsonPayload = JsonConvert.SerializeObject(jsonPayloadList);

        UpdateTMPTextServerRpc(jsonPayload);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateTMPTextServerRpc(string jsonPayload)
    {
        Debug.Log(jsonPayload);
        // Call the ClientRpc 
        UpdateTMPTextClientRpc(jsonPayload);
    }

    [ClientRpc]
    private void UpdateTMPTextClientRpc(string jsonPayload)
    {
        Debug.Log("client");
        Debug.Log(jsonPayload);

        ArrayList syncArrayList = JsonConvert.DeserializeObject <ArrayList>(jsonPayload);
        Dictionary<string, float[]> syncFormulasAndSolutions = JsonConvert.DeserializeObject<Dictionary<string, float[]>>(syncArrayList[0].ToString());
        //Debug.Log("1 " + syncArrayList[0].ToString());
        //Debug.Log("2 " + syncArrayList[1].ToString());
        Debug.Log(syncFormulasAndSolutions.Keys);
        int[] syncSolutions = JsonConvert.DeserializeObject<int[]>(syncArrayList[1].ToString());
        Debug.Log(syncSolutions);

        for(int i = 0; i < gearParent.transform.childCount; i++)
        {
            setGearText(i, syncFormulasAndSolutions);
        }

        //SpawnGears(syncFormulasAndSolutions, syncSolutions);
    }

    
    private void setGearText(int gearIndex, Dictionary<string, float[]> formulasAndSolutions)
    {
        GameObject bigGear = gearParent.transform.GetChild(gearIndex).gameObject;
        TextMeshProUGUI tmp = bigGear.GetComponentInChildren<TextMeshProUGUI>();

        // Controleer of er een TMP-component is gevonden
        if (tmp != null)
        {
                // Haal de formule op uit de dictionary
                KeyValuePair<string, float[]> formulaEntry = formulasAndSolutions.ElementAt(gearIndex);
                string formula = formulaEntry.Key;
                Debug.Log(formula);
                // Pas de formule toe op de tekst van het TMP-object
                tmp.text = formula;
        }
        else
        {
            Debug.LogError("TextMeshPro component not found on big gear!");
        }
    }
}