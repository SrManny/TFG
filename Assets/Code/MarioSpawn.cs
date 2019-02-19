using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MarioSpawn : MonoBehaviour
{
    public GameObject Mariobros;
    GameObject ListOfMarios;
    Dictionary<int, Node> Graph;
    NavMeshTriangulation triangulization;
    Mesh path;
    int count = 0;
    public Material material;
    // Start is called before the first frame update
    void Start()
    {
        triangulization = NavMesh.CalculateTriangulation();
        path = GetComponent<MeshFilter>().mesh;
        path.Clear();
        Graph = new Dictionary<int, Node>();
        ListOfMarios = new GameObject();
        ListOfMarios.transform.parent = this.transform;
        ListOfMarios.name = "ListOfMarios";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Spawn();
        }
        else if (Input.GetKeyDown(KeyCode.Delete))
        {
            foreach (Transform child in ListOfMarios.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            Mesh ola = this.GetComponent<MeshFilter>().mesh;
            ola.Clear();
        }
    }

    private void Spawn()
    {
        GameObject MarioSpawned = Instantiate(Mariobros, new Vector3(35 + Random.Range(-30, 25.0f), 20, -45 + Random.Range(-30 , 20)), transform.rotation);
        MarioSpawned.transform.parent = ListOfMarios.transform;
        MarioSpawned.name = " " + count;
        MarioMove script = MarioSpawned.GetComponent<MarioMove>();
        script._destination = this.gameObject.transform.GetChild(3).transform;
        ++count;
        //script.Graph = astar.Graph;
    }

}
