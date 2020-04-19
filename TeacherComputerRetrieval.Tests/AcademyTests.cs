using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TeacherComputerRetrieval.Algorithms;

namespace TeacherComputerRetrieval.Tests
{
    [TestClass]
    public class AcademyTests
    {
        DirectedWeightedGraph<string> directedWeightedGraph;
        DijkstraAllPairsShortestPaths<DirectedWeightedGraph<string>, string> dijkstraAlgo;

        [TestInitialize]
        public void Initialize()
        {
            var inputRoutes = new(string SourceAcademy, string DestinationAcademy, int Distance)[]{
                ("A", "B", 5),
                ("B", "C", 4),
                ("C", "D", 8),
                ("D", "C", 8),
                ("D", "E", 6),
                ("A", "D", 5),
                ("C", "E", 2),
                ("E", "B", 3),
                ("A", "E", 7)
            };

            directedWeightedGraph = new DirectedWeightedGraph<string>();
            directedWeightedGraph.AddVertices(new string []{"A", "B", "C", "D", "E"});
            foreach (var route in inputRoutes)
            {
                directedWeightedGraph.AddEdge(route.SourceAcademy, route.DestinationAcademy, route.Distance);
            }
            dijkstraAlgo = new DijkstraAllPairsShortestPaths<DirectedWeightedGraph<string>, string>(directedWeightedGraph);
        }

        [TestMethod]
        public void Edge_Distance_Between_Vertices_ABC()
        {
            Assert.IsTrue(directedWeightedGraph.GetConsecutiveEdgeWeights(new string[] { "A", "B", "C"}) == "9");
        }

        [TestMethod]
        public void Edge_Distance_Between_Vertices_AEBCD()
        {
            Assert.IsTrue(directedWeightedGraph.GetConsecutiveEdgeWeights(new string[] { "A", "E", "B", "C", "D" }) == "22");
        }

        [TestMethod]
        public void Edge_Distance_Between_Vertices_AED()
        {
            Assert.IsTrue(directedWeightedGraph.GetConsecutiveEdgeWeights(new string[] { "A", "E", "D" }) == "NO SUCH ROUTE");
        }

        [TestMethod]
        public void Shortest_Distance_Between_Vertices_AC()
        {
            Assert.IsTrue(dijkstraAlgo.PathDistance("A", "C") == 9);
        }

        [TestMethod]
        public void All_Cyclic_Paths_From_Vertex_C()
        {
            var pathList = directedWeightedGraph.GetAllCyclicPaths("C", true);
            Assert.IsTrue(pathList.Where(x => x.Key.Count <= 4).Count() == 2);
        }

        [TestMethod]
        public void Number_of_Paths_Between_Vertices_AC()
        {
            var pathList = directedWeightedGraph.GetAllPaths("A", "C");
            Assert.IsTrue((pathList.Count(path => path.Key.Count == 5) == 3));
        }

        [TestMethod]
        public void Shortest_Distance_Between_Vertices_BB()
        {
            Assert.IsTrue(dijkstraAlgo.ShortestCyclicDistance("B") == 9);
        }

        [TestMethod]
        public void Cyclic_Paths_From_Vertices_Limit_C()
        {
            var pathList = directedWeightedGraph.GetLimitedCyclicPaths("C", 30);
            Assert.IsTrue(pathList.Count() == 7);
        }
    }
}
