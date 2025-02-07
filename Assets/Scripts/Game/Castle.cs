using System;
using System.Collections;
using OneTapArmyCase.Army;
using UnityEngine;

namespace OneTapArmyCase.Game
{
    public class Castle : MonoBehaviour
    {
        public static event Action<Soldier> OnGenerateSoldier;

        [SerializeField] private Transform _spawnPoint;
        
        private float _currentHealth;
        private float _currentExp;

        [SerializeField] private float _soldierGenerateRate;

        private Coroutine _soldierGenerateCoroutine;
        
        private void Start()
        {
            _soldierGenerateCoroutine = StartCoroutine(CO_GenerateSoldier());
        }


        private IEnumerator CO_GenerateSoldier()
        {
            while (true)
            {
                var soldier = ObjectPooling.Instance.SoldierPool.Get();
                soldier.transform.position = _spawnPoint.position;
                
                OnGenerateSoldier?.Invoke(soldier);
                
                yield return new WaitForSeconds(1 / _soldierGenerateRate);
            }
        }
    }
}
