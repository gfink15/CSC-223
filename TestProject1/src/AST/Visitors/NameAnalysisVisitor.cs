using System;
using System.Collections.Generic;
using System.Text;
using AST;
using Microsoft.Win32.SafeHandles;
using Utilities.Containers;

namespace AST
{
    /// <summary>
    /// Exception thrown when an evaluation error occurs
    /// </summary>
    public class NameAnalysisException(string message) : Exception(message) { }

    /// <summary>
    /// Visitor that evaluates an AST, executing the program and returning the final value
    /// Uses symbol tables to store variable values during execution
    /// </summary>
    public class NameAnalysisVisitor : IVisitor<Tuple<SymbolTable<string, object>, Statement>, bool>
    {
        // Flag to indicate if a return statement has been encountered
        private List<Statement> _error_stataments;

        private List<string> _error_messages;

        /// <summary>
        /// Initializes a new instance of the EvaluateVisitor class
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
            // Both sides must be valid because either side may reference variables.
            if (node.Left.Accept(this, tuple) && node.Right.Accept(this, tuple))
            {
                return true;
            }
            else
            {
                // Return failure when any operand fails analysis.
                return false;
            }
        }

        /// <summary>
        /// Validates identifier usage within both operands of a subtraction expression.
        /// </summary>
        /// <param name="node">The subtraction expression node.</param>
        /// <param name="tuple">Analysis context containing the active symbol table and owning statement.</param>
        /// <returns><c>true</c> when both operands pass analysis; otherwise <c>false</c>.</returns>
        public bool Visit(MinusNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Analyze the left and right expression subtrees recursively.
            if (node.Left.Accept(this, tuple) && node.Right.Accept(this, tuple))
            {
                return true;
            }
            else
            {
                // Any unresolved name in either subtree propagates failure upward.
                return false;
            }
        }

        /// <summary>
        /// Validates identifier usage within both operands of a multiplication expression.
        /// </summary>
        /// <param name="node">The multiplication expression node.</param>
        /// <param name="tuple">Analysis context containing the active symbol table and owning statement.</param>
        /// <returns><c>true</c> when both operands pass analysis; otherwise <c>false</c>.</returns>
        public bool Visit(TimesNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Require successful analysis for both factors.
            if (node.Left.Accept(this, tuple) && node.Right.Accept(this, tuple))
            {
                return true;
            }
            else
            {
                // Multiplication is invalid if either side contains unresolved names.
                return false;
            }
        }

        /// <summary>
        /// Validates identifier usage within both operands of a floating-point division expression.
        /// </summary>
        /// <param name="node">The floating-point division expression node.</param>
        /// <param name="tuple">Analysis context containing the active symbol table and owning statement.</param>
        /// <returns><c>true</c> when both operands pass analysis; otherwise <c>false</c>.</returns>
        public bool Visit(FloatDivNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Validate each child expression before this operator is considered valid.
            if (node.Left.Accept(this, tuple) && node.Right.Accept(this, tuple))
            {
                return true;
            }
            else
            {
                // Name-analysis failures in descendants are not recovered here.
                return false;
            }
        }

        /// <summary>
        /// Validates identifier usage within both operands of an integer division expression.
        /// </summary>
        /// <param name="node">The integer division expression node.</param>
        /// <param name="tuple">Analysis context containing the active symbol table and owning statement.</param>
        /// <returns><c>true</c> when both operands pass analysis; otherwise <c>false</c>.</returns>
        public bool Visit(IntDivNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Division operands are analyzed exactly like other binary expressions.
            if (node.Left.Accept(this, tuple) && node.Right.Accept(this, tuple))
            {
                return true;
            }
            else
            {
                // Return immediately when any nested variable lookup fails.
                return false;
            }
        }

        /// <summary>
        /// Validates identifier usage within both operands of a modulus expression.
        /// </summary>
        /// <param name="node">The modulus expression node.</param>
        /// <param name="tuple">Analysis context containing the active symbol table and owning statement.</param>
        /// <returns><c>true</c> when both operands pass analysis; otherwise <c>false</c>.</returns>
        public bool Visit(ModulusNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Check each side for unresolved variable names.
            if (node.Left.Accept(this, tuple) && node.Right.Accept(this, tuple))
            {
                return true;
            }
            else
            {
                // The operator itself performs no recovery; propagate failure.
                return false;
            }
        }

        /// <summary>
        /// Validates identifier usage within both operands of an exponentiation expression.
        /// </summary>
        /// <param name="node">The exponentiation expression node.</param>
        /// <param name="tuple">Analysis context containing the active symbol table and owning statement.</param>
        /// <returns><c>true</c> when both operands pass analysis; otherwise <c>false</c>.</returns>
        public bool Visit(ExponentiationNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Analyze base and exponent expressions recursively.
            if (node.Left.Accept(this, tuple) && node.Right.Accept(this, tuple))
            {
                return true;
            }
            else
            {
                // Preserve a simple pass/fail contract for parent nodes.
                return false;
            }
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