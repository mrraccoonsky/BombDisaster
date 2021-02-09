using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Fields
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _dropOrigin;
    [SerializeField] private Vector2 _hitBox;

    private Vector2 _screenBounds;
    private float _speed;
    private int _movement;
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

    public Transform DropOrigin
    {
        get => _dropOrigin;
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

                _animator.SetBool("Win", false);
                StartCoroutine(MovementCoroutine());
                StartCoroutine(ChangeDirectionCoroutine());
                break;

            case GameController.GameState.WaveFailed:

                _animator.SetBool("Win", true);
                break;

            case GameController.GameState.GameOver:

                _animator.SetBool("Win", true);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Обработчик событий, вызываемых спавнером
    /// </summary>
    public void HandleSpawnerEvent(object sender, BombSpawner.SpawnerEvent spawnerEvent)
    {
        switch (spawnerEvent)
        {
            case BombSpawner.SpawnerEvent.Drop:

                _animator.SetTrigger("Drop");
                break;

            case BombSpawner.SpawnerEvent.LastDrop:

                StopAllCoroutines();
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Сопрограмма перемещения противника
    /// </summary>
    private IEnumerator MovementCoroutine()
    {
        Vector3 clampedPosition;

        while (true)
        {
            if (transform.position.x < -_screenBounds.x + _hitBox.x * 2 && _movement < 0)
                _movement = 1;

            else if (transform.position.x > _screenBounds.x - _hitBox.x * 2 && _movement > 0)
                _movement = -1;

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
    /// Сопрограмма смены направления движения противника
    /// </summary>
    private IEnumerator ChangeDirectionCoroutine()
    {
        while (true)
        {
            if (UnityEngine.Random.value > 0.5f)
                _movement = 1;

            else
                _movement = -1;

            float delay = UnityEngine.Random.Range(0.1f, 0.5f);
            yield return new WaitForSeconds(delay);
        }
    }
}