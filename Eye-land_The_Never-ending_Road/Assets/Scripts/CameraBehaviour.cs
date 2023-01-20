using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public float speed = 12f;
    public float sensitivity = 10f;
    public float smoothness = 100000000000000000f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject car = GameObject.Find("Car");
        Vector3 pos = car.transform.position;
        Vector3 cameraOffset = new Vector3(0, 1.25f, -3.5f);
        Vector3 newPos = car.transform.position + car.transform.TransformDirection(cameraOffset);
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * speed/2);
        float acceleration = Mathf.Lerp(0, 8f, Time.time);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(car.transform.position - transform.position), Time.deltaTime * acceleration);
    } 
}
