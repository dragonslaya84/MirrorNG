using Mirror;
using UnityEngine;

namespace Mirror.Examples.AOI
{
    public class PlayerCameraFinder : NetworkBehaviour
    {
        #region Fields

        private Camera _playerCameraFollow;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            NetIdentity.OnStartClient.AddListener(FindCamera);
        }

        #endregion

        #region Event Listeners

        private void FindCamera()
        {
            if(!IsLocalPlayer) return;

            _playerCameraFollow = Camera.main;

            Vector3 tempPosition = _playerCameraFollow.transform.position;

            _playerCameraFollow.transform.SetParent(transform);
            _playerCameraFollow.transform.localPosition = tempPosition;
        }

        #endregion
    }
}
