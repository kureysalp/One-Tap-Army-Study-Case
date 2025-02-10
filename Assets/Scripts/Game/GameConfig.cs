using UnityEngine;

namespace OneTapArmyCase.Game
{
    [CreateAssetMenu(fileName = "SO_Game_Config", menuName = "Scriptable Objects/Game Config", order = 0)]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private int _expToLevelUp;
        [SerializeField] private int _expIncreasePerLevel;
        [SerializeField] private int _expPerKill;
        [SerializeField] private int _maxLevel;
        [SerializeField] private int _castleChangeLevelFrequency;
        [SerializeField] private int _maxNumberOfSoldiersPerArmy;

        [SerializeField] private Sprite[] _castleSprites;
        [SerializeField] private Sprite _castleUpgradeTextSprite;
        [SerializeField] private Sprite _castleUpgradeBgSprite;

        public int ExpToLevelUp => _expToLevelUp;
        public int ExpIncreasePerLevel => _expIncreasePerLevel;
        public int ExpPerKill => _expPerKill;
        public int MaxLevel => _maxLevel;
        public int CastleChangeLevelFrequency => _castleChangeLevelFrequency;
        public int MaxNumberOfSoldiersPerArmy => _maxNumberOfSoldiersPerArmy;

        public Sprite[] CastleSprites => _castleSprites;
        public Sprite CastleUpgradeTextSprite => _castleUpgradeTextSprite;
        public Sprite CastleUpgradeBgSprite => _castleUpgradeBgSprite;
    }
}
