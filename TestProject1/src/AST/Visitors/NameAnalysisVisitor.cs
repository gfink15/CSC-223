using System;
using System.Collections.Generic;
using System.Text;
using AST;
using Utilities.Containers;

/**
 * NameAnalysisVisitor: Performs static name analysis on an AST.
 * Checks that every variable referenced in an expression has been
 * declared (exists in the symbol table) before use.
 *
 * Implements IVisitor<Tuple<SymbolTable, Statement>, bool> where:
 *   - The Tuple carries the current scope (symbol table) and the
 *     enclosing statement (for error reporting).
 *   - The bool result indicates whether analysis passed (true) or
 *     found an undeclared variable (false).
 *
 * Bugs: None known.
 *
 * @author Graham Fink, Mridul Agrawal
 * @date   3/30/2026
 */

namespace AST
{
    /// <summary>
    /// Exception thrown when a name analysis error occurs.
    /// </summary>
    public class NameAnalysisException(string message) : Exception(message) { }

    /// <summary>
    /// Visitor that performs static name analysis on an AST.
    /// Verifies every variable is declared before use and collects
    /// error messages for all undeclared variable references.
    /// </summary>
    public class NameAnalysisVisitor : IVisitor<Tuple<SymbolTable<string, object>, Statement>, bool>
    {
        /// <summary>Statements that caused name-analysis errors.</summary>
        private List<Statement> _error_stataments;

        /// <summary>Human-readable error descriptions for each failure.</summary>
        private List<string> _error_messages;

        /// <summary>
        /// Public read-only access to the list of statements that caused errors.
        /// </summary>
        public List<Statement> ErrorStatements { get { return _error_stataments; } }

        /// <summary>
        /// Public read-only access to the list of error messages.
        /// </summary>
        public List<string> ErrorMessages { get { return _error_messages; } }

        /// <summary>
        /// Initializes a new NameAnalysisVisitor with empty error lists.
        /// </summary>
        public NameAnalysisVisitor()
        {
            // Track statements that fail name analysis so callers can inspect context.
            _error_stataments = new List<Statement>();
            // Store human-readable error messages that correspond to failed statements.
            _error_messages = new List<string>();
        }

        #region Expression Node Visit Methods

        /// <summary>
        /// Validates that a referenced variable exists in the current symbol table scope chain.
        /// </summary>
        /// <param name="node">The variable expression node being analyzed.</param>
        /// <param name="tuple">Analysis context containing the active symbol table and owning statement.</param>
        /// <returns><c>true</c> when the variable name is resolvable; otherwise <c>false</c>.</returns>
        public bool Visit(VariableNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // If the variable is already declared in an accessible scope, analysis succeeds.
            if (tuple.Item1.ContainsKey(node.Name))
            {
                return true;
            }
            else
            {
                // Build an unparsed statement representation to include precise source context.
                var _unparse_visitor = new UnparseVisitor();
                // Record the statement and message so all errors can be reported together.
                _error_stataments.Add(tuple.Item2);
                _error_messages.Add("Variable " + node.Name + $" not found in symbol table at line {tuple.Item2.Accept(_unparse_visitor, 0)}");
                return false;
            }
        }

        /// <summary>
        /// Accepts literal nodes because literals do not require name resolution.
        /// </summary>
        /// <param name="node">The literal expression node.</param>
        /// <param name="tuple">Analysis context containing the active symbol table and owning statement.</param>
        /// <returns>Always <c>true</c>.</returns>
        public bool Visit(LiteralNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Literal values introduce no identifiers, so there is nothing to validate.
            return true;
        }

        /// <summary>
        /// Validates identifier usage within both operands of an addition expression.
        /// </summary>
        /// <param name="node">The addition expression node.</param>
        /// <param name="tuple">Analysis context containing the active symbol table and owning statement.</param>
        /// <returns><c>true</c> when both operands pass analysis; otherwise <c>false</c>.</returns>
        public bool Visit(PlusNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Evaluate both sides independently to collect all errors (no short-circuit).
            bool left = node.Left.Accept(this, tuple);
            bool right = node.Right.Accept(this, tuple);
            return left & right;
        }

        /// <summary>
        /// Validates identifier usage within both operands of a subtraction expression.
        /// </summary>
        /// <param name="node">The subtraction expression node.</param>
        /// <param name="tuple">Analysis context containing the active symbol table and owning statement.</param>
        /// <returns><c>true</c> when both operands pass analysis; otherwise <c>false</c>.</returns>
        public bool Visit(MinusNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Evaluate both sides independently to collect all errors (no short-circuit).
            bool left = node.Left.Accept(this, tuple);
            bool right = node.Right.Accept(this, tuple);
            return left & right;
        }

        /// <summary>
        /// Validates identifier usage within both operands of a multiplication expression.
        /// </summary>
        /// <param name="node">The multiplication expression node.</param>
        /// <param name="tuple">Analysis context containing the active symbol table and owning statement.</param>
        /// <returns><c>true</c> when both operands pass analysis; otherwise <c>false</c>.</returns>

        /// <summary>
        /// Checks both operands of multiplication. Passes only if both pass.
        /// </summary>
        public bool Visit(TimesNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Evaluate both sides independently to collect all errors (no short-circuit).
            bool left = node.Left.Accept(this, tuple);
            bool right = node.Right.Accept(this, tuple);
            return left & right;
        }

