using System;
using System.Collections;
using System.Collections.Generic;
using OneTapArmyCase.Army;
using OneTapArmyCase.Cards;
using OneTapArmyCase.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
                }
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

        public int Level => _level;
        public SoldierType SoldierType => _soldierType;
    }

    public class Castle : MonoBehaviour, IAttackable
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private GameObject[] _levelledCastleObjects;

        private CastleConfig _castleConfig;

        [SerializeField] private float _baseHealth;
        [SerializeField] private float _healthPerLevel;

        private float MaxHealth => _baseHealth + _healthPerLevel * _playerLevel;

        private float _currentHealth;
        private int _currentExp;
        private int _soldierGenerateOrder;

        [SerializeField] private float _soldierGenerateRate;
        [SerializeField] private ArmyManager _armyManager;
        [SerializeField] private Material _castleMaterial;
        [SerializeField] private Material _soldierMaterial;

        [SerializeField] private Image _soldierSpawnTimerBar;
        [SerializeField] private Image _healthBar;

        [SerializeField] private Image _expProgressBar;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Transform _levelTextHolder;
        [SerializeField] private Transform _levelHolderStart;
        [SerializeField] private Transform _levelHolderEnd;

        [SerializeField] private ParticleSystem _levelUpParticle;
        [SerializeField] private TextMeshProUGUI _expText;

        public Transform UnitTransform { get; set; }
        public Collider Collider => _collider;
        public bool IsDead { get; set; }

        private Collider _collider;

        private int _expToLevelUp;
        private int _playerLevel = 1;
        public static event Action<CastleConfig> OnCastleLevelUp;

        private void Awake()
        {
            GameManager.OnGameStart += StartTheGame;
            _collider = GetComponent<Collider>();
            _currentHealth = _baseHealth;
        }


        private void OnEnable()
        {
            SoldierSelectionCard.OnSoldierCardSelected += AddSoldierToTheCastle;
            UnitTransform = transform;
            if (_armyManager.PlayerType == PlayerType.Human)
            {
                Soldier.OnSoldierDeath += AddExp;
                SoldierSelectionCard.OnCastleCardSelected += LevelUpCastle;
            }
        }

        private void OnDisable()
        {
            SoldierSelectionCard.OnSoldierCardSelected += AddSoldierToTheCastle;

            if (_armyManager.PlayerType == PlayerType.Human)
            {
                Soldier.OnSoldierDeath -= AddExp;
                SoldierSelectionCard.OnCastleCardSelected -= LevelUpCastle;
            }

        }

        private void OnDestroy()
        {
            GameManager.OnGameStart -= StartTheGame;
        }

        private void StartTheGame()
        {
            _castleConfig = new CastleConfig();

            _healthBar.color = _soldierMaterial.color;
            LevelUp();
            StartCoroutine(CO_GenerateSoldier());
        }


        private IEnumerator CO_GenerateSoldier()
        {
            yield return new WaitForSeconds(.2f);
            while (GameManager.Instance.GameState == GameState.Playing)
            {

                var spawnTime = 1 / _soldierGenerateRate;
                var timer = 0f;

                if (_armyManager.SoldierCount == _gameConfig.MaxNumberOfSoldiersPerArmy)
                {
                    yield return null;
                    continue;
                }

                _soldierSpawnTimerBar.fillAmount = 0;
                while (timer < spawnTime)
                {
                    timer += Time.deltaTime;
                    _soldierSpawnTimerBar.fillAmount = timer / spawnTime;
                    yield return null;
                }

                if (_castleConfig.Soldiers.Count == 0)
                    continue;

                var nextSoldierTypeToGenerate = _castleConfig.Soldiers[_soldierGenerateOrder];

                var soldier = ObjectPooling.Instance.GetFromPool(nextSoldierTypeToGenerate.SoldierType);
                var randomVector = Random.insideUnitSphere * 2;
                randomVector.y = 0;
                soldier.transform.position = _spawnPoint.position + randomVector;

                _armyManager.AddSoldier(soldier);
                soldier.SoldierSetup(_armyManager.PlayerType, _soldierMaterial, nextSoldierTypeToGenerate.Level);
                soldier.gameObject.SetActive(true);

                _soldierGenerateOrder++;
                if (_soldierGenerateOrder == _castleConfig.Soldiers.Count)
                    _soldierGenerateOrder = 0;
            }
        }

        private void AddExp()
        {
            if (_playerLevel == _gameConfig.MaxLevel) return;
            _currentExp += _gameConfig.ExpPerKill;

            SetExpBarFill();

            if (_currentExp >= _expToLevelUp)
                LevelUp();
        }

        private void SetExpBarFill()
        {
            var fillAmount = Mathf.Clamp01((float)_currentExp / _expToLevelUp);
            _expProgressBar.fillAmount = fillAmount;

            var levelHolderPosition = Vector2.Lerp(_levelHolderStart.position, _levelHolderEnd.position, fillAmount);
            _levelTextHolder.position = levelHolderPosition;

            _expText.SetText($"XP {_currentExp}/{_expToLevelUp}");
        }

        private void LevelUp()
        {
            if (_armyManager.PlayerType != PlayerType.Human) return;
            _castleConfig.LevelUp();

            _levelText.SetText(_playerLevel.ToString());
            _levelTextHolder.position = _levelHolderStart.position;
            _currentExp = 0;
            SetExpBarFill();

            SetCastleLevel();
            OnCastleLevelUp?.Invoke(_castleConfig);
            GameManager.Instance.SelectCard();
        }

        private void SetCastleLevel()
        {
            _expToLevelUp = _gameConfig.ExpToLevelUp + _gameConfig.ExpIncreasePerLevel * _playerLevel;
            _playerLevel++;
        }

        private void AddSoldierToTheCastle(SoldierConfig soldierConfig)
        {
            if (_armyManager.PlayerType == PlayerType.Human)
                _levelUpParticle.Play();
            _castleConfig.AddSoldier(soldierConfig);
        }


        public void GetHit(float damage)
        {
            _currentHealth -= damage;
            var fillAmount = _currentHealth / MaxHealth;
            _healthBar.fillAmount = fillAmount;

            if (_currentHealth <= 0)
                DestroyTheCastle();
        }

        private void DestroyTheCastle()
        {
            IsDead = true;
            if (_armyManager.PlayerType == PlayerType.Human)
                GameManager.Instance.LoseTheGame();
            else
                GameManager.Instance.WinTheGame();
        }

        private void LevelUpCastle()
        {
            _levelUpParticle.Play();
            _castleConfig.LevelUp();
            var castleIndex = _castleConfig.Level / _gameConfig.CastleChangeLevelFrequency;

            foreach (var castleObject in _levelledCastleObjects)
                castleObject.SetActive(false);

            _levelledCastleObjects[castleIndex].SetActive(true);
        }
    }
}
