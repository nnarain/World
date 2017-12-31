using System;
using System.Collections.Generic;
using UnityEngine;

public interface MeshExtractor
{
    void Extract(Field field, Action<MeshData> callback);
}
