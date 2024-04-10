using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerActions : MonoBehaviour {
    private bool isNearActivationBall;
    private GameObject ballThatIsNear;

	private bool isNearCauldron;
	private GameObject cauldronCanvas;

	private GameObject potionControllerRef;

	public bool IsNearActivationBall {
		get => isNearActivationBall;
		set => isNearActivationBall = value;
	}
	public GameObject BallThatIsNear {
		get => ballThatIsNear;
		set => ballThatIsNear = value;
	}

	public bool IsNearCauldron {
		get => isNearCauldron;
		set => isNearCauldron = value;
	}

	public GameObject CauldronCanvas {
		get => cauldronCanvas;
		set => cauldronCanvas = value;
	}

	private string inputAnswerB;
	private string correctAnswerB;
	private string inputAnswerD;
	private string correctAnswerD;

	// Start is called before the first frame update
	void Start()
    {
		inputAnswerB = "aap";
		correctAnswerB = "noot";
		inputAnswerD = "mies";
		correctAnswerD = "hond";
        IsNearActivationBall = false;
		IsNearCauldron = false;
    }

    // Update is called once per frame
    void Update()
    {
		potionControllerRef = GameObject.FindWithTag("PotionController");
		if(Input.GetButtonDown("Activate")) {
			if(IsNearActivationBall) {
				BallThatIsNear.GetComponent<InteractableMechanism>().Activate();
			} else if(IsNearCauldron) {
				cauldronCanvas.SetActive(!cauldronCanvas.activeSelf);
				this.gameObject.GetComponent<PlayerMotor>().CanWalk = !this.gameObject.GetComponent<PlayerMotor>().CanWalk;
				if(Cursor.lockState == CursorLockMode.Locked)
                {
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
                } else
                {
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
                }

				if(!cauldronCanvas.activeSelf)
                {
					inputAnswerB = FindChildWithTag(cauldronCanvas, "InputAnswerYBTag").GetComponent<TMP_InputField>().text;
					correctAnswerB = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().answerYB.ToString();
					inputAnswerD = FindChildWithTag(cauldronCanvas, "InputAnswerYDTag").GetComponent<TMP_InputField>().text;
					correctAnswerD = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().answerYD.ToString();

					Debug.Log("Geg. Antw. B: " + inputAnswerB);
					Debug.Log("Cor. Antw. B: " + correctAnswerB);
					Debug.Log("Geg. Antw. D: " + inputAnswerD);
					Debug.Log("Cor. Antw. D: " + correctAnswerD);

					if (inputAnswerB == correctAnswerB && inputAnswerD == correctAnswerD)
                    {
						Debug.Log("Hoeraaa!");
                    }

				}
			}
		}
    }

	GameObject FindChildWithTag(GameObject parent, string tag)
	{
		GameObject child = null;

		foreach (Transform transform in parent.transform)
		{
			if (transform.CompareTag(tag))
			{
				child = transform.gameObject;
				break;
			}
		}

		return child;
	}

}
