using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class OculoBehaviour : MonoBehaviour
{
    public GameObject _RoadGenerator;

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
    void Start()
    {
        this._trees = this._RoadGenerator.GetComponent<RoadGeneratorBehaviour>()._allTrees;
        this._trees = this._RoadGenerator.GetComponent<RoadGeneratorBehaviour>()._allBreads;

        for (int i = 0; i < _FREQ_RECOVER_FIXATION; i++)
        {
            this._fixations.Add(new Dictionary<int, int>());
        }
    }

    // Update is called once per frame
    void Update()
    {   
        // We will replace that by the oculos record position
        Vector3 mousePos = Input.mousePosition;
        HandleFixationHistory();
        CheckIfLookAtDisturbing(new Vector2(mousePos.x, mousePos.y));
        Debug.Log("Count of fixation in the last " + this._FREQ_RECOVER_FIXATION + " seconds : " + GetTheNumberOfFixationsInLastNSecond() + " on " + GetTheNumberOfObjectFixedInLastNSecond() + " objects.");
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
