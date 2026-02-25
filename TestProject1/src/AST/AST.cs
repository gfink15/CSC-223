/**
 * Abstract Syntax Tree: add description
 * Claude AI was only used for writing XML and inline comments.
 * 
 * @author Graham Fink, Mridul Agrawal
 * @date   2/25/2026
 */
using Tokenizer;
using Utilities;
using Utilities.Containers;

namespace AST;

public class AbstractSyntaxTree
{
    public Statement Root
    {
        get; set;
    }
    public AbstractSyntaxTree(Statement e)
    {
        Root = e;
    }
}

public abstract class ExpressionNode
{
    public abstract string Unparse(int level = 0);
    public ExpressionNode? Left
    {
        get; set;
    }
    public ExpressionNode? Right
    {
        get; set;
    }
    public ExpressionNode(ExpressionNode? l, ExpressionNode? r)
    {
        Left = l;
        Right = r;
    }
}

public class LiteralNode<T> : ExpressionNode
{
    public override string Unparse(int level = 0)
    {
        return Data.ToString();
    }
    public T Data
    {
        get;
        set;
    }
    public LiteralNode(T d) : base(default, default)
    {
        Data = d;
    }
}

public class VariableNode : ExpressionNode
{
    public override string Unparse(int level = 0)
    {
        return Name;
    }
    public string Name
    {
        get;
        set;
    }
    public VariableNode(string n) : base(default, default)
    {
        Name = n;
    }
}

public abstract class Operator : ExpressionNode
{
    
    public Operator(ExpressionNode l, ExpressionNode r) : base(l, r)
    {
        
    }
}

public abstract class BinaryOperator : Operator
{
    public override string Unparse(int level = 0)
    {
        return "(" + Left.Unparse(level) + ToString() + Right.Unparse(level) + ")";
    }
    public BinaryOperator(ExpressionNode l, ExpressionNode r) : base(l, r)
    {
        
    }
}

public class PlusNode : BinaryOperator
{
    public override string ToString()
    {
        return TokenConstants.PLUS;
    }
    public PlusNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {
        
    }
}

public class MinusNode : BinaryOperator
{
    public override string ToString()
    {
        return TokenConstants.MINUS;
    }
    public MinusNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {
        
    }
}

public class TimesNode : BinaryOperator
{
    public override string ToString()
    {
        return TokenConstants.MULTIPLY;
    }
    public TimesNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {
        
    }
}

public class FloatDivNode : BinaryOperator
{
    public override string ToString()
    {
        return TokenConstants.DIVIDE;
    }
    public FloatDivNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {
        
    }
}

public class IntDivNode : BinaryOperator
{
    public override string ToString()
    {
        return TokenConstants.INTEGERDIVIDE;
    }
    public IntDivNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {
        
    }
}

public class ModulusNode : BinaryOperator
{
    public override string ToString()
    {
        return TokenConstants.MODULUS;
    }
    public ModulusNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {
        
    }
}

public class ExponentiationNode : BinaryOperator
{
    public override string ToString()
    {
        return TokenConstants.EXPONENTIATE;
    }
    public ExponentiationNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {
        
    }
}

public abstract class Statement
{
    public abstract string Unparse(int level = 0);
}

public class BlockStmt : Statement
{

    public override string Unparse(int level = 0)
    {
        string builder = GeneralUtils.GetIndentation(level) + "{";
        foreach (Statement s in children)
        {
            builder += "\n" + s.Unparse(level + 1);
        }
        builder += "\n" + GeneralUtils.GetIndentation(level) + "}";
        return builder;
    }

    public List<Statement> children;
    public BlockStmt(SymbolTable<string, object> symbolTable) : base()
    {
        children = new List<Statement>();
    }
}

public class AssignmentStmt : Statement
{
    public override string ToString()
    {
        return TokenConstants.ASSIGNMENT;
    }

    public override string Unparse(int level = 0)
    {
        string builder = GeneralUtils.GetIndentation(level);
        builder += Right.Unparse(0) + ToString() + Left.Unparse();
        return builder;
    }

    public ExpressionNode Left
    {
        get; set;
    }
    public ExpressionNode Right
    {
        get; set;
    }
    public AssignmentStmt(ExpressionNode l, ExpressionNode r)
    {
        Left = l;
        Right = r;
    }
}

public class ReturnStmt : Statement
{
    public override string ToString()
    {
        return TokenConstants.RETURN;
    }

    public override string Unparse(int level = 0)
    {
        return GeneralUtils.GetIndentation(level) + ToString() + Child.Unparse(0);
    }

    public ExpressionNode Child
    {
        get; set;
    }
    public ReturnStmt(ExpressionNode c)
    {
        Child = c;
    }
}