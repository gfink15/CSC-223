/**
 * Debug Builder: Diagnostic subclass of DefaultBuilder that logs each node
 * creation to the console before delegating to the standard constructor.
 * Claude AI was only used for writing XML and inline comments.
 *
 * @author Graham Fink, Mridul Agrawal
 * @date   2/25/2026
 */
using System;
using System.Collections.Generic;
using Utilities.Containers;

namespace AST
{
    /// <summary>
    /// A diagnostic builder that extends DefaultBuilder to print a message to
    /// the console each time a node is created. This is useful for tracing the
    /// parser's behaviour and verifying that the correct nodes are being built
    /// in the correct order, without changing any of the returned objects.
    ///
    /// Every method logs the node type and its key data (operands, value, name)
    /// via Console.WriteLine, then constructs and returns the same object that
    /// DefaultBuilder would produce. The caller receives a fully usable AST node
    /// identical to what DefaultBuilder would have returned.
    /// </summary>
    public class DebugBuilder : DefaultBuilder
    {
        // ── Binary Operator Nodes ─────────────────────────────────────────────
        // Each override below logs the node type and the unparsed forms of both
        // operands before constructing the node. Unparse() is called on the
        // operands rather than storing them as strings so that nested expressions
        // are always represented in their fully expanded, human-readable form.

        /// <summary>
        /// Logs the creation of a PlusNode with its operands, then constructs and
        /// returns the node exactly as DefaultBuilder would.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>A new PlusNode with the given operands as children.</returns>
        public override PlusNode CreatePlusNode(ExpressionNode left, ExpressionNode right)
        {
            // Print the node type and the unparsed source text of each operand
            // so the trace output mirrors the original expression structure.
            Console.WriteLine("Creating a PlusNode. Left: " + left.Unparse() + ", Right: " + right.Unparse());
            return new PlusNode(left, right);
        }

        /// <summary>
        /// Logs the creation of a MinusNode with its operands, then constructs and
        /// returns the node exactly as DefaultBuilder would.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>A new MinusNode with the given operands as children.</returns>
        public override MinusNode CreateMinusNode(ExpressionNode left, ExpressionNode right)
        {
            Console.WriteLine("Creating a MinusNode. Left: " + left.Unparse() + ", Right: " + right.Unparse());
            return new MinusNode(left, right);
        }

        /// <summary>
        /// Logs the creation of a TimesNode with its operands, then constructs and
        /// returns the node exactly as DefaultBuilder would.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>A new TimesNode with the given operands as children.</returns>
        public override TimesNode CreateTimesNode(ExpressionNode left, ExpressionNode right)
        {
            Console.WriteLine("Creating a TimesNode. Left: " + left.Unparse() + ", Right: " + right.Unparse());
            return new TimesNode(left, right);
        }

        /// <summary>
        /// Logs the creation of a FloatDivNode with its operands, then constructs
        /// and returns the node exactly as DefaultBuilder would.
        /// </summary>
        /// <param name="left">The left operand expression (the dividend).</param>
        /// <param name="right">The right operand expression (the divisor).</param>
        /// <returns>A new FloatDivNode with the given operands as children.</returns>
        public override FloatDivNode CreateFloatDivNode(ExpressionNode left, ExpressionNode right)
        {
            Console.WriteLine("Creating a FloatDivNode. Left: " + left.Unparse() + ", Right: " + right.Unparse());
            return new FloatDivNode(left, right);
        }

        /// <summary>
        /// Logs the creation of an IntDivNode with its operands, then constructs
        /// and returns the node exactly as DefaultBuilder would.
        /// </summary>
        /// <param name="left">The left operand expression (the dividend).</param>
        /// <param name="right">The right operand expression (the divisor).</param>
        /// <returns>A new IntDivNode with the given operands as children.</returns>
        public override IntDivNode CreateIntDivNode(ExpressionNode left, ExpressionNode right)
        {
            Console.WriteLine("Creating an IntDivNode. Left: " + left.Unparse() + ", Right: " + right.Unparse());
            return new IntDivNode(left, right);
        }

