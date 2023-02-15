using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

public class MLModelEvaluator : MonoBehaviour
{
    private string modelPath;
    private string scriptPath;
    // Start is called before the first frame update
    
    void Start()
    {
        this.scriptPath = Application.dataPath + "/Resources/predict.py";
        this.modelPath = Application.dataPath + "/Resources/svm.pkl";
    }

    // Update is called once per frame
    void Update()
    {
    }

    public int make_prediction(int distance, int nbJerks, int nbLookDisruptive, float OnRoadRate, float meanSpeed)
    {
        var engine = Python.CreateEngine();
        var scope = engine.CreateScope();

        engine.GetSearchPaths().Add(Application.dataPath + "/Resources");
        engine.ExecuteFile(this.scriptPath, scope);

        var predictFunc = scope.GetVariable("make_prediction");
        var result = predictFunc(this.modelPath, distance, nbJerks, nbLookDisruptive, OnRoadRate, meanSpeed);

        return result;
    }
}
