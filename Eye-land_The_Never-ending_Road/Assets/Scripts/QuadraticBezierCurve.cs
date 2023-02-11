using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class QuadraticBezierCurve : MonoBehaviour {

    public Vector3[] _points = new Vector3[4];
    
    public float _width = 10;
    public int _resolution = 50;
    public float _thickness = 10;

    private MeshFilter _filter;
    private MeshRenderer _renderer;
    public Mesh _mesh;

    public Vector3[] _vertices;
    public Vector2[] _uv;
    public int[] _triangles;

    // Constructor // Is destructor implied in C# ???
    public void SetPoints(Vector3[] points) {
        this._points = points;
    }

    // Calculate a point along t value with the quadratic Bezier formula
    public Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    // Derive at a given t to know the direction
    public Vector3 CalculateBezierDirection(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        Vector3 d = -3 * p0 * (1 - t) * (1 - t) + 3 * p1 * (1 - t) * (1 - t) - 6 * p1 * t * (1 - t) + 6 * p2 * t * (1 - t) - 3 * p2 * t * t + 3 * p3 * t * t;
        return d.normalized;
    }

    // Processing Bezier formula for each t, then transform that into Mesh
    public GameObject GenerateBezierRoad(float _difficulty) {
        Vector3[] vertices = new Vector3[_resolution * 2];
        Vector2[] uv = new Vector2[_resolution * 2];
        int[] triangles = new int[(_resolution - 1) * 6];

        for (int i = 0; i < _resolution; i++) {
            float t = (float)i / (_resolution - 1);
            Vector3 point = CalculateBezierPoint(this._points[0], this._points[1], this._points[2], this._points[3], t);
            Vector3 direction = CalculateBezierDirection(this._points[0], this._points[1], this._points[2], this._points[3], t);
            Vector3 normal = Vector3.Cross(direction, Vector3.up).normalized;
            // Debug.Log(point);
            // Debug.Log(direction);
            vertices[i * 2] = point - normal * _width / 2;
            vertices[i * 2 + 1] = point + normal * _width / 2;
            uv[i * 2] = new Vector2(0, t);
            uv[i * 2 + 1] = new Vector2(1, t);

            if (i > 0) {
                int index = (i - 1) * 6;
                triangles[index] = i * 2 - 2;
                triangles[index + 1] = i * 2 - 1;
                triangles[index + 2] = i * 2;
                triangles[index + 3] = i * 2 + 1;
                triangles[index + 4] = i * 2;
                triangles[index + 5] = i * 2 - 1;
            }
        }

        GameObject go = new GameObject();

        _filter = go.AddComponent<MeshFilter>();
        _renderer = go.AddComponent<MeshRenderer>();
        _mesh = new Mesh();
        _filter.mesh = _mesh;

        _mesh.vertices = vertices;
        _mesh.uv = uv;
        _mesh.triangles = triangles;

        _vertices = vertices;
        _uv = uv;
        _triangles = triangles;

        //_renderer.material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        _renderer.material.color = new Color(_difficulty, (1f - _difficulty), 0.1f);

        go.name = "curve";
        //go.tag = "curve";
        return go;
    }

    // EZ Debug with controls points
    public void OnDrawGizmos() {
        
        for (int i = 0; i < this._points.Length; i++) {
            Gizmos.color = Color.yellow;
            if (i == 1 ||i == 2) {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawSphere(this._points[i], _width/5);
        }

        for (int i = 0; i < this._points.Length - 1; i++) {
            Vector3 start = this._points[i];
            Vector3 end = this._points[i + 1];
            Vector3 startTangent = start + Vector3.right * 0.5f;
            Vector3 endTangent = end + Vector3.left * 0.5f;
            Gizmos.DrawLine(start, startTangent);
            Gizmos.DrawLine(end, endTangent);
            for (float t = 0; t <= 1; t += 0.1f) {
                Vector3 point = CalculateBezierPoint(start, startTangent, endTangent, end, t);
                Gizmos.DrawSphere(point, 0.02f);
            }
        }
    }



    // Ca marche, mais ici je veux rendre une unique courbe de bezier quadratique qui se suit ! Pas rendre un objet de toutes les courbes. Le but seraitd'avoir une seule courbe lisse, sans espace entre les courbes.
    public void MergeAllCurves(List<QuadraticBezierCurve> _allCurves) {
        List<Vector3> verticesList = new List<Vector3>();
        List<Vector2> uvList = new List<Vector2>();
        List<int> trianglesList = new List<int>();

        int currentIndex = 0;
        for(int i = 0; i < _allCurves.Count; i++) {
            QuadraticBezierCurve currentCurve = _allCurves[i];
            for(int j = 0; j < currentCurve._resolution; j++) {
                verticesList.Add(currentCurve._vertices[j * 2]);
                verticesList.Add(currentCurve._vertices[j * 2 + 1]);
                uvList.Add(currentCurve._uv[j * 2]);
                uvList.Add(currentCurve._uv[j * 2 + 1]);

                if(j > 0) {
                    int currentTriangleIndex = (j - 1) * 6;
                    int[] currentTriangles = currentCurve._triangles;
                    trianglesList.Add(currentTriangles[currentTriangleIndex] + currentIndex);
                    trianglesList.Add(currentTriangles[currentTriangleIndex + 1] + currentIndex);
                    trianglesList.Add(currentTriangles[currentTriangleIndex + 2] + currentIndex);
                    trianglesList.Add(currentTriangles[currentTriangleIndex + 3] + currentIndex);
                    trianglesList.Add(currentTriangles[currentTriangleIndex + 4] + currentIndex);
                    trianglesList.Add(currentTriangles[currentTriangleIndex + 5] + currentIndex);
                }
            }
            currentIndex += currentCurve._resolution * 2;
        }

        Vector3[] finalVertices = verticesList.ToArray();
        Vector2[] finalUv = uvList.ToArray();
        int[] finalTriangles = trianglesList.ToArray();

        GameObject go = new GameObject();
        MeshFilter filter = go.AddComponent<MeshFilter>();
        MeshRenderer renderer = go.AddComponent<MeshRenderer>();
        Mesh finalMesh = new Mesh();
        filter.mesh = finalMesh;

        finalMesh.vertices = finalVertices;
        finalMesh.uv = finalUv;
        finalMesh.triangles = finalTriangles;
        renderer.material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }
}


    // Processing Bezier formula for each t, then transform that into Mesh
    // public void GenerateBezierRoad() {
    //     Vector3[] vertices = new Vector3[_resolution * 2];
    //     Vector2[] uv = new Vector2[_resolution * 2];
    //     int[] triangles = new int[(_resolution - 1) * 6];

    //     for (int i = 0; i < _resolution; i++) {
    //         float t = (float)i / (_resolution - 1);
    //         Vector3 point = CalculateBezierPoint(this._points[0], this._points[1], this._points[2], this._points[3], t);
    //         Vector3 direction = CalculateBezierDirection(this._points[0], this._points[1], this._points[2], this._points[3], t);
    //         Vector3 normal = Vector3.Cross(direction, Vector3.up).normalized;
    //         // Debug.Log(point);
    //         // Debug.Log(direction);
    //         vertices[i * 2] = point - normal * _width / 2;
    //         vertices[i * 2 + 1] = point + normal * _width / 2;
    //         uv[i * 2] = new Vector2(0, t);
    //         uv[i * 2 + 1] = new Vector2(1, t);

    //         if (i > 0) {
    //             int index = (i - 1) * 6;
    //             triangles[index] = i * 2 - 2;
    //             triangles[index + 1] = i * 2 - 1;
    //             triangles[index + 2] = i * 2;
    //             triangles[index + 3] = i * 2 + 1;
    //             triangles[index + 4] = i * 2;
    //             triangles[index + 5] = i * 2 - 1;
    //         }
    //     }
      
    //     // Add thickness to the road by adding additional vertices and triangles
    //     Vector3[] verticesWithThickness = new Vector3[vertices.Length * 2];
    //     for (int i = 0; i < vertices.Length; i++) {
    //         verticesWithThickness[i] = vertices[i];
    //         verticesWithThickness[i + vertices.Length] = vertices[i] + new Vector3(0, _thickness, 0);
    //     }

    //     // Add triangles for the additional vertices
    //     int[] trianglesWithThickness = new int[triangles.Length + (_resolution - 1) * 12];
    //     for (int i = 0; i < triangles.Length; i++) {
    //     trianglesWithThickness[i] = triangles[i];
    //     }
        
    //     int baseIndex = triangles.Length;
    //     for (int i = 0; i < _resolution - 1; i++) {
    //         int index = i * 4;
    //         int newIndex = baseIndex + i * 12;
    //         // Triangles supérieurs
    //         trianglesWithThickness[newIndex] = index + 2;
    //         trianglesWithThickness[newIndex + 1] = index + 3;
    //         trianglesWithThickness[newIndex + 2] = index + 6;
    //         trianglesWithThickness[newIndex + 3] = index + 7;
    //         trianglesWithThickness[newIndex + 4] = index + 6;
    //         trianglesWithThickness[newIndex + 5] = index + 3;

    //         // Triangles latéraux
    //         trianglesWithThickness[newIndex + 6] = index;
    //         trianglesWithThickness[newIndex + 7] = index + 2;
    //         trianglesWithThickness[newIndex + 8] = index + 6;
    //         trianglesWithThickness[newIndex + 9] = index + 1;
    //         trianglesWithThickness[newIndex + 10] = index + 3;
    //         trianglesWithThickness[newIndex + 11] = index + 7;
    //     }
    //     Vector2[] uvWithThickness = new Vector2[verticesWithThickness.Length];
    //         for (int i = 0; i < _resolution * 2; i++) {
    //         uvWithThickness[i] = uv[i];
    //         uvWithThickness[i + _resolution * 2] = uv[i];
    //     }
    //     GameObject go = new GameObject();
    //     _filter = go.AddComponent<MeshFilter>();
    //     _renderer = go.AddComponent<MeshRenderer>();
    //     _mesh = new Mesh();
    //     _filter.mesh = _mesh;

    //     _mesh.vertices = verticesWithThickness;
    //     _mesh.triangles = trianglesWithThickness;
    //     _mesh.RecalculateNormals();
    //     _mesh.RecalculateBounds();

    //     _mesh.uv = uvWithThickness;

    //     _renderer.material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

    // }