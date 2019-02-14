using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighbor
{
    public Node neighbor;
    public bool byVertex;
    List<Vector3> AdjPoints;

    public Neighbor(Node neighbor, bool byVertex, List<Vector3> AdjPoints)
    {
        this.neighbor = neighbor;
        this.byVertex = byVertex;
        this.AdjPoints = AdjPoints;
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
}