        /// <summary>
        /// Logs the creation of a ModulusNode with its operands, then constructs
        /// and returns the node exactly as DefaultBuilder would.
        /// </summary>
        /// <param name="left">The left operand expression (the dividend).</param>
        /// <param name="right">The right operand expression (the divisor).</param>
        /// <returns>A new ModulusNode with the given operands as children.</returns>
        public override ModulusNode CreateModulusNode(ExpressionNode left, ExpressionNode right)
        {
            Console.WriteLine("Creating a ModulusNode. Left: " + left.Unparse() + ", Right: " + right.Unparse());
            return new ModulusNode(left, right);
        }

        /// <summary>
        /// Logs the creation of an ExponentiationNode with its operands, then
        /// constructs and returns the node exactly as DefaultBuilder would.
        /// </summary>
        /// <param name="left">The base expression.</param>
        /// <param name="right">The exponent expression.</param>
        /// <returns>A new ExponentiationNode with the given operands as children.</returns>
        public override ExponentiationNode CreateExponentiationNode(ExpressionNode left, ExpressionNode right)
        {
            Console.WriteLine("Creating an ExponentiationNode. Left: " + left.Unparse() + ", Right: " + right.Unparse());
            return new ExponentiationNode(left, right);
        }

        // ── Leaf Nodes ────────────────────────────────────────────────────────
        // For leaf nodes there are no child expressions to unparse, so the log
        // message uses an interpolated string to directly embed the stored value
        // or name instead.

        /// <summary>
        /// Logs the creation of a LiteralNode with its stored value, then
        /// constructs and returns the node exactly as DefaultBuilder would.
        /// </summary>
        /// <param name="value">The literal value to store in the node.</param>
        /// <returns>A new LiteralNode containing the given value.</returns>
        public override LiteralNode CreateLiteralNode(object value)
        {
            // String interpolation is used here (and for VariableNode) because
            // there are no child nodes to unparse — the value is logged directly.
            Console.WriteLine($"Creating a LiteralNode with value: {value}");
            return new LiteralNode(value);
        }

        /// <summary>
        /// Logs the creation of a VariableNode with its identifier name, then
        /// constructs and returns the node exactly as DefaultBuilder would.
        /// </summary>
        /// <param name="name">The identifier name of the variable.</param>
        /// <returns>A new VariableNode with the given name.</returns>
        public override VariableNode CreateVariableNode(string name)
        {
            Console.WriteLine($"Creating a VariableNode with name: {name}");
            return new VariableNode(name);
        }

        // ── Statement Nodes ───────────────────────────────────────────────────

        /// <summary>
        /// Logs the creation of an AssignmentStmt with its target variable and
        /// value expression, then constructs and returns the statement exactly
        /// as DefaultBuilder would.
        /// </summary>
        /// <param name="variable">The target variable being assigned to.</param>
        /// <param name="expression">The value expression whose result is assigned.</param>
        /// <returns>A new AssignmentStmt with the given variable and expression.</returns>
        public override AssignmentStmt CreateAssignmentStmt(VariableNode variable, ExpressionNode expression)
        {
            // Unparse both children so the log shows the full source-level text,
            // e.g. "Variable: x, Expression: (2+3)" rather than raw object references.
            Console.WriteLine("Creating an AssignmentStmt. Variable: " + variable.Unparse() + ", Expression: " + expression.Unparse());
            return new AssignmentStmt(variable, expression);
        }

        /// <summary>
        /// Logs the creation of a ReturnStmt with its return expression, then
        /// constructs and returns the statement exactly as DefaultBuilder would.
        /// </summary>
        /// <param name="expression">The expression whose value is returned.</param>
        /// <returns>A new ReturnStmt wrapping the given expression.</returns>
        public override ReturnStmt CreateReturnStmt(ExpressionNode expression)
        {
            Console.WriteLine("Creating a ReturnStmt. Expression: " + expression.Unparse());
            return new ReturnStmt(expression);
        }

        /// <summary>
        /// Logs the creation of a BlockStmt, then constructs and returns the
        /// statement exactly as DefaultBuilder would. The symbol table is not
        /// logged since it has no meaningful string representation at this stage.
        /// </summary>
        /// <param name="symbolTable">The symbol table for this block's scope.</param>
        /// <returns>A new, empty BlockStmt with the given symbol table.</returns>
        public override BlockStmt CreateBlockStmt(SymbolTable<string, object> symbolTable)
        {
            // No operands or values to log for a block — just note that one is being created.
            Console.WriteLine("Creating a BlockStmt");
            return new BlockStmt(symbolTable);
        }
    }
}