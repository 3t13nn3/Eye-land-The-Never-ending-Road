using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComputeEase : MonoBehaviour
{
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
        for (int i = 0; i < 100; i++)
        {
            this._totalRatioHistory.Add(0.25f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        PreComputeAllRatios();
        ComputeAllRatios();
        // Debug.Log("Ratio of On Road " + this._onRoadRatio);
        // Debug.Log("Ratio of Speed " + this._averageSpeedRatio);
        // Debug.Log("Ratio of Distance " + this._traveledDistanceRatio);
        // Debug.Log("Ratio of Jerks " + this._jerksRatio);
        // Debug.Log("Ratio of Disturb " + this._disturbRatio);
        // Debug.Log("Ratio of Pupil Size " + this._pupilSizeRatio);
        // Debug.Log("EASE RATIO: " + this._totalRatioHistory);
    }

    public float GetEaseRatio() {
        return this._totalRatio;
    }
}
