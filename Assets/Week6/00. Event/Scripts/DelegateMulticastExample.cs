using UnityEngine;

namespace Week6
{
    public class DelegateMulticastExample : MonoBehaviour
    {
        delegate void SomeDelegate(int n);

        void RunThis(int val)
        {
            Debug.Log("받은 값은 : " + val);
        }

        void OtherTask(int index)
        {
            Debug.Log("번호는 : " + index + " 입니다.");
        }

        private void Start()
        {
            SomeDelegate run = RunThis;
            run += OtherTask;

            run(2023);
        }
    }
}