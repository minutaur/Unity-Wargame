using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Wargame.AISystem;
using Wargame.PlayerSystem;
using Wargame.WeaponSystem;
using Random = UnityEngine.Random;

namespace Wargame
{
    public class Team : MonoBehaviour
    {
        public int teamIndex = -1;
        public int life = 100;
        public int playerCount = 8;
        public float respawnTimer = 3f;
        public int owningSectors;
        public List<Transform> spawnPoints;
        public List<Entity> players;
        public GameObject aiPrefab;

        private const float SpawnOffset = 3f;
        private int _aiCount;

        public void InitTeam()
        {
            _aiCount = playerCount - players.Count;
            for (int i = 0; i < _aiCount; i++)
            {
                Entity ai = Instantiate(aiPrefab, GetRespawnPoint(), Quaternion.identity).GetComponent<Entity>();
                ai.gameObject.SetActive(false);
                players.Add(ai);
            }

            foreach (var e in players)
            {
                e.team = teamIndex;
                StartCoroutine(RespawnPlayer(e));
            }
        }

        public bool AddPlayer(Entity player)
        {
            if (players.Count >= playerCount)
                return false;

            if (_aiCount > 0)
            {
                _aiCount--;
                for(int i = 0; i < players.Count; i++)
                {
                    if (!players[i].GetComponent<PlayerController>())
                    {
                        players.RemoveAt(i);
                        break;
                    }
                }
            }

            players.Add(player);
            return true;
        }
        
        public void RemovePlayer(Entity player)
        {
            players.Remove(player);
            _aiCount++;
            players.Add(Instantiate(aiPrefab).GetComponent<Entity>());
        }

        public void RespawnOnDeath(Entity diedPlayer)
        {
            StartCoroutine(RespawnPlayer(diedPlayer));
        }

        IEnumerator RespawnPlayer(Entity toRespawn)
        {
            yield return new WaitForSeconds(respawnTimer);

            if (toRespawn.TryGetComponent(out NavMeshAgent agent))
            {
                if(NavMesh.SamplePosition(GetRespawnPoint(), out var closestHit, 500, 1 ) ){
                    agent.transform.position = closestHit.position;
                }
                toRespawn.GetComponent<AIController>().OnAIRespawn();
            }
            
            
            if (toRespawn.TryGetComponent(out PlayerController pc))
            {
                pc.transform.position = GetRespawnPoint();
                pc.OnPlayerRespawn();
            }
            else
            {
                toRespawn.gameObject.SetActive(true);
            }
        }

        private Vector3 GetRespawnPoint()
        {
            Vector3 spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position;

            spawnPos += new Vector3(Random.Range(-SpawnOffset, SpawnOffset), 0, Random.Range(-SpawnOffset, SpawnOffset));
            
            return spawnPos;
        }
    }
}