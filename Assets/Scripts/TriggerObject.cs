using System.Collections.Generic;
using UnityEngine;
public class TriggerObject : MonoBehaviour {
	[SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();
	[SerializeField] private GameObject keyHintImage;

	private bool playerIsNear = false;

	private void Start() {

	}

	private void Update() {
		foreach(GameObject objectToActivate in objectsToActivate) {
			if(Input.GetButton("Activate") && playerIsNear) {
				Debug.Log("check");
				objectToActivate.GetComponent<InteractableMechanism>().Activate();
			}
		}
	}

	private void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Player")) {
			keyHintImage.SetActive(true);
			playerIsNear = true;
		}
	}

	private void OnTriggerExit(Collider other) {
		if(other.gameObject.CompareTag("Player")) {
			keyHintImage.SetActive(false);
			playerIsNear = false;
		}
	}
}