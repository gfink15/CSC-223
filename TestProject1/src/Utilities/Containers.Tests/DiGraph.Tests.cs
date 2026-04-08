using System;
using System.Collections.Generic;
using Xunit;

namespace Utilities.Containers.Tests
{
    public class DiGraphTests
    {
        private sealed class TestDiGraph : DiGraph<string>
        {
            public int VertexCountInternal => _adjacencyList.Count;
            public bool ContainsVertex(string vertex) => _adjacencyList.ContainsKey(vertex);
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

        [Fact]
        public void AddVertex_DuplicateVertex_ReturnsFalse()
        {
            var graph = new TestDiGraph();
            graph.AddVertex("A");

            bool addedAgain = graph.AddVertex("A");

            Assert.False(addedAgain);
            Assert.Equal(1, graph.VertexCountInternal);
        }

        [Fact]
        public void AddEdge_ValidVertices_ReturnsTrueAndCreatesEdge()
        {
            var graph = new TestDiGraph();
            graph.AddVertex("A");
            graph.AddVertex("B");

            bool added = graph.AddEdge("A", "B");

            Assert.True(added);
            Assert.True(graph.HasEdge("A", "B"));
        }

        [Fact]
        public void AddEdge_DuplicateEdge_AddsAnotherEdgeInstance()
        {
            var graph = new TestDiGraph();
            graph.AddVertex("A");
            graph.AddVertex("B");

            bool firstAdd = graph.AddEdge("A", "B");
            bool secondAdd = graph.AddEdge("A", "B");

            Assert.True(firstAdd);
            Assert.False(secondAdd);

            // One remove should only remove one instance when duplicates exist.
            Assert.True(graph.RemoveEdge("A", "B"));
            Assert.False(graph.HasEdge("A", "B"));
        }

        [Fact]
        public void AddEdge_MissingSource_ThrowsArgumentException()
        {
            var graph = new TestDiGraph();
            graph.AddVertex("B");

            Assert.Throws<ArgumentException>(() => graph.AddEdge("A", "B"));
        }

        [Fact]
        public void AddEdge_MissingDestination_ThrowsArgumentException()
        {
            var graph = new TestDiGraph();
            graph.AddVertex("A");

            Assert.Throws<ArgumentException>(() => graph.AddEdge("A", "B"));
        }

        [Fact]
        public void RemoveVertex_ExistingVertex_RemovesVertexAndIncidentEdges()
        {
            var graph = new TestDiGraph();
            graph.AddVertex("A");
            graph.AddVertex("B");
            graph.AddVertex("C");
            graph.AddEdge("A", "B");
            graph.AddEdge("C", "B");
            graph.AddEdge("B", "A");

            bool removed = graph.RemoveVertex("B");

            Assert.True(removed);
            Assert.False(graph.ContainsVertex("B"));
            Assert.False(graph.HasEdge("A", "B"));
            Assert.False(graph.HasEdge("C", "B"));
            Assert.True(graph.AddVertex("B"));
        }

        [Fact]
        public void RemoveVertex_MissingVertex_ThrowsArgumentException()
        {
            var graph = new TestDiGraph();
            graph.AddVertex("A");

            Assert.Throws<ArgumentException>(() => graph.RemoveVertex("B"));
        }

        [Fact]
        public void RemoveEdge_ExistingEdge_ReturnsTrueAndRemovesEdge()
        {
            var graph = new TestDiGraph();
            graph.AddVertex("A");
            graph.AddVertex("B");
            graph.AddEdge("A", "B");

            bool removed = graph.RemoveEdge("A", "B");

            Assert.True(removed);
            Assert.False(graph.HasEdge("A", "B"));
        }

        [Fact]
        public void RemoveEdge_MissingEdge_ReturnsFalse()
        {
            var graph = new TestDiGraph();
            graph.AddVertex("A");
            graph.AddVertex("B");

            bool removed = graph.RemoveEdge("A", "B");

            Assert.False(removed);
        }

        [Fact]
        public void RemoveEdge_InvalidVertices_ThrowsArgumentException()
        {
            var graph = new TestDiGraph();
            graph.AddVertex("A");

            Assert.Throws<ArgumentException>(() => graph.RemoveEdge("A", "B"));
        }

        [Fact]
        public void HasEdge_NoEdgeBetweenExistingVertices_ReturnsFalse()
        {
            var graph = new TestDiGraph();
            graph.AddVertex("A");
            graph.AddVertex("B");

            Assert.False(graph.HasEdge("A", "B"));
        }

        [Fact]
        public void HasEdge_MissingSource_ThrowsKeyNotFoundException()
        {
            var graph = new TestDiGraph();
            graph.AddVertex("A");

            Assert.Throws<KeyNotFoundException>(() => graph.HasEdge("B", "A"));
        }
    }
}
