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

/// <summary>
/// The top-level container for a parsed DEC program. Holds a reference to the
/// root statement of the tree, which is typically a BlockStmt representing the
/// entire program body.
/// </summary>
public class AbstractSyntaxTree
{
    /// <summary>The root statement of the tree. All other nodes are descendants of this.</summary>
    public Statement Root
    {
        get; set;
    }

    /// <summary>
    /// Constructs an AbstractSyntaxTree with the given statement as its root.
    /// </summary>
    /// <param name="e">The root statement, usually a BlockStmt.</param>
    public AbstractSyntaxTree(Statement e)
    {
        Root = e;
    }
}

/// <summary>
/// Abstract base class for all expression nodes in the AST. An expression is any
/// construct that produces a value, such as a literal, variable, or binary operation.
/// Every ExpressionNode may optionally have a left and right child, supporting the
/// binary tree structure used by operator nodes.
/// </summary>
public abstract class ExpressionNode
{
    /// <summary>
    /// Converts this node and all its descendants back into readable DEC source code.
    /// </summary>
    /// <param name="level">The indentation depth (unused for most expression nodes).</param>
    /// <returns>A string representation of this expression.</returns>
    public abstract string Unparse(int level = 0);

    /// <summary>The left child expression (e.g. the left operand of a binary operator).</summary>
    public ExpressionNode? Left
    {
        get; set;
    }

    /// <summary>The right child expression (e.g. the right operand of a binary operator).</summary>
    public ExpressionNode? Right
    {
        get; set;
    }

    /// <summary>
    /// Constructs an ExpressionNode with the given left and right children.
    /// Leaf nodes (LiteralNode, VariableNode) pass default(null) for both.
    /// </summary>
    /// <param name="l">The left child, or null if this is a leaf node.</param>
    /// <param name="r">The right child, or null if this is a leaf node.</param>
    public ExpressionNode(ExpressionNode? l, ExpressionNode? r)
    {
        Left = l;
        Right = r;
    }
}

/// <summary>
/// Represents a literal value in the AST, such as the integer 42 or the
/// floating-point number 3.14. LiteralNode uses object as its data type so
/// that it can hold any primitive value without requiring separate subclasses.
/// </summary>
public class LiteralNode : ExpressionNode
{
    /// <summary>
    /// Unparses the literal by converting its stored value to a string.
    /// Indentation level is ignored since literals are always inline.
    /// </summary>
    public override string Unparse(int level = 0)
    {
        // ToString() on the stored object produces the source-code representation,
        // e.g. 42 -> "42", 3.14 -> "3.14".
        return Data.ToString();
    }

    /// <summary>The raw value stored in this literal node (e.g. an int, double, or string).</summary>
    public object Data
    {
        get;
        set;
    }

    /// <summary>
    /// Constructs a LiteralNode holding the given value.
    /// Passes null for both children since a literal is always a leaf in the tree.
    /// </summary>
    /// <param name="d">The literal value to store.</param>
    public LiteralNode(object d) : base(default, default)
    {
        Data = d;
    }
}

/// <summary>
/// Represents a variable reference in the AST, identified by its name.
/// When unparsed, a VariableNode simply emits the variable's name as it
/// appeared in the source code.
/// </summary>
public class VariableNode : ExpressionNode
{
    /// <summary>
    /// Unparses the variable by returning its name directly.
    /// Indentation level is ignored since variables are always inline.
    /// </summary>
    public override string Unparse(int level = 0)
    {
        return Name;
    }

    /// <summary>The identifier name of this variable as it appears in source code.</summary>
    public string Name
    {
        get;
        set;
    }

    /// <summary>
    /// Constructs a VariableNode with the given identifier name.
    /// Passes null for both children since a variable reference is always a leaf.
    /// </summary>
    /// <param name="n">The variable name.</param>
    public VariableNode(string n) : base(default, default)
    {
        Name = n;
    }
}

/// <summary>
/// Abstract base class for all operator nodes. Operators always have exactly
/// two children (left and right operands), inherited from ExpressionNode.
/// This class sits between ExpressionNode and BinaryOperator in the hierarchy,
/// leaving room for potential future unary operator subclasses.
/// </summary>
public abstract class Operator : ExpressionNode
{
    /// <summary>
    /// Constructs an Operator node with the given left and right operands,
    /// forwarding them to ExpressionNode's constructor.
    /// </summary>
    /// <param name="l">The left operand expression.</param>
    /// <param name="r">The right operand expression.</param>
    public Operator(ExpressionNode l, ExpressionNode r) : base(l, r)
    {

    }
}

