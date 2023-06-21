using UnityEngine;

public class CubeSlicer : MonoBehaviour
{
    public GameObject slicedCubePrefab; // Prefab for the sliced cube halves

    private MeshFilter meshFilter;

    private void Start()
    {
        // Get the MeshFilter component attached to the cube game object
        meshFilter = GetComponent<MeshFilter>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the saber collider
        if (collision.gameObject.CompareTag("Saber"))
        {
            // Slice the cube
            SliceCube(collision);
        }
    }

    private void SliceCube(Collision collision)
    {
        // Get the current mesh data
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Calculate the mid-point along the slicing axis (e.g., the x-axis)
        float slicePosition = (transform.position.x + collision.transform.position.x) * 0.5f;

        // Create separate vertex and triangle lists for the two halves of the sliced cube
        Vector3[] verticesHalf1 = new Vector3[vertices.Length];
        Vector3[] verticesHalf2 = new Vector3[vertices.Length];
        int[] trianglesHalf1 = new int[triangles.Length];
        int[] trianglesHalf2 = new int[triangles.Length];

        int half1VertexIndex = 0;
        int half2VertexIndex = 0;
        int half1TriangleIndex = 0;
        int half2TriangleIndex = 0;

        // Iterate through all vertices and triangles
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Get the three vertices of the current triangle
            Vector3 vertex1 = vertices[triangles[i]];
            Vector3 vertex2 = vertices[triangles[i + 1]];
            Vector3 vertex3 = vertices[triangles[i + 2]];

            // Check if any of the vertices crosses the slicing plane
            bool vertex1CrossesPlane = (vertex1.x <= slicePosition);
            bool vertex2CrossesPlane = (vertex2.x <= slicePosition);
            bool vertex3CrossesPlane = (vertex3.x <= slicePosition);

            // Slice the triangle based on vertex positions
            if (vertex1CrossesPlane && vertex2CrossesPlane && vertex3CrossesPlane)
            {
                // All vertices are on one side of the plane (either side1 or side2)
                // Add the triangle to the respective half
                trianglesHalf1[half1TriangleIndex] = half1VertexIndex;
                trianglesHalf1[half1TriangleIndex + 1] = half1VertexIndex + 1;
                trianglesHalf1[half1TriangleIndex + 2] = half1VertexIndex + 2;

                // Increment the triangle and vertex indices
                half1TriangleIndex += 3;
                half1VertexIndex += 3;
            }
            else if (!vertex1CrossesPlane && !vertex2CrossesPlane && !vertex3CrossesPlane)
            {
                // All vertices are on the other side of the plane
                trianglesHalf2[half2TriangleIndex] = half2VertexIndex;
                trianglesHalf2[half2TriangleIndex + 1] = half2VertexIndex + 1;
                trianglesHalf2[half2TriangleIndex + 2] = half2VertexIndex + 2;

                half2TriangleIndex += 3;
                half2VertexIndex += 3;
            }
            else
            {
                // The triangle intersects the slicing plane
                // Calculate the intersection points on the edges of the triangle
                Vector3 intersectionPoint1, intersectionPoint2;

                if (vertex1CrossesPlane && vertex2CrossesPlane)
                {
                    intersectionPoint1 = vertex1;
                    intersectionPoint2 = vertex3;
                }
                else if (vertex2CrossesPlane && vertex3CrossesPlane)
                {
                    intersectionPoint1 = vertex2;
                    intersectionPoint2 = vertex1;
                }
                else
                {
                    intersectionPoint1 = vertex3;
                    intersectionPoint2 = vertex2;
                }

                // Calculate the interpolation factor for the intersection points
                float t = Mathf.InverseLerp(vertex1.x, vertex3.x, slicePosition);

                // Interpolate the intersection points along the edge
                Vector3 interpolatedPoint1 = Vector3.Lerp(intersectionPoint1, intersectionPoint2, t);
                Vector3 interpolatedPoint2 = interpolatedPoint1;

                // Add the interpolated points to both halves
                verticesHalf1[half1VertexIndex] = interpolatedPoint1;
                verticesHalf2[half2VertexIndex] = interpolatedPoint2;

                // Add the triangle to the respective half
                trianglesHalf1[half1TriangleIndex] = half1VertexIndex;
                trianglesHalf1[half1TriangleIndex + 1] = half1VertexIndex + 1;
                trianglesHalf1[half1TriangleIndex + 2] = half1VertexIndex + 2;

                trianglesHalf2[half2TriangleIndex] = half2VertexIndex;
                trianglesHalf2[half2TriangleIndex + 1] = half2VertexIndex + 1;
                trianglesHalf2[half2TriangleIndex + 2] = half2VertexIndex + 2;

                // Increment the triangle and vertex indices
                half1TriangleIndex += 3;
                half1VertexIndex += 3;
                half2TriangleIndex += 3;
                half2VertexIndex += 3;
            }
        }

        // Create new mesh objects for the sliced halves
        Mesh slicedMesh1 = new Mesh();
        slicedMesh1.vertices = verticesHalf1;
        slicedMesh1.triangles = trianglesHalf1;
        slicedMesh1.RecalculateNormals();

        Mesh slicedMesh2 = new Mesh();
        slicedMesh2.vertices = verticesHalf2;
        slicedMesh2.triangles = trianglesHalf2;
        slicedMesh2.RecalculateNormals();

        // Instantiate the sliced cube halves as separate game objects
        GameObject slicedCube1 = Instantiate(slicedCubePrefab, transform.position, transform.rotation);
        slicedCube1.GetComponent<MeshFilter>().mesh = slicedMesh1;

        GameObject slicedCube2 = Instantiate(slicedCubePrefab, transform.position, transform.rotation);
        slicedCube2.GetComponent<MeshFilter>().mesh = slicedMesh2;

        // Destroy the original cube game object
        Destroy(gameObject);
    }
}
