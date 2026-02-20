namespace Homework;

public class BinaryTreeTest
{
    [Fact]
    public void Test1()
    {
        BinaryTree<int>.BinaryDigitTree b = new BinaryTree<int>.BinaryDigitTree();
        b.Add(0);
        b.Add(1);
        b.Add(1);
        b.Add(0);
        b.Add(1);
        b.Add(0);
        b.Add(1);
        Assert.Equal("0, 1, 1, 0, 1, 0, 1, ", b.ToString());
        Assert.Equal(53, b.CalculateBaseTen());
    }
}