using System.Collections.Generic;
using UnityEngine;
using static GearPuzzleController;

public class FormulaGenerator : MonoBehaviour {
    [SerializeField] 
    private int minCoefficient = -10; // Minimumwaarde voor de coëfficiënten in de vergelijking
    
    [SerializeField] 
    private int maxCoefficient = 10; // Maximumwaarde voor de coëfficiënten in de vergelijking

    private int solutionCountPuzzleOne = 6; //Aantal oplossingen verdeeld over de formules
    private int[] solutions;

    private Dictionary<string, float[]> formulasAndSolutions; // Dictionary om formules en bijbehorende oplossingen bij te houden.


    public Dictionary<string, float[]> GetFormulasAndSolutions() {
        return formulasAndSolutions;
    }

    public int[] GetSolutions() {
        return solutions;
    }

    private PuzzleOptions chosenPuzzle;

    private void Start() {
        //GenerateFormulas();
    }

	private void Update() {
        
    }

	public void GenerateFormulas() {
        chosenPuzzle = this.gameObject.GetComponent<GearPuzzleController>().GetSelectedPuzzle();
        Debug.Log("Selected option: " + chosenPuzzle);

        formulasAndSolutions = new Dictionary<string, float[]>();
        solutions = new int[solutionCountPuzzleOne];

        if(chosenPuzzle == PuzzleOptions.Option1) {
            PuzzleOne();
        }
    }

    private void PuzzleOne() {
        for(int i = 0; i < solutionCountPuzzleOne; i++) {
            solutions[i] = GenerateSolution();
            //Ervoor zorgen dat er niks dubbel komt
        }

        string equationOne = BuildEquationFromSolutions(new float[] { solutions[0], solutions[1] });
        formulasAndSolutions.Add(equationOne, new float[] { solutions[0], solutions[1] });

        string equationTwo = BuildEquationFromSolutions(new float[] { solutions[0], solutions[2] });
        formulasAndSolutions.Add(equationTwo, new float[] { solutions[0], solutions[2] });

        string equationThree = BuildEquationFromSolutions(new float[] { solutions[0], solutions[3] });
        formulasAndSolutions.Add(equationThree, new float[] { solutions[0], solutions[3] });

        string equationFour = BuildEquationFromSolutions(new float[] { solutions[3], solutions[4] });
        formulasAndSolutions.Add(equationFour, new float[] { solutions[3], solutions[4] });

        string equationFive = BuildEquationFromSolutions(new float[] { solutions[4], solutions[5] });
        formulasAndSolutions.Add(equationFive, new float[] { solutions[4], solutions[5] });

        foreach(KeyValuePair<string, float[]> formulaEntry in formulasAndSolutions) {
            Debug.Log("Formula: " + formulaEntry.Key);
            Debug.Log("Solutions:");
            foreach(float solution in formulaEntry.Value) {
                Debug.Log(solution);
            }
        }

    }

    private int GenerateSolution() {
        int x;
        bool isDuplicate;

        do {
            x = Random.Range(minCoefficient, maxCoefficient + 1);
            isDuplicate = false;

            // Controleer of x al eerder is gegenereerd
            for(int i = 0; i < solutionCountPuzzleOne; i++) {
                if(solutions[i] == x) {
                    isDuplicate = true;
                    break;
                }
            }
        } while(isDuplicate);

        //Voor nu op deze manier, maar later gewoon checken of de formules niet anders zijn ipv dit want sommige solutions kunnen wel dubbel zijn en alsnog unieke formules maken

        return x;
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
                equation += "x²";
            } else if(a == -1) {
                equation += "-x²";
            } else {
                equation += $"{a}x²";
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