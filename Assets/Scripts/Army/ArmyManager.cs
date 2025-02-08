using System;
using System.Collections;
using System.Collections.Generic;
using OneTapArmyCase.Enums;
using OneTapArmyCase.Game;
using OneTapArmyCase.Player;
using UnityEngine;

namespace OneTapArmyCase.Army
{
    public class ArmyManager : MonoBehaviour
    {
        [SerializeField] private float _armyIterationFrequencyInSeconds;
        [SerializeField] private float _formationSpacing;
        [SerializeField] private int _maxColumnSize;
        
        [SerializeField] private PlayerType _playerType;
        
        private readonly List<Soldier> _soldiers = new();
        public PlayerType PlayerType => _playerType;

        private void Start()
        {
            StartCoroutine(CO_IterateArmy());
        }

        private void OnEnable()
        {
            if (_playerType == PlayerType.Human)
                UserInput.OnPlayerTapMove += MoveArmy;

        }
        
        private void OnDisable()
        {
            if (_playerType == PlayerType.Human)
                UserInput.OnPlayerTapMove -= MoveArmy;
            
        }

        public void AddSoldier(Soldier soldier)
        {
            _soldiers.Add(soldier);
            soldier.AssignToArmy(this);
        }

        private IEnumerator CO_IterateArmy()
        {
            if (_playerType == PlayerType.AI) yield break;
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

        private void MoveArmy(Vector3 targetDestination)
        {
            for (var i = 0; i < _soldiers.Count; i++)
            {
                var soldier = _soldiers[i];
                if (soldier == null) continue;
                if (soldier.SoldierState == SoldierState.InCombat) continue;

                var columnIndex = i / _maxColumnSize;
                var soldierPositionIndex = (_soldiers.Count % _maxColumnSize)  / 2;
                
                var startPosition = targetDestination - soldierPositionIndex * _formationSpacing * Vector3.right; 
                var horizontalPosition = _formationSpacing * (i  % _maxColumnSize) * Vector3.right;
                var verticalPosition = _formationSpacing * columnIndex  * Vector3.back;
                var nextSoldierDestination = startPosition + horizontalPosition + verticalPosition;
                
                soldier.Move(nextSoldierDestination);
            }
        }

        public void ReleaseSoldier(Soldier soldier)
        {
            _soldiers.Remove(soldier);
        }
    }
}
