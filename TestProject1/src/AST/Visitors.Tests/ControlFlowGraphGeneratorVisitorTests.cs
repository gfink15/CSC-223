using System;
using System.Collections.Generic;
using AST;
using Xunit;
using Utilities.Containers;

namespace AST.Visitors.Tests
{
	public class ControlFlowGraphGeneratorVisitorTests
	{
		private static AssignmentStmt MakeAssignment(string name = "x", int value = 1)
		{
			return new AssignmentStmt(new VariableNode(name), new LiteralNode(value));
		}

		private static ReturnStmt MakeReturn(int value = 0)
		{
			return new ReturnStmt(new LiteralNode(value));
		}

		private static BlockStmt MakeBlock(params Statement[] statements)
		{
			var block = new BlockStmt(new SymbolTable<string, object>());
			foreach (var statement in statements)
			{
				block.Add(statement);
			}

			return block;
		}

		public static IEnumerable<object[]> ExpressionNodes()
		{
			yield return new object[] { new PlusNode(new LiteralNode(1), new LiteralNode(2)) };
			yield return new object[] { new MinusNode(new LiteralNode(3), new LiteralNode(1)) };
			yield return new object[] { new TimesNode(new LiteralNode(2), new LiteralNode(4)) };
			yield return new object[] { new FloatDivNode(new LiteralNode(7), new LiteralNode(2)) };
			yield return new object[] { new IntDivNode(new LiteralNode(7), new LiteralNode(2)) };
			yield return new object[] { new ModulusNode(new LiteralNode(9), new LiteralNode(4)) };
			yield return new object[] { new ExponentiationNode(new LiteralNode(2), new LiteralNode(8)) };
			yield return new object[] { new LiteralNode(42) };
			yield return new object[] { new VariableNode("value") };
		}

		[Fact]
		public void Constructor_InitializesEmptyCfg()
		{
			var visitor = new ControlFlowGraphGeneratorVisitor();

			Assert.NotNull(visitor._cfg);
			Assert.Equal(0, visitor._cfg.VertexCount());
			Assert.Equal(0, visitor._cfg.EdgeCount());
			Assert.Null(visitor._cfg.Start);
		}

		[Theory]
		[MemberData(nameof(ExpressionNodes))]
		public void ExpressionVisit_ReturnsNull_DoesNotMutateCfg(ExpressionNode node)
		{
			var visitor = new ControlFlowGraphGeneratorVisitor();
			var predecessor = MakeAssignment("p", 99);
			visitor._cfg.AddVertex(predecessor);

			Statement result = node.Accept(visitor, predecessor);

			Assert.Null(result);
			Assert.Equal(1, visitor._cfg.VertexCount());
			Assert.Equal(0, visitor._cfg.EdgeCount());
		}

		[Fact]
		public void AssignmentVisit_WithValidPredecessor_AddsVertexAndEdge()
		{
			var visitor = new ControlFlowGraphGeneratorVisitor();
			var predecessor = MakeAssignment("a", 1);
			var current = MakeAssignment("b", 2);
			visitor._cfg.AddVertex(predecessor);

			Statement result = current.Accept(visitor, predecessor);

			Assert.Null(result);
			Assert.Equal(2, visitor._cfg.VertexCount());
			Assert.Equal(1, visitor._cfg.EdgeCount());
			Assert.True(visitor._cfg.HasEdge(predecessor, current));
			Assert.Contains(current, visitor._cfg.GetVertices());
		}

		[Fact]
		public void AssignmentVisit_WhenPredecessorMissing_ThrowsAndStillAddsDestinationVertex()
		{
			var visitor = new ControlFlowGraphGeneratorVisitor();
			var missingPredecessor = MakeAssignment("missing", 0);
			var current = MakeAssignment("x", 10);

			Assert.Throws<ArgumentException>(() => current.Accept(visitor, missingPredecessor));
			Assert.Equal(1, visitor._cfg.VertexCount());
			Assert.Equal(0, visitor._cfg.EdgeCount());
			Assert.Contains(current, visitor._cfg.GetVertices());
		}

		[Fact]
		public void AssignmentVisit_WhenPredecessorNull_ThrowsArgumentNullException_AndAddsDestinationVertex()
		{
			var visitor = new ControlFlowGraphGeneratorVisitor();
			var current = MakeAssignment("x", 10);

			Assert.Throws<ArgumentNullException>(() => current.Accept(visitor, null!));
			Assert.Equal(1, visitor._cfg.VertexCount());
			Assert.Equal(0, visitor._cfg.EdgeCount());
			Assert.Contains(current, visitor._cfg.GetVertices());
		}

