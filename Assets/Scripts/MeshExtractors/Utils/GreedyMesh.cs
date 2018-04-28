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
    public MeshData[] ReduceMesh(Chunk chunk)
    {
        MeshDataAllocator meshAllocator = new MeshDataAllocator();

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
            int size = (dims[u] + 1) * (dims[v] + 1);
            int[] mask = new int[size];


            q[d] = 1;

            for (x[d] = -1; x[d] < dims[d];)
            {
                // Compute the mask
                int n = 0;
                for (x[v] = 0; x[v] < dims[v]; ++x[v])
                {
                    for (x[u] = 0; x[u] < dims[u]; ++x[u], ++n)
                    {
                        var v1 = chunk.GetField(x[0], x[1], x[2]);
                        var v2 = chunk.GetField(x[0] + q[0], x[1] + q[1], x[2] + q[2]);

                        int t1 = (int)v1.Type;
                        int t2 = (int)v2.Type;

                        int a = (0 <= x[d] ? t1 : 0);
                        int b = (x[d] < dims[d] - 1 ? t2 : 0);

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
                            

                            //Direction direction = GetDirection(d, bitset);
                            // TODO: this should be replaced with block properties. 
                            //TextureAtlas.BlockFaces faces = chunk.GetFaces((byte)c);
                            int meshIndex = Blocks.BlockMeshProperty[c];

                            //var faceUVs = faces.GetUVs(direction);

                            //meshAllocator.Get(meshIndex).AddQuad(v1, v2, v3, v4, faceUVs[3]);
                            meshAllocator.Get(meshIndex).AddQuad(v1, v2, v3, v4, chunk.GetVoxelColor((byte)c));

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

        return meshAllocator.Get();
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
}
