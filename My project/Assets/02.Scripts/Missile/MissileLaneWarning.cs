using UnityEngine;

public class MissileLaneWarning : MonoBehaviour
{
    [SerializeField] private GameObject warningVisual;
    private float playerZPos;
    private float warningZPos;
    [SerializeField] private float warning2player = 4f;

    private void Awake()
    {
        SetVisible(false);
    }

    private void Start()
    {
        playerZPos = PlayerMovement.Instance.transform.position.z;
    }

    private void Update()
    {
        playerZPos = PlayerMovement.Instance.transform.position.z;
        warningZPos = playerZPos + warning2player;
        transform.position = new Vector3(transform.position.x, transform.position.y, warningZPos);
    }

    public void SetVisible(bool visible)
    {
        if (warningVisual != null)
        {
            warningVisual.SetActive(visible);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}