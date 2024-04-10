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
	private string inputAnswerARoomA;
	private string correctAnswerARoomA;
	private string inputAnswerBRoomA;
	private string correctAnswerBRoomA;

	//Kamer B
	private string inputAnswerCRoomB;
	private string correctAnswerCRoomB;
	private string inputAnswerDRoomB;
	private string correctAnswerDRoomB;

	private bool RoomACorrect;
	private bool RoomBCorrect;

	// Start is called before the first frame update
	void Start()
    {
		//Kamer A
		inputAnswerARoomA = "aap";
		correctAnswerARoomA = "noot";
		inputAnswerBRoomA = "mies";
		correctAnswerBRoomA = "hond";

		//Kamer B
		inputAnswerCRoomB = "paa";
		correctAnswerCRoomB = "toon";
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
					inputAnswerARoomA = FindChildWithTag(cauldronCanvasA, "InputAnswerYBTag").GetComponent<TMP_InputField>().text;
					correctAnswerARoomA = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().answerYA.ToString();
					inputAnswerBRoomA = FindChildWithTag(cauldronCanvasA, "InputAnswerYDTag").GetComponent<TMP_InputField>().text;
					correctAnswerBRoomA = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().answerYB.ToString();

					//Kamer B
					inputAnswerCRoomB = FindChildWithTag(cauldronCanvasB, "InputAnswerYBTag").GetComponent<TMP_InputField>().text;
					correctAnswerCRoomB = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().answerYC.ToString();
					inputAnswerDRoomB = FindChildWithTag(cauldronCanvasB, "InputAnswerYDTag").GetComponent<TMP_InputField>().text;
					correctAnswerDRoomB = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().answerYD.ToString();

					RequestAnswersServerRpc();
/*
					Debug.Log("Kamer A Geg. Antw. A: " + inputAnswerARoomA);
					Debug.Log("Kamer A Cor. Antw. A: " + correctAnswerARoomA);
					Debug.Log("Kamer A Geg. Antw. B: " + inputAnswerBRoomA);
					Debug.Log("Kamer A Cor. Antw. B: " + correctAnswerBRoomA);

					Debug.Log("Kamer B Geg. Antw. C: " + inputAnswerCRoomB);
					Debug.Log("Kamer B Cor. Antw. C: " + correctAnswerCRoomB);
					Debug.Log("Kamer B Geg. Antw. D: " + inputAnswerDRoomB);
					Debug.Log("Kamer B Cor. Antw. D: " + correctAnswerDRoomB);

					//Kamer A
					if (inputAnswerARoomA == correctAnswerARoomA && inputAnswerBRoomA == correctAnswerBRoomA)
                    {
						RoomACorrect = true;
						
						Debug.Log("Hoeraaa!");
                    }

					//Kamer B
					if (inputAnswerCRoomB == correctAnswerCRoomB && inputAnswerDRoomB == correctAnswerDRoomB)
					{
						RoomBCorrect = true;

						Debug.Log("Hoeraaa!");
					}

					if (RoomACorrect == true && RoomBCorrect == true)
					{
						movingWallA.GetComponent<InteractableMechanism>().Activate();
						movingWallB.GetComponent<InteractableMechanism>().Activate();
					}*/

				}
			}
		}
    }

	[ServerRpc(RequireOwnership = false)]
	private void RequestAnswersServerRpc()
	{
		
		ArrayList jsonPayloadAnswersList = new ArrayList();
		jsonPayloadAnswersList.Add(inputAnswerARoomA);
		jsonPayloadAnswersList.Add(inputAnswerBRoomA);
		string jsonPayload = JsonConvert.SerializeObject(jsonPayloadAnswersList);

		RequestAnswersClientRpc(jsonPayload);
	}

	[ClientRpc]
	private void RequestAnswersClientRpc(string jsonPayload)
	{
		Debug.Log("client");
		ArrayList tempSyncArrayList = JsonConvert.DeserializeObject<ArrayList>(jsonPayload);
		tempSyncArrayList.Add(inputAnswerCRoomB);
		tempSyncArrayList.Add(inputAnswerDRoomB);
		string jsonPayloadTwo = JsonConvert.SerializeObject(tempSyncArrayList);
		GatherAnswersServerRpc(jsonPayloadTwo);
	}

	[ServerRpc(RequireOwnership = false)]
	private void GatherAnswersServerRpc(string jsonPayloadTwo)
    {
		CheckAnswers(jsonPayloadTwo, correctAnswerARoomA, correctAnswerBRoomA, correctAnswerCRoomB, correctAnswerDRoomB);
    }

	[ClientRpc]
	private void CheckAnswers(string jsonPayloadThree, string corAnswAA, string corAnswBA, string corAnswCB, string corAnswDB) {
		ArrayList allAnswersArrayList = JsonConvert.DeserializeObject<ArrayList>(jsonPayloadThree);
		string inAnswAA = JsonConvert.DeserializeObject<string>(allAnswersArrayList[0].ToString());
		string inAnswBA = JsonConvert.DeserializeObject<string>(allAnswersArrayList[1].ToString());
		string inAnswCB = JsonConvert.DeserializeObject<string>(allAnswersArrayList[2].ToString());
		string inAnswDB = JsonConvert.DeserializeObject<string>(allAnswersArrayList[3].ToString());


		Debug.Log("Kamer A Geg. Antw. A: " + inAnswAA);
		Debug.Log("Kamer A Cor. Antw. A: " + corAnswAA);
		Debug.Log("Kamer A Geg. Antw. B: " + inAnswBA);
		Debug.Log("Kamer A Cor. Antw. B: " + corAnswBA);

		Debug.Log("Kamer B Geg. Antw. C: " + inAnswCB);
		Debug.Log("Kamer B Cor. Antw. C: " + corAnswCB);
		Debug.Log("Kamer B Geg. Antw. D: " + inAnswDB);
		Debug.Log("Kamer B Cor. Antw. D: " + corAnswDB);

		//Kamer A
		if (inAnswAA == corAnswAA && inAnswBA == corAnswBA)
		{
			RoomACorrect = true;

			Debug.Log("Hoeraaa!");
		}

		//Kamer B
		if (inAnswCB == corAnswCB && inAnswDB == corAnswDB)
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
