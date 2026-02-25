using System;
using System.Collections.Generic;
using Utilities.Containers;

namespace AST
{
    /// <summary>
    /// NullBuilder that does not create any objects; useful for assessing parsing problems
    /// while avoiding object creation
    /// </summary>
    public class DebugBuilder : DefaultBuilder
    {
        // Override all creation methods to return null
        public override PlusNode CreatePlusNode(ExpressionNode left, ExpressionNode right)
        {
            return new PlusNode(left, right);
        }

        public override MinusNode CreateMinusNode(ExpressionNode left, ExpressionNode right)
        {
            return new MinusNode(left, right);
        }

        public override TimesNode CreateTimesNode(ExpressionNode left, ExpressionNode right)
        {
            return new TimesNode(left, right);
        }

        public override FloatDivNode CreateFloatDivNode(ExpressionNode left, ExpressionNode right)
        {
            return new FloatDivNode(left, right);
        }

        public override IntDivNode CreateIntDivNode(ExpressionNode left, ExpressionNode right)
        {
            return new IntDivNode(left, right);
        }

        public override ModulusNode CreateModulusNode(ExpressionNode left, ExpressionNode right)
        {
            return new ModulusNode(left, right);
        }

        public override ExponentiationNode CreateExponentiationNode(ExpressionNode left, ExpressionNode right)
        {
            return new ExponentiationNode(left, right);
        }

        public override LiteralNode<T> CreateLiteralNode<T>(T value)
        {
            return new LiteralNode<T>(value);
        }

        public override VariableNode CreateVariableNode(string name)
        {
            return new VariableNode(name);
        }

        public override AssignmentStmt CreateAssignmentStmt(VariableNode variable, ExpressionNode expression)
        {
            return new AssignmentStmt(variable, expression);
        }

        public override ReturnStmt CreateReturnStmt(ExpressionNode expression)
        {
            return new ReturnStmt(expression);
        }

        public override BlockStmt CreateBlockStmt(SymbolTable<string, object> symbolTable)
        {
            return new BlockStmt(symbolTable);
        }
    }
}