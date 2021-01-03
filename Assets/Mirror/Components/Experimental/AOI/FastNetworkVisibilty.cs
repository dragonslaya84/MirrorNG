using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Experimental
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/Experimental/FastNetworkVisibility")]
    [HelpURL("https://mirror-networking.com/docs/Components/NetworkTransform.html")]
    public class FastNetworkVisibility : BaseNetworkVisibility
    {
        public override bool OnCheckObserver(INetworkConnection conn)
        {
            return true;
        }

        public override void OnRebuildObservers(HashSet<INetworkConnection> observers, bool initialize)
        {
        }
    }
}
