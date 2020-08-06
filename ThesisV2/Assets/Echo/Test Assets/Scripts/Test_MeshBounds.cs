using UnityEngine;

public class Test_MeshBounds : MonoBehaviour
{
    public MeshFilter m_meshFilter;
    public MeshRenderer m_meshRenderer;

    private void OnDrawGizmosSelected()
    {
        Vector3 cubePos = this.transform.position;

        Color extentsColor = Color.red;
        Color sizeColor = Color.blue;
        Color scaleColor = Color.green;

        Vector3 meshFilterSize = m_meshFilter.sharedMesh.bounds.size;
        Vector3 meshFilterExtents = m_meshFilter.sharedMesh.bounds.extents;

        Gizmos.color = extentsColor;
        Gizmos.DrawWireCube(cubePos, meshFilterExtents);

        Gizmos.color = sizeColor;
        Gizmos.DrawWireCube(cubePos, meshFilterSize);

        Gizmos.color = scaleColor;
        Vector3 scaledSize = new Vector3();
        scaledSize.x = meshFilterSize.x * transform.lossyScale.x;
        scaledSize.y = meshFilterSize.y * transform.lossyScale.y;
        scaledSize.z = meshFilterSize.z * transform.lossyScale.z;
        Gizmos.DrawWireCube(cubePos, scaledSize);
    }
}
