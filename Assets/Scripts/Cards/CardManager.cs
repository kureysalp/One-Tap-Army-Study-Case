using System;
using System.Collections.Generic;
using OneTapArmyCase.Army;
using OneTapArmyCase.Game;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace OneTapArmyCase.Cards
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private SoldierSelectionCard[] _soldierSelectionCards;

        [SerializeField] private SoldierProperties[] _allSoldiers;

        [SerializeField] private GameObject _cardsParentObject;

        [SerializeField] private Image _cardChoiceTextImage;
        [SerializeField] private Sprite _firstChoiceSprite;
        [SerializeField] private Sprite _nextChoiceSprite;

        [SerializeField] private GameConfig _gameConfig;


        private bool _offeredBefore;

        private void OnEnable()
        {
            Castle.OnCastleLevelUp += ShowNewCards;
        }

        private void OnDisable()
        {
            Castle.OnCastleLevelUp -= ShowNewCards;
        }

        private void ShowNewCards(CastleConfig castleConfig)
        {
            _cardChoiceTextImage.sprite = _offeredBefore ? _nextChoiceSprite : _firstChoiceSprite;
            var offeredSoldiers = new List<SoldierProperties>();
            var offeredCastle = false;

            foreach (var soldierSelectionCard in _soldierSelectionCards)
            {
                if (!offeredCastle && _offeredBefore)
                {
                    if (Random.Range(0, 100) < 50)
                    {
                        offeredCastle = true;
                        soldierSelectionCard.SetCastleCard(castleConfig, _gameConfig, this);
                        continue;
                    }
                }

                SoldierProperties randomSoldier;
                do
                {
                    randomSoldier = _allSoldiers[Random.Range(0, _allSoldiers.Length)];
                } while (offeredSoldiers.Contains(randomSoldier));

                offeredSoldiers.Add(randomSoldier);

                var newSoldierToAdd = new SoldierConfig(1, randomSoldier.SoldierType);

                foreach (var soldierInCastle in castleConfig.Soldiers)
                {
                    if (soldierInCastle.SoldierType == randomSoldier.SoldierType)
                    {
                        soldierInCastle.SetLevel(soldierInCastle.Level + 1);
                        newSoldierToAdd = soldierInCastle;
                    }
                }

                soldierSelectionCard.SetTheCard(newSoldierToAdd, randomSoldier, this);
            }

            _cardsParentObject.SetActive(true);
            _offeredBefore = true;
        }

        public void HideAllCards()
        {
            _cardsParentObject.SetActive(false);
            GameManager.Instance.CardSelected();
        }
    }
}
