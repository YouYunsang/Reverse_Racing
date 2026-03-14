using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ParryMesh : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;

    // 셰이더의 컬러 프로퍼티 이름
    [SerializeField] private string colorPropertyName = "_Color";

    private Mesh mesh;
    private float lifeTimer;

    private MaterialPropertyBlock propertyBlock;
    private int colorPropertyId;

    private void Awake()
    {
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();

        mesh = new Mesh();
        mesh.name = "ParryMesh_Instance";
        meshFilter.mesh = mesh;

        propertyBlock = new MaterialPropertyBlock();
        colorPropertyId = Shader.PropertyToID(colorPropertyName);
    }

    private void Update()
    {
        lifeTimer -= Time.deltaTime;

        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void Setup(float range, float angle, float lifeTime, Color color)
    {
        lifeTimer = lifeTime;

        ApplyColor(color);
        GenerateSectorMesh(range, angle);
    }

    private void ApplyColor(Color color)
    {
        if (meshRenderer == null)
            return;

        meshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(colorPropertyId, color);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    private void GenerateSectorMesh(float range, float angle)
    {
        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "ParryMesh_Instance";
            meshFilter.mesh = mesh;
        }

        mesh.Clear();

        int segmentCount = 30;
        float halfAngle = angle * 0.5f;

        Vector3[] vertices = new Vector3[segmentCount + 2];
        int[] triangles = new int[segmentCount * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i <= segmentCount; i++)
        {
            float t = (float)i / segmentCount;
            float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, t);
            float rad = currentAngle * Mathf.Deg2Rad;

            float x = Mathf.Sin(rad) * range;
            float z = Mathf.Cos(rad) * range;

            vertices[i + 1] = new Vector3(x, 0f, z);
        }

        for (int i = 0; i < segmentCount; i++)
        {
            int startIndex = i + 1;

            triangles[i * 3 + 0] = 0;
            triangles[i * 3 + 1] = startIndex;
            triangles[i * 3 + 2] = startIndex + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private void OnDestroy()
    {
        if (mesh != null)
        {
            Destroy(mesh);
        }
    }
}