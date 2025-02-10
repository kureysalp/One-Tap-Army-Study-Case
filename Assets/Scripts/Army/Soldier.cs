using System;
using System.Collections;
using OneTapArmyCase.Enums;
using OneTapArmyCase.Game;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace OneTapArmyCase.Army
{
    public class Soldier : MonoBehaviour, IAttackable
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

        protected IAttackable _currentTarget;
        private ArmyManager _currentArmy;
        private Animator _animator;

        private static readonly int WalkAnimationString = Animator.StringToHash("Walk");
        private static readonly int AttackAnimationString = Animator.StringToHash("Attack");
        private static readonly int IdleAnimationString = Animator.StringToHash("Idle");
        private static readonly int DieAnimationString = Animator.StringToHash("Die");

        [SerializeField] private Image _healthBar;
        [SerializeField] private Renderer[] _renderers;

        private Coroutine _walkCoroutine;

        public static event Action OnSoldierDeath;

        private bool isDead;
        public Transform UnitTransform { get; set; }
        public Collider Collider => _collider;
        public bool IsDead { get; set; }

        private Collider _collider;

        private PlayerType _playerType;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _collider = GetComponent<Collider>();
        }

        private void OnEnable()
        {
            if (_navMeshAgent == null)
                _navMeshAgent = GetComponent<NavMeshAgent>();

            UnitTransform = transform;

        }

        public void AssignToArmy(ArmyManager army)
        {
            _currentArmy = army;
        }

        public void SoldierSetup(PlayerType playerType, Material material, int soldierLevel)
        {
            var playerLayer = LayerMask.NameToLayer("Friend_Soldier");
            var aiLayer = LayerMask.NameToLayer("Enemy_Soldier");
            _enemySoldierLayer &= ~(1 << playerLayer);
            _enemySoldierLayer &= ~(1 << aiLayer);

            _playerType = playerType;
//            _healthBar.transform.parent.gameObject.SetActive(true);
            _healthBar.color = material.color;
            IsDead = false;
            _currentTarget = null;
            StopAllCoroutines();

            switch (playerType)
            {
                case PlayerType.Human:
                    gameObject.layer = playerLayer;
                    _enemySoldierLayer |= 1 << aiLayer;
                    break;
                case PlayerType.AI:
                    gameObject.layer = aiLayer;
                    _enemySoldierLayer |= 1 << playerLayer;
                    break;
            }

            foreach (var meshRenderer in _renderers)
                meshRenderer.sharedMaterial = material;

            _currentHealth = _soldierProperties.MaxHealth + soldierLevel * .1f;
            _attackPower = _soldierProperties.AttackPower + soldierLevel * .2f;
            _attackSpeed = _soldierProperties.AttackSpeed + soldierLevel * .15f;
            _enemyDetectDistance = _soldierProperties.EnemyDetectDistance;
            _attackRange = _soldierProperties.AttackRange;

            _navMeshAgent.speed = _soldierProperties.MovementSpeed;

            _soldierState = SoldierState.Idle;
        }

        public void Move(Vector3 destination)
        {
            if (_soldierState != SoldierState.Moving)
                _animator.SetTrigger(WalkAnimationString);
            _soldierState = SoldierState.Moving;
            _navMeshAgent.destination = destination;

            if (_walkCoroutine != null)
                StopCoroutine(_walkCoroutine);
            _walkCoroutine = StartCoroutine(CO_CheckReachedDestination());
        }

        public void ApproachForAttacking(IAttackable target)
        {
            _currentTarget = target;
            _soldierState = SoldierState.InCombat;
            StartCoroutine(CO_AttackingEnemy());
        }

        private IEnumerator CO_AttackingEnemy()
        {
            var colliderOfTarget = _currentTarget.Collider;
            while (!_currentTarget.IsDead)
            {
                var closestPoint = colliderOfTarget.ClosestPoint(transform.position);
                var attackPosition = closestPoint + (transform.position - closestPoint).normalized * _attackRange;
                while (Vector3.Distance(transform.position, attackPosition) >= _navMeshAgent.stoppingDistance)
                {
                    closestPoint = colliderOfTarget.ClosestPoint(transform.position);
                    attackPosition = closestPoint + (transform.position - closestPoint).normalized * _attackRange;
                    Move(attackPosition);
                    yield return null;
                }

                _soldierState = SoldierState.InCombat;

                Attack();
                var nextAttackTime = 1 / _attackSpeed;
                yield return new WaitForSeconds(nextAttackTime);
            }

            _currentArmy.AddSoldierToWalk(this);
            SetIdle();
        }

        private IEnumerator CO_CheckReachedDestination()
        {
            yield return null;
            while (_navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance)
                yield return null;
            SetIdle();
        }

        private void SetIdle()
        {
            if (_soldierState != SoldierState.Idle)
                _animator.SetTrigger(IdleAnimationString);
            _soldierState = SoldierState.Idle;
        }

        protected virtual void Attack()
        {
            _animator.SetTrigger(AttackAnimationString);
        }

        protected void DealDamage()
        {
            _currentTarget.GetHit(_attackPower);
        }

        public IAttackable CheckForEnemies()
        {
            var enemyCount = Physics.OverlapSphereNonAlloc(transform.position, _enemyDetectDistance, _enemyBuffer, _enemySoldierLayer);

            if (enemyCount == 0)
            {
                return null;
            }

            InsertionSort(_enemyBuffer, enemyCount);
            return _enemyBuffer[0].GetComponent<IAttackable>();
        }


        public void GetHit(float damage)
        {
            _currentHealth -= damage;
            var fillAmount = _currentHealth / _soldierProperties.MaxHealth;
            _healthBar.fillAmount = fillAmount;

            if (_currentHealth <= 0)
                Die();
        }

        protected virtual void Die()
        {
            if (IsDead) return;
            IsDead = true;
            _animator.SetTrigger(DieAnimationString);
            _currentArmy.ReleaseSoldier(this);
            _healthBar.transform.parent.gameObject.SetActive(false);

            StopAllCoroutines();
            StartCoroutine(CO_ReleaseAfterDeath());
            if (_playerType == PlayerType.AI)
                OnSoldierDeath?.Invoke();
        }

        private IEnumerator CO_ReleaseAfterDeath()
        {
            yield return new WaitForSeconds(2f);
            ObjectPooling.Instance.Release(this, _soldierProperties.SoldierType);
        }

        private void InsertionSort(Collider[] array, int length)
        {
            for (var i = 0; i < length; i++)
            {
                var key = array[i];
                var keyDistance = (transform.position - key.transform.position).sqrMagnitude;
                var j = i - 1;


                while (j >= 0 && (transform.position - array[j].transform.position).sqrMagnitude > keyDistance)
                {
                    array[j + 1] = array[j];
                    j--;
                }

                array[j + 1] = key;
            }
        }
    }
}
