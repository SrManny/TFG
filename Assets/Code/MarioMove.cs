using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;


public class MarioMove : MonoBehaviour
{
    //[SerializeField]

    public Transform _destination;
    public GameObject ListLines, parent;
    public Animator _animator;
    public Material material, mat;
    NavMeshAgent _navMeshAgent;
    public NavMeshTriangulation triangulization;
    public static Dictionary<int, Node> Graph;
    public int NumTri = 19, indexPath = 0, pointID, prevNeighID;
    List<Vector3> trianglePath;
    AStar astar;
    Vector3 mousePos;
    bool MarioHasDestination, Lastmovement, FirsTriangle, stopped;
    public Mesh path;
    public Vector3 NextPoint;
    int currentTriangle;
    List<int> trianglePathID;
    float Speed;//Don't touch this
    float MaxSpeed;//This is the maximum speed that the object will achieve
    float Acceleration;//How fast will object reach a maximum speed
    float Deceleration;//How fast will object reach a speed of 0
    GraphData script;
    // Start is called before the first frame update

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
        MarioHasDestination = false;
        Lastmovement = false;
        FirsTriangle = true;
        stopped = false;
        NextPoint = new Vector3();
        Speed = 10;
        MaxSpeed = 10;
        Acceleration = 10;
        Deceleration = 10;
        currentTriangle = -1;
        //parent = GetComponentInParent<GameObject>();
        script = GetComponentInParent<GraphData>();
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

    private void SetDestination2()
    {
        if (_destination != null)
        {
            Vector3 targetVector = _destination.transform.position;
            MarioHasDestination = true;
            astar.setOrigin(transform.position);
            astar.setDestination(_destination.position);
            trianglePathID = astar.trianglePath2();//script.trianglePathFromTo(transform.position, _destination.position);
            currentTriangle = trianglePathID[indexPath];
            //_navMeshAgent.SetDestination(targetVector);
        }
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


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //SetDestination();
            SetDestination2();
            //Graphics.DrawMesh(path, Vector3.zero, Quaternion.identity, material, 0);
            //Debug.Log(triangulization.areas);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            //paintTriangleNeigh(NumTri, Color.black, Color.red);
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            astar.setOrigin(this.transform.position);
            paintPath(script.trianglePath(transform.position, _destination.position), Color.red);
        }
        if (MarioHasDestination) GoToNextPoint();
        _animator.SetFloat("Speed", Speed / 10.0f);
    }

    private void GoToNextPoint()
    {
        //Debug.Log("Brillamos?");
        
        if (Lastmovement && Vector3.Distance(NextPoint, transform.position) < 0.01)
        {
            //Debug.Log("Brillamos?1");
            MarioHasDestination = false;
        }
        else if (Lastmovement && Vector3.Distance(NextPoint, transform.position) >= 0.01)
        {
           //- Debug.Log("Brillamos?2");
            move();
        }
        else if ((FirsTriangle == true || Vector3.Distance(NextPoint, transform.position) < 1) || stopped)
        {
           // Debug.Log("Brillamos?3");
            AsignNewPoint();
        }
        else
        {
            //Debug.Log(NextPoint);
            //AsignNewPoint();
            move();
        }
    }

    private void AsignNewPoint()
    {
        
        
        if (indexPath + 1 < trianglePathID.Count)
        {

            List<Neighbor> aux = script.Graph[currentTriangle].Neighbors;
            int neiID = -1;
            for (int i = 0; i < aux.Count; ++i)
            {
                if (aux[i].NeighborID() == script.Graph[trianglePathID[indexPath + 1]].id)
                {
                    neiID = i;
                    //break;
                }
            }
            bool anyFreeWayPoint = script.anyFreeWayPoint(currentTriangle, neiID);

            if (anyFreeWayPoint)
            {

                if (!FirsTriangle)
                {
                    //Debug.Log(this.gameObject.name + " " + indexPath + " " + neiID + " " + pointID + " " + script.Graph[trianglePathID[indexPath - 1]].Neighbors.Count);
                    script.liberateWayPoint(trianglePathID[indexPath - 1], prevNeighID, pointID);
                }
                else FirsTriangle = false;
                Debug.Log(script.anyFreeWayPoint(currentTriangle, neiID));
                NextPoint = script.getClosestWayPoint(currentTriangle, transform.position, out pointID, neiID, this.gameObject.name);
                prevNeighID = neiID;
                //Debug.Log(pointID + " a ver que es");
                ++indexPath;
                currentTriangle = trianglePathID[indexPath];
                stopped = false;
            }
            else
            {
                //Debug.Log("TE JURO QUE TE MATO");
                stopped = true;
            }
        }
        else
        {
            script.liberateWayPoint(trianglePathID[indexPath - 1], prevNeighID, pointID);
            Lastmovement = true;
            NextPoint = _destination.transform.position;
        }
    }

    private void move()
    {
        float step = Speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, NextPoint, step);
        /*if ((Input.GetKey("left")) && (Speed < MaxSpeed))
            Speed = Speed - Acceleration  Time.deltaTime;
        else if ((Input.GetKey("right")) && (Speed > -MaxSpeed))
            Speed = Speed + Acceleration  Time.deltaTime;
        else
        {
            if (Speed > Deceleration  Time.deltaTime)
                Speed = Speed - Deceleration  Time.deltaTime;
            else if (Speed < -Deceleration  Time.deltaTime)
                Speed = Speed + Deceleration  Time.deltaTime;
            else
                Speed = 0;
            }
            transform.position.x = transform.position.x + Speed* Time.deltaTime;*/
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
