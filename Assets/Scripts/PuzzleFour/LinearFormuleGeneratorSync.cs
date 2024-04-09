using TMPro;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using Newtonsoft.Json;
using System.Collections;

public class LinearFormuleGeneratorSync : NetworkBehaviour
{
    [SerializeField]
    private int minValue = -10;

    [SerializeField]
    private int maxValue = 10;

    Vector2 aAndBValue = new Vector2(99, 99); //Later uit elkaar trekken?
    string linearFormula = "placeholder";

    Vector2 pointA = new Vector2(99, 99);
    Vector2 pointB = new Vector2(99, 99);

    Vector2 aAndBValueTwo = new Vector2(99, 99);
    string linearFormulaTwo = "placeholder two";

    Vector2 pointC = new Vector2(99, 99);
    Vector2 pointD = new Vector2(99, 99);

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
        Debug.Log(IsServer + "Start");
        if (IsServer)
        {
            Debug.Log("ik ben server");
            xCoords = XPointsOnGraph();

            aAndBValue = GenerateAAndB();
            Debug.Log("Debug 1: A: " + aAndBValue.x.ToString() + ", B: " + aAndBValue.y.ToString());
            linearFormula = BuildFormulaFromAAndB((int)aAndBValue.x, (int)aAndBValue.y);
            Debug.Log("Debug 2: Formula: " + linearFormula);
            pointA.x = xCoords[0];
            pointA.y = CalculateY((int)aAndBValue.x, (int)aAndBValue.y, (int)pointA.x);
            Debug.Log("Debug 3: Point A: (" + pointA.x.ToString() + ", " + pointA.y.ToString() + ").");
            pointB.x = xCoords[1];
            pointB.y = CalculateY((int)aAndBValue.x, (int)aAndBValue.y, (int)pointB.x);
            Debug.Log("Debug 4: Point B: (" + pointB.x.ToString() + ", " + pointB.y.ToString() + ").");
            textXA.text = pointA.x.ToString();
            textXB.text = pointB.x.ToString();
            textYA.text = pointA.y.ToString();

            aAndBValueTwo = GenerateAAndB();
            Debug.Log("Debug 5: A: " + aAndBValueTwo.x.ToString() + ", B: " + aAndBValueTwo.y.ToString());
            linearFormulaTwo = BuildFormulaFromAAndB((int)aAndBValueTwo.x, (int)aAndBValueTwo.y);
            Debug.Log("Debug 6: Formula 2: " + linearFormulaTwo);
            pointC.x = xCoords[2];
            pointC.y = CalculateY((int)aAndBValueTwo.x, (int)aAndBValueTwo.y, (int)pointC.x);
            Debug.Log("Debug 7: Point C: (" + pointC.x.ToString() + ", " + pointC.y.ToString() + ").");
            pointD.x = xCoords[3];
            pointD.y = CalculateY((int)aAndBValueTwo.x, (int)aAndBValueTwo.y, (int)pointD.x);
            Debug.Log("Debug 8: Point D: (" + pointD.x.ToString() + ", " + pointD.y.ToString() + ").");
            textXC.text = pointC.x.ToString();
            textXD.text = pointD.x.ToString();
            textYC.text = pointC.y.ToString();

            ArrayList jsonPayloadList = new ArrayList();
            jsonPayloadList.Add(pointA);
            jsonPayloadList.Add(pointB);
            jsonPayloadList.Add(pointC);
            jsonPayloadList.Add(pointD);
            string jsonPayload = JsonConvert.SerializeObject(jsonPayloadList);

            UpdateTMPTextServerRpc(jsonPayload);
        }
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
        ArrayList syncArrayList = JsonConvert.DeserializeObject<ArrayList>(jsonPayload);
        
        Debug.Log(syncArrayList);
    }

        private Vector2 GenerateAAndB()
    {
        int valueA = Random.Range(minValue, maxValue);
        int valueB = Random.Range(minValue, maxValue);

        Vector2 returnValue = new Vector2(valueA, valueB);

        return returnValue;
    }

    private string BuildFormulaFromAAndB(int valueA, int valueB)
    {
        string returnValue = valueA.ToString() + "x";
        if (valueB < 0)
        {
            returnValue = returnValue + " - " + (valueB * -1).ToString();
        }
        else
        {
            returnValue = returnValue + " + " + valueB.ToString();
        }

        return returnValue;
    }

    private int[] XPointsOnGraph()
    {
        int[] xPoints = new int[4] { 99, 99, 99, 99 };

        for (int i = 0; i < xPoints.Length; i++)
        {
            xPoints[i] = Random.Range(minValue, maxValue);
            //Debug.Log("Nr. " + i.ToString() + " value: " + xPoints[i].ToString());

            for (int j = 0; j < xPoints.Length; j++)
            {
                if (i != j)
                {
                    while (xPoints[i] == xPoints[j])
                    {
                        xPoints[i] = Random.Range(minValue, maxValue);
                    }
                }
            }
            //Debug.Log("Na dubbel check. Nr. " + i.ToString() + " value: " + xPoints[i].ToString());
        }

        return xPoints;
    }

    private int CalculateY(int aValue, int bValue, int xValue)
    {
        return (xValue * aValue) + bValue;
    }

    //Calc y from x
}