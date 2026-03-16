using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private float playerZPos;
    private float cameraZPos;

    void Start()
    {
        playerZPos = PlayerMovement.Instance.transform.position.z;
    }

    void LateUpdate()
    {
        playerZPos = PlayerMovement.Instance.transform.position.z;
        cameraZPos = playerZPos - 2.285f;
        transform.position = new Vector3(transform.position.x, transform.position.y, cameraZPos);
    }
}
