using System;
using System.Collections;
using OneTapArmyCase.Enums;
using UnityEngine;
using UnityEngine.AI;

namespace OnaTapArmyCase.Army
{
    public class Soldier : MonoBehaviour
    {
        [SerializeField] protected SoldierProperties _soldierProperties;
        [SerializeField] private LayerMask _enemySoldierLayer;
        
        private SoldierState _soldierState;
        public SoldierState SoldierState => _soldierState;
        
        protected float _currentHealth;
        protected float _attackPower;
        protected float _attackSpeed;
        protected float _movementSpeed;
        protected float _enemyDetectDistance;
        protected float _attackRange;

        protected int _level;
        
        private NavMeshAgent _navMeshAgent;
        public NavMeshAgent NavMeshAgent => _navMeshAgent;
        
        private readonly Collider[] _enemyBuffer = new Collider[10];

        private Soldier _currentTarget;

        public bool IsDead => _currentHealth <= 0;
        
        private void OnEnable()
        {
            if(_navMeshAgent == null)
                _navMeshAgent = GetComponent<NavMeshAgent>();
            
            SoldierSetup();
        }

        private void SoldierSetup()
        {
            _currentHealth = _soldierProperties.MaxHealth;
            _attackPower = _soldierProperties.AttackPower;
            _attackSpeed = _soldierProperties.AttackSpeed;
            _movementSpeed = _soldierProperties.MovementSpeed;
            _enemyDetectDistance = _soldierProperties.EnemyDetectDistance;
            _attackRange = _soldierProperties.AttackRange;
        }

        public void Move(Vector3 destination)
        {
            _navMeshAgent.destination = destination;
            _soldierState = SoldierState.Moving;
        }

        public void ApproachForAttacking(Soldier target)
        {
            _currentTarget = target;

            StartCoroutine(CO_AttackingEnemy());
        }

        private IEnumerator CO_AttackingEnemy()
        {
            while (!_currentTarget.IsDead)
            {
                while (Vector3.Distance(transform.position, _currentTarget.transform.position) > _attackRange)
                {
                    var attackPosition = (transform.position - _currentTarget.transform.position).normalized * _attackRange;
                    Move(attackPosition);
                }

                _soldierState = SoldierState.InCombat;
                Debug.Log("attacked to " + _currentTarget);
            
                var nextAttackTime = 1 / _attackSpeed;
                yield return new WaitForSeconds(nextAttackTime);
                
               // yield return null;
            }
        }

        private IEnumerator CO_Attack()
        {
            _soldierState = SoldierState.InCombat;
            Debug.Log("attacked to " + _currentTarget);
            
            var nextAttackTime = 1 / _attackSpeed;
            yield return new WaitForSeconds(nextAttackTime);
        }

        public Soldier CheckForEnemies()
        {
            var enemyCount = Physics.OverlapSphereNonAlloc(transform.position, _enemyDetectDistance, _enemyBuffer,_enemySoldierLayer);
            
            if (enemyCount == 0) return null;
            
            InsertionSort(_enemyBuffer, enemyCount);
            return _enemyBuffer[0].GetComponent<Soldier>();
        }

        private void InsertionSort(Collider[] array, int length)
        {
            for (var i = 0; i < length; i++)
            {
                var key = array[i];
                var keyDistance = (transform.position - key.transform.position).sqrMagnitude;
                var j = i - 1;

                
                while (j>=0 && (transform.position - array[j].transform.position).sqrMagnitude > keyDistance)
                {
                    array[j + 1] = array[j];
                    j--;
                }
                
                array[j + 1] = key;
            }
        }
    }
}
