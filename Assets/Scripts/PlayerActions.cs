using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour {
    private bool isNearActivationBall;
    private GameObject ballThatIsNear;

	private bool isNearCauldron;
	private GameObject cauldronCanvas;

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
			}
		}
    }
}
