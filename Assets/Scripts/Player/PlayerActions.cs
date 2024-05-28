using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Newtonsoft.Json;
using System.Linq;

public class PlayerActions : NetworkBehaviour
{
    private bool isNearActivationBall;
    private GameObject ballThatIsNear;

    private bool isNearCauldron;
    private GameObject cauldronCanvasA;
    private GameObject cauldronCanvasB;

    private bool isNearLever;

    private bool isNearPipeLever;
    private GameObject sliderCanvas;

    private GameObject potionControllerRef;

    [SerializeField]
    private GameObject movingWallA;

    [SerializeField]
    private GameObject movingWallB;

    private string canvasName;

    public bool IsNearActivationBall
    {
        get => isNearActivationBall;
        set => isNearActivationBall = value;
    }
    public GameObject BallThatIsNear
    {
        get => ballThatIsNear;
        set => ballThatIsNear = value;
    }

    public bool IsNearCauldron
    {
        get => isNearCauldron;
        set => isNearCauldron = value;
    }
    public string CanvasName
    {
        get => canvasName;
        set => canvasName = value;
    }

    public bool IsNearLever
    {
        get => isNearLever;
        set => isNearLever = value;
    }

    public bool IsNearPipeLever
    {
        get => isNearPipeLever;
        set => isNearPipeLever = value;
    }

    public GameObject SliderCanvas
    {
        get => sliderCanvas;
        set => sliderCanvas = value;
    }

    //Kamer A
    public GameObject CauldronCanvasA
    {
        get => cauldronCanvasA;
        set => cauldronCanvasA = value;
    }

    //Kamer B
    public GameObject CauldronCanvasB
    {
        get => cauldronCanvasB;
        set => cauldronCanvasB = value;
    }

    //Kamer A
    private PointData inputAnswerCRoomA;
    private PointData correctAnswerCRoomA;

    //Kamer B
    private PointData inputAnswerFRoomB;
    private PointData correctAnswerFRoomB;

    private bool RoomACorrect;
    private bool RoomBCorrect;

    private Dictionary<ulong, PointData> clientAnswers = new Dictionary<ulong, PointData>();

    // Start is called before the first frame update
    void Start()
    {
        // Initialize input and correct answers with placeholder values
        inputAnswerCRoomA = new PointData { x = 666, y = 668 };
        correctAnswerCRoomA = new PointData { x = 667, y = 669 };

        inputAnswerFRoomB = new PointData { x = 670, y = 672 };
        correctAnswerFRoomB = new PointData { x = 671, y = 673 };

        IsNearActivationBall = false;
        IsNearCauldron = false;
        IsNearLever = false;

        movingWallA = GameObject.Find("DoorA");
        movingWallB = GameObject.Find("DoorB");

        RoomACorrect = false;
        RoomBCorrect = false;
    }

