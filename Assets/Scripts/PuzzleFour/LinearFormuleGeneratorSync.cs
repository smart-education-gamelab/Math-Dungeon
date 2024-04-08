using TMPro;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Newtonsoft.Json;

public class LinearFormuleGeneratorSync : NetworkBehaviour
{
    [SerializeField]
    private int minValue = -10;

    [SerializeField]
    private int maxValue = 10;

    Vector2 aAndBValue = new Vector2(99, 99);
    string linearFormula = "placeholder";

    Vector2 pointA = new Vector2(99, 99);
    Vector2 pointB = new Vector2(99, 99);

    Vector2 aAndBValueTwo = new Vector2(99, 99);
    string linearFormulaTwo = "placeholder two";

    Vector2 pointC = new Vector2(99, 99);
    Vector2 pointD = new Vector2(99, 99);

    private List<object> payloadList = new List<object>();

    int[] xCoords = new int[4];

    [SerializeField]
    private TextMeshProUGUI textXA;
    [SerializeField]
    private TextMeshProUGUI textXB;
    [SerializeField]
    private TextMeshProUGUI textYA;

    [SerializeField]
    private TextMeshProUGUI textXC;
    [SerializeField]
    private TextMeshProUGUI textXD;
    [SerializeField]
    private TextMeshProUGUI textYC;

    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            GenerateValuesAndSync();
        }
    }

    void GenerateValuesAndSync()
    {
        xCoords = XPointsOnGraph();

        aAndBValue = GenerateAAndB();
        linearFormula = BuildFormulaFromAAndB((int)aAndBValue.x, (int)aAndBValue.y);
        pointA.x = xCoords[0];
        pointA.y = CalculateY((int)aAndBValue.x, (int)aAndBValue.y, (int)pointA.x);
        pointB.x = xCoords[1];
        pointB.y = CalculateY((int)aAndBValue.x, (int)aAndBValue.y, (int)pointB.x);

        aAndBValueTwo = GenerateAAndB();
        linearFormulaTwo = BuildFormulaFromAAndB((int)aAndBValueTwo.x, (int)aAndBValueTwo.y);
        pointC.x = xCoords[2];
        pointC.y = CalculateY((int)aAndBValueTwo.x, (int)aAndBValueTwo.y, (int)pointC.x);
        pointD.x = xCoords[3];
        pointD.y = CalculateY((int)aAndBValueTwo.x, (int)aAndBValueTwo.y, (int)pointD.x);

        // Serialize data
        List<object> payloadList = new List<object>();
        payloadList.Add(new Dictionary<string, object>
        {
            { "Formulas", new float[][] { new float[] { aAndBValue.x, aAndBValue.y }, new float[] { aAndBValueTwo.x, aAndBValueTwo.y } } },
            { "Solutions", new int[] { (int)pointA.y, (int)pointB.y, (int)pointC.y, (int)pointD.y } }
        });

        string jsonPayload = JsonConvert.SerializeObject(payloadList);

        RpcSyncValuesClientRpc(jsonPayload);
    }

    [ClientRpc]
    void RpcSyncValuesClientRpc(string jsonPayload)
    {
        List<object> syncData = JsonConvert.DeserializeObject<List<object>>(jsonPayload);
        Dictionary<string, object> dataDict = (Dictionary<string, object>)syncData[0];
        float[][] formulasArray = (float[][])dataDict["Formulas"];
        int[] solutions = (int[])dataDict["Solutions"];

        float[] formula1 = formulasArray[0];
        float[] formula2 = formulasArray[1];

        // Update UI or do something with the synchronized data
        // For example:
        textXA.text = formula1[0].ToString();
        textXB.text = formula2[0].ToString();
        textYA.text = solutions[0].ToString();
        textXC.text = formula1[1].ToString();
        textXD.text = formula2[1].ToString();
        textYC.text = solutions[2].ToString();
    }

    private Vector2 GenerateAAndB()
    {
        int valueA = Random.Range(minValue, maxValue);
        int valueB = Random.Range(minValue, maxValue);

        return new Vector2(valueA, valueB);
    }

    private string BuildFormulaFromAAndB(int valueA, int valueB)
    {
        string formula = valueA + "x";
        if (valueB < 0)
            formula += " - " + Mathf.Abs(valueB);
        else
            formula += " + " + valueB;

        return formula;
    }

    private int[] XPointsOnGraph()
    {
        List<int> xPoints = new List<int>();

        while (xPoints.Count < 4)
        {
            int rand = Random.Range(minValue, maxValue);
            if (!xPoints.Contains(rand))
                xPoints.Add(rand);
        }

        return xPoints.ToArray();
    }

    private int CalculateY(int aValue, int bValue, int xValue)
    {
        return (xValue * aValue) + bValue;
    }
}