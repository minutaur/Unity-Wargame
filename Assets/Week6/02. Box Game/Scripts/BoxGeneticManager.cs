using System.Collections.Generic;
using UnityEngine;

namespace Week6.BoxGame
{//전체 유전자 제어
    public class BoxGeneticManager : MonoBehaviour
    {
        public int agentCount;
        public GameObject agentPrefab;
        public Transform goal;
        public List<BoxAgent> population;

        public int maxBehaviors;
        public float waitSeconds = 1f;
        public float generationTimeout;

        private float _totalFitness;

        private void Start()
        {
            Init();
        }

        private void Init()//초기화
        {
            population = new List<BoxAgent>(agentCount);//박스 에이전트 코드 가져옴
            for (int i = 0; i < agentCount; i++)
            {
                BoxAgent go = Instantiate(agentPrefab).GetComponent<BoxAgent>();
                go.transform.position = transform.position;
                go.Genes = new int[maxBehaviors];
                for (int j = 0; j < maxBehaviors; j++)
                    go.Genes[j] = Random.Range(0, 4);
                go.Target = goal;
                population.Add(go);
            }
            
            Invoke("RunGeneration", .1f);
            //0.1초느리게 에이전트 코드에있는 스타트함수는 더 늦게 실행 되기 때문에 골같은 정보를 받아오기 위해 조금의 시간 지연을 줌
        }

        public void RunGeneration()
        {
            generationTimeout = waitSeconds * (maxBehaviors + 1);//점수계산하는 시간을 준것
            
            for (int i = 0; i < agentCount; i++)
            {
                population[i].WaitSeconds = waitSeconds;
                population[i].StartMove();
            }

            Invoke("Replace", generationTimeout);//시간이 지나면 적합도 합계를 구함(룰렛형을 사용하기위해
        }

        public void Replace()
        {
            population.Sort();
            
            _totalFitness = 0;
            for (int i = 0; i < agentCount; i++)
            {
                _totalFitness += population[i].Fitness;
            }

            List<BoxAgent> newPopulation = new List<BoxAgent>();
            for (int i = 0; i < agentCount; i++)
            {
                BoxAgent a = Selection();
                BoxAgent b = Selection();

                BoxAgent child = Crossover(a, b);
                child.Mutate(0.08f);//8퍼센트로 변이를 일으키는 것을 볼수 있음
                newPopulation.Add(child);
            }

            for (int i = 0; i < agentCount; i++)
            {
                Destroy(population[i].gameObject);
            }
            
            population = newPopulation;
            RunGeneration();
        }

        public BoxAgent Selection()//선택
        {
            for (int i = 0; i < agentCount; i++)
                if (Random.Range(0f, 1f) < population[i].Fitness / _totalFitness)
                    return population[i];

            return population[0];
        }

        public BoxAgent Crossover(BoxAgent a, BoxAgent b)//교차
        {
            BoxAgent child = Instantiate(agentPrefab).GetComponent<BoxAgent>();
            child.Genes = new int[maxBehaviors];
            for (int i = 0; i < maxBehaviors; i++)
            {
                if (Random.Range(0f, 1f) <= 0.5f)//50프로로 a50프로로 b에서 불러옴
                    child.Genes[i] = a.Genes[i];
                else
                    child.Genes[i] = b.Genes[i];
            }

            return child;
        }
    }
}