using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Newtonsoft.Json;

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
	private string inputAnswerCXRoomA;
	private string correctAnswerCXRoomA;
	private string inputAnswerCYRoomA;
	private string correctAnswerCYRoomA;

	//Kamer B
	private string inputAnswerFXRoomB;
	private string correctAnswerFXRoomB;
	private string inputAnswerFYRoomB;
	private string correctAnswerFYRoomB;

	private bool RoomACorrect;
	private bool RoomBCorrect;

	// Start is called before the first frame update
	void Start()
    {
		//Kamer A
		inputAnswerCXRoomA = "aap";
		correctAnswerCXRoomA = "noot";
		inputAnswerCYRoomA = "mies";
		correctAnswerCYRoomA = "hond";

		//Kamer B
		inputAnswerFXRoomB = "paa";
		correctAnswerFXRoomB = "toon";
		inputAnswerFYRoomB = "seim";
		correctAnswerFYRoomB = "dnoh";


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
				if (canvasName == "A") {
					cauldronCanvasA.SetActive(!cauldronCanvasA.activeSelf);
				} else if (canvasName == "B") {
					cauldronCanvasB.SetActive(!cauldronCanvasB.activeSelf);
                }

				this.gameObject.GetComponent<PlayerMotor>().CanWalk = !this.gameObject.GetComponent<PlayerMotor>().CanWalk;
				if(Cursor.lockState == CursorLockMode.Locked) {
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
                } else {
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
                }

				if(!cauldronCanvasA.activeSelf || !cauldronCanvasB.activeSelf) {
					//Kamer A
					inputAnswerCXRoomA = FindChildWithTag(cauldronCanvasA, "InputAnswerXCTag").GetComponent<TMP_InputField>().text;
					correctAnswerCXRoomA = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().answerXC.ToString();
					inputAnswerCYRoomA = FindChildWithTag(cauldronCanvasA, "InputAnswerYCTag").GetComponent<TMP_InputField>().text;
					correctAnswerCYRoomA = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().answerYC.ToString();

					//Kamer B
					inputAnswerFXRoomB = FindChildWithTag(cauldronCanvasB, "InputAnswerXFTag").GetComponent<TMP_InputField>().text;
					correctAnswerFXRoomB = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().answerXF.ToString();
					inputAnswerFYRoomB = FindChildWithTag(cauldronCanvasB, "InputAnswerYFTag").GetComponent<TMP_InputField>().text;
					correctAnswerFYRoomB = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().answerYF.ToString();

					RequestAnswersServerRpc();
				}
			}
		}
    }

	[ServerRpc(RequireOwnership = false)]
	private void RequestAnswersServerRpc()
	{
		
		ArrayList jsonPayloadAnswersList = new ArrayList();
		jsonPayloadAnswersList.Add(inputAnswerFXRoomB);
		jsonPayloadAnswersList.Add(inputAnswerFYRoomB);
		string jsonPayload = JsonConvert.SerializeObject(jsonPayloadAnswersList);

		RequestAnswersClientRpc(jsonPayload);
	}

	[ClientRpc]
	private void RequestAnswersClientRpc(string jsonPayload)
	{
		Debug.Log("client");
		ArrayList tempSyncArrayList = JsonConvert.DeserializeObject<ArrayList>(jsonPayload);
		tempSyncArrayList.Add(inputAnswerCXRoomA);
		tempSyncArrayList.Add(inputAnswerCYRoomA);
		string jsonPayloadTwo = JsonConvert.SerializeObject(tempSyncArrayList);
		GatherAnswersServerRpc(jsonPayloadTwo);
	}

	[ServerRpc(RequireOwnership = false)]
	private void GatherAnswersServerRpc(string jsonPayloadTwo)
    {
		CheckAnswers(jsonPayloadTwo, correctAnswerCXRoomA, correctAnswerCYRoomA, correctAnswerFXRoomB, correctAnswerFYRoomB);
    }

	[ClientRpc]
	private void CheckAnswers(string jsonPayloadThree, string corAnswCX, string corAnswCY, string corAnswFX, string corAnswFY) {
		ArrayList allAnswersArrayList = JsonConvert.DeserializeObject<ArrayList>(jsonPayloadThree);
		string inAnswCX = JsonConvert.DeserializeObject<string>(allAnswersArrayList[2].ToString());
		string inAnswCY = JsonConvert.DeserializeObject<string>(allAnswersArrayList[3].ToString());
		string inAnswFX = JsonConvert.DeserializeObject<string>(allAnswersArrayList[0].ToString());
		string inAnswFY = JsonConvert.DeserializeObject<string>(allAnswersArrayList[1].ToString());


		Debug.Log("Kamer A Geg. Antw. X van punt C: " + inAnswCX);
		Debug.Log("Kamer A Cor. Antw. X van punt C: " + corAnswCX);
		Debug.Log("Kamer A Geg. Antw. Y van punt C: " + inAnswCY);
		Debug.Log("Kamer A Cor. Antw. Y van punt C: " + corAnswCY);

		Debug.Log("Kamer B Geg. Antw. X van punt F: " + inAnswFX);
		Debug.Log("Kamer B Cor. Antw. X van punt F: " + corAnswFX);
		Debug.Log("Kamer B Geg. Antw. Y van punt F: " + inAnswFY);
		Debug.Log("Kamer B Cor. Antw. Y van punt F: " + corAnswFY);

		//Kamer A
		if (inAnswCX == corAnswCX && inAnswCY == corAnswCY)
		{
			RoomACorrect = true;

			Debug.Log("Hoeraaa!");
		}

		//Kamer B
		if (inAnswFX == corAnswFX && inAnswFY == corAnswFY)
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
