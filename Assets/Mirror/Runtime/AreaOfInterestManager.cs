using UnityEngine;

namespace Mirror
{
    [DisallowMultipleComponent]
    public class AreaOfInterestManager : MonoBehaviour
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(ClientObjectManager));

        #region Fields

        [Header("Owner Of Manager"), Tooltip("The server in which this manager will operate under.")]
        public ServerObjectManager serverObjectManager;

        private SpatialHash<INetworkConnection> _observedConnections;

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
            for (int i = 0; i < _observedConnections.Count; i++)
            {
            }
        }

        #endregion

        #region Event Listener's

        private void OnClientAuthenticated(INetworkConnection conn)
        {
        }

        private void OnClientDisconnected(INetworkConnection conn)
        {
        }

        #endregion
    }
}
