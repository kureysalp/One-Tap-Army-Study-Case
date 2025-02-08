using System;
using System.Collections;
using System.Collections.Generic;
using OneTapArmyCase.Army;
using OneTapArmyCase.Cards;
using OneTapArmyCase.Enums;
using UnityEngine;

namespace OneTapArmyCase.Game
{
    
    public class CastleConfig
    {
        public int Level { get; private set; }
        private readonly List<SoldierConfig> _soldiers = new();
        
        public void AddSoldier(SoldierConfig soldier)
        {
            foreach (var soldierConfig in _soldiers)
            {
                if (soldierConfig.SoldierType == soldier.SoldierType)
                {
                    soldierConfig.SetLevel(soldier.Level);
                        return;
                };
            }
            
            _soldiers.Add(soldier);
        }

        public void LevelUp() => Level++;
        public List<SoldierConfig> Soldiers => _soldiers;

    }

    public class SoldierConfig
    {
        private int _level;
        private SoldierType _soldierType;
        public SoldierConfig(int level, SoldierType soldierType)
        {
            _level = level;
            _soldierType = soldierType;
        }

        public void SetLevel(int level)
        {
            _level = level;
        }
        
        public int Level=>_level;
        public SoldierType SoldierType => _soldierType;
    }
    
    public class Castle : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private GameObject[] _levelledCastleObjects;
        
        private CastleConfig _castleConfig;
        
        private float _currentHealth;
        private int _currentExp;
        private int _soldierGenerateOrder;

        [SerializeField] private float _soldierGenerateRate;
        [SerializeField] private ArmyManager _armyManager;
        [SerializeField] private Material _castleMaterial;
        [SerializeField] private Material _soldierMaterial;
        
        private Coroutine _soldierGenerateCoroutine;

        private int _expToLevelUp;

        public static event Action<CastleConfig> OnCastleLevelUp;

        private void Awake()
        {
            GameManager.OnGameStart += StartTheGame;
        }

        private void Start()
        {
            
        }

        private void OnEnable()
        {
            SoldierSelectionCard.OnSoldierCardSelected += AddSoldierToTheCastle;
        }
        
        private void OnDisable()
        {
            SoldierSelectionCard.OnSoldierCardSelected += AddSoldierToTheCastle;
        }

        private void StartTheGame()
        {
            _castleConfig = new CastleConfig();
            LevelUp();
            _soldierGenerateCoroutine = StartCoroutine(CO_GenerateSoldier());
            Debug.Log("game started");
        }


        private IEnumerator CO_GenerateSoldier()
        {
            yield return new WaitForSeconds(.5f);
            Debug.Log(GameManager.Instance.GameState);
            while (GameManager.Instance.GameState == GameState.Playing)
            {
                Debug.Log("generating soldiers");
                yield return new WaitForSeconds(1 / _soldierGenerateRate);
                
                if(_castleConfig.Soldiers.Count == 0)
                    continue;

                Debug.Log("generated");
                var soldier = ObjectPooling.Instance.SoldierPool.Get();
                soldier.transform.position = _spawnPoint.position;
                
                _armyManager.AddSoldier(soldier);
                soldier.SoldierSetup(_armyManager.PlayerType, _soldierMaterial);

                _soldierGenerateOrder++;
                if (_soldierGenerateOrder == _castleConfig.Soldiers.Count - 1)
                    _soldierGenerateOrder = 0;
            }
        }

        private void AddExp()
        {
            if (_castleConfig.Level == _gameConfig.MaxLevel) return;
            _currentExp += _gameConfig.ExpPerKill;

            if (_currentExp >= _expToLevelUp)
                LevelUp();
        }

        private void LevelUp()
        {
            _castleConfig.LevelUp();
            SetCastleLevel();
            OnCastleLevelUp?.Invoke(_castleConfig);
            GameManager.Instance.SelectCard();
        }

        private void SetCastleLevel()
        {
            _expToLevelUp = _gameConfig.ExpToLevelUp + _gameConfig.ExpIncreasePerLevel * _castleConfig.Level;
            var castleIndex = _castleConfig.Level / _gameConfig.CastleChangeLevelFrequency;

            foreach (var castleObject in _levelledCastleObjects)
                castleObject.SetActive(false);
            
            _levelledCastleObjects[castleIndex].SetActive(true);
        }

        private void AddSoldierToTheCastle(SoldierConfig soldierConfig)
        {
            _castleConfig.AddSoldier(soldierConfig);           
        }
    }
}
