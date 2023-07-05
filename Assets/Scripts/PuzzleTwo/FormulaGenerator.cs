using UnityEngine;

public class FormulaGenerator : MonoBehaviour {
    public int minCoefficient = -10; // Minimumwaarde voor de coëfficiënten in de vergelijking
    public int maxCoefficient = 10; // Maximumwaarde voor de coëfficiënten in de vergelijking
    public int formulaCount = 3; // Aantal formules dat gegenereerd moet worden

    private void Start() {
        GenerateFormulas();
    }

    public void GenerateFormulas() {
        for(int i = 0; i < formulaCount; i++) {
            int a, b, c;
            bool hasIntegerSolutions;

            // Genereren van formules met gehele getallen als oplossingen
            do {
                //a = Random.Range(minCoefficient, maxCoefficient + 1);
                a = 1;
                b = Random.Range(minCoefficient, maxCoefficient + 1);
                c = Random.Range(minCoefficient, maxCoefficient + 1);

                hasIntegerSolutions = CheckIntegerSolutions(a, b, c);
            } while(!hasIntegerSolutions);

            string equation = BuildEquation(a, b, c);

            Debug.Log($"Formula {i + 1}:");
            Debug.Log("Equation: " + equation);
            float[] solutions = SolveEquation(a, b, c);

            if(solutions.Length == 0) {
                Debug.Log("No solutions");
            } else {
                Debug.Log("Solutions:");
                foreach(float solution in solutions) {
                    Debug.Log(solution);
                }
            }
        }
    }

    private bool CheckIntegerSolutions(int a, int b, int c) {
        // Controleert of de vergelijking gehele getallen heeft als oplossingen
        int discriminant = b * b - 4 * a * c;

        // Als de discriminant geen perfect vierkant is, heeft de vergelijking geen gehele oplossingen
        if(!IsPerfectSquare(discriminant)) {
            return false;
        }

        // Controleert of de oplossingen gehele getallen zijn
        float x1 = (-b + Mathf.Sqrt(discriminant)) / (2f * a);
        float x2 = (-b - Mathf.Sqrt(discriminant)) / (2f * a);

        return Mathf.Round(x1) == x1 && Mathf.Round(x2) == x2;
    }

    private bool IsPerfectSquare(int number) {
        int sqrt = (int) Mathf.Sqrt(number);
        return sqrt * sqrt == number;
    }

    private float[] SolveEquation(int a, int b, int c) {
        float discriminant = b * b - 4 * a * c;

        // Als de discriminant negatief is, heeft de vergelijking geen snijpunten met de x-as
        if(discriminant < 0) {
            return new float[0];
        }
        // Als de discriminant gelijk aan nul is, heeft de vergelijking één snijpunt met de x-as
        else if(discriminant == 0) {
            float x = -b / (2f * a);
            return new float[] { x };
        }
        // Als de discriminant positief is, heeft de vergelijking twee snijpunten met de x-as
        else {
            float x1 = (-b + Mathf.Sqrt(discriminant)) / (2f * a);
            float x2 = (-b - Mathf.Sqrt(discriminant)) / (2f * a);
            return new float[] { x1, x2 };
        }
    }

    private string BuildEquation(int a, int b, int c) {
        string equation = "";

        // Opbouwen van de vergelijking
        if(a != 0) {
            if(a == 1) {
                equation += "x^2";
            } else if(a == -1) {
                equation += "-x^2";
            } else {
                equation += $"{a}x^2";
            }
        }

        if(b != 0) {
            if(b > 0) {
                if(equation.Length > 0) {
                    equation += " + ";
                }
            } else {
                equation += " - ";
                b = -b;
            }

            if(b == 1) {
                equation += "x";
            } else {
                equation += $"{b}x";
            }
        }

        if(c != 0) {
            if(c > 0) {
                if(equation.Length > 0) {
                    equation += " + ";
                }
            } else {
                equation += " - ";
                c = -c;
            }

            equation += c;
        }

        return equation;
    }
}