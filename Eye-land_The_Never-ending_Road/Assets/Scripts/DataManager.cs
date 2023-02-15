using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public GameObject car;
    [SerializeField] GameObject rg;
    [SerializeField] GameObject oc;
    private int traveledDistance;
    private float timer;
    private int elapsedTime;
    private bool save;
    private string fileName;
    private string filePath;
    private string csvHeader;

    // Start is called before the first frame update
    void Start()
    {
        this.car = GameObject.Find("Car");
        this.traveledDistance = 0;
        this.timer = 0.0f;
        this.elapsedTime = 0;
        this.save = false;
        Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + "/Data/");
        //Debug.Log(System.IO.Directory.GetCurrentDirectory());
        this.fileName = "gameData-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv";
        this.filePath = System.IO.Directory.GetCurrentDirectory() + "/Data/" + fileName;
        this.csvHeader = "distance, nbOfJerks, nbLookDisruptive, OnRoadRate, meanSpeed";
    }

    // Update is called once per frame
    void Update()
    {
        if (car.GetComponent<CarMovingBehaviour>().gameStart())
        {
            this.timer += Time.deltaTime;

            // count seconds by seconds
            if ((int)(timer % 60) > this.elapsedTime)
            {
                // every 10 seconds we save a line of data into a csv file of current game
                if (this.elapsedTime > 0 && this.elapsedTime%10 == 0)
                {
                    int distance = rg.GetComponent<RoadGeneratorBehaviour>().GetPlayerDistance() - this.traveledDistance;
                    this.traveledDistance = rg.GetComponent<RoadGeneratorBehaviour>().GetPlayerDistance();
                    int nbOfJerks = oc.GetComponent<OculoBehaviour>().GetNbOfJerks();
                    int nbLookDisruptive = oc.GetComponent<OculoBehaviour>().GetTheNumberOfFixationsInLastNSecond();
                    float meanOnRoadRate = car.GetComponent<CarMovingBehaviour>().GetOnOffRoadRatio();
                    float meanSpeed = car.GetComponent<CarMovingBehaviour>().GetMeanSpeed();
                    string data = distance.ToString() + ", " + nbOfJerks.ToString() + ", " + nbLookDisruptive.ToString() + ", "
                    + meanOnRoadRate.ToString() + ", " + meanSpeed.ToString();
                    // Debug.Log(this.elapsedTime + " : " + data);
                    this.writeToCsv(data);
                }
            }

            this.elapsedTime = (int)(timer % 60);
        }
    }

    private void writeToCsv(string data)
    {
        if (!File.Exists(filePath))
        {
            using (StreamWriter streamWriter = File.CreateText(filePath))
                streamWriter.WriteLine(this.csvHeader);
        }
        using (StreamWriter streamWriter = File.AppendText(filePath))
            streamWriter.WriteLine(data);
    }
}
