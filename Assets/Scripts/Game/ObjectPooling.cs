using System.Collections.Generic;
using OneTapArmyCase.Army;
using OneTapArmyCase.Enums;
using OneTapArmyCase.System;
using UnityEngine;
using UnityEngine.Pool;

namespace OneTapArmyCase.Game
{
    public class ObjectPooling : Singleton<ObjectPooling>
    {

        readonly Dictionary<SoldierType, ObjectPool<Soldier>> _soldierPools = new();

        [SerializeField] private Soldier _soldierPrefab;

        [SerializeField] private Soldier _archerUnit;
        [SerializeField] private Soldier _swordsmanUnit;
        [SerializeField] private Soldier _warriorUnit;
        [SerializeField] private Soldier _giantUnit;

        [SerializeField] private GameObject _arrowPrefab;

        private ObjectPool<GameObject> _arrowPool;

        [SerializeField] private int _poolSize;
        [SerializeField] private int _poolArrow;


        private void Awake()
        {
            CreatePools();
        }


        private void OnGetSoldier(Soldier soldier)
        {
            //soldier.gameObject.SetActive(true);
        }

        private void OnReleaseSoldier(Soldier soldier)
        {
            soldier.gameObject.SetActive(false);
        }

        private void OnDestroySoldier(Soldier soldier)
        {
            Destroy(soldier.gameObject);
        }

        private void CreatePools()
        {
            var archerPool = new ObjectPool<Soldier>(() => Instantiate(_archerUnit), OnGetSoldier, OnReleaseSoldier, OnDestroySoldier, false, _poolSize, _poolSize * 2);
            var swordsmanPool = new ObjectPool<Soldier>(() => Instantiate(_swordsmanUnit), OnGetSoldier, OnReleaseSoldier, OnDestroySoldier, false, _poolSize, _poolSize * 2);
            var warriorPool = new ObjectPool<Soldier>(() => Instantiate(_warriorUnit), OnGetSoldier, OnReleaseSoldier, OnDestroySoldier, false, _poolSize, _poolSize * 2);
            var giantPool = new ObjectPool<Soldier>(() => Instantiate(_giantUnit), OnGetSoldier, OnReleaseSoldier, OnDestroySoldier, false, _poolSize, _poolSize * 2);

            _soldierPools.Add(SoldierType.Archer, archerPool);
            _soldierPools.Add(SoldierType.Swordsman, swordsmanPool);
            _soldierPools.Add(SoldierType.Warrior, warriorPool);
            _soldierPools.Add(SoldierType.Giant, giantPool);

            _arrowPool = new ObjectPool<GameObject>(() => Instantiate(_arrowPrefab), OnGetArrow, OnReleaseArrow, OnDestroyArrow, false, _poolArrow, _poolSize * 2);
        }

        private void OnGetArrow(GameObject arrow)
        {
            arrow.SetActive(true);
        }

        private void OnReleaseArrow(GameObject arrow)
        {
            arrow.SetActive(false);
        }

        private void OnDestroyArrow(GameObject arrow)
        {
            Destroy(arrow);
        }

        public Soldier GetFromPool(SoldierType soldierType)
        {
            return _soldierPools[soldierType].Get();
        }

        public void Release(Soldier soldier, SoldierType soldierType)
        {
            _soldierPools[soldierType].Release(soldier);
        }

        public GameObject GetArrowFromPool()
        {
            return _arrowPool.Get();
        }

        public void ReleaseArrow(GameObject arrow)
        {
            _arrowPool.Release(arrow);
        }

    }
}
