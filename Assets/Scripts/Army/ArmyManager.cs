using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OneTapArmyCase.Enums;
using OneTapArmyCase.Game;
using OneTapArmyCase.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OneTapArmyCase.Army
{
    public class ArmyManager : MonoBehaviour
    {
        [SerializeField] private float _armyIterationFrequencyInSeconds;
        [SerializeField] private float _formationSpacing;
        [SerializeField] private int _maxColumnSize;

        [SerializeField] private PlayerType _playerType;

        private readonly List<Soldier> _soldiers = new();
        private readonly List<Soldier> _soldiersToWalk = new();
        public PlayerType PlayerType => _playerType;

        public int SoldierCount => _soldiers.Count;

        [SerializeField] private Transform _playerCastleForAI;

        private void Start()
        {
            StartCoroutine(CO_IterateArmy());

            if (_playerType == PlayerType.AI)
                StartCoroutine(CO_AIGameplay());
        }

        private IEnumerator CO_AIGameplay()
        {
            while (true)
            {
                var nextRandomMoveTime = Random.Range(5f, 8f);
                yield return new WaitForSeconds(nextRandomMoveTime);
                MoveArmy(_playerCastleForAI.position);
            }
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
            _soldiersToWalk.Add(soldier);
            soldier.AssignToArmy(this);
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

                    if (nearestEnemy != null)
                    {
                        soldier.ApproachForAttacking(nearestEnemy);
                        _soldiersToWalk.Remove(soldier);
                    }
                }
                yield return new WaitForSeconds(_armyIterationFrequencyInSeconds);
            }
        }

        private void MoveArmy(Vector3 targetDestination)
        {
            var totalPlaced = 0;
            var availableSoldiers = _soldiers.Where(s => s.SoldierState != SoldierState.InCombat).Count();

            for (var i = 0; i < _soldiers.Count; i++)
            {
                if (_soldiers[i] == null)
                    continue;

                var soldier = _soldiers[i];
                if (soldier.SoldierState == SoldierState.InCombat)
                    continue;

                var row = totalPlaced / _maxColumnSize;
                var colInRow = totalPlaced % _maxColumnSize;

                int remainingSoldiers = availableSoldiers - (row * _maxColumnSize);
                var soldiersInRow = Mathf.Min(_maxColumnSize, remainingSoldiers);
                var rowWidth = (soldiersInRow - 1) * _formationSpacing;

                var xPos = colInRow * _formationSpacing - rowWidth / 2.0f;
                var zPos = -row * _formationSpacing;

                var soldierPosition = new Vector3(xPos, 0, zPos) + targetDestination;

                soldier.Move(soldierPosition);

                totalPlaced++;
            }

        }

        public void ReleaseSoldier(Soldier soldier)
        {
            _soldiers.Remove(soldier);
        }

        public void AddSoldierToWalk(Soldier soldier)
        {
            _soldiersToWalk.Add(soldier);
        }
    }
}
