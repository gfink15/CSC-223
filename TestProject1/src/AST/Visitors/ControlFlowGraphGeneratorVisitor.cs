using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using AST;
using Utilities.Containers;

namespace AST;

public class ControlFlowGraphGeneratorVisitor : IVisitor<Statement, Statement>
{
    public Statement Visit(PlusNode node, Statement param)
    {
        throw new NotImplementedException();
    }

    public Statement Visit(MinusNode node, Statement param)
    {
        throw new NotImplementedException();
    }

    public Statement Visit(TimesNode node, Statement param)
    {
        throw new NotImplementedException();
    }

    public Statement Visit(FloatDivNode node, Statement param)
    {
        throw new NotImplementedException();
    }

    public Statement Visit(IntDivNode node, Statement param)
    {
        throw new NotImplementedException();
    }

    public Statement Visit(ModulusNode node, Statement param)
    {
        throw new NotImplementedException();
    }

    public Statement Visit(ExponentiationNode node, Statement param)
    {
        throw new NotImplementedException();
    }

    public Statement Visit(LiteralNode node, Statement param)
    {
        throw new NotImplementedException();
    }

    public Statement Visit(VariableNode node, Statement param)
    {
        throw new NotImplementedException();
    }

    public Statement Visit(AssignmentStmt node, Statement param)
    {
        throw new NotImplementedException();
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