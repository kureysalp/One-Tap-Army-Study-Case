using System.Collections;
using OneTapArmyCase.Enums;
using OneTapArmyCase.Game;
using UnityEngine;
using UnityEngine.AI;

namespace OneTapArmyCase.Army
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
        protected float _enemyDetectDistance;
        protected float _attackRange;

        protected int _level;
        
        private NavMeshAgent _navMeshAgent;
        
        private readonly Collider[] _enemyBuffer = new Collider[10];

        private Soldier _currentTarget;
        private ArmyManager _currentArmy;
        private Animator _animator;
        
        private static readonly int WalkAnimationString = Animator.StringToHash("Walk");
        private static readonly int AttackAnimationString = Animator.StringToHash("Attack");
        private static readonly int IdleAnimationString = Animator.StringToHash("Idle");


        public bool IsDead => _currentHealth <= 0;
        
        private void OnEnable()
        {
            if(_navMeshAgent == null)
                _navMeshAgent = GetComponent<NavMeshAgent>();
            
            SoldierSetup();
        }

        public void AssignToArmy(ArmyManager army)
        {
            _currentArmy = army;
        }

        private void SoldierSetup()
        {
            _currentHealth = _soldierProperties.MaxHealth;
            _attackPower = _soldierProperties.AttackPower;
            _attackSpeed = _soldierProperties.AttackSpeed;
            _enemyDetectDistance = _soldierProperties.EnemyDetectDistance;
            _attackRange = _soldierProperties.AttackRange;

            _navMeshAgent.speed = _soldierProperties.MovementSpeed;

            _soldierState = SoldierState.Idle;
        }

        public void Move(Vector3 destination)
        {
            _animator.SetTrigger(WalkAnimationString);
            _navMeshAgent.destination = destination;
        }

        public void ApproachForAttacking(Soldier target)
        {
            _currentTarget = target;
            _soldierState = SoldierState.InCombat;
            StartCoroutine(CO_AttackingEnemy());
        }

        private IEnumerator CO_AttackingEnemy()
        {
            while (!_currentTarget.IsDead)
            {
                while (Vector3.Distance(transform.position, _currentTarget.transform.position) > _attackRange)
                {
                    var attackPosition = _currentTarget.transform.position + (transform.position - _currentTarget.transform.position).normalized * _attackRange;
                    Move(attackPosition);
                    yield return null;
                }
                _soldierState = SoldierState.InCombat;

                Attack();                
                var nextAttackTime = 1 / _attackSpeed;
                yield return new WaitForSeconds(nextAttackTime);
            }
            
            SetIdle();
        }

        private void SetIdle()
        {
            _soldierState = SoldierState.Idle;
            _animator.SetTrigger(IdleAnimationString);
        }

        protected virtual void Attack()
        {
            _animator.SetTrigger(AttackAnimationString);
        }

        private void DealDamage()
        {
            _currentTarget.GetHit(_attackPower);
        }

        public Soldier CheckForEnemies()
        {
            var enemyCount = Physics.OverlapSphereNonAlloc(transform.position, _enemyDetectDistance, _enemyBuffer,_enemySoldierLayer);
            
            if (enemyCount == 0)
            {
                return null;
            }
            
            InsertionSort(_enemyBuffer, enemyCount);
            return _enemyBuffer[0].GetComponent<Soldier>();
        }

        private void GetHit(float damage)
        {
            _currentHealth -= damage;

            if (_currentHealth <= 0)
                Die();
        }

        private void Die()
        {
            ObjectPooling.Instance.SoldierPool.Release(this);
            _currentArmy.ReleaseSoldier(this);
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
