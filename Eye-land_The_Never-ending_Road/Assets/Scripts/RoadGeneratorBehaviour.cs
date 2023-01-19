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
    
    public Vector2 _tileDimension = new Vector2(30.0f, 30.0f);

    public GameObject _tile;
    private List<GameObject> _roadTiles = new List<GameObject>();
    private List<int> _directionHistory = new List<int>();

    private List<QuadraticBezierCurve> _allCurves = new List<QuadraticBezierCurve>();
    public Vector3[] _allPoints;

    // Based on previous tile. Generate a new tile to append and continue the road
    void GenerateNextTile() {

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
        this._tile.transform.localScale = new Vector3 (this._tileDimension.x, 0.1f, this._tileDimension.y);
        Instantiate(this._tile);
        this._roadTiles.Add(this._tile);
    }

    // Generate the road on a certain Tile, based on the previous one
    void GenerateRoad(int indexOfTile) {
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
        c.GenerateBezierRoad();
    }

    // Start is called before the first frame update
    void Start()
    {   
        
        // First tile is always the same, straight
        {
            this._tile.transform.position = new Vector3(0f, 0.5f, 0f);
            this._tile.transform.localScale = new Vector3 (this._tileDimension.x, 0.1f, this._tileDimension.y);
            Instantiate(this._tile);
            this._roadTiles.Add(this._tile);
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
            c.GenerateBezierRoad();
        }

        for (int i = 1; i < 100; i++)
        {
            GenerateNextTile();
            GenerateRoad(i);
        }

        /*
        // Generating all the road as one object
        QuadraticBezierCurve ccc = gameObject.AddComponent<QuadraticBezierCurve>();
        ccc.MergeAllCurves(this._allCurves);
        */
    }



    // Update is called once per frame
    void Update()
    {
        // handle when whe leave a tile: we generate a new one        
    }
}
