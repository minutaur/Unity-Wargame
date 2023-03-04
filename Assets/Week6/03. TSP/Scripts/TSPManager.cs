using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Week6.TSP
{
    public class TSPManager : MonoBehaviour
    {
        public GameObject CityPrefab;
        public int AgentCount;
        public List<IndividualAgent> Agents;
        public Button nextButton;

        public int tournamentSize = 3;
        public float offsetX = 20f;

        [SerializeField]
        private int CityCount;
        
        
        private void Start()
        {
            Init();
        }

        public void Init()
        {
            CityCount = CityPrefab.transform.childCount;
            Agents = new List<IndividualAgent>();
            for (int i = 0; i < AgentCount; i++)
            {
                IndividualAgent go = Instantiate(CityPrefab).GetComponent<IndividualAgent>();
                go.transform.position = Vector3.right * offsetX * i;
                List<int> genes = Enumerable.Range(0, CityCount).ToList();
                Shuffle(genes);
                go.VisitOrders = genes;
                go.Cities = go.GetComponentsInChildren<City>();
                go.CalculateFitness();
                
                Agents.Add(go);
            }
            
            for (int i = 0; i < AgentCount; i++)
            {
                Agents[i].ShowPath();
            }
        }

        public void RunGeneration()
        {
            nextButton.interactable = false;
            List<IndividualAgent> newAgents = new List<IndividualAgent>();
            
            for (int i = 0; i < AgentCount; i++)
            {
                IndividualAgent a = Selection();
                IndividualAgent b = Selection();
                IndividualAgent child = Instantiate(CityPrefab).GetComponent<IndividualAgent>();
                child.transform.position = Vector3.right * offsetX * i;

                child.VisitOrders = Crossover(a, b);
                child.Cities = child.GetComponentsInChildren<City>();
                child.Mutate(0.05f);
                child.CalculateFitness();
                
                newAgents.Add(child);
            }

            for (int i = 0; i < AgentCount; i++)
            {
                Destroy(Agents[i].gameObject);
            }

            Agents = newAgents;
            for (int i = 0; i < AgentCount; i++)
            {
                Agents[i].ShowPath();
            }
            nextButton.interactable = true;
        }
        
        private IndividualAgent Selection()
        {
            IndividualAgent bestAgent = Agents[Random.Range(0, AgentCount)];
            for (int j = 0; j < tournamentSize; j++)
            {
                IndividualAgent temp = Agents[Random.Range(0, AgentCount)];
                if (bestAgent.Fitness < temp.Fitness)
                    bestAgent = temp;
            }

            return bestAgent;
        }
        

        public List<int> Crossover(IndividualAgent a, IndividualAgent b)
        {
            int pointA = Random.Range(0, CityCount);
            int pointB = Random.Range(0, CityCount);

            if (pointA > pointB)
                (pointA, pointB) = (pointB, pointA);

            List<int> child = new List<int>(CityCount);
            
            for (int i = pointA; i <= pointB; i++)
                child.Add(a.VisitOrders[i]);

            for (int i = 0; i < CityCount; i++)
                if (!child.Contains(b.VisitOrders[i]))
                    child.Add(b.VisitOrders[i]);

            return child;
        }
        
        public void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int j = Random.Range(i, list.Count);
                
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
