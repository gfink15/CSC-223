using System.Linq.Expressions;
using System.Text.RegularExpressions;
using AST;
using Tokenizer;
using Utilities;
using Utilities.Containers;


/**
 * Parser: Converts a string of DEC source code into an
 * Abstract Syntax Tree (AST). Supports expressions with
 * binary operators, assignment and return statements,
 * and nested block scoping.
 *
 * Bugs: None known.
 *
 * @author Graham Fink, Mridul Agrawal
 * @date   3/11/2026
 */

namespace Parser;

public static class Parser
{
    private static DefaultBuilder _builder = new DefaultBuilder();

    /// <summary>
    /// Replaces the default AST builder with a custom one.
    /// </summary>
    /// <param name="b">The builder to use for AST creation.</param>
    public static void SetBuilder(DefaultBuilder b)
    {
        _builder = b;
    }

    #region Expressions — Parsing parenthesised expressions

    /// <summary>
    /// Parses a full parenthesised expression from the token list.
    /// Consumes the opening and closing parentheses.
    /// </summary>
    /// <param name="l">Token list; tokens are consumed in place.</param>
    /// <returns>The parsed ExpressionNode subtree.</returns>
    /// <exception cref="ParseException">
    /// Thrown if parentheses are missing or the expression is empty.
    /// </exception>
    private static AST.ExpressionNode ParseExpression(List<Token> l)
    {
        if (l.Count < 1) throw new ParseException("Missing Expression");
        if (l[0].Type != TokenType.LEFT_PAREN) throw new ParseException("Expression must begin with a (");

        // Remove opening paren and parse inner content
        l.RemoveAt(0);
        var parsed = ParseExpressionContent(l);

        if (l[0].Type != TokenType.RIGHT_PAREN) throw new ParseException("Expression must end with a )");

        // Remove closing paren
        l.RemoveAt(0);
        return parsed;
    }

    /// <summary>
    /// Parses the content between parentheses, building up
    /// sub-expressions, operators, and literals until a closing
    /// paren is encountered.
    /// </summary>
    /// <param name="l">Token list; tokens are consumed in place.</param>
    /// <returns>The parsed ExpressionNode subtree.</returns>
    /// <exception cref="ParseException">
    /// Thrown on counter mismatches or a missing closing paren.
    /// </exception>
    private static AST.ExpressionNode ParseExpressionContent(List<Tokenizer.Token> l)
    {
        List<ExpressionNode> expressions = new List<ExpressionNode>();
        int counter = 0;
        string op = "";

        while (l.Count > 0)
        {
            // Nested sub-expression
            if (l[0].Type == TokenType.LEFT_PAREN)
            {
                expressions.Add(ParseExpression(l));
                counter++;
            }
            // Binary operator between two operands
            else if (l[0].Type == TokenType.OPERATOR)
            {
                op = l[0].Value;
                l.RemoveAt(0);
                counter++;
            }
            // Closing paren — assemble and return the node
            else if (l[0].Type == TokenType.RIGHT_PAREN)
            {
                if (counter == 3 && expressions.Count == 2) return CreateBinaryOperatorNode(op, expressions[0], expressions[1]);
                else if (counter == 1 && expressions.Count == 1) return expressions[0];

                // Debug dump for unexpected state
                string s = "";
                foreach (ExpressionNode e in expressions)
                {
                    s += e.Unparse() + " ";
                }
                s += "Operator: " + op;
                throw new ParseException("Counter mismatch or incorrect; Counter: " + counter + ", Count: " + expressions.Count + " Data dump: " + s);
            }
            // Literal or variable token
            else
            {
                expressions.Add(HandleSingleToken(l[0]));
                l.RemoveAt(0);
                counter++;
            }
        }

        throw new ParseException("Missing )");
    }

    /// <summary>
    /// Converts a single token into the appropriate leaf node
    /// (variable, integer literal, or double literal).
    /// </summary>
    /// <param name="t">The token to convert.</param>
    /// <returns>A LiteralNode or VariableNode.</returns>
    /// <exception cref="ParseException">
    /// Thrown if the token type is unknown or unsupported.
    /// </exception>
    private static AST.ExpressionNode HandleSingleToken(Tokenizer.Token t)
    {
        if (t.Type == TokenType.UNKNOWN) throw new ParseException("Invalid operator / unknown single token");
        if (t.Type == TokenType.VARIABLE) return ParseVariableNode(t.Value);
        else if (t.Type == TokenType.DOUBLE) return _builder.CreateLiteralNode(Convert.ToDouble(t.Value));
        else if (t.Type == TokenType.INTEGER) return _builder.CreateLiteralNode(Convert.ToInt32(t.Value));
        else if (t.Type == TokenType.RIGHT_CURLY) throw new ParseException("must end with a )");
        throw new ParseException("Token type is not a variable or a number: " + t.Type + " " + t.Value);
    }

    /// <summary>
    /// Creates the correct BinaryOperator node for the given
    /// operator string.
    /// </summary>
    /// <param name="op">Operator symbol (e.g. "+", "//").</param>
    /// <param name="l">Left operand expression.</param>
    /// <param name="r">Right operand expression.</param>
    /// <returns>A BinaryOperator subclass node.</returns>
    /// <exception cref="ParseException">
    /// Thrown if the operator string is not recognised.
    /// </exception>
    private static AST.ExpressionNode CreateBinaryOperatorNode(string op, AST.ExpressionNode l, AST.ExpressionNode r)
    {
        if (!GeneralUtils.IsValidOperator(op)) throw new ParseException("Invalid Operator");

        // Match operator to the corresponding AST node
        switch (op)
        {
            case TokenConstants.PLUS:
                return _builder.CreatePlusNode(l, r);
            case TokenConstants.MINUS:
                return _builder.CreateMinusNode(l, r);
            case TokenConstants.MULTIPLY:
                return _builder.CreateTimesNode(l, r);
            case TokenConstants.DIVIDE:
                return _builder.CreateFloatDivNode(l, r);
            case TokenConstants.INTEGERDIVIDE:
                return _builder.CreateIntDivNode(l, r);
            case TokenConstants.EXPONENTIATE:
                return _builder.CreateExponentiationNode(l, r);
            case TokenConstants.MODULUS:
                return _builder.CreateModulusNode(l, r);
        }
        throw new ParseException("Invalid operator after check");
    }

    /// <summary>
    /// Validates and creates a VariableNode from a name string.
    /// </summary>
    /// <param name="s">The variable name.</param>
    /// <returns>A new VariableNode.</returns>
    /// <exception cref="ParseException">
    /// Thrown if the variable name is invalid.
    /// </exception>
    private static AST.VariableNode ParseVariableNode(string s)
    {
        if (!GeneralUtils.IsValidVariable(s)) throw new ParseException("Invalid Variable Name");
        return _builder.CreateVariableNode(s);
    }
    #endregion

    #region Individual Statements — Assignment, Return, dispatch

    /// <summary>
    /// Parses an assignment statement of the form: variable := (expr).
    /// Adds the variable to the symbol table.
    /// </summary>
    /// <param name="l">Token list; tokens are consumed in place.</param>
    /// <param name="s">Symbol table for the current scope.</param>
    /// <returns>An AssignmentStmt node.</returns>
    /// <exception cref="ParseException">
    /// Thrown if the variable name or assignment operator is missing.
    /// </exception>
    private static AST.AssignmentStmt ParseAssignmentStmt(List<Tokenizer.Token> l, SymbolTable<string, object> s)
    {
        // Validate variable and assignment tokens
        if (l[0].Type != TokenType.VARIABLE)
        {
            throw new ParseException("Invalid variable name.");
        }

        if (l[1].Type != TokenType.ASSIGNMENT)
        {
            throw new ParseException("Expected assignment operator.");
        }

        // Extract and consume variable name and := tokens
        string variable_name = l[0].Value;
        l.RemoveAt(0);
        l.RemoveAt(0);

        // Register variable in the current scope
        s[variable_name] = default;

        return _builder.CreateAssignmentStmt(ParseVariableNode(variable_name), ParseExpression(l));
    }

    /// <summary>
    /// Parses a return statement of the form: return (expr).
    /// </summary>
    /// <param name="l">Token list; tokens are consumed in place.</param>
    /// <returns>A ReturnStmt node.</returns>
    /// <exception cref="ParseException">
    /// Thrown if the first token is not the return keyword.
    /// </exception>
    private static AST.ReturnStmt ParseReturnStatement(List<Tokenizer.Token> l)
    {
        if (l[0].Type == TokenType.RETURN)
        {
            l.RemoveAt(0);
            return _builder.CreateReturnStmt(ParseExpression(l));
        }
        throw new ParseException("Return Statement does not begin with return");
    }

    /// <summary>
    /// Dispatches a single line's tokens to the correct
    /// statement parser (return or assignment).
    /// </summary>
    /// <param name="l">Token list for one statement line.</param>
    /// <param name="s">Symbol table for the current scope.</param>
    /// <returns>The parsed Statement node.</returns>
    /// <exception cref="ParseException">
    /// Thrown if the token list is empty or unrecognised.
    /// </exception>
    private static AST.Statement ParseStatement(List<Tokenizer.Token> l, SymbolTable<string, object> s)
    {
        if (l.Count < 1) throw new ParseException("Missing Statement");
        else if (l[0].Type == TokenType.RETURN) return ParseReturnStatement(l);
        else if (l.Count >= 2 && l[0].Type == TokenType.VARIABLE && l[1].Type == TokenType.ASSIGNMENT)
        {
            return ParseAssignmentStmt(l, s);
        }
        else throw new ParseException("Invalid Token. Expected 1 token.");
    }
    #endregion

