using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : InteractableMechanism {
    [SerializeField] private Vector3 openedPosition = new Vector3(0,0,0);
    [SerializeField] private float speed = 1;

    public override void Activate() {
        base.Activate();
        StartCoroutine(Slide());
    }

    IEnumerator Slide() {
        while(AtOpenedPosition() == false) {
            this.transform.position = Vector3.MoveTowards(this.transform.position, openedPosition, Time.deltaTime * speed);
            yield return null;
        }
    }

    bool AtOpenedPosition() {
        if(this.transform.position == this.openedPosition) {
            return true;
        } else {
            return false;
        }
    }
}
