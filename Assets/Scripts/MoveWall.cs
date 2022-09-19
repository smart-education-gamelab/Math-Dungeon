using UnityEngine;
public class MoveWall : MonoBehaviour {
	[SerializeField] private Vector3 target = new Vector3(1, 1, 1);
	[SerializeField] private float speed = 1;
	[SerializeField] private GameObject wall;
	private void Update() {
		
	}

	private void MoveIt() {
		// Moves the object to target position
		wall.transform.position = Vector3.MoveTowards(wall.transform.position, wall.transform.position + target, Time.deltaTime * speed);
	}

	private void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Player") {
			MoveIt();
			Debug.Log("bingo");
		}
	}
}