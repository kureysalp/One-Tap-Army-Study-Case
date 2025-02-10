using UnityEngine;

namespace OneTapArmyCase.Game
{
    public interface IAttackable
    {
        public Transform UnitTransform { get; set; }
        public Collider Collider { get; }
        public bool IsDead { get; set; }
        public void GetHit(float damage);
    }
}
