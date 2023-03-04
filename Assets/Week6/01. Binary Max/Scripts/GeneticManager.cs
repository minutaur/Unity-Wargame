using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Week6.Binary_Max
{
    class Individual : IComparable<Individual>
    {
        public int[] Genes;
        public int Fitness;
        public Individual(int[] genes)
        {
            Genes = genes;
        }
        public void CalculateFitness()
        {
            Fitness = 0;
            for (int i = 0; i < Genes.Length; i++)
                Fitness += Genes[i];
        }

        public int CompareTo(Individual other)
        {
            // 내림차순으로 정렬하기 위해 (-)부호를 붙임
            return -Fitness.CompareTo(other.Fitness);
        }

        public void Mutate(float mutationRate)//변이
        {
            for (int i = 0; i < Genes.Length; i++)
                if (Random.Range(0f, 1f) < mutationRate)
                    Genes[i] = 1 - Genes[i];
        }

        public static Individual Crossover(Individual a, Individual b)
        {
            int len = a.Genes.Length;
            int[] childGenes = new int[len];
            int point = Random.Range(0, len);
            
            for (int i = 0; i < len; i++)
                childGenes[i] = i < point ? a.Genes[i] : b.Genes[i];

            return new Individual(childGenes);
        }
    }

    public class GeneticManager : MonoBehaviour
    {
        public int PopulationSize = 10;
        public int BinaryLength = 5;
        public int Generation;
        
        public TextMeshProUGUI GenerationText;
        public TextMeshProUGUI DNAText;
        
        List<Individual> population;
        
        void Start()
        {
            Init();
            ShowPopulation();
        }

        public void Init()
        {
            Generation = 0;
            population = new List<Individual>(PopulationSize);
            for (int i = 0; i < PopulationSize; i++)
            {
                population.Add(new Individual(new int[BinaryLength]));
                
                for (int j = 0; j < BinaryLength; j++)
                    population[i].Genes[j] = Random.Range(0, 2);
                
                population[i].CalculateFitness();
            }
            population.Sort();
        }
        
        public void RunGeneration()
        {
            ++Generation;
            GenerationText.text = "Generation " + Generation;

            List<Individual> newPopulation = new List<Individual>(PopulationSize);

            for (int i = 0; i < PopulationSize; i++)
            {
                Individual a = Select(3);
                Individual b = Select(3);
                Individual child = Individual.Crossover(a, b);
                child.Mutate(0.01f);
                child.CalculateFitness();
                newPopulation.Add(child);
            }

            population = newPopulation;
            population.Sort();
            ShowPopulation();
        }

        Individual Select(int k)
        {
            Individual best = population[Random.Range(0, PopulationSize)];

            for (int i = 1; i < k; i++)
            {
                Individual temp = population[Random.Range(0, PopulationSize)];
                if (temp.Fitness > best.Fitness)
                    best = temp;
            }

            return best;
        }

        public void ShowPopulation()
        {
            string result = "";

            for (int i = 0; i < PopulationSize; i++)
            {
                result += string.Join("", population[i].Genes) + " ";
            }

            DNAText.text = result;
        }
    }
}