using System;
using OneTapArmyCase.Army;
using OneTapArmyCase.Game;
using UnityEngine;
using UnityEngine.UI;

namespace OneTapArmyCase.Cards
{
    public class SoldierSelectionCard : MonoBehaviour
    {
        [SerializeField] private Image _soldierImage;
        [SerializeField] private Image _unitNameImage;
        [SerializeField] private Image _cardBgImage;

        [SerializeField] private GameObject[] _levelStars;

        [SerializeField] private GameObject _levelUpStats;

        private SoldierConfig _soldierConfig;

        private Button _button;

        public static event Action<SoldierConfig> OnSoldierCardSelected;
        public static event Action OnCastleCardSelected;

        private CardManager _cardManager;


        public void SetTheCard(SoldierConfig soldierConfig, SoldierProperties soldierProperties, CardManager cardManager)
        {
            if (_button == null)
                _button = GetComponent<Button>();

            _button.onClick.RemoveAllListeners();
            _soldierConfig = soldierConfig;
            _soldierImage.sprite = soldierProperties.SoldierSprites[soldierConfig.Level - 1];
            _unitNameImage.sprite = soldierProperties.SoldierName;
            _cardBgImage.sprite = soldierProperties.SoldierCardBg;
            _cardManager = cardManager;

            _levelUpStats.SetActive(_soldierConfig.Level > 1);

            for (var i = 0; i < _levelStars.Length; i++)
                _levelStars[i].SetActive(soldierConfig.Level > i);

            _button.onClick.AddListener(SelectTheSoldierCard);
        }

        public void SetCastleCard(CastleConfig castleConfig, GameConfig gameConfig, CardManager cardManager)
        {
            if (_button == null)
                _button = GetComponent<Button>();

            _button.onClick.RemoveAllListeners();
            _soldierImage.sprite = gameConfig.CastleSprites[castleConfig.Level - 1];
            _unitNameImage.sprite = gameConfig.CastleUpgradeTextSprite;
            _cardBgImage.sprite = gameConfig.CastleUpgradeBgSprite;
            _cardManager = cardManager;

            _levelUpStats.SetActive(castleConfig.Level > 1);

            for (var i = 0; i < _levelStars.Length; i++)
                _levelStars[i].SetActive(castleConfig.Level > i);

            _button.onClick.AddListener(SelectCastleTheCard);
        }

        private void SelectTheSoldierCard()
        {
            OnSoldierCardSelected?.Invoke(_soldierConfig);
            _cardManager.HideAllCards();
        }

        private void SelectCastleTheCard()
        {
            OnCastleCardSelected?.Invoke();
            _cardManager.HideAllCards();
        }
    }
}
