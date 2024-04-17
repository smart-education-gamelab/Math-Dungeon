using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using Unity.Netcode;
using System.Collections;

public class LinearFormulaGeneratorSync : NetworkBehaviour
{
    [SerializeField]
    private int minValue = -10;

    [SerializeField]
    private int maxValue = 10;

    Vector2 aAndBValue = new Vector2(99, 99); //Later uit elkaar trekken?
    string linearFormula = "placeholder";

    Vector2 pointA = new Vector2(99, 99);
    Vector2 pointB = new Vector2(99, 99);
    Vector2 pointC = new Vector2(99, 99);

    Vector2 aAndBValueTwo = new Vector2(99, 99);
    string linearFormulaTwo = "placeholder two";

    Vector2 pointD = new Vector2(99, 99);
    Vector2 pointE = new Vector2(99, 99);
    Vector2 pointF = new Vector2(99, 99);

    int[] xCoords = new int[6];

    [SerializeField]
    private TextMeshProUGUI textXA;
    [SerializeField]
    private TextMeshProUGUI textXB;
    [SerializeField]
    private TextMeshProUGUI textYA;
    [SerializeField]
    private TextMeshProUGUI textYB;
    [SerializeField]
    private TextMeshProUGUI textXC;


    [SerializeField]
    private TextMeshProUGUI textXD;
    [SerializeField]
    private TextMeshProUGUI textXE;
    [SerializeField]
    private TextMeshProUGUI textYD;
    [SerializeField]
    private TextMeshProUGUI textYE;
    [SerializeField]
    private TextMeshProUGUI textXF;

    public int AnswerXC
    {
        get => (int)pointC.x;
    }

    public int AnswerYC
    {
        get => (int)pointC.y;
    }

    public int AnswerXF
    {
        get => (int)pointF.x;
    }

