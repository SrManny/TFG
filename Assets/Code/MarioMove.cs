using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;


public class MarioMove : MonoBehaviour
{
    //[SerializeField]
    public Transform _destination;
    public GameObject ListLines;
    public Animator _animator;
    public Material material, mat;
    NavMeshAgent _navMeshAgent;
    public NavMeshTriangulation triangulization;
    public Dictionary<int, Node> Graph;
    public int NumTri = 19;
    List<Vector3> trianglePath;
    AStar astar;
    Vector3 mousePos;

    public Mesh path;
    // Start is called before the first frame update

    static double area(int x1, int y1, int x2,
                       int y2, int x3, int y3)
    {
        return System.Math.Abs((x1 * (y2 - y3) +
                         x2 * (y3 - y1) +
                         x3 * (y1 - y2)) / 2.0);
    }

    void Start()
    {
        _navMeshAgent = this.GetComponent<NavMeshAgent>();
        _animator = this.GetComponent<Animator>();
        ListLines = new GameObject();
        ListLines.transform.parent = this.transform.parent;
        ListLines.name = "ListLines";
        path = GetComponentInParent<MeshFilter>().mesh;
        path.Clear();
        astar = new AStar();
        path.name = "ola";
        Graph = astar.Graph;
        triangulization = astar.triangulation;
        astar.setOrigin(transform.position);
        astar.setDestination(_destination.position);
        //paintPath(trianglePath, Color.red);
        //paintTriangleNeigh(26, Color.black, Color.red);
    }

    private void SetDestination()
    {
        if (_destination != null)
        {
            Vector3 targetVector = _destination.transform.position;
            _navMeshAgent.SetDestination(targetVector);
        }
    }

    private Vector3[] triangleintersected()
    {
        int[] indices = triangulization.indices;
        Vector3[] vertices = triangulization.vertices;
        List<Vector3> tmp = new List<Vector3>();
        Vector2 position = new Vector2(transform.position.x, transform.position.z);
        for (int i = 0; i < indices.Length; i +=3)
        {
            int i1 = indices[i];
            int i2 = indices[i+1];
            int i3 = indices[i+2];
            tmp.Add(vertices[i1]);
            tmp.Add(vertices[i2]);
            tmp.Add(vertices[i3]);
            if (intersect(tmp, position)) return tmp.ToArray();
            else tmp.Clear();
        }
        return tmp.ToArray();
    }

    private bool intersect(List<Vector3> triangle, Vector2 position)
    {
        int x1 = (int) triangle[0].x;
        int x2 = (int) triangle[1].x;
        int x3 = (int) triangle[2].x;
        int z1 = (int) triangle[0].z;
        int z2 = (int) triangle[1].z;
        int z3 = (int) triangle[2].z;
        int x = (int) position.x;
        int z = (int) position.y;

        /* Calculate area of triangle ABC */
        double A = area(x1, z1, x2, z2, x3, z3);

        /* Calculate area of triangle PBC */
        double A1 = area(x, z, x2, z2, x3, z3);

        /* Calculate area of triangle PAC */
        double A2 = area(x1, z1, x, z, x3, z3);

        /* Calculate area of triangle PAB */
        double A3 = area(x1, z1, x2, z2, x, z);

        /* Check if sum of A1, A2 and A3 is same as A */
        return (A == A1 + A2 + A3);
    }

    public void paintPath(List<Vector3> trianglePath, Color color1)
    {
     
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();
        int count = 0;
        
        for (int j = 0; j < trianglePath.Count; j += 3)
        {
            colors.Add(color1);
            colors.Add(color1);
            colors.Add(color1);
            DrawLine(trianglePath[j], trianglePath[j + 1], Color.white);
            DrawLine(trianglePath[j+1], trianglePath[j + 2], Color.white);
            DrawLine(trianglePath[j+2], trianglePath[j], Color.white);
            triangles.Add(count);
            triangles.Add(count + 1);
            triangles.Add(count + 2);
            count += 3;
        }
        path.vertices = trianglePath.ToArray();
        path.colors = colors.ToArray();
        path.triangles = triangles.ToArray();
    }

    private void paintTriangle(int index, Color color)
    {
        Vector3[] vertexs = triangulization.vertices;
        Color[] colors = new Color[3];
        int[] indices = triangulization.indices;

        int lenght = indices.Length;
        for (int i = 0; i < 1; i += 3)
        {
            int i1 = indices[i];
            int i2 = indices[i + 1];
            int i3 = indices[i + 2];
            colors[0] = color;
            colors[1] = color;
            colors[2] = color;
            Debug.Log("INICIO TRIANGULO");
            Debug.Log(vertexs[i1]);
            Debug.Log(vertexs[i2]);
            Debug.Log(vertexs[i3]);
            Debug.Log("FIN TRIANGULO");

        }
        path.vertices = Graph[index].triangle.ToArray();
        path.colors = colors;
        //path.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
        path.triangles = new int[] { 0, 1, 2 };
    }
    private void paintTriangleNeigh(int index, Color color1, Color color2)
    {
        path.Clear();
        List<Vector3> vertexs = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();
        int[] indices = triangulization.indices;

        int lenght = indices.Length;
        int count = 0;
        colors.Add(color1);
        colors.Add(color1);
        colors.Add(color1);
        vertexs.AddRange(Graph[index].triangle);
        triangles.Add(count);
        triangles.Add(count + 1);
        triangles.Add(count + 2);
        List<Neighbor> neighbors = Graph[index].Neighbors;
        for (int j = 0; j < neighbors.Count; ++j)
        {
            count += 3;
            colors.Add(color2);
            colors.Add(color2);
            colors.Add(color2);
            vertexs.AddRange(neighbors[j].neighbor.triangle);
            triangles.Add(count);
            triangles.Add(count + 1);
            triangles.Add(count + 2);
        }
        path.vertices = vertexs.ToArray();
        path.colors = colors.ToArray();
        path.triangles = triangles.ToArray();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetDestination();
            //Graphics.DrawMesh(path, Vector3.zero, Quaternion.identity, material, 0);
            //Debug.Log(triangulization.areas);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            paintTriangleNeigh(NumTri, Color.black, Color.red);
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            astar.setOrigin(this.transform.position);
            paintPath(astar.trianglePath(), Color.red);
        }
        _animator.SetFloat("Speed", _navMeshAgent.velocity.magnitude / 8.0f);
    }

    void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.name = "Line";
        myLine.transform.parent = ListLines.transform;
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = mat;
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.5f;
        lr.endWidth = 0.5f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

}
