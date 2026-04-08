using Utilities.Containers;
using AST;


namespace Optimizer;
public class CFG : DiGraph<Statement>
{
    public Statement? Start { get; set; }
    
}