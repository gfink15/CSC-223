/**
 * Default Builder: Concrete implementation of the Builder pattern for constructing
 * AST nodes. All Create methods are marked virtual so that subclasses (DebugBuilder)
 * can override them to add behaviour such as diagnostic output while still
 * delegating the actual node construction back to this base implementation.
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
    /// Concrete builder that constructs every AST node type by directly
    /// instantiating the corresponding class. This is the standard builder
    /// used during normal parsing; it performs no extra work beyond allocation.
    ///
    /// All methods are virtual so that subclasses such as DebugBuilder can
    /// intercept calls (e.g. to log diagnostic output) and then delegate to
    /// this base implementation via base.CreateXxx().
    /// </summary>
    public class DefaultBuilder
    {
        // ── Binary Operator Nodes ─────────────────────────────────────────────
        // Each method below follows the same pattern: accept left and right
        // operand expressions, construct the corresponding operator node, and
        // return it. The methods are intentionally thin — all structural logic
        // lives in the AST node classes themselves.

        /// <summary>Creates a PlusNode representing the addition of two expressions.</summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>A new PlusNode with the given operands as children.</returns>
        public virtual PlusNode CreatePlusNode(ExpressionNode left, ExpressionNode right)
        {
            return new PlusNode(left, right);
        }

        /// <summary>Creates a MinusNode representing the subtraction of two expressions.</summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>A new MinusNode with the given operands as children.</returns>
        public virtual MinusNode CreateMinusNode(ExpressionNode left, ExpressionNode right)
        {
            return new MinusNode(left, right);
        }

        /// <summary>Creates a TimesNode representing the multiplication of two expressions.</summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>A new TimesNode with the given operands as children.</returns>
        public virtual TimesNode CreateTimesNode(ExpressionNode left, ExpressionNode right)
        {
            return new TimesNode(left, right);
        }

        /// <summary>Creates a FloatDivNode representing floating-point division of two expressions.</summary>
        /// <param name="left">The left operand expression (the dividend).</param>
        /// <param name="right">The right operand expression (the divisor).</param>
        /// <returns>A new FloatDivNode with the given operands as children.</returns>
        public virtual FloatDivNode CreateFloatDivNode(ExpressionNode left, ExpressionNode right)
        {
            return new FloatDivNode(left, right);
        }

        /// <summary>Creates an IntDivNode representing integer division (//) of two expressions.</summary>
        /// <param name="left">The left operand expression (the dividend).</param>
        /// <param name="right">The right operand expression (the divisor).</param>
        /// <returns>A new IntDivNode with the given operands as children.</returns>
        public virtual IntDivNode CreateIntDivNode(ExpressionNode left, ExpressionNode right)
        {
            return new IntDivNode(left, right);
        }

        /// <summary>Creates a ModulusNode representing the remainder (%) of two expressions.</summary>
        /// <param name="left">The left operand expression (the dividend).</param>
        /// <param name="right">The right operand expression (the divisor).</param>
        /// <returns>A new ModulusNode with the given operands as children.</returns>
        public virtual ModulusNode CreateModulusNode(ExpressionNode left, ExpressionNode right)
        {
            return new ModulusNode(left, right);
        }

        /// <summary>Creates an ExponentiationNode representing the raising of a base to a power (**).</summary>
        /// <param name="left">The base expression.</param>
        /// <param name="right">The exponent expression.</param>
        /// <returns>A new ExponentiationNode with the given operands as children.</returns>
        public virtual ExponentiationNode CreateExponentiationNode(ExpressionNode left, ExpressionNode right)
        {
            return new ExponentiationNode(left, right);
        }

        // ── Leaf Nodes ────────────────────────────────────────────────────────
        // Literal and variable nodes are leaves in the AST — they have no
        // child expressions and directly store a value or identifier name.

        /// <summary>
        /// Creates a LiteralNode wrapping the given value. The value may be any
        /// object (e.g. int, double) and is stored as-is for later use during
        /// unparsing or evaluation.
        /// </summary>
        /// <param name="value">The literal value to store in the node.</param>
        /// <returns>A new LiteralNode containing the given value.</returns>
        public virtual LiteralNode CreateLiteralNode(object value)
        {
            return new LiteralNode(value);
        }

        /// <summary>
        /// Creates a VariableNode representing a reference to a named variable.
        /// The name is stored as-is and emitted verbatim during unparsing.
        /// </summary>
        /// <param name="name">The identifier name of the variable.</param>
        /// <returns>A new VariableNode with the given name.</returns>
        public virtual VariableNode CreateVariableNode(string name)
        {
            return new VariableNode(name);
        }

        // ── Statement Nodes ───────────────────────────────────────────────────

        /// <summary>
        /// Creates an AssignmentStmt representing a variable assignment, e.g. x := (2 + 3).
        /// The variable node becomes the left-hand side and the expression becomes
        /// the right-hand side of the := operator.
        /// </summary>
        /// <param name="variable">The target variable being assigned to.</param>
        /// <param name="expression">The value expression whose result is assigned.</param>
        /// <returns>A new AssignmentStmt with the given variable and expression.</returns>
        public virtual AssignmentStmt CreateAssignmentStmt(VariableNode variable, ExpressionNode expression)
        {
            return new AssignmentStmt(variable, expression);
        }

        /// <summary>
        /// Creates a ReturnStmt that returns the result of the given expression
        /// from the current block, e.g. return (a // b).
        /// </summary>
        /// <param name="expression">The expression whose value is returned.</param>
        /// <returns>A new ReturnStmt wrapping the given expression.</returns>
        public virtual ReturnStmt CreateReturnStmt(ExpressionNode expression)
        {
            return new ReturnStmt(expression);
        }

        /// <summary>
        /// Creates an empty BlockStmt associated with the given symbol table.
        /// Statements are added to the block after construction via BlockStmt.Add().
        /// The symbol table represents the scope introduced by this block and will
        /// be populated with variable bindings during interpretation.
        /// </summary>
        /// <param name="symbolTable">The symbol table for this block's scope.</param>
        /// <returns>A new, empty BlockStmt with the given symbol table.</returns>
        public virtual BlockStmt CreateBlockStmt(SymbolTable<string, object> symbolTable)
        {
            return new BlockStmt(symbolTable);
        }
    }
}