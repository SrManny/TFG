using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphData : MonoBehaviour
{
    public AStar astar;
    public Dictionary<int, Node> Graph;
    
    // Start is called before the first frame update
    void Start()
    {
        this.astar = new AStar();
        this.Graph = astar.Graph;
    }
    
    public List<int> trianglePathFromTo(Vector3 origin, Vector3 destination)
    {
        astar.setOrigin(origin);
        astar.setDestination(destination);
        return astar.trianglePath2();
    }

    public List<Vector3> trianglePath(Vector3 origin, Vector3 destination)
    {

        astar.setOrigin(origin);
        astar.setDestination(destination);
        return astar.trianglePath();
    }

    public Vector3 getClosestWayPoint(int current, Vector3 position, out int pointID, int neighBorID, string tag)
    {
        Vector3 aux = Graph[current].Neighbors[neighBorID].getClosestWayPoint(position, out pointID);
        Debug.Log(tag + " " + Graph[current].Neighbors[neighBorID].isFree(pointID));
        return aux;
    }

    public void liberateWayPoint(int triangle, int neighBorID, int pointID)
    {
        Graph[triangle].Neighbors[neighBorID].liberateWayPoint(pointID);
    }

    public bool anyFreeWayPoint(int triangle, int neighBorID)
    {
        //Debug.Log("EsTAM)OS LOCOS" + triangle + " " + neighBorID + " " + Graph[triangle].Neighbors[neighBorID].anyFreeWayPoint());
        return Graph[triangle].Neighbors[neighBorID].anyFreeWayPoint();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
