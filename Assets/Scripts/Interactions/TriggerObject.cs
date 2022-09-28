using System.Collections.Generic;
using UnityEngine;
public class TriggerObject : InteractableMechanism {
	[SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();
	[SerializeField] private GameObject keyHintImage;

	private void Start() {

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
			other.gameObject.GetComponent<PlayerActions>().IsNearActivationBall = true;
			other.gameObject.GetComponent<PlayerActions>().BallThatIsNear = this.gameObject;
		}
	}

	private void OnTriggerExit(Collider other) {
		if(other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<LocalPlayerManager>().IsLocalPlayer) {
			keyHintImage.SetActive(false);
			other.gameObject.GetComponent<PlayerActions>().IsNearActivationBall = false;
		}
	}
}