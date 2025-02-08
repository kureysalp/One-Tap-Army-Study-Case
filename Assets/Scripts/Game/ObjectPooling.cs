using OneTapArmyCase.Army;
using OneTapArmyCase.System;
using UnityEngine;
using UnityEngine.Pool;

namespace OneTapArmyCase.Game
{
    public class ObjectPooling : Singleton<ObjectPooling>
    {
        private ObjectPool<Soldier> _soldierPool;
        public ObjectPool<Soldier>  SoldierPool => _soldierPool;
        
        [SerializeField] private Soldier _soldierPrefab;
        
        [SerializeField] private int _poolSize;

        private void Awake()
        {
            _soldierPool = new ObjectPool<Soldier>(OnCreateSoldier, OnGetSoldier, OnReleaseSoldier,OnDestroySoldier,false,_poolSize);
        }

        private Soldier OnCreateSoldier()
        {
            var soldier = Instantiate(_soldierPrefab);
            return soldier;
        }

        private void OnGetSoldier(Soldier soldier)
        {
            soldier.gameObject.SetActive(true);
        }
        
        private void OnReleaseSoldier(Soldier soldier)
        {
            soldier.gameObject.SetActive(false);
        }
        
        private void OnDestroySoldier(Soldier soldier)
        {
            Destroy(soldier.gameObject);
        }

    }
}
