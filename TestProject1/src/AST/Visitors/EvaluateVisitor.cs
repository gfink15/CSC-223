using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using AST;
using Utilities.Containers;

namespace AST
{
    /// <summary>
    /// Exception thrown when an evaluation error occurs
    /// </summary>
    public class EvaluationException(string message) : Exception(message) { }

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
            // Start each evaluator instance in a non-returning state.
            _returnEncountered = false;
            // No return value exists until a return statement is evaluated.
            _returnValue = null;
        }

        /// <summary>
        /// Evaluates the given AST and returns the result
        /// </summary>
        /// <param name="ast">The AST to evaluate</param>
        /// <returns>The result of the evaluation (typically from a return statement)</returns>
        public object Evaluate(Statement ast)
        {
            // Reset visitor state so multiple Evaluate calls are independent.
            _returnEncountered = false;
            // Clear any stale return value from previous evaluations.
            _returnValue = null;

            // Execute the AST with a null initial scope
            // (the BlockStmt will use its own symbol table)
            //ast.Accept(this, null);

            // Delegate execution to the root statement node.
            return ast.Accept(this, null);
        }

        // TODO

        #region Expression Node Visit Methods

        // TODO

        /// <summary>
        /// Resolves a variable reference by reading its value from the current symbol table chain.
        /// </summary>
        /// <param name="node">The variable expression node to evaluate.</param>
        /// <param name="symbolTable">The active scope used for identifier lookup.</param>
        /// <returns>The runtime value bound to the variable name.</returns>
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

        /// <summary>
        /// Evaluates an addition expression by evaluating both operands and summing them as doubles.
        /// </summary>
        /// <param name="node">The addition expression node.</param>
        /// <param name="symbolTable">The active scope for resolving operand variables.</param>
        /// <returns>The numeric sum of the left and right operand values.</returns>
        public object Visit(PlusNode node, SymbolTable<string, object> symbolTable)
        {
            // Evaluate child expressions first to obtain runtime operand values.
            object left = node.Left.Accept(this, symbolTable);
            object right = node.Right.Accept(this, symbolTable);
            // Perform arithmetic using double conversion for numeric compatibility.
            return Convert.ToDouble(left) + Convert.ToDouble(right);
        }

        /// <summary>
        /// Evaluates a subtraction expression by evaluating both operands and subtracting as doubles.
        /// </summary>
        /// <param name="node">The subtraction expression node.</param>
        /// <param name="symbolTable">The active scope for resolving operand variables.</param>
        /// <returns>The numeric difference of the left and right operand values.</returns>
        public object Visit(MinusNode node, SymbolTable<string, object> symbolTable)
        {
            // Recursively evaluate the left and right subexpressions.
            object left = node.Left.Accept(this, symbolTable);
            object right = node.Right.Accept(this, symbolTable);
            // Subtract the right operand from the left operand.
            return Convert.ToDouble(left) - Convert.ToDouble(right);
        }

        /// <summary>
        /// Evaluates a multiplication expression by evaluating both operands and multiplying as doubles.
        /// </summary>
        /// <param name="node">The multiplication expression node.</param>
        /// <param name="symbolTable">The active scope for resolving operand variables.</param>
        /// <returns>The numeric product of the left and right operand values.</returns>
        public object Visit(TimesNode node, SymbolTable<string, object> symbolTable)
        {
            // Evaluate each operand expression in the current scope.
            object left = node.Left.Accept(this, symbolTable);
            object right = node.Right.Accept(this, symbolTable);
            // Multiply using double precision to match evaluator numeric conventions.
            return Convert.ToDouble(left) * Convert.ToDouble(right);
        }

        /// <summary>
        /// Evaluates a floating-point division expression with divide-by-zero checking.
        /// </summary>
        /// <param name="node">The floating-point division expression node.</param>
        /// <param name="symbolTable">The active scope for resolving operand variables.</param>
        /// <returns>The floating-point quotient of the left and right operand values.</returns>
        /// <exception cref="EvaluationException">Thrown when the divisor evaluates to zero.</exception>
        public object Visit(FloatDivNode node, SymbolTable<string, object> symbolTable)
        {
            // Evaluate numerator and denominator before applying the operator.
            object left = node.Left.Accept(this, symbolTable);
            object right = node.Right.Accept(this, symbolTable);
            // Guard against invalid division by zero.
            if (Convert.ToInt32(right) == 0) throw new EvaluationException("Can\'t FloatDiv by 0");
            // Return a floating-point division result.
            return Convert.ToDouble(left) / Convert.ToDouble(right);
        }

        /// <summary>
        /// Evaluates an integer division expression with divide-by-zero checking.
        /// </summary>
        /// <param name="node">The integer division expression node.</param>
        /// <param name="symbolTable">The active scope for resolving operand variables.</param>
        /// <returns>The integer quotient of the left and right operand values.</returns>
        /// <exception cref="EvaluationException">Thrown when the divisor evaluates to zero.</exception>
        public object Visit(IntDivNode node, SymbolTable<string, object> symbolTable)
        {
            // Evaluate both operands in the current execution scope.
            object left = node.Left.Accept(this, symbolTable);
            object right = node.Right.Accept(this, symbolTable);
            // Reject integer division by zero before attempting conversion and division.
            if (Convert.ToInt32(right) == 0) throw new EvaluationException("Can\'t IntDiv by 0");
            // Perform integer division semantics.
            return Convert.ToInt32(left) / Convert.ToInt32(right);
        }

        /// <summary>
        /// Evaluates a modulus expression with divide-by-zero checking.
        /// </summary>
        /// <param name="node">The modulus expression node.</param>
        /// <param name="symbolTable">The active scope for resolving operand variables.</param>
        /// <returns>The remainder from dividing the left operand by the right operand.</returns>
        /// <exception cref="EvaluationException">Thrown when the divisor evaluates to zero.</exception>
        public object Visit(ModulusNode node, SymbolTable<string, object> symbolTable)
        {
            // Evaluate dividend and divisor expressions first.
            object left = node.Left.Accept(this, symbolTable);
            object right = node.Right.Accept(this, symbolTable);
            // Modulus by zero is undefined and must be rejected.
            if (Convert.ToInt32(right) == 0) throw new EvaluationException("Can\'t Mod by 0");
            // Return the numeric remainder.
            return Convert.ToDouble(left) % Convert.ToDouble(right);
        }

        /// <summary>
        /// Evaluates an exponentiation expression by raising the left operand to the right operand power.
        /// </summary>
        /// <param name="node">The exponentiation expression node.</param>
        /// <param name="symbolTable">The active scope for resolving operand variables.</param>
        /// <returns>The computed power value.</returns>
        public object Visit(ExponentiationNode node, SymbolTable<string, object> symbolTable)
        {
            // Evaluate base and exponent expressions recursively.
            object left = node.Left.Accept(this, symbolTable);
            object right = node.Right.Accept(this, symbolTable);
            // Use Math.Pow for exponentiation behavior.
            return Math.Pow(Convert.ToDouble(left), Convert.ToDouble(right));
        }

        /// <summary>
        /// Evaluates a literal expression by returning its stored value directly.
        /// </summary>
        /// <param name="node">The literal node containing the constant value.</param>
        /// <param name="symbolTable">The active scope (unused for literals).</param>
        /// <returns>The literal's underlying data value.</returns>
        public object Visit(LiteralNode node, SymbolTable<string, object> symbolTable)
        {
            // Literals evaluate to themselves and do not consult scope.
            return node.Data;
        }


        #endregion

        #region Statement Node Visit Methods

        // TODO

        /// <summary>
        /// Evaluates an assignment statement and stores the resulting value in the current scope.
        /// </summary>
        /// <param name="node">The assignment statement node.</param>
        /// <param name="symbolTable">The active scope where the variable binding is written.</param>
        /// <returns>Always <c>null</c> because statements do not produce direct expression values.</returns>
        public object Visit(AssignmentStmt node, SymbolTable<string, object> symbolTable)
        {
            // Compute the right-hand side value first.
            _returnValue = node.Expression.Accept(this, symbolTable);
            // Store the computed value under the target variable name.
            symbolTable[node.Variable.Name] = _returnValue;
            // Statement visitors conventionally return null.
            return null;
        }

        /// <summary>
        /// Evaluates a return statement and records that execution should stop at the current block level.
        /// </summary>
        /// <param name="node">The return statement node.</param>
        /// <param name="symbolTable">The active scope used to evaluate the return expression.</param>
        /// <returns>Always <c>null</c> because control-flow effects are tracked via visitor state.</returns>
        public object Visit(ReturnStmt node, SymbolTable<string, object> symbolTable)
        {
            // Signal to enclosing block execution that a return has occurred.
            _returnEncountered = true;
            // Capture the expression value as the function/program result.
            _returnValue = node.Expression.Accept(this, symbolTable);
            // Statement visitors conventionally return null.
            return null;
        }

        /// <summary>
        /// Evaluates each statement in a block scope and returns the final normalized return value.
        /// </summary>
        /// <param name="node">The block statement containing child statements.</param>
        /// <param name="symbolTable">The parent scope (unused because the block owns its scope object).</param>
        /// <returns>The normalized numeric return value captured during block execution.</returns>
        public object Visit(BlockStmt node, SymbolTable<string, object> symbolTable)
        {
            // Use this block's symbol table, which is already linked to its parent
            SymbolTable<string, object> currentScope = node.SymbolTable;
            // TODO
            foreach (Statement s in node.Statements)
            {
                // Execute statements in order using the block-local scope.
                s.Accept(this, currentScope);
                // Stop early once a return statement has been evaluated.
                if (_returnEncountered) break;
            }
            // Convert return value to double for numeric normalization checks.
            double _returnValueNumber = Convert.ToDouble(_returnValue);
            // If the value is mathematically integral, return it as an int.
            if (Math.Floor(_returnValueNumber) == Convert.ToDouble(_returnValue)) return Convert.ToInt32(_returnValue);
            // Otherwise keep and return the double representation.
            return _returnValueNumber;
        }

        #endregion
    }
}