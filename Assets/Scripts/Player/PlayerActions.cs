using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerActions : MonoBehaviour {
    private bool isNearActivationBall;
    private GameObject ballThatIsNear;

	private bool isNearCauldron;
	private GameObject cauldronCanvas;

	[SerializeField]
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

	// Start is called before the first frame update
	void Start()
    {
        IsNearActivationBall = false;
		IsNearCauldron = false;
    }

    // Update is called once per frame
    void Update()
    {
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

				if(cauldronCanvas.activeSelf == true)
                {
					string inputAnswerB = FindChildWithTag(cauldronCanvas, "InputAnswerYBTag").GetComponent<TMP_InputField>().text;
					string correctAnswerB = potionControllerRef.GetComponent<LinearFormulaGenerator>().answerYB.ToString();
					string inputAnswerD = FindChildWithTag(cauldronCanvas, "InputAnswerYDTag").GetComponent<TMP_InputField>().text;
					string correctAnswerD = potionControllerRef.GetComponent<LinearFormulaGenerator>().answerYD.ToString();

					if(inputAnswerB == correctAnswerB && inputAnswerD == correctAnswerD)
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