        /// <summary>
        /// Validates identifier usage within both operands of a floating-point division expression.
        /// </summary>
        /// <param name="node">The floating-point division expression node.</param>
        /// <param name="tuple">Analysis context containing the active symbol table and owning statement.</param>
        /// <returns><c>true</c> when both operands pass analysis; otherwise <c>false</c>.</returns>
        public bool Visit(FloatDivNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Evaluate both sides independently to collect all errors (no short-circuit).
            bool left = node.Left.Accept(this, tuple);
            bool right = node.Right.Accept(this, tuple);
            return left & right;
        }

        /// <summary>
        /// Validates identifier usage within both operands of an integer division expression.
        /// </summary>
        /// <param name="node">The integer division expression node.</param>
        /// <param name="tuple">Analysis context containing the active symbol table and owning statement.</param>
        /// <returns><c>true</c> when both operands pass analysis; otherwise <c>false</c>.</returns>
        public bool Visit(IntDivNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Evaluate both sides independently to collect all errors (no short-circuit).
            bool left = node.Left.Accept(this, tuple);
            bool right = node.Right.Accept(this, tuple);
            return left & right;
        }

        /// <summary>
        /// Validates identifier usage within both operands of a modulus expression.
        /// </summary>
        /// <param name="node">The modulus expression node.</param>
        /// <param name="tuple">Analysis context containing the active symbol table and owning statement.</param>
        /// <returns><c>true</c> when both operands pass analysis; otherwise <c>false</c>.</returns>
        public bool Visit(ModulusNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Evaluate both sides independently to collect all errors (no short-circuit).
            bool left = node.Left.Accept(this, tuple);
            bool right = node.Right.Accept(this, tuple);
            return left & right;
        }

        /// <summary>
        /// Validates identifier usage within both operands of an exponentiation expression.
        /// </summary>
        /// <param name="node">The exponentiation expression node.</param>
        /// <param name="tuple">Analysis context containing the active symbol table and owning statement.</param>
        /// <returns><c>true</c> when both operands pass analysis; otherwise <c>false</c>.</returns>
        public bool Visit(ExponentiationNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Evaluate both sides independently to collect all errors (no short-circuit).
            bool left = node.Left.Accept(this, tuple);
            bool right = node.Right.Accept(this, tuple);
            return left & right;
        }

        #endregion

        #region Statement Node Visit Methods

        // TODO

        /// <summary>
        /// Validates the expression assigned to a variable and records the variable in the current scope.
        /// </summary>
        /// <param name="statement">The assignment statement to analyze.</param>
        /// <param name="tuple">Analysis context containing the active symbol table and parent statement.</param>
        /// <returns><c>true</c> when the right-hand expression passes analysis; otherwise <c>false</c>.</returns>
        public bool Visit(AssignmentStmt statement, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Analyze only the expression side first; variable declaration/update occurs on success.
            bool error = !statement.Expression.Accept(this, new Tuple<SymbolTable<string, object>, Statement>(tuple.Item1, statement));

            if (error)
            {
                // If the right-hand side has unresolved names, keep scope unchanged.
                return false;
            }
            else
            {
                // Mark the assigned variable as present in the current scope.
                tuple.Item1[statement.Variable.Name] = 0;
                return true;
            }

        }

        /// <summary>
        /// Validates identifier usage in a return expression.
        /// </summary>
        /// <param name="statement">The return statement being analyzed.</param>
        /// <param name="tuple">Analysis context containing the active symbol table and parent statement.</param>
        /// <returns><c>true</c> when the return expression passes analysis; otherwise <c>false</c>.</returns>
        public bool Visit(ReturnStmt statement, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // The return statement contributes no new symbols; only its expression is checked.
            bool error = !statement.Expression.Accept(this, new Tuple<SymbolTable<string, object>, Statement>(tuple.Item1, statement));

            // Return success only when expression analysis succeeds.
            if (error) return false;
            else return true;

        }

        /// <summary>
        /// Validates each statement in a block using a nested symbol table scope.
        /// </summary>
        /// <param name="statement">The block statement containing child statements.</param>
        /// <param name="tuple">Analysis context containing the parent symbol table and parent statement.</param>
        /// <returns><c>true</c> if all statements pass analysis; otherwise <c>false</c>.</returns>
        public bool Visit(BlockStmt statement, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Create a child scope so block-local assignments do not leak to outer scopes.
            var symbolTable = new SymbolTable<string, object>(tuple.Item1);
            // Track whether all statements seen so far have passed analysis.
            bool noError = true;

            foreach (Statement s in statement.Statements)
            {
                if (noError)
                {
                    // While no errors exist, preserve the first failure status.
                    noError = s.Accept(this, new Tuple<SymbolTable<string, object>, Statement>(symbolTable, statement));
                }
                else
                {
                    // Continue traversal after failure to gather additional diagnostics.
                    s.Accept(this, new Tuple<SymbolTable<string, object>, Statement>(symbolTable, statement));
                }
            }

            // Caller receives aggregate success/failure for the whole block.
            return noError;

        }

        #endregion
    }
}