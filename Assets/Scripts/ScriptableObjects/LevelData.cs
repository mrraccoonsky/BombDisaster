using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 0)]
public class LevelData : ScriptableObject
{
    public float PlayerSpeed;
    public float EnemySpeed;

    public int BombCount;
    public float BombSpawnDelay;
    public float BombSpeed;
    public int BombScore;
}
