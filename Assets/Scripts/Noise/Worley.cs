using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Worley : INoiseSampler
{
    public int seed;
    public double cellSize;

    private Dictionary<Vector2Int, Vector3[]> pointsCache;

    private readonly object cacheLock = new object();

    public Worley()
    {
        pointsCache = new Dictionary<Vector2Int, Vector3[]>();
    }

    public Vector3[] GetPoints(double x, double z)
    {
        // current grid position
        var gridX = (int)Math.Round(x / cellSize);
        var gridZ = (int)Math.Round(z / cellSize);

        var currentCell = new Vector2Int(gridX, gridZ);

        var hasCell = false;
        lock(cacheLock)
        {
            hasCell = pointsCache.ContainsKey(currentCell);
        }

        if (hasCell)
        {
            Vector3[] points = null;
            lock(cacheLock)
            {
                points = pointsCache[currentCell];
            }
            return points;
        }
        else
        {
            Vector3[] points = new Vector3[9];
            var c = 0;

            // loop for adjacent and current grid cells
            for (var i = gridX - 1; i <= gridX + 1; ++i)
            {
                for (var j = gridZ - 1; j <= gridZ + 1; ++j)
                {
                    // calculate a hash for the given grid cell
                    var hash = new Vector2Int(i, j).GetHashCode();
                    // seed a random generator with the hash
                    System.Random rnd = new System.Random(hash + seed);

                    // calculate the grid center
                    var cx = (double)i * cellSize;
                    var cz = (double)j * cellSize;

                    var px = cx + (rnd.NextDouble() * cellSize);
                    var pz = cz + (rnd.NextDouble() * cellSize);

                    points[c++] = new Vector3((float)px, 0, (float)pz);
                }
            }

            lock(cacheLock)
            {
                pointsCache[currentCell] = points;
            }

            return points;
        }
    }

    public Vector3[] GetPoints(Vector3 ws)
    {
        return GetPoints(ws.x, ws.z);
    }

    public Vector3 GetClosestPoint(Vector3 ws)
    {
        var points = GetPoints(ws);

        Vector3 closest = points[0];
        float minDistance = float.MaxValue;

        foreach (var p in points)
        {
            var distance = (ws - p).magnitude;
            if (distance < minDistance)
            {
                closest = p;
                minDistance = distance;
            }
        }

        return closest;
    }

    public double Sample(double x, double y, double z)
    {
        var maxDistance = cellSize;

        Vector3 ws = new Vector3((float)x, (float)y, (float)z);

        var closestPoint = GetClosestPoint(ws);
        
        return (ws - closestPoint).magnitude / maxDistance;
    }

    public double Sample(double x, double z)
    {
        return Sample(x, 0, z);
    }
}
