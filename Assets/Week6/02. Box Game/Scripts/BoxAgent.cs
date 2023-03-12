using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Week6.BoxGame
{
    public class BoxAgent : MonoBehaviour, IComparable<BoxAgent>
    {
        public float WaitSeconds = 1f;
        public Transform Target;//목표지점
        public int[] Genes;
        public float Fitness;
        public float MoveSpeed = 3f;
        public Vector3 moveDir;

        public Rigidbody _rigid;

        private void Start()
        {
            _rigid = GetComponent<Rigidbody>();
            _rigid.constraints = RigidbodyConstraints.FreezeRotation;
            if (!Target)
                Target = GameObject.Find("Goal").transform;
        }

        private void FixedUpdate()//현재 속도값을 
        {
            _rigid.velocity = moveDir.normalized * MoveSpeed;
        }

        public void CalculateFitness()//골위치랑 내위치 비교해서 적합도 구하기
        {
            Fitness = Vector3.Distance(transform.position, Target.position);
        }

        public void StartMove()//내가 움직이게 하는 코드
        {
            StartCoroutine(MoveGenes());
        }

        IEnumerator MoveGenes()//이동 방향
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                GetIndexMoveDir(i);
                yield return new WaitForSeconds(WaitSeconds);
            }
            CalculateFitness();
        }

        public void Mutate(float mutationRate)//내 유전 인자 반복하면서 일정확률로 변이 
        {
            for (int i = 0; i < Genes.Length; i++)
                if (Random.Range(0f, 1f) < mutationRate)
                    Genes[i] = Random.Range(0, 4);
        }
        
        private void GetIndexMoveDir(int index)//0이면 오른쪽...
        {
            switch (Genes[index])
            {
                case 0:
                    moveDir = transform.right;
                    break;
                case 1:
                    moveDir = -transform.right;
                    break;
                case 2:
                    moveDir = transform.forward;
                    break;
                case 3:
                    moveDir = -transform.forward;
                    break;
            }
        }

        public int CompareTo(BoxAgent other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;//문제가있음
            return Fitness.CompareTo(other.Fitness);
        }
    }
}