using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : InteractableMechanism {
    [SerializeField] private Vector3 alternatePosition = new Vector3(0, 0, 0);
    [SerializeField] private float speed = 1;

    private Vector3 startPosition;
    private bool isAtStart;

	private void Start() {
        startPosition = this.transform.localPosition;
        isAtStart = true;
	}

	public override void Activate() {
        base.Activate();
        if(isAtStart) {
            StartCoroutine(Slide(alternatePosition));
        } else {
            StartCoroutine(Slide(startPosition));
        }
    }

    IEnumerator Slide(Vector3 toPosition) {
        while(DoneMovingTo(toPosition) == false) {
            this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, toPosition, Time.deltaTime * speed);
            yield return null;
        }
        isAtStart = !isAtStart;
    }

    bool DoneMovingTo(Vector3 toPosition) {
        if(this.transform.localPosition == toPosition) {
            return true;
        } else {
            return false;
        }
    }
}
