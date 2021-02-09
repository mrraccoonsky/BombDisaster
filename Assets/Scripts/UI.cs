using System;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    #region Fields
    [SerializeField] private Text _score;

    [SerializeField] private CanvasGroup _textPanel;
    [SerializeField] private Text _textPanelHeader, _textPanelText;

    [SerializeField] private String _instructionsHeader, _pauseHeader, _waveFailedHeader, _gameOverHeader;
    [SerializeField] [TextArea] private String _instructionsText, _pauseText, _waveFailedText, _gameOverText;
    #endregion

    private void Awake()
    {
        SetPanelText(_instructionsHeader, _instructionsText);
        ShowPanel(true);
    }

    /// <summary>
    /// Обработчик события переключения состояния игры
    /// </summary>
    public void HandleGameStateChange(object sender, GameController.GameState state)
    {
        switch (state)
        {
            case GameController.GameState.Start:

                ShowPanel(false);
                break;

            case GameController.GameState.Pause:

                SetPanelText(_pauseHeader, _pauseText);
                ShowPanel(true);
                break;

            case GameController.GameState.Resume:

                ShowPanel(false);
                break;

            case GameController.GameState.WaveFailed:

                SetPanelText(_waveFailedHeader, _waveFailedText);
                ShowPanel(true);
                break;

            case GameController.GameState.GameOver:

                SetPanelText(_gameOverHeader, _gameOverText);
                ShowPanel(true);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Обработчик события изменения игрового счета
    /// </summary>
    public void HandleScoreChange(object sender, int score)
    {
        _score.text = $"{score}";
    }

    /// <summary>
    /// Метод отображения панели информации
    /// </summary>
    private void ShowPanel(bool state)
    {
        if (state)
            _textPanel.alpha = 1;

        else
            _textPanel.alpha = 0;
    }

    /// <summary>
    /// Метод установки необходимого текста в панель информации
    /// </summary>
    private void SetPanelText(string header, string text)
    {
        _textPanelHeader.text = header;
        _textPanelText.text = text;
    }
}
