using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Week6.TSP
{
    public class IndividualAgent : MonoBehaviour
    {
        public City[] Cities;
        public List<int> VisitOrders;
        public float Fitness;
        public void CalculateFitness()
        {
            Fitness = Cities[VisitOrders[0]].GetDistance(Cities[VisitOrders[Cities.Length - 1]]);
            for (int i = 1; i < Cities.Length; i++)
            {
                Fitness += Cities[VisitOrders[i]].GetDistance(Cities[VisitOrders[i - 1]]);
            }
            Fitness = -Fitness;
        }

        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < Cities.Length; i++)
            {
                if (Random.Range(0f, 1f) < mutationRate)
                {
                    int a = Random.Range(0, Cities.Length);
                    int b = Random.Range(0, Cities.Length);

                    (VisitOrders[a], VisitOrders[b]) = (VisitOrders[b], VisitOrders[a]);
                }
            }
        }

        public void ShowPath()
        {
            for (int i = 0; i < Cities.Length; i++)
            {
                if(i == Cities.Length - 1)
                    Cities[VisitOrders[i]].ShowLinePath(Cities[VisitOrders[0]]);
                else
                    Cities[VisitOrders[i]].ShowLinePath(Cities[VisitOrders[i + 1]]);
            }
        }
    }
}