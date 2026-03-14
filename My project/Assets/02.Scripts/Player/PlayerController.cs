using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerMovement LaneMovement { get; private set; }

    private void Awake()
    {
        LaneMovement = GetComponent<PlayerMovement>();
    }
}