using System;
using System.Collections;
using System.Collections.Generic;
using OneTapArmyCase.Enums;
using OneTapArmyCase.System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneTapArmyCase.Game
{
    public class GameManager : Singleton<GameManager>
    {
        private GameState _gameState;
        public GameState GameState => _gameState;

        [SerializeField] private GameObject _winScreen;
        [SerializeField] private GameObject _loseScreen;

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

        public void WinTheGame()
        {
            PauseTheGame();
            _gameState = GameState.End;
            _winScreen.SetActive(true);
        }

        public void LoseTheGame()
        {
            PauseTheGame();
            _gameState = GameState.End;
            _loseScreen.SetActive(true);
        }

        public void Restart()
        {
            if (_gameState != GameState.End) return;

            _gameState = GameState.Idle;
            StartCoroutine(LoadSceneAsync());
        }

        private IEnumerator LoadSceneAsync()
        {
            var sceneLoadProcess = SceneManager.LoadSceneAsync(0);

            while (sceneLoadProcess is { isDone: false })
            {
                yield return null;
            }

            if (sceneLoadProcess != null) sceneLoadProcess.allowSceneActivation = true;
        }
    }
}
