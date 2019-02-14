using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.AI;

public class AStar : MonoBehaviour
{
    public Dictionary<int, Node> Graph;
    public Dictionary<int, Node> invertedPath;
    public SortedDictionary<int, float> gScore, fScore;
    public HashSet<int> closedSet, openSet;
    Vector3 origin, destination;
    public NavMeshTriangulation triangulation;

    public AStar()
    {
        this.triangulation = NavMesh.CalculateTriangulation();
        GraphGenerator graphG = new GraphGenerator(triangulation);
        this.Graph = graphG.Graph;
        closedSet = new HashSet<int>();
        openSet = new HashSet<int>();
        gScore = new SortedDictionary<int, float>();
        fScore = new SortedDictionary<int, float>();
        invertedPath = new Dictionary<int, Node>();
        foreach (KeyValuePair<int, Node> node in Graph)
        {
            gScore[node.Key] = float.PositiveInfinity;
            fScore[node.Key] = float.PositiveInfinity;
        }
    }

    static double area(int x1, int y1, int x2,
                       int y2, int x3, int y3)
    {
        return System.Math.Abs((x1 * (y2 - y3) +
                         x2 * (y3 - y1) +
                         x3 * (y1 - y2)) / 2.0);
    }

    private float heuristicCost(Vector3 A)
    {
        return Vector3.Distance(A, destination) ;
    }

    private int getTheMinimum()
    {
        int res = -1;
        float min = float.PositiveInfinity;
        foreach(int id in openSet)
        {
            if (fScore[id] < min)
            {
                min = fScore[id];
                res = id;
            }
        }
        return res;
    }

    private int initialNode()
    {
        Vector2 position = new Vector2(origin.x, origin.z);
        foreach (KeyValuePair<int, Node> node in Graph)
        {
            if (intersect(node.Value.triangle, position)) return node.Key;
        }
        return -1;
    }

    private int LastNode()
    {
        Vector2 position = new Vector2(destination.x, destination.z);
        foreach (KeyValuePair<int, Node> node in Graph)
        {

            if (intersect(node.Value.triangle, position)) return node.Key;
        }
        return -1;
    }

