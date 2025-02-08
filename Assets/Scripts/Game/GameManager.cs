using System;
using OneTapArmyCase.Enums;
using OneTapArmyCase.System;
using UnityEngine;

namespace OneTapArmyCase.Game
{
    public class GameManager : Singleton<GameManager>
    {
        private GameState _gameState;
        public GameState GameState => _gameState;
        
        public static event Action OnGameStart; 

        private void Start()
        {
            StartTheGame();
        }

        public void StartTheGame()
        {
            _gameState = GameState.Playing;
            SelectCard();
            OnGameStart?.Invoke();
        }

        public void SelectCard()
        {
            PauseTheGame();
        }

        public void CardSelected()
        {
            ContinueTheGame();
        }
        
        private void PauseTheGame()
        {
            _gameState = GameState.Idle;
            Time.timeScale = 0.0f;
        }
        
        private void ContinueTheGame()
        {
            _gameState = GameState.Playing;
            Time.timeScale = 1.0f;
        }
    }
}
