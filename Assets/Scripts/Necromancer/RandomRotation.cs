using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.transform.eulerAngles = new Vector3(0, UnityEngine.Random.Range(0, 360), 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
