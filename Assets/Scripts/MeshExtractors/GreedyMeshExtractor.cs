using System;
using System.Collections.Generic;
using UnityEngine;

public class GreedyMeshExtractor : IMeshExtractor
{
    void IMeshExtractor.Extract(Chunk chunk, Action<MeshData> callback)
    {
        GreedyMesh greedy = new GreedyMesh();

        MeshData data = greedy.ReduceMesh(chunk);
        callback(data);
    }
}
