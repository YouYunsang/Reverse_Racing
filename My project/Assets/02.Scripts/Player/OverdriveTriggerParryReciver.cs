using UnityEngine;

public class OverdriveTriggerParryReceiver : MonoBehaviour
{
    [SerializeField] private PlayerOverdrive playerOverdrive;

    private void Awake()
    {
        if (playerOverdrive == null)
        {
            playerOverdrive = GetComponentInParent<PlayerOverdrive>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerOverdrive == null || !playerOverdrive.IsOverdriveActive)
            return;

        if (other.GetComponentInParent<IParryable>() == null)
            return;

        playerOverdrive.TryParryOnCollision(other);
    }
}