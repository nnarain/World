using System;
using System.Collections.Generic;
using UnityEngine;

public interface IMeshExtractor
{
    void Extract(Chunk chunk, Action<MeshData[]> callback);
}
