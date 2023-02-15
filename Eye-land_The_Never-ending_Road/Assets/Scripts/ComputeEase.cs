using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using Python.Runtime;
using System.IO;

using System.Diagnostics;

public class ComputeEase : MonoBehaviour
{
    private bool _mode;
    // Define Objects to recover data from
    public GameObject _road ;
    public GameObject _car;
    public GameObject _oculo;

    // Player Ability
    public float _traveledDistanceRatio; // can never really reach 1.0
    public float _jerksRatio;
    public float _disturbRatio;
    public float _onRoadRatio;
    public float _averageSpeedRatio;
    public float _totalRatio;

    // Flow / Fun
    public float _pupilSizeRatio;

    public List<float> _totalRatioHistory = new List<float>();

    void ComputeFilesToShareWithPython() {
        using (StreamWriter writer = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "/Python/data_in.txt"))
        {
            writer.WriteLine(this._traveledDistanceRatio);
            writer.WriteLine(this._jerksRatio);
            writer.WriteLine(this._disturbRatio);
            writer.WriteLine(this._onRoadRatio);
            writer.WriteLine(this._averageSpeedRatio);
        }

        using (StreamReader reader = new StreamReader(System.IO.Directory.GetCurrentDirectory() + "/Python/predict.txt"))
        {
            int r = int.Parse(reader.ReadLine());
            if(r == 1) {
                this._totalRatio += 0.05f;
            } else {
                this._totalRatio += 0.05f;
            }
        }
        

    }

    void PreComputeAllRatios() {
        this._onRoadRatio = this._car.GetComponent<CarMovingBehaviour>()._meanOnRoad;
        this._averageSpeedRatio = this._car.GetComponent<CarMovingBehaviour>()._meanSpeed / this._car.GetComponent<CarMovingBehaviour>()._maxSpeed;
        this._traveledDistanceRatio = Mathf.Clamp(this._road.GetComponent<RoadGeneratorBehaviour>().GetPlayerDistance() / 1000f, 0f, 1f);
        
        this._disturbRatio = Mathf.Clamp(1f - (this._oculo.GetComponent<OculoBehaviour>().GetTheNumberOfFixationsInLastNSecond() / 10f),  0f, 1f); // base the ratio on 10 fixations
        this._jerksRatio = Mathf.Clamp(1f - (this._oculo.GetComponent<OculoBehaviour>().GetTheNumberOfObjectJerkInLastNSecond() / 20f),  0f, 1f);
        //this._pupilSizeRatio
    }

    void ComputeAllRatios() {
        float currentRatio = Mathf.Clamp(
                                this._traveledDistanceRatio * 0.15f +
                                this._averageSpeedRatio * this._onRoadRatio * 0.6f +
                                this._disturbRatio * 0.25f +
                                this._jerksRatio * 0.15f +
                                this._pupilSizeRatio * 0.0f,
                            0f, 1f);
        this._totalRatioHistory.RemoveAt(0);
        this._totalRatioHistory.Add(currentRatio);

        this._totalRatio = this._totalRatioHistory.Count > 0 ? this._totalRatioHistory.Average() : 0.25f;
    }
    // Start is called before the first frame update
    void Start()
    {
        this._mode = GameObject.Find("modeBtn").GetComponent<ModeHandler>()._mode;
        if(GameObject.Find("CanvasMenu") != null)
            Destroy(GameObject.Find("CanvasMenu"));
        if(GameObject.Find("CanvasEnd") != null)
            Destroy(GameObject.Find("CanvasEnd"));

        if(this._mode) {
            for (int i = 0; i < 100; i++)
            {
                this._totalRatioHistory.Add(0.25f);
            }
        } else {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "python";
            startInfo.Arguments = System.IO.Directory.GetCurrentDirectory() + "/Python/predict.py";
            process.StartInfo = startInfo;
            process.Start();

            // process.WaitForExit();
            // process.Close();
        }

        InvokeRepeating("ComputeFilesToShareWithPython", 0.0f, 1f);

    }

    // Update is called once per frame
    void Update()
    {   
        PreComputeAllRatios();
        if(this._mode) {
            UnityEngine.Debug.Log("EASE METHOD 1");
            ComputeAllRatios();
        } else {
            UnityEngine.Debug.Log("EASE METHOD 2");
        }
        
        // UnityEngine.Debug.Log("Ratio of On Road " + this._onRoadRatio);
        // UnityEngine.Debug.Log("Ratio of Speed " + this._averageSpeedRatio);
        // UnityEngine.Debug.Log("Ratio of Distance " + this._traveledDistanceRatio);
        // UnityEngine.Debug.Log("Ratio of Jerks " + this._jerksRatio);
        // UnityEngine.Debug.Log("Ratio of Disturb " + this._disturbRatio);
        // UnityEngine.Debug.Log("Ratio of Pupil Size " + this._pupilSizeRatio);
        // UnityEngine.Debug.Log("EASE RATIO: " + this._totalRatioHistory);
    }

    public float GetEaseRatio() {
        return this._totalRatio;
    }
}
