using Mirror;

namespace SyncListTests.SyncListNestedInAbstractClass
{
    class SyncListNestedStruct : NetworkBehaviour
    {
        SomeAbstractClass.MyNestedStructList Foo;


        public abstract class SomeAbstractClass
        {
            public struct MyNestedStruct
            {
                public int potato;
                public float floatingpotato;
                public double givemetwopotatoes;
            }
            public class MyNestedStructList : SyncList<MyNestedStruct> { }
        }
    }
}
