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
            _error_stataments = new List<Statement>();
            _error_messages = new List<string>();
        }

        #region Expression Node Visit Methods

        public bool Visit(VariableNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            // Variables return their value from the symbol table
            if (tuple.Item1.ContainsKey(node.Name))
            {
                return true;
            }
            else
            {
                var _unparse_visitor = new UnparseVisitor();
                _error_stataments.Add(tuple.Item2);
                _error_messages.Add("Variable " + node.Name + $" not found in symbol table at line {tuple.Item2.Accept(_unparse_visitor, 0)}");
                return false;
            }
        }

        public bool Visit(LiteralNode node, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            return true;
        }

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

        // TODO

        public bool Visit(AssignmentStmt statement, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            bool error = !statement.Expression.Accept(this, new Tuple<SymbolTable<string, object>, Statement>(tuple.Item1, statement));

            if (error)
            {
                return false;
            }
            else
            {
                tuple.Item1[statement.Variable.Name] = 0;
                return true;
            }

        }

        public bool Visit(ReturnStmt statement, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            bool error = !statement.Expression.Accept(this, new Tuple<SymbolTable<string, object>, Statement>(tuple.Item1, statement));

            if (error) return false;
            else return true;

        }

        public bool Visit(BlockStmt statement, Tuple<SymbolTable<string, object>, Statement> tuple)
        {
            var symbolTable = new SymbolTable<string, object>(tuple.Item1);
            bool error = false;

            foreach (Statement s in statement.Statements)
            {
                if (!error)
                {
                    error = s.Accept(this, new Tuple<SymbolTable<string, object>, Statement>(symbolTable, statement));
                }
                else
                {
                    s.Accept(this, new Tuple<SymbolTable<string, object>, Statement>(symbolTable, statement));
                }
            }

            return !error;

        }

        #endregion
    }
}