/// <summary>
/// Abstract base class for all binary operator nodes (e.g. PlusNode, TimesNode).
/// Provides a shared Unparse implementation that wraps the operation in parentheses
/// and places the operator symbol between the unparsed left and right children.
/// Concrete subclasses only need to override ToString() to supply their operator symbol.
/// </summary>
public abstract class BinaryOperator : Operator
{
    /// <summary>
    /// Unparses the binary operation as a fully parenthesised expression,
    /// e.g. (2 + 3) or (x * 4). Parentheses are always emitted to make the
    /// order of operations explicit and unambiguous in the output.
    /// </summary>
    public override string Unparse(int level = 0)
    {
        // Recursively unparse the left and right children, then wrap the result
        // in parentheses with the operator symbol (from ToString()) in between.
        return "(" + Left.Unparse(level) + " " + ToString() + " " + Right.Unparse(level) + ")";
    }

    /// <summary>
    /// Constructs a BinaryOperator with the given left and right operands,
    /// forwarding them up to Operator and then ExpressionNode.
    /// </summary>
    /// <param name="l">The left operand expression.</param>
    /// <param name="r">The right operand expression.</param>
    public BinaryOperator(ExpressionNode l, ExpressionNode r) : base(l, r)
    {

    }
}

/// <summary>Represents the addition operator (+) in a binary expression.</summary>
public class PlusNode : BinaryOperator
{
    /// <summary>Returns the DEC addition operator symbol used during unparsing.</summary>
    public override string ToString()
    {
        return TokenConstants.PLUS;
    }

    public PlusNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {

    }
}

/// <summary>Represents the subtraction operator (-) in a binary expression.</summary>
public class MinusNode : BinaryOperator
{
    /// <summary>Returns the DEC subtraction operator symbol used during unparsing.</summary>
    public override string ToString()
    {
        return TokenConstants.MINUS;
    }

    public MinusNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {

    }
}

/// <summary>Represents the multiplication operator (*) in a binary expression.</summary>
public class TimesNode : BinaryOperator
{
    /// <summary>Returns the DEC multiplication operator symbol used during unparsing.</summary>
    public override string ToString()
    {
        return TokenConstants.MULTIPLY;
    }

    public TimesNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {

    }
}

/// <summary>Represents the floating-point division operator (/) in a binary expression.</summary>
public class FloatDivNode : BinaryOperator
{
    /// <summary>Returns the DEC float division operator symbol used during unparsing.</summary>
    public override string ToString()
    {
        return TokenConstants.DIVIDE;
    }

    public FloatDivNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {

    }
}

/// <summary>Represents the integer division operator (//) in a binary expression.</summary>
public class IntDivNode : BinaryOperator
{
    /// <summary>Returns the DEC integer division operator symbol used during unparsing.</summary>
    public override string ToString()
    {
        return TokenConstants.INTEGERDIVIDE;
    }

    public IntDivNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {

    }
}

/// <summary>Represents the modulus operator (%) in a binary expression.</summary>
public class ModulusNode : BinaryOperator
{
    /// <summary>Returns the DEC modulus operator symbol used during unparsing.</summary>
    public override string ToString()
    {
        return TokenConstants.MODULUS;
    }

    public ModulusNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {

    }
}

/// <summary>Represents the exponentiation operator (**) in a binary expression.</summary>
public class ExponentiationNode : BinaryOperator
{
    /// <summary>Returns the DEC exponentiation operator symbol used during unparsing.</summary>
    public override string ToString()
    {
        return TokenConstants.EXPONENTIATE;
    }

    public ExponentiationNode(ExpressionNode l, ExpressionNode r) : base(l, r)
    {

    }
}

/// <summary>
/// Abstract base class for all statement nodes in the AST. Unlike expressions,
/// statements perform actions (assignments, returns, blocks) and do not directly
/// produce a value. Every concrete statement must implement Unparse.
/// </summary>
public abstract class Statement
{
    /// <summary>
    /// Converts this statement and all its descendants back into readable DEC source code.
    /// </summary>
    /// <param name="level">The current indentation depth, used for pretty-printing.</param>
    /// <returns>A string representation of this statement.</returns>
    public abstract string Unparse(int level = 0);
}

/// <summary>
/// Represents a block of statements enclosed in curly braces, e.g. { ... }.
/// A BlockStmt owns a SymbolTable that maps variable names to their values
/// for the scope defined by this block.
/// </summary>
public class BlockStmt : Statement
{
    /// <summary>
    /// Unparses the block by emitting an opening brace, then each child statement
    /// on its own indented line, then a closing brace at the original indentation.
    /// Each child is unparsed at level + 1 to produce one extra level of indentation.
    /// </summary>
    public override string Unparse(int level = 0)
    {
        // Start with the opening brace, indented to the current level.
        string builder = GeneralUtils.GetIndentation(level) + "{";

        // Recursively unparse each child statement at one deeper indentation level.
        foreach (Statement s in Statements)
        {
            builder += "\n" + s.Unparse(level + 1);
        }

        // Close the block with a brace back at the original indentation level.
        builder += "\n" + GeneralUtils.GetIndentation(level) + "}";
        return builder;
    }

    /// <summary>The ordered list of statements contained within this block.</summary>
    public List<Statement> Statements;

    /// <summary>
    /// The symbol table for this block's scope, mapping variable names to their
    /// runtime values. Populated during interpretation or semantic analysis.
    /// </summary>
    public SymbolTable<string, object> SymbolTable
    {
        get; set;
    }

    /// <summary>
    /// Constructs an empty BlockStmt with the given symbol table.
    /// The children list starts empty; statements are added via Add().
    /// </summary>
    /// <param name="symbolTable">The symbol table representing this block's scope.</param>
    public BlockStmt(SymbolTable<string, object> symbolTable) : base()
    {
        Statements = new List<Statement>();
        SymbolTable = symbolTable;
    }

    /// <summary>
    /// Appends a statement to the end of this block's child list.
    /// Statements are unparsed in the order they were added.
    /// </summary>
    /// <param name="s">The statement to add.</param>
    public void Add(Statement s)
    {
        Statements.Add(s);
    }
}

/// <summary>
/// Represents a variable assignment statement, e.g. x := (2 + 3).
/// </summary>
public class AssignmentStmt : Statement
{
    /// <summary>Returns the DEC assignment operator symbol used during unparsing.</summary>
    public override string ToString()
    {
        return TokenConstants.ASSIGNMENT;
    }

    /// <summary>
    /// Unparses the assignment as: indentation + variable + := + expression,
    /// matching the order in which an assignment appears in DEC source code.
    /// </summary>
    public override string Unparse(int level = 0)
    {
        // Apply indentation for this statement's nesting depth.
        string builder = GeneralUtils.GetIndentation(level);

        // Return: variable_name := value_expression
        builder += Variable.Unparse(0) + " " + ToString() + " " + Expression.Unparse();
        return builder;
    }

    /// <summary>The target variable being assigned to</summary>
    public VariableNode Variable
    {
        get; set;
    }

    /// <summary>The value expression whose result is assigned to the variable</summary>
    public ExpressionNode Expression
    {
        get; set;
    }

    /// <summary>
    /// Constructs an AssignmentStmt.
    /// </summary>
    /// <param name="l">The target variable</param>
    /// <param name="r">The value expression</param>
    public AssignmentStmt(VariableNode l, ExpressionNode r)
    {
        Variable = l;
        Expression = r;
    }
}

/// <summary>
/// Represents a return statement, e.g. return (a // b).
/// Holds a single child expression whose value is returned from the current block.
/// </summary>
public class ReturnStmt : Statement
{
    /// <summary>Returns the DEC return keyword used during unparsing.</summary>
    public override string ToString()
    {
        return TokenConstants.RETURN;
    }

    /// <summary>
    /// Unparses the return statement as: indentation + "return" + expression.
    /// The child expression is unparsed at level 0 since it is always inline.
    /// </summary>
    public override string Unparse(int level = 0)
    {
        // Combine indentation, the return keyword, and the unparsed child expression.
        return GeneralUtils.GetIndentation(level) + " " + ToString() + " " + Expression.Unparse(0);
    }

    /// <summary>The expression whose value is returned by this statement.</summary>
    public ExpressionNode Expression
    {
        get; set;
    }

    /// <summary>
    /// Constructs a ReturnStmt that returns the value of the given expression.
    /// </summary>
    /// <param name="c">The expression to evaluate and return.</param>
    public ReturnStmt(ExpressionNode c)
    {
        Expression = c;
    }
}