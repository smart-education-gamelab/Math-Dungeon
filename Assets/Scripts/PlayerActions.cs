using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour {
    private bool isNearActivationBall;
    private GameObject ballThatIsNear;

	public bool IsNearActivationBall {
		get => isNearActivationBall;
		set => isNearActivationBall = value;
	}
	public GameObject BallThatIsNear {
		get => ballThatIsNear;
		set => ballThatIsNear = value;
	}

	// Start is called before the first frame update
	void Start()
    {
        IsNearActivationBall = false;
    }

    // Update is called once per frame
    void Update()
    {
		if(Input.GetButtonDown("Activate")) {
			if(IsNearActivationBall) {
				BallThatIsNear.GetComponent<InteractableMechanism>().Activate();
			}
		}
    }
}
