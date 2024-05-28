using TMPro;
using UnityEngine;
using Unity.Netcode;

[System.Serializable]
public struct PointData : INetworkSerializable
{
    public int x;
    public int y;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref x);
        serializer.SerializeValue(ref y);
    }
}

public class LinearFormulaGeneratorSync : NetworkBehaviour
{
    [SerializeField]
    private int minValue = -10;
    [SerializeField]
    private int maxValue = 10;

    private NetworkVariable<PointData> pointA = new NetworkVariable<PointData>();
    private NetworkVariable<PointData> pointB = new NetworkVariable<PointData>();
    private NetworkVariable<PointData> pointC = new NetworkVariable<PointData>();
    private NetworkVariable<PointData> pointD = new NetworkVariable<PointData>();
    private NetworkVariable<PointData> pointE = new NetworkVariable<PointData>();
    private NetworkVariable<PointData> pointF = new NetworkVariable<PointData>();

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

    public int AnswerXC => pointC.Value.x;
    public int AnswerYC => pointC.Value.y;
    public int AnswerXF => pointF.Value.x;
    public int AnswerYF => pointF.Value.y;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            CalculatePoints();
        }
        base.OnNetworkSpawn();
        UpdateUI();
    }

    private void OnEnable()
    {
        pointA.OnValueChanged += OnPointsChanged;
        pointB.OnValueChanged += OnPointsChanged;
        pointC.OnValueChanged += OnPointsChanged;
        pointD.OnValueChanged += OnPointsChanged;
        pointE.OnValueChanged += OnPointsChanged;
        pointF.OnValueChanged += OnPointsChanged;
    }

    private void OnDisable()
    {
        pointA.OnValueChanged -= OnPointsChanged;
        pointB.OnValueChanged -= OnPointsChanged;
        pointC.OnValueChanged -= OnPointsChanged;
        pointD.OnValueChanged -= OnPointsChanged;
        pointE.OnValueChanged -= OnPointsChanged;
        pointF.OnValueChanged -= OnPointsChanged;
    }

    private void OnPointsChanged(PointData oldValue, PointData newValue)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        textXA.text = pointA.Value.x.ToString();
        textYA.text = pointA.Value.y.ToString();
        textXB.text = pointB.Value.x.ToString();
        textYB.text = pointB.Value.y.ToString();
        textXC.text = pointC.Value.x.ToString();
        textXD.text = pointD.Value.x.ToString();
        textYD.text = pointD.Value.y.ToString();
        textXE.text = pointE.Value.x.ToString();
        textYE.text = pointE.Value.y.ToString();
        textXF.text = pointF.Value.x.ToString();
        textXF.text = pointF.Value.y.ToString();
    }

    public void CalculatePoints()
    {
        int[] xCoords = XPointsOnGraph();

        // First formula
        Vector2 aAndBValue = GenerateAAndB();
        pointA.Value = new PointData { x = xCoords[0], y = CalculateY((int)aAndBValue.x, (int)aAndBValue.y, xCoords[0]) };
        pointB.Value = new PointData { x = xCoords[1], y = CalculateY((int)aAndBValue.x, (int)aAndBValue.y, xCoords[1]) };
        pointC.Value = new PointData { x = xCoords[2], y = CalculateY((int)aAndBValue.x, (int)aAndBValue.y, xCoords[2]) };

        // Second formula
        Vector2 aAndBValueTwo = GenerateAAndB();
        pointD.Value = new PointData { x = xCoords[3], y = CalculateY((int)aAndBValueTwo.x, (int)aAndBValueTwo.y, xCoords[3]) };
        pointE.Value = new PointData { x = xCoords[4], y = CalculateY((int)aAndBValueTwo.x, (int)aAndBValueTwo.y, xCoords[4]) };
        pointF.Value = new PointData { x = xCoords[5], y = CalculateY((int)aAndBValueTwo.x, (int)aAndBValueTwo.y, xCoords[5]) };
    }

    private Vector2 GenerateAAndB()
    {
        int valueA = Random.Range(minValue, maxValue);
        int valueB = Random.Range(minValue, maxValue);
        return new Vector2(valueA, valueB);
    }

    private int[] XPointsOnGraph()
    {
        int[] xPoints = new int[6];

        for (int i = 0; i < xPoints.Length; i++)
        {
            xPoints[i] = Random.Range(minValue, maxValue);
            while (xPoints[i] == 0)
            {
                xPoints[i] = Random.Range(minValue, maxValue);
            }

            for (int j = 0; j < i; j++)
            {
                if (xPoints[i] == xPoints[j])
                {
                    i--; // regenerate this point
                    break;
                }
            }
        }

        return xPoints;
    }

    private int CalculateY(int aValue, int bValue, int xValue)
    {
        return (xValue * aValue) + bValue;
    }
}