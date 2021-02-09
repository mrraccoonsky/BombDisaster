using UnityEngine;

public class Factory_Game
{
    public static Level CreateLevel()
    {
        GameObject go = Resources.Load<GameObject>("Levels/Level");
        Level level = UnityEngine.Object.Instantiate(go).GetComponent<Level>();

        return level;
    }

    public static Player CreatePlayer(Transform spawnPoint, Vector2 screenBounds)
    {
        GameObject go = Resources.Load<GameObject>("Prefabs/Player");
        Player player = UnityEngine.Object.Instantiate(go, spawnPoint.position, Quaternion.identity).GetComponent<Player>();

        player.ScreenBounds = screenBounds;

        return player;
    }

    public static Enemy CreateEnemy(Transform spawnPoint, Vector2 screenBounds)
    {
        GameObject go = Resources.Load<GameObject>("Prefabs/Enemy");
        Enemy enemy = UnityEngine.Object.Instantiate(go, spawnPoint.position, Quaternion.identity).GetComponent<Enemy>();

        enemy.ScreenBounds = screenBounds;

        return enemy;
    }

    public static BombSpawner CreateBombSpawner(Transform parent)
    {
        GameObject go = new GameObject("BombSpawner");
        BombSpawner bombSpawner = go.AddComponent<BombSpawner>();

        go.transform.position = parent.transform.position;
        go.transform.SetParent(parent);

        return bombSpawner;
    }

    public static Bomb CreateBomb(Transform spawnPoint, float speed)
    {
        GameObject go = Resources.Load<GameObject>("Prefabs/Bomb");
        Bomb bomb = UnityEngine.Object.Instantiate(go, spawnPoint.position, Quaternion.identity).GetComponent<Bomb>();

        bomb.Speed = speed;

        return bomb;
    }

    public static UI CreateUI(Canvas canvas)
    {
        GameObject go = Resources.Load<GameObject>("UI/UI");
        UI ui = UnityEngine.Object.Instantiate(go, canvas.transform).GetComponent<UI>();

        return ui;
    }
}
