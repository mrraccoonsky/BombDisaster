using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] private Camera _boundCamera;
    [SerializeField] private Camera _renderCamera;

    [SerializeField] private Canvas _canvas;
    [SerializeField] private List<LevelData> _levelData;

    private GameController _gameController;

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        if (_levelData.Count > 0)
            _gameController = new GameController(_canvas, _boundCamera, _renderCamera, _levelData);

        else
            Debug.LogWarning("Can't create a game: no LevelData assigned!");
    }
}