    public int AnswerYF
    {
        get => (int)pointF.y;
    }


    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        //Debug.Log("Am I client? " + IsClient + "Am I host? " + IsHost + "Am I server? " + IsServer);
        CalculatePoints();
        base.OnNetworkSpawn();
    }

    public void CalculatePoints()
    {
        //Debug.Log("2 Am I client? " + IsClient + "Am I host? " + IsHost + "Am I server? " + IsServer);
        if (IsServer || IsOwner)
        {
            xCoords = XPointsOnGraph();

            //Eerste formule

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
            pointC.x = xCoords[2];
            pointC.y = CalculateY((int)aAndBValue.x, (int)aAndBValue.y, (int)pointC.x);
            Debug.Log("Debug 5: Point C: (" + pointC.x.ToString() + ", " + pointC.y.ToString() + ").");

            //Tweede formule

            aAndBValueTwo = GenerateAAndB();
            Debug.Log("Debug 6: A: " + aAndBValueTwo.x.ToString() + ", B: " + aAndBValueTwo.y.ToString());
            linearFormulaTwo = BuildFormulaFromAAndB((int)aAndBValueTwo.x, (int)aAndBValueTwo.y);
            Debug.Log("Debug 7: Formula 2: " + linearFormulaTwo);
            pointD.x = xCoords[3];
            pointD.y = CalculateY((int)aAndBValueTwo.x, (int)aAndBValueTwo.y, (int)pointD.x);
            Debug.Log("Debug 8: Point D: (" + pointD.x.ToString() + ", " + pointD.y.ToString() + ").");
            pointE.x = xCoords[4];
            pointE.y = CalculateY((int)aAndBValueTwo.x, (int)aAndBValueTwo.y, (int)pointE.x);
            Debug.Log("Debug 9: Point E: (" + pointE.x.ToString() + ", " + pointE.y.ToString() + ").");
            pointF.x = xCoords[5];
            pointF.y = CalculateY((int)aAndBValueTwo.x, (int)aAndBValueTwo.y, (int)pointF.x);
            Debug.Log("Debug 10: Point F: (" + pointF.x.ToString() + ", " + pointF.y.ToString() + ").");



            ArrayList jsonPayloadList = new ArrayList();
            jsonPayloadList.Add(pointA.x.ToString());
            jsonPayloadList.Add(pointA.y.ToString());
            jsonPayloadList.Add(pointB.x.ToString());
            jsonPayloadList.Add(pointB.y.ToString());
            jsonPayloadList.Add(pointC.x.ToString());
            jsonPayloadList.Add(pointC.y.ToString());
            jsonPayloadList.Add(pointD.x.ToString());
            jsonPayloadList.Add(pointD.y.ToString());
            jsonPayloadList.Add(pointE.x.ToString());
            jsonPayloadList.Add(pointE.y.ToString());
            jsonPayloadList.Add(pointF.x.ToString());
            jsonPayloadList.Add(pointF.y.ToString());
            string jsonPayload = JsonConvert.SerializeObject(jsonPayloadList);

            UpdatePointsTextServerRpc(jsonPayload);
        } else
        {
            return;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePointsTextServerRpc(string jsonPayload)
    {
        Debug.Log(jsonPayload + " SERVER RPC LINFORGENSYN");
        UpdatePointsTextClientRpc(jsonPayload);
    }

    [ClientRpc]
    private void UpdatePointsTextClientRpc(string jsonPayload)
    {
        //Debug.Log("client");
        Debug.Log(jsonPayload + "  CLIENT RPC LINFORGENSYN");

        ArrayList syncArrayList = JsonConvert.DeserializeObject<ArrayList>(jsonPayload);
        string pointAX = JsonConvert.DeserializeObject<string>(syncArrayList[0].ToString());
        string pointAY = JsonConvert.DeserializeObject<string>(syncArrayList[1].ToString());
        string pointBX = JsonConvert.DeserializeObject<string>(syncArrayList[2].ToString());
        string pointBY = JsonConvert.DeserializeObject<string>(syncArrayList[3].ToString());
        string pointCX = JsonConvert.DeserializeObject<string>(syncArrayList[4].ToString());
        string pointCY = JsonConvert.DeserializeObject<string>(syncArrayList[5].ToString());
        string pointDX = JsonConvert.DeserializeObject<string>(syncArrayList[6].ToString());
        string pointDY = JsonConvert.DeserializeObject<string>(syncArrayList[7].ToString());
        string pointEX = JsonConvert.DeserializeObject<string>(syncArrayList[8].ToString());
        string pointEY = JsonConvert.DeserializeObject<string>(syncArrayList[9].ToString());
        string pointFX = JsonConvert.DeserializeObject<string>(syncArrayList[10].ToString());
        string pointFY = JsonConvert.DeserializeObject<string>(syncArrayList[11].ToString());
        Debug.Log("A: (" + pointAX + ", " + pointAY + ")");
        Debug.Log("B: (" + pointBX + ", " + pointBY + ")");
        Debug.Log("C: (" + pointCX + ", " + pointCY + ")");
        Debug.Log("D: (" + pointDX + ", " + pointDY + ")");
        Debug.Log("E: (" + pointEX + ", " + pointEY + ")");
        Debug.Log("F: (" + pointFX + ", " + pointFY + ")");

        textXA.text = pointAX;
        textXB.text = pointBX;
        textYA.text = pointAY;
        textYB.text = pointBY;
        
        textXC.text = pointCX;


        textXD.text = pointDX;
        textXE.text = pointEX;
        textYD.text = pointDY;
        textYE.text = pointEY;

        textXF.text = pointFX;
    }

    private Vector2 GenerateAAndB()
    {
        int valueA = 0;
        valueA = Random.Range(minValue, maxValue);

        int valueB = 0;
        valueB = Random.Range(minValue, maxValue);

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
        int[] xPoints = new int[6] { 99, 99, 99, 99, 99, 99 };

        for (int i = 0; i <= xPoints.Length; i++)
        {
            while (xPoints[i] == 0)
            {
                xPoints[i] = Random.Range(minValue, maxValue);
            }

            for (int j = 0; j <= xPoints.Length; j++)
            {
                if (i != j)
                {
                    while (xPoints[i] == xPoints[j])
                    {
                        xPoints[i] = Random.Range(minValue, maxValue);
                    }
                }
            }
        }

        return xPoints;
    }

    private int CalculateY(int aValue, int bValue, int xValue)
    {
        return (xValue * aValue) + bValue;
    }

    //Calc y from x
}