using System;
using System.Collections.Generic;
using AST;
using Utilities.Containers;
using Xunit;

namespace AST.Visitors.Tests
{
	public class ControlFlowGraphGeneratorVisitorTests
	{
		private readonly ControlFlowGraphGeneratorVisitor _visitor;

		public ControlFlowGraphGeneratorVisitorTests()
		{
			_visitor = new ControlFlowGraphGeneratorVisitor();
		}

		private static BlockStmt NewBlock(params Statement[] statements)
		{
			var block = new BlockStmt(new SymbolTable<string, object>());
			foreach (var statement in statements)
			{
				block.Add(statement);
			}
			return block;
		}

		private static AssignmentStmt NewAssignment(string name, int value)
		{
			return new AssignmentStmt(new VariableNode(name), new LiteralNode(value));
		}

		public static IEnumerable<object[]> ExpressionNodes()
		{
			yield return new object[] { new PlusNode(new LiteralNode(1), new LiteralNode(2)) };
			yield return new object[] { new MinusNode(new LiteralNode(3), new LiteralNode(1)) };
			yield return new object[] { new TimesNode(new LiteralNode(2), new LiteralNode(4)) };
			yield return new object[] { new FloatDivNode(new LiteralNode(8), new LiteralNode(2)) };
			yield return new object[] { new IntDivNode(new LiteralNode(9), new LiteralNode(2)) };
			yield return new object[] { new ModulusNode(new LiteralNode(9), new LiteralNode(4)) };
			yield return new object[] { new ExponentiationNode(new LiteralNode(2), new LiteralNode(3)) };
			yield return new object[] { new LiteralNode(42) };
			yield return new object[] { new VariableNode("x") };
		}

		#region Constructor and Expression Visit Tests

		[Fact]
		public void Constructor_InitializesEmptyCfg()
		{
			var visitor = new ControlFlowGraphGeneratorVisitor();

			Assert.NotNull(visitor._cfg);
			Assert.Equal(0, visitor._cfg.VertexCount());
			Assert.Equal(0, visitor._cfg.EdgeCount());
		}

		[Theory]
		[MemberData(nameof(ExpressionNodes))]
		public void ExpressionVisit_ReturnsNullAndDoesNotMutateCfg(ExpressionNode node)
		{
			Statement? result = node.Accept(_visitor, null);

			Assert.Null(result);
			Assert.Equal(0, _visitor._cfg.VertexCount());
			Assert.Equal(0, _visitor._cfg.EdgeCount());
		}

		#endregion

		#region Assignment and Return Visit Tests

		[Fact]
		public void AssignmentVisit_NullParam_AddsVertexOnly()
		{
			var assignment = NewAssignment("a", 1);

			Statement? result = assignment.Accept(_visitor, null);

			Assert.Null(result);
			Assert.Equal(1, _visitor._cfg.VertexCount());
			Assert.Equal(0, _visitor._cfg.EdgeCount());
		}

		[Fact]
		public void AssignmentVisit_WithExistingPredecessor_AddsDirectedEdge()
		{
			var previous = NewAssignment("a", 1);
			var current = NewAssignment("b", 2);
			_visitor._cfg.AddVertex(previous);

			current.Accept(_visitor, previous);

			Assert.Equal(2, _visitor._cfg.VertexCount());
			Assert.Equal(1, _visitor._cfg.EdgeCount());
			Assert.True(_visitor._cfg.HasEdge(previous, current));
		}

		[Fact]
		public void AssignmentVisit_WithMissingPredecessor_ThrowsArgumentException()
		{
			var missingPrevious = NewAssignment("a", 1);
			var current = NewAssignment("b", 2);

			Assert.Throws<ArgumentException>(() => current.Accept(_visitor, missingPrevious));
		}

		[Fact]
		public void ReturnVisit_WithExistingPredecessor_AddsDirectedEdge()
		{
			var previous = NewAssignment("a", 1);
			var returnStmt = new ReturnStmt(new VariableNode("a"));
			_visitor._cfg.AddVertex(previous);

			returnStmt.Accept(_visitor, previous);

			Assert.Equal(2, _visitor._cfg.VertexCount());
			Assert.Equal(1, _visitor._cfg.EdgeCount());
			Assert.True(_visitor._cfg.HasEdge(previous, returnStmt));
		}

		[Fact]
		public void ReturnVisit_NullParam_AddsVertexOnly()
		{
			var returnStmt = new ReturnStmt(new LiteralNode(5));

			Statement? result = returnStmt.Accept(_visitor, null);

			Assert.Null(result);
			Assert.Equal(1, _visitor._cfg.VertexCount());
			Assert.Equal(0, _visitor._cfg.EdgeCount());
		}

		[Fact]
		public void ReturnVisit_WithMissingPredecessor_ThrowsArgumentException()
		{
			var missingPrevious = NewAssignment("a", 1);
			var returnStmt = new ReturnStmt(new VariableNode("a"));

			Assert.Throws<ArgumentException>(() => returnStmt.Accept(_visitor, missingPrevious));
		}

		[Fact]
		public void AssignmentVisit_WithSamePredecessorAndCurrentTwice_DoesNotAddDuplicateEdge()
		{
			var previous = NewAssignment("a", 1);
			var current = NewAssignment("b", 2);
			_visitor._cfg.AddVertex(previous);

			current.Accept(_visitor, previous);
			current.Accept(_visitor, previous);

			Assert.Equal(2, _visitor._cfg.VertexCount());
			Assert.Equal(1, _visitor._cfg.EdgeCount());
			Assert.True(_visitor._cfg.HasEdge(previous, current));
		}

		#endregion

		#region Block Visit Tests

		[Fact]
		public void BlockVisit_EmptyBlock_ThrowsArgumentOutOfRangeException()
		{
			var block = NewBlock();

			Assert.Throws<ArgumentOutOfRangeException>(() => block.Accept(_visitor, null));
		}

		[Fact]
		public void BlockVisit_WithStatements_BuildsSequentialFlowEdges()
		{
			var first = NewAssignment("x", 1);
			var second = NewAssignment("y", 2);
			var block = NewBlock(first, second);

			Statement? result = block.Accept(_visitor, null);

			Assert.Null(result);
			Assert.Equal(2, _visitor._cfg.VertexCount());
			Assert.Equal(1, _visitor._cfg.EdgeCount());
			Assert.True(_visitor._cfg.HasEdge(first, second));
		}

		[Fact]
		public void BlockVisit_WithThreeStatements_BuildsLinearFlowEdges()
		{
			var first = NewAssignment("x", 1);
			var second = NewAssignment("y", 2);
			var third = NewAssignment("z", 3);
			var block = NewBlock(first, second, third);

			Statement? result = block.Accept(_visitor, null);

			Assert.Null(result);
			Assert.Equal(3, _visitor._cfg.VertexCount());
			Assert.Equal(2, _visitor._cfg.EdgeCount());
			Assert.True(_visitor._cfg.HasEdge(first, second));
			Assert.True(_visitor._cfg.HasEdge(second, third));
		}

		#endregion
		[Fact]
		public void ManualTest()
		{
			string program = @"{
			x := (1)
			y := (2)
			{
				z := (3)
				a := (4)
				b := (z + a)
				return b
			}
			c := (8)
			}";
			var parsed = Parser.Parser.Parse(program);
			//var visited = parsed.Statements[0].Accept(_visitor, this);
		}
	}
}
