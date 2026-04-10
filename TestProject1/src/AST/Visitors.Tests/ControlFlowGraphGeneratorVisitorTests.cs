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
			var parsed = Parser.Parser.Parse("{}");

			Assert.Throws<ArgumentOutOfRangeException>(() => parsed.Accept(_visitor, null));
		}

		[Fact]
		public void BlockVisit_WithStatements_BuildsSequentialFlowEdges()
		{
			string program = @"{
			x := (1)
			y := (2)
			}";
			var parsed = Parser.Parser.Parse(program);

			Statement? result = parsed.Accept(_visitor, null);

			var first = (AssignmentStmt)parsed.Statements[0];
			var second = (AssignmentStmt)parsed.Statements[1];

			Assert.Null(result);
			Assert.Equal(2, _visitor._cfg.VertexCount());
			Assert.Equal(1, _visitor._cfg.EdgeCount());
			Assert.True(_visitor._cfg.HasEdge(first, second));
		}

		[Fact]
		public void BlockVisit_WithThreeStatements_BuildsLinearFlowEdges()
		{
			string program = @"{
			x := (1)
			y := (2)
			z := (3)
			}";
			var parsed = Parser.Parser.Parse(program);

			Statement? result = parsed.Accept(_visitor, null);

			var first = (AssignmentStmt)parsed.Statements[0];
			var second = (AssignmentStmt)parsed.Statements[1];
			var third = (AssignmentStmt)parsed.Statements[2];

			Assert.Null(result);
			Assert.Equal(3, _visitor._cfg.VertexCount());
			Assert.Equal(2, _visitor._cfg.EdgeCount());
			Assert.True(_visitor._cfg.HasEdge(first, second));
			Assert.True(_visitor._cfg.HasEdge(second, third));
		}

		[Fact]
		public void BlockVisit_SingleAssignmentStatement_AddsOnlyOneVertex()
		{
			string program = @"{
			solo := (1)
			}";
			var parsed = Parser.Parser.Parse(program);

			Statement? result = parsed.Accept(_visitor, null);
			var only = (AssignmentStmt)parsed.Statements[0];

			Assert.Null(result);
			Assert.Equal(1, _visitor._cfg.VertexCount());
			Assert.Equal(0, _visitor._cfg.EdgeCount());
			Assert.Contains(only, _visitor._cfg.GetVertices());
		}

		[Fact]
		public void BlockVisit_OnlyReturnStatement_AddsOnlyOneVertex()
		{
			string program = @"{
			return (99)
			}";
			var parsed = Parser.Parser.Parse(program);

			Statement? result = parsed.Accept(_visitor, null);
			var onlyReturn = (ReturnStmt)parsed.Statements[0];

			Assert.Null(result);
			Assert.Equal(1, _visitor._cfg.VertexCount());
			Assert.Equal(0, _visitor._cfg.EdgeCount());
			Assert.Contains(onlyReturn, _visitor._cfg.GetVertices());
		}

		[Fact]
		public void BlockVisit_NestedBlocks_ConnectsInnerFlowWithoutBlockVertices()
		{
			string program = @"{
			a := (1)
			{
				b := (2)
				c := (3)
			}
			d := (4)
			}";
			var parsed = Parser.Parser.Parse(program);

			Statement? result = parsed.Accept(_visitor, null);
			var first = (AssignmentStmt)parsed.Statements[0];
			var innerBlock = (BlockStmt)parsed.Statements[1];
			var afterInner = (AssignmentStmt)parsed.Statements[2];
			var innerFirst = (AssignmentStmt)innerBlock.Statements[0];
			var innerSecond = (AssignmentStmt)innerBlock.Statements[1];

			bool hasBlockVertex = false;
			foreach (var vertex in _visitor._cfg.GetVertices())
			{
				if (vertex is BlockStmt)
				{
					hasBlockVertex = true;
					break;
				}
			}

			Assert.Null(result);
			Assert.Equal(4, _visitor._cfg.VertexCount());
			Assert.Equal(3, _visitor._cfg.EdgeCount());
			Assert.True(_visitor._cfg.HasEdge(first, innerFirst));
			Assert.True(_visitor._cfg.HasEdge(innerFirst, innerSecond));
			Assert.True(_visitor._cfg.HasEdge(innerSecond, afterInner));
			Assert.False(hasBlockVertex);
		}

		[Fact]
		public void BlockVisit_MultipleSequentialReturns_AddsBothReturnVertices()
		{
			string program = @"{
			return (1)
			return (2)
			}";
			var parsed = Parser.Parser.Parse(program);

			Statement? result = parsed.Accept(_visitor, null);
			var firstReturn = (ReturnStmt)parsed.Statements[0];
			var secondReturn = (ReturnStmt)parsed.Statements[1];

			Assert.Null(result);
			Assert.Equal(2, _visitor._cfg.VertexCount());
			Assert.Equal(0, _visitor._cfg.EdgeCount());
			Assert.Contains(firstReturn, _visitor._cfg.GetVertices());
			Assert.Contains(secondReturn, _visitor._cfg.GetVertices());
			Assert.False(_visitor._cfg.HasEdge(firstReturn, secondReturn));
		}

		[Fact]
		public void BlockVisit_SetsCfgStartToFirstStatement()
		{
			string program = @"{
			start := (1)
			next := (2)
			}";
			var parsed = Parser.Parser.Parse(program);

			Statement? result = parsed.Accept(_visitor, null);
			var first = (AssignmentStmt)parsed.Statements[0];

			Assert.Null(result);
			Assert.Same(first, _visitor._cfg.Start);
		}

		[Fact]
		public void BlockVisit_SameStatementObjectTwice_HandlesDuplicateObject()
		{
			var repeated = NewAssignment("dup", 1);
			var block = NewBlock(repeated, repeated);

			Statement? result = block.Accept(_visitor, null);

			Assert.Null(result);
			Assert.Equal(1, _visitor._cfg.VertexCount());
			Assert.Equal(1, _visitor._cfg.EdgeCount());
			Assert.True(_visitor._cfg.HasEdge(repeated, repeated));
		}

		[Fact]
		public void BlockVisit_LongChain_AssignmentsThenReturn_HasLinearEdges()
		{
			string program = @"{
			a := (1)
			b := (2)
			c := (3)
			d := (4)
			e := (5)
			return (e)
			}";
			var parsed = Parser.Parser.Parse(program);

			Statement? result = parsed.Accept(_visitor, null);

			var s1 = (AssignmentStmt)parsed.Statements[0];
			var s2 = (AssignmentStmt)parsed.Statements[1];
			var s3 = (AssignmentStmt)parsed.Statements[2];
			var s4 = (AssignmentStmt)parsed.Statements[3];
			var s5 = (AssignmentStmt)parsed.Statements[4];
			var ret = (ReturnStmt)parsed.Statements[5];

			Assert.Null(result);
			Assert.Equal(6, _visitor._cfg.VertexCount());
			Assert.Equal(5, _visitor._cfg.EdgeCount());
			Assert.True(_visitor._cfg.HasEdge(s1, s2));
			Assert.True(_visitor._cfg.HasEdge(s2, s3));
			Assert.True(_visitor._cfg.HasEdge(s3, s4));
			Assert.True(_visitor._cfg.HasEdge(s4, s5));
			Assert.True(_visitor._cfg.HasEdge(s5, ret));
		}

		#endregion

		#region Complete Program Tests

		[Fact]
		public void SimpleProgramTest()
		{
			string program = @"{
			x := (1)
			y := (2)
			{
				b := (x + y)
				return (b)
			}
			c := (8)
			}";
			var parsed = Parser.Parser.Parse(program);

			Statement? result = parsed.Accept(_visitor, null);

			var x = (AssignmentStmt)parsed.Statements[0];
			var y = (AssignmentStmt)parsed.Statements[1];
			var innerBlock = (BlockStmt)parsed.Statements[2];
			var c = (AssignmentStmt)parsed.Statements[3];

			var b = (AssignmentStmt)innerBlock.Statements[0];
			var returnStmt = (ReturnStmt)innerBlock.Statements[1];

			Assert.Null(result);
			Assert.Equal(5, _visitor._cfg.VertexCount());
			Assert.Equal(3, _visitor._cfg.EdgeCount());

			Assert.True(_visitor._cfg.HasEdge(x, y));
			Assert.True(_visitor._cfg.HasEdge(y, b));
			Assert.True(_visitor._cfg.HasEdge(b, returnStmt));
			Assert.False(_visitor._cfg.HasEdge(returnStmt, c));
		}

		[Fact]
		public void ComplexProgramTest()
		{
			string program = @"{
			x := (1)
			{
				y := (2)
				{
					z := (3)
					return (z)
					w := (4)
				}
				return (y)
				q := (5)
			}
			m := (6)
			return (m)
			n := (7)
			}";

			var parsed = Parser.Parser.Parse(program);
			Statement? result = parsed.Accept(_visitor, null);

			var x = (AssignmentStmt)parsed.Statements[0];
			var outerBlock = (BlockStmt)parsed.Statements[1];
			var m = (AssignmentStmt)parsed.Statements[2];
			var returnM = (ReturnStmt)parsed.Statements[3];
			var n = (AssignmentStmt)parsed.Statements[4];

			var y = (AssignmentStmt)outerBlock.Statements[0];
			var innerBlock = (BlockStmt)outerBlock.Statements[1];
			var returnY = (ReturnStmt)outerBlock.Statements[2];
			var q = (AssignmentStmt)outerBlock.Statements[3];

			var z = (AssignmentStmt)innerBlock.Statements[0];
			var returnZ = (ReturnStmt)innerBlock.Statements[1];
			var w = (AssignmentStmt)innerBlock.Statements[2];

			Assert.Null(result);
			Assert.Equal(10, _visitor._cfg.VertexCount());
			Assert.Equal(6, _visitor._cfg.EdgeCount());

			Assert.True(_visitor._cfg.HasEdge(x, y));
			Assert.True(_visitor._cfg.HasEdge(y, z));
			Assert.True(_visitor._cfg.HasEdge(z, returnZ));
			Assert.True(_visitor._cfg.HasEdge(w, returnY));
			Assert.True(_visitor._cfg.HasEdge(q, m));
			Assert.True(_visitor._cfg.HasEdge(m, returnM));

			Assert.False(_visitor._cfg.HasEdge(returnZ, w));
			Assert.False(_visitor._cfg.HasEdge(returnY, q));
			Assert.False(_visitor._cfg.HasEdge(returnM, n));
		}

		#endregion

	}
}
