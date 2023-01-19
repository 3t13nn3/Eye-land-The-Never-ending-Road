using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class QuadraticBezierCurve : MonoBehaviour {

    public Vector3[] _points = new Vector3[4];
    
    public float _width = 10;
    public int _resolution = 50;

    private MeshFilter _filter;
    private MeshRenderer _renderer;
    private Mesh _mesh;

    // Constructor // Is destructor implied in C# ???
    public void SetPoints(Vector3[] points) {
        this._points = points;
    }

    // Calculate a point along t value with the quadratic Bezier formula
    public Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3,float t) {
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
    public Vector3 CalculateBezierDirection(float t) {
        Vector3 d = -3 * this._points[0] * (1 - t) * (1 - t) + 3 * this._points[1] * (1 - t) * (1 - t) - 6 * this._points[1] * t * (1 - t) + 6 * this._points[2] * t * (1 - t) - 3 * this._points[2] * t * t + 3 * this._points[3] * t * t;
        return d.normalized;
    }

    // Processing Bezier formula for each t, then transform that into Mesh
    public void GenerateBezierRoad() {
        Vector3[] vertices = new Vector3[_resolution * 2];
        Vector2[] uv = new Vector2[_resolution * 2];
        int[] triangles = new int[(_resolution - 1) * 6];

        for (int i = 0; i < _resolution; i++) {
            float t = (float)i / (_resolution - 1);
            Vector3 point = CalculateBezierPoint(this._points[0], this._points[1], this._points[2], this._points[3], t);
            Vector3 direction = CalculateBezierDirection(t);
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
        
    }

    // EZ Debug with controls points
    public void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < this._points.Length; i++) {
            Gizmos.DrawSphere(this._points[i], 0.1f);
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
}
