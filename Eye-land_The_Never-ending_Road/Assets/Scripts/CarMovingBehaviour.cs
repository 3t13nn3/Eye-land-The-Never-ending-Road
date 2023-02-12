using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovingBehaviour : MonoBehaviour
{
    private int _meanTime = 20;

    private float _totalTime = 0;

    private float _onRoadTime = 0;

    private float _onRoadRatio = 0;

    public float _sensitivity = 10f;

    private float _maxSpeed = 30.0f;

    private float _maxAngle = 15.0f;

    private float _screenMiddleX = Screen.width / 2;

    private float _screenMiddleY = Screen.height / 2;

    private static float _limitFactor = 0.15f;

    private float _leftLimitX = Screen.width * _limitFactor;

    private float _rightLimitX = Screen.width * (1 - _limitFactor);

    private float _topLimitY = Screen.height * (1 - _limitFactor);

    private float _bottomLimitY = Screen.height * _limitFactor;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(CalculateOnOffRoadRatio());
        Debug.Log("The on-road ratio is: " + this._onRoadRatio);
        // move the car when mouse pressed
        if (Input.GetMouseButton(0))
        {
            float mousePosX = Input.mousePosition.x;
            float mousePosY = Input.mousePosition.y;

            float newSpeed = 0.0f;
            float turnAngle = 0.0f;

            // Debug.Log("mid X:" + screenMiddleX + ", mid Y :" + screenMiddleY  + ", left lim X :" + leftLimitX + ", right lim X :" + rightLimitX
            //  + ", top lim Y :" + topLimitY + ", bot lim Y :" + bottomLimitY + ", pos X :" +mousePosX + ", pos Y :" + mousePosY);
            // Move forward
            if (mousePosY >= this._bottomLimitY && mousePosY <= this._topLimitY)
                newSpeed = ComputeSpeed(mousePosY);
            else if (mousePosY < this._bottomLimitY)
                newSpeed = 0.0f;
            else if (mousePosY > this._topLimitY) newSpeed = this._maxSpeed;

            // if mousePosX is at left side of screen then turn left, else turn right
            if (mousePosX >= this._leftLimitX && mousePosX <= this._rightLimitX)
                turnAngle = ComputeTurnAngle(mousePosX);
            else if (mousePosX < this._leftLimitX)
                turnAngle = -this._maxAngle;
            else if (mousePosX > this._rightLimitX) turnAngle = this._maxAngle;

            transform.Translate(new Vector3(0, 0, newSpeed * Time.deltaTime)); // apply speed
            transform
                .Rotate(0.0f,
                Time.deltaTime * this._sensitivity * turnAngle,
                0.0f); // apply rotation
        }
    }

    float ComputeSpeed(float mousePosY)
    {
        // if the mouse point at bottomLimitY or below, so the speed is 0
        if (mousePosY == this._screenMiddleY)
            return 0.0f;
        else
        {
            // if mousePoxY is at topLimitY, so the speed is maximal, else it decrease in term of mousePosY
            // return maxSpeed * (2 * ((mousePosY - bottomLimitY) / (topLimitY - bottomLimitY)) - 1);
            return this._maxSpeed *
            (mousePosY - this._bottomLimitY) /
            (this._topLimitY - this._bottomLimitY);
        }
    }

    float ComputeTurnAngle(float mousePosX)
    {
        if (mousePosX == this._screenMiddleX)
            return 0.0f;
        else
            return this._maxAngle *
            (
            2 *
            (mousePosX - this._leftLimitX) /
            (this._rightLimitX - this._leftLimitX) -
            1
            );
    }

    private bool IsOnRoad()
    {
        GameObject[] roadObjects = GameObject.FindGameObjectsWithTag("sphere");
        Collider vehicleCollider = gameObject.GetComponent<Collider>();

        foreach (GameObject roadObject in roadObjects) {
            SphereCollider [] roadColliders = roadObject.GetComponents<SphereCollider>();
            foreach (SphereCollider roadCollider in roadColliders) {
                //Debug.Log(roadCollider);
                if (vehicleCollider.bounds.Intersects(roadCollider.bounds)) {
                    return true;
                }
            }
        }

            return false;
    }

    private IEnumerator CalculateOnOffRoadRatio()
    {
        this._totalTime += Time.deltaTime;

        if (IsOnRoad()) {
            this._onRoadTime += Time.deltaTime;
            Debug.Log("IN THE ROAD");
        } else {
            Debug.Log("OUTTTT THE ROAD");
        }

        if (this._totalTime >= _meanTime) {
            this._totalTime -= _meanTime;
            this._onRoadTime -= this._onRoadRatio * _meanTime;
        }

        this._onRoadRatio = this._onRoadTime / this._totalTime;
        yield return new WaitForEndOfFrame();
    }
}
