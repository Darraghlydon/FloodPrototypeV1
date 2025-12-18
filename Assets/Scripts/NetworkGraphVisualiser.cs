using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkGraphVisualiser : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject carPrefab;

    [Header("Visual Settings")]
    [SerializeField] private float nodeScale = 1.0f;
    [SerializeField] private float edgeThickness = 0.1f;
    [SerializeField] private Color nodeColor = Color.blue;
    [SerializeField] private Color edgeColor = Color.gray;

    [Header("Traffic Settings")]
    [SerializeField] private int carsPerRoute = 3;
    [SerializeField] private float spawnInterval = 2.0f;
    [SerializeField] private float baseCarSpeed = 80f;

    private Dictionary<string, Vector3> nodePositions = new Dictionary<string, Vector3>()
    {
        {"A", new Vector3(166f, 20, 883f)},
        {"B", new Vector3(147f, 20, 844f)},
        {"C", new Vector3(324f, 20, 460f)},
        {"D", new Vector3(800f, 20, 103f)},
        {"E", new Vector3(852f, 20, 112f)},
        {"F", new Vector3(998f, 20, 33f)},
        {"G", new Vector3(858f, 20, 431f)}
    };

    private Dictionary<string, List<string>> adjacencyList = new Dictionary<string, List<string>>()
    {
        {"A", new List<string> {"B"}},
        {"B", new List<string> {"C"}},
        {"C", new List<string> {"D"}},
        {"D", new List<string> {"E"}},
        {"E", new List<string> {"F"}},
        {"F", new List<string> {"G"}},
        {"G", new List<string> {"A"}}
    };

    private List<List<string>> trafficRoutes = new List<List<string>>()
    {
        new List<string> { "A", "B", "C", "D", "E", "F", "G" },
        new List<string> { "C", "D", "E", "F", "G", "A", "B" },
        new List<string> { "F", "G", "A", "B", "C", "D" }
    };

    private Dictionary<string, GameObject> visualizedNodes = new Dictionary<string, GameObject>();

    private void Start()
    {
        if (nodePrefab == null || carPrefab == null)
        {
            Debug.LogError("NetworkGraphVisualiser: Missing prefab references.");
            return;
        }

        VisualizeNodes();
        VisualizeEdges();

        StartCoroutine(SpawnTrafficLoop());
    }
    private void VisualizeNodes()
    {
        GameObject nodeParent = new GameObject("Nodes");

        foreach (var entry in nodePositions)
        {
            GameObject nodeObj = Instantiate(
                nodePrefab,
                entry.Value,
                Quaternion.identity,
                nodeParent.transform
            );

            nodeObj.name = $"Node_{entry.Key}";
            nodeObj.transform.localScale = Vector3.one * nodeScale;

            if (nodeObj.TryGetComponent(out Renderer renderer))
                renderer.material.color = nodeColor;

            TextMesh label = nodeObj.GetComponentInChildren<TextMesh>();
            if (label != null)
                label.text = entry.Key;

            visualizedNodes.Add(entry.Key, nodeObj);
        }
    }

    private void VisualizeEdges()
    {
        GameObject edgeParent = new GameObject("Edges");

        foreach (var source in adjacencyList)
        {
            Vector3 sourcePos = nodePositions[source.Key];

            foreach (string target in source.Value)
            {
                Vector3 targetPos = nodePositions[target];

                GameObject edgeObj = new GameObject($"Edge_{source.Key}_{target}");
                edgeObj.transform.SetParent(edgeParent.transform);

                LineRenderer lr = edgeObj.AddComponent<LineRenderer>();
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.startColor = edgeColor;
                lr.endColor = edgeColor;
                lr.startWidth = edgeThickness;
                lr.endWidth = edgeThickness;
                lr.positionCount = 2;
                lr.SetPosition(0, sourcePos);
                lr.SetPosition(1, targetPos);
            }
        }
    }

    private IEnumerator SpawnTrafficLoop()
    {
        while (true)
        {
            foreach (var route in trafficRoutes)
            {
                for (int i = 0; i < carsPerRoute; i++)
                {
                    StartTrafficTrip(route, baseCarSpeed);
                    yield return new WaitForSeconds(spawnInterval);
                }
            }
        }
    }

    private void StartTrafficTrip(List<string> routeNodes, float speed)
    {
        if (routeNodes == null || routeNodes.Count < 2)
            return;

        Vector3 startPos = nodePositions[routeNodes[0]];
        GameObject car = Instantiate(carPrefab, startPos, Quaternion.identity);
        car.name = "TrafficCar";

        TrafficCar trafficCar = car.GetComponent<TrafficCar>();
        if (trafficCar == null)
            trafficCar = car.AddComponent<TrafficCar>();

        List<Vector3> routePositions = new List<Vector3>();
        foreach (string node in routeNodes)
            routePositions.Add(nodePositions[node]);

        trafficCar.Initialize(routePositions, speed, loop: true);
    }

    public float GetEdgeDistance(string from, string to)
    {
        if (!nodePositions.ContainsKey(from) || !nodePositions.ContainsKey(to))
            return -1f;

        return Vector3.Distance(nodePositions[from], nodePositions[to]);
    }

    public GameObject GetNodeGameObject(string nodeId)
    {
        return visualizedNodes.TryGetValue(nodeId, out var node)
            ? node
            : null;
    }
}
