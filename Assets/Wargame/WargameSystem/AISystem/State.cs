using Unity.VisualScripting;
using UnityEngine;

namespace Wargame.AISystem
{
    public abstract class State : MonoBehaviour
    {
        public abstract State Run();
    }
}


class Test : IComparable
{
    private int value;
    public bool CompareTo(object obj)
    {
        if (obj is Test)
        {
            Test other = obj as Test;
            return value == other.value;
        }

        return false;
    }
}

public interface IComparable
{
    bool CompareTo(object obj);
}