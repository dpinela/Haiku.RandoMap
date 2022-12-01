using UE = UnityEngine;

namespace RandoMap
{
    internal class Destructor : UE.MonoBehaviour
    {
        public System.Action Func = () => {};

        public void OnDestroy()
        {
            Func();
        }
    }
}