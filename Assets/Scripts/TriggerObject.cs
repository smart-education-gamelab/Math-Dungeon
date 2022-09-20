using UnityEngine;
public class TriggerObject : MonoBehaviour {
	[SerializeField] private GameObject objectToActivate;

	private void Update() {
		
	}

	private void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Player") {
			objectToActivate.GetComponent<InteractableMechanism>().Activate();
			/*Debug.Log("bingo");*/
		}
	}
}