using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MeshDataAllocator
{
    private List<MeshData> meshData;

    public MeshDataAllocator()
    {
        meshData = new List<MeshData>();
    }

    public MeshData[] Get()
    {
        return meshData.ToArray();
    }

    /// <summary>
    /// Get the mesh data for mesh `i`
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public MeshData Get(int i)
    {
        if (i < meshData.Count)
        {
            return meshData[i];
        }
        else
        {
            return CreateNewMeshData(i + 1);
        }
    }

    private MeshData CreateNewMeshData(int i)
    {
        int diff = i - meshData.Count;

        for (int j = 0; j < diff; ++j)
        {
            meshData.Add(new MeshData());
        }

        return meshData[i - 1];
    }
}
