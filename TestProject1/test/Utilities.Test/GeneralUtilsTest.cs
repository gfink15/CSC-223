// Claude AI was used to create these xUnit tests.

namespace Utilities;

public class GeneralUtilsTests
{
    #region Contains Tests
    
    public static IEnumerable<object[]> ContainsIntTestData =>
        new List<object[]>
        {
            new object[] { new int[] { 1, 2, 3, 4, 5 }, 3, true },
            new object[] { new int[] { 1, 2, 3, 4, 5 }, 6, false },
            new object[] { new int[] { 10 }, 10, true },
            new object[] { new int[] { 10 }, 5, false },
            new object[] { new int[] { -1, 0, 1 }, 0, true },
            new object[] { new int[] { }, 1, false }
        };

    [Theory]
    [MemberData(nameof(ContainsIntTestData))]
    public void Contains_WithIntArray_ReturnsExpectedResult(int[] arr, int target, bool expected)
    {
        bool result = GeneralUtils.Contains(arr, target);
        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> ContainsStringTestData =>
        new List<object[]>
        {
            new object[] { new string[] { "apple", "banana", "cherry" }, "banana", true },
            new object[] { new string[] { "apple", "banana", "cherry" }, "orange", false },
            new object[] { new string[] { "hello" }, "hello", true },
            new object[] { new string[] { "" }, "", true },
            new object[] { new string[] { }, "test", false }
        };

    [Theory]
    [MemberData(nameof(ContainsStringTestData))]
    public void Contains_WithStringArray_ReturnsExpectedResult(string[] arr, string target, bool expected)
    {
        bool result = GeneralUtils.Contains(arr, target);
        Assert.Equal(expected, result);
    }

    #endregion

    #region GetIndentation Tests

    public static IEnumerable<object[]> IndentationTestData =>
        new List<object[]>
        {
            new object[] { 0, "" },
            new object[] { 1, "    " },
            new object[] { 2, "        " },
            new object[] { 3, "            " },
            new object[] { 5, "                    " }
        };

    [Theory]
    [MemberData(nameof(IndentationTestData))]
    public void GetIndentation_WithValidLevel_ReturnsCorrectSpaces(int level, string expected)
    {
        string result = GeneralUtils.GetIndentation(level);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 4)]
    [InlineData(2, 8)]
    [InlineData(3, 12)]
    public void GetIndentation_WithValidLevel_ReturnsCorrectLength(int level, int expectedLength)
    {
        string result = GeneralUtils.GetIndentation(level);
        Assert.Equal(expectedLength, result.Length);
    }

    [Fact]
    public void GetIndentation_WithZeroLevel_ReturnsEmptyString()
    {
        string result = GeneralUtils.GetIndentation(0);
        Assert.Empty(result);
    }

    #endregion

    #region IsValidVariable Tests (only lowercase letters)

    public static IEnumerable<object[]> ValidVariableNames =>
        new List<object[]>
        {
            new object[] { "test" },
            new object[] { "abc" },
            new object[] { "lowercase" },
            new object[] { "a" },
            new object[] { "z" }
        };

    [Theory]
    [MemberData(nameof(ValidVariableNames))]
    public void IsValidVariable_WithOnlyLowercaseLetters_ReturnsTrue(string variableName)
    {
        bool result = GeneralUtils.IsValidVariable(variableName);
        Assert.True(result);
    }

    public static IEnumerable<object[]> InvalidVariableNames =>
        new List<object[]>
        {
            new object[] { "Test" },
            new object[] { "TEST" },
            new object[] { "test!" },
            new object[] { "283495" },
            new object[] { "test123" },
            new object[] { "test_var" },
            new object[] { "test var" },
            new object[] { "" }
        };

    [Theory]
    [MemberData(nameof(InvalidVariableNames))]
    public void IsValidVariable_WithNonLowercaseCharacters_ReturnsFalse(string variableName)
    {
        bool result = GeneralUtils.IsValidVariable(variableName);
        Assert.False(result);
    }

    #endregion

    #region IsValidOperator Tests

