using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPuzzle : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        Loader.LoadNetwork(Loader.Scene.PuzzleTwoGears);
    }
}