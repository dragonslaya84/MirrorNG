using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror
{
    [DisallowMultipleComponent]
    public class AreaOfInterestManager : MonoBehaviour
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(AreaOfInterestManager));

        #region Fields

        [Header("Owner Of Manager"), Tooltip("The server in which this manager will operate under.")]
        public ServerObjectManager serverObjectManager;

        private SpatialHash<INetworkConnection> _observedConnections;
        private SpatialHash<Quad> _quadTree;

        public Action<INetworkConnection, HashSet<INetworkConnection>> ObserversUpdate;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (serverObjectManager is null)
            {
                logger.LogWarning("[AreaOfInterestManager] - Missing variable assignment through inspector or code..");
                return;
            }

            _observedConnections = new SpatialHash<INetworkConnection>(16);
            _quadTree = new SpatialHash<Quad>(16);

            serverObjectManager.server.Authenticated.AddListener(OnClientAuthenticated);
            serverObjectManager.server.Disconnected.AddListener(OnClientDisconnected);
        }

        private void OnDestroy()
        {
            serverObjectManager.server.Authenticated.RemoveListener(OnClientAuthenticated);
            serverObjectManager.server.Disconnected.RemoveListener(OnClientDisconnected);
        }

        private void Update()
        {
        }

        public void OnDrawGizmos()
        {
            List<Quad> trees = _quadTree.Query(new Vector3());

            for (int i = 0; i < trees.Count; i++)
            {
                Gizmos.DrawWireCube(trees[i].rect.center, new Vector3(trees[i].rect.halfWidth * 2, 0, trees[i].rect.halfLength * 2));
            }
        }

        #endregion

        #region Event Listener's

        private void OnClientAuthenticated(INetworkConnection conn)
        {
            var position = conn.Identity.transform.position;
            var quad = new Quad(new BoundingBox(position, 2, 2));
            var quadNode = new QuadNode();

            _observedConnections.UpdateOrInsert(position, conn);
            _quadTree.UpdateOrInsert(position, quad);
        }

        private void OnClientDisconnected(INetworkConnection conn)
        {
        }

        #endregion
    }
}
