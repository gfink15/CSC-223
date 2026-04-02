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
        /// Initializes a new NameAnalysisVisitor with empty error lists.
        /// </summary>
        public NameAnalysisVisitor()
        {
            _error_stataments = new List<Statement>();
            _error_messages = new List<string>();
        }

        #region Expression Node Visit Methods

        /// <summary>
        /// Checks whether a variable exists in the current scope.
        /// Logs an error if the variable is undeclared.
        /// </summary>
        public bool Visit(VariableNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            if (tuple.Item1.ContainsKey(node.Name))
            {
                return true;
            }
            else
            {
                // Record the error with the offending statement context
                var _unparse_visitor = new UnparseVisitor();
                _error_stataments.Add(tuple.Item2);
                _error_messages.Add("Variable " + node.Name + $" not found in symbol table at line {tuple.Item2.Accept(_unparse_visitor, 0)}");
                return false;
            }
        }

        /// <summary>
        /// Literals always pass name analysis — no variable to resolve.
        /// </summary>
        public bool Visit(LiteralNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            return true;
        }

        /// <summary>
        /// Checks both operands of addition. Passes only if both pass.
        /// </summary>
        public bool Visit(PlusNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            if (node.Left.Accept(this, tuple) && node.Right.Accept(this, tuple))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks both operands of subtraction. Passes only if both pass.
        /// </summary>
        public bool Visit(MinusNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            if (node.Left.Accept(this, tuple) && node.Right.Accept(this, tuple))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks both operands of multiplication. Passes only if both pass.
        /// </summary>
        public bool Visit(TimesNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            if (node.Left.Accept(this, tuple) && node.Right.Accept(this, tuple))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks both operands of float division. Passes only if both pass.
        /// </summary>
        public bool Visit(FloatDivNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            if (node.Left.Accept(this, tuple) && node.Right.Accept(this, tuple))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks both operands of integer division. Passes only if both pass.
        /// </summary>
        public bool Visit(IntDivNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            if (node.Left.Accept(this, tuple) && node.Right.Accept(this, tuple))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks both operands of modulus. Passes only if both pass.
        /// </summary>
        public bool Visit(ModulusNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            if (node.Left.Accept(this, tuple) && node.Right.Accept(this, tuple))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks both operands of exponentiation. Passes only if both pass.
        /// </summary>
        public bool Visit(ExponentiationNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            if (node.Left.Accept(this, tuple) && node.Right.Accept(this, tuple))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Statement Node Visit Methods

        /// <summary>
        /// Assignment: first checks the RHS expression, then registers the
        /// variable in the current scope if no errors were found.
        /// </summary>
        public bool Visit(AssignmentStmt statement, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            bool error = !statement.Expression.Accept(this, new Tuple<SymbolTable<string, object>, Statement>(tuple.Item1, statement));

            if (error)
            {
                return false;
            }
            else
            {
                // Register the variable in the current scope
                tuple.Item1[statement.Variable.Name] = 0;
                return true;
            }

        }

        /// <summary>
        /// Return: validates that every variable in the expression is declared.
        /// </summary>
        public bool Visit(ReturnStmt statement, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            bool error = !statement.Expression.Accept(this, new Tuple<SymbolTable<string, object>, Statement>(tuple.Item1, statement));

            if (error) return false;
            else return true;

        }

        /// <summary>
        /// Block: creates a child scope and analyzes all contained statements.
        /// Continues checking all statements even after finding an error so
        /// that every issue is reported.
        /// </summary>
        public bool Visit(BlockStmt statement, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Child scope inherits from the enclosing scope
            SymbolTable<string, object> currentScope = statement.SymbolTable;

            bool noError = true;

            foreach (Statement s in statement.Statements)
            {
                if (noError)
                {
                    noError = s.Accept(this, new Tuple<SymbolTable<string, object>, Statement>(currentScope, statement));
                }
                else
                {
                    // Keep analyzing to collect all errors
                    s.Accept(this, new Tuple<SymbolTable<string, object>, Statement>(currentScope, statement));
                }
            }

            return noError;

        }

        #endregion
    }
}