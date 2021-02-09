using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    /// <summary>
    /// Перечисление возможных событий, вызываемых спавнером
    /// </summary>
    public enum SpawnerEvent
    {
        Drop, LastDrop, Catch, WaveCompleted, WaveFailed
    }

    #region Events
    public EventHandler<SpawnerEvent> OnSpawnerEvent = (sender, e) => { };
    #endregion

    #region Fields
    private int _bombCount;
    private float _bombSpawnDelay;
    private float _bombSpeed;

    private List<Bomb> _bombs = new List<Bomb>();
    private Bomb _lastBomb;

    private Coroutine _bombSpawnCoroutine;
    #endregion

    #region Accessors
    public int BombCount
    {
        get => _bombCount;
        set
        {
            if (_bombCount != value)
                _bombCount = value;
        }
    }

    public float BombSpawnDelay
    {
        get => _bombSpawnDelay;
        set
        {
            if (_bombSpawnDelay != value)
                _bombSpawnDelay = value;
        }
    }

    public float BombSpeed
    {
        get => _bombSpeed;
        set
        {
            if (_bombSpeed != value)
                _bombSpeed = value;
        }
    }
    #endregion

    /// <summary>
    /// Обработчик события переключения состояния игры
    /// </summary>
    public void HandleGameStateChange(object sender, GameController.GameState state)
    {
        switch (state)
        {
            case GameController.GameState.Start:

                _bombSpawnCoroutine = StartCoroutine(BombSpawnCoroutine(_bombCount, _bombSpawnDelay));
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Обработчик события столкновения бомбы, созданной спавнером
    /// </summary>
    private void HandleBombCollision(object sender, CollisionArgs args)
    {
        if (args.WasCatched)
        {
            OnSpawnerEvent(this, SpawnerEvent.Catch);

            args.Bomb.OnCollision -= HandleBombCollision;
            _bombs.Remove(args.Bomb);
        }

        else
        {
            OnSpawnerEvent(this, SpawnerEvent.LastDrop);

            StopCoroutine(_bombSpawnCoroutine);
            StartCoroutine(BombDestroyCoroutine());
        }
    }

    /// <summary>
    /// Сопрограмма спавна бомб по заданным параметрам
    /// </summary>
    private IEnumerator BombSpawnCoroutine(int bombCount, float bombSpawnDelay)
    {
        int droppedBombs = 0;

        while (droppedBombs < bombCount)
        {
            yield return new WaitForSeconds(bombSpawnDelay);

            _lastBomb = Factory_Game.CreateBomb(transform, _bombSpeed);
            _lastBomb.OnCollision += HandleBombCollision;
            _bombs.Add(_lastBomb);
            _lastBomb = null;

            OnSpawnerEvent(this, SpawnerEvent.Drop);
            droppedBombs++;
        }

        OnSpawnerEvent(this, SpawnerEvent.LastDrop);

        yield return new WaitUntil(() => _bombs.Count == 0);
        OnSpawnerEvent(this, SpawnerEvent.WaveCompleted);
    }

    /// <summary>
    /// Сопрограмма уничтожения всех созданных спавнером бомб
    /// </summary>
    private IEnumerator BombDestroyCoroutine()
    {
        foreach (var bomb in _bombs)
        {
            bomb.StopAllCoroutines();
            bomb.OnCollision -= HandleBombCollision;
        }

        foreach (var bomb in _bombs)
        {
            Factory_Particle.CreateParticleSystem(Factory_Particle.Explosion, bomb.transform);
            Factory_SFX.PlaySFX(Factory_SFX.SFX_Boom);
            bomb.DestroyBomb();

            yield return new WaitForSeconds(0.25f);
        }

        _bombs.Clear();
        OnSpawnerEvent(this, SpawnerEvent.WaveFailed);
    }
}