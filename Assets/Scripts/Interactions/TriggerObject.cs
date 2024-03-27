using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerObject : InteractableMechanism {
	[SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();
	[SerializeField] private GameObject keyHintImage;

	[SerializeField] private GameObject cauldronCanvasLink;

	//private Image crosshairImage; // Referentie naar de image component van de crosshair

	private void Start() {
		//crosshairImage = GameObject.FindWithTag("Crosshair").GetComponent<Image>();
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
			} else if(this.gameObject.CompareTag("Cauldron")) {
				other.gameObject.GetComponent<PlayerActions>().IsNearCauldron = true;
				other.gameObject.GetComponent<PlayerActions>().CauldronCanvas = cauldronCanvasLink;
			}
			//crosshairImage.color = Color.magenta;
		}
	}

	private void OnTriggerExit(Collider other) {
		if(other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<LocalPlayerManager>().IsLocalPlayer) {
			keyHintImage.SetActive(false);
			if(this.gameObject.CompareTag("Ball")) {
				other.gameObject.GetComponent<PlayerActions>().IsNearActivationBall = false;
			} else if(this.gameObject.CompareTag("Cauldron")) {
				other.gameObject.GetComponent<PlayerActions>().IsNearCauldron = false;
			}
			//crosshairImage.color = Color.white;
		}
	}
}