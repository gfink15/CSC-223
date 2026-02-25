/**
 * Abstract Syntax Tree: add description
 * Claude AI was only used for writing XML and inline comments.
 * 
 * @author Graham Fink, Mridul Agrawal
 * @date   2/24/2026
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

public abstract class ExpressionNode {}

public class LiteralNode<T> : ExpressionNode
{
    AbstractSyntaxTree a = new AbstractSyntaxTree(new AssignmentStmt());
    public T? Data
    {
        get;
        set;
    }
    public LiteralNode(T? d)
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
    public VariableNode(string n)
    {
        Name = n;
    }
}

public abstract class Operator : ExpressionNode {}

public abstract class BinaryOperator : Operator {}

public class PlusNode : BinaryOperator {}

public class MinusNode : BinaryOperator {}

public class TimesNode : BinaryOperator {}

public class FloatDivNode : BinaryOperator {}

public class IntDivNode : BinaryOperator {}

public class ModulusNode : BinaryOperator {}

public class ExponentiationNode : BinaryOperator {}

public abstract class Statement {}

public class BlockStmt : Statement {}

public class AssignmentStmt : Statement {}

public class ReturnStmt : Statement {}