using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarBehaviour : MonoBehaviour
{
    private bool _start = false;
    public float speed = 12f;
    public float sensitivity = 10f;

    public GameObject rg;

    private List<QuadraticBezierCurve> _allCurves = new List<QuadraticBezierCurve>();

    private float t = 0.0f;
    private Vector3 oldDirection;

    private QuadraticBezierCurve currentPath;
    // Start is called before the first frame update
    void Start()
    {
        if (!EndGameManager.replayGame)
            BeginGameManager.loadMenuScene();
        new WaitForSeconds(5);
        this._allCurves = rg.GetComponent<RoadGeneratorBehaviour>()._allCurves;
        
    }

    // Update is called once per frame
    void Update()
    {   
        //  HandleMenuCamera();
        this._allCurves = rg.GetComponent<RoadGeneratorBehaviour>()._allCurves;
        if(!this._start) {
            if(this._allCurves.Count > 2) {
                currentPath = this._allCurves[2];
                transform.position = this._allCurves[2]._points[0];
                this._start = true;
            }
        } else {
            t += (Time.deltaTime * speed);

            Vector3 newPos = currentPath.CalculateBezierPoint(currentPath._points[0] ,currentPath._points[1] ,currentPath._points[2], currentPath._points[3] ,t);
            Vector3 direction = currentPath.CalculateBezierPoint(currentPath._points[0] ,currentPath._points[1] ,currentPath._points[2], currentPath._points[3], t + 0.01f) - newPos;

            if (t >= 1.0f)
            {
                t = 0.0f;
                oldDirection = direction;
                for (int i = 0; i < this._allCurves.Count; i++)
                {
                    if(this._allCurves[i] == currentPath) {
                        currentPath = this._allCurves[i+1];
                        break;
                    }
                }
                //currentCurve++;
            }

            transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);

            // if (currentCurve > 0)
            // {
                direction = Vector3.Lerp(oldDirection, direction, t * 10);
            // }
            
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        }
        
    }

    void HandleMenuCamera() {
        Scene scene = SceneManager.GetActiveScene();

        // Check if the name of the current Active Scene is your first Scene.
        if (scene.name == "MenuScene")
        {
            Debug.Log("YES");
            //scene.SetComponent<MainCamera>() =  GameObject.Find("Main Camera");
        }
    }
}
