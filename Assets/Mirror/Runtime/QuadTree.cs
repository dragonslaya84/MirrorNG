using System;
using UnityEngine;

namespace Mirror
{
    public struct BoundingBox
    {
        public BoundingBox(Vector3 centerPos, float halfWidth, float halfHeight)
        {
            center = centerPos;
            halfLength = halfHeight;
            this.halfWidth = halfWidth;
        }

        public Vector3 center;
        public float halfWidth;
        public float halfLength;
    }

    public struct Quad
    {
        public QuadNode[] Nodes;
        public BoundingBox rect;

        public Quad(BoundingBox rect)
        {
            this.rect = rect;
            Nodes = new QuadNode[ushort.MaxValue];
        }
    }

    /// <summary>
    ///     Node that is part of <see cref="Quad"/>. Can be a leaf on its own or have branch children of <see cref="QuadNode"/>
    /// </summary>
    public readonly struct QuadNode
    {
        private readonly QuadNode[] _children;

        /// <summary>
        ///     Setup the internal children of the <see cref="QuadNode"/>
        /// <param name="children">NOT USED! Hack to bypass struct constuctionless whining.</param>
        /// </summary>
        public QuadNode(int children = 4)
        {
            _children = new QuadNode[4];
        }

        /// <summary>
        ///     Whether or not this node has children branches or standalone leaf
        /// </summary>
        public bool IsLeaf => _children.Length == 0;

        /// <summary>
        ///     The length of our children nodes.
        /// </summary>
        public int Length
        {
            get
            {
                int length = 0;

                for (int index = 0; index < _children.Length; index++)
                {
                    QuadNode item = _children[index];

                    if (!item.Equals(default(QuadNode)))
                        length++;
                }

                return length;
            }
        }

        /// <summary>
        ///     Get or set the index of a children
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public QuadNode this[int index]
        {
            get
            {
                if (index < 0 || index > 3)
                    throw new IndexOutOfRangeException();

                return _children[index];
            }
            set
            {
                _children[index] = value;
            }
        }
    }
}