    private bool intersect(List<Vector3> triangle, Vector2 position)
    {
        int x1 = (int)triangle[0].x;
        int x2 = (int)triangle[1].x;
        int x3 = (int)triangle[2].x;
        int z1 = (int)triangle[0].z;
        int z2 = (int)triangle[1].z;
        int z3 = (int)triangle[2].z;
        int x = (int)position.x;
        int z = (int)position.y;

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
    public void setOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void setDestination(Vector3 destination)
    {
        this.destination = destination;
    }

    //Problema porque estamos contando el coste del punto mas cercano del personaje al siguiente triangulo
    public List<Vector3> reconstructTrianglePath(int current)
    {
        List<Vector3> trianglePath = new List<Vector3>();
        trianglePath.AddRange(Graph[current].triangle);
        //if (invertedPath.ContainsKey(invertedPath[current].id)) trianglePath.AddRange(IntersectedTriangles(current, invertedPath[current].id));
        //Debug.Log(current + "con un coste gScore " + gScore[current] + "y el coso de fScore " + fScore[current] + " Y ADEMAS QUIERO SABER PORQUE PUNTO PASO " + Graph[current].actualPosition);
        while (invertedPath.ContainsKey(current))
        {
            current = invertedPath[current].id;
            trianglePath.InsertRange(0,Graph[current].triangle);
            //if (invertedPath.ContainsKey(invertedPath[current].id)) trianglePath.InsertRange(0, IntersectedTriangles(current, invertedPath[current].id));
            //Debug.Log(current + "con un coste gScore " + gScore[current] + "y el coso de fScore " + fScore[current] + " Y ADEMAS QUIERO SABER PORQUE PUNTO PASO " + Graph[current].actualPosition);
        }
        return trianglePath;
    }

    /*public List<Vector3> IntersectedTriangles(int triangleA, int triangleB)
    {
        List<Neighbor> neighbors = Graph[triangleA].Neighbors;
        List<Vector3> aux = new List<Vector3>();
        List<Vector2> segment;
        segment.Add(new Vector2());
        segment.Add(new Vector2());
        foreach (Neighbor nei in neighbors)
        {
            if (nei.NeighborID() != triangleB && triangleIntersectSegment(nei.triangle, Vector2)) aux.AddRange(nei.triangle());
        }
        return aux;
    }*/
    //Pintar todos los triangulos atravesados por el camino minimo
    // EL problema esta al asignar una nueva potencial position, ya que ahora mismo me estoy quedando con la que llegaria al triangulo destino, cuando en realidad para llegar a mi destino parto de otro punto.
    public float distanceBetweenNodes(int act, Neighbor nei)
    {
        //return Vector3.Distance(Graph[act].Getbarycenter(), nei.neighbor.Getbarycenter());
        if (nei.byVertex)
        {
            //Graph[act].Neighbors.Remove(nei);
            // nei.neighbor.potencialPosition = nei.getAdjPoints()[0];
            //Graph[nei.NeighborID()].potencialPosition = nei.getAdjPoints()[0];
            //Graph[act].Neighbors.Add(nei);
            //Debug.Log(nei.neighbor.id + "el tamaño es " + nei.getAdjPoints().Count);
            //return Vector2.Distance(new Vector2(Graph[act].actualPosition.x, Graph[act].actualPosition.z), new Vector2(nei.getAdjPoints()[0].x, nei.getAdjPoints()[0].z));
            return Vector2.Distance(new Vector2(destination.x, destination.z), new Vector2(nei.getAdjPoints()[0].x, nei.getAdjPoints()[0].z));
        }
        else {
            //Graph[act].Neighbors.Remove(nei);
            //float x = Graph[act].actualPosition.x;
            //float y = Graph[act].actualPosition.z;
            float x = destination.x;
            float y = destination.z;
            //Debug.Log(Graph[act].actualPosition + " y tiene un id " + nei.neighbor.id);
            //Debug.Log("Vertex1 : " + nei.getAdjPoints()[0] + " Vertex2: " + nei.getAdjPoints()[1]);
            float x1 = nei.getAdjPoints()[0].x;
            float y1 = nei.getAdjPoints()[0].z;
            float x2 = nei.getAdjPoints()[1].x;
            float y2 = nei.getAdjPoints()[1].z;

            float A = x - x1;
            float B = y - y1;
            float C = x2 - x1;
            float D = y2 - y1;

            float dot = A * C + B * D;
            float len_sq = C * C + D * D;
            float param = -1;
            if (len_sq != 0) //in case of 0 length line
                param = dot / len_sq;

            float xx, yy;

            if (param < 0)
            {
                xx = x1;
                yy = y1;
            }
            else if (param > 1)
            {
                xx = x2;
                yy = y2;
            }
            else
            {
                xx = x1 + param * C;
                yy = y1 + param * D;
            }

            var dx = x - xx;
            var dy = y - yy;
            Vector3 aux = new Vector3(xx, 20.1f, yy);
            //Debug.Log(aux);
            //nei.neighbor.potencialPosition = aux;
            Graph[nei.NeighborID()].potencialPosition = aux;
            return (float) Math.Sqrt(dx * dx + dy * dy);
        }
    }
    public List<Vector3> trianglePath()
    {
        int start = initialNode();
        Debug.Log(start);
        int last = LastNode();
        Graph[start].actualPosition = origin;
        openSet.Add(start);
        gScore[start] = 0;
        //La pos
        fScore[start] = heuristicCost(Graph[start].actualPosition);
        int count = 0;
        while (openSet.Count > 0)
        {
            int current = getTheMinimum();
            //Debug.Log("De momento tenemos que estamos mirando " + current);
            //foreach (int k in openSet) Debug.Log(gScore[k] + " del nodo " + k);
            if (current == last)
            {
                return reconstructTrianglePath(current);
            }
            openSet.Remove(current);
            closedSet.Add(current);
            List<Neighbor> neighbors = Graph[current].Neighbors;
            foreach (Neighbor neigh in neighbors)
            {
                //Debug.Log("MIRANDO POR TODOS LOS VECINOS DE " + current);

                int neighID = neigh.NeighborID();
                if (closedSet.Contains(neighID)) continue;

                float potencialNewScore = gScore[current] + distanceBetweenNodes(current, neigh);
                if (!openSet.Contains(neighID)) openSet.Add(neighID);
                else if (potencialNewScore >= gScore[neighID]) continue;
                // This path is the best until now. Record it!
                invertedPath[neighID] = Graph[current];
                gScore[neighID] = potencialNewScore;
                Graph[neighID].actualPosition = Graph[neighID].potencialPosition;
                fScore[neighID] = gScore[neighID] + heuristicCost(Graph[neighID].actualPosition);
                //Debug.Log("El gScore es " + gScore[neighID] + " del nodo que miramos es " + neighID);
            }
            //Debug.Log("FIN DE BUCLE");
            ++count;
        }
        return new List<Vector3>();
    }
}
