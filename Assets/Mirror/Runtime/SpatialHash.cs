using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror
{
    public class SpatialHash<T>
    {
        private HashSet<int> _positionHashes;
        private Dictionary<int, List<T>> _objects;
        private int _hashSize;

        public SpatialHash(int hashSize)
        {
            _hashSize = hashSize;

            _positionHashes = new HashSet<int>();
            _objects = new Dictionary<int, List<T>>();
        }

        /// <summary>
        ///     Current size of 
        /// </summary>
        public int Count { get { return _positionHashes.Count; } }

        public void UpdateOrInsert(Vector3 position, T obj)
        {
            // Calculate the position hash to check against.
            int key = CalculateHash(position);

            if (_positionHashes.Contains(key))
            {
                _objects[key].Remove(obj);
            }

            if (_objects.ContainsKey(key))
            {
                _objects[key].Add(obj);
            }
            else
            {
                _objects[key] = new List<T> { obj };
            }
        }

        public List<T> Query(Vector3 v)
        {
            int key = CalculateHash(v);

            return _positionHashes.Contains(key) ? _objects[key] : new List<T>();
        }

        /// <summary>
        ///     Calculates a hash to be stored in our list for fast tracking.
        /// </summary>
        /// <param name="position">The position to use for calculating random hash</param>
        /// <returns></returns>
        private int CalculateHash(Vector3 position)
        {
            return ((int)Mathf.Floor(position.x / _hashSize) * 36928046) ^
                   ((int)Mathf.Floor(position.y / _hashSize) * 9674831) ^
                   ((int)Mathf.Floor(position.z / _hashSize) * 41746395);
        }
    }
}
