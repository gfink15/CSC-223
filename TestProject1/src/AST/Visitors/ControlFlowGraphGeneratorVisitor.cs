using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using AST;
using Utilities.Containers;
using Optimizer;

namespace AST;

public class ControlFlowGraphGeneratorVisitor : IVisitor<Statement, Statement?>
{

    public CFG _cfg = new CFG();
    private Statement last;
    public Statement? Visit(PlusNode node, Statement param)
    {
        return null;
    }

    public Statement? Visit(MinusNode node, Statement param)
    {
        return null;
    }

    public Statement? Visit(TimesNode node, Statement param)
    {
        return null;
    }

    public Statement? Visit(FloatDivNode node, Statement param)
    {
        return null;
    }

    public Statement? Visit(IntDivNode node, Statement param)
    {
        return null;
    }

    public Statement? Visit(ModulusNode node, Statement param)
    {
        return null;
    }

    public Statement? Visit(ExponentiationNode node, Statement param)
    {
        return null;
    }

    public Statement? Visit(LiteralNode node, Statement param)
    {
        return null;
    }

    public Statement? Visit(VariableNode node, Statement param)
    {
        return null;
    }

    public Statement? Visit(AssignmentStmt node, Statement param)
    {
        _cfg.AddVertex(node);
        _cfg.AddEdge(param, node);
        return null;
    }

    public Statement? Visit(ReturnStmt node, Statement param)
    {
        _cfg.AddVertex(node);
        _cfg.AddEdge(param, node);
        return null;
    }

    public Statement? Visit(BlockStmt node, Statement param)
    {
        if (_cfg.VertexCount() == 0) node.Statements[0].Accept(this, null);
        else node.Statements[0].Accept(this, last);
        for (int i = 0; i < node.Statements.Count; i++)
        {
            last = node.Statements[i];
            node.Statements[i].Accept(this, node.Statements[i-1]);
        }
        return null;
    }
}