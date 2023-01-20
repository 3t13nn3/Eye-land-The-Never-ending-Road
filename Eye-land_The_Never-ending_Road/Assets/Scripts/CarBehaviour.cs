using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBehaviour : MonoBehaviour
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
            transform.Translate(new Vector3(speed/1.5f * Time.deltaTime,0,0));
        }
        if(Input.GetKey(KeyCode.Q))
        {
            transform.Translate(new Vector3(-speed/1.5f * Time.deltaTime,0,0));
        }
        if(Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
        }
        if(Input.GetKey(KeyCode.Z))
        {
            transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        }
        
        transform.Rotate(0, Input.GetAxis("Mouse X")* sensitivity, 0);

        if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0, Time.deltaTime * sensitivity * 15f, 0);
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(0, -Time.deltaTime * sensitivity * 15f, 0);
        }

    } 
}
