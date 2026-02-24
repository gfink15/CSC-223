/**
 * Binary Tree Homework: Provides basic implementation for
 * manipulating binary numbers stored in trees.
 * Claude AI was only used for writing XML and inline comments.
 * 
 * @author Graham Fink, Mridul Agrawal
 * @date   2/23/2026
 */
namespace Homework
{
    /// <summary>
    /// A binary tree where each internal node stores one data element in its Right child,
    /// and the Left child points to the next node. The tree is traversed left-spine first.
    /// </summary>
    public class BinaryTree<DataT>
    {
        // The sentinel root node; actual data is stored in Right children along the left spine
        protected TreeNode<DataT> Root { get; set; }

        /// <summary>
        /// Initializes an empty BinaryTree with a sentinel root node.
        /// </summary>
        public BinaryTree()
        {
            // Root is a placeholder node with no data and no children
            Root = new TreeNode<DataT>(null, default, null);
        }

        /// <summary>
        /// Adds a new element to the front of the tree (most significant position).
        /// Traverses to the leftmost node, then appends a new node pair.
        /// </summary>
        /// <param name="e">The data element to add.</param>
        public void Add(DataT e)
        {
            // Start traversal from the root
            TreeNode<DataT> currentNode = Root;

            // Walk down the left spine to find the leftmost node
            while (currentNode.Left != null)
            {
                currentNode = currentNode.Left;
            }

            // Append a new sentinel (Left) and data node (Right) at the end
            currentNode.Left = new TreeNode<DataT>(default);
            currentNode.Right = new TreeNode<DataT>(e);
        }

        /// <summary>
        /// Removes the top (most significant) node by advancing Root to its Left child.
        /// </summary>
        /// <param name="n">The node to remove (unused; always removes Root).</param>
        protected void RemoveTop(TreeNode<DataT> n)
        {
            // Discard the current root, promoting its left child as the new root
            this.Root = this.Root.Left;
        }

        /// <summary>
        /// Returns a string representation of the tree's data,
        /// reading from the most significant to least significant element.
        /// </summary>
        /// <returns>A concatenated string of all data values in order.</returns>
        public string ToString()
        {
            string output = "";
            var currentNode = Root;

            // Traverse left spine, prepending each Right node's data to build the output string
            while (currentNode.Left != null)
            {
                output = currentNode.Right.Data + output;
                currentNode = currentNode.Left;
            }

            return output;
        }

        /// <summary>
        /// Internal node class for the BinaryTree. Each node holds a data value
        /// and references to Left and Right child nodes.
        /// </summary>
        protected class TreeNode<T>
        {
            // Left child points to the next node along the spine; Right holds this node's data node
            public TreeNode<T> Left { get; set; }
            public TreeNode<T> Right { get; set; }
            public T Data { get; set; }

            /// <summary>
            /// Initializes a TreeNode with explicit left, data, and right values.
            /// </summary>
            public TreeNode(TreeNode<T> left, T data, TreeNode<T> right)
            {
                Left = left;
                Data = data;
                Right = right;
            }

            /// <summary>
            /// Convenience constructor: creates a leaf node with no children.
            /// </summary>
            public TreeNode(T data) : this(null, data, null) { }
        }

        /// <summary>
        /// A specialization of BinaryTree for integers, representing a binary number.
        /// Each node along the left spine holds one binary digit (0 or 1) in its Right child.
        /// The most significant bit is at the root.
        /// </summary>
        public class BinaryDigitTree : BinaryTree<int>
        {
            public BinaryDigitTree() : base() { }
            #region Question 1
            /// <summary>
            /// Divides the binary number by two by removing the most significant bit (the root).
            /// Equivalent to a right bit-shift by one.
            /// </summary>
            public void DivideByTwo()
            {
                Root = Root.Left;
            }
            #endregion
            #region Question 2
            /// <summary>
            /// Divides the binary number by 2^n by removing the top n bits.
            /// Equivalent to a right bit-shift by n.
            /// </summary>
            /// <param name="n">The power of two to divide by. Must be non-negative.</param>
            /// <exception cref="ArgumentException">Thrown if n is negative.</exception>
            public void DivideByPower(int n)
            {
                if (n < 0) throw new ArgumentException("Input must be positive");

                // Remove the top node n times, each time shifting the number right by one bit
                while (n > 0)
                {
                    DivideByTwo();
                    n--;
                }
            }
            #endregion
            #region Question 3
            /// <summary>
            /// Calculates and returns the base-10 (decimal) value of the stored binary number.
            /// </summary>
            /// <returns>The decimal integer equivalent of the binary tree's value.</returns>
            public int CalculateBaseTen()
            {
                return CalculateBaseTen(Root);
            }

            /// <summary>
            /// Recursively computes the decimal value of the binary number
            /// represented by the subtree rooted at <paramref name="n"/>.
            /// </summary>
            /// <param name="n">The current node in the recursive traversal.</param>
            /// <returns>The decimal value of the subtree.</returns>
            private int CalculateBaseTen(TreeNode<int> n)
            {
                // Base case: reached the least significant bit, return it directly
                if (n.Left.Left == null) return n.Right.Data;

                // Recursive case: double the value of the left subtree and add this bit
                return 2 * CalculateBaseTen(n.Left) + n.Right.Data;
            }
            #endregion
            #region Question 4
            /// <summary>
            /// Increments the binary number stored in the tree by one.
            /// Handles carry propagation automatically.
            /// </summary>
            public void Increment()
            {
                Increment(Root);
            }

            /// <summary>
            /// Recursively increments the binary number at node <paramref name="n"/>,
            /// propagating carry to more significant bits as needed.
            /// </summary>
            /// <param name="n">The current node being incremented.</param>
            private void Increment(TreeNode<int> n)
            {
                // If this bit is 0, simply set it to 1 — no carry needed
                if (n.Right.Data == 0) n.Right.Data = 1;
                else
                {
                    // This bit is 1; set it to 0 and carry over to the next significant bit
                    n.Right.Data = 0;

                    if (n.Left == null)
                    {
                        // No more significant bit exists; extend the tree with a new leading 1
                        n.Left = new TreeNode<int>(default);
                        n.Left.Right = new TreeNode<int>(1);
                    }
                    else Increment(n.Left); // Propagate carry to the next bit
                }
            }
            #endregion
        }
    }
}