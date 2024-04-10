using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerActions : MonoBehaviour {
    private bool isNearActivationBall;
    private GameObject ballThatIsNear;

	private bool isNearCauldron;
	private GameObject cauldronCanvasA;
	private GameObject cauldronCanvasB;

	private GameObject potionControllerRef;

	[SerializeField]
	private GameObject movingWallA;

	[SerializeField]
	private GameObject movingWallB;

	private string canvasName;

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
	public string CanvasName{
		get => canvasName;
		set => canvasName = value;
	}

	//Kamer A
	public GameObject CauldronCanvasA {
		get => cauldronCanvasA;
		set => cauldronCanvasA = value;
	}

	//Kamer B
	public GameObject CauldronCanvasB {
		get => cauldronCanvasB;
		set => cauldronCanvasB = value;
    }

	//Kamer A
	private string inputAnswerBRoomA;
	private string correctAnswerBRoomA;
	private string inputAnswerDRoomA;
	private string correctAnswerDRoomA;

	//Kamer B
	private string inputAnswerBRoomB;
	private string correctAnswerBRoomB;
	private string inputAnswerDRoomB;
	private string correctAnswerDRoomB;

	private bool RoomACorrect;
	private bool RoomBCorrect;

	// Start is called before the first frame update
	void Start()
    {
		//Kamer A
		inputAnswerBRoomA = "aap";
		correctAnswerBRoomA = "noot";
		inputAnswerDRoomA = "mies";
		correctAnswerDRoomA = "hond";

		//Kamer B
		inputAnswerBRoomB = "paa";
		correctAnswerBRoomB = "toon";
		inputAnswerDRoomB = "seim";
		correctAnswerDRoomB = "dnoh";


		IsNearActivationBall = false;
		IsNearCauldron = false;

		movingWallA = GameObject.Find("DoorA");
		movingWallB = GameObject.Find("DoorB");


		RoomACorrect = false;
		RoomBCorrect = false;
    }

    // Update is called once per frame
    void Update()
    {
		potionControllerRef = GameObject.FindWithTag("PotionController");
		if(Input.GetButtonDown("Activate")) {
			if(IsNearActivationBall) {
				BallThatIsNear.GetComponent<InteractableMechanism>().Activate();
			} else if(IsNearCauldron) {
				if (canvasName == "A")
				{
					cauldronCanvasA.SetActive(!cauldronCanvasA.activeSelf);
				} else if (canvasName == "B")
                {
					cauldronCanvasB.SetActive(!cauldronCanvasB.activeSelf);
                }

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

				if(!cauldronCanvasA.activeSelf || !cauldronCanvasB.activeSelf)
                {
					//Kamer A
					inputAnswerBRoomA = FindChildWithTag(cauldronCanvasA, "InputAnswerYBTag").GetComponent<TMP_InputField>().text;
					correctAnswerBRoomA = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().answerYB.ToString();
					inputAnswerDRoomA = FindChildWithTag(cauldronCanvasA, "InputAnswerYDTag").GetComponent<TMP_InputField>().text;
					correctAnswerDRoomA = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().answerYD.ToString();

					//Kamer B
					inputAnswerBRoomB = FindChildWithTag(cauldronCanvasB, "InputAnswerYBTag").GetComponent<TMP_InputField>().text;
					correctAnswerBRoomB = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().answerYB.ToString();
					inputAnswerDRoomB = FindChildWithTag(cauldronCanvasB, "InputAnswerYDTag").GetComponent<TMP_InputField>().text;
					correctAnswerDRoomB = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().answerYD.ToString();

					Debug.Log("Kamer A Geg. Antw. B: " + inputAnswerBRoomA);
					Debug.Log("Kamer A Cor. Antw. B: " + correctAnswerBRoomA);
					Debug.Log("Kamer A Geg. Antw. D: " + inputAnswerDRoomA);
					Debug.Log("Kamer A Cor. Antw. D: " + correctAnswerDRoomA);

					Debug.Log("Kamer B Geg. Antw. B: " + inputAnswerBRoomB);
					Debug.Log("Kamer B Cor. Antw. B: " + correctAnswerBRoomB);
					Debug.Log("Kamer B Geg. Antw. D: " + inputAnswerDRoomB);
					Debug.Log("Kamer B Cor. Antw. D: " + correctAnswerDRoomB);

					//Kamer A
					if (inputAnswerBRoomA == correctAnswerBRoomA && inputAnswerDRoomA == correctAnswerDRoomA)
                    {
						RoomACorrect = true;
						
						Debug.Log("Hoeraaa!");
                    }

					//Kamer B
					if (inputAnswerBRoomB == correctAnswerBRoomB && inputAnswerDRoomB == correctAnswerDRoomB)
					{
						RoomBCorrect = true;

						Debug.Log("Hoeraaa!");
					}

					if (RoomACorrect == true && RoomBCorrect == true)
					{
						movingWallA.GetComponent<InteractableMechanism>().Activate();
						movingWallB.GetComponent<InteractableMechanism>().Activate();
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
