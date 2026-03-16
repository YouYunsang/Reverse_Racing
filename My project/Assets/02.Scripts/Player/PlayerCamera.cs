using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private float playerZPos;
    private float cameraZPos;
    public float camera2player = 2.4f;

    void Start()
    {
        playerZPos = PlayerMovement.Instance.transform.position.z;
    }

    void LateUpdate()
    {
        playerZPos = PlayerMovement.Instance.transform.position.z;
        cameraZPos = playerZPos - camera2player;
        transform.position = new Vector3(transform.position.x, transform.position.y, cameraZPos);
    }
}
