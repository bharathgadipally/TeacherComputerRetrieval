# Teacher Computer Retrieval
The IT team is responsible for retrieving teacher computers that are broken and returning them to HQ for repair and replacement. The teaching academies are connected with routes (one-way and two-way) between them. The purpose of this program is to provide information about possible routes between nodes like

1. The distance along certain routes.
2. The number of different routes between two academies.
3. The shortest route between two academies.

Since we have the list academies and their routes directed specifically, a Directed Wegihted Graph Data structure would suite to solve this problem.

 ### The Directed Weighted Graph Data Structure.
  
  Definition:
  A dense graph is a graph G = (V, E) in which |E| = O(|V|^2).
  A directed graph is a graph where each edge follow one direction only between any two vertices. Cyclic paths are allowed
  A weighted graph is a graph where each edge has a weight (zero weights mean there is no edge).
 
  An adjacency-matrix (two dimensional array of longs) weighted digraph representation.
 
 ### Shortest Distance Algorithms
  Dijkstra algorithm helps us to find the shortest paths from a single source vertex to all destination vertices in a non-negative    Directed and weighted graph. The same can be run for all the vertices.
  
 ### Recursive Methods
  To generate cyclic paths, get all paths from source to its neighbours and from each neighbour back to the source vertex. Now concatenate or map all these route combinations until the limit path weight specified.

## Data

## Running the tests
1. Download or clone the repository
2. Open the Solution in Visual Studio. Open with sln file.
3. Build the Solution.
4. Under the TeacherComputerRetrieval.Tests project, Run all the test methods.
5. AcademyTests.cs has all the written test methods for the given problem
