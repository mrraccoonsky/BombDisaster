using UnityEngine;

public class Level : MonoBehaviour
{
    #region Fields
    [SerializeField] private Transform _playerSpawn;
    [SerializeField] private Transform _enemySpawn;
    #endregion

    #region Accessors
    public Transform PlayerSpawn
    {
        get => _playerSpawn;
    }

    public Transform EnemySpawn
    {
        get => _enemySpawn;
    }
    #endregion

    private void Awake()
    {
        if (_playerSpawn == null)
            Debug.Log("No PlayerSpawn assigned!");

        if (_enemySpawn == null)
            Debug.Log("No EnemySpawn assigned!");
    }
}
