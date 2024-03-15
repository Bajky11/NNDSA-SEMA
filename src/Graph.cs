using System;
using System.Collections.Generic;
using System.Linq;

namespace NNDSA_SEMA.src
{
    public interface IGenericGraph<K, E>
    {
        void AddVertex(K key, E data);
        void RemoveVertex(K key);
        void AddEdge(K keyOfStartVertex, K keyOfEndVertex, E edgeData);
        void RemoveEdge(K keyOfStartVertex, K keyOfEndVertex);
        void PrintGraph();
        List<List<K>> FindAllPathsBetweenTwoVertexes(K startVertexKey, K endVertexKey);
    }

    public class GenericGraph<K, E> : IGenericGraph<K, E>
    {
        private Dictionary<K, Vertex<K, E>> adjencyList = new Dictionary<K, Vertex<K, E>>();

        public void AddVertex(K key, E data)
        {
            if (adjencyList.ContainsKey(key))
                throw new InvalidOperationException("Vertex already exists.");
            adjencyList[key] = new Vertex<K, E>(key, data);
        }

        public void RemoveVertex(K key)
        {
            if (!adjencyList.ContainsKey(key))
                throw new InvalidOperationException("Vertex does not exist.");

            var vertexToRemove = adjencyList[key];
            adjencyList.Remove(key);

            // Remove this vertex from the neighbors of other vertices and remove the corresponding edges
            foreach (var vertex in adjencyList.Values)
            {
                if (vertex.Neighbors.Contains(vertexToRemove))
                {
                    vertex.RemoveEdge(vertexToRemove);
                }
            }
        }

        public void AddEdge(K startKey, K endKey, E data)
        {
            if (!adjencyList.ContainsKey(startKey) || !adjencyList.ContainsKey(endKey))
                throw new InvalidOperationException("One or both vertices do not exist.");

            var startVertex = adjencyList[startKey];
            var endVertex = adjencyList[endKey];
            startVertex.AddEdge(endVertex, data);
        }

        public void RemoveEdge(K startKey, K endKey)
        {
            if (!adjencyList.ContainsKey(startKey) || !adjencyList.ContainsKey(endKey))
                throw new InvalidOperationException("One or both vertices do not exist.");

            adjencyList[startKey].RemoveEdge(adjencyList[endKey]);
        }

        public void PrintGraph()
        {
            Console.WriteLine();
            foreach (var vertex in adjencyList.Values)
            {
                Console.Write($"Vertex {vertex.Key} ({vertex.Data}): ");
                foreach (var edge in vertex.Edges)
                {
                    Console.Write($"{edge.EndVertex.Key} ({edge.Data}) ");
                }
                Console.WriteLine();
            }
        }

        public List<List<K>> FindAllPathsBetweenStartAndEndVertexes(K[] startVertexes, K[] endVertexes)
        {
            List<List<K>> allPaths = new List<List<K>>();
            foreach (var startVertex in startVertexes)
            {
                foreach (var endVertex in endVertexes)
                {
                    var allPathsBetweenTwoVertexes = FindAllPathsBetweenTwoVertexes(startVertex, endVertex);
                    foreach (var path in allPathsBetweenTwoVertexes)
                    {
                        if (path.Count > 1)
                        {
                            allPaths.Add(path);
                        }
                    }
                }
            }
            return allPaths;
        }


        public List<List<K>> FindAllPathsBetweenTwoVertexes(K startVertexKey, K endVertexKey)
        {
            List<List<K>> allPaths = new List<List<K>>();
            HashSet<K> visited = new HashSet<K>();
            List<K> currentPath = new List<K>();
            FindAllPathsRecursive(adjencyList[startVertexKey], endVertexKey, visited, currentPath, allPaths);
            return allPaths;
        }

        private void FindAllPathsRecursive(Vertex<K, E> currentVertex, K endVertexKey, HashSet<K> visited, List<K> currentPath, List<List<K>> allPaths)
        {
            visited.Add(currentVertex.Key);
            currentPath.Add(currentVertex.Key);

            if (currentVertex.Key.Equals(endVertexKey))
            {
                allPaths.Add(new List<K>(currentPath));
            }
            else
            {
                foreach (var neighbor in currentVertex.Neighbors)
                {
                    if (!visited.Contains(neighbor.Key))
                    {
                        FindAllPathsRecursive(neighbor, endVertexKey, visited, currentPath, allPaths);
                    }
                }
            }

            visited.Remove(currentVertex.Key);
            currentPath.RemoveAt(currentPath.Count - 1);
        }

        public class Vertex<K, E>
        {
            public K Key { get; }
            public E Data { get; }
            public List<Edge<K, E>> Edges { get; } = new List<Edge<K, E>>();
            public List<Vertex<K, E>> Neighbors { get; } = new List<Vertex<K, E>>();

            public Vertex(K key, E data)
            {
                Key = key;
                Data = data;
            }

            public void AddEdge(Vertex<K, E> toVertex, E edgeData)
            {
                Edges.Add(new Edge<K, E>(this, toVertex, edgeData));
                if (!Neighbors.Contains(toVertex))
                {
                    Neighbors.Add(toVertex);
                }
            }

            public void RemoveEdge(Vertex<K, E> toVertex)
            {
                Edges.RemoveAll(edge => edge.EndVertex.Equals(toVertex));
                Neighbors.Remove(toVertex);
            }
        }

        public class Edge<K, E>
        {
            public Vertex<K, E> StartVertex { get; }
            public Vertex<K, E> EndVertex { get; }
            public E Data { get; }

            public Edge(Vertex<K, E> startVertex, Vertex<K, E> endVertex, E data)
            {
                StartVertex = startVertex;
                EndVertex = endVertex;
                Data = data;
            }
        }
    }
}
