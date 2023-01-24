using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovingBehaviour : MonoBehaviour
{
    public float speed = 12f;
    public float sensitivity = 10f;

    private float maxSpeed = 30.0f;
    private float maxAngle = 15.0f;

    private float screenMiddleX = Screen.width / 2;
    private float screenMiddleY = Screen.height / 2;

    private static float limitFactor = 0.15f;
    private float leftLimitX = Screen.width * limitFactor;
    private float rightLimitX = Screen.width * (1 - limitFactor);
    private float topLimitY = Screen.height * (1 - limitFactor);
    private float bottomLimitY = Screen.height * limitFactor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // move the car when mouse pressed
        if (Input.GetMouseButton(0))
        {
            float mousePosX = Input.mousePosition.x;
            float mousePosY = Input.mousePosition.y;

            float new_speed = 0.0f;
            float turnAngle = 0.0f;

            // Debug.Log("mid X:" + screenMiddleX + ", mid Y :" + screenMiddleY  + ", left lim X :" + leftLimitX + ", right lim X :" + rightLimitX
            //  + ", top lim Y :" + topLimitY + ", bot lim Y :" + bottomLimitY + ", pos X :" +mousePosX + ", pos Y :" + mousePosY);

            // Move forward
            if (mousePosY >= bottomLimitY && mousePosY <= topLimitY)
                new_speed = computeSpeed(mousePosY);
            else if (mousePosY < bottomLimitY)
                new_speed = 0.0f;
            else if (mousePosY > topLimitY)
                new_speed = maxSpeed;

            // if mousePosX is at left side of screen then turn left, else turn right
            if (mousePosX >= leftLimitX && mousePosX <= rightLimitX)
                turnAngle = computeTurnAngle(mousePosX);
            else if (mousePosX < leftLimitX)
                turnAngle = -maxAngle;
            else if (mousePosX > rightLimitX)
                turnAngle = maxAngle;

            transform.Translate(new Vector3(0, 0, new_speed * Time.deltaTime)); // apply speed
            transform.Rotate(0.0f, Time.deltaTime * sensitivity * turnAngle, 0.0f); // apply rotation
        }
    }

    float computeSpeed(float mousePosY)
    {
        // if the mouse point at bottomLimitY or below, so the speed is 0
        if (mousePosY == screenMiddleY)
            return 0.0f;
        else
        {
            // if mousePoxY is at topLimitY, so the speed is maximal, else it decrease in term of mousePosY
            // return maxSpeed * (2 * ((mousePosY - bottomLimitY) / (topLimitY - bottomLimitY)) - 1);
            return maxSpeed * (mousePosY - bottomLimitY) / (topLimitY - bottomLimitY);
        }
    }

    float computeTurnAngle(float mousePosX)
    {
        if (mousePosX == screenMiddleX)
            return 0.0f;
        else
            return maxAngle * (2 * (mousePosX - leftLimitX) / (rightLimitX - leftLimitX) - 1);
    }

    // marche pas
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.name == "curve")
        {
            Debug.Log("IN THE ROAD");
        }    
    }
 }
