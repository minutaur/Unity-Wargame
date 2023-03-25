using Unity.VisualScripting;
using UnityEngine;

namespace Wargame.AISystem
{
    public abstract class State : MonoBehaviour
    {
        public abstract State Run();
    }
}