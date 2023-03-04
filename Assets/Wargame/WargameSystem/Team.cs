using System.Collections.Generic;
using UnityEngine;

namespace WargameSystem
{
    public class Team : MonoBehaviour
    {
        public int life = 100;
        public int playerCount = 8;
        public int owningSectors;
        public List<Transform> spawnPoints;

        private const float SpawnOffset = 3f;

        public Vector3 GetRespawnPoint()
        {
            Vector3 spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position;

            spawnPos += new Vector3(Random.Range(-SpawnOffset, SpawnOffset), 0, Random.Range(-SpawnOffset, SpawnOffset));
            
            return spawnPos;
        }
    }
}