    [Theory]
    [InlineData("+", true)]
    [InlineData("-", true)]
    [InlineData("*", true)]
    [InlineData("/", true)]
    [InlineData("//", true)]
    [InlineData("%", true)]
    [InlineData("**", true)]
    public void IsValidOperator_WithValidOperator_ReturnsTrue(string op, bool expected)
    {
        bool result = GeneralUtils.IsValidOperator(op);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("&")]
    [InlineData("|")]
    [InlineData("^")]
    [InlineData("=")]
    [InlineData("++")]
    [InlineData("--")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("+ ")]
    [InlineData(" +")]
    [InlineData("abc")]
    [InlineData("***")]
    [InlineData("///")]
    public void IsValidOperator_WithInvalidOperator_ReturnsFalse(string op)
    {
        bool result = GeneralUtils.IsValidOperator(op);
        Assert.False(result);
    }

    #endregion

    #region CountCharacter Tests

    [Theory]
    [InlineData("hello", 'l', 2)]
    [InlineData("hello", 'h', 1)]
    [InlineData("hello", 'o', 1)]
    [InlineData("hello", 'x', 0)]
    [InlineData("", 'a', 0)]
    [InlineData("aaa", 'a', 3)]
    [InlineData("Hello", 'h', 0)]
    [InlineData("Hello", 'H', 1)]
    public void CountCharacter_WithVariousInputs_ReturnsCorrectCount(string str, char ch, int expected)
    {
        int result = GeneralUtils.CountOccurrences(str, ch);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CountCharacter_WithSpaceCharacter_ReturnsCorrectCount()
    {
        int result = GeneralUtils.CountOccurrences("hello world test", ' ');
        Assert.Equal(2, result);
    }

    #endregion

    #region ToCamelCase Tests

    [Theory]
    [InlineData("Hello world test string", "helloWorldTestString")]
    [InlineData("hello world", "helloWorld")]
    [InlineData("test", "test")]
    [InlineData("a b c d", "aBCD")]
    [InlineData("first second third", "firstSecondThird")]
    [InlineData("UPPER CASE", "upperCase")]
    public void ToCamelCase_WithSpaceSeparatedWords_ReturnsCamelCase(string input, string expected)
    {
        string result = GeneralUtils.ToCamelCase(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void ToCamelCase_WithEmptyOrWhitespace_ReturnsEmptyString(string input)
    {
        string result = GeneralUtils.ToCamelCase(input);
        Assert.Equal("", result);
    }

    [Fact]
    public void ToCamelCase_WithMultipleSpaces_HandlesProperly()
    {
        string result = GeneralUtils.ToCamelCase("hello  world");
        Assert.Equal("helloWorld", result);
    }

    #endregion

    #region IsStrongPassword Tests

    [Theory]
    [InlineData("Passw0rd!", true)]
    [InlineData("Str0ng#Pass", true)]
    [InlineData("MyP@ssw0rd", true)]
    [InlineData("Secur3$Pass", true)]
    public void IsStrongPassword_WithValidPassword_ReturnsTrue(string password, bool expected)
    {
        bool result = GeneralUtils.IsPasswordStrong(password);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("short1!")]           // Too short (7 chars)
    [InlineData("PASSWORD1!")]        // No lowercase
    [InlineData("password1!")]        // No uppercase
    [InlineData("Password!")]         // No digit
    [InlineData("Password1")]         // No special character
    [InlineData("")]                  // Empty
    [InlineData("Pass1!")]            // Too short
    [InlineData("onlylowercase")]     // No uppercase, digit, or special
    [InlineData("ONLYUPPERCASE")]     // No lowercase, digit, or special
    [InlineData("12345678")]          // Only digits
    public void IsStrongPassword_WithWeakPassword_ReturnsFalse(string password)
    {
        bool result = GeneralUtils.IsPasswordStrong(password);
        Assert.False(result);
    }

    [Fact]
    public void IsStrongPassword_WithExactly8Chars_AndAllRequirements_ReturnsTrue()
    {
        bool result = GeneralUtils.IsPasswordStrong("Passw0r!");
        Assert.True(result);
    }

    #endregion

    #region GetUniqueItems Tests

    public static IEnumerable<object[]> UniqueItemsTestData =>
        new List<object[]>
        {
            new object[] { new List<int> { 1, 2, 3, 2, 1 }, new List<int> { 1, 2, 3 } },
            new object[] { new List<int> { 1, 2, 3, 4, 5 }, new List<int> { 1, 2, 3, 4, 5 } },
            new object[] { new List<int> { 1, 1, 1, 1 }, new List<int> { 1 } },
            new object[] { new List<int> { }, new List<int> { } }
        };

    [Theory]
    [MemberData(nameof(UniqueItemsTestData))]
    public void GetUniqueItems_WithIntList_ReturnsUniqueItems(List<int> input, List<int> expected)
    {
        List<int> result = GeneralUtils.GetUniqueItems(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetUniqueItems_WithNull_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => GeneralUtils.GetUniqueItems<int>(null));
    }

    [Fact]
    public void GetUniqueItems_DoesNotModifyOriginalList()
    {
        List<int> original = new List<int> { 1, 2, 3, 2, 1 };
        List<int> originalCopy = new List<int>(original);
        
        GeneralUtils.GetUniqueItems(original);
        
        Assert.Equal(originalCopy, original);
    }

    public static IEnumerable<object[]> UniqueStringTestData =>
        new List<object[]>
        {
            new object[] { new List<string> { "a", "b", "a" }, new List<string> { "a", "b" } },
            new object[] { new List<string> { "hello", "world", "hello" }, new List<string> { "hello", "world" } }
        };

    [Theory]
    [MemberData(nameof(UniqueStringTestData))]
    public void GetUniqueItems_WithStringList_ReturnsUniqueItems(List<string> input, List<string> expected)
    {
        List<string> result = GeneralUtils.GetUniqueItems(input);
        Assert.Equal(expected, result);
    }

    #endregion

    #region CalculateAverage Tests

    [Theory]
    [InlineData(new int[] { 1, 2, 3, 4, 5 }, 3.0)]
    [InlineData(new int[] { 10, 20, 30 }, 20.0)]
    [InlineData(new int[] { 5 }, 5.0)]
    [InlineData(new int[] { -5, 5 }, 0.0)]
    [InlineData(new int[] { 100, 200, 300 }, 200.0)]
    public void CalculateAverage_WithValidArray_ReturnsCorrectAverage(int[] array, double expected)
    {
        double result = GeneralUtils.CalculateAverage(array);
        Assert.Equal(expected, result, 2);
    }

    [Fact]
    public void CalculateAverage_WithNull_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => GeneralUtils.CalculateAverage(null));
    }

    [Fact]
    public void CalculateAverage_WithEmptyArray_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => GeneralUtils.CalculateAverage(new int[] { }));
    }

    [Fact]
    public void CalculateAverage_WithNegativeNumbers_ReturnsCorrectAverage()
    {
        double result = GeneralUtils.CalculateAverage(new int[] { -10, -20, -30 });
        Assert.Equal(-20.0, result, 2);
    }

    #endregion

    #region GetDuplicates Tests

    public static IEnumerable<object[]> DuplicatesTestData =>
        new List<object[]>
        {
            new object[] { new int[] { 1, 2, 3, 2, 1, 4 }, new int[] { 1, 2 } },
            new object[] { new int[] { 1, 1, 1, 2, 2, 3 }, new int[] { 1, 2 } },
            new object[] { new int[] { 1, 2, 3, 4, 5 }, new int[] { } },
            new object[] { new int[] { 5, 5, 5, 5 }, new int[] { 5 } },
            new object[] { new int[] { }, new int[] { } }
        };

    [Theory]
    [MemberData(nameof(DuplicatesTestData))]
    public void GetDuplicates_WithIntArray_ReturnsCorrectDuplicates(int[] array, int[] expected)
    {
        int[] result = GeneralUtils.Duplicates(array);
        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> StringDuplicatesTestData =>
        new List<object[]>
        {
            new object[] { new string[] { "a", "b", "a", "c", "b" }, new string[] { "a", "b" } },
            new object[] { new string[] { "hello", "world", "hello" }, new string[] { "hello" } },
            new object[] { new string[] { "unique" }, new string[] { } }
        };

    [Theory]
    [MemberData(nameof(StringDuplicatesTestData))]
    public void GetDuplicates_WithStringArray_ReturnsCorrectDuplicates(string[] array, string[] expected)
    {
        string[] result = GeneralUtils.Duplicates(array);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetDuplicates_WithAllSameElements_ReturnsSingleElement()
    {
        int[] result = GeneralUtils.Duplicates(new int[] { 7, 7, 7, 7, 7 });
        Assert.Single(result);
        Assert.Contains(7, result);
    }

    #endregion
}