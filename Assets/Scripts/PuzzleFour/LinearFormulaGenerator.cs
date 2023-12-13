using UnityEngine;

public class LinearFormulaGenerator : MonoBehaviour {
    [SerializeField]
    private int minValue = -10;

    [SerializeField]
    private int maxValue = 10;

    Vector2 aAndBValue = new Vector2(99, 99); //Later uit elkaar trekken?
    string linearFormula = "placeholder";

    Vector2 pointA = new Vector2(99, 99);
    Vector2 pointB = new Vector2(99, 99);
    Vector2 pointC = new Vector2(99, 99);
    Vector2 pointD = new Vector2(99, 99);

    // Start is called before the first frame update
    void Start() {
        aAndBValue = GenerateAAndB();
        Debug.Log("Debug 1: A: " + aAndBValue.x.ToString() + "B: " + aAndBValue.y.ToString());
        linearFormula = BuildFormulaFromAAndB((int) aAndBValue.x, (int) aAndBValue.y);
        Debug.Log("Debug 2: Formula: " + linearFormula);
        pointA.x = XPointsOnGraph()[0];
        Debug.Log("Debug 3: Point A: (" + pointA.x.ToString() + ", " + pointA.y.ToString() + ").");
        pointB.x = XPointsOnGraph()[1];
        Debug.Log("Debug 4: Point B: (" + pointB.x.ToString() + ", " + pointB.y.ToString() + ").");
        pointC.x = XPointsOnGraph()[2];
        Debug.Log("Debug 5: Point C: (" + pointC.x.ToString() + ", " + pointC.y.ToString() + ").");
        pointD.x = XPointsOnGraph()[3];
        Debug.Log("Debug 6: Point D: (" + pointD.x.ToString() + ", " + pointD.y.ToString() + ").");
    }

    // Update is called once per frame
    void Update() {

    }

    private Vector2 GenerateAAndB() {
        int valueA = Random.Range(minValue, maxValue);
        int valueB = Random.Range(minValue, maxValue);

        Vector2 returnValue = new Vector2(valueA, valueB);

        return returnValue;
    }

    private string BuildFormulaFromAAndB(int valueA, int valueB) {
        string returnValue = valueA.ToString() + "x";
        if(valueB < 0) {
            returnValue = returnValue + " - " + (valueB * -1).ToString();
        } else {
            returnValue = returnValue + " + " + valueB.ToString();
        }

        return returnValue;
    }

    private int[] XPointsOnGraph() {
		int[] xPoints = new int[4] { 99, 99, 99, 99 };

        foreach(int i in xPoints) {
            xPoints[i] = Random.Range(minValue, maxValue);
            Debug.Log("Nr. " + i.ToString() + " value: " + xPoints[i].ToString());

            foreach(int j in xPoints) {
                if(i != j) {
                    while(xPoints[i] == xPoints[j]) {
                        xPoints[i] = Random.Range(minValue, maxValue);
                    }
                }
            }
            Debug.Log("Na dubbel check. Nr. " + i.ToString() + " value: " + xPoints[i].ToString());
        }

        return xPoints;
    }

    //Calc y from x
}