using System;
using System.Collections.Generic;
using Utilities.Containers;

namespace AST
{
    /// <summary>
    /// NullBuilder that does not create any objects; useful for assessing parsing problems
    /// while avoiding object creation
    /// </summary>
    public class DefaultBuilder
    {
        // Override all creation methods to return null
        public virtual PlusNode CreatePlusNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public virtual MinusNode CreateMinusNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public virtual TimesNode CreateTimesNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public virtual FloatDivNode CreateFloatDivNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public virtual IntDivNode CreateIntDivNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public virtual ModulusNode CreateModulusNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public virtual ExponentiationNode CreateExponentiationNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public virtual LiteralNode CreateLiteralNode(object value)
        {
            return null;
        }

        public virtual VariableNode CreateVariableNode(string name)
        {
            return null;
        }

        public virtual AssignmentStmt CreateAssignmentStmt(VariableNode variable, ExpressionNode expression)
        {
            return null;
        }

        public virtual ReturnStmt CreateReturnStmt(ExpressionNode expression)
        {
            return null;
        }

        public virtual BlockStmt CreateBlockStmt(SymbolTable<string, object> symbolTable)
        {
            return null;
        }
    }
}