using OneTapArmyCase.Enums;
using UnityEngine;

namespace OneTapArmyCase.Army
{
    [CreateAssetMenu(fileName = "SO_Soldier_Properties", menuName = "Scriptable Objects/Soldier Properties", order = 0)]
    public class SoldierProperties : ScriptableObject
    {
        [SerializeField] private float _maxHealth;
        [SerializeField] private float _attackPower;
        [SerializeField] private float _attackSpeed;
        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _enemyDetectDistance;
        [SerializeField] private float _attackRange;
        
        [SerializeField] private Sprite[] _soldierSprites;
        
        [SerializeField] private SoldierType _soldierType;
        
        public float MaxHealth => _maxHealth;
        public float AttackPower => _attackPower;
        public float AttackSpeed => _attackSpeed;
        public float MovementSpeed => _movementSpeed;
        public float EnemyDetectDistance => _enemyDetectDistance;
        public float AttackRange => _attackRange;
        public Sprite[] SoldierSprites => _soldierSprites;
        public SoldierType SoldierType => _soldierType;
    }
}
