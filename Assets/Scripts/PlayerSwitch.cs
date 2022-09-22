using UnityEngine;

public class PlayerSwitch : MonoBehaviour {
    [SerializeField] private GameObject otherPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X)) {
            otherPlayer.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}