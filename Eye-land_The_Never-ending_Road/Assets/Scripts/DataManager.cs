using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public GameObject car;
    public GameObject roadGenerator;
    private List<GameObject> _roadTiles;

    // Start is called before the first frame update
    void Start()
    {
        car = GameObject.Find("Car");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
