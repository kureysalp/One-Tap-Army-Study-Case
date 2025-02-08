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
        
        private SoldierConfig _soldierConfig;
        
        private Button _button;
        
        public static event Action<SoldierConfig> OnSoldierCardSelected;
        
        private CardManager _cardManager;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(SelectTheCard);
        }
        
        public void SetTheCard(SoldierConfig soldierConfig, SoldierProperties soldierProperties, CardManager cardManager)
        {
            _soldierConfig = soldierConfig;
            _soldierImage.sprite = soldierProperties.SoldierSprites[soldierConfig.Level - 1];
            _cardManager = cardManager;
        }

        private void SelectTheCard()
        {
            OnSoldierCardSelected?.Invoke(_soldierConfig);
            _cardManager.HideAllCards();
        }
    }
}
