using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighbor
{
    public Node neighbor;
    public bool byVertex;
    public List<Vector3> AdjPoints;
    public Dictionary<int, Vector3> WayPoints;
    public HashSet<int> FreeWayPoints;
    float radius = 1;

    public Neighbor(Node neighbor, bool byVertex, List<Vector3> AdjPoints)
    {
        this.neighbor = neighbor;
        this.byVertex = byVertex;
        this.AdjPoints = AdjPoints;
        this.WayPoints = new Dictionary<int, Vector3>();
        this.FreeWayPoints = new HashSet<int>();
        generateWayPoint();
    }
    private void generateWayPoint()
    {
        Vector2 StartPoint = new Vector2(AdjPoints[0].x, AdjPoints[0].z);
        Vector2 EndPoint = new Vector2(AdjPoints[1].x, AdjPoints[1].z);
        float K = Vector2.Distance(StartPoint, EndPoint)/ (2*radius);
        for (int i = 0; i <= K; ++i)
        {
            WayPoints.Add(i, new Vector3(StartPoint.x + (i / K) * (EndPoint.x - StartPoint.x), AdjPoints[0].y, (StartPoint.y + (i / K) * (EndPoint.y - StartPoint.y))));
            FreeWayPoints.Add(i);
        }

    }
    public bool isConnedtedByVertex()
    {
        return byVertex;
    }

    public List<Vector3> getAdjPoints()
    {
        return AdjPoints;
    }

    public int NeighborID()
    {
        return neighbor.id;
    }

    public List<Vector3> triangle()
    {
        return neighbor.triangle;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        Neighbor objAsNeighbor = obj as Neighbor;
        if (objAsNeighbor == null) return false;
        else return Equals(objAsNeighbor);
    }

    public bool Equals(Neighbor other)
    {
        if (other == null) return false;
        return (this.neighbor.id.Equals(other.neighbor.id));
    }

    public Vector3 getClosestWayPoint(Vector3 position, out int pointID)
    {
        int index = -1;
        float minDist = float.PositiveInfinity;
        for (int i = 0; i < WayPoints.Count; ++i)
        {
            if (FreeWayPoints.Contains(i))
            {
                float aux = Vector3.Distance(position, WayPoints[i]);
                if (minDist > aux)
                {
                    minDist = aux;
                    index = i;
                }
            }
        }
        this.FreeWayPoints.Remove(index);
        pointID = index;
        return WayPoints[index];
    }

    public void liberateWayPoint(int pointID)
    {
        this.FreeWayPoints.Add(pointID);
    }

    public bool isFree(int id)
    {
        return this.FreeWayPoints.Contains(id);
    }
    public bool anyFreeWayPoint()
    {
        return (this.FreeWayPoints.Count > 0);
    }
}
