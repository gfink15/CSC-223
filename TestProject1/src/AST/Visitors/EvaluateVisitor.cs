using System;
using System.Collections.Generic;
using System.Text;
using AST;
using Utilities.Containers;

namespace AST
{
    /// <summary>
    /// Exception thrown when an evaluation error occurs
    /// </summary>
    public class EvaluationException : Exception
    {
        public EvaluationException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Visitor that evaluates an AST, executing the program and returning the final value
    /// Uses symbol tables to store variable values during execution
    /// </summary>
    public class EvaluateVisitor : IVisitor<SymbolTable<string, object>, object>
    {
        // Flag to indicate if a return statement has been encountered
        private bool _returnEncountered;

        // Value from the return statement
        private object _returnValue;

        /// <summary>
        /// Initializes a new instance of the EvaluateVisitor class
        /// </summary>
        public EvaluateVisitor()
        {
            _returnEncountered = false;
            _returnValue = null;
        }

        /// <summary>
        /// Evaluates the given AST and returns the result
        /// </summary>
        /// <param name="ast">The AST to evaluate</param>
        /// <returns>The result of the evaluation (typically from a return statement)</returns>
        public object Evaluate(Statement ast)
        {
            _returnEncountered = false;
            _returnValue = null;

            // Execute the AST with a null initial scope
            // (the BlockStmt will use its own symbol table)
            ast.Accept(this, null);

            return _returnValue;
        }

        // TODO

        #region Expression Node Visit Methods

        // TODO

        public object Visit(VariableNode node, SymbolTable<string, object> symbolTable)
        {
            // Variables return their value from the symbol table
            return symbolTable[node.Name];
        }

        // public object Visit(BinaryOperator node, SymbolTable<string, object> symbolTable)
        // {
        //     string left = node.Left.Accept(this, symbolTable);
        //     string right = node.Right.Accept(this, symbolTable);
        //     string op = node.ToString();
        //     return $"({left} {op} {right})";
        // }

        public object Visit(PlusNode node, SymbolTable<string, object> symbolTable)
        {
            object left = node.Left.Accept(this, symbolTable);
            object right = node.Right.Accept(this, symbolTable);
            return Convert.ToDouble(left) + Convert.ToDouble(right);
        }

        public object Visit(MinusNode node, SymbolTable<string, object> symbolTable)
        {
            object left = node.Left.Accept(this, symbolTable);
            object right = node.Right.Accept(this, symbolTable);
            return Convert.ToDouble(left) - Convert.ToDouble(right);
        }

        public object Visit(TimesNode node, SymbolTable<string, object> symbolTable)
        {
            object left = node.Left.Accept(this, symbolTable);
            object right = node.Right.Accept(this, symbolTable);
            return Convert.ToDouble(left) * Convert.ToDouble(right);
        }

        public object Visit(FloatDivNode node, SymbolTable<string, object> symbolTable)
        {
            object left = node.Left.Accept(this, symbolTable);
            object right = node.Right.Accept(this, symbolTable);
            return Convert.ToDouble(left) / Convert.ToDouble(right);
        }

        public object Visit(IntDivNode node, SymbolTable<string, object> symbolTable)
        {
            object left = node.Left.Accept(this, symbolTable);
            object right = node.Right.Accept(this, symbolTable);
            return Convert.ToInt32(left) / Convert.ToInt32(right);
        }

        public object Visit(ModulusNode node, SymbolTable<string, object> symbolTable)
        {
            object left = node.Left.Accept(this, symbolTable);
            object right = node.Right.Accept(this, symbolTable);
            return Convert.ToDouble(left) % Convert.ToDouble(right);
        }

        public object Visit(ExponentiationNode node, SymbolTable<string, object> symbolTable)
        {
            object left = node.Left.Accept(this, symbolTable);
            object right = node.Right.Accept(this, symbolTable);
            return Math.Pow(Convert.ToDouble(left), Convert.ToDouble(right));
        }

        public object Visit(LiteralNode node, SymbolTable<string, object> symbolTable)
        {
            return node.Data;
        }


        #endregion

        #region Statement Node Visit Methods

        // TODO

        public object Visit(AssignmentStmt node, SymbolTable<string, object> symbolTable)
        {
            symbolTable[node.Variable.Name] = node.Expression.Accept(this, symbolTable);

        }

        public object Visit(ReturnStmt node, SymbolTable<string, object> symbolTable)
        {
            _returnEncountered = true;
            _returnValue = node.Expression.Accept(this, symbolTable);
            return _returnValue;
        }

        public object Visit(BlockStmt node, SymbolTable<string, object> symbolTable)
        {
            // Use this block's symbol table, which is already linked to its parent
            SymbolTable<string, object> currentScope = node.SymbolTable;

            // TODO
            foreach(Statement s in node.Statements)
            {
                if (s is AssignmentStmt) currentScope.Add(s.Accept<SymbolTable<string, object>, object>(this, currentScope));
            }
        }

        #endregion
    }
}