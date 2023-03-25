using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Wargame.AISystem;
using Wargame.PlayerSystem;
using Wargame.WeaponSystem;

namespace Wargame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager inst;
        public int defaultLife = 100;
        public int teamPlayerCount = 8;
        public int gameTime = 600;

        public float lifeLossTime = 6f;
        public List<Team> teams;
        public Sector[] sectors;
        public bool isGameRunning;
        
        private GameUIManager _gameUI;

        private void Awake()
        {
            if (!inst)
            {
                inst = this;
            }
            _gameUI = FindObjectOfType<GameUIManager>();
        }

        private void Start()
        {
            // 이 부분은 싱글플레이용으로 임시로 만든 것이므로 수정해야함
            PlayerController player = FindObjectOfType<PlayerController>();
            teams[player.team].AddPlayer(player.GetComponent<Entity>());
            
            InitGame();
        }

        public void Update()
        {
            for(int i = 0; i < teams.Count; i++)
            {
                _gameUI.UpdateLife(i, teams[i].life);
            }
        }

        void OnConquerSector(int team)
        {
            Debug.Log("Team " + team + " has Captured Sector");
            teams[team].owningSectors++;
        }

        void OnLostSector(int team)
        {
            Debug.Log("Team " + team + " has Lost Sector");
            teams[team].owningSectors--;
        }

        private void InitGame()
        {
            int index = 0;
            foreach (var team in teams)
            {
                team.life = defaultLife;
                team.playerCount = teamPlayerCount;
                team.owningSectors = 0;
                team.teamIndex = index++;
                team.InitTeam();
            }
            sectors = FindObjectsOfType<Sector>();
            foreach (var sector in sectors)
            {
                sector.owningTeam = -1;
                sector.teamCount = new int[teams.Count];
                sector.onConquer.RemoveAllListeners();
                sector.onLost.RemoveAllListeners();
                
                sector.onConquer.AddListener(OnConquerSector);
                sector.onLost.AddListener(OnLostSector);
            }

            foreach (var t in teams)
            {
                foreach (var e in t.players)
                {
                    if (e.TryGetComponent(out AIController ai))
                    {
                        ai.AfterGameInit();
                    }
                }
            }

            isGameRunning = true;
            StartCoroutine(GameCounter());
            StartCoroutine(LifeLoss());
        }

        IEnumerator GameCounter()
        {
            int delta = 0;
            while (delta <= gameTime && isGameRunning)
            {
                _gameUI.UpdateTime(gameTime - delta);
                
                yield return new WaitForSeconds(1f);
                delta += 1;
            }
        }
        
        // 밑의 코드는 팀이 2개라고 가정하고 제작하였음. 혹시 팀이 3명 이상이 존재하게 하고 싶다면 수정바람

        IEnumerator LifeLoss()
        {
            while (isGameRunning)
            {
                yield return new WaitForSeconds(lifeLossTime);
                teams[0].life -= teams[1].owningSectors;
                teams[1].life -= teams[0].owningSectors;
                CheckVictory();
            }
        }

        public void CheckVictory()
        {
            if (teams[0].life <= 0)
                GameVictory(1);
            if (teams[1].life <= 0)
                GameVictory(0);
        }

        public void GameVictory(int team)
        {
            Debug.Log(team + " 팀이 승리!");
            isGameRunning = false;
        }
    }
}