using UnityEngine;

public static class Factory_Particle
{
    public static readonly GameObject Explosion = Resources.Load<GameObject>("ParticleSystems/Explosion");
    public static readonly GameObject WaterSplash = Resources.Load<GameObject>("ParticleSystems/WaterSplash");
    public static readonly GameObject BucketDrop = Resources.Load<GameObject>("ParticleSystems/BucketDrop");

    public static void CreateParticleSystem(GameObject particleSystem, Transform spawnPoint)
    {
        UnityEngine.Object.Instantiate(particleSystem, spawnPoint.position, Quaternion.identity);
    }
}