		[Fact]
		public void ReturnVisit_WithValidPredecessor_AddsVertexAndEdge()
		{
			var visitor = new ControlFlowGraphGeneratorVisitor();
			var predecessor = MakeAssignment("a", 1);
			var ret = MakeReturn(2);
			visitor._cfg.AddVertex(predecessor);

			Statement result = ret.Accept(visitor, predecessor);

			Assert.Null(result);
			Assert.Equal(2, visitor._cfg.VertexCount());
			Assert.Equal(1, visitor._cfg.EdgeCount());
			Assert.True(visitor._cfg.HasEdge(predecessor, ret));
			Assert.Contains(ret, visitor._cfg.GetVertices());
		}

		[Fact]
		public void ReturnVisit_WhenPredecessorMissing_ThrowsAndStillAddsDestinationVertex()
		{
			var visitor = new ControlFlowGraphGeneratorVisitor();
			var missingPredecessor = MakeAssignment("missing", 0);
			var ret = MakeReturn(10);

			Assert.Throws<ArgumentException>(() => ret.Accept(visitor, missingPredecessor));
			Assert.Equal(1, visitor._cfg.VertexCount());
			Assert.Equal(0, visitor._cfg.EdgeCount());
			Assert.Contains(ret, visitor._cfg.GetVertices());
		}

		[Fact]
		public void ReturnVisit_WhenPredecessorNull_ThrowsArgumentNullException_AndAddsDestinationVertex()
		{
			var visitor = new ControlFlowGraphGeneratorVisitor();
			var ret = MakeReturn(10);

			Assert.Throws<ArgumentNullException>(() => ret.Accept(visitor, null!));
			Assert.Equal(1, visitor._cfg.VertexCount());
			Assert.Equal(0, visitor._cfg.EdgeCount());
			Assert.Contains(ret, visitor._cfg.GetVertices());
		}

		[Fact]
		public void BlockVisit_EmptyBlock_ThrowsArgumentOutOfRangeException()
		{
			var visitor = new ControlFlowGraphGeneratorVisitor();
			var block = MakeBlock();

			Assert.Throws<ArgumentOutOfRangeException>(() => block.Accept(visitor, null!));
			Assert.Equal(0, visitor._cfg.VertexCount());
			Assert.Equal(0, visitor._cfg.EdgeCount());
		}

		[Fact]
		public void BlockVisit_FirstInvocationWithStatements_ThrowsArgumentNullException_AfterAddingFirstVertex()
		{
			var visitor = new ControlFlowGraphGeneratorVisitor();
			var first = MakeAssignment("x", 1);
			var second = MakeReturn(2);
			var block = MakeBlock(first, second);

			Assert.Throws<ArgumentNullException>(() => block.Accept(visitor, null!));
			Assert.Equal(1, visitor._cfg.VertexCount());
			Assert.Equal(0, visitor._cfg.EdgeCount());
			Assert.Contains(first, visitor._cfg.GetVertices());
		}

		[Fact]
		public void BlockVisit_WithPreexistingGraphAndSingleStatement_ThrowsArgumentNullException_FromUninitializedLast()
		{
			var visitor = new ControlFlowGraphGeneratorVisitor();
			visitor._cfg.AddVertex(MakeAssignment("seed", 7));
			var first = MakeAssignment("x", 1);
			var block = MakeBlock(first);

			Assert.Throws<ArgumentNullException>(() => block.Accept(visitor, null!));
			Assert.Equal(2, visitor._cfg.VertexCount());
			Assert.Equal(0, visitor._cfg.EdgeCount());
			Assert.Contains(first, visitor._cfg.GetVertices());
		}

		[Fact]
		public void BlockVisit_WhenCfgNotEmptyAndLastManuallySet_ThrowsArgumentOutOfRange_FromLoopIndex()
		{
			var visitor = new ControlFlowGraphGeneratorVisitor();
			var seed = MakeAssignment("seed", 7);
			visitor._cfg.AddVertex(seed);

			// Prime private field "last" to avoid the earlier null-predecessor failure
			// and reach the loop body where i - 1 is accessed for i == 0.
			var first = MakeAssignment("x", 1);
			var second = MakeReturn(2);
			var block = MakeBlock(first, second);

			var lastField = typeof(ControlFlowGraphGeneratorVisitor).GetField("last",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			Assert.NotNull(lastField);
			lastField!.SetValue(visitor, seed);

			Assert.Throws<ArgumentOutOfRangeException>(() => block.Accept(visitor, null!));
			Assert.Equal(2, visitor._cfg.VertexCount());
			Assert.Equal(1, visitor._cfg.EdgeCount());
			Assert.True(visitor._cfg.HasEdge(seed, first));
		}
	}
}
