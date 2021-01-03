using UnityEngine;

namespace Mirror.Experimental
{
    public abstract class BaseNetworkVisibility : NetworkVisibility
    {
        #region Fields

        [Header("Network Visibility Config")]
        [SerializeField] private int _cellSize = 5;
        [SerializeField] private int _cellHeight = 1000;

        private Vector3 _objectPosition;

        #endregion

        #region Unity Methods

        private void Update()
        {
            _objectPosition = transform.position;
        }

        public void OnDrawGizmos()
        {
            //Gizmos.DrawWireCube(new Vector3(_objectPosition.x, _cellSize / 2 , _objectPosition.z), new Vector3(_cellSize, _cellSize, _cellSize));
        }

        #endregion
    }
}
