using System;
using UnityEngine;
using UnityEngine.Events;

namespace Week6
{
    public class JumpListener : MonoBehaviour
    {
        private EventExample target;
        private Rigidbody rigid;

        private void Start()
        {
            rigid = GetComponent<Rigidbody>();
            target = FindObjectOfType<EventExample>();
            target.OnJump += DoJump;
        }

        void DoJump()
        {
            Debug.Log(gameObject.name + "는 " + target.gameObject.name + "를 보고 화들짝!");
            rigid.AddForce(Vector3.up * 5f);
        }
    }
}
