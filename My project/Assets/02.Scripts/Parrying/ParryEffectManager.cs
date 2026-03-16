using UnityEngine;

public class ParryEffectManager : MonoBehaviour
{
    public static ParryEffectManager Instance { get; private set; }

    [Header("Effect Prefabs")]
    [SerializeField] private GameObject smallImpactEffectPrefab;
    [SerializeField] private GameObject missileImpactEffectPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlayEffect(ParryEffectType effectType, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = GetEffectPrefab(effectType);
        if (prefab == null)
            return;

        GameObject spawned = Instantiate(prefab, position, rotation);
        Destroy(spawned, 3f);
    }

    private GameObject GetEffectPrefab(ParryEffectType effectType)
    {
        switch (effectType)
        {
            case ParryEffectType.SmallImpact:
                return smallImpactEffectPrefab;

            case ParryEffectType.MissileImpact:
                return missileImpactEffectPrefab;

            default:
                return null;
        }
    }
}