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
        Assert.Equal(26, b.CalculateBaseTen());
    }
}