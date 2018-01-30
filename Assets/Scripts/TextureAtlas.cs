using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAtlas : MonoBehaviour
{
    [System.Serializable]
    public class BlockFaces
    {
        public string name;
        public Sprite leftFace;
        public Sprite rightFace;
        public Sprite topFace;
        public Sprite bottomFace;
        public Sprite nearFace;
        public Sprite farFace;

        private Vector2[] leftUVs;
        private Vector2[] rightUVs;
        private Vector2[] topUVs;
        private Vector2[] bottomUVs;
        private Vector2[] nearUVs;
        private Vector2[] farUVs;


        public Vector2[] GetUVs(Direction d)
        {
            switch(d)
            {
                case Direction.Left:
                    return leftUVs;
                case Direction.Right:
                    return rightUVs;
                case Direction.Top:
                    return topUVs;
                case Direction.Bottom:
                    return bottomUVs;
                case Direction.Near:
                    return nearUVs;
                case Direction.Far:
                    return farUVs;
                default:
                    return null;
            }
        }

        public void Init()
        {
            leftUVs = leftFace.uv;
            rightUVs = rightFace.uv;
            topUVs = topFace.uv;
            bottomUVs = bottomFace.uv;
            nearUVs = nearFace.uv;
            farUVs = farFace.uv;

            ReorderUVs(leftUVs);
            ReorderUVs(rightUVs);
            ReorderUVs(topUVs);
            ReorderUVs(bottomUVs);
            ReorderUVs(nearUVs);
            ReorderUVs(farUVs);
        }


        /// <summary>
        /// </summary>
        private void ReorderUVs(Vector2[] uvs)
        {
            var tl = uvs[0];
            var tr = uvs[1];
            var bl = uvs[2];
            var br = uvs[3];

            uvs[0] = bl;
            uvs[1] = tl;
            uvs[2] = tr;
            uvs[3] = br;
        }
    }

    public BlockFaces[] blockFaces;

    /// <summary>
    /// Pull the UVs out of the sprites before they are actually needed 
    /// </summary>
    public void Init()
    {
        foreach (var b in blockFaces)
        {
            b.Init();
        }
    }

    public BlockFaces GetBlockFaces(Blocks.Type t)
    {
        // TODO: remove this check
        int i = (int)t.ToByte();
        if (i >= blockFaces.Length)
            i = blockFaces.Length - 1;

        return blockFaces[i];
    }
}
