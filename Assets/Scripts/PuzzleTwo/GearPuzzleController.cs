using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearPuzzleController : MonoBehaviour {
    [SerializeField] private List<GameObject> smallGearList = new List<GameObject>();
    [SerializeField] private List<GameObject> bigGearList = new List<GameObject>();

    [SerializeField] private int minimumValueSmallGear = 0;
    [SerializeField] private int maximumValueSmallGear = 10;

    private List<int> smallGearValues = new List<int>();

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        for(int i = 0; i <= smallGearList.Count; i++) {
            smallGearValues[smallGearValues[i]] = Random.Range(minimumValueSmallGear, maximumValueSmallGear);
        }

		for(int j = 0; j < smallGearList.Count; j++) {

		}
    }
}
