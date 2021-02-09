using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    #region Fields
    [SerializeField] private Vector2 _hitBox;
    [SerializeField] private Transform _topBucket, _middleBucket, _bottomBucket;

    private Vector2 _screenBounds;
    private float _speed;
    private float _movement;

    private Coroutine _movementCoroutine;
    private Transform _activeBucket;
    #endregion

    #region Accessors
    public Vector2 ScreenBounds
    {
        get => _screenBounds;
        set
        {
            if (_screenBounds != value)
                _screenBounds = value;
        }
    }

    public float Speed
    {
        get => _speed;
        set
        {
            if (_speed != value)
                _speed = value;
        }
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, _hitBox);
    }

    /// <summary>
    /// Обработчик события переключения состояния игры
    /// </summary>
    public void HandleGameStateChange(object sender, GameController.GameState state)
    {
        switch (state)
        {
            case GameController.GameState.Start:

                if(_movementCoroutine == null)
                    _movementCoroutine = StartCoroutine(MovementCoroutine());

                break;

            case GameController.GameState.WaveFailed:

                Factory_SFX.PlaySFX(Factory_SFX.SFX_BigBoom);
                Factory_Particle.CreateParticleSystem(Factory_Particle.BucketDrop, _activeBucket);
                break;

            case GameController.GameState.GameOver:

                Factory_SFX.PlaySFX(Factory_SFX.SFX_GameOver);
                Factory_Particle.CreateParticleSystem(Factory_Particle.BucketDrop, _activeBucket);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Обработчик событий пользовательского ввода, вызываемых через InputManager
    /// </summary>
    public void HandleMovement(float movement)
    {
        _movement = movement;
    }

    /// <summary>
    /// Обработка событий изменения количества жизней
    /// </summary>
    public void HandleLivesChange(object sender, int lives)
    {
        UpdateBuckets(lives);
    }

    /// <summary>
    /// Сопрограмма перемещения игрока
    /// </summary>
    private IEnumerator MovementCoroutine()
    {
        Vector3 clampedPosition;

        while (true)
        {
            transform.Translate(new Vector3(_movement, 0f) * _speed * Time.deltaTime);

            clampedPosition = transform.position;

            clampedPosition.x = Mathf.Clamp(clampedPosition.x,
                _screenBounds.x * -1 + _hitBox.x / 2,
                _screenBounds.x - _hitBox.x / 2);

            transform.position = clampedPosition;

            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Метод обновления визуального отображения объекта игрока
    /// </summary>
    private void UpdateBuckets(int lives)
    {
        switch (lives)
        {
            case 3:
                _activeBucket = _bottomBucket;
                SwitchBucketState(true, true, true);
                break;

            case 2:
                _activeBucket = _middleBucket;
                SwitchBucketState(false, true, true);
                break;

            case 1:
                _activeBucket = _topBucket;
                SwitchBucketState(false, false, true);
                break;

            case 0:
                SwitchBucketState(false, false, false);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Метод переключения активных вложенных объектов
    /// </summary>
    private void SwitchBucketState(bool bottomBucketState, bool middleBucketState, bool topBucketState)
    {
        _bottomBucket.gameObject.SetActive(bottomBucketState);
        _middleBucket.gameObject.SetActive(middleBucketState);
        _topBucket.gameObject.SetActive(topBucketState);
    }
}