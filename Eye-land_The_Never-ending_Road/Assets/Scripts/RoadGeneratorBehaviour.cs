using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using System;
using Random = UnityEngine.Random;

public class RoadGeneratorBehaviour : MonoBehaviour
{
    private enum Direction {
        SOUTH, WEST, NORTH, EAST
    }
    public int _totalNumberOfTiles = 100;
    public Vector2 _tileDimension = new Vector2(30.0f, 30.0f);
    public float _tileThickness = 0.1f;
    public float _maxHeightOfTile = 0.0f;

    public GameObject _tile;
    private List<GameObject> _roadTiles = new List<GameObject>();
    private List<int> _directionHistory = new List<int>();

    private List<QuadraticBezierCurve> _allCurves = new List<QuadraticBezierCurve>();
    private List<GameObject> _allCurvesGO = new List<GameObject>();
    public List<List<GameObject>> _allEnv = new List<List<GameObject>>();
    public List<List<Vector3>> _allEnvPos = new List<List<Vector3>>();
    public int _randValueToSpawnEnv = 60;

    private List<float> _startTime = new List<float>();

    // Based on previous tile. Generate a new tile to append and continue the road
    private void GenerateNextTile() {

        float xOffset = this._roadTiles[this._roadTiles.Count - 1].transform.position.x;
        float zOffset = this._roadTiles[this._roadTiles.Count - 1].transform.position.z;

        int direction = this._directionHistory[this._directionHistory.Count - 1];

        if (direction == (int)Direction.WEST) {
            xOffset -= this._tileDimension.x;
        } else if (direction == (int)Direction.NORTH) {
            zOffset += this._tileDimension.y;
        } else if (direction == (int)Direction.EAST) {
            xOffset += this._tileDimension.x;
        }

        // choosing direction for next tile

        int nextDirection = 0;
        while(true) {
            nextDirection = Random.Range(1, 4); 
            if (Math.Abs(nextDirection - this._directionHistory[this._directionHistory.Count - 1]) != (int)Direction.NORTH) {
                // Debug.Log(direction);
                break;
            }
        }
        this._directionHistory.Add(nextDirection);
        this._tile.transform.position = new Vector3(xOffset, 0.5f, zOffset);
        this._tile.transform.localScale = new Vector3 (this._tileDimension.x, this._tileThickness, this._tileDimension.y);
        GameObject instance = Instantiate(this._tile);
        this._roadTiles.Add(instance);
    }

    // Generate the road on a certain Tile, based on the previous one
    private void GenerateRoad(int indexOfTile) {
        Vector3[] points = new Vector3[4];

        GameObject currentTile = this._roadTiles[indexOfTile];

        points[0] = this._allCurves[this._allCurves.Count - 1]._points[3];
        

        for (int i = 1; i < 4; i++)
        {
            points[i] = new Vector3(
                currentTile.transform.position.x + Random.Range(-(this._tileDimension.x/3), (this._tileDimension.x/3)),
                0.555f, 
                currentTile.transform.position.z + Random.Range(-(this._tileDimension.y/3), (this._tileDimension.y/3))
            ); 
        }

        // Not ending proprely
        {
            int direction = this._directionHistory[indexOfTile];
            if (direction == (int)Direction.WEST) {
                points[3].x = this._roadTiles[indexOfTile].transform.position.x - this._tileDimension.x/2;
            } else if (direction == (int)Direction.NORTH) {
                points[3].z = this._roadTiles[indexOfTile].transform.position.z + this._tileDimension.y/2;
            } else if (direction == (int)Direction.EAST) {
                points[3].x = this._roadTiles[indexOfTile].transform.position.x + this._tileDimension.x/2;
            }
        }
        
        // From the article: Force first control point to be aligned with the last one
        Vector3 t = (this._allCurves[this._allCurves.Count - 1]._points[2] - points[0]);
        // Scaling to avoid exiting tiles
        t.x /= 5;
        t.z /= 5;
        // Then applying the transformation
        points[1] = points[0] - t;

        QuadraticBezierCurve c = gameObject.AddComponent<QuadraticBezierCurve>();
        c.SetPoints(points);
        this._allCurves.Add(c);
        this._allCurvesGO.Add(c.GenerateBezierRoad());
    }

