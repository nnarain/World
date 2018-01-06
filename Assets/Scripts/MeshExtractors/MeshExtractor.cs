using System;
using System.Collections.Generic;
using UnityEngine;

public interface MeshExtractor
{
    void Extract(Chunk chunk, Action<MeshData> callback);
}
