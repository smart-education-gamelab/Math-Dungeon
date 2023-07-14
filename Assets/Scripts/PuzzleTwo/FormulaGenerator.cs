using System.Collections.Generic;
using UnityEngine;

public class FormulaGenerator : MonoBehaviour {
    public int minCoefficient = -10; // Minimumwaarde voor de coëfficiënten in de vergelijking
    public int maxCoefficient = 10; // Maximumwaarde voor de coëfficiënten in de vergelijking
    public int formulaCount = 6; // Aantal formules dat gegenereerd moet worden

    private Dictionary<string, float[]> formulasAndSolutions; // Dictionary om formules en bijbehorende oplossingen bij te houden

    public Dictionary<string, float[]> GetFormulasAndSolutions() {
        return formulasAndSolutions;
    }

    private void Start() {
        GenerateFormulas();
    }

    public void GenerateFormulas() {
        formulasAndSolutions = new Dictionary<string, float[]>();

        float[] previousSolutions = null; // Vorige oplossingen om te vergelijken

        for(int i = 0; i < formulaCount; i++) {
            float[] solutions;

            // Genereren van de oplossingen
            if(i == 0 || i == 1 || i == 2) {
                solutions = GenerateSolutions();
            } else if(i == 3) {
                solutions = GenerateSolutions(previousSolutions, i);
            } else if(i == 4 || i == 5) {
                solutions = GenerateSolutions(previousSolutions, i);
            } else {
                solutions = GenerateSolutions();
            }

            string equation = BuildEquationFromSolutions(solutions);

            formulasAndSolutions.Add(equation, solutions);

            Debug.Log($"Formula {i + 1}:" + " " + "Equation: " + equation);

            if(solutions.Length == 0) {
                Debug.Log("No solutions");
            } else {
                Debug.Log("Solutions:");
                foreach(float solution in solutions) {
                    Debug.Log(solution);
                }
            }

            // Bewaar de oplossingen voor vergelijking met volgende formules
            previousSolutions = solutions;
        }
    }

    private float[] GenerateSolutions() {
        float x1 = Random.Range(minCoefficient, maxCoefficient + 1);
        float x2 = Random.Range(minCoefficient, maxCoefficient + 1);
        return new float[] { x1, x2 };
    }

    private float[] GenerateSolutions(float[] previousSolutions, int solutionIndex) {
        float x1;
        float x2;

        if(solutionIndex == 2) { //TODO: Nog ont-hardcoden
            x1 = previousSolutions[0]; // Neem de eerste oplossing van de vorige formule
            x2 = Random.Range(minCoefficient, maxCoefficient + 1);

            // Zorg ervoor dat x2 niet hetzelfde is als de vorige oplossing
            //TODO MOET DIT WEL?
            while(x2 == x1) {
                x2 = Random.Range(minCoefficient, maxCoefficient + 1);
            }
        } else {
            if(previousSolutions == null) {
                x1 = Random.Range(minCoefficient, maxCoefficient + 1);
                x2 = Random.Range(minCoefficient, maxCoefficient + 1);
            } else {
                x1 = previousSolutions[1]; // Neem de tweede oplossing van de vorige formule
                x2 = Random.Range(minCoefficient, maxCoefficient + 1);

                // Zorg ervoor dat x2 niet hetzelfde is als de vorige oplossing
                //TODO MOET DIT WEL?
                while(x2 == x1) {
                    x2 = Random.Range(minCoefficient, maxCoefficient + 1);
                }
            }
        }

        return new float[] { x1, x2 };
    }

    private string BuildEquationFromSolutions(float[] solutions) {
        float x1 = solutions[0];
        float x2 = solutions[1];

        int a = 1;
        int b = -((int) x1 + (int) x2);
        int c = (int) (x1 * x2);

        string equation = BuildEquation(a, b, c);
        return equation;
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