    private void ProcessEnvTils(bool nonOverlap, float x, float z, int nbTiles, int offsetDirection, List<GameObject> envGO, List<Vector3> envPos) {
        if(nonOverlap) {
            for (int i = 1; i < nbTiles; i++) {
                float r = (float)Random.Range(1, 100)/100;
                float g = (float)Random.Range(1, 100)/200;
                float b = (float)Random.Range(1, 100)/100;
                // height
                float rand = (float)Random.Range(1, this._maxHeightOfTile);

                envPos.Add(new Vector3(x + offsetDirection * this._tileDimension.x * i, 0.5f + (this._tileThickness * rand) / 2, this._tile.transform.position.z));
                // Handle nonOverlap between env tiles movement
                int xRand = Random.Range(i* ((offsetDirection * this._randValueToSpawnEnv)/nbTiles), (i+1)* ((offsetDirection * this._randValueToSpawnEnv)/nbTiles));
                int yRand = Random.Range(-this._randValueToSpawnEnv, this._randValueToSpawnEnv);
                this._tile.transform.position = new Vector3(
                    (Math.Sign(xRand) * this._randValueToSpawnEnv / 2) + xRand + x + offsetDirection * this._tileDimension.x * i,
                    (Math.Sign(yRand) * this._randValueToSpawnEnv) + yRand + 0.5f + (this._tileThickness * rand) / 2,
                    this._tile.transform.position.z
                );
                this._tile.transform.localScale = new Vector3 (this._tileDimension.x, this._tileThickness * rand, this._tileDimension.y);
                GameObject instance = Instantiate(this._tile);
                instance.GetComponent<Renderer>().material.color = new Color(r, g, b);
                envGO.Add(instance);

                // Effet sympa
                if (Random.Range(1, 5) == 1)
                {
                    // Handle nonOverlap between env tiles movement
                    xRand = Random.Range(i* ((offsetDirection * this._randValueToSpawnEnv)/nbTiles), (i+1)* ((offsetDirection * this._randValueToSpawnEnv)/nbTiles));
                    yRand = Random.Range(0, this._randValueToSpawnEnv);
                    envPos.Add(new Vector3(x + offsetDirection * this._tileDimension.x * i, 0.5f + rand, this._tile.transform.position.z));
                    this._tile.transform.position = new Vector3(
                        (Math.Sign(xRand) * this._randValueToSpawnEnv / 2) + xRand + x + offsetDirection * this._tileDimension.x * i,
                        this._randValueToSpawnEnv + yRand + 0.5f + rand,
                        this._tile.transform.position.z
                    );
                    GameObject instanceDecor = Instantiate(this._tile);
                    instanceDecor.GetComponent<Renderer>().material.color = new Color(r, g, b);
                    envGO.Add(instanceDecor);
                }

            }
        }
    }

    void GenerateEnv(int nbTiles) {
        this._tile.transform.position = this._roadTiles[this._roadTiles.Count - nbTiles].transform.position;

        for (int k = this._roadTiles.Count - nbTiles; k < this._directionHistory.Count(); k++) {
            
            float x = this._tile.transform.position.x;
            float z = this._tile.transform.position.z;

            List<GameObject> envGO = new List<GameObject>();
            List<Vector3> envPos = new List<Vector3>();
            if(k == 0){
                ProcessEnvTils(true, x, z, 10, 1, envGO, envPos);
                ProcessEnvTils(true, x, z, 10, -1, envGO, envPos);    
            } else {
                bool nonOverlap = (k > 0 && k < this._directionHistory.Count() - 1 &&
                                this._directionHistory[k] != (int)Direction.EAST &&
                                this._directionHistory[k-1] != (int)Direction.WEST);
            
                ProcessEnvTils(nonOverlap, x, z, 10, 1, envGO, envPos);

                // width
                nonOverlap = (k > 0 && k < this._directionHistory.Count() - 1 &&
                                this._directionHistory[k] != (int)Direction.WEST &&
                                this._directionHistory[k-1] != (int)Direction.EAST);
                ProcessEnvTils(nonOverlap, x, z, 10, -1, envGO, envPos);     
            }
                       
            
            this._allEnv.Add(envGO);
            this._allEnvPos.Add(envPos);
            this._startTime.Add(Time.time);

            if (this._directionHistory[k] == (int)Direction.WEST) {
                x -= this._tileDimension.x;
            } else if (this._directionHistory[k] == (int)Direction.NORTH) {
                z += this._tileDimension.y;
            } else if (this._directionHistory[k] == (int)Direction.EAST) {
                x += this._tileDimension.x;
            }
            this._tile.transform.position = new Vector3(x, 0.5f, z);
        }
    }

