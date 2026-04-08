using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Utilities.Containers.Tests
{
    /// <summary>
    /// Comprehensive unit tests for DiGraph.
    /// Covers all currently implemented public methods.
    /// </summary>
    public class DiGraphTests
    {
        private sealed class TestDiGraph : DiGraph<string>
        {
            public int VertexCountInternal => _adjacencyList.Count;
            public bool ContainsVertex(string vertex) => _adjacencyList.ContainsKey(vertex);
        }

        private static TestDiGraph CreateGraphWithVertices(params string[] vertices)
        {
            var graph = new TestDiGraph();
            foreach (var v in vertices)
            {
                graph.AddVertex(v);
            }
            return graph;
        }

        #region Constructor and AddVertex Tests

        [Fact]
        public void Constructor_NewGraph_HasZeroVerticesAndEdges()
        {
            var graph = new TestDiGraph();

            Assert.Equal(0, graph.VertexCount());
            Assert.Equal(0, graph.EdgeCount());
            Assert.Empty(graph.GetVertices());
        }

        [Fact]
        public void AddVertex_NewVertex_ReturnsTrue()
        {
            var graph = new TestDiGraph();

            bool added = graph.AddVertex("A");

            Assert.True(added);
            Assert.True(graph.ContainsVertex("A"));
            Assert.Equal(1, graph.VertexCountInternal);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("Node_01")]
        [InlineData("vertex-with-dashes")]
        public void AddVertex_VariousValidNames_AddsSuccessfully(string vertex)
        {
            var graph = new TestDiGraph();

            bool added = graph.AddVertex(vertex);

            Assert.True(added);
            Assert.True(graph.ContainsVertex(vertex));
        }

        [Fact]
        public void AddVertex_DuplicateVertex_ReturnsFalse()
        {
            var graph = new TestDiGraph();
            graph.AddVertex("A");

            bool addedAgain = graph.AddVertex("A");

            Assert.False(addedAgain);
            Assert.Equal(1, graph.VertexCountInternal);
        }

        #endregion

        #region AddEdge and HasEdge Tests

        [Fact]
        public void AddEdge_ValidVertices_ReturnsTrueAndCreatesEdge()
        {
            var graph = CreateGraphWithVertices("A", "B");

            bool added = graph.AddEdge("A", "B");

            Assert.True(added);
            Assert.True(graph.HasEdge("A", "B"));
        }

        [Theory]
        [InlineData("A", "B")]
        [InlineData("A", "C")]
        [InlineData("B", "C")]
        public void AddEdge_MultiplePairs_AddsEachEdge(string source, string destination)
        {
            var graph = CreateGraphWithVertices("A", "B", "C");

            bool added = graph.AddEdge(source, destination);

            Assert.True(added);
            Assert.True(graph.HasEdge(source, destination));
        }

        [Fact]
        public void AddEdge_DuplicateEdge_ReturnsFalseAndKeepsSingleEdge()
        {
            var graph = CreateGraphWithVertices("A", "B");

            bool firstAdd = graph.AddEdge("A", "B");
            bool secondAdd = graph.AddEdge("A", "B");

            Assert.True(firstAdd);
            Assert.False(secondAdd);
            Assert.Equal(1, graph.EdgeCount());

            Assert.True(graph.RemoveEdge("A", "B"));
            Assert.False(graph.HasEdge("A", "B"));
        }

        [Theory]
        [InlineData("X", "B")]
        [InlineData("A", "Y")]
        [InlineData("X", "Y")]
        public void AddEdge_MissingVertex_ThrowsArgumentException(string source, string destination)
        {
            var graph = CreateGraphWithVertices("A", "B");

            Assert.Throws<ArgumentException>(() => graph.AddEdge(source, destination));
        }

        [Fact]
        public void HasEdge_NoEdgeBetweenExistingVertices_ReturnsFalse()
        {
            var graph = CreateGraphWithVertices("A", "B");

            Assert.False(graph.HasEdge("A", "B"));
        }

        [Fact]
        public void HasEdge_MissingSource_ThrowsKeyNotFoundException()
        {
            var graph = CreateGraphWithVertices("A");

            Assert.Throws<KeyNotFoundException>(() => graph.HasEdge("B", "A"));
        }

        [Fact]
        public void HasEdge_MissingDestination_ReturnsFalse()
        {
            var graph = CreateGraphWithVertices("A", "B");
            graph.AddEdge("A", "B");

            Assert.False(graph.HasEdge("A", "C"));
        }

        #endregion

        #region RemoveVertex and RemoveEdge Tests

        [Fact]
        public void RemoveVertex_ExistingVertex_RemovesVertexAndIncidentEdges()
        {
            var graph = CreateGraphWithVertices("A", "B", "C");
            graph.AddEdge("A", "B");
            graph.AddEdge("C", "B");
            graph.AddEdge("B", "A");

            bool removed = graph.RemoveVertex("B");

            Assert.True(removed);
            Assert.False(graph.ContainsVertex("B"));
            Assert.False(graph.HasEdge("A", "B"));
            Assert.False(graph.HasEdge("C", "B"));
            Assert.Equal(0, graph.EdgeCount());
            Assert.True(graph.AddVertex("B"));
        }

        [Fact]
        public void RemoveVertex_MissingVertex_ThrowsArgumentException()
        {
            var graph = CreateGraphWithVertices("A");

            Assert.Throws<ArgumentException>(() => graph.RemoveVertex("B"));
        }

        [Theory]
        [InlineData("A")]
        [InlineData("B")]
        [InlineData("C")]
        public void RemoveVertex_AnyExistingVertex_DecrementsVertexCount(string vertex)
        {
            var graph = CreateGraphWithVertices("A", "B", "C");

            bool removed = graph.RemoveVertex(vertex);

            Assert.True(removed);
            Assert.Equal(2, graph.VertexCount());
            Assert.DoesNotContain(vertex, graph.GetVertices());
        }

        [Fact]
        public void RemoveEdge_ExistingEdge_ReturnsTrueAndRemovesEdge()
        {
            var graph = CreateGraphWithVertices("A", "B");
            graph.AddEdge("A", "B");

            bool removed = graph.RemoveEdge("A", "B");

            Assert.True(removed);
            Assert.False(graph.HasEdge("A", "B"));
            Assert.Equal(0, graph.EdgeCount());
        }

        [Fact]
        public void RemoveEdge_MissingEdge_ReturnsFalse()
        {
            var graph = CreateGraphWithVertices("A", "B");

            bool removed = graph.RemoveEdge("A", "B");

            Assert.False(removed);
        }

        [Theory]
        [InlineData("A", "Z")]
        [InlineData("Z", "A")]
        [InlineData("Z", "Y")]
        public void RemoveEdge_InvalidVertices_ThrowsArgumentException(string source, string destination)
        {
            var graph = CreateGraphWithVertices("A", "B");

            Assert.Throws<ArgumentException>(() => graph.RemoveEdge(source, destination));
        }

        #endregion

        #region GetNeighbors and GetVertices Tests

        [Fact]
        public void GetNeighbors_VertexWithNoOutgoingEdges_ReturnsEmptyList()
        {
            var graph = CreateGraphWithVertices("A");

            var neighbors = graph.GetNeighbors("A");

            Assert.Empty(neighbors);
        }

        [Fact]
        public void GetNeighbors_VertexWithMultipleEdges_ReturnsAllNeighbors()
        {
            var graph = CreateGraphWithVertices("A", "B", "C", "D");
            graph.AddEdge("A", "B");
            graph.AddEdge("A", "C");
            graph.AddEdge("A", "D");

            var neighbors = graph.GetNeighbors("A");

            Assert.Equal(3, neighbors.Count);
            Assert.Equal(new List<string> { "B", "C", "D" }, neighbors);
        }

        [Fact]
        public void GetNeighbors_MissingVertex_ThrowsArgumentException()
        {
            var graph = CreateGraphWithVertices("A", "B");

            Assert.Throws<ArgumentException>(() => graph.GetNeighbors("Z"));
        }

        [Fact]
        public void GetNeighbors_ReturnsCopy_NotBackedByGraphStorage()
        {
            var graph = CreateGraphWithVertices("A", "B", "C");
            graph.AddEdge("A", "B");

            var neighbors = graph.GetNeighbors("A");
            neighbors.Add("C");

            Assert.Single(graph.GetNeighbors("A"));
            Assert.True(graph.HasEdge("A", "B"));
            Assert.False(graph.HasEdge("A", "C"));
        }

        [Fact]
        public void GetVertices_EmptyGraph_ReturnsEmptySequence()
        {
            var graph = new TestDiGraph();

            Assert.Empty(graph.GetVertices());
        }

        [Fact]
        public void GetVertices_AfterAddingVertices_ReturnsAllVertices()
        {
            var graph = CreateGraphWithVertices("A", "B", "C");

            var vertices = graph.GetVertices().ToList();

            Assert.Equal(3, vertices.Count);
            Assert.Contains("A", vertices);
            Assert.Contains("B", vertices);
            Assert.Contains("C", vertices);
        }

        #endregion

        #region Count Tests

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public void VertexCount_MatchesNumberOfAddedDistinctVertices(int vertexCount)
        {
            var graph = new TestDiGraph();
            for (int i = 0; i < vertexCount; i++)
            {
                graph.AddVertex($"V{i}");
            }

            Assert.Equal(vertexCount, graph.VertexCount());
        }

        [Fact]
        public void EdgeCount_TracksAddsAndRemoves()
        {
            var graph = CreateGraphWithVertices("A", "B", "C");

            Assert.Equal(0, graph.EdgeCount());

            graph.AddEdge("A", "B");
            graph.AddEdge("B", "C");
            Assert.Equal(2, graph.EdgeCount());

            graph.RemoveEdge("A", "B");
            Assert.Equal(1, graph.EdgeCount());
        }

        [Fact]
        public void EdgeCount_DuplicateAddAttempt_DoesNotIncreaseCount()
        {
            var graph = CreateGraphWithVertices("A", "B");
            graph.AddEdge("A", "B");

            bool secondAdd = graph.AddEdge("A", "B");

            Assert.False(secondAdd);
            Assert.Equal(1, graph.EdgeCount());
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_EmptyGraph_ReturnsEmptyString()
        {
            var graph = new TestDiGraph();

            string output = graph.ToString();

            Assert.Equal(string.Empty, output);
        }

        [Fact]
        public void ToString_NonEmptyGraph_ContainsVertexAndEdgeInformation()
        {
            var graph = CreateGraphWithVertices("A", "B", "C");
            graph.AddEdge("A", "B");
            graph.AddEdge("A", "C");

            string output = graph.ToString();

            Assert.Contains("Node A connects to:", output);
            Assert.Contains("B", output);
            Assert.Contains("C", output);
        }

        [Fact]
        public void ToString_VertexWithNoOutgoingEdges_StillIncludesVertexHeader()
        {
            var graph = CreateGraphWithVertices("Solo");

            string output = graph.ToString();

            Assert.Contains("Node Solo connects to:", output);
        }

        #endregion
    }
}
