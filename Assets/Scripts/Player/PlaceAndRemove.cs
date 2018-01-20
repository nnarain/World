using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceAndRemove : MonoBehaviour
{
    public ChunkManager chunkManager;

    private int chunkSizeX;
    private int chunkSizeY;
    private int chunkSizeZ;

    private const float HALF_BLOCK_WIDTH = CubeMetrics.CUBE_SIZE / 2f;

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
        bool placeBlock = Input.GetMouseButtonDown(0);
        bool removeBlock = Input.GetMouseButtonDown(1);

        if (placeBlock)
        {
            PlaceBlock();
        }

        if (removeBlock)
        {
            RemoveBlock();
        }
    }

    private void RemoveBlock()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            Debug.DrawLine(inputRay.origin, hit.point, Color.red);
            
        }
    }

    private void PlaceBlock()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            //Debug.DrawLine(inputRay.origin, hit.point, Color.blue);
            SetBlockType(RaycastHitToBlockWorldPosition(hit), 1);
        }
    }

    private void SetBlockType(Vector3 position, byte type)
    {
        Chunk chunk = chunkManager.GetChunkFromWorldPosition(position);

        if (chunk != null)
        {
            var bp = BlockPosition(position);

            chunk.Field.Set(bp.x, bp.y, bp.z, type);
            chunkManager.UpdateChunk(chunk);
        }
    }

    private void RemoveBlock(Vector3 position)
    {

    }

    private Vector3 RaycastHitToBlockWorldPosition(RaycastHit hit)
    {
        // Take the hit point and add the normal vector of the surface scale to a half block width
        return hit.point + (hit.normal * HALF_BLOCK_WIDTH);
    }

    private Vector3Int BlockPosition(Vector3 position)
    {
        int px = (int)Mathf.Abs(position.x) % chunkSizeX;
        int py = (int)Mathf.Abs(position.y) % chunkSizeY;
        int pz = (int)Mathf.Abs(position.z) % chunkSizeZ;


        int x = (position.x >= 0) ? px : chunkSizeX - (px + 1);
        int y = (position.y >= 0) ? py : chunkSizeY - (py + 1);
        int z = (position.z >= 0) ? pz : chunkSizeZ - (pz + 1);

        return new Vector3Int(x, y, z);
    }
}