    // Start is called before the first frame update
    void Start()
    {   

        // First tile is always the same, straight
        {
            this._tile.transform.position = new Vector3(0f, 0.5f, 0f);
            this._tile.transform.localScale = new Vector3 (this._tileDimension.x, this._tileThickness, this._tileDimension.y);
            GameObject instance = Instantiate(this._tile);
            this._roadTiles.Add(instance);
            this._directionHistory.Add((int)Direction.NORTH);

            QuadraticBezierCurve c = gameObject.AddComponent<QuadraticBezierCurve>();
            c.SetPoints(new Vector3[]{
                new Vector3(0f, 0.555f, -this._tileDimension.y/2),
                new Vector3(0f, 0.555f, 0f),
                new Vector3(0f, 0.555f, 0f),
                new Vector3(0f, 0.555f, this._tileDimension.y/2),
                }
            );
            this._allCurves.Add(c);
            this._allCurvesGO.Add(c.GenerateBezierRoad());
        }
        

        for (int i = 1; i < this._totalNumberOfTiles; i++)
        {
            GenerateNextTile();
            GenerateRoad(i);
        }

        GenerateEnv(this._totalNumberOfTiles);
        /*
        // Generating all the road as one object
        QuadraticBezierCurve ccc = gameObject.AddComponent<QuadraticBezierCurve>();
        ccc.MergeAllCurves(this._allCurves);
        */
    }



    // Update is called once per frame
    void Update()
    {   
        GameObject car = GameObject.Find("Car");
        Vector3 pos = car.transform.position;
        //car.transform.position = new Vector3(car.transform.position.x, car.transform.position.y, car.transform.position.z + 0.3f);
        if (this._roadTiles.Count > 0) {
            //float dist = Vector2.Distance(new Vector2(this._roadTiles[0].transform.position.x, this._roadTiles[0].transform.position.z), new Vector2(pos.x, pos.z));
            float distLast = Math.Abs(this._roadTiles[this._roadTiles.Count - 1].transform.position.z - pos.z);
            float distFirst = Math.Abs(this._roadTiles[0].transform.position.z - pos.z);
            // Debug.Log(distLast);
            if(distLast < this._tileDimension.y * 8) {
                
                if(distFirst > this._tileDimension.y * 5){
                    GameObject.Destroy(this._roadTiles[0]);
                    this._roadTiles.RemoveAt(0);
                    this._directionHistory.RemoveAt(0);
                    this._allCurves[0]._mesh.Clear();
                    GameObject.Destroy(this._allCurvesGO[0]);
                    GameObject.Destroy(this._allCurves[0]);
                    this._allCurves.RemoveAt(0);
                    this._allCurvesGO.RemoveAt(0);

                    foreach (var e in this._allEnv[0]) {
                        GameObject.DestroyImmediate(e);
                    }
                    this._allEnv.RemoveAt(0);
                    this._allEnvPos.RemoveAt(0);
                    this._startTime.RemoveAt(0);

                }
                GenerateNextTile();
                GenerateRoad(this._allCurves.Count - 1);
                GenerateEnv(2);

                // Increase size of wall by the time
                this._maxHeightOfTile += 3;
            }


        }
        
        
        // handle when whe leave a tile: we generate a new one
                
        float currentTime = Time.time;

        for (int i = 0; i < this._allEnv.Count; i++)
        {
            float elapsedTime = currentTime - this._startTime[i];
            float t = elapsedTime / this._randValueToSpawnEnv / 2;
            //t = Mathf.Clamp01(t);
            for (int j = 0; j < this._allEnv[i].Count; j++)
            {   
                this._allEnv[i][j].transform.position = Vector3.Lerp(this._allEnv[i][j].transform.position, this._allEnvPos[i][j], t);
            }
        }
        
    }
}
