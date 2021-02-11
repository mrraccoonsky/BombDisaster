using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionArgs : EventArgs
{
    public CollisionArgs(Bomb bomb, bool wasCatched)
    {
        Bomb = bomb;
        WasCatched = wasCatched;
    }

    public Bomb Bomb { get; set; }
    public bool WasCatched { get; set; }
}

public class Bomb : MonoBehaviour
{
    #region Events
    public EventHandler<bool> OnCollision;
    #endregion

    #region Fields
    [SerializeField] private Vector2 _hitBox;
    [SerializeField] private ContactFilter2D _contactFilter;

    private float _speed;
    #endregion

    #region Accessors
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

    private void Start()
    {
        StartCoroutine(MovementCoroutine());
        StartCoroutine(CheckCollisionCoroutine());
    }

    private IEnumerator MovementCoroutine()
    {
        transform.rotation = UnityEngine.Random.rotation;
        Factory_SFX.PlaySFX(Factory_SFX.SFX_Drop);

        while (true)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime, Space.World);
            yield return null;
        }
    }

    private IEnumerator CheckCollisionCoroutine()
    {
        List<Collider2D> _hitResults = new List<Collider2D>();

        while (true)
        {
            Physics2D.OverlapBox(transform.position, _hitBox, 0f, _contactFilter, _hitResults);

            if (_hitResults.Count > 0)
            {
                foreach (Collider2D result in _hitResults)
                {
                    MonoBehaviour mb = result.GetComponentInParent<MonoBehaviour>();

                    if (mb is IPlayer)
                    {
                        Factory_Particle.CreateParticleSystem(Factory_Particle.WaterSplash, result.gameObject.transform);
                        Factory_SFX.PlaySFX(Factory_SFX.SFX_Collect);

                        DestroyBomb();

                        OnCollision(this, true);
                        break;
                    }

                    else
                    {
                        OnCollision(this, false);
                        break;
                    }
                }
            }

            yield return null;
        }
    }

    public void DestroyBomb()
    {
        Destroy(gameObject);
    }
}