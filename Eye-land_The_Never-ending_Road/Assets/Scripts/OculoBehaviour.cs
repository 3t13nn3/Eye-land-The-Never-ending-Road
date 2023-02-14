using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class OculoBehaviour : MonoBehaviour
{
    public GameObject _RoadGenerator;
    public GameObject car;

    private List<GameObject> _trees;
    private List<GameObject> _breads;

    public int _FREQ_RECOVER_FIXATION = 10;
    public float _fixationTimer = 0f;
    List<Dictionary<int, int>> _fixations = new List<Dictionary<int, int>>();

    public float _FIXATION_VALUE = 0.250f;
    public int _fixationCount = 0;
    private float _lookTimeDisturb = 0f;
    private bool _isLookingDisturb = false;
    // Start is called before the first frame update
    
    private int rectWidth = 15;
    private int rectHeight = 10;
    private Rect fixationZone;
    private float fixationTimer = 0.0f;
    private int fixationCount = 0;

    void Start()
    {
        this.car = GameObject.Find("Car");

        this._trees = this._RoadGenerator.GetComponent<RoadGeneratorBehaviour>()._allTrees;
        this._trees = this._RoadGenerator.GetComponent<RoadGeneratorBehaviour>()._allBreads;

        for (int i = 0; i < _FREQ_RECOVER_FIXATION; i++)
        {
            this._fixations.Add(new Dictionary<int, int>());
        }

        // this.fixationZone = null;
    }

    // Update is called once per frame
    void Update()
    {   
        // We will replace that by the oculos record position
        Vector3 mousePos = Input.mousePosition;
        HandleFixationHistory();
        CheckIfLookAtDisturbing(new Vector2(mousePos.x, mousePos.y));
        // Debug.Log("Count of fixation in the last " + this._FREQ_RECOVER_FIXATION + " seconds : " + GetTheNumberOfFixationsInLastNSecond() + " on " + GetTheNumberOfObjectFixedInLastNSecond() + " objects.");
        
        if (car.GetComponent<CarMovingBehaviour>().gameStart())
            this.CheckJerks(mousePos);
    }

    private void CheckJerks(Vector2 eyePosition)
    {
        // if eye is into fixation zone
        if (this.fixationZone.Contains(eyePosition))
        {
            // Debug.Log("eye (" + eyePosition + ") into fixation zone, fixed time : " + this.fixationTimer);
            // increase fixation time
            this.fixationTimer += Time.deltaTime;
            // if fixation time is higher than FIXATION_VALUE then we count it as a fixation
            if (this.fixationTimer > this._FIXATION_VALUE)
            {
                this.fixationCount++;
                this.fixationTimer = 0.0f;
                // Debug.Log("add fixation count : " + this.fixationCount);
            }
        }
        // else create another fixation zone since user looks at another position
        else
        {
            this.fixationZone = new Rect(eyePosition.x - (this.rectWidth/2), eyePosition.y - (this.rectHeight/2), 
                this.rectWidth, this.rectHeight);
            // Debug.Log("eye (" + eyePosition + ") not into zone, create new zone : " + this.fixationZone);
        }
    }

    public int GetNbOfJerks()
    {
        int nbOfJerks = this.fixationCount;
        this.fixationTimer = 0.0f;
        this.fixationCount = 0;
        return nbOfJerks;
    }


    void CheckIfLookAtDisturbing(Vector2 _position) {
        Vector3 currentPos = new Vector3(_position.x, _position.y, Camera.main.nearClipPlane);

        Ray ray = Camera.main.ScreenPointToRay(currentPos);
        RaycastHit hit;
        if(Physics.Raycast( ray, out hit, 500)) {
            if(hit.transform.gameObject.tag == "disturb"){
                //Debug.Log(hit.transform.gameObject.name);
                FixationChecker(hit);
            } else {
                this._lookTimeDisturb = 0f;
            }
        } else {
            this._lookTimeDisturb = 0f;
        }
        
    }

    void FixationChecker(RaycastHit hit) {

        this._lookTimeDisturb += Time.deltaTime;
        if(this._lookTimeDisturb >= this._FIXATION_VALUE){
            ++this._fixationCount;
            //Debug.Log("Fixation " + this._fixationCount + " : "+ this._FIXATION_VALUE + " seconds (" + hit.transform.gameObject.name + ")");
            this._lookTimeDisturb = 0f;

            for (int i = 0; i < this._FREQ_RECOVER_FIXATION; i++)
            {
                if(this._fixations[i].ContainsKey(hit.transform.gameObject.GetInstanceID())){
                    ++this._fixations[i][hit.transform.gameObject.GetInstanceID()];
                } else {
                    this._fixations[i].Add(hit.transform.gameObject.GetInstanceID(), 1);
                }
            }
            // foreach(var kvp in this._fixations[0])
            // {
            //     Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
            // }
        }
        
    }

    void HandleFixationHistory() {
        this._fixationTimer += Time.deltaTime;
        if (this._fixationTimer >= 1f)
        {
            this._fixationTimer = 0f;
            if(!(this._fixations.Count < this._FREQ_RECOVER_FIXATION / 1f)) {
                this._fixations.RemoveAt(0);
            }
            this._fixations.Add(new Dictionary<int, int>());
        }
    }

    public int GetTheNumberOfFixationsInLastNSecond() {
        return this._fixations[0].Sum(x => x.Value);
    }

    public int GetTheNumberOfObjectFixedInLastNSecond() {
        return this._fixations[0].Count;
    }
}
