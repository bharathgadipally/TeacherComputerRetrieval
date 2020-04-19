/***
 * The Directed Weighted Graph Data Structure.
 * 
 * Definition:
 * A dense graph is a graph G = (V, E) in which |E| = O(|V|^2).
 * A directed graph is a graph where each edge follow one direction only between any two vertices.
 * A weighted graph is a graph where each edge has a weight (zero weights mean there is no edge).
 * 
 * An adjacency-matrix (two dimensional array of longs) weighted digraph representation. 
 * Inherits and extends the Directed Dense verion (DirectedDenseGraph<T> class).
 * Implements the IWeightedGraph<T> interface.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using TeacherComputerRetrieval.DataStructures;
using TeacherComputerRetrieval.Utlility;

namespace TeacherComputerRetrieval
{

    /// <summary>
    /// This class represents the graph as an adjacency-matrix (two dimensional integer array).
    /// </summary>
    public class DirectedWeightedGraph<T> : IGraph<T>, IWeightedGraph<T> where T : IComparable<T>
    {
        #region Instance Variables
        private const long EMPTY_EDGE_SLOT = 0;
        private const object EMPTY_VERTEX_SLOT = (object)null;
        protected int _edgesCount { get; set; }
        protected int _verticesCount { get; set; }
        protected int _verticesCapacity { get; set; }
        protected ArrayList<object> _vertices { get; set; }
        protected Dictionary<T, int> _nodesToIndices { get; set; }
        protected T _firstInsertedNode { get; set; }

        /// <summary>
        // Store edges and their weights as integers.
        // Any edge with a value of zero means it doesn't exist. Otherwise, it exist with a specific weight value.
        // Default value for positive edges is 1.
        /// </summary>
        protected long[,] _adjacencyMatrix { get; set; }
        #endregion

        public DirectedWeightedGraph(uint capacity = 10)
        {
            _edgesCount = 0;
            _verticesCount = 0;
            _verticesCapacity = (int)capacity;

            _nodesToIndices = new Dictionary<T, int>(_verticesCapacity);
            _vertices = new ArrayList<object>(_verticesCapacity);
            _adjacencyMatrix = new long[_verticesCapacity, _verticesCapacity];
            _adjacencyMatrix.Populate(rows: _verticesCapacity, columns: _verticesCapacity, defaultValue: EMPTY_EDGE_SLOT);
        }

        #region Public Properties
        
        /// <summary>
        /// Returns true, if graph is weighted; false otherwise.
        /// </summary>
        public bool IsWeighted
        {
            get { return true; }
        }
        public bool IsDirected
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the count of vetices.
        /// </summary>
        public int VerticesCount
        {
            get { return _verticesCount; }
        }

        /// <summary>
        /// Gets the count of edges.
        /// </summary>
        public int EdgesCount
        {
            get { return _edgesCount; }
        }

        /// <summary>
        /// Returns the list of Vertices.
        /// </summary>
        public IEnumerable<T> Vertices
        {
            get
            {
                foreach (var vertex in _vertices)
                    if (vertex != null)
                        yield return (T)vertex;
            }
        }

        /// <summary>
        /// An enumerable collection of all weighted directed edges in graph.
        /// </summary>
        public IEnumerable<IEdge<T>> Edges
        {
            get
            {
                foreach (var vertex in _vertices)
                    foreach (var outgoingEdge in OutgoingEdges((T)vertex))
                        yield return outgoingEdge;
            }
        }

        IEnumerable<IEdge<T>> IGraph<T>.Edges
        {
            get { return this.Edges; }
        }
        #endregion

        IEnumerable<IEdge<T>> IGraph<T>.IncomingEdges(T vertex)
        {
            return this.IncomingEdges(vertex);
        }

        /// <summary>
        /// Get all incoming unweighted edges to a vertex.
        /// </summary>
        public IEnumerable<WeightedEdge<T>> IncomingEdges(T vertex)
        {
            if (!HasVertex(vertex))
                throw new KeyNotFoundException("Vertex doesn't belong to graph.");

            int source = _vertices.IndexOf(vertex);

            for (int adjacent = 0; adjacent < _vertices.Count; ++adjacent)
            {
                if (_vertices[adjacent] != null && _doesEdgeExist(adjacent, source))
                {
                    yield return (new WeightedEdge<T>(
                        (T)_vertices[adjacent],             // from
                        vertex,                             // to
                        _getEdgeWeight(source, adjacent)    // weight
                    ));
                }
            }//end-for
        }

        /// <summary>
        /// Get all outgoing unweighted edges from a vertex.
        /// </summary>
        public IEnumerable<IEdge<T>> OutgoingEdges(T vertex)
        {
            if (!HasVertex(vertex))
                throw new KeyNotFoundException("Vertex doesn't belong to graph.");

            int source = _vertices.IndexOf(vertex);

            for (int adjacent = 0; adjacent < _vertices.Count; ++adjacent)
            {
                if (_vertices[adjacent] != null && _doesEdgeExist(source, adjacent))
                {
                    yield return (new WeightedEdge<T>(
                        vertex,                             // from
                        (T)_vertices[adjacent],             // to
                        _getEdgeWeight(source, adjacent)    // weight
                    ));
                }
            }//end-for
        }

        /// <summary>
        /// Obsolete. Another AddEdge function is implemented with a weight parameter.
        /// </summary>
        [Obsolete("Use the AddEdge method with the weight parameter.")]
        public bool AddEdge(T source, T destination)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Connects two vertices together with a weight, in the direction: first->second.
        /// </summary>
        public bool AddEdge(T source, T destination, long weight)
        {
            // Return if the weight is equals to the empty edge value
            if (weight == EMPTY_EDGE_SLOT)
                return false;

            // Get indices of vertices
            int srcIndex = _vertices.IndexOf(source);
            int dstIndex = _vertices.IndexOf(destination);

            // Check existence of vertices and non-existence of edge
            if (srcIndex == -1 || dstIndex == -1)
                return false;
            if (_doesEdgeExist(srcIndex, dstIndex))
                return false;

            _adjacencyMatrix[srcIndex, dstIndex] = weight;

            // Increment edges count
            ++_edgesCount;

            return true;
        }

        /// <summary>
        /// Removes edge, if exists, from source to destination.
        /// </summary>
        public bool RemoveEdge(T source, T destination)
        {
            // Get indices of vertices
            int srcIndex = _vertices.IndexOf(source);
            int dstIndex = _vertices.IndexOf(destination);

            // Check existence of vertices and non-existence of edge
            if (srcIndex == -1 || dstIndex == -1)
                return false;
            if (!_doesEdgeExist(srcIndex, dstIndex))
                return false;

            _adjacencyMatrix[srcIndex, dstIndex] = EMPTY_EDGE_SLOT;

            // Increment edges count
            --_edgesCount;

            return true;
        }

        /// <summary>
        /// Add a collection of vertices to the graph.
        /// </summary>
        public void AddVertices(IList<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException();

            foreach (var vertex in collection)
                AddVertex(vertex);
        }

        /// <summary>
        /// Add vertex to the graph
        /// </summary>
        public bool AddVertex(T vertex)
        {
            // Return of the capacity is reached
            if (_verticesCount >= _verticesCapacity)
                return false;

            // Return if vertex already exists
            if (HasVertex(vertex))
                return false;

            // Initialize first inserted node
            if (_verticesCount == 0)
                _firstInsertedNode = vertex;

            // Try inserting vertex at previously lazy-deleted slot
            int indexOfNull = _vertices.IndexOf(EMPTY_VERTEX_SLOT);

            if (indexOfNull != -1)
                _vertices[indexOfNull] = vertex;
            else
                _vertices.Add(vertex);

            _nodesToIndices.Add(vertex, _verticesCount);

            // Increment vertices count
            ++_verticesCount;
            return true;
        }

        /// <summary>
        /// Removes the specified vertex from graph.
        /// </summary>
        public bool RemoveVertex(T vertex)
        {
            // Return if graph is empty
            if (_verticesCount == 0)
                return false;

            // Get index of vertex
            int index = _vertices.IndexOf(vertex);

            // Return if vertex doesn't exists
            if (index == -1)
                return false;

            // Lazy-delete the vertex from graph
            //_vertices.Remove (vertex);
            _vertices[index] = EMPTY_VERTEX_SLOT;

            // Decrement the vertices count
            --_verticesCount;

            // Remove all outgoing and incoming edges to this vertex
            for (int i = 0; i < _verticesCapacity; ++i)
            {
                // Outgoing edge
                if (_doesEdgeExist(index, i))
                {
                    _adjacencyMatrix[index, i] = EMPTY_EDGE_SLOT;

                    // Decrement the edges count
                    --_edgesCount;
                }

                // Incoming edge
                if (_doesEdgeExist(i, index))
                {
                    _adjacencyMatrix[i, index] = EMPTY_EDGE_SLOT;

                    // Decrement the edges count
                    --_edgesCount;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks whether there is an edge from source to destination.
        /// </summary>
        public bool HasEdge(T source, T destination)
        {
            // Get indices of vertices
            int srcIndex = _vertices.IndexOf(source);
            int dstIndex = _vertices.IndexOf(destination);

            // Check the existence of vertices and the directed edge
            return (srcIndex != -1 && dstIndex != -1 && _doesEdgeExist(srcIndex, dstIndex));
        }

        /// <summary>
        /// Get edge object from source to destination.
        /// </summary>
        public WeightedEdge<T> GetEdge(T source, T destination)
        {
            // Get indices of vertices
            int srcIndex = _vertices.IndexOf(source);
            int dstIndex = _vertices.IndexOf(destination);

            // Check the existence of vertices and the directed edge
            if (srcIndex == -1 || dstIndex == -1)
                throw new Exception("One of the vertices or both of them doesn't exist.");
            if (!_doesEdgeExist(srcIndex, dstIndex))
                throw new Exception("Edge doesn't exist.");

            return (new WeightedEdge<T>(source, destination, _getEdgeWeight(srcIndex, dstIndex)));
        }

        /// <summary>
        /// Returns the edge weight from source to destination.
        /// </summary>
        public long GetEdgeWeight(T source, T destination)
        {
            return GetEdge(source, destination).Weight;
        }

        /// <summary>
        /// Return the sum of weights for array of vertices
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public string GetConsecutiveEdgeWeights(T[] vertices)
        {
            long totalWeight = 0;
            for (int i = 0; i < vertices.Length - 1; i++)
            {
                T source = vertices[i];
                T destination = vertices[i + 1];
                // Get indices of vertices
                int srcIndex = _vertices.IndexOf(source);
                int dstIndex = _vertices.IndexOf(destination);

                if (srcIndex == -1)
                    throw new Exception($"Vertex {source} doesn't exist.");

                if (dstIndex == -1)
                    throw new Exception($"Vertex {destination} doesn't exist.");

                if (!_doesEdgeExist(srcIndex, dstIndex))
                    return "NO SUCH ROUTE";

                totalWeight = totalWeight + new WeightedEdge<T>(source, destination, _getEdgeWeight(srcIndex, dstIndex)).Weight;
            }

            return totalWeight.ToString();
        }

        /// <summary>
        /// Determines whether this graph has the specified vertex.
        /// </summary>
        public bool HasVertex(T vertex)
        {
            return _vertices.Contains(vertex);
        }

        /// <summary>
        /// Returns the neighbours doubly-linked list for the specified vertex.
        /// </summary>
        public DLinkedList<T> Neighbours(T vertex)
        {
            var neighbors = new DLinkedList<T>();
            int source = _vertices.IndexOf(vertex);

            // Check existence of vertex
            if (source != -1)
                for (int adjacent = 0; adjacent < _vertices.Count; ++adjacent)
                    if (_vertices[adjacent] != null && _doesEdgeExist(source, adjacent))
                        neighbors.Append((T)_vertices[adjacent]);

            return neighbors;
        }

        /// <summary>
        /// Returns the neighbours of a vertex as a dictionary of nodes-to-weights.
        /// </summary>
        public Dictionary<T, long> NeighboursMap(T vertex)
        {
            if (!HasVertex(vertex))
                return null;

            var neighbors = new Dictionary<T, long>();
            int source = _vertices.IndexOf(vertex);

            // Check existence of vertex
            if (source != -1)
                for (int adjacent = 0; adjacent < _vertices.Count; ++adjacent)
                    if (_vertices[adjacent] != null && _doesEdgeExist(source, adjacent))
                        neighbors.Add((T)_vertices[adjacent], _getEdgeWeight(source, adjacent));

            return neighbors;
        }

        /// <summary>
        /// Clear this graph.
        /// </summary>
        public void Clear()
        {
            _edgesCount = 0;
            _verticesCount = 0;
            _vertices = new ArrayList<object>(_verticesCapacity);
            _adjacencyMatrix = new long[_verticesCapacity, _verticesCapacity];
            _adjacencyMatrix.Populate(rows: _verticesCapacity, columns: _verticesCapacity, defaultValue: EMPTY_EDGE_SLOT);
        }

        /// <summary>
        /// Gets all possible repeating cyclic paths with wight limit from the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<List<T>, long>> GetLimitedCyclicPaths(T source, long limit)
        {
            var cyclicPaths = GetAllCyclicPaths(source);
            Dictionary<List<T>, long> finalCombinedPaths = new Dictionary<List<T>, long>(cyclicPaths);
            CombinePathsByLimit(cyclicPaths, cyclicPaths, limit, ref finalCombinedPaths);
            return finalCombinedPaths;
        }

        /// <summary>
        /// Gets all Cyclic paths for the given source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="includeSourceVetex"></param>
        /// <returns></returns>
        public Dictionary<List<T>, long> GetAllCyclicPaths(T source, bool includeSourceVetex = false)
        {
            bool[] isVisited = new bool[_verticesCount];
            List<T> pathList = new List<T>();
            Dictionary<List<T>, long> allpathsList = new Dictionary<List<T>, long>();

            foreach (var neighbour in Neighbours(source))
            {
                foreach (var path in GetAllPaths(neighbour, source, false))
                {
                    if (includeSourceVetex)
                        path.Key.Insert(0, source);

                    allpathsList.Add(path.Key, path.Value + GetEdgeWeight(source, neighbour));
                }
            }

            return allpathsList;
        }

        /// <summary>
        /// Returns list of paths from source to destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public Dictionary<List<T>, long> GetAllPaths(T source, T destination, bool addCyclicPaths = true)
        {
            bool[] isVisited = new bool[_verticesCount];
            List<T> pathList = new List<T>();
            Dictionary<List<T>, long> allpathsList = new Dictionary<List<T>, long>();
            // add source to path[]  
            pathList.Add(source);

            // Call recursive utility  
            GetAllPathsUtil(source, destination, isVisited, pathList, 0, ref allpathsList);

            if (addCyclicPaths)
            {
                var newCyclicPaths = new Dictionary<List<T>, long>();
                foreach (var cyclicPath in GetAllCyclicPaths(destination))
                {
                    foreach (var path in allpathsList)
                    {
                        var newPath = path.Key.ToList();
                        newPath.AddRange(cyclicPath.Key);
                        newCyclicPaths.Add(newPath, path.Value + cyclicPath.Value);
                    }
                }
                foreach (var newCyclicPath in newCyclicPaths)
                {
                    allpathsList.Add(newCyclicPath.Key, newCyclicPath.Value);
                }
            }

            return allpathsList;
        }

        #region Private Methods

        /// <summary>
        ///  Recursive method for itreating all paths from source Vertex to desination vertex
        /// </summary>
        /// <param name="sourceVertex"></param>
        /// <param name="destinationVertex"></param>
        /// <param name="isVisited"></param>
        /// <param name="localPathList"></param>
        /// <param name="weight"></param>
        /// <param name="allpathsList"></param>
        private void GetAllPathsUtil(T sourceVertex, T destinationVertex, bool[] isVisited, List<T> localPathList, long weight, ref Dictionary<List<T>, long> allpathsList)
        {
            // Mark the current node  
            isVisited[_nodesToIndices[sourceVertex]] = true;

            if (_nodesToIndices[sourceVertex].Equals(_nodesToIndices[destinationVertex]))
            {
                allpathsList.Add(localPathList.ToList(), weight);

                // if match found then no need  
                // to traverse more till depth  
                isVisited[_nodesToIndices[sourceVertex]] = false;
                return;
            }

            // Recur for all the vertices  
            // adjacent to current vertex  
            foreach (var i in NeighboursMap(sourceVertex))
            {
                if (!isVisited[_nodesToIndices[i.Key]])
                {
                    // store current node  
                    // in path[]  
                    localPathList.Add(i.Key);
                    weight = weight + i.Value;
                    GetAllPathsUtil(i.Key, destinationVertex, isVisited, localPathList, weight, ref allpathsList);

                    // remove current node  
                    // in path[]  
                    localPathList.Remove(i.Key);
                    weight = weight - i.Value;
                }
            }

            // Mark the current node  
            isVisited[_nodesToIndices[sourceVertex]] = false;
        }

        /// <summary>
        /// Cross Map all the array elements from array1 to array 2 and return combined paths
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <param name="limit"></param>
        /// <param name="finalArray"></param>
        private void CombinePathsByLimit(Dictionary<List<T>, long> array1, Dictionary<List<T>, long> array2, long limit, ref Dictionary<List<T>, long> finalArray)
        {
            Dictionary<List<T>, long> resultArray = new Dictionary<List<T>, long>();

            if (array1.Count == 0)
            {
                return;
            }

            foreach (var arrayItem1 in array1)
            {
                foreach (var arrayItem2 in array2)
                {
                    var combinedWeightPath = arrayItem1.Value + arrayItem2.Value;
                    if (combinedWeightPath < limit)
                    {
                        var arrayItem1Clone = arrayItem1.Key.ToList();
                        arrayItem1Clone.AddRange(arrayItem2.Key);
                        resultArray.Add(arrayItem1Clone, combinedWeightPath);
                    }
                }
            }

            foreach (var item in resultArray)
                finalArray.Add(item.Key, item.Value);

            CombinePathsByLimit(resultArray, array2, limit, ref finalArray);
        }

        /// <summary>
        /// Helper function. Gets the weight of a directed edge.
        /// </summary>
        private long _getEdgeWeight(int source, int destination)
        {
            return _adjacencyMatrix[source, destination];
        }

        /// <summary>
        /// Helper function. Checks if edge exist in graph.
        /// </summary>
        private bool _doesEdgeExist(int source, int destination)
        {
            return (_adjacencyMatrix[source, destination] != EMPTY_EDGE_SLOT);
        }

        #endregion

    }

}

