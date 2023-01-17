using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Random = UnityEngine.Random;

public class RoadGeneratorBehaviour : MonoBehaviour
{
    public enum Direction {
        SOUTH, WEST, NORTH, EAST
    }
    
    public Vector2 tileDimension = new Vector2(5.0f, 5.0f);
    
    private List<int> directionHistory = new List<int>();

    public GameObject tile;
    public List<GameObject> roadTiles = new List<GameObject>();

    // Based on previous tile. Generate a new tile to append and continue the road
    void GenerateNextTile() {

        // choosing direction
        float xOffset = roadTiles[roadTiles.Count - 1].transform.position.x;
        float zOffset = roadTiles[roadTiles.Count - 1].transform.position.z;

        int direction = 0;
        while(true) {
            direction = Random.Range(1, 4); 
            if (Math.Abs(direction - directionHistory[directionHistory.Count - 1]) != 2) {
            Debug.Log(direction);
                break;
            }
        }

        if (direction == (int)Direction.WEST) {
            xOffset -= tileDimension.x;
        } else if (direction == (int)Direction.NORTH) {
            zOffset += tileDimension.y;
        } else if (direction == (int)Direction.EAST) {
            xOffset += tileDimension.x;
        }

        directionHistory.Add(direction);
        tile.transform.position = new Vector3(xOffset, 0.5f, zOffset);
        tile.transform.localScale = new Vector3 (tileDimension.x, 0.1f, tileDimension.y);
        Instantiate(tile);
        roadTiles.Add(tile);
    }

    // Generate the road on a certain Tile, based on the previous one
    void GenerateRoad(int indexOfTile) {

    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            tile.transform.position = new Vector3(0f, 0.5f, tileDimension.y * (i));
            tile.transform.localScale = new Vector3 (tileDimension.x, 0.1f, tileDimension.y);
            Instantiate(tile);
            roadTiles.Add(tile);
            directionHistory.Add(2);
        }
        for (int i = 0; i < 500; i++)
        {
           GenerateNextTile(); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        // handle when whe leave a tile: we generate a new one        
    }
}
