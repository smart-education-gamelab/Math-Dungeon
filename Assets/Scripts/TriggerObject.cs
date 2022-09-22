using System.Collections.Generic;
using UnityEngine;
public class TriggerObject : MonoBehaviour {
	[SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();
	[SerializeField] private GameObject KeyHintImage;

	private void Start() {
		//KeyHintImage = GameObject.FindWithTag("Key Hint");
		//KeyHintImage.SetActive(false);
	}

	private void Update() {
		
	}

	private void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Player") {
			foreach(GameObject objectToActivate in objectsToActivate) {
				if(Input.GetButtonDown("Activate")) {
					objectToActivate.GetComponent<InteractableMechanism>().Activate();
				}

				KeyHintImage.SetActive(true);
			}
			/*Debug.Log("bingo");*/
		}
	}

	private void OnTriggerExit(Collider other) {
		if(other.gameObject.tag == "Player") {
			KeyHintImage.SetActive(false);
		}
	}
}