public class BinaryTree <DataT>
{
    protected TreeNode <DataT> Root { get ; set ; }
    protected class TreeNode <T> 
    {
        public TreeNode <T> Left { get ; set ; }
        public TreeNode <T> Right { get ; set ; }
        public T Data { get ; set ; }
        public TreeNode ( TreeNode <T> left , T data , TreeNode <T> right ) 
        {
            Left = left ;
            Data = data ;
            Right = right ;
        }
        public TreeNode ( T data ) : this ( null , data , null ) {}
        public class BinaryDigitTree : BinaryTree < int > 
        {
            // Methods to be completed below in the questions below
            
            public void DivideByTwo()
            {
                
            }
        }
    }
}