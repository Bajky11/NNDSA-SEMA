using NNDSA_SEMA.src;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

public class Graph
{
    // Dictionary to hold the list of adjacency lists. Each key is a node,
    // and its value is the list of nodes it points to.
    private Dictionary<int, List<int>> adjList;

    // Constructor
    public Graph()
    {
        adjList = new Dictionary<int, List<int>>();
    }

    // Method to add a vertex to the graph
    public void AddVertex(int vertex)
    {
        if (!adjList.ContainsKey(vertex))
        {
            adjList[vertex] = new List<int>();
        }
    }

    // Method to add an edge from vertex u to vertex v
    public void AddEdge(int u, int v)
    {
        // If u or v are not already in the graph, they are added.
        if (!adjList.ContainsKey(u))
        {
            AddVertex(u);
        }
        if (!adjList.ContainsKey(v))
        {
            AddVertex(v);
        }

        // Add an edge from u to v
        adjList[u].Add(v);
    }

    // Method to print the adjacency list of each vertex
    public void PrintGraph()
    {
        foreach (var vertex in adjList)
        {
            Console.Write($"Vertex {vertex.Key}: ");
            foreach (var edge in vertex.Value)
            {
                Console.Write($"{edge} ");
            }
            Console.WriteLine();
        }
    }

    public List<List<int>> FindAllPathsBetweenStartAndEnd(int[] startVertices, int[] endVertices)
    {
        List<List<int>> allPaths = new List<List<int>>();

        foreach (var startVertex in startVertices)
        {
            foreach (var endVertex in endVertices)
            {
                List<int> path = new List<int>();
                path.Add(startVertex);
                DFSUtil(startVertex, endVertex, new HashSet<int>(), path, allPaths);
            }
        }

        return allPaths;
    }

    public List<List<int>> FindAllPathsBetweenTwoVertices(int startVertex, int endVertex)
    {
        List<List<int>> allPaths = new List<List<int>>();
        List<int> path = new List<int>();
        path.Add(startVertex);
        DFSUtil(startVertex, endVertex, new HashSet<int>(), path, allPaths);
        return allPaths;
    }

    private void DFSUtil(int vertex, int targetVertex, HashSet<int> visited, List<int> path, List<List<int>> allPaths)
    {
        visited.Add(vertex);

        if (vertex == targetVertex && path.Count > 1)
        {
            allPaths.Add(new List<int>(path));
            return;
        }

        foreach (var neighbor in adjList[vertex])
        {
            if (!visited.Contains(neighbor))
            {
                List<int> newPath = new List<int>(path);
                newPath.Add(neighbor);
                DFSUtil(neighbor, targetVertex, new HashSet<int>(visited), newPath, allPaths);
            }
        }
    }
}


class Program
{
    static int[] startVertices = { 23, 21, 22, 24, 30, 29 };
    static int[] endVertices = { 30, 29, 27, 28 };

    static int[] vertices = { 23, 12, 14, 30, 17, 29, 18, 27, 21, 22, 15, 16, 19, 28, 24, 13 };
    static int[,] edges = new int[,]{
    { 23, 12 },
    { 12, 14 },
    { 14, 30 },
    { 30, 17 },
    { 17, 29 },
    { 29, 18 },
    { 18, 27 },
    { 18, 19 },
    { 21, 14 },
    { 22, 15 },
    { 15, 16 },
    { 16, 17 },
    { 16, 19 },
    { 19, 28 },
    { 24, 13 },
    { 13, 15 }
};

    static void Main(string[] args)
    {
        /*
        Graph graph = new Graph();

        foreach (int number in verticies)
        {
            graph.AddVertex(number);
        }

        for (int i = 0; i < data.GetLength(0); i++)
        {
            graph.AddEdge(data[i, 0], data[i, 1]);
        }

        Console.WriteLine("\nAll paths between start and end vertices:");
        List<List<int>> allPaths = graph.FindAllPathsBetweenStartAndEnd(startVertices, endVertices);
        foreach (var path in allPaths)
        {
            Console.WriteLine(string.Join(" -> ", path));
        }
        Console.WriteLine(allPaths.Count);

        Console.WriteLine("\nAll paths between vertex 23 and vertex 17:");
        List<List<int>> paths23To17 = graph.FindAllPathsBetweenTwoVertices(23, 17);
        foreach (var path in paths23To17)
        {
            Console.WriteLine(string.Join(" -> ", path));
        }
        */











        // Create a graph with integer vertices and string edges
        GenerericGraph<int, string> graph = new GenerericGraph<int, string>();

        // Fill the graph with vertices
        foreach (var vertex in vertices)
        {
            graph.AddVertex(vertex, $"V{vertex}");
        }

        // Fill the graph with edges and associated data
        for (int i = 0; i < edges.GetLength(0); i++)
        {
            int startVertex = edges[i, 0];
            int endVertex = edges[i, 1];
            string edgeData = $"Edge from {startVertex} to {endVertex}";
            graph.AddEdge(startVertex, endVertex, edgeData);
        }

        graph.PrintGraph();


        Console.WriteLine("All paths found:");
        var allPaths = graph.FindAllPathsBetweenStartAndEndVertexes(startVertices, endVertices);
        printAllPaths(allPaths);
    }
    static void printAllPaths(List<List<int>> allPaths)
    {
        foreach (var path in allPaths)
        {
            if (path.Count > 1)
            {
                Console.WriteLine(string.Join(" -> ", path));
            }
    }
    }
}
