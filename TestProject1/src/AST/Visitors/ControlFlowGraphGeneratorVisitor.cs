/**
 * Control Flow Graph Generator Visitor: builds a CFG from AST statements.
 * Claude AI was only used for writing XML and inline comments.
 * 
 * @author Graham Fink, Mridul Agrawal
 * @date   4/9/2026
 */

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using AST;
using Utilities.Containers;
using Optimizer;

namespace AST;

/// <summary>
/// Builds a control-flow graph (CFG) over statement nodes by visiting AST nodes.
/// Expression visits are no-ops for CFG structure and return <c>null</c>.
/// </summary>
public class ControlFlowGraphGeneratorVisitor : IVisitor<Statement?, Statement?>
{

    /// <summary>
    /// The graph being constructed during traversal.
    /// Vertices are statements and directed edges represent execution flow.
    /// </summary>
    public CFG _cfg = new CFG();

    /// <summary>
    /// Tracks the last visited statement to link sequential statements in blocks.
    /// </summary>
    private Statement? last;

    /// <summary>
    /// Expression nodes do not add CFG vertices or edges.
    /// </summary>
    /// <param name="node">The plus expression node.</param>
    /// <param name="param">Unused predecessor statement.</param>
    /// <returns>Always <c>null</c>.</returns>
    public Statement? Visit(PlusNode node, Statement? param) { return null; }

    /// <summary>
    /// Expression nodes do not add CFG vertices or edges.
    /// </summary>
    /// <param name="node">The minus expression node.</param>
    /// <param name="param">Unused predecessor statement.</param>
    /// <returns>Always <c>null</c>.</returns>
    public Statement? Visit(MinusNode node, Statement? param) { return null; }

    /// <summary>
    /// Expression nodes do not add CFG vertices or edges.
    /// </summary>
    /// <param name="node">The multiplication expression node.</param>
    /// <param name="param">Unused predecessor statement.</param>
    /// <returns>Always <c>null</c>.</returns>
    public Statement? Visit(TimesNode node, Statement? param) { return null; }

    /// <summary>
    /// Expression nodes do not add CFG vertices or edges.
    /// </summary>
    /// <param name="node">The floating-point division expression node.</param>
    /// <param name="param">Unused predecessor statement.</param>
    /// <returns>Always <c>null</c>.</returns>
    public Statement? Visit(FloatDivNode node, Statement? param) { return null; }

    /// <summary>
    /// Expression nodes do not add CFG vertices or edges.
    /// </summary>
    /// <param name="node">The integer division expression node.</param>
    /// <param name="param">Unused predecessor statement.</param>
    /// <returns>Always <c>null</c>.</returns>
    public Statement? Visit(IntDivNode node, Statement? param) { return null; }

    /// <summary>
    /// Expression nodes do not add CFG vertices or edges.
    /// </summary>
    /// <param name="node">The modulus expression node.</param>
    /// <param name="param">Unused predecessor statement.</param>
    /// <returns>Always <c>null</c>.</returns>
    public Statement? Visit(ModulusNode node, Statement? param) { return null; }

    /// <summary>
    /// Expression nodes do not add CFG vertices or edges.
    /// </summary>
    /// <param name="node">The exponentiation expression node.</param>
    /// <param name="param">Unused predecessor statement.</param>
    /// <returns>Always <c>null</c>.</returns>
    public Statement? Visit(ExponentiationNode node, Statement? param) { return null; }

    /// <summary>
    /// Expression nodes do not add CFG vertices or edges.
    /// </summary>
    /// <param name="node">The literal expression node.</param>
    /// <param name="param">Unused predecessor statement.</param>
    /// <returns>Always <c>null</c>.</returns>
    public Statement? Visit(LiteralNode node, Statement? param) { return null; }

    /// <summary>
    /// Expression nodes do not add CFG vertices or edges.
    /// </summary>
    /// <param name="node">The variable expression node.</param>
    /// <param name="param">Unused predecessor statement.</param>
    /// <returns>Always <c>null</c>.</returns>
    public Statement? Visit(VariableNode node, Statement? param) { return null; }

    /// <summary>
    /// Adds an assignment statement as a CFG vertex and links it from its predecessor when present.
    /// </summary>
    /// <param name="node">The assignment statement being visited.</param>
    /// <param name="param">The predecessor statement, if one exists.</param>
    /// <returns>Always <c>null</c>.</returns>
    public Statement? Visit(AssignmentStmt node, Statement? param)
    {
        _cfg.AddVertex(node);

        // Add Edge only if a predecessor is present in the param.
        if (param != null)
        {
            _cfg.AddEdge(param, node);
        }

        // Set the last to current node.
        last = node;
        return null;
    }

    /// <summary>
    /// Adds a return statement as a CFG vertex and links it from its predecessor when present.
    /// </summary>
    /// <param name="node">The return statement being visited.</param>
    /// <param name="param">The predecessor statement, if one exists.</param>
    /// <returns>Always <c>null</c>.</returns>
    public Statement? Visit(ReturnStmt node, Statement? param)
    {
        _cfg.AddVertex(node);

        // Add Edge only if a predecessor is present in the param.
        if (param != null)
        {
            _cfg.AddEdge(param, node);
        }

        // A return ends normal sequential flow from this point.
        last = null;
        return null;
    }

    /// <summary>
    /// Visits statements in a block in order, connecting each statement to its predecessor.
    /// </summary>
    /// <param name="node">The block statement to traverse.</param>
    /// <param name="param">An optional predecessor from outside the block.</param>
    /// <returns>Always <c>null</c>.</returns>
    public Statement? Visit(BlockStmt node, Statement? param)
    {
        // For the first visited block in an empty CFG, start without a predecessor.
        if (_cfg.VertexCount() == 0) node.Statements[0].Accept(this, null);

        // For any nested block, continue using the last statement/null added.
        else node.Statements[0].Accept(this, last);

        // Loop through all statements in the current block.
        for (int i = 1; i < node.Statements.Count; i++)
        {
            // Connect current statement to the last one
            node.Statements[i].Accept(this, last);
        }
        return null;
    }
}