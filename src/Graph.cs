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

    internal class GenerericGraph<K, E> : IGenericGraph<K, E>
    {
        private Dictionary<K, Vertex<K, E>> vertices;
        private Dictionary<Tuple<K, K>, E> edges;

        public GenerericGraph()
        {
            vertices = new Dictionary<K, Vertex<K, E>>();
            edges = new Dictionary<Tuple<K, K>, E>();
        }

        public void AddVertex(K key, E data)
        {
            if (vertices.ContainsKey(key))
            {
                throw new InvalidOperationException("Vertex already exists.");
            }
            vertices[key] = new Vertex<K, E>(key, data);
        }

        public void AddEdge(K keyOfStartVertex, K keyOfEndVertex, E edgeData)
        {
            if (!vertices.ContainsKey(keyOfStartVertex) || !vertices.ContainsKey(keyOfEndVertex))
            {
                throw new InvalidOperationException("Start or end vertex does not exist.");
            }

            vertices[keyOfStartVertex].AddNeighbor(vertices[keyOfEndVertex]);
            edges[new Tuple<K, K>(keyOfStartVertex, keyOfEndVertex)] = edgeData;
        }

        public void RemoveEdge(K keyOfStartVertex, K keyOfEndVertex)
        {
            if (!vertices.ContainsKey(keyOfStartVertex) || !vertices.ContainsKey(keyOfEndVertex))
            {
                throw new InvalidOperationException("Start or end vertex does not exist.");
            }

            vertices[keyOfStartVertex].RemoveNeighbor(vertices[keyOfEndVertex]);
            edges.Remove(new Tuple<K, K>(keyOfStartVertex, keyOfEndVertex));
        }

        public void RemoveVertex(K key)
        {
            if (!vertices.ContainsKey(key))
            {
                throw new InvalidOperationException("Vertex does not exist.");
            }

            var vertexToRemove = vertices[key];
            vertices.Remove(key);

            foreach (var vertex in vertices.Values)
            {
                vertex.RemoveNeighbor(vertexToRemove);
            }

            // Remove edges connected to the removed vertex
            var edgesToRemove = edges.Keys.Where(t => t.Item1.Equals(key) || t.Item2.Equals(key)).ToList();
            foreach (var edgeToRemove in edgesToRemove)
            {
                edges.Remove(edgeToRemove);
            }
        }

        public void PrintGraph()
        {
            Console.WriteLine();
            foreach (var vertex in vertices.Values)
            {
                Console.Write($"Vertex {vertex.Key} ({vertex.Data}): ");
                foreach (var neighbor in vertex.Neighbors)
                {
                    var edgeData = edges[new Tuple<K, K>(vertex.Key, neighbor.Key)];
                    Console.Write($"{neighbor.Key} ({edgeData}) ");
                }
                Console.WriteLine();
            }
        }

        public List<List<K>> FindAllPathsBetweenTwoVertexes(K startVertexKey, K endVertexKey)
        {
            List<List<K>> allPaths = new List<List<K>>();
            HashSet<K> visited = new HashSet<K>();
            List<K> currentPath = new List<K>();
            FindAllPathsRecursive(startVertexKey, endVertexKey, visited, currentPath, allPaths);
            return allPaths;
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


        private void FindAllPathsRecursive(K currentVertexKey, K endVertexKey, HashSet<K> visited, List<K> currentPath, List<List<K>> allPaths)
        {
            visited.Add(currentVertexKey);
            currentPath.Add(currentVertexKey);

            if (currentVertexKey.Equals(endVertexKey))
            {
                allPaths.Add(new List<K>(currentPath));
            }
            else
            {
                foreach (var neighbor in vertices[currentVertexKey].Neighbors)
                {
                    if (!visited.Contains(neighbor.Key))
                    {
                        FindAllPathsRecursive(neighbor.Key, endVertexKey, visited, currentPath, allPaths);
                    }
                }
            }

            visited.Remove(currentVertexKey);
            currentPath.Remove(currentVertexKey);
        }
    }

    public interface IVertex<K, E>
    {
        K Key { get; }
        E Data { get; }
        List<Vertex<K, E>> Neighbors { get; }

        void AddNeighbor(Vertex<K, E> neighbor);
        void RemoveNeighbor(Vertex<K, E> neighbor);
    }

    public class Vertex<K, E> : IVertex<K, E>
    {
        public K Key { get; }
        public E Data { get; }
        public List<Vertex<K, E>> Neighbors { get; }

        public Vertex(K key, E data)
        {
            Key = key;
            Data = data;
            Neighbors = new List<Vertex<K, E>>();
        }

        public void AddNeighbor(Vertex<K, E> neighbor)
        {
            if (!Neighbors.Contains(neighbor))
            {
                Neighbors.Add(neighbor);
            }
        }

        public void RemoveNeighbor(Vertex<K, E> neighbor)
        {
            Neighbors.Remove(neighbor);
        }
    }

    public interface IEdge<K, E>
    {
        Vertex<K, E> StartVertex { get; }
        Vertex<K, E> EndVertex { get; }
        E Data { get; }
    }

    internal class Edge<K, E> : IEdge<K, E>
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
