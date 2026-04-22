using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Utilities.Containers.Tests
{
    /// <summary>
    /// Unit tests for DiGraph.DepthFirstSearch.
    /// The returned stack holds vertices in reverse postorder, so popping the
    /// stack yields a topological ordering for DAGs.
    /// </summary>
    public class DFSTests
    {
        private static DiGraph<string> CreateGraphWithVertices(params string[] vertices)
        {
            var graph = new DiGraph<string>();
            foreach (var v in vertices)
            {
                graph.AddVertex(v);
            }
            return graph;
        }

        private static List<T> StackToPopOrder<T>(Stack<T> stack)
        {
            var result = new List<T>(stack.Count);
            while (stack.Count > 0)
            {
                result.Add(stack.Pop());
            }
            return result;
        }

        #region Empty and Trivial Graphs

        [Fact]
        public void DepthFirstSearch_EmptyGraph_ReturnsEmptyStack()
        {
            var graph = new DiGraph<string>();

            var stack = graph.DepthFirstSearch();

            Assert.Empty(stack);
        }

        [Fact]
        public void DepthFirstSearch_SingleVertex_ReturnsStackWithThatVertex()
        {
            var graph = CreateGraphWithVertices("A");

            var stack = graph.DepthFirstSearch();

            Assert.Single(stack);
            Assert.Equal("A", stack.Peek());
        }

        [Fact]
        public void DepthFirstSearch_IsolatedVertices_ReturnsAllVertices()
        {
            var graph = CreateGraphWithVertices("A", "B", "C");

            var stack = graph.DepthFirstSearch();

            Assert.Equal(3, stack.Count);
            Assert.Contains("A", stack);
            Assert.Contains("B", stack);
            Assert.Contains("C", stack);
        }

        #endregion

        #region Linear Chain

        [Fact]
        public void DepthFirstSearch_SimpleChain_PopOrderIsTopological()
        {
            var graph = CreateGraphWithVertices("A", "B", "C");
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");

            var popOrder = StackToPopOrder(graph.DepthFirstSearch());

            Assert.Equal(new List<string> { "A", "B", "C" }, popOrder);
        }

        [Fact]
        public void DepthFirstSearch_LongerChain_PopOrderIsTopological()
        {
            var graph = CreateGraphWithVertices("A", "B", "C", "D", "E");
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");
            graph.AddEdge("C", "D");
            graph.AddEdge("D", "E");

            var popOrder = StackToPopOrder(graph.DepthFirstSearch());

            Assert.Equal(new List<string> { "A", "B", "C", "D", "E" }, popOrder);
        }

        #endregion

        #region DAG Topological Ordering

        [Fact]
        public void DepthFirstSearch_DiamondDag_PopOrderIsValidTopologicalSort()
        {
            var graph = CreateGraphWithVertices("A", "B", "C", "D");
            graph.AddEdge("A", "B");
            graph.AddEdge("A", "C");
            graph.AddEdge("B", "D");
            graph.AddEdge("C", "D");

            var popOrder = StackToPopOrder(graph.DepthFirstSearch());

            Assert.Equal(4, popOrder.Count);
            // Each source must precede its destination in the pop order.
            Assert.True(popOrder.IndexOf("A") < popOrder.IndexOf("B"));
            Assert.True(popOrder.IndexOf("A") < popOrder.IndexOf("C"));
            Assert.True(popOrder.IndexOf("B") < popOrder.IndexOf("D"));
            Assert.True(popOrder.IndexOf("C") < popOrder.IndexOf("D"));
        }

        [Fact]
        public void DepthFirstSearch_WideDag_AllVerticesAppearOnce()
        {
            var graph = CreateGraphWithVertices("A", "B", "C", "D", "E", "F");
            graph.AddEdge("A", "B");
            graph.AddEdge("A", "C");
            graph.AddEdge("B", "D");
            graph.AddEdge("C", "E");
            graph.AddEdge("D", "F");
            graph.AddEdge("E", "F");

            var popOrder = StackToPopOrder(graph.DepthFirstSearch());

            Assert.Equal(6, popOrder.Count);
            Assert.Equal(6, popOrder.Distinct().Count());
            Assert.True(popOrder.IndexOf("A") < popOrder.IndexOf("F"));
            Assert.True(popOrder.IndexOf("B") < popOrder.IndexOf("D"));
            Assert.True(popOrder.IndexOf("C") < popOrder.IndexOf("E"));
            Assert.True(popOrder.IndexOf("D") < popOrder.IndexOf("F"));
            Assert.True(popOrder.IndexOf("E") < popOrder.IndexOf("F"));
        }

        #endregion

        #region Cycles and Self-Loops

        [Fact]
        public void DepthFirstSearch_GraphWithCycle_VisitsEachVertexExactlyOnce()
        {
            var graph = CreateGraphWithVertices("A", "B", "C");
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");
            graph.AddEdge("C", "A");

            var stack = graph.DepthFirstSearch();
            var popOrder = StackToPopOrder(stack);

            Assert.Equal(3, popOrder.Count);
            Assert.Equal(3, popOrder.Distinct().Count());
            Assert.Contains("A", popOrder);
            Assert.Contains("B", popOrder);
            Assert.Contains("C", popOrder);
        }

        [Fact]
        public void DepthFirstSearch_SelfLoop_TerminatesAndVisitsVertexOnce()
        {
            var graph = CreateGraphWithVertices("A");
            graph.AddEdge("A", "A");

            var stack = graph.DepthFirstSearch();

            Assert.Single(stack);
            Assert.Equal("A", stack.Peek());
        }

        [Fact]
        public void DepthFirstSearch_TwoNodeCycle_VisitsBothVerticesOnce()
        {
            var graph = CreateGraphWithVertices("A", "B");
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "A");

            var popOrder = StackToPopOrder(graph.DepthFirstSearch());

            Assert.Equal(2, popOrder.Count);
            Assert.Contains("A", popOrder);
            Assert.Contains("B", popOrder);
        }

        #endregion

        #region Disconnected Components

        [Fact]
        public void DepthFirstSearch_DisconnectedComponents_VisitsAllVertices()
        {
            var graph = CreateGraphWithVertices("A", "B", "C", "D");
            graph.AddEdge("A", "B");
            graph.AddEdge("C", "D");

            var popOrder = StackToPopOrder(graph.DepthFirstSearch());

            Assert.Equal(4, popOrder.Count);
            Assert.True(popOrder.IndexOf("A") < popOrder.IndexOf("B"));
            Assert.True(popOrder.IndexOf("C") < popOrder.IndexOf("D"));
        }

        [Fact]
        public void DepthFirstSearch_IsolatedAndConnectedMix_VisitsEveryVertex()
        {
            var graph = CreateGraphWithVertices("A", "B", "C", "D", "E");
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");
            // D and E are isolated.

            var popOrder = StackToPopOrder(graph.DepthFirstSearch());

            Assert.Equal(5, popOrder.Count);
            Assert.Equal(5, popOrder.Distinct().Count());
            Assert.True(popOrder.IndexOf("A") < popOrder.IndexOf("B"));
            Assert.True(popOrder.IndexOf("B") < popOrder.IndexOf("C"));
        }

        #endregion

        #region Shared Descendants

        [Fact]
        public void DepthFirstSearch_MultipleEdgesToSameVertex_VisitsTargetOnlyOnce()
        {
            var graph = CreateGraphWithVertices("A", "B", "C", "D");
            graph.AddEdge("A", "D");
            graph.AddEdge("B", "D");
            graph.AddEdge("C", "D");

            var popOrder = StackToPopOrder(graph.DepthFirstSearch());

            Assert.Equal(4, popOrder.Count);
            Assert.Single(popOrder.Where(v => v == "D"));
            Assert.True(popOrder.IndexOf("A") < popOrder.IndexOf("D"));
            Assert.True(popOrder.IndexOf("B") < popOrder.IndexOf("D"));
            Assert.True(popOrder.IndexOf("C") < popOrder.IndexOf("D"));
        }

        [Fact]
        public void DepthFirstSearch_IntegerVertices_ProducesTopologicalOrder()
        {
            var graph = new DiGraph<int>();
            foreach (var v in new[] { 1, 2, 3, 4 })
            {
                graph.AddVertex(v);
            }
            graph.AddEdge(1, 2);
            graph.AddEdge(1, 3);
            graph.AddEdge(2, 4);
            graph.AddEdge(3, 4);

            var popOrder = StackToPopOrder(graph.DepthFirstSearch());

            Assert.Equal(4, popOrder.Count);
            Assert.True(popOrder.IndexOf(1) < popOrder.IndexOf(2));
            Assert.True(popOrder.IndexOf(1) < popOrder.IndexOf(3));
            Assert.True(popOrder.IndexOf(2) < popOrder.IndexOf(4));
            Assert.True(popOrder.IndexOf(3) < popOrder.IndexOf(4));
        }

        #endregion

        #region Determinism

        [Fact]
        public void DepthFirstSearch_CalledTwice_ProducesSameResult()
        {
            var graph = CreateGraphWithVertices("A", "B", "C", "D");
            graph.AddEdge("A", "B");
            graph.AddEdge("A", "C");
            graph.AddEdge("B", "D");
            graph.AddEdge("C", "D");

            var firstPopOrder = StackToPopOrder(graph.DepthFirstSearch());
            var secondPopOrder = StackToPopOrder(graph.DepthFirstSearch());

            Assert.Equal(firstPopOrder, secondPopOrder);
        }

        #endregion
    }
}
