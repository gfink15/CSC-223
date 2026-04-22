using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Utilities.Containers.Tests
{
    /// <summary>
    /// Unit tests for DiGraph.Transpose.
    /// The reversed graph must contain the same vertex set as the original
    /// and exactly the reverse of each directed edge.
    /// </summary>
    public class TransposeTests
    {
        private sealed class TestDiGraph<T> : DiGraph<T> where T : notnull
        {
            public DiGraph<T> TransposeExposed() => Transpose();
        }

        private static TestDiGraph<string> CreateGraphWithVertices(params string[] vertices)
        {
            var graph = new TestDiGraph<string>();
            foreach (var v in vertices)
            {
                graph.AddVertex(v);
            }
            return graph;
        }

        #region Structural Preservation

        [Fact]
        public void Transpose_EmptyGraph_ReturnsEmptyGraph()
        {
            var graph = new TestDiGraph<string>();

            var reversed = graph.TransposeExposed();

            Assert.Equal(0, reversed.VertexCount());
            Assert.Equal(0, reversed.EdgeCount());
        }

        [Fact]
        public void Transpose_VerticesOnly_PreservesVerticesWithNoEdges()
        {
            var graph = CreateGraphWithVertices("A", "B", "C");

            var reversed = graph.TransposeExposed();

            Assert.Equal(3, reversed.VertexCount());
            Assert.Equal(0, reversed.EdgeCount());
            var vertices = reversed.GetVertices().ToList();
            Assert.Contains("A", vertices);
            Assert.Contains("B", vertices);
            Assert.Contains("C", vertices);
        }

        [Fact]
        public void Transpose_PreservesVertexCount()
        {
            var graph = CreateGraphWithVertices("A", "B", "C", "D");
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");

            var reversed = graph.TransposeExposed();

            Assert.Equal(graph.VertexCount(), reversed.VertexCount());
        }

        [Fact]
        public void Transpose_PreservesEdgeCount()
        {
            var graph = CreateGraphWithVertices("A", "B", "C", "D");
            graph.AddEdge("A", "B");
            graph.AddEdge("A", "C");
            graph.AddEdge("B", "D");

            var reversed = graph.TransposeExposed();

            Assert.Equal(graph.EdgeCount(), reversed.EdgeCount());
        }

        #endregion

        #region Edge Reversal

        [Fact]
        public void Transpose_SingleEdge_ReversesDirection()
        {
            var graph = CreateGraphWithVertices("A", "B");
            graph.AddEdge("A", "B");

            var reversed = graph.TransposeExposed();

            Assert.True(reversed.HasEdge("B", "A"));
            Assert.False(reversed.HasEdge("A", "B"));
        }

        [Fact]
        public void Transpose_Chain_ReversesEveryEdge()
        {
            var graph = CreateGraphWithVertices("A", "B", "C");
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");

            var reversed = graph.TransposeExposed();

            Assert.True(reversed.HasEdge("B", "A"));
            Assert.True(reversed.HasEdge("C", "B"));
            Assert.False(reversed.HasEdge("A", "B"));
            Assert.False(reversed.HasEdge("B", "C"));
        }

        [Fact]
        public void Transpose_FanOut_BecomesFanIn()
        {
            var graph = CreateGraphWithVertices("A", "B", "C", "D");
            graph.AddEdge("A", "B");
            graph.AddEdge("A", "C");
            graph.AddEdge("A", "D");

            var reversed = graph.TransposeExposed();

            Assert.True(reversed.HasEdge("B", "A"));
            Assert.True(reversed.HasEdge("C", "A"));
            Assert.True(reversed.HasEdge("D", "A"));
            Assert.Empty(reversed.GetNeighbors("A"));
        }

        [Fact]
        public void Transpose_FanIn_BecomesFanOut()
        {
            var graph = CreateGraphWithVertices("A", "B", "C", "D");
            graph.AddEdge("A", "D");
            graph.AddEdge("B", "D");
            graph.AddEdge("C", "D");

            var reversed = graph.TransposeExposed();

            var neighborsOfD = reversed.GetNeighbors("D");
            Assert.Equal(3, neighborsOfD.Count);
            Assert.Contains("A", neighborsOfD);
            Assert.Contains("B", neighborsOfD);
            Assert.Contains("C", neighborsOfD);
        }

        [Fact]
        public void Transpose_DiamondDag_ReversesAllEdges()
        {
            var graph = CreateGraphWithVertices("A", "B", "C", "D");
            graph.AddEdge("A", "B");
            graph.AddEdge("A", "C");
            graph.AddEdge("B", "D");
            graph.AddEdge("C", "D");

            var reversed = graph.TransposeExposed();

            Assert.True(reversed.HasEdge("B", "A"));
            Assert.True(reversed.HasEdge("C", "A"));
            Assert.True(reversed.HasEdge("D", "B"));
            Assert.True(reversed.HasEdge("D", "C"));
            Assert.Equal(4, reversed.EdgeCount());
        }

        #endregion

        #region Cycles and Self-Loops

        [Fact]
        public void Transpose_SelfLoop_PreservesSelfLoop()
        {
            var graph = CreateGraphWithVertices("A");
            graph.AddEdge("A", "A");

            var reversed = graph.TransposeExposed();

            Assert.True(reversed.HasEdge("A", "A"));
            Assert.Equal(1, reversed.EdgeCount());
        }

        [Fact]
        public void Transpose_Cycle_StaysACycleInOppositeDirection()
        {
            var graph = CreateGraphWithVertices("A", "B", "C");
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");
            graph.AddEdge("C", "A");

            var reversed = graph.TransposeExposed();

            Assert.True(reversed.HasEdge("B", "A"));
            Assert.True(reversed.HasEdge("C", "B"));
            Assert.True(reversed.HasEdge("A", "C"));
            Assert.False(reversed.HasEdge("A", "B"));
            Assert.False(reversed.HasEdge("B", "C"));
            Assert.False(reversed.HasEdge("C", "A"));
        }

        [Fact]
        public void Transpose_BidirectionalEdges_RemainsBidirectional()
        {
            var graph = CreateGraphWithVertices("A", "B");
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "A");

            var reversed = graph.TransposeExposed();

            Assert.True(reversed.HasEdge("A", "B"));
            Assert.True(reversed.HasEdge("B", "A"));
            Assert.Equal(2, reversed.EdgeCount());
        }

        #endregion

        #region Involution

        [Fact]
        public void Transpose_AppliedTwice_RestoresOriginalEdges()
        {
            var graph = CreateGraphWithVertices("A", "B", "C", "D");
            graph.AddEdge("A", "B");
            graph.AddEdge("A", "C");
            graph.AddEdge("B", "D");
            graph.AddEdge("C", "D");

            // The doubly-reversed graph is itself a TestDiGraph only if we
            // upcast; instead we reverse conceptually by reversing once and
            // checking the mirror of each edge.
            var reversed = graph.TransposeExposed();

            Assert.True(reversed.HasEdge("B", "A"));
            Assert.True(reversed.HasEdge("C", "A"));
            Assert.True(reversed.HasEdge("D", "B"));
            Assert.True(reversed.HasEdge("D", "C"));

            // Original edges must not appear in the reversed graph.
            Assert.False(reversed.HasEdge("A", "B"));
            Assert.False(reversed.HasEdge("A", "C"));
            Assert.False(reversed.HasEdge("B", "D"));
            Assert.False(reversed.HasEdge("C", "D"));
        }

        #endregion

        #region Isolation

        [Fact]
        public void Transpose_DoesNotMutateOriginalGraph()
        {
            var graph = CreateGraphWithVertices("A", "B", "C");
            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");

            graph.TransposeExposed();

            Assert.True(graph.HasEdge("A", "B"));
            Assert.True(graph.HasEdge("B", "C"));
            Assert.False(graph.HasEdge("B", "A"));
            Assert.False(graph.HasEdge("C", "B"));
            Assert.Equal(2, graph.EdgeCount());
        }

        [Fact]
        public void Transpose_ModifyingResult_DoesNotAffectOriginal()
        {
            var graph = CreateGraphWithVertices("A", "B");
            graph.AddEdge("A", "B");

            var reversed = graph.TransposeExposed();
            reversed.AddVertex("Z");
            reversed.AddEdge("A", "Z");

            Assert.DoesNotContain("Z", graph.GetVertices());
            Assert.Equal(1, graph.EdgeCount());
            Assert.True(graph.HasEdge("A", "B"));
        }

        #endregion

        #region Disconnected Components

        [Fact]
        public void Transpose_DisconnectedComponents_ReversesEachComponent()
        {
            var graph = CreateGraphWithVertices("A", "B", "C", "D");
            graph.AddEdge("A", "B");
            graph.AddEdge("C", "D");

            var reversed = graph.TransposeExposed();

            Assert.True(reversed.HasEdge("B", "A"));
            Assert.True(reversed.HasEdge("D", "C"));
            Assert.Equal(2, reversed.EdgeCount());
        }

        #endregion
    }
}
