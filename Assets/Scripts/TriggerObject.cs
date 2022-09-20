using System.Collections.Generic;
using UnityEngine;
public class TriggerObject : MonoBehaviour {
	[SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();

	private void Update() {
		
	}

	private void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Player") {
			foreach(GameObject objectToActivate in objectsToActivate) {
				objectToActivate.GetComponent<InteractableMechanism>().Activate();
			}
			/*Debug.Log("bingo");*/
		}
	}
}