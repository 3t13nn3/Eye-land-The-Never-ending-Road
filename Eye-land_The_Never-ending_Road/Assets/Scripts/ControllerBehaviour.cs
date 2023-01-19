using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerBehaviour : MonoBehaviour
{
    public float speed = 12f;
    public float sensitivity = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(speed * Time.deltaTime,0,0));
        }
        if(Input.GetKey(KeyCode.Q))
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime,0,0));
        }
        if(Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
        }
        if(Input.GetKey(KeyCode.Z))
        {
            transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        }

        var c = Camera.main.transform;
        c.Rotate(0, Input.GetAxis("Mouse X")* sensitivity, 0);
        c.Rotate(-Input.GetAxis("Mouse Y")* sensitivity, 0, 0);

    } 
}
