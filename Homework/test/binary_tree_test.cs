using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Homework.src;

namespace Homework.Tests
{
    public class BinaryTreeTest
    {
        [Fact]
        public void Add_Test()
        {
            BinaryTree<int>.BinaryDigitTree b = new BinaryTree<int>.BinaryDigitTree();
            b.Add(0);
            b.Add(1);
            b.Add(1);
            b.Add(0);
            b.Add(1);
            b.Add(0);
            b.Add(1);

            Assert.Equal("1010110", b.ToString());
            Assert.Equal(86, b.CalculateBaseTen());
        }
    }
}