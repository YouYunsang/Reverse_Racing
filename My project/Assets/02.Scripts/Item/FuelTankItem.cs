using UnityEngine;

public class FuelTankItem : MonoBehaviour
{
    [Header("Fuel")]
    [SerializeField] private float recoverAmount = 25f;
    [SerializeField] private string playerTag = "Player";

    private FuelTankSpawner ownerSpawner;
    private bool isCollected = false;

    public void Initialize(FuelTankSpawner spawner)
    {
        ownerSpawner = spawner;
        isCollected = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected)
            return;

        if (!other.CompareTag(playerTag))
            return;

        PlayerFuel playerFuel = other.GetComponentInParent<PlayerFuel>();
        if (playerFuel != null)
        {
            playerFuel.RecoverFuel(recoverAmount);
        }

        isCollected = true;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (ownerSpawner != null)
        {
            ownerSpawner.NotifyFuelTankRemoved(this);
        }
    }
}