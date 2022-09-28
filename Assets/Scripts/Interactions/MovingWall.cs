using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

	private void Update() {
        /*if(!IsOwner) {
            return;
        }*/
	}

	public override void Activate() {
        base.Activate();
        if(isAtStart) {
            RequestFireServerRpc(alternatePosition);
            //StartCoroutine(Slide(alternatePosition));
        } else {
            RequestFireServerRpc(startPosition);
            //StartCoroutine(Slide(startPosition));
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestFireServerRpc(Vector3 dir) {
        FireClientRpc(dir);
    }

    [ClientRpc]
    private void FireClientRpc(Vector3 dir) {
        //if(!IsOwner) {
            StartCoroutine(Slide(dir));
        //}
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