    #region Blocks — Block statements and statement list parsing

    /// <summary>
    /// Iterates through source lines, parsing each as a statement
    /// or nested block until a closing brace is encountered.
    /// Parsed statements are added to the given BlockStmt.
    /// </summary>
    /// <param name="lines">
    /// Source lines; consumed in place as they are parsed.
    /// </param>
    /// <param name="b">
    /// The BlockStmt to accumulate statements into.
    /// </param>
    private static void ParseStmtList(List<string> lines, BlockStmt b)
    {

        while (lines.Count > 0)
        {
            string line = lines[0].Trim();

            // Skip blank lines
            if (line.Length == 0)
            {
                lines.RemoveAt(0);
                continue;
            }

            // Check for nested block opening brace
            int indexLeft = line.IndexOf(TokenConstants.LEFT_CURLY);

            if (indexLeft >= 0)
            {
                string firstPart = line[..indexLeft].Trim();
                string secondPart = line[indexLeft..];

                // Split content before { onto its own line
                if (firstPart.Length > 0)
                {
                    lines[0] = firstPart;
                    lines.Insert(1, secondPart);
                }
                else
                {
                    lines[0] = secondPart;
                }

                b.Add(ParseBlockStmt(lines, b.SymbolTable));

                continue;

            }

            // Stop at closing brace — let ParseBlockStmt handle it
            int indexRight = line.IndexOf(TokenConstants.RIGHT_CURLY);

            if (indexRight >= 0)
            {
                // Parse any statement text before the brace
                string lastLine = line[..(indexRight)].Trim();
                lines[0] = line[indexRight..];
                if (lastLine.Length > 0)
                {
                    b.Add(ParseStatement(TokenizerImpl.Tokenize(lastLine), b.SymbolTable));
                }

                return;

            }
            else
            {
                // Regular statement line
                b.Add(ParseStatement(TokenizerImpl.Tokenize(line), b.SymbolTable));
                lines.RemoveAt(0);
            }
        }
        throw new ParseException("missing a }");
    }

    /// <summary>
    /// Parses a block statement enclosed in curly braces.
    /// Creates a new child scope and delegates to ParseStmtList
    /// for the block body.
    /// </summary>
    /// <param name="lines">
    /// Source lines; consumed in place as they are parsed.
    /// </param>
    /// <param name="s">Parent symbol table for scope chaining.</param>
    /// <returns>A BlockStmt with its own SymbolTable.</returns>
    /// <exception cref="ParseException">
    /// Thrown if braces are missing or extra tokens follow }.
    /// </exception>
    private static AST.BlockStmt ParseBlockStmt(List<string> lines, SymbolTable<string, object> s)
    {
        if (lines.Count < 1) throw new ParseException("Missing Block Statement");

        // Consume opening brace
        if (lines[0][0].ToString() != TokenConstants.LEFT_CURLY) throw new ParseException("Block Statement must begin with {");
        lines[0] = lines[0][1..];
        if (lines[0].Trim().Length == 0) lines.RemoveAt(0);

        // Parse the body into a new scoped block
        AST.BlockStmt b = _builder.CreateBlockStmt(new SymbolTable<string, object>(s));
        ParseStmtList(lines, b);

        // Consume closing brace
        if (lines[0][0].ToString() != TokenConstants.RIGHT_CURLY) throw new ParseException("Block Statement must end with }");
        lines[0] = lines[0][1..];
        if (lines[0].Trim().Length == 0) lines.RemoveAt(0);
        else throw new ParseException("Extra tokens on last line");

        return b;
    }
    #endregion

    #region Public API — Entry point for parsing a full program

    /// <summary>
    /// Parses a complete DEC program string into a BlockStmt AST.
    /// The program must be enclosed in curly braces.
    /// </summary>
    /// <param name="program">
    /// The full program source code as a single string.
    /// </param>
    /// <returns>The root BlockStmt of the parsed AST.</returns>
    /// <exception cref="ParseException">
    /// Thrown if the program is empty or missing outer braces.
    /// </exception>
    public static AST.BlockStmt Parse(string program)
    {
        if (program.Length == 0) throw new ParseException("Empty program");

        // Validate outer braces
        if (program[0].ToString() != TokenConstants.LEFT_CURLY) throw new ParseException($"Program must start with {TokenConstants.LEFT_CURLY}");
        if (program[^1].ToString() != TokenConstants.RIGHT_CURLY) throw new ParseException($"Program must end with {TokenConstants.RIGHT_CURLY}");

        // Split into lines and delegate to block parser
        List<string> lines = program.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();

        return ParseBlockStmt(lines, new SymbolTable<string, object>());

    }
    #endregion
}

/// <summary>
/// Custom exception for parser errors, providing descriptive
/// messages about syntax or structural issues in DEC source code.
/// </summary>
public class ParseException : Exception
{
    public ParseException(string s) : base(s) { }
}