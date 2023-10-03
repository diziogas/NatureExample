using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FavouriteScript : MonoBehaviour
{


    public int counter = 0;
    private List<TileIndex> SearchedTiles = new List<TileIndex>();
    public List<List<TileIndex>> WaterBodies = new List<List<TileIndex>>();
    public int[,] waterBodiesMap = new int[128, 128];
    public int index = 1;
    public void SeparateWaterBodies()
    {
        for (int i = 0; i < City.Size; i++)
        {
            for (int j = 0; j < City.Size; j++)
            {
                if (waterMap[i, j] == 1 && !IsTileInAnyWaterBody(i, j))
                {

                    SearchedTiles.Clear();
                    FindConnectedNodes(new TileIndex(i, j));
                    Debug.Log("number of tiles of waterbody" + index + "is:" + counter);
                    if (SearchedTiles.Count > 0)
                    {
                        WaterBodies.Add(new List<TileIndex>(SearchedTiles));
                        index++;
                    }
                }
            }
        }

    }

    // recursively find all the water tiles of a waterbody
    private void FindConnectedNodes(TileIndex tile)
    {
        counter++;
        if (tile.X < 0 || tile.X >= City.Size || tile.Y < 0 || tile.Y >= City.Size) return;
        SearchedTiles.Add(tile);
        waterBodiesMap[tile.X, tile.Y] = index;
        if (waterMap[tile.X, tile.Y] == 0) return;
        // using many ifs in order to avoid too many recursive calls
        if (!SearchedTiles.Contains(new TileIndex(tile.X + 1, tile.Y)))
        {
            FindConnectedNodes(new TileIndex(tile.X + 1, tile.Y));
        }
        if (!SearchedTiles.Contains(new TileIndex(tile.X, tile.Y + 1)))
        {
            FindConnectedNodes(new TileIndex(tile.X, tile.Y + 1));
        }
        if (!SearchedTiles.Contains(new TileIndex(tile.X - 1, tile.Y)))
        {
            FindConnectedNodes(new TileIndex(tile.X - 1, tile.Y));
        }
        if (!SearchedTiles.Contains(new TileIndex(tile.X, tile.Y - 1)))
        {
            FindConnectedNodes(new TileIndex(tile.X, tile.Y - 1));
        }
        return;
    }




    private bool IsTileInAnyWaterBody(int x, int y)
        {
            foreach (List<TileIndex> waterBody in WaterBodies)
            {
                if (waterBody.Any(tile => tile.X == x && tile.Y == y))
                {
                    return true;
                }
            }
            return false;
     }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
