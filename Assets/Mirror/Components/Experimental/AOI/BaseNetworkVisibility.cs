using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Experimental
{
    public abstract class BaseNetworkVisibility : NetworkVisibility
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(BaseNetworkVisibility));

        #region Fields

        [Header("Person's Vision Range")]
        protected int VisibleRange = 10;

        private AreaOfInterestManager _areaOfInterestManager;
        protected HashSet<INetworkConnection> Observers;

        #endregion

        #region Commands

        /// <summary>
        /// 
        /// </summary>
        /// <param name="range"></param>
        [ServerRpc]
        public void AdjustVisibleRange(int range)
        {
#if UNITY_EDITOR || UNITY_SERVER
            VisibleRange = range;
#endif
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _areaOfInterestManager = FindObjectOfType<AreaOfInterestManager>();

            if (_areaOfInterestManager is null)
            {
                logger.LogError("[BaseNetworkVisibility] - Missing area of interest manager this component cannot work without it.");
                return;
            }

            _areaOfInterestManager.ObserversUpdate += ObserversUpdate;
        }

        private void ObserversUpdate(INetworkConnection conn, HashSet<INetworkConnection> observers)
        {
            if(conn.Identity.NetId != NetId) return;

            Observers = observers;
        }

        #endregion
    }
}
