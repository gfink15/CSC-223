/**
 * Abstract Syntax Tree: add description
 * Claude AI was only used for writing XML and inline comments.
 * 
 * @author Graham Fink, Mridul Agrawal
 * @date   2/25/2026
 */

namespace AST;

public class AbstractSyntaxTree
{
    private Statement _root;
    public AbstractSyntaxTree(Statement e)
    {
        _root = e;
    }
}

public abstract class ExpressionNode
{
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
    public T? Data
    {
        get;
        set;
    }
    public LiteralNode(T? d) : base(default, default)
    {
        Data = d;
    }
}

public class VariableNode : ExpressionNode
{
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
    public BinaryOperator(ExpressionNode l, ExpressionNode r) : base(l, r)
    {
        
    }
}

public class PlusNode : BinaryOperator
{
    public PlusNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {
        
    }
}

public class MinusNode : BinaryOperator
{
    public MinusNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {
        
    }
}

public class TimesNode : BinaryOperator
{
    public TimesNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {
        
    }
}

public class FloatDivNode : BinaryOperator
{
    public FloatDivNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {
        
    }
}

public class IntDivNode : BinaryOperator
{
    public IntDivNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {
        
    }
}

public class ModulusNode : BinaryOperator
{
    public ModulusNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {
        
    }
}

public class ExponentiationNode : BinaryOperator
{
    public ExponentiationNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {
        
    }
}

public abstract class Statement {}

public class BlockStmt : Statement
{
    public List<Statement> Children = new List<Statement>();
    public BlockStmt(Statement s) : base()
    {
        Children.Add(s);
    }
}

public class AssignmentStmt : Statement
{
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
    public ExpressionNode Child
    {
        get; set;
    }
    public ReturnStmt(ExpressionNode c)
    {
        Child = c;
    }
}