using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController
{
    public enum GameState
    {
        Start, Pause, Resume, WaveFailed, GameOver
    }

    #region Events
    public EventHandler<GameState> OnGameStateChange;
    public EventHandler<int> OnScoreChange;
    public EventHandler<int> OnLivesChange;
    #endregion

    #region Fields
    private readonly Canvas _canvas;
    private readonly Camera _renderCamera;
    private readonly Camera _boundCamera;
    private readonly List<LevelData> _levelData;

    private InputSystem _inputSystem;
    private UI _ui;
    private Level _level;
    private Player _player;
    private Enemy _enemy;
    private BombSpawner _bombSpawner;

    private Vector2 _screenBounds;

    private LevelData _currentLevel;
    private int _currentLevelNumber;
    private int _lives;
    private int _score;
    private int _nextLiveScore;

    private bool _lastStartState;
    #endregion

    public GameController(Canvas canvas, Camera boundCamera, Camera renderCamera, List<LevelData> levelData)
    {
        _canvas = canvas;
        _boundCamera = boundCamera;
        _renderCamera = renderCamera;
        _levelData = levelData;

        CreateGame();
    }

    public void CreateGame()
    {
        _screenBounds = GetScreenBounds(_boundCamera);

        #region Creating game elements
        _inputSystem = new InputSystem();
        _ui = Factory_Game.CreateUI(_canvas);
        _level = Factory_Game.CreateLevel();
        _player = Factory_Game.CreatePlayer(_level.PlayerSpawn, _screenBounds);
        _enemy = Factory_Game.CreateEnemy(_level.EnemySpawn, _screenBounds);
        _bombSpawner = Factory_Game.CreateBombSpawner(_enemy.DropOrigin);
        #endregion

        #region Signing game events
        OnGameStateChange += _ui.HandleGameStateChange;
        OnGameStateChange += _player.HandleGameStateChange;
        OnGameStateChange += _enemy.HandleGameStateChange;
        OnGameStateChange += _bombSpawner.HandleGameStateChange;

        OnScoreChange += _ui.HandleScoreChange;
        OnLivesChange += _player.HandleLivesChange;

        _inputSystem.Game.Start.performed += ctx => StartGame();
        _inputSystem.Game.Pause.performed += ctx => PauseGame();
        _inputSystem.Game.Quit.performed += ctx => QuitGame();

        _inputSystem.Player.Movement.performed += ctx => _player.HandleMovement(ctx.ReadValue<float>());
        _inputSystem.Player.Movement.canceled += ctx => _player.HandleMovement(0f);

        _bombSpawner.OnSpawnerEvent += HandleSpawnerEvent;
        _bombSpawner.OnSpawnerEvent += _enemy.HandleSpawnerEvent;
        #endregion

        _inputSystem.Game.Start.Enable();
        _inputSystem.Game.Pause.Disable();
        _inputSystem.Player.Enable();
    }

    private void StartGame()
    {
        if (_lives < 1)
            ResetGame();

        UpdateLevelData();

        _inputSystem.Game.Quit.Disable();
        _inputSystem.Game.Start.Disable();
        _inputSystem.Game.Pause.Enable();

        OnGameStateChange(this, GameState.Start);
    }

    private void PauseGame()
    {
        if (Time.timeScale > 0f)
        {
            _lastStartState = _inputSystem.Game.Start.enabled;

            Time.timeScale = 0f;

            _inputSystem.Game.Start.Disable();
            _inputSystem.Game.Quit.Enable();

            OnGameStateChange(this, GameState.Pause);
        }

        else
        {
            Time.timeScale = 1f;

            if(_lastStartState)
                _inputSystem.Game.Start.Enable();

            _inputSystem.Game.Quit.Disable();

            OnGameStateChange(this, GameState.Resume);
        }
    }

    private void ResetGame()
    {
        _player.transform.position = _level.PlayerSpawn.position;
        _enemy.transform.position = _level.EnemySpawn.position;

        _currentLevelNumber = 0;

        _lives = 3;
        OnLivesChange(this, _lives);

        _score = 0;
        OnScoreChange(this, _score);

        _nextLiveScore = 1000;
    }

    private void QuitGame()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    public void UpdateLevelData()
    {
        _currentLevel = _levelData[_currentLevelNumber];

        _player.Speed = _currentLevel.PlayerSpeed;
        _enemy.Speed = _currentLevel.EnemySpeed;

        _bombSpawner.BombCount = _currentLevel.BombCount;
        _bombSpawner.BombSpawnDelay = _currentLevel.BombSpawnDelay;
        _bombSpawner.BombSpeed = _currentLevel.BombSpeed;
    }

    private Vector2 GetScreenBounds(Camera boundCamera)
    {
        if (boundCamera.enabled == false)
            boundCamera.enabled = true;

        Vector2 screenBounds = boundCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        boundCamera.enabled = false;

        if (_renderCamera.enabled == false)
            _renderCamera.enabled = true;

        return screenBounds;
    }

    private void HandleSpawnerEvent(object sender, BombSpawner.SpawnerEvent spawnerEvent)
    {
        switch (spawnerEvent)
        {
            case BombSpawner.SpawnerEvent.Catch:

                _score += _currentLevel.BombScore;
                OnScoreChange(this, _score);

                if (_score >= _nextLiveScore)
                {
                    _nextLiveScore += 1000;
                    Factory_SFX.PlaySFX(Factory_SFX.SFX_1Up);

                    if (_lives < 3)
                    {
                        _lives++;
                        OnLivesChange(this, _lives);
                    }
                }

                break;

            case BombSpawner.SpawnerEvent.WaveCompleted:

                if (_currentLevelNumber < _levelData.Count - 1)
                    _currentLevelNumber++;

                _inputSystem.Game.Start.Enable();
                break;

            case BombSpawner.SpawnerEvent.WaveFailed:

                _lives--;

                if (_lives > 0)
                {
                    OnGameStateChange(this, GameState.WaveFailed);

                    if (_currentLevelNumber > 0)
                        _currentLevelNumber--;
                }

                else
                {
                    _inputSystem.Game.Quit.Enable();
                    OnGameStateChange(this, GameState.GameOver);
                }

                OnLivesChange(this, _lives);

                _inputSystem.Game.Pause.Disable();
                _inputSystem.Game.Start.Enable();
                break;

            default:
                break;
        }
    }
}
