using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Week6
{
    public class EventExample : MonoBehaviour
    {

        public delegate void JumpEvent();

        public event JumpEvent OnJump;

        private Rigidbody rigid;

        private void Start()
        {
            rigid = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (Keyboard.current.spaceKey.isPressed)
            {
                // 아래 코드 대신 OnJump?.Invoke(); 과 같이 사용가능
                if (OnJump != null)
                    OnJump();
                
                rigid.AddForce(Vector3.up * 10f);
            }
        }
    }
}