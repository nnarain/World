// The MIT License (MIT)
//
// Copyright (c) 2012-2013 Mikola Lysenko
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedyMesh
{
    public MeshData ReduceMesh(Chunk chunk)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> elements = new List<int>();
        List<Color> colors = new List<Color>();
        List<Vector2> uvs = new List<Vector2>();

        int[] dims = { chunk.chunkSizeX, chunk.chunkSizeY, chunk.chunkSizeZ };

        //Sweep over 3-axes
        for (int d = 0; d < 3; d++)
        {
            int i = 0;
            int j = 0;
            int k = 0;
            int l = 0;
            int w = 0;
            int h = 0;

            int u = (d + 1) % 3;
            int v = (d + 2) % 3;

            int[] x = { 0, 0, 0 };
            int[] q = { 0, 0, 0 };
            int[] mask = new int[(dims[u] + 1) * (dims[v] + 1)];


            q[d] = 1;

            for (x[d] = -1; x[d] < dims[d];)
            {
                // Compute the mask
                int n = 0;
                for (x[v] = 0; x[v] < dims[v]; ++x[v])
                {
                    for (x[u] = 0; x[u] < dims[u]; ++x[u], ++n)
                    {
                        int vox1 = (int)chunk.GetField(x[0], x[1], x[2]).Type;
                        int vox2 = (int)chunk.GetField(x[0] + q[0], x[1] + q[1], x[2] + q[2]).Type;

                        int a = (0 <= x[d] ? vox1 : 0);
                        int b = (x[d] < dims[d] - 1 ? vox2 : 0);

                        if ((a != 0) == (b != 0))
                        {
                            mask[n] = 0;
                        }
                        else if ((a !=0))
                        {
                            mask[n] = a;
                        }
                        else
                        {
                            mask[n] = -b;
                        }
                    }
                }

                // Increment x[d]
                ++x[d];

                // Generate mesh for mask using lexicographic ordering
                n = 0;
                for (j = 0; j < dims[v]; ++j)
                {
                    for (i = 0; i < dims[u];)
                    {
                        var c = mask[n];

                        if (c != 0)
                        {
                            // compute width
                            for (w = 1; mask[n + w] == c && (i + w) < dims[u]; ++w) { }

                            // compute height
                            bool done = false;
                            for (h = 1; (j + h) < dims[v]; ++h)
                            {
                                for (k = 0; k < w; ++k)
                                {
                                    if (mask[n + k + h * dims[u]] != c)
                                    {
                                        done = true;
                                        break;
                                    }
                                }
                                if (done)
                                {
                                    break;
                                }
                            }

                            // add quad
                            x[u] = i;
                            x[v] = j;

                            int[] du = { 0, 0, 0 };
                            int[] dv = { 0, 0, 0 };

                            bool bitset = c > 0;

                            if (c > 0)
                            {
                                dv[v] = h;
                                du[u] = w;
                            }
                            else
                            {
                                c = -c;
                                du[v] = h;
                                dv[u] = w;
                            }

                            Vector3 v1 = new Vector3(x[0], x[1], x[2]);
                            Vector3 v2 = new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]);
                            Vector3 v3 = new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]);
                            Vector3 v4 = new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]);

                            //AddQuad(v1, v2, v3, v4, chunk.GetVoxelColor((byte)c), vertices, elements, colors);
                            
                            Direction direction = GetDirection(d, bitset);
                            TextureAtlas.BlockFaces faces = chunk.GetFaces((byte)c);

                            var faceUVs = faces.GetUVs(direction);

                            if (direction == Direction.Far || direction == Direction.Left || direction == Direction.Bottom)
                            {
                                faceUVs = RotateUVs(faceUVs);
                            }

                            AddQuad(v1, v2, v3, v4, faceUVs, vertices, uvs, elements);

                            for (l = 0; l < h; ++l)
                            {
                                for (k = 0; k < w; ++k)
                                {
                                    mask[n + k + l * dims[u]] = 0;
                                }
                            }
                            // increment counters
                            i += w;
                            n += w;
                        }
                        else
                        {
                            ++i;
                            ++n;
                        }
                    }
                }
            }
        }

        MeshData data = new MeshData(vertices, elements, colors, uvs);

        return data;
    }

    private void AddQuad(
        Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, 
        Vector2[] uvs,
        List<Vector3> vertices, List<Vector2> textCoords, 
        List<int> elements)
    {
        int i = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);

        textCoords.AddRange(uvs);

        elements.Add(i + 0);
        elements.Add(i + 1);
        elements.Add(i + 2);
        elements.Add(i + 2);
        elements.Add(i + 3);
        elements.Add(i + 0);
    }

    private Vector2[] RotateUVs(Vector2[] uvs)
    {
        return new Vector2[] { uvs[3], uvs[0], uvs[1], uvs[2] };
    }

    private Direction GetDirection(int d, bool c)
    {
        switch(d)
        {
            case 0:
                return (c) ? Direction.Right : Direction.Left;
            case 1:
                return (c) ? Direction.Top : Direction.Bottom;
            case 2:
                return (c) ? Direction.Far : Direction.Near;
            default:
                // should be not here.
                return default(Direction);
        }
    }

    private void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color c,  List<Vector3> vertices, List<int> elements, List<Color> colors)
    {
        int i = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);

        colors.Add(c);
        colors.Add(c);
        colors.Add(c);
        colors.Add(c);

        elements.Add(i + 0);
        elements.Add(i + 1);
        elements.Add(i + 2);
        elements.Add(i + 2);
        elements.Add(i + 3);
        elements.Add(i + 0);
    }
}
