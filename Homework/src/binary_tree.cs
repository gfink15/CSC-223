namespace Homework.src
{
    public class BinaryTree<DataT>
    {
        protected TreeNode<DataT> Root { get; set; }
        public BinaryTree()
        {
            Root = new TreeNode<DataT>(null, default, null);
        }
        public void Add(DataT e)
        {
            TreeNode<DataT> currentNode = Root;
            while(currentNode.Left != null)
            {
                currentNode = currentNode.Left;
            }
            currentNode.Left = new TreeNode<DataT>(default);
            currentNode.Right = new TreeNode<DataT>(e);
        }
        protected void RemoveTop(TreeNode<DataT> n)
        {
            this.Root = this.Root.Left;
        }
        public string ToString()
        {
            string output = "";
            var currentNode = Root;
            // output += currentNode.Right.Data + ", ";
            while(currentNode.Left != null)
            {
                output = currentNode.Right.Data + output;
                currentNode = currentNode.Left;
            }
            return output;
        }
        protected class TreeNode <T> 
        {
            public TreeNode<T> Left { get; set; }
            public TreeNode<T> Right { get; set; }
            public T Data { get; set; }
            public TreeNode(TreeNode<T> left, T data, TreeNode<T> right) 
            {
                Left = left ;
                Data = data ;
                Right = right ;
            }
            public TreeNode(T data) : this(null, data, null){}
        }
        public class BinaryDigitTree : BinaryTree <int> 
        {
            public BinaryDigitTree() : base() {}
            // Methods to be completed below in the questions below

            public void DivideByTwo()
            {
                RemoveTop(Root);
            }
            public void DivideByPower(int n)
            {
                if (n < 0) throw new ArgumentException("Input must be positive");
                while (n > 0)
                {
                    RemoveTop(Root);
                    n--;
                }
            }
            public int CalculateBaseTen()
            {
                return CalculateBaseTen(Root);
            }
            private int CalculateBaseTen(TreeNode<int> n)
            {
                if (n.Left.Left == null) return n.Right.Data;
                return 2*CalculateBaseTen(n.Left) + n.Right.Data;
            }
            public void Increment()
            {
                Increment(Root);
            }
            private void Increment(TreeNode<int> n)
            {
                if (n.Right.Data == 0) n.Right.Data = 1;
                else
                {
                    n.Right.Data = 0;
                    if (n.Left == null)
                    {
                        n.Left = new TreeNode<int>(default);
                        n.Left.Right = new TreeNode<int>(1);
                    }
                    else Increment(n.Left);
                }
            }
        }
    }
}