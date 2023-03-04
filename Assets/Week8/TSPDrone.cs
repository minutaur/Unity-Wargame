using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Week8
{
    public class TSPDrone : MonoBehaviour
    {
        public List<Transform> initialLocations = new List<Transform>();
        
        public float moveSpeed = 8f;
        public List<Vector3> workingLocations;
        public Queue<Vector3> locations;
        public Transform basePos;
        public int populationSize = 50;
        public int maxRepeat = 1000;
        public float delayBeforeStart = 3f;
        [SerializeField] private List<int[]> population;
        [SerializeField] private int[] bestFitness;

        private bool _isRunning;
        private int _locCount;
        private CharacterController _character;

        private void Start()
        {
            _character = GetComponent<CharacterController>();
            locations = new Queue<Vector3>();

            foreach (var tr in initialLocations)
            {
                AddLocation(tr.position);
            }
        }

        public void AddLocation(Vector3 newLocation)
        {
            locations.Enqueue(newLocation);
            if (!_isRunning && locations.Count >= 7)
            {
                CancelInvoke(nameof(StartDelay));
                Invoke(nameof(StartDelay), delayBeforeStart);
            }
        }

        IEnumerator VisitInOrder(int[] order)
        {
            Debug.Log("Start Moving Toward Destinations");

            float bestDist = Mathf.Infinity;
            int firstVisit = 0;
            for (int i = 0; i < _locCount; i++)
            {
                float dist = Vector3.Distance(workingLocations[order[i]], transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    firstVisit = i;
                }
            }
            
            for (int i = 0; i < _locCount; i++)
            {
                while (Vector3.Distance(workingLocations[order[(firstVisit + i) % _locCount]], transform.position) > 3f)
                {
                    _character.Move((workingLocations[order[(firstVisit + i) % _locCount]] - transform.position).normalized *
                                    (moveSpeed * Time.deltaTime));
                    transform.forward = (workingLocations[order[(firstVisit + i) % _locCount]] - transform.position).normalized;
                    yield return null;
                }
            }
            while (Vector3.Distance(basePos.position, transform.position) > 3f)
            {
                _character.Move((basePos.position - transform.position).normalized *
                                (moveSpeed * Time.deltaTime));
                yield return null;
            }

            EndVisiting();
        }

        void EndVisiting()
        {
            Debug.Log("Done Finding Path");
            transform.position = basePos.position;
            _isRunning = false;
        }

        void StartDelay()
        {
            StartCoroutine(FindShortest());
        }

        IEnumerator FindShortest()
        {
            yield return new WaitForSeconds(delayBeforeStart);
            Debug.Log("Start Finding Path");
            Init();
            for (int i = 0; i < maxRepeat; i++)
            {
                yield return null;
                Generation();
            }

            int bestIndex = 0;
            for (int i = 1; i < populationSize; i++)
            {
                if (CalcFitness(population[bestIndex]) < CalcFitness(population[i]))
                {
                    bestIndex = i;
                }
            }

            bestFitness = population[bestIndex];
            StartCoroutine(VisitInOrder(population[bestIndex]));
        }

        void Init()
        {
            _isRunning = true;
            _locCount = locations.Count;
            workingLocations = new List<Vector3>(_locCount);
            foreach (var loc in locations)
            {
                workingLocations.Add(loc);
            }
            locations.Clear();

            population = new List<int[]>(populationSize);
            for (int i = 0; i < populationSize; i++)
            {
                int[] genes = Enumerable.Range(0, _locCount).ToArray();
                Shuffle(genes);
                population.Add(genes);
            }
        }

        void Generation()
        {

            List<int[]> newPopulation = new List<int[]>(populationSize);
            for (int i = 0; i < populationSize; i++)
            {
                int parentA = Select(10);
                int parentB = Select(10);
                int[] newGenes = Crossover(population[parentA], population[parentB]);
                if (Random.Range(0f, 1f) < 0.04f)
                {
                    int a = Random.Range(0, _locCount);
                    int b = Random.Range(0, _locCount);

                    (newGenes[a], newGenes[b]) = (newGenes[b], newGenes[a]);
                }
                newPopulation.Add(newGenes);
            }
        }

        int CalcFitness(int[] genes)
        {
            int fitness = (int)Vector3.Distance(workingLocations[0], workingLocations[^1]);
            
            for (int i = 1; i < _locCount; i++)
            {
                fitness += (int)Vector3.Distance(workingLocations[i], workingLocations[i - 1]);
            }

            return -fitness;
        }

        int Select(int k)
        {
            int bestIndex = Random.Range(0, populationSize);
            for (int i = 1; i < k; i++)
            {
                int index = Random.Range(0, populationSize);
                if (CalcFitness(population[bestIndex]) < CalcFitness(population[index]))
                {
                    bestIndex = index;
                }
            }
            return bestIndex;
        }
        
        int[] Crossover(int[] a, int[] b)
        {
            int pointA = Random.Range(0, _locCount);
            int pointB = Random.Range(0, _locCount);

            if (pointA > pointB)
                (pointA, pointB) = (pointB, pointA);

            int[] child = new int[_locCount];
            int childLen = 0;
            
            for (int i = pointA; i <= pointB; i++)
                child[childLen++] = a[i];

            for (int i = 0; i < _locCount; i++)
                if (!child.Contains(b[i]))
                    child[childLen++] = a[i];

            return child;
        }
        static void Shuffle<T>(T[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                int j = Random.Range(i, list.Length);
                
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
