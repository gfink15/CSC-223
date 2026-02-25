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
            Console.WriteLine("Creating a PlusNode. Left: " + left.Unparse() + ", Right: " + right.Unparse());
            return new PlusNode(left, right);
        }

        public override MinusNode CreateMinusNode(ExpressionNode left, ExpressionNode right)
        {
            Console.WriteLine("Creating a MinusNode. Left: " + left.Unparse() + ", Right: " + right.Unparse());
            return new MinusNode(left, right);
        }

        public override TimesNode CreateTimesNode(ExpressionNode left, ExpressionNode right)
        {
            Console.WriteLine("Creating a TimesNode. Left: " + left.Unparse() + ", Right: " + right.Unparse());
            return new TimesNode(left, right);
        }

        public override FloatDivNode CreateFloatDivNode(ExpressionNode left, ExpressionNode right)
        {
            Console.WriteLine("Creating a FloatDivNode. Left: " + left.Unparse() + ", Right: " + right.Unparse());
            return new FloatDivNode(left, right);
        }

        public override IntDivNode CreateIntDivNode(ExpressionNode left, ExpressionNode right)
        {
            Console.WriteLine("Creating an IntDivNode. Left: " + left.Unparse() + ", Right: " + right.Unparse());
            return new IntDivNode(left, right);
        }

        public override ModulusNode CreateModulusNode(ExpressionNode left, ExpressionNode right)
        {
            Console.WriteLine("Creating a ModulusNode. Left: " + left.Unparse() + ", Right: " + right.Unparse());
            return new ModulusNode(left, right);
        }

        public override ExponentiationNode CreateExponentiationNode(ExpressionNode left, ExpressionNode right)
        {
            Console.WriteLine("Creating an ExponentiationNode. Left: " + left.Unparse() + ", Right: " + right.Unparse());
            return new ExponentiationNode(left, right);
        }

        public override LiteralNode CreateLiteralNode(object value)
        {
            Console.WriteLine($"Creating a LiteralNode with value: {value}");
            return new LiteralNode(value);
        }

        public override VariableNode CreateVariableNode(string name)
        {
            Console.WriteLine($"Creating a VariableNode with name: {name}");
            return new VariableNode(name);
        }

        public override AssignmentStmt CreateAssignmentStmt(VariableNode variable, ExpressionNode expression)
        {
            Console.WriteLine("Creating an AssignmentStmt. Variable: " + variable.Unparse() + ", Expression: " + expression.Unparse());
            return new AssignmentStmt(variable, expression);
        }

        public override ReturnStmt CreateReturnStmt(ExpressionNode expression)
        {
            Console.WriteLine("Creating a ReturnStmt. Expression: " + expression.Unparse());
            return new ReturnStmt(expression);
        }

        public override BlockStmt CreateBlockStmt(SymbolTable<string, object> symbolTable)
        {
            Console.WriteLine("Creating a BlockStmt");
            return new BlockStmt(symbolTable);
        }
    }
}