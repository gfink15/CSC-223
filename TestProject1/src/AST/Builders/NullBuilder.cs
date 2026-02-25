using System;
using System.Collections.Generic;

namespace AST
{
    /// <summary>
    /// NullBuilder that does not create any objects; useful for assessing parsing problems
    /// while avoiding object creation
    /// </summary>
    public class NullBuilder : DefaultBuilder
    {
        // Override all creation methods to return null
        public override PlusNode CreatePlusNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public override MinusNode CreateMinusNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public override TimesNode CreateTimesNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public override FloatDivNode CreateFloatDivNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public override IntDivNode CreateIntDivNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public override ModulusNode CreateModulusNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public override ExponentiationNode CreateExponentiationNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public override LiteralNode CreateLiteralNode(object value)
        {
            return null;
        }

        public override VariableNode CreateVariableNode(string name)
        {
            return null;
        }

        public override AssignmentStmt CreateAssignmentStmt(VariableNode variable, ExpressionNode expression)
        {
            return null;
        }

        public override ReturnStmt CreateReturnStmt(ExpressionNode expression)
        {
            return null;
        }

        public override BlockStmt CreateBlockStmt(SymbolTable<string, object> symbolTable)
        {
            return null;
        }
    }
}