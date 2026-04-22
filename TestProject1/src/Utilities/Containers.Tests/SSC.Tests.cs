using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Utilities.Containers.Tests
{
    /// <summary>
    /// Unit tests for DiGraph.FindStronglyConnectedComponents.
    ///
    /// A strongly connected component (SCC) is a maximal set of vertices such
    /// that every vertex is reachable from every other vertex via directed
    /// paths. The SCCs of a digraph form a partition of its vertex set:
    /// every vertex appears in exactly one SCC.
    ///
    /// Tests verify behavioral properties (partition invariants, mutual
    /// reachability inside an SCC, lack of mutual reachability across SCCs)
    /// rather than any particular internal ordering.
    /// </summary>
    public class SSCTests
    {
        #region Helpers

        /// <summary>
        /// Builds a DiGraph from a list of vertex names and directed edges.
        /// Keeps the arrange step of each test to a single readable line.
        /// </summary>
        private static DiGraph<string> CreateGraph(string[] vertices, (string, string)[] edges)
        {
            var graph = new DiGraph<string>();
            foreach (var v in vertices) graph.AddVertex(v);
            foreach (var (src, dst) in edges) graph.AddEdge(src, dst);
            return graph;
        }

        /// <summary>
        /// Sorts each SCC's vertices and then sorts the outer list of SCCs.
        /// The order SCCs come back in, and the order of vertices inside each,
        /// is implementation-defined — canonicalizing lets us compare results
        /// against a fixed expected layout.
        /// </summary>
        private static List<List<T>> Canonicalize<T>(List<List<T>> sccs) where T : IComparable<T>
        {
            return sccs
                .Select(scc => scc.OrderBy(x => x).ToList())
                .OrderBy(scc => scc.Count == 0 ? default! : scc[0])
                .ToList();
        }

        /// <summary>
        /// Returns every vertex reachable from <paramref name="start"/> by
        /// following directed edges (including <paramref name="start"/> itself).
        /// Used by <see cref="AssertSCCsMatchReachability"/> as the ground-truth
        /// reachability oracle — we trust a simple DFS here rather than the
        /// SCC algorithm under test.
        /// </summary>
        private static HashSet<T> Reachable<T>(DiGraph<T> graph, T start) where T : notnull
        {
            var visited = new HashSet<T>();
            var stack = new Stack<T>();
            stack.Push(start);
            while (stack.Count > 0)
            {
                var v = stack.Pop();
                // Skip vertices we have already expanded.
                if (!visited.Add(v)) continue;
                foreach (var n in graph.GetNeighbors(v)) stack.Push(n);
            }
            return visited;
        }

        /// <summary>
        /// Asserts the partition invariants that must hold for any SCC result:
        ///   1. No inner list is null or empty.
        ///   2. Every graph vertex appears in exactly one SCC.
        ///   3. No vertex outside the graph appears in any SCC.
        /// </summary>
        private static void AssertValidPartition<T>(DiGraph<T> graph, List<List<T>> sccs) where T : notnull
        {
            Assert.NotNull(sccs);
            foreach (var scc in sccs)
            {
                Assert.NotNull(scc);
                Assert.NotEmpty(scc);
            }

            var graphVertices = graph.GetVertices().ToHashSet();
            var flattened = sccs.SelectMany(s => s).ToList();

            // Same total count, no duplicates, and the exact same vertex set —
            // together these three checks prove "every vertex exactly once".
            Assert.Equal(graphVertices.Count, flattened.Count);
            Assert.Equal(graphVertices.Count, flattened.ToHashSet().Count);
            Assert.Equal(graphVertices, flattened.ToHashSet());
        }

        /// <summary>
        /// Asserts that every pair of vertices inside an SCC is mutually
        /// reachable, and that no pair drawn from two different SCCs is
        /// mutually reachable.
        /// </summary>
        private static void AssertSCCsMatchReachability<T>(DiGraph<T> graph, List<List<T>> sccs) where T : notnull
        {
            // Precompute reachability from every vertex once, so the O(V^2)
            // pair checks below don't each trigger a fresh DFS.
            var reachFrom = graph.GetVertices().ToDictionary(v => v, v => Reachable(graph, v));

            // Inside an SCC: every pair (u, v) must be mutually reachable.
            foreach (var scc in sccs)
            {
                foreach (var u in scc)
                {
                    foreach (var v in scc)
                    {
                        Assert.Contains(v, reachFrom[u]);
                        Assert.Contains(u, reachFrom[v]);
                    }
                }
            }

            // Across SCCs: no pair may be mutually reachable. If two vertices
            // in different SCCs could reach each other, they would by
            // definition belong to the same SCC.
            for (int i = 0; i < sccs.Count; i++)
            {
                for (int j = i + 1; j < sccs.Count; j++)
                {
                    foreach (var u in sccs[i])
                    {
                        foreach (var v in sccs[j])
                        {
                            bool uReachesV = reachFrom[u].Contains(v);
                            bool vReachesU = reachFrom[v].Contains(u);
                            Assert.False(uReachesV && vReachesU,
                                $"Vertices {u} and {v} are mutually reachable but live in different SCCs.");
                        }
                    }
                }
            }
        }

        #endregion

        #region Trivial Graphs

        [Fact]
        public void FindSCC_EmptyGraph_ReturnsEmptyList()
        {
            var graph = new DiGraph<string>();

            var sccs = graph.FindStronglyConnectedComponents();

            Assert.NotNull(sccs);
            Assert.Empty(sccs);
        }

        [Fact]
        public void FindSCC_SingleIsolatedVertex_ReturnsOneSingletonComponent()
        {
            var graph = CreateGraph(new[] { "A" }, Array.Empty<(string, string)>());

            var sccs = graph.FindStronglyConnectedComponents();

            Assert.Single(sccs);
            Assert.Single(sccs[0]);
            Assert.Equal("A", sccs[0][0]);
            AssertValidPartition(graph, sccs);
        }

        [Fact]
        public void FindSCC_SingleVertexWithSelfLoop_ReturnsOneComponentWithOneVertex()
        {
            var graph = CreateGraph(new[] { "A" }, new[] { ("A", "A") });

            var sccs = graph.FindStronglyConnectedComponents();

            Assert.Single(sccs);
            Assert.Single(sccs[0]);
            Assert.Equal("A", sccs[0][0]);
            AssertValidPartition(graph, sccs);
        }

        [Fact]
        public void FindSCC_MultipleIsolatedVertices_EachIsOwnComponent()
        {
            var graph = CreateGraph(new[] { "A", "B", "C", "D" }, Array.Empty<(string, string)>());

            var sccs = graph.FindStronglyConnectedComponents();

            Assert.Equal(4, sccs.Count);
            Assert.All(sccs, scc => Assert.Single(scc));
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        #endregion

        #region Two-Vertex Cases

        [Fact]
        public void FindSCC_OneWayEdge_TwoSingletonComponents()
        {
            var graph = CreateGraph(new[] { "A", "B" }, new[] { ("A", "B") });

            var sccs = graph.FindStronglyConnectedComponents();

            Assert.Equal(2, sccs.Count);
            Assert.All(sccs, scc => Assert.Single(scc));
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        [Fact]
        public void FindSCC_BidirectionalEdge_OneComponentOfSizeTwo()
        {
            var graph = CreateGraph(new[] { "A", "B" }, new[] { ("A", "B"), ("B", "A") });

            var sccs = graph.FindStronglyConnectedComponents();

            Assert.Single(sccs);
            Assert.Equal(2, sccs[0].Count);
            Assert.Contains("A", sccs[0]);
            Assert.Contains("B", sccs[0]);
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        #endregion

        #region DAG Cases

        [Fact]
        public void FindSCC_LinearChain_EachVertexIsOwnComponent()
        {
            var graph = CreateGraph(new[] { "A", "B", "C", "D" },
                new[] { ("A", "B"), ("B", "C"), ("C", "D") });

            var sccs = graph.FindStronglyConnectedComponents();

            Assert.Equal(4, sccs.Count);
            Assert.All(sccs, scc => Assert.Single(scc));
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        [Fact]
        public void FindSCC_DiamondDag_FourSingletonComponents()
        {
            var graph = CreateGraph(new[] { "A", "B", "C", "D" },
                new[] { ("A", "B"), ("A", "C"), ("B", "D"), ("C", "D") });

            var sccs = graph.FindStronglyConnectedComponents();

            Assert.Equal(4, sccs.Count);
            Assert.All(sccs, scc => Assert.Single(scc));
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        [Fact]
        public void FindSCC_TreeShapedDag_EachVertexIsOwnComponent()
        {
            var graph = CreateGraph(new[] { "A", "B", "C", "D", "E", "F", "G" },
                new[] { ("A", "B"), ("A", "C"), ("B", "D"), ("B", "E"), ("C", "F"), ("C", "G") });

            var sccs = graph.FindStronglyConnectedComponents();

            Assert.Equal(7, sccs.Count);
            Assert.All(sccs, scc => Assert.Single(scc));
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        #endregion

        #region Single Cycle Cases

        [Fact]
        public void FindSCC_TriangleCycle_OneComponentWithAllThreeVertices()
        {
            var graph = CreateGraph(new[] { "A", "B", "C" },
                new[] { ("A", "B"), ("B", "C"), ("C", "A") });

            var sccs = graph.FindStronglyConnectedComponents();

            Assert.Single(sccs);
            Assert.Equal(3, sccs[0].Count);
            Assert.Contains("A", sccs[0]);
            Assert.Contains("B", sccs[0]);
            Assert.Contains("C", sccs[0]);
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        [Fact]
        public void FindSCC_LargeCycle_OneComponentContainingEveryVertex()
        {
            int n = 10;
            var vertices = Enumerable.Range(0, n).Select(i => $"V{i}").ToArray();
            var edges = Enumerable.Range(0, n).Select(i => (vertices[i], vertices[(i + 1) % n])).ToArray();
            var graph = CreateGraph(vertices, edges);

            var sccs = graph.FindStronglyConnectedComponents();

            Assert.Single(sccs);
            Assert.Equal(n, sccs[0].Count);
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        [Fact]
        public void FindSCC_CompleteDigraph_OneComponent()
        {
            var vertices = new[] { "A", "B", "C", "D" };
            var edges = new List<(string, string)>();
            foreach (var u in vertices)
                foreach (var v in vertices)
                    if (!u.Equals(v)) edges.Add((u, v));
            var graph = CreateGraph(vertices, edges.ToArray());

            var sccs = graph.FindStronglyConnectedComponents();

            Assert.Single(sccs);
            Assert.Equal(4, sccs[0].Count);
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        #endregion

        #region Multiple Components

        [Fact]
        public void FindSCC_TwoDisjointCycles_TwoComponents()
        {
            var graph = CreateGraph(new[] { "A", "B", "C", "X", "Y", "Z" },
                new[] { ("A", "B"), ("B", "C"), ("C", "A"),
                        ("X", "Y"), ("Y", "Z"), ("Z", "X") });

            var sccs = graph.FindStronglyConnectedComponents();
            var canonical = Canonicalize(sccs);

            Assert.Equal(2, canonical.Count);
            Assert.Equal(new List<string> { "A", "B", "C" }, canonical[0]);
            Assert.Equal(new List<string> { "X", "Y", "Z" }, canonical[1]);
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        [Fact]
        public void FindSCC_TwoCyclesLinkedOneWay_RemainTwoComponents()
        {
            // A single bridge C -> X lets the first cycle reach the second,
            // but not vice versa. No mutual reachability across the bridge,
            // so the two cycles stay as separate SCCs.
            var graph = CreateGraph(new[] { "A", "B", "C", "X", "Y", "Z" },
                new[] { ("A", "B"), ("B", "C"), ("C", "A"),
                        ("X", "Y"), ("Y", "Z"), ("Z", "X"),
                        ("C", "X") });

            var sccs = graph.FindStronglyConnectedComponents();
            var canonical = Canonicalize(sccs);

            Assert.Equal(2, canonical.Count);
            Assert.Equal(new List<string> { "A", "B", "C" }, canonical[0]);
            Assert.Equal(new List<string> { "X", "Y", "Z" }, canonical[1]);
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        [Fact]
        public void FindSCC_TwoCyclesLinkedBothWays_MergeIntoOneComponent()
        {
            // Bridges in *both* directions (C -> X and Z -> A) make every
            // vertex in one cycle mutually reachable with every vertex in the
            // other — so all six vertices collapse into a single SCC.
            var graph = CreateGraph(new[] { "A", "B", "C", "X", "Y", "Z" },
                new[] { ("A", "B"), ("B", "C"), ("C", "A"),
                        ("X", "Y"), ("Y", "Z"), ("Z", "X"),
                        ("C", "X"), ("Z", "A") });

            var sccs = graph.FindStronglyConnectedComponents();

            Assert.Single(sccs);
            Assert.Equal(6, sccs[0].Count);
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        [Fact]
        public void FindSCC_CycleWithDanglingDagTail_CycleAndSingletonsSeparate()
        {
            // A triangle A-B-C plus a tail C -> D -> E. D and E can be reached
            // from the cycle but cannot reach it back, so they are singleton
            // SCCs rather than part of the triangle.
            var graph = CreateGraph(new[] { "A", "B", "C", "D", "E" },
                new[] { ("A", "B"), ("B", "C"), ("C", "A"),
                        ("C", "D"), ("D", "E") });

            var sccs = graph.FindStronglyConnectedComponents();
            var canonical = Canonicalize(sccs);

            Assert.Equal(3, canonical.Count);
            Assert.Equal(new List<string> { "A", "B", "C" }, canonical[0]);
            Assert.Equal(new List<string> { "D" }, canonical[1]);
            Assert.Equal(new List<string> { "E" }, canonical[2]);
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        [Fact]
        public void FindSCC_ClassicCLRSExample_FourComponents()
        {
            // Vertices and edges from CLRS Figure 22.9.
            // Expected SCCs: {a, b, e}, {c, d}, {f, g}, {h}.
            var graph = CreateGraph(
                new[] { "a", "b", "c", "d", "e", "f", "g", "h" },
                new[] {
                    ("a", "b"), ("b", "c"), ("b", "e"), ("b", "f"),
                    ("c", "d"), ("c", "g"),
                    ("d", "c"), ("d", "h"),
                    ("e", "a"), ("e", "f"),
                    ("f", "g"),
                    ("g", "f"), ("g", "h"),
                    ("h", "h"),
                });

            var sccs = graph.FindStronglyConnectedComponents();
            var canonical = Canonicalize(sccs);

            Assert.Equal(4, canonical.Count);
            Assert.Equal(new List<string> { "a", "b", "e" }, canonical[0]);
            Assert.Equal(new List<string> { "c", "d" }, canonical[1]);
            Assert.Equal(new List<string> { "f", "g" }, canonical[2]);
            Assert.Equal(new List<string> { "h" }, canonical[3]);
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        [Fact]
        public void FindSCC_IsolatedVerticesAndCycle_ComponentsCountsAreCorrect()
        {
            var graph = CreateGraph(new[] { "A", "B", "C", "X", "Y" },
                new[] { ("A", "B"), ("B", "C"), ("C", "A") });

            var sccs = graph.FindStronglyConnectedComponents();
            var canonical = Canonicalize(sccs);

            Assert.Equal(3, canonical.Count);
            Assert.Equal(new List<string> { "A", "B", "C" }, canonical[0]);
            Assert.Equal(new List<string> { "X" }, canonical[1]);
            Assert.Equal(new List<string> { "Y" }, canonical[2]);
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        #endregion

        #region Self-Loops Mixed With Other Edges

        [Fact]
        public void FindSCC_SelfLoopOnDagVertex_DoesNotAffectComponentMembership()
        {
            // A self-loop on B (B -> B) does not introduce mutual reachability
            // with any other vertex, so the DAG still decomposes into singletons.
            var graph = CreateGraph(new[] { "A", "B", "C" },
                new[] { ("A", "B"), ("B", "C"), ("B", "B") });

            var sccs = graph.FindStronglyConnectedComponents();
            var canonical = Canonicalize(sccs);

            Assert.Equal(3, canonical.Count);
            Assert.All(canonical, scc => Assert.Single(scc));
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        [Fact]
        public void FindSCC_SelfLoopInsideLargerCycle_CycleStillSingleComponent()
        {
            var graph = CreateGraph(new[] { "A", "B", "C" },
                new[] { ("A", "B"), ("B", "C"), ("C", "A"), ("B", "B") });

            var sccs = graph.FindStronglyConnectedComponents();

            Assert.Single(sccs);
            Assert.Equal(3, sccs[0].Count);
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        #endregion

        #region Larger / Stress-ish Graphs

        [Fact]
        public void FindSCC_TwoLargeCyclesWithBridges_ProducesExpectedPartition()
        {
            var vertices = new[] {
                "a1", "a2", "a3", "a4",
                "b1", "b2", "b3", "b4",
                "c1", "c2",
            };
            var edges = new[] {
                ("a1", "a2"), ("a2", "a3"), ("a3", "a4"), ("a4", "a1"),
                ("b1", "b2"), ("b2", "b3"), ("b3", "b4"), ("b4", "b1"),
                ("a3", "b1"),
                ("b4", "c1"), ("c1", "c2"),
            };
            var graph = CreateGraph(vertices, edges);

            var sccs = graph.FindStronglyConnectedComponents();
            var canonical = Canonicalize(sccs);

            Assert.Equal(4, canonical.Count);
            Assert.Equal(new List<string> { "a1", "a2", "a3", "a4" }, canonical[0]);
            Assert.Equal(new List<string> { "b1", "b2", "b3", "b4" }, canonical[1]);
            Assert.Equal(new List<string> { "c1" }, canonical[2]);
            Assert.Equal(new List<string> { "c2" }, canonical[3]);
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        [Fact]
        public void FindSCC_ChainOfCyclesWithBackEdge_MergesIntoOneComponent()
        {
            // Three small 2-cycles chained forward (V1->V2, V3->V4) plus one
            // back-edge (V5->V0) that closes the whole loop. Every vertex can
            // reach every other, so all six belong to one SCC.
            var vertices = Enumerable.Range(0, 6).Select(i => $"V{i}").ToArray();
            var edges = new[] {
                ("V0", "V1"), ("V1", "V0"),
                ("V2", "V3"), ("V3", "V2"),
                ("V4", "V5"), ("V5", "V4"),
                ("V1", "V2"), ("V3", "V4"), ("V5", "V0"),
            };
            var graph = CreateGraph(vertices, edges);

            var sccs = graph.FindStronglyConnectedComponents();

            Assert.Single(sccs);
            Assert.Equal(6, sccs[0].Count);
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        #endregion

        #region Generic Type

        [Fact]
        public void FindSCC_IntegerVertices_ProducesCorrectComponents()
        {
            var graph = new DiGraph<int>();
            foreach (var v in new[] { 1, 2, 3, 4, 5 }) graph.AddVertex(v);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 1);
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 5);

            var sccs = graph.FindStronglyConnectedComponents();
            var canonical = Canonicalize(sccs);

            Assert.Equal(3, canonical.Count);
            Assert.Equal(new List<int> { 1, 2, 3 }, canonical[0]);
            Assert.Equal(new List<int> { 4 }, canonical[1]);
            Assert.Equal(new List<int> { 5 }, canonical[2]);
            AssertValidPartition(graph, sccs);
            AssertSCCsMatchReachability(graph, sccs);
        }

        #endregion

        #region Determinism and Isolation

        [Fact]
        public void FindSCC_CalledTwice_ProducesIdenticalPartition()
        {
            var graph = CreateGraph(
                new[] { "a", "b", "c", "d", "e", "f", "g", "h" },
                new[] {
                    ("a", "b"), ("b", "c"), ("b", "e"), ("b", "f"),
                    ("c", "d"), ("c", "g"),
                    ("d", "c"), ("d", "h"),
                    ("e", "a"), ("e", "f"),
                    ("f", "g"),
                    ("g", "f"), ("g", "h"),
                    ("h", "h"),
                });

            var first = Canonicalize(graph.FindStronglyConnectedComponents());
            var second = Canonicalize(graph.FindStronglyConnectedComponents());

            Assert.Equal(first.Count, second.Count);
            for (int i = 0; i < first.Count; i++)
            {
                Assert.Equal(first[i], second[i]);
            }
        }

        [Fact]
        public void FindSCC_DoesNotMutateGraph()
        {
            var graph = CreateGraph(new[] { "A", "B", "C" },
                new[] { ("A", "B"), ("B", "C"), ("C", "A") });

            int verticesBefore = graph.VertexCount();
            int edgesBefore = graph.EdgeCount();
            var neighborsABefore = graph.GetNeighbors("A");
            var neighborsBBefore = graph.GetNeighbors("B");
            var neighborsCBefore = graph.GetNeighbors("C");

            graph.FindStronglyConnectedComponents();

            Assert.Equal(verticesBefore, graph.VertexCount());
            Assert.Equal(edgesBefore, graph.EdgeCount());
            Assert.Equal(neighborsABefore, graph.GetNeighbors("A"));
            Assert.Equal(neighborsBBefore, graph.GetNeighbors("B"));
            Assert.Equal(neighborsCBefore, graph.GetNeighbors("C"));
        }

        [Fact]
        public void FindSCC_ReturnedListsAreIndependent_MutatingOneDoesNotAffectOthers()
        {
            var graph = CreateGraph(new[] { "A", "B", "C", "D" },
                new[] { ("A", "B"), ("B", "A"), ("C", "D"), ("D", "C") });

            var sccs = graph.FindStronglyConnectedComponents();
            int totalBefore = sccs.Sum(s => s.Count);

            // Mutating the returned SCC list should not affect the graph or a
            // subsequent call's result.
            sccs[0].Clear();

            var second = graph.FindStronglyConnectedComponents();
            int totalAfter = second.Sum(s => s.Count);

            Assert.Equal(4, totalAfter);
            Assert.NotEqual(totalBefore, sccs.Sum(s => s.Count));
        }

        #endregion
    }
}
