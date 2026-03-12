

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

class Parser
{
    #region Expressions
    public AST.ExpressionNode ParseExpression(List<Token> l)
    {
        throw new NotImplementedException();
    }
    public AST.ExpressionNode ParseExpressionContent(List<Tokenizer.Token> l)
    {
        throw new NotImplementedException();
    }
    public AST.ExpressionNode HandleSingleToken(Tokenizer.Token t)
    {
        throw new NotImplementedException();
    }
    public AST.ExpressionNode CreateBinaryOperatorNode(string op, AST.ExpressionNode l, AST.ExpressionNode r)
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
    public AST.VariableNode ParseVariableNode(string s)
    {
        if (!GeneralUtils.IsValidVariable(s)) throw new ParseException("Invalid Variable Name");
        return new AST.VariableNode(s);
    }
    #endregion

    #region Individual Statements
    public AST.AssignmentStmt ParseAssignemntStmt(List<Tokenizer.Token> l, SymbolTable<Object, Object> s)
    {
        throw new NotImplementedException();
    }
    public AST.ReturnStmt ParseReturnStmt(List<Tokenizer.Token> l)
    {
        throw new NotImplementedException();
    }
    public AST.Statement ParseStatement(List<Tokenizer.Token> l, SymbolTable<Object, Object> s)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Blocks
    public void ParseStmtList(List<string> lines, BlockStmt b)
    {
        throw new NotImplementedException();
    }
    public AST.BlockStmt ParseBlockStmt(List<string> lines, SymbolTable<Object, Object> s)
    {
        throw new NotImplementedException();
    }
    #endregion

    public AST.AbstractSyntaxTree Parse(string s)
    {
        throw new NotImplementedException();
    }
}
public class ParseException : Exception
{
    public ParseException(string s) : base(s) {}
}