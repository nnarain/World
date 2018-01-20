using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAndPlace : MonoBehaviour
{
    public ChunkManager chunkManager;

    private int chunkSizeX;
    private int chunkSizeY;
    private int chunkSizeZ;

    // Use this for initialization
    void Start()
    {
        chunkSizeX = chunkManager.chunkPrefab.chunkSizeX;
        chunkSizeY = chunkManager.chunkPrefab.chunkSizeY;
        chunkSizeZ = chunkManager.chunkPrefab.chunkSizeZ;
    }

    // Update is called once per frame
    void Update()
    {
        bool pick = Input.GetMouseButtonDown(0);
        bool place = Input.GetMouseButtonDown(1);

        if (pick)
        {
            Pick();
        }

        if (place)
        {
            Place();
        }
    }

    private void Place()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            Debug.DrawLine(inputRay.origin, hit.point, Color.red);
        }
    }

    private void Pick()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            Debug.DrawLine(inputRay.origin, hit.point, Color.blue);
            GetBlockFromWorldPosition(hit.point);
        }
    }

    private Chunk GetChunkFromWorldPosition(Vector3 position)
    {
        return chunkManager.GetChunkFromWorldPosition(position);
    }

    private Voxel GetBlockFromWorldPosition(Vector3 position)
    {
        Chunk chunk = chunkManager.GetChunkFromWorldPosition(position);
        

        if (chunk != null)
        {
            //Debug.Log(string.Format("Chunk Position: {0}", chunk.transform.position));
            //int x = (int)position.x % chunkManager.chunkPrefab.chunkSizeX - ((position.x < 0) ? 1 : 0);
            //int y = (int)position.y % chunkManager.chunkPrefab.chunkSizeY - ((position.y < 0) ? 1 : 0);
            //int z = (int)position.z % chunkManager.chunkPrefab.chunkSizeZ - ((position.z < 0) ? 1 : 0);

            int px = (int)Mathf.Abs(position.x) % chunkSizeX;
            int py = (int)Mathf.Abs(position.y) % chunkSizeY;
            int pz = (int)Mathf.Abs(position.z) % chunkSizeZ;


            int x = (position.x >= 0) ? px : chunkSizeX - (px + 1);
            int y = (position.y >= 0) ? py : chunkSizeY - (py + 1);
            int z = (position.z >= 0) ? pz : chunkSizeZ - (pz + 1);



            //Debug.Log(string.Format("Block: {0}, {1}, {2}", x, y, z));
            Debug.Log(string.Format("p.z = {0}, z = {1}", position.z, z));

            chunk.Field.Set(x, y, z, 1);
            chunkManager.UpdateChunk(chunk);

            return chunk.GetField(x, y, z);
        }
        else
        {
            return new Voxel();
        }
    }
}