    // Update is called once per frame
    void Update()
    {
        potionControllerRef = GameObject.FindWithTag("PotionController");
        if (Input.GetButtonDown("Activate"))
        {
            if (IsNearActivationBall)
            {
                BallThatIsNear.GetComponent<InteractableMechanism>().Activate();
            }
            else if (IsNearCauldron)
            {
                if (canvasName == "A")
                {
                    cauldronCanvasA.SetActive(!cauldronCanvasA.activeSelf);
                }
                else if (canvasName == "B")
                {
                    cauldronCanvasB.SetActive(!cauldronCanvasB.activeSelf);
                }

                this.gameObject.GetComponent<PlayerMotor>().CanWalk = !this.gameObject.GetComponent<PlayerMotor>().CanWalk;
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
            else if (IsNearLever)
            {
                // Kamer A
                inputAnswerCRoomA = new PointData
                {
                    x = int.Parse(FindChildWithTag(cauldronCanvasA, "InputAnswerXCTag").GetComponent<TMP_InputField>().text),
                    y = int.Parse(FindChildWithTag(cauldronCanvasA, "InputAnswerYCTag").GetComponent<TMP_InputField>().text)
                };
                correctAnswerCRoomA = new PointData
                {
                    x = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().AnswerXC,
                    y = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().AnswerYC
                };

                Debug.Log($"Kamer A correct answers: X = {correctAnswerCRoomA.x}, Y = {correctAnswerCRoomA.y}");

                // Kamer B
                inputAnswerFRoomB = new PointData
                {
                    x = int.Parse(FindChildWithTag(cauldronCanvasB, "InputAnswerXFTag").GetComponent<TMP_InputField>().text),
                    y = int.Parse(FindChildWithTag(cauldronCanvasB, "InputAnswerYFTag").GetComponent<TMP_InputField>().text)
                };
                correctAnswerFRoomB = new PointData
                {
                    x = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().AnswerXF,
                    y = potionControllerRef.GetComponent<LinearFormulaGeneratorSync>().AnswerYF
                };

                Debug.Log($"Kamer B correct answers: X = {correctAnswerFRoomB.x}, Y = {correctAnswerFRoomB.y}");

                PointData answerToSend;
                if (inputAnswerCRoomA.x == 0 && inputAnswerCRoomA.y == 0)
                {
                    Debug.Log("Kamer B antwoord x: " + inputAnswerFRoomB.x);
                    Debug.Log("Kamer B antwoord y: " + inputAnswerFRoomB.y);
                    answerToSend = inputAnswerFRoomB;
                }
                else
                {
                    Debug.Log("Kamer A antwoord x: " + inputAnswerCRoomA.x);
                    Debug.Log("Kamer A antwoord y: " + inputAnswerCRoomA.y);
                    answerToSend = inputAnswerCRoomA;
                }
                string jsonPayload = JsonConvert.SerializeObject(answerToSend);
                SendAnswersServerRpc(jsonPayload);
            }
            else if (IsNearPipeLever)
            {
                sliderCanvas.SetActive(!sliderCanvas.activeSelf);

                this.gameObject.GetComponent<PlayerMotor>().CanWalk = !this.gameObject.GetComponent<PlayerMotor>().CanWalk;

                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendAnswersServerRpc(string payload, ServerRpcParams serverRpcParams = default)
    {
        ulong clientID = serverRpcParams.Receive.SenderClientId;
        PointData answersFromClient = JsonConvert.DeserializeObject<PointData>(payload);

        clientAnswers[clientID] = answersFromClient;

        Debug.Log("Client ID: " + clientID);
        Debug.Log("clients: " + string.Join(", ", clientAnswers.Keys.ToArray()));
        Debug.Log("aantal clients: " + clientAnswers.Count);

        bool isItGood = false;

        if (clientID == 1)
        {
            Debug.Log($"Checking answers for Client 1 against correct answers: X = {correctAnswerCRoomA.x}, Y = {correctAnswerCRoomA.y}");
            if (answersFromClient.x == correctAnswerCRoomA.x && answersFromClient.y == correctAnswerCRoomA.y)
            {
                isItGood = true;
            }
        }
        else
        {
            Debug.Log($"Checking answers for Client {clientID} against correct answers: X = {correctAnswerFRoomB.x}, Y = {correctAnswerFRoomB.y}");
            if (answersFromClient.x == correctAnswerFRoomB.x && answersFromClient.y == correctAnswerFRoomB.y)
            {
                isItGood = true;
            }
        }

        handleSubmittedAnswersClientRpc(isItGood);
    }

    [ClientRpc]
    private void handleSubmittedAnswersClientRpc(bool isGood)
    {
        if (isGood)
        {
            Debug.Log("Hoeraaaaaaaaaaaaaaaaaaaaaaaaaa BOTH");
            movingWallA.GetComponent<InteractableMechanism>().Activate();
            movingWallB.GetComponent<InteractableMechanism>().Activate();
        }
        else
        {
            Debug.Log("Incorrect answers.");
        }
    }

    GameObject FindChildWithTag(GameObject parent, string tag)
    {
        foreach (Transform transform in parent.transform)
        {
            if (transform.CompareTag(tag))
            {
                return transform.gameObject;
            }
        }
        return null;
    }
}
