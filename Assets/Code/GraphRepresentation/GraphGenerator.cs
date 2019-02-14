using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GraphGenerator 
{
    NavMeshTriangulation triangulization;
    public Dictionary<int, Node> Graph;
    // Start is called before the first frame update
    public GraphGenerator(NavMeshTriangulation triangulization)
    {
        this.triangulization = triangulization;
        Graph = new Dictionary<int, Node>();
        GenerateGraph();
    }

    private void GenerateGraph()
    {
        int[] indices = triangulization.indices;
        Vector3[] vertices = triangulization.vertices;
        for (int i = 0; i < indices.Length; i += 3)
        {
            List<Vector3> tmp = new List<Vector3>();
            int i1 = indices[i];
            int i2 = indices[i + 1];
            int i3 = indices[i + 2];
            tmp.Add(vertices[i1]);
            tmp.Add(vertices[i2]);
            tmp.Add(vertices[i3]);
            //Nodes.Add(tmp);
            Node node = new Node(i / 3, tmp);
            List<Neighbor> neighbors = FindNeighbors(i + 3, tmp);

            if (!Graph.ContainsKey(i / 3)) Graph.Add(i / 3, node);
            Graph[(i / 3)].AddNeighBors(neighbors);

            for (int j = 0; j < neighbors.Count; ++j)
            {
                if (!Graph.ContainsKey(neighbors[j].NeighborID())) Graph.Add(neighbors[j].NeighborID(), neighbors[j].neighbor);
                Graph[neighbors[j].NeighborID()].AddNeigh(new Neighbor(Graph[(i / 3)], neighbors[j].isConnedtedByVertex(), neighbors[j].getAdjPoints()));
            }
        }
    }

    private List<Neighbor> FindNeighbors(int index, List<Vector3> triangle2)
    {
        List<Neighbor> neighbors = new List<Neighbor>();
        int[] indices = triangulization.indices;
        Vector3[] vertices = triangulization.vertices;
        for (int i = index; i < indices.Length; i += 3)
        {
            List<Vector3> triangle1 = new List<Vector3>();
            int i1 = indices[i];
            int i2 = indices[i + 1];
            int i3 = indices[i + 2];
            triangle1.Add(vertices[i1]);
            triangle1.Add(vertices[i2]);
            triangle1.Add(vertices[i3]);
            List<Vector3> AdjPoints = vertexShared(triangle1, triangle2);
            if (AdjPoints.Count > 0)
            {
                
                Node node = new Node(i / 3, triangle1);
                bool byVertex = false;
                if (AdjPoints.Count == 1) byVertex = true;
                neighbors.Add(new Neighbor(node, byVertex, AdjPoints));
            }
        }
        return neighbors;
    }

    private List<Vector3> vertexShared(List<Vector3> triangle1, List<Vector3> triangle2)
    {
        int count = 0;
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                if (triangle2[i].x == triangle1[j].x && triangle2[i].z == triangle1[j].z)
                {
                    points.Add(triangle2[i]);
                    ++count;
                }
            }
        }
        if (count == 2) return points;
        return new List<Vector3>();
        //return points;
    }
}
