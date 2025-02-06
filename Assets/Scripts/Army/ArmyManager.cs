using System;
using System.Collections;
using System.Collections.Generic;
using OneTapArmyCase.Enums;
using UnityEngine;

namespace OnaTapArmyCase.Army
{
    public class ArmyManager : MonoBehaviour
    {
        [SerializeField] private float _armyIterationFrequencyInSeconds;
        
        public List<Soldier> _soldiers = new();

        private void Start()
        {
            StartCoroutine(CO_IterateArmy());
        }

        public void AddSoldier(Soldier soldier)
        {
            _soldiers.Add(soldier);
        }

        private IEnumerator CO_IterateArmy()
        {
            while (true)
            {
                for (var i = 0; i < _soldiers.Count; i++)
                {
                    var soldier = _soldiers[i];

                    if (soldier == null) continue;
                    if (soldier.SoldierState == SoldierState.InCombat) continue;

                    var nearestEnemy = _soldiers[i].CheckForEnemies();

                    if (nearestEnemy)
                        soldier.ApproachForAttacking(nearestEnemy);


                }
                yield return new WaitForSeconds(_armyIterationFrequencyInSeconds);
            }
        }

        private void MoveArmy(Vector3 moveDestination)
        {
            foreach (var soldier in _soldiers)
            {
                if (soldier == null) continue;
                if (soldier.SoldierState == SoldierState.InCombat) continue;
                
                soldier.Move(moveDestination);
            }
        }
    }
}
