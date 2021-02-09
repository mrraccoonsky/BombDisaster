using UnityEngine;

public class Factory_SFX
{
    public static readonly AudioClip SFX_Drop = Resources.Load<AudioClip>("SFX/Drop");
    public static readonly AudioClip SFX_Collect = Resources.Load<AudioClip>("SFX/Collect");
    public static readonly AudioClip SFX_Boom = Resources.Load<AudioClip>("SFX/Boom");
    public static readonly AudioClip SFX_BigBoom = Resources.Load<AudioClip>("SFX/BigBoom");
    public static readonly AudioClip SFX_GameOver = Resources.Load<AudioClip>("SFX/GameOver");
    public static readonly AudioClip SFX_1Up = Resources.Load<AudioClip>("SFX/1Up");

    public static void PlaySFX(AudioClip audioClip)
    {
        if (audioClip != null)
        {
            GameObject go = new GameObject($"SFX: {audioClip.name}");
            AudioSource source = go.AddComponent<AudioSource>();
            source.PlayOneShot(audioClip);

            UnityEngine.GameObject.Destroy(go, audioClip.length);
        }
    }
}
