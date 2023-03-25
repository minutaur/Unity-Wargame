using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Wargame.PlayerSystem;
using Wargame.WeaponSystem;

namespace Wargame
{
    public class Sector : MonoBehaviour
    {
        public int owningTeam = -1;
        public int[] teamCount;
        public float conquerTime = 10f;
        public float curConquerPercent { get; private set; }


        public UnityEvent<int> onConquer, onLost;

        public int tryOwningTeam = -1;

        public void Update()
        {
            if (owningTeam < 0 && tryOwningTeam >= 0)
            {
                if (curConquerPercent < conquerTime)
                    curConquerPercent += Time.deltaTime;
                else
                {
                    curConquerPercent = conquerTime;
                    owningTeam = tryOwningTeam;
                    onConquer?.Invoke(owningTeam);
                }
            }
            else if (owningTeam != tryOwningTeam && tryOwningTeam >= 0)
            {
                if (curConquerPercent > 0)
                    curConquerPercent -= Time.deltaTime;
                else
                {
                    curConquerPercent = 0;
                    onLost?.Invoke(owningTeam);
                    owningTeam = -1;
                }
            }
            else if (owningTeam < 0)
            {
                curConquerPercent = 0;
            } else if (tryOwningTeam < 0)
            {
                if (curConquerPercent < conquerTime)
                    curConquerPercent += Time.deltaTime;
                else
                    curConquerPercent = conquerTime;
            }
        }

        void CheckTeamCanOwn(int team) {
            int sum = 0;
            for (int i = 0; i < teamCount.Length; i++)
            {
                if (i != team)
                {
                    sum += teamCount[i];
                }
            }

            if (teamCount[team] > sum)
            {
                tryOwningTeam = team;
            }
            else
            {
                tryOwningTeam = -1;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Entity e = other.GetComponent<Entity>();
                if (e.team >= 0)
                    teamCount[e.team]++;
                if (other.GetComponent<PlayerController>())
                    PlayerHUDManager.inst.SetSector(this);
                CheckTeamCanOwn(e.team);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Entity e = other.GetComponent<Entity>();
                if (e.team >= 0)
                    teamCount[e.team]--;

                if (other.GetComponent<PlayerController>())
                    PlayerHUDManager.inst.UnsetSector();
                CheckTeamCanOwn(e.team);
            }
        }
    }
}
