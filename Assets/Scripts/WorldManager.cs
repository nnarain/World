using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Chunk chunkPrefab;

    // Use this for initialization
    void Start()
    {
        int s = 15;
        float r = 5f;
        

        Grid grid = new Grid(s, s, s);

        Vector3 center = new Vector3((float)s / 2f, (float)s / 2f, (float)s / 2f);

        for (int x = 0; x < s; ++x)
        {
            for (int y = 0; y < s; ++y)
            {
                for (int z = 0; z < s; ++z)
                {
                    Vector3 p = new Vector3(x, y, z);
                    if ((p - center).magnitude <= r)
                    {
                        grid.Set(x, y, z, 1);
                    }
                }
            }
        }

        Chunk chunk = Instantiate<Chunk>(chunkPrefab);
        chunk.transform.SetParent(transform, false);

        chunk.Build(grid);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
