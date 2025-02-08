using UnityEngine;

namespace OneTapArmyCase.Army
{
    public class ArcherUnit : Soldier
    {
        protected override void Attack()
        {
            Debug.Log("archer attack");
        }
    }
}
