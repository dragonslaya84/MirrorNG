using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace Mirror.Examples.AOI
{
    public class EntityGridSpawner : MonoBehaviour
    {
        #region Fields

        [Header("Grid Spawn Location Configuration")]
        [SerializeField] private PlayerSpawner _playerSpawner;

        [SerializeField, Range(1, 10)] private int _spawnPointLimiter = 5;

        private int _currentSpawnIndex = 1;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if(_playerSpawner is null) return;

            NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

            int maxIndices = navMeshData.indices.Length - 3;
            int maxVertices = navMeshData.vertices.Length - 3;


            for (int i = 0; i < maxIndices; i++)
            {
                for (int j = 0; j < maxVertices; j++)
                {
                    Vector3 point = navMeshData.vertices[navMeshData.indices[i]];
                    point.y *= point.y;

                    var spawnPoint = new GameObject($"Spawn Location {_currentSpawnIndex}");

                    spawnPoint.transform.position = point;
                    spawnPoint.transform.SetParent(transform);

                    _playerSpawner.startPositions.Add(spawnPoint.transform);

                    _currentSpawnIndex++;

                    j += _spawnPointLimiter;
                }

                i += _spawnPointLimiter;
            }
        }

        #endregion
    }
}
