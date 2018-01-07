using System;
using System.Collections.Generic;
using UnityEngine;

public class GreedyMeshExtractor : MeshExtractor
{
    void MeshExtractor.Extract(Chunk chunk, Action<MeshData> callback)
    {
        GreedyMesh greedy = new GreedyMesh();

        MeshData data = greedy.ReduceMesh(chunk);
        callback(data);
    }
}
