using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerObject : InteractableMechanism {
	[SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();
	[SerializeField] private GameObject keyHintImage;

	[SerializeField] private GameObject cauldronCanvasLinkA;
	[SerializeField] private GameObject cauldronCanvasLinkB;

	[SerializeField] private GameObject sliderCanvas;

	private string canvasName;

	//private Image crosshairImage; // Referentie naar de image component van de crosshair

	private void Start() {
		//crosshairImage = GameObject.FindWithTag("Crosshair").GetComponent<Image>();
		canvasName = "C";
	}

	public override void Activate() {
		base.Activate();
		Debug.Log("check");
		foreach(GameObject objectToActivate in objectsToActivate) {
			objectToActivate.GetComponent<InteractableMechanism>().Activate();
		}
	}

	private void Update() {

	}

	private void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<LocalPlayerManager>().IsLocalPlayer) {
			keyHintImage.SetActive(true);
			if(this.gameObject.CompareTag("Ball")) {
				other.gameObject.GetComponent<PlayerActions>().IsNearActivationBall = true;
				other.gameObject.GetComponent<PlayerActions>().BallThatIsNear = this.gameObject;
			} else if(this.gameObject.CompareTag("Cauldron A") || this.gameObject.CompareTag("Cauldron B")) {
				other.gameObject.GetComponent<PlayerActions>().IsNearCauldron = true;
				other.gameObject.GetComponent<PlayerActions>().CauldronCanvasA = cauldronCanvasLinkA;
				other.gameObject.GetComponent<PlayerActions>().CauldronCanvasB = cauldronCanvasLinkB;
				if (this.gameObject.CompareTag("Cauldron A"))
                {
					canvasName = "A";
					other.gameObject.GetComponent<PlayerActions>().CanvasName = canvasName;
                } else if (this.gameObject.CompareTag("Cauldron B"))
                {
					canvasName= "B";
					other.gameObject.GetComponent<PlayerActions>().CanvasName = canvasName;
                }
			} else if (this.gameObject.CompareTag("Lever"))
            {
				other.gameObject.GetComponent<PlayerActions>().IsNearLever = true;
            } else if(this.gameObject.CompareTag("PipeLever"))
            {
				other.gameObject.GetComponent<PlayerActions>().IsNearPipeLever = true;
				other.gameObject.GetComponent<PlayerActions>().SliderCanvas = sliderCanvas;
			}
			//crosshairImage.color = Color.magenta;
		}
	}

	private void OnTriggerExit(Collider other) {
		if(other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<LocalPlayerManager>().IsLocalPlayer) {
			keyHintImage.SetActive(false);
			if(this.gameObject.CompareTag("Ball")) {
				other.gameObject.GetComponent<PlayerActions>().IsNearActivationBall = false;
			} else if(this.gameObject.CompareTag("Cauldron A") || this.gameObject.CompareTag("Cauldron B")) {
				other.gameObject.GetComponent<PlayerActions>().IsNearCauldron = false;
			}
			//crosshairImage.color = Color.white;
		}
	}
}