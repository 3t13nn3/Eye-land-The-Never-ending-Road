using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeHandler : MonoBehaviour
{
    public bool _mode = true;

    // Start is called before the first frame update
    void Start()
    {    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMode() {
        this._mode = !this._mode;
        if(this._mode) {
            GameObject.Find("modeBtn").GetComponent<Image>().color = new Color32(0x78, 0xAB, 0x84, 0xFF);
        } else {
            GameObject.Find("modeBtn").GetComponent<Image>().color = new Color32(0xAB, 0x78, 0x84, 0xFF);
        }
    }
}
