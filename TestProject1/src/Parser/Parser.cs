

using System.Linq.Expressions;
using System.Text.RegularExpressions;
using AST;
using Tokenizer;
using Utilities;
using Utilities.Containers;


/**
 * Parser: Takes in a string of DEC code and outputs an AST
 * Claude AI was only used for writing XML and inline comments.
 * 
 * @author Graham Fink, Mridul Agrawal
 * @date   3/11/2026
 */
namespace Parser;

public static class Parser
{
    #region Expressions
    public static AST.ExpressionNode ParseExpression(List<Token> l)
    {
        if (l[0].Type != TokenType.LEFT_PAREN) throw new ParseException("Expression does not begin with parentheses");
        l.RemoveAt(0);
        var parsed = ParseExpressionContent(l);
        if (l[0].Type != TokenType.RIGHT_PAREN) throw new ParseException("Expression does not begin with parentheses");
        return parsed;
    }
    public static AST.ExpressionNode ParseExpressionContent(List<Tokenizer.Token> l)
    {
        List<ExpressionNode> expressions = new List<ExpressionNode>();
        try {
            while (l.Count > 1)
            {
                if (l[0].Type == TokenType.LEFT_PAREN)
                {
                    expressions.Add(ParseExpression(l));
                }
                if (l[0].Type == TokenType.OPERATOR)
                {
                    string s = l[0].Value;
                    l.RemoveAt(0);
                    expressions.Add(CreateBinaryOperatorNode(s, expressions[0], ParseExpressionContent(l)));
                }
                else
                {
                    expressions.Add(HandleSingleToken(l[0]));
                    l.RemoveAt(0);
                }
            }
        } catch {
           throw new ParseException("Error while parsing expression"); 
        }
        
        //if (l[2].Type == TokenType.LEFT_PAREN) return CreateBinaryOperatorNode(l[1].Value, l[0], ParseExpression());
        
        throw new NotImplementedException();
    }
    public static AST.ExpressionNode HandleSingleToken(Tokenizer.Token t)
    {
        if (t.Type == TokenType.VARIABLE) return ParseVariableNode(t.Value);
        else if (t.Type == TokenType.DOUBLE || t.Type == TokenType.INTEGER) return new LiteralNode(t.Value);
        throw new ParseException("Token type is not a variable or a number");
    }
    public static AST.ExpressionNode CreateBinaryOperatorNode(string op, AST.ExpressionNode l, AST.ExpressionNode r)
    {
        if (!GeneralUtils.IsValidOperator(op)) throw new ParseException("Invalid Operator");
        switch (op)
        {
            case TokenConstants.PLUS:
                return new PlusNode(l, r);
            case TokenConstants.MINUS:
                return new MinusNode(l, r);
            case TokenConstants.MULTIPLY:
                return new TimesNode(l, r);
            case TokenConstants.DIVIDE:
                return new FloatDivNode(l, r);
            case TokenConstants.INTEGERDIVIDE:
                return new IntDivNode(l, r);
            case TokenConstants.EXPONENTIATE:
                return new ExponentiationNode(l, r);
            case TokenConstants.MODULUS:
                return new ModulusNode(l, r);
        }
        throw new ParseException("Invalid operator after check");
    }
    public static AST.VariableNode ParseVariableNode(string s)
    {
        if (!GeneralUtils.IsValidVariable(s)) throw new ParseException("Invalid Variable Name");
        return new AST.VariableNode(s);
    }
    #endregion

    #region Individual Statements
    public static AST.AssignmentStmt ParseAssignemntStmt(List<Tokenizer.Token> l, SymbolTable<Object, Object> s)
    {
        throw new NotImplementedException();
    }
    public static AST.ReturnStmt ParseReturnStmt(List<Tokenizer.Token> l)
    {
        throw new NotImplementedException();
    }
    public static AST.Statement ParseStatement(List<Tokenizer.Token> l, SymbolTable<Object, Object> s)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Blocks
    public static void ParseStmtList(List<string> lines, BlockStmt b)
    {
        throw new NotImplementedException();
    }
    public static AST.BlockStmt ParseBlockStmt(List<string> lines, SymbolTable<Object, Object> s)
    {
        throw new NotImplementedException();
    }
    #endregion

    public static AST.BlockStmt Parse(string s)
    {
        throw new NotImplementedException();
    }
}
public class ParseException : Exception
{
    public ParseException(string s) : base(s) {}
}