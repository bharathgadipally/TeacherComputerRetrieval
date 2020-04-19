using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;


namespace TeacherComputerRetrieval.Tests
{
    [TestClass]
    public class GraphsDirectedWeightedGraphTest
    {
        DirectedWeightedGraph<string> _graph;

        [TestInitialize]
        public void Initialize()
        {
            _graph = new DirectedWeightedGraph<string>();

            var verticesSet1 = new string[] { "a", "z", "s", "x", "d", "c", "f", "v" };

            _graph.AddVertices(verticesSet1);

            _graph.AddEdge("a", "s", 1);
            _graph.AddEdge("a", "z", 2);
            _graph.AddEdge("s", "x", 3);
            _graph.AddEdge("x", "d", 1);
            _graph.AddEdge("x", "c", 2);
            _graph.AddEdge("x", "a", 3);
            _graph.AddEdge("d", "f", 1);
            _graph.AddEdge("d", "c", 2);
            _graph.AddEdge("d", "s", 3);
            _graph.AddEdge("c", "f", 1);
            _graph.AddEdge("c", "v", 2);
            _graph.AddEdge("c", "d", 3);
            _graph.AddEdge("v", "f", 1);
            _graph.AddEdge("f", "c", 2);

        }

        [TestMethod]
        public void Verify_Vertices_And_Edges_Count()
        {
            Assert.IsTrue(_graph.VerticesCount == 8, "Wrong vertices count.");
            Assert.IsTrue(_graph.EdgesCount == 14, "Wrong edges count.");
            var allEdges = _graph.Edges.ToList();
            Assert.IsTrue(_graph.EdgesCount == allEdges.Count, "Wrong edges count.");
        }

        [TestMethod]
        public void Verify_Outgoing_And_Incoming_Edges_Count()
        {
            Assert.IsTrue(_graph.OutgoingEdges("a").ToList().Count == 2, "Wrong outgoing edges from 'a'.");
            Assert.IsTrue(_graph.OutgoingEdges("s").ToList().Count == 1, "Wrong outgoing edges from 's'.");
            Assert.IsTrue(_graph.OutgoingEdges("d").ToList().Count == 3, "Wrong outgoing edges from 'd'.");
            Assert.IsTrue(_graph.OutgoingEdges("x").ToList().Count == 3, "Wrong outgoing edges from 'x'.");
            Assert.IsTrue(_graph.OutgoingEdges("c").ToList().Count == 3, "Wrong outgoing edges from 'c'.");
            Assert.IsTrue(_graph.OutgoingEdges("v").ToList().Count == 1, "Wrong outgoing edges from 'v'.");
            Assert.IsTrue(_graph.OutgoingEdges("f").ToList().Count == 1, "Wrong outgoing edges from 'f'.");
            Assert.IsTrue(_graph.OutgoingEdges("z").ToList().Count == 0, "Wrong outgoing edges from 'z'.");

            Assert.IsTrue(_graph.IncomingEdges("a").ToList().Count == 1, "Wrong incoming edges from 'a'.");
            Assert.IsTrue(_graph.IncomingEdges("s").ToList().Count == 2, "Wrong incoming edges from 's'.");
            Assert.IsTrue(_graph.IncomingEdges("d").ToList().Count == 2, "Wrong incoming edges from 'd'.");
            Assert.IsTrue(_graph.IncomingEdges("x").ToList().Count == 1, "Wrong incoming edges from 'x'.");
            Assert.IsTrue(_graph.IncomingEdges("c").ToList().Count == 3, "Wrong incoming edges from 'c'.");
            Assert.IsTrue(_graph.IncomingEdges("v").ToList().Count == 1, "Wrong incoming edges from 'v'.");
            Assert.IsTrue(_graph.IncomingEdges("f").ToList().Count == 3, "Wrong incoming edges from 'f'.");
            Assert.IsTrue(_graph.IncomingEdges("z").ToList().Count == 1, "Wrong incoming edges from 'z'.");
        }

        [TestMethod]
        public void Verify_Edge_And_Weights()
        {
            var f_to_c = _graph.HasEdge("f", "c");
            var f_to_c_weight = _graph.GetEdgeWeight("f", "c");
            Assert.IsTrue(f_to_c == true, "Edge f->c doesn't exist.");
            Assert.IsTrue(f_to_c_weight == 2, "Edge f->c must have a weight of 2.");

            var d_to_s = _graph.HasEdge("d", "s");
            var d_to_s_weight = _graph.GetEdgeWeight("d", "s");
            Assert.IsTrue(d_to_s == true, "Edge d->s doesn't exist.");
            Assert.IsTrue(d_to_s_weight == 3, "Edge d->s must have a weight of 3.");
        }

        [TestMethod]
        public void Verify_Remove_Edge()
        {
            _graph.RemoveEdge("d", "c");
            _graph.RemoveEdge("c", "v");
            _graph.RemoveEdge("a", "z");
            Assert.IsTrue(_graph.VerticesCount == 8, "Wrong vertices count.");
            Assert.IsTrue(_graph.EdgesCount == 11, "Wrong edges count.");
        }
    }

}

