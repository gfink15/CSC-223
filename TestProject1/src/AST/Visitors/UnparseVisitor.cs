using System;
using System.Text;
using AST;
using Parser.Tests;
using Utilities;

namespace AST
{
    /// <summary>
    /// Visitor implementation that unparses the AST back to string representation
    /// Uses the generic visitor pattern with indentation level as parameter and string as result
    /// </summary>
    public class UnparseVisitor : IVisitor<int, string>
    {
        /// <summary>
        /// Unparses the given AST node with the specified indentation level
        /// </summary>
        /// <param name="node">The AST node to unparse</param>
        /// <param name="level">The indentation level</param>
        /// <returns>String representation of the node</returns>
        public string Unparse(ExpressionNode node, int level = 0)
        {
            return node.Accept(this, level);
        }

        /// <summary>
        /// Unparses the given statement with the specified indentation level
        /// </summary>
        /// <param name="stmt">The statement to unparse</param>
        /// <param name="level">The indentation level</param>
        /// <returns>String representation of the statement</returns>
        // public string Unparse(Statement stmt, int level = 0)
        // {
        //     return stmt.Accept(this, level);
        // }

        #region Expression Node Visit Methods

        public string Visit(BinaryOperator node, int level)
        {
            string left = node.Left.Accept(this, level);
            string right = node.Right.Accept(this, level);
            string op = node.ToString();
            return $"({left} {op} {right})";
        }

        public string Visit(PlusNode node, int level)
        {
            string left = node.Left.Accept(this, level);
            string right = node.Right.Accept(this, level);
            return $"({left} + {right})";
        }

        public string Visit(MinusNode node, int level)
        {
            string left = node.Left.Accept(this, level);
            string right = node.Right.Accept(this, level);
            return $"({left} - {right})";
        }

        public string Visit(TimesNode node, int level)
        {
            string left = node.Left.Accept(this, level);
            string right = node.Right.Accept(this, level);
            return $"({left} * {right})";
        }

        public string Visit(FloatDivNode node, int level)
        {
            string left = node.Left.Accept(this, level);
            string right = node.Right.Accept(this, level);
            return $"({left} / {right})";
        }

        public string Visit(IntDivNode node, int level)
        {
            string left = node.Left.Accept(this, level);
            string right = node.Right.Accept(this, level);
            return $"({left} // {right})";
        }

        public string Visit(ModulusNode node, int level)
        {
            string left = node.Left.Accept(this, level);
            string right = node.Right.Accept(this, level);
            return $"({left} % {right})";
        }

        public string Visit(ExponentiationNode node, int level)
        {
            string left = node.Left.Accept(this, level);
            string right = node.Right.Accept(this, level);
            return $"({left} ** {right})";
        }

        public string Visit(LiteralNode node, int level)
        {
            return node.Data.ToString();
        }

        public string Visit(VariableNode node, int level)
        {
            return node.Name;
        }
        #endregion

        #region Statement Node Visit Methods

        public string Visit(AssignmentStmt statement, int level)
        {
            return GeneralUtils.GetIndentation(level) + $"{statement.Variable.Accept(this, level)} := {statement.Expression.Accept(this, level)}";
        }

        public string Visit(ReturnStmt statement, int level)
        {
            return GeneralUtils.GetIndentation(level) + $"return {statement.Expression.Accept(this, level)}";
        }

        public string Visit(BlockStmt statement, int level)
        {
            string builder = GeneralUtils.GetIndentation(level) + "{";

            foreach (Statement s in statement.Statements)
            {
                builder += "\n" + s.Accept(this, level + 1);
            }

            builder += "\n" + GeneralUtils.GetIndentation(level) + "}";
            return builder;
        }

        #endregion
    }
}