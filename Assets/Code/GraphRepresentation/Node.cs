using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    // Start is called before the first frame update
    public int id;
    public Vector3 actualPosition, potencialPosition;
    public List<Vector3> triangle;
    public List<Neighbor> Neighbors;
    public Vector3 barycenter;

    public Node(int id, List<Vector3> triangle)
    {
        this.id = id;
        this.triangle = triangle;
        this.barycenter = (triangle[0] + triangle[1] + triangle[2])/3;
        Neighbors = new List<Neighbor>();
    }

    public void AddNeigh(Neighbor nei)
    {
        Neighbors.Add(nei);
    }
    
    public void AddNeighBors(List<Neighbor> neighbors)
    {
        Neighbors.AddRange(neighbors);
    }

    public  Vector3 Getbarycenter()
    {
        return barycenter;
    }
}
