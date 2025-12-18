using System;
using UnityEngine;

public class DataInsightUIController : MonoBehaviour
{
    [SerializeField] private ScoreGraphTile[] scoreGraphTiles;
    [SerializeField] private TextAsset dataFeedSample;

    private GraphData graphData;

    private void Start()
    {
        InitialiseGraphs(dataFeedSample.text);
    }

    private void InitialiseGraphs(string json)
    {
        graphData = JsonUtility.FromJson<GraphData>(json);

        int graphCount = Mathf.Min(scoreGraphTiles.Length, graphData.graphs.Length);

        for (int i = 0; i < graphCount; i++)
        {
            var graph = graphData.graphs[i];

            scoreGraphTiles[i].SetGraphData(
                graph.label,
                graph.scores
            );
        }
    }

    public void IncreaseDayTick(int dayIndex)
    {
        foreach (var tile in scoreGraphTiles)
        {
            tile.UpdateGraphTile(dayIndex);
        }
    }
}


[Serializable]
public class GraphData
{
    public GraphSeries[] graphs;
}

[Serializable]
public class GraphSeries
{
    public string id;
    public string label;
    public int[] scores;
}
