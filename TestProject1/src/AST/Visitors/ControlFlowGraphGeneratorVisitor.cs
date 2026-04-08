using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using AST;
using Utilities.Containers;
using Optimizer;

namespace AST;

public class ControlFlowGraphGeneratorVisitor : IVisitor<Statement, Statement>
{

    public CFG _cfg = new CFG();
    public Statement Visit(PlusNode node, Statement param)
    {
        return null;
    }

    public Statement Visit(MinusNode node, Statement param)
    {
        return null;
    }

    public Statement Visit(TimesNode node, Statement param)
    {
        return null;
    }

    public Statement Visit(FloatDivNode node, Statement param)
    {
        return null;
    }

    public Statement Visit(IntDivNode node, Statement param)
    {
        return null;
    }

    public Statement Visit(ModulusNode node, Statement param)
    {
        return null;
    }

    public Statement Visit(ExponentiationNode node, Statement param)
    {
        return null;
    }

    public Statement Visit(LiteralNode node, Statement param)
    {
        return null;
    }

    public Statement Visit(VariableNode node, Statement param)
    {
        return null;
    }

    public Statement Visit(AssignmentStmt node, Statement param)
    {
        // Add Vertex
        // Add this node to the previous as a directed edge
    }

    public Statement Visit(ReturnStmt node, Statement param)
    {
        throw new NotImplementedException();
    }

    public Statement Visit(BlockStmt node, Statement param)
    {
        throw new NotImplementedException();
    }
}