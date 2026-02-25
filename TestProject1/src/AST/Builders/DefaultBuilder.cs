using System;
using System.Collections.Generic;

namespace AST
{
    /// <summary>
    /// NullBuilder that does not create any objects; useful for assessing parsing problems
    /// while avoiding object creation
    /// </summary>
    public class DefaultBuilder
    {
        // Override all creation methods to return null
        public PlusNode CreatePlusNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public MinusNode CreateMinusNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public TimesNode CreateTimesNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public FloatDivNode CreateFloatDivNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public IntDivNode CreateIntDivNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public ModulusNode CreateModulusNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public ExponentiationNode CreateExponentiationNode(ExpressionNode left, ExpressionNode right)
        {
            return null;
        }

        public LiteralNode CreateLiteralNode(object value)
        {
            return null;
        }

        public VariableNode CreateVariableNode(string name)
        {
            return null;
        }

        public AssignmentStmt CreateAssignmentStmt(VariableNode variable, ExpressionNode expression)
        {
            return null;
        }

        public ReturnStmt CreateReturnStmt(ExpressionNode expression)
        {
            return null;
        }

        public BlockStmt CreateBlockStmt(SymbolTable<string, object> symbolTable)
        {
            return null;
        }
    }
}