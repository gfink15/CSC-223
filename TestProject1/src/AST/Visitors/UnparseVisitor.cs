using System;
using System.Text;
using AST;
using Utilities;

/**
 * UnparseVisitor: Converts an AST back into DEC source code.
 * Implements IVisitor<int, string> where the int parameter is the
 * indentation level and the string result is the unparsed output.
 *
 * Bare literals and variables in statement contexts (assignment RHS,
 * return expression) are wrapped in parentheses so the output is
 * re-parseable by the Parser.
 *
 * Bugs: None known.
 *
 * @author Graham Fink, Mridul Agrawal
 * @date   3/30/2026
 */

namespace AST
{
    public class UnparseVisitor : IVisitor<int, string>
    {
        /// <summary>
        /// Entry point: unparses an expression node at the given indentation level.
        /// </summary>
        /// <param name="node">The expression node to unparse.</param>
        /// <param name="level">Current indentation depth (default 0).</param>
        /// <returns>The string representation of the expression.</returns>
        public string Unparse(ExpressionNode node, int level = 0)
        {
            return node.Accept(this, level);
        }

        #region Expression Node Visit Methods

        /// <summary>
        /// Unparses an addition: (left + right).
        /// </summary>
        public string Visit(PlusNode node, int level)
        {
            string left = node.Left.Accept(this, level);
            string right = node.Right.Accept(this, level);
            return $"({left} + {right})";
        }

        /// <summary>
        /// Unparses a subtraction: (left - right).
        /// </summary>
        public string Visit(MinusNode node, int level)
        {
            string left = node.Left.Accept(this, level);
            string right = node.Right.Accept(this, level);
            return $"({left} - {right})";
        }

        /// <summary>
        /// Unparses a multiplication: (left * right).
        /// </summary>
        public string Visit(TimesNode node, int level)
        {
            string left = node.Left.Accept(this, level);
            string right = node.Right.Accept(this, level);
            return $"({left} * {right})";
        }

        /// <summary>
        /// Unparses a float division: (left / right).
        /// </summary>
        public string Visit(FloatDivNode node, int level)
        {
            string left = node.Left.Accept(this, level);
            string right = node.Right.Accept(this, level);
            return $"({left} / {right})";
        }

        /// <summary>
        /// Unparses an integer division: (left // right).
        /// </summary>
        public string Visit(IntDivNode node, int level)
        {
            string left = node.Left.Accept(this, level);
            string right = node.Right.Accept(this, level);
            return $"({left} // {right})";
        }

        /// <summary>
        /// Unparses a modulus operation: (left % right).
        /// </summary>
        public string Visit(ModulusNode node, int level)
        {
            string left = node.Left.Accept(this, level);
            string right = node.Right.Accept(this, level);
            return $"({left} % {right})";
        }

        /// <summary>
        /// Unparses an exponentiation: (left ** right).
        /// </summary>
        public string Visit(ExponentiationNode node, int level)
        {
            string left = node.Left.Accept(this, level);
            string right = node.Right.Accept(this, level);
            return $"({left} ** {right})";
        }

        /// <summary>
        /// Unparses a literal node by converting its value to a string.
        /// </summary>
        public string Visit(LiteralNode node, int level)
        {
            return node.Data.ToString();
        }

        /// <summary>
        /// Unparses a variable node by returning its name.
        /// </summary>
        public string Visit(VariableNode node, int level)
        {
            return node.Name;
        }

        #endregion

        #region Statement Node Visit Methods

        /// <summary>
        /// Unparses an assignment: indent + variable := expression.
        /// Wraps bare literals/variables in parens for parser compatibility.
        /// </summary>
        public string Visit(AssignmentStmt statement, int level)
        {
            string e = statement.Expression.Accept(this, level);

            // Wrap bare values so the output is re-parseable
            if (statement.Expression is LiteralNode || statement.Expression is VariableNode)
            {
                e = $"({e})";
            }
            return GeneralUtils.GetIndentation(level) + $"{statement.Variable.Accept(this, level)} := {e}";
        }

        /// <summary>
        /// Unparses a return statement: indent + return expression.
        /// Wraps bare literals/variables in parens for parser compatibility.
        /// </summary>
        public string Visit(ReturnStmt statement, int level)
        {
            string e = statement.Expression.Accept(this, level);

            // Wrap bare values so the output is re-parseable
            if (statement.Expression is LiteralNode || statement.Expression is VariableNode)
            {
                e = $"({e})";
            }
            return GeneralUtils.GetIndentation(level) + $"return {e}";
        }

        /// <summary>
        /// Unparses a block: { stmts } with each child indented one level deeper.
        /// </summary>
        public string Visit(BlockStmt statement, int level)
        {
            string builder = GeneralUtils.GetIndentation(level) + "{";

            // Go through every statement in the block and indent at one level higher than parent.
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