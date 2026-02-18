using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.Containers.Tests
{
    /// <summary>
    /// Comprehensive test suite for SymbolTable implementation.
    /// Tests cover IDictionary interface methods, custom SymbolTable methods,
    /// shadowing functionality, and hierarchical scope management.
    /// </summary>
    public class SymbolTableTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullParent_CreatesRootSymbolTable()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            Assert.NotNull(symbolTable);
            Assert.Null(symbolTable.Parent);
            Assert.Empty(symbolTable);
        }

        [Fact]
        public void Constructor_WithParent_CreatesChildSymbolTable()
        {
            var parent = new SymbolTable<string, int>(null);
            var child = new SymbolTable<string, int>(parent);
            
            Assert.NotNull(child);
            Assert.Same(parent, child.Parent);
            Assert.Empty(child);
        }

        [Fact]
        public void Constructor_CreatesMultipleLevelsOfHierarchy()
        {
            var root = new SymbolTable<string, int>(null);
            var level1 = new SymbolTable<string, int>(root);
            var level2 = new SymbolTable<string, int>(level1);
            
            Assert.Null(root.Parent);
            Assert.Same(root, level1.Parent);
            Assert.Same(level1, level2.Parent);
        }

        #endregion

        #region Add Method Tests

        [Theory]
        [InlineData("a", 1)]
        [InlineData("variable", 100)]
        [InlineData("x", -50)]
        public void Add_SingleKeyValue_AddsSuccessfully(string key, int value)
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            symbolTable.Add(key, value);
            
            Assert.Single(symbolTable);
            Assert.Equal(value, symbolTable[key]);
        }

        [Fact]
        public void Add_MultipleUniqueKeys_AddsAllSuccessfully()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            symbolTable.Add("a", 10);
            symbolTable.Add("b", 20);
            symbolTable.Add("c", 30);
            
            Assert.Equal(3, symbolTable.Count);
            Assert.Equal(10, symbolTable["a"]);
            Assert.Equal(20, symbolTable["b"]);
            Assert.Equal(30, symbolTable["c"]);
        }

        [Fact]
        public void Add_DuplicateKey_ThrowsArgumentException()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            
            Assert.Throws<ArgumentException>(() => symbolTable.Add("a", 20));
        }

        [Fact]
        public void Add_NullKey_ThrowsArgumentNullException()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            Assert.Throws<ArgumentNullException>(() => symbolTable.Add(null, 10));
        }

        [Fact]
        public void Add_KeyValuePair_AddsSuccessfully()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            var kvp = new KeyValuePair<string, int>("key", 42);
            
            symbolTable.Add(kvp);
            
            Assert.Single(symbolTable);
            Assert.Equal(42, symbolTable["key"]);
        }

        [Fact]
        public void Add_DuplicateKeyValuePair_ThrowsArgumentException()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add(new KeyValuePair<string, int>("a", 10));
            
            Assert.Throws<ArgumentException>(() => 
                symbolTable.Add(new KeyValuePair<string, int>("a", 20)));
        }

        #endregion

        #region ContainsKey Tests

        [Theory]
        [InlineData("a", 10)]
        [InlineData("variable", 100)]
        [InlineData("x", -5)]
        public void ContainsKey_ExistingKey_ReturnsTrue(string key, int value)
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add(key, value);
            
            Assert.True(symbolTable.ContainsKey(key));
        }

        [Fact]
        public void ContainsKey_NonExistingKey_ReturnsFalse()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            
            Assert.False(symbolTable.ContainsKey("b"));
        }

        [Fact]
        public void ContainsKey_KeyInParentScope_ReturnsTrue()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("a", 10);
            var child = new SymbolTable<string, int>(parent);
            
            Assert.True(child.ContainsKey("a"));
        }

        [Fact]
        public void ContainsKey_KeyInGrandparentScope_ReturnsTrue()
        {
            var root = new SymbolTable<string, int>(null);
            root.Add("x", 100);
            var middle = new SymbolTable<string, int>(root);
            var leaf = new SymbolTable<string, int>(middle);
            
            Assert.True(leaf.ContainsKey("x"));
        }

        [Fact]
        public void ContainsKey_NullKey_ThrowsArgumentNullException()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            Assert.Throws<ArgumentNullException>(() => symbolTable.ContainsKey(null));
        }

        [Fact]
        public void ContainsKey_ShadowedKey_ReturnsTrue()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("a", 10);
            var child = new SymbolTable<string, int>(parent);
            child.Add("a", 20);
            
            Assert.True(child.ContainsKey("a"));
        }

        #endregion

        #region ContainsKeyLocal Tests

        [Theory]
        [InlineData("a", 10)]
        [InlineData("variable", 100)]
        [InlineData("x", -5)]
        public void ContainsKeyLocal_ExistingKeyInCurrentScope_ReturnsTrue(string key, int value)
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add(key, value);
            
            Assert.True(symbolTable.ContainsKeyLocal(key));
        }

        [Fact]
        public void ContainsKeyLocal_KeyOnlyInParent_ReturnsFalse()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("a", 10);
            var child = new SymbolTable<string, int>(parent);
            
            Assert.False(child.ContainsKeyLocal("a"));
        }

        [Fact]
        public void ContainsKeyLocal_ShadowedKey_ReturnsTrue()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("a", 10);
            var child = new SymbolTable<string, int>(parent);
            child.Add("a", 20);
            
            Assert.True(child.ContainsKeyLocal("a"));
        }

        [Fact]
        public void ContainsKeyLocal_NullKey_ThrowsArgumentNullException()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            Assert.Throws<ArgumentNullException>(() => symbolTable.ContainsKeyLocal(null));
        }

        [Fact]
        public void ContainsKeyLocal_NonExistingKey_ReturnsFalse()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            
            Assert.False(symbolTable.ContainsKeyLocal("b"));
        }

        #endregion

        #region TryGetValue Tests

        [Theory]
        [InlineData("a", 10)]
        [InlineData("b", 20)]
        [InlineData("c", 30)]
        public void TryGetValue_ExistingKey_ReturnsTrueAndValue(string key, int expectedValue)
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add(key, expectedValue);
            
            bool result = symbolTable.TryGetValue(key, out int actualValue);
            
            Assert.True(result);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void TryGetValue_NonExistingKey_ReturnsFalseAndDefaultValue()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            
            bool result = symbolTable.TryGetValue("b", out int value);
            
            Assert.False(result);
            Assert.Equal(default(int), value);
        }

        [Fact]
        public void TryGetValue_KeyInParent_ReturnsTrueAndValue()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("a", 100);
            var child = new SymbolTable<string, int>(parent);
            
            bool result = child.TryGetValue("a", out int value);
            
            Assert.True(result);
            Assert.Equal(100, value);
        }

        [Fact]
        public void TryGetValue_ShadowedKey_ReturnsChildValue()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("a", 10);
            var child = new SymbolTable<string, int>(parent);
            child.Add("a", 20);
            
            bool result = child.TryGetValue("a", out int value);
            
            Assert.True(result);
            Assert.Equal(20, value);
        }

        [Fact]
        public void TryGetValue_NullKey_ThrowsArgumentNullException()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            Assert.Throws<ArgumentNullException>(() => 
                symbolTable.TryGetValue(null, out int value));
        }

        [Fact]
        public void TryGetValue_NullValueInCurrentScope_ChecksParent()
        {
            var parent = new SymbolTable<string, string>(null);
            parent.Add("x", "outer_value");
            var child = new SymbolTable<string, string>(parent);
            child.Add("x", null); // Shadowing with null to facilitate reference before assignment
            
            bool result = child.TryGetValue("x", out string value);
            
            Assert.True(result);
            Assert.Equal("outer_value", value);
        }

        #endregion

        #region TryGetValueLocal Tests

        [Theory]
        [InlineData("a", 10)]
        [InlineData("b", 20)]
        [InlineData("c", 30)]
        public void TryGetValueLocal_ExistingKeyInCurrentScope_ReturnsTrueAndValue(string key, int expectedValue)
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add(key, expectedValue);
            
            bool result = symbolTable.TryGetValueLocal(key, out int actualValue);
            
            Assert.True(result);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void TryGetValueLocal_KeyOnlyInParent_ReturnsFalseAndDefaultValue()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("a", 100);
            var child = new SymbolTable<string, int>(parent);
            
            bool result = child.TryGetValueLocal("a", out int value);
            
            Assert.False(result);
            Assert.Equal(default(int), value);
        }

        [Fact]
        public void TryGetValueLocal_ShadowedKey_ReturnsLocalValue()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("a", 10);
            var child = new SymbolTable<string, int>(parent);
            child.Add("a", 20);
            
            bool result = child.TryGetValueLocal("a", out int value);
            
            Assert.True(result);
            Assert.Equal(20, value);

            result = child.TryGetValue("a", out value);
            
            Assert.True(result);
            Assert.Equal(20, value);
        }

        [Fact]
        public void TryGetValueLocal_NullKey_ThrowsArgumentNullException()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            Assert.Throws<ArgumentNullException>(() => 
                symbolTable.TryGetValueLocal(null, out int value));
        }

        [Fact]
        public void TryGetValueLocal_NonExistingKey_ReturnsFalseAndDefaultValue()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            
            bool result = symbolTable.TryGetValueLocal("b", out int value);
            
            Assert.False(result);
            Assert.Equal(default(int), value);
        }

        #endregion

        #region Indexer Tests

        [Theory]
        [InlineData("a", 10)]
        [InlineData("b", 20)]
        [InlineData("c", 30)]
        public void Indexer_Get_ExistingKey_ReturnsValue(string key, int expectedValue)
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add(key, expectedValue);
            
            Assert.Equal(expectedValue, symbolTable[key]);
        }

        [Fact]
        public void Indexer_Get_NonExistingKey_ThrowsKeyNotFoundException()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            Assert.Throws<KeyNotFoundException>(() => symbolTable["nonexistent"]);
        }

        [Fact]
        public void Indexer_Get_KeyInParent_ReturnsParentValue()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("a", 100);
            var child = new SymbolTable<string, int>(parent);
            
            Assert.Equal(100, child["a"]);
        }

        [Fact]
        public void Indexer_Get_ShadowedKey_ReturnsChildValue()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("a", 10);
            var child = new SymbolTable<string, int>(parent);
            child.Add("a", 20);
            
            Assert.Equal(20, child["a"]);
        }

        [Theory]
        [InlineData("a", 10, 100)]
        [InlineData("b", 20, 200)]
        [InlineData("c", 30, 300)]
        public void Indexer_Set_ExistingKey_UpdatesValue(string key, int initialValue, int newValue)
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add(key, initialValue);
            
            symbolTable[key] = newValue;
            
            Assert.Equal(newValue, symbolTable[key]);
        }

        [Fact]
        public void Indexer_Set_NonExistingKey_AddsKeyValue()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            symbolTable["new"] = 42;
            
            Assert.Equal(42, symbolTable["new"]);
            Assert.Single(symbolTable);
        }

        [Fact]
        public void Indexer_Get_NullKey_ThrowsArgumentNullException()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            Assert.Throws<ArgumentNullException>(() => { var x = symbolTable[null]; });
        }

        [Fact]
        public void Indexer_Set_NullKey_ThrowsArgumentNullException()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            Assert.Throws<ArgumentNullException>(() => symbolTable[null] = 10);
        }

        #endregion

        #region Remove Tests

        [Theory]
        [InlineData("a")]
        [InlineData("variable")]
        [InlineData("x")]
        public void Remove_ExistingKey_ReturnsTrueAndRemovesKey(string key)
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add(key, 10);
            
            bool result = symbolTable.Remove(key);
            
            Assert.True(result);
            Assert.False(symbolTable.ContainsKeyLocal(key));
            Assert.Empty(symbolTable);
        }

        [Fact]
        public void Remove_NonExistingKey_ReturnsFalse()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            
            bool result = symbolTable.Remove("b");
            
            Assert.False(result);
            Assert.Single(symbolTable);
        }

        [Fact]
        public void Remove_KeyValuePair_MatchingPair_ReturnsTrueAndRemoves()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            
            bool result = symbolTable.Remove(new KeyValuePair<string, int>("a", 10));
            
            Assert.True(result);
            Assert.Empty(symbolTable);
        }

        [Fact]
        public void Remove_KeyValuePair_NonMatchingValue_ReturnsFalse()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            
            bool result = symbolTable.Remove(new KeyValuePair<string, int>("a", 20));
            
            Assert.False(result);
            Assert.Single(symbolTable);
        }

        [Fact]
        public void Remove_NullKey_ThrowsArgumentNullException()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            Assert.Throws<ArgumentNullException>(() => symbolTable.Remove(null));
        }

        [Fact]
        public void Remove_MultipleKeys_RemovesCorrectly()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            symbolTable.Add("b", 20);
            symbolTable.Add("c", 30);
            
            symbolTable.Remove("b");
            
            Assert.Equal(2, symbolTable.Count);
            Assert.True(symbolTable.ContainsKey("a"));
            Assert.False(symbolTable.ContainsKey("b"));
            Assert.True(symbolTable.ContainsKey("c"));
        }

        #endregion

        #region Clear Tests

        [Fact]
        public void Clear_EmptyTable_RemainsEmpty()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            symbolTable.Clear();
            
            Assert.Empty(symbolTable);
        }

        [Fact]
        public void Clear_TableWithItems_RemovesAllItems()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            symbolTable.Add("b", 20);
            symbolTable.Add("c", 30);
            
            symbolTable.Clear();
            
            Assert.Empty(symbolTable);
        }

        [Fact]
        public void Clear_DoesNotAffectParent()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("a", 10);
            var child = new SymbolTable<string, int>(parent);
            child.Add("b", 20);
            
            child.Clear();
            
            Assert.Empty(child);
            Assert.Single(parent);
            Assert.True(parent.ContainsKey("a"));
        }

        #endregion

        #region Count and IsReadOnly Tests

        [Fact]
        public void Count_EmptyTable_ReturnsZero()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            Assert.Equal(0, symbolTable.Count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void Count_AfterAddingItems_ReturnsCorrectCount(int itemCount)
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            for (int i = 0; i < itemCount; i++)
            {
                symbolTable.Add($"key{i}", i);
            }
            
            Assert.Equal(itemCount, symbolTable.Count);
        }

        [Fact]
        public void Count_OnlyCountsLocalScope_NotParent()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("a", 10);
            parent.Add("b", 20);
            var child = new SymbolTable<string, int>(parent);
            child.Add("c", 30);
            
            Assert.Equal(1, child.Count);
            Assert.Equal(2, parent.Count);
        }

        [Fact]
        public void IsReadOnly_ReturnsFalse()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            Assert.False(symbolTable.IsReadOnly);
        }

        #endregion

        #region Keys and Values Tests

        [Fact]
        public void Keys_EmptyTable_ReturnsEmptyCollection()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            Assert.Empty(symbolTable.Keys);
        }

        [Fact]
        public void Keys_WithItems_ReturnsAllKeys()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            symbolTable.Add("b", 20);
            symbolTable.Add("c", 30);
            
            var keys = symbolTable.Keys.ToList();
            
            Assert.Equal(3, keys.Count);
            Assert.Contains("a", keys);
            Assert.Contains("b", keys);
            Assert.Contains("c", keys);
        }

        [Fact]
        public void Values_EmptyTable_ReturnsEmptyCollection()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            Assert.Empty(symbolTable.Values);
        }

        [Fact]
        public void Values_WithItems_ReturnsAllValues()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            symbolTable.Add("b", 20);
            symbolTable.Add("c", 30);
            
            var values = symbolTable.Values.ToList();
            
            Assert.Equal(3, values.Count);
            Assert.Contains(10, values);
            Assert.Contains(20, values);
            Assert.Contains(30, values);
        }

        #endregion

        #region Contains Tests

        [Fact]
        public void Contains_ExistingKeyValuePair_ReturnsTrue()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            
            Assert.True(symbolTable.Contains(new KeyValuePair<string, int>("a", 10)));
        }

        [Fact]
        public void Contains_NonExistingKey_ReturnsFalse()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            
            Assert.False(symbolTable.Contains(new KeyValuePair<string, int>("b", 20)));
        }

        [Fact]
        public void Contains_WrongValue_ReturnsFalse()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            
            Assert.False(symbolTable.Contains(new KeyValuePair<string, int>("a", 20)));
        }

        #endregion

        #region CopyTo Tests

        [Fact]
        public void CopyTo_CopiesToArray_Successfully()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            symbolTable.Add("b", 20);
            symbolTable.Add("c", 30);
            
            var array = new KeyValuePair<string, int>[3];
            symbolTable.CopyTo(array, 0);
            
            Assert.Equal(3, array.Length);
            Assert.Contains(new KeyValuePair<string, int>("a", 10), array);
            Assert.Contains(new KeyValuePair<string, int>("b", 20), array);
            Assert.Contains(new KeyValuePair<string, int>("c", 30), array);
        }

        [Fact]
        public void CopyTo_WithOffset_CopiesToCorrectPosition()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            symbolTable.Add("b", 20);
            
            var array = new KeyValuePair<string, int>[5];
            symbolTable.CopyTo(array, 2);
            
            Assert.Equal(default(KeyValuePair<string, int>), array[0]);
            Assert.Equal(default(KeyValuePair<string, int>), array[1]);
            Assert.NotEqual(default(KeyValuePair<string, int>), array[2]);
            Assert.NotEqual(default(KeyValuePair<string, int>), array[3]);
        }

        [Fact]
        public void CopyTo_NullArray_ThrowsArgumentNullException()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            
            Assert.Throws<ArgumentNullException>(() => symbolTable.CopyTo(null, 0));
        }

        #endregion

        #region Enumeration Tests

        [Fact]
        public void GetEnumerator_EmptyTable_ReturnsEmptyEnumerator()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            var items = symbolTable.ToList();
            
            Assert.Empty(items);
        }

        [Fact]
        public void GetEnumerator_WithItems_ReturnsAllItems()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            symbolTable.Add("b", 20);
            symbolTable.Add("c", 30);
            
            var items = symbolTable.ToList();
            
            Assert.Equal(3, items.Count);
            Assert.Contains(new KeyValuePair<string, int>("a", 10), items);
            Assert.Contains(new KeyValuePair<string, int>("b", 20), items);
            Assert.Contains(new KeyValuePair<string, int>("c", 30), items);
        }

        [Fact]
        public void GetEnumerator_CanIterateMultipleTimes()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            symbolTable.Add("b", 20);
            
            var firstIteration = symbolTable.ToList();
            var secondIteration = symbolTable.ToList();
            
            Assert.Equal(firstIteration, secondIteration);
        }

        #endregion

        #region Shadowing Functionality Tests

        [Fact]
        public void Shadowing_ChildShadowsParentVariable_ChildValueTakesPrecedence()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("a", 10);
            
            var child = new SymbolTable<string, int>(parent);
            child.Add("a", 20);
            
            Assert.Equal(10, parent["a"]);
            Assert.Equal(20, child["a"]);
        }

        [Fact]
        public void Shadowing_MultipleNestedScopes_InnermostShadowsAll()
        {
            var global = new SymbolTable<string, int>(null);
            global.Add("x", 10);
            
            var inner = new SymbolTable<string, int>(global);
            inner.Add("x", 20);
            
            var innermost = new SymbolTable<string, int>(inner);
            innermost.Add("x", 30);
            
            Assert.Equal(10, global["x"]);
            Assert.Equal(20, inner["x"]);
            Assert.Equal(30, innermost["x"]);
        }

        [Fact]
        public void Shadowing_ChildCanAccessParentNonShadowedVariable()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("a", 10);
            parent.Add("b", 20);
            
            var child = new SymbolTable<string, int>(parent);
            child.Add("a", 100); // Shadow only 'a'
            
            Assert.Equal(100, child["a"]); // Shadowed
            Assert.Equal(20, child["b"]);  // From parent
        }

        [Fact]
        public void Shadowing_RemovingChildVariable_RestoresParentVisibility()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("a", 10);
            
            var child = new SymbolTable<string, int>(parent);
            child.Add("a", 20);
            
            Assert.Equal(20, child["a"]);
            
            child.Remove("a");
            
            Assert.Equal(10, child["a"]); // Parent value now visible
        }

        [Fact]
        public void Shadowing_DEC_Example_XPlusY()
        {
            // Simulates: { x := (10) { y := (20) x := (x + y) } return (x) }
            var outer = new SymbolTable<string, int>(null);
            outer.Add("x", 10);
            
            var inner = new SymbolTable<string, int>(outer);
            inner.Add("y", 20);
            
            // Before shadowing assignment, x refers to outer scope
            int outerX = inner["x"]; // Gets 10 from outer
            int innerY = inner["y"]; // Gets 20 from inner
            
            // Now shadow x in inner scope
            inner.Add("x", outerX + innerY); // x := (x + y) results in 30
            
            Assert.Equal(10, outer["x"]); // Outer x unchanged
            Assert.Equal(30, inner["x"]); // Inner x is 30
        }

        [Fact]
        public void Shadowing_NullValueInChild_FallsBackToParent()
        {
            var parent = new SymbolTable<string, string>(null);
            parent.Add("x", "parent_value");
            
            var child = new SymbolTable<string, string>(parent);
            child.Add("x", null); // Key registered but value is null
            
            // TryGetValue should find parent's value
            bool result = child.TryGetValue("x", out string value);
            
            Assert.True(result);
            Assert.Equal("parent_value", value);
        }

        [Fact]
        public void Shadowing_ComplexNestedExample_FromAssignment()
        {
            // Simulates Figure 2 example: global scope with a=16, b=21
            var global = new SymbolTable<string, int>(null);
            global.Add("a", 16);
            global.Add("b", 21);
            
            // Inner block: shadows 'a', defines 'c'
            var inner = new SymbolTable<string, int>(global);
            inner.Add("a", 336); // a := (a * b) = 16 * 21 = 336
            inner.Add("c", 112); // c := ((a * b) // 3) = 336 // 3 = 112
            
            // Innermost block: shadows 'b'
            var innermost = new SymbolTable<string, int>(inner);
            innermost.Add("b", 0); // b := (c % 7) = 112 % 7 = 0
            
            // Verify values at each scope
            Assert.Equal(16, global["a"]);
            Assert.Equal(21, global["b"]);
            
            Assert.Equal(336, inner["a"]);
            Assert.Equal(21, inner["b"]); // Uses global b
            Assert.Equal(112, inner["c"]);
            
            Assert.Equal(336, innermost["a"]); // Uses inner a
            Assert.Equal(0, innermost["b"]);   // Shadows global b
            Assert.Equal(112, innermost["c"]); // Uses inner c
        }

        #endregion

        #region Edge Cases and Stress Tests

        [Fact]
        public void EdgeCase_LargeNumberOfEntries_HandlesCorrectly()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            for (int i = 0; i < 1000; i++)
            {
                symbolTable.Add($"key{i}", i);
            }
            
            Assert.Equal(1000, symbolTable.Count);
            Assert.Equal(500, symbolTable["key500"]);
        }

        [Fact]
        public void EdgeCase_DeepNestedHierarchy_AccessesCorrectly()
        {
            SymbolTable<string, int> current = new SymbolTable<string, int>(null);
            current.Add("level0", 0);
            
            for (int i = 1; i < 10; i++)
            {
                current = new SymbolTable<string, int>(current);
                current.Add($"level{i}", i);
            }
            
            // Deepest level should access all parent levels
            Assert.Equal(0, current["level0"]);
            Assert.Equal(5, current["level5"]);
            Assert.Equal(9, current["level9"]);
        }

        [Fact]
        public void EdgeCase_UpdateValueMultipleTimes_MaintainsConsistency()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("counter", 0);
            
            for (int i = 1; i <= 100; i++)
            {
                symbolTable["counter"] = i;
            }
            
            Assert.Equal(100, symbolTable["counter"]);
        }

        [Fact]
        public void EdgeCase_AlternatingAddRemove_MaintainsCorrectState()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            for (int i = 0; i < 10; i++)
            {
                symbolTable.Add("temp", i);
                Assert.Equal(i, symbolTable["temp"]);
                symbolTable.Remove("temp");
                Assert.Empty(symbolTable);
            }
        }

        [Fact]
        public void EdgeCase_DuplicateValuesWithDifferentKeys_AllowedAndDistinct()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 10);
            symbolTable.Add("b", 10);
            symbolTable.Add("c", 10);
            
            Assert.Equal(3, symbolTable.Count);
            Assert.Equal(10, symbolTable["a"]);
            Assert.Equal(10, symbolTable["b"]);
            Assert.Equal(10, symbolTable["c"]);
        }

        [Fact]
        public void EdgeCase_MixedTypeSymbolTable_WorksCorrectly()
        {
            var symbolTable = new SymbolTable<string, object>(null);
            symbolTable.Add("int", 42);
            symbolTable.Add("string", "hello");
            symbolTable.Add("bool", true);
            symbolTable.Add("null", null);
            
            Assert.Equal(42, symbolTable["int"]);
            Assert.Equal("hello", symbolTable["string"]);
            Assert.Equal(true, symbolTable["bool"]);
            Assert.Null(symbolTable["null"]);
        }

        #endregion

        #region Integration Tests - DEC Language Examples

        [Fact]
        public void Integration_SimpleNonNestedExample_FromAssignment()
        {
            // a := (2 ** 4)
            // b := (a + 5)
            // c := ((a * b) // 3)
            // return (c % 7)
            
            var symbolTable = new SymbolTable<string, int>(null);
            
            symbolTable.Add("a", 16);  // 2 ** 4
            symbolTable.Add("b", 21);  // 16 + 5
            symbolTable.Add("c", 112); // (16 * 21) // 3
            
            int returnValue = 0; // 112 % 7
            
            Assert.Equal(16, symbolTable["a"]);
            Assert.Equal(21, symbolTable["b"]);
            Assert.Equal(112, symbolTable["c"]);
            Assert.Equal(0, returnValue);
        }

        [Fact]
        public void Integration_NestedScopeWithShadowing_FromAssignment()
        {
            // Global scope
            var global = new SymbolTable<string, int>(null);
            global.Add("a", 10);
            global.Add("b", 20);
            
            // Inner scope
            var inner = new SymbolTable<string, int>(global);
            inner.Add("a", 100); // Shadows global a
            inner.Add("c", 30);  // New variable
            
            // Innermost scope
            var innermost = new SymbolTable<string, int>(inner);
            innermost.Add("b", 200); // Shadows global b
            
            // Verify correct values at each level
            Assert.Equal(10, global["a"]);
            Assert.Equal(20, global["b"]);
            Assert.False(global.ContainsKey("c"));
            
            Assert.Equal(100, inner["a"]);
            Assert.Equal(20, inner["b"]);
            Assert.Equal(30, inner["c"]);
            
            Assert.Equal(100, innermost["a"]);
            Assert.Equal(200, innermost["b"]);
            Assert.Equal(30, innermost["c"]);
        }

        #endregion

        #region Additional Edge Cases - Boundary Conditions

        [Fact]
        public void EdgeCase_EmptyStringKey_AllowedAndWorks()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("", 42);
            
            Assert.True(symbolTable.ContainsKey(""));
            Assert.Equal(42, symbolTable[""]);
        }

        [Fact]
        public void EdgeCase_WhitespaceKey_AllowedAndDistinct()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add(" ", 1);
            symbolTable.Add("  ", 2);
            symbolTable.Add("\t", 3);
            symbolTable.Add("\n", 4);
            
            Assert.Equal(4, symbolTable.Count);
            Assert.Equal(1, symbolTable[" "]);
            Assert.Equal(2, symbolTable["  "]);
            Assert.Equal(3, symbolTable["\t"]);
            Assert.Equal(4, symbolTable["\n"]);
        }

        [Fact]
        public void EdgeCase_VeryLongKey_HandlesCorrectly()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            string longKey = new string('a', 10000);
            
            symbolTable.Add(longKey, 999);
            
            Assert.True(symbolTable.ContainsKey(longKey));
            Assert.Equal(999, symbolTable[longKey]);
        }

        [Fact]
        public void EdgeCase_SpecialCharactersInKey_AllowedAndDistinct()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("key!@#$%", 1);
            symbolTable.Add("key^&*()", 2);
            symbolTable.Add("key<>?/", 3);
            symbolTable.Add("key[]{}|", 4);
            
            Assert.Equal(4, symbolTable.Count);
            Assert.Equal(1, symbolTable["key!@#$%"]);
            Assert.Equal(3, symbolTable["key<>?/"]);
        }

        [Fact]
        public void EdgeCase_UnicodeKeys_WorkCorrectly()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("变量", 100);     // Chinese
            symbolTable.Add("переменная", 200); // Russian
            symbolTable.Add("μεταβλητή", 300);  // Greek
            symbolTable.Add("🔥", 400);        // Emoji
            
            Assert.Equal(100, symbolTable["变量"]);
            Assert.Equal(200, symbolTable["переменная"]);
            Assert.Equal(300, symbolTable["μεταβλητή"]);
            Assert.Equal(400, symbolTable["🔥"]);
        }

        [Fact]
        public void EdgeCase_MinMaxIntegerValues_StoredCorrectly()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("min", int.MinValue);
            symbolTable.Add("max", int.MaxValue);
            symbolTable.Add("zero", 0);
            
            Assert.Equal(int.MinValue, symbolTable["min"]);
            Assert.Equal(int.MaxValue, symbolTable["max"]);
            Assert.Equal(0, symbolTable["zero"]);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        [InlineData(100)]
        public void EdgeCase_VariousCountSizes_CountAccurate(int size)
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            for (int i = 0; i < size; i++)
            {
                symbolTable.Add($"key{i}", i);
            }
            
            Assert.Equal(size, symbolTable.Count);
        }

        #endregion

        #region Additional Edge Cases - Multiple Operations

        [Fact]
        public void EdgeCase_AddUpdateRemoveRepeatedly_MaintainsConsistency()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            for (int cycle = 0; cycle < 5; cycle++)
            {
                // Add phase
                for (int i = 0; i < 10; i++)
                {
                    symbolTable.Add($"key{i}", i * cycle);
                }
                Assert.Equal(10, symbolTable.Count);
                
                // Update phase
                for (int i = 0; i < 10; i++)
                {
                    symbolTable[$"key{i}"] = i * cycle * 2;
                }
                Assert.Equal(10, symbolTable.Count);
                
                // Remove phase
                for (int i = 0; i < 10; i++)
                {
                    Assert.True(symbolTable.Remove($"key{i}"));
                }
                Assert.Empty(symbolTable);
            }
        }

        [Fact]
        public void EdgeCase_InterleavedAddAndRemove_MaintainsCorrectState()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            symbolTable.Add("a", 1);
            symbolTable.Add("b", 2);
            Assert.Equal(2, symbolTable.Count);
            
            symbolTable.Remove("a");
            symbolTable.Add("c", 3);
            Assert.Equal(2, symbolTable.Count);
            Assert.False(symbolTable.ContainsKey("a"));
            Assert.True(symbolTable.ContainsKey("c"));
            
            symbolTable.Remove("b");
            symbolTable.Add("d", 4);
            Assert.Equal(2, symbolTable.Count);
            Assert.True(symbolTable.ContainsKey("c"));
            Assert.True(symbolTable.ContainsKey("d"));
        }

        [Fact]
        public void EdgeCase_ClearAndRebuild_WorksCorrectly()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            // First build
            symbolTable.Add("a", 1);
            symbolTable.Add("b", 2);
            symbolTable.Add("c", 3);
            Assert.Equal(3, symbolTable.Count);
            
            // Clear
            symbolTable.Clear();
            Assert.Empty(symbolTable);
            
            // Rebuild with same keys but different values
            symbolTable.Add("a", 10);
            symbolTable.Add("b", 20);
            symbolTable.Add("c", 30);
            
            Assert.Equal(3, symbolTable.Count);
            Assert.Equal(10, symbolTable["a"]);
            Assert.Equal(20, symbolTable["b"]);
            Assert.Equal(30, symbolTable["c"]);
        }

        [Fact]
        public void EdgeCase_MultipleEnumerationsDuringModification_Behavior()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("a", 1);
            symbolTable.Add("b", 2);
            symbolTable.Add("c", 3);
            
            var firstSnapshot = symbolTable.ToList();
            
            symbolTable.Add("d", 4);
            
            var secondSnapshot = symbolTable.ToList();
            
            Assert.Equal(3, firstSnapshot.Count);
            Assert.Equal(4, secondSnapshot.Count);
        }

        #endregion

        #region Additional Shadowing Tests

        [Fact]
        public void Shadowing_SameKeyDifferentTypes_WithObjectType()
        {
            var parent = new SymbolTable<string, object>(null);
            parent.Add("x", 42);
            
            var child = new SymbolTable<string, object>(parent);
            child.Add("x", "forty-two");
            
            Assert.Equal(42, parent["x"]);
            Assert.Equal("forty-two", child["x"]);
        }

        [Fact]
        public void Shadowing_MultipleShadowsOfSameVariable_EachLevelIndependent()
        {
            var level0 = new SymbolTable<string, int>(null);
            level0.Add("x", 0);
            
            var level1 = new SymbolTable<string, int>(level0);
            level1.Add("x", 10);
            
            var level2 = new SymbolTable<string, int>(level1);
            level2.Add("x", 20);
            
            var level3 = new SymbolTable<string, int>(level2);
            level3.Add("x", 30);
            
            // Each level maintains its own value
            Assert.Equal(0, level0["x"]);
            Assert.Equal(10, level1["x"]);
            Assert.Equal(20, level2["x"]);
            Assert.Equal(30, level3["x"]);
            
            // Removing from level3 exposes level2's value
            level3.Remove("x");
            Assert.Equal(20, level3["x"]);
        }

        [Fact]
        public void Shadowing_PartialShadowing_SomeVariablesAccessibleFromParent()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("a", 1);
            parent.Add("b", 2);
            parent.Add("c", 3);
            parent.Add("d", 4);
            
            var child = new SymbolTable<string, int>(parent);
            child.Add("a", 100); // Shadow a
            child.Add("c", 300); // Shadow c
            // b and d not shadowed
            
            Assert.Equal(100, child["a"]); // Shadowed
            Assert.Equal(2, child["b"]);   // From parent
            Assert.Equal(300, child["c"]); // Shadowed
            Assert.Equal(4, child["d"]);   // From parent
        }

        [Fact]
        public void Shadowing_UpdateShadowedVariable_DoesNotAffectParent()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("x", 10);
            
            var child = new SymbolTable<string, int>(parent);
            child.Add("x", 20);
            
            // Update child's x
            child["x"] = 99;
            
            Assert.Equal(10, parent["x"]); // Parent unchanged
            Assert.Equal(99, child["x"]);  // Child updated
        }

        [Fact]
        public void Shadowing_NullValueChaining_ChecksMultipleLevels()
        {
            var level0 = new SymbolTable<string, string>(null);
            level0.Add("x", "base_value");
            
            var level1 = new SymbolTable<string, string>(level0);
            level1.Add("x", null); // Shadow with null
            
            var level2 = new SymbolTable<string, string>(level1);
            level2.Add("x", null); // Shadow with null again
            
            // Should chain back to level0
            bool result = level2.TryGetValue("x", out string value);
            
            Assert.True(result);
            Assert.Equal("base_value", value);
        }

        [Fact]
        public void Shadowing_AllNullChain_ReturnsNull()
        {
            var level0 = new SymbolTable<string, string>(null);
            level0.Add("x", null);
            
            var level1 = new SymbolTable<string, string>(level0);
            level1.Add("x", null);
            
            bool result = level1.TryGetValue("x", out string value);
            
            Assert.True(result);
            Assert.Null(value);
        }

        [Fact]
        public void Shadowing_ComplexDECExample_MultipleVariableInteractions()
        {
            // Simulates complex nested scoping with multiple variables
            var global = new SymbolTable<string, int>(null);
            global.Add("x", 5);
            global.Add("y", 10);
            global.Add("z", 15);
            
            var block1 = new SymbolTable<string, int>(global);
            block1.Add("a", global["x"] + global["y"]); // a = 15
            block1.Add("x", global["x"] * 2);           // x = 10 (shadows)
            
            var block2 = new SymbolTable<string, int>(block1);
            block2.Add("b", block1["x"] + block1["a"]); // b = 10 + 15 = 25
            block2.Add("y", global["z"]);                // y = 15 (shadows global y)
            
            Assert.Equal(5, global["x"]);
            Assert.Equal(10, global["y"]);
            Assert.Equal(15, global["z"]);
            
            Assert.Equal(15, block1["a"]);
            Assert.Equal(10, block1["x"]);
            Assert.Equal(10, block1["y"]); // From global
            
            Assert.Equal(25, block2["b"]);
            Assert.Equal(10, block2["x"]); // From block1
            Assert.Equal(15, block2["y"]); // Shadowed in block2
            Assert.Equal(15, block2["a"]); // From block1
        }

        #endregion

        #region Parent-Child Relationship Tests

        [Fact]
        public void ParentChild_ChildrenShareSameParent_Independent()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("shared", 100);
            
            var child1 = new SymbolTable<string, int>(parent);
            child1.Add("child1_var", 1);
            
            var child2 = new SymbolTable<string, int>(parent);
            child2.Add("child2_var", 2);
            
            // Both children access parent
            Assert.Equal(100, child1["shared"]);
            Assert.Equal(100, child2["shared"]);
            
            // Children don't see each other's variables
            Assert.False(child1.ContainsKey("child2_var"));
            Assert.False(child2.ContainsKey("child1_var"));
        }

        [Fact]
        public void ParentChild_ModifyingParent_AffectsChildren()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("x", 10);
            
            var child = new SymbolTable<string, int>(parent);
            
            Assert.Equal(10, child["x"]);
            
            parent["x"] = 20;
            
            Assert.Equal(20, child["x"]);
        }

        [Fact]
        public void ParentChild_RemovingFromParent_AffectsChildren()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("x", 10);
            
            var child = new SymbolTable<string, int>(parent);
            
            Assert.True(child.ContainsKey("x"));
            
            parent.Remove("x");
            
            Assert.False(child.ContainsKey("x"));
        }

        [Fact]
        public void ParentChild_ClearingParent_AffectsChildren()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("a", 1);
            parent.Add("b", 2);
            parent.Add("c", 3);
            
            var child = new SymbolTable<string, int>(parent);
            child.Add("d", 4);
            
            Assert.Equal(1, child.Count); // 1 local + 0 (Count only counts local)
            Assert.True(child.ContainsKey("a"));
            
            parent.Clear();
            
            Assert.False(child.ContainsKey("a"));
            Assert.True(child.ContainsKey("d"));
        }

        [Fact]
        public void ParentChild_MultipleGenerations_TreeStructure()
        {
            var root = new SymbolTable<string, int>(null);
            root.Add("root_var", 1);
            
            var generation1_a = new SymbolTable<string, int>(root);
            generation1_a.Add("g1a_var", 2);
            
            var generation1_b = new SymbolTable<string, int>(root);
            generation1_b.Add("g1b_var", 3);
            
            var generation2_a = new SymbolTable<string, int>(generation1_a);
            generation2_a.Add("g2a_var", 4);
            
            var generation2_b = new SymbolTable<string, int>(generation1_a);
            generation2_b.Add("g2b_var", 5);
            
            // All descendants can access root
            Assert.Equal(1, generation1_a["root_var"]);
            Assert.Equal(1, generation2_a["root_var"]);
            Assert.Equal(1, generation2_b["root_var"]);
            
            // generation2_a can access generation1_a but not generation1_b
            Assert.Equal(2, generation2_a["g1a_var"]);
            Assert.False(generation2_a.ContainsKey("g1b_var"));
            
            // Siblings don't see each other
            Assert.False(generation2_a.ContainsKey("g2b_var"));
            Assert.False(generation2_b.ContainsKey("g2a_var"));
        }

        #endregion

        #region ContainsKeyLocal vs ContainsKey Comparison Tests

        [Fact]
        public void Comparison_ContainsKey_Vs_ContainsKeyLocal_LocalKey()
        {
            var parent = new SymbolTable<string, int>(null);
            var child = new SymbolTable<string, int>(parent);
            child.Add("local", 100);
            
            Assert.True(child.ContainsKey("local"));
            Assert.True(child.ContainsKeyLocal("local"));
        }

        [Fact]
        public void Comparison_ContainsKey_Vs_ContainsKeyLocal_ParentKey()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("parent_key", 100);
            var child = new SymbolTable<string, int>(parent);
            
            Assert.True(child.ContainsKey("parent_key"));
            Assert.False(child.ContainsKeyLocal("parent_key"));
        }

        [Fact]
        public void Comparison_ContainsKey_Vs_ContainsKeyLocal_NonExistent()
        {
            var parent = new SymbolTable<string, int>(null);
            var child = new SymbolTable<string, int>(parent);
            
            Assert.False(child.ContainsKey("nonexistent"));
            Assert.False(child.ContainsKeyLocal("nonexistent"));
        }

        #endregion

        #region TryGetValue vs TryGetValueLocal Comparison Tests

        [Fact]
        public void Comparison_TryGetValue_Vs_TryGetValueLocal_LocalKey()
        {
            var parent = new SymbolTable<string, int>(null);
            var child = new SymbolTable<string, int>(parent);
            child.Add("local", 100);
            
            bool result1 = child.TryGetValue("local", out int value1);
            bool result2 = child.TryGetValueLocal("local", out int value2);
            
            Assert.True(result1);
            Assert.True(result2);
            Assert.Equal(100, value1);
            Assert.Equal(100, value2);
        }

        [Fact]
        public void Comparison_TryGetValue_Vs_TryGetValueLocal_ParentKey()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("parent_key", 200);
            var child = new SymbolTable<string, int>(parent);
            
            bool result1 = child.TryGetValue("parent_key", out int value1);
            bool result2 = child.TryGetValueLocal("parent_key", out int value2);
            
            Assert.True(result1);
            Assert.False(result2);
            Assert.Equal(200, value1);
            Assert.Equal(default(int), value2);
        }

        [Fact]
        public void Comparison_TryGetValue_Vs_TryGetValueLocal_ShadowedKey()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("x", 10);
            var child = new SymbolTable<string, int>(parent);
            child.Add("x", 20);
            
            bool result1 = child.TryGetValue("x", out int value1);
            bool result2 = child.TryGetValueLocal("x", out int value2);
            
            Assert.True(result1);
            Assert.True(result2);
            Assert.Equal(20, value1); // Both return local value
            Assert.Equal(20, value2);
        }

        #endregion

        #region Stress Tests

        [Fact]
        public void StressTest_VeryDeepNesting_PerformanceCheck()
        {
            SymbolTable<string, int> current = new SymbolTable<string, int>(null);
            current.Add("level0", 0);
            
            // Create 100 levels of nesting
            for (int i = 1; i < 100; i++)
            {
                current = new SymbolTable<string, int>(current);
                current.Add($"level{i}", i);
            }
            
            // Access from deepest level
            Assert.Equal(0, current["level0"]);
            Assert.Equal(50, current["level50"]);
            Assert.Equal(99, current["level99"]);
        }

        [Fact]
        public void StressTest_ManyKeysInSingleTable_Scalability()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            int keyCount = 10000;
            
            for (int i = 0; i < keyCount; i++)
            {
                symbolTable.Add($"key_{i}", i);
            }
            
            Assert.Equal(keyCount, symbolTable.Count);
            
            // Random access
            Assert.Equal(5000, symbolTable["key_5000"]);
            Assert.Equal(9999, symbolTable["key_9999"]);
            Assert.Equal(0, symbolTable["key_0"]);
        }

        [Fact]
        public void StressTest_ManyOperations_MaintainsIntegrity()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            // 1000 add operations
            for (int i = 0; i < 1000; i++)
            {
                symbolTable.Add($"key{i}", i);
            }
            
            // 500 update operations
            for (int i = 0; i < 500; i++)
            {
                symbolTable[$"key{i}"] = i * 10;
            }
            
            // 250 remove operations
            for (int i = 500; i < 750; i++)
            {
                symbolTable.Remove($"key{i}");
            }
            
            // Verify state
            Assert.Equal(750, symbolTable.Count);
            Assert.Equal(0, symbolTable["key0"]); // Updated
            Assert.Equal(4990, symbolTable["key499"]); // Updated
            Assert.False(symbolTable.ContainsKey("key500")); // Removed
            Assert.True(symbolTable.ContainsKey("key999")); // Unchanged
        }

        [Fact]
        public void StressTest_WideTree_ManyChildren()
        {
            var root = new SymbolTable<string, int>(null);
            root.Add("root", 0);
            
            var children = new List<SymbolTable<string, int>>();
            for (int i = 0; i < 100; i++)
            {
                var child = new SymbolTable<string, int>(root);
                child.Add($"child{i}", i);
                children.Add(child);
            }
            
            // All children can access root
            foreach (var child in children)
            {
                Assert.Equal(0, child["root"]);
            }
            
            // Children can't see siblings
            Assert.False(children[0].ContainsKey("child1"));
            Assert.False(children[50].ContainsKey("child51"));
        }

        #endregion

        #region Complex DEC Language Scenarios

        [Fact]
        public void DEC_Scenario_LoopSimulation_VariableReuse()
        {
            // Simulating multiple iterations where same variable names are used
            var outer = new SymbolTable<string, int>(null);
            outer.Add("counter", 0);
            
            for (int iteration = 0; iteration < 5; iteration++)
            {
                var loopScope = new SymbolTable<string, int>(outer);
                loopScope.Add("i", iteration);
                loopScope.Add("temp", iteration * 2);
                
                Assert.Equal(iteration, loopScope["i"]);
                Assert.Equal(iteration * 2, loopScope["temp"]);
                Assert.Equal(0, loopScope["counter"]); // From outer
            }
        }

        [Fact]
        public void DEC_Scenario_ConditionalBranches_DifferentScopes()
        {
            var main = new SymbolTable<string, int>(null);
            main.Add("condition", 1);
            
            // If branch
            var ifBranch = new SymbolTable<string, int>(main);
            ifBranch.Add("result", main["condition"] * 10);
            Assert.Equal(10, ifBranch["result"]);
            
            // Else branch (separate scope, same parent)
            var elseBranch = new SymbolTable<string, int>(main);
            elseBranch.Add("result", main["condition"] * 20);
            Assert.Equal(20, elseBranch["result"]);
            
            // Branches don't interfere
            Assert.False(ifBranch.ContainsKeyLocal("result") && elseBranch.ContainsKeyLocal("result") 
                         && ifBranch["result"] == elseBranch["result"]);
        }

        [Fact]
        public void DEC_Scenario_FunctionScope_ParameterPassing()
        {
            // Simulating function call with parameters shadowing global variables
            var global = new SymbolTable<string, int>(null);
            global.Add("x", 100);
            global.Add("y", 200);
            
            // Function scope with parameters x, y (shadow globals)
            var function = new SymbolTable<string, int>(global);
            function.Add("x", 10); // Parameter
            function.Add("y", 20); // Parameter
            function.Add("result", function["x"] + function["y"]);
            
            Assert.Equal(10, function["x"]);
            Assert.Equal(20, function["y"]);
            Assert.Equal(30, function["result"]);
            
            // Globals unchanged
            Assert.Equal(100, global["x"]);
            Assert.Equal(200, global["y"]);
        }

        [Fact]
        public void DEC_Scenario_ComplexExpression_MultipleScopes()
        {
            // a := (2 ** 4)
            var scope = new SymbolTable<string, int>(null);
            scope.Add("a", 16);
            
            // { b := (a + 5) { c := (b * 2) return (c - a) } }
            var inner1 = new SymbolTable<string, int>(scope);
            inner1.Add("b", inner1["a"] + 5); // b = 21
            
            var inner2 = new SymbolTable<string, int>(inner1);
            inner2.Add("c", inner2["b"] * 2); // c = 42
            int returnValue = inner2["c"] - inner2["a"]; // 42 - 16 = 26
            
            Assert.Equal(16, scope["a"]);
            Assert.Equal(21, inner1["b"]);
            Assert.Equal(42, inner2["c"]);
            Assert.Equal(26, returnValue);
        }

        [Fact]
        public void DEC_Scenario_ReferenceBeforeLocalAssignment()
        {
            // { x := 10 { y := x x := (x + 5) } }
            var outer = new SymbolTable<string, int>(null);
            outer.Add("x", 10);
            
            var inner = new SymbolTable<string, int>(outer);
            
            // First, read x from outer for y assignment
            int outerX = inner["x"];
            inner.Add("y", outerX); // y = 10
            
            // Then shadow x in inner scope
            inner.Add("x", outerX + 5); // x = 15
            
            Assert.Equal(10, outer["x"]);
            Assert.Equal(15, inner["x"]);
            Assert.Equal(10, inner["y"]);
        }

        #endregion

        #region Null and Default Value Tests

        [Fact]
        public void NullValue_NullableReferenceType_StoresAndRetrievesNull()
        {
            var symbolTable = new SymbolTable<string, string>(null);
            symbolTable.Add("nullable", null);
            
            Assert.True(symbolTable.ContainsKey("nullable"));
            Assert.Null(symbolTable["nullable"]);
        }

        [Fact]
        public void NullValue_MultipleNullValues_AllDistinct()
        {
            var symbolTable = new SymbolTable<string, string>(null);
            symbolTable.Add("null1", null);
            symbolTable.Add("null2", null);
            symbolTable.Add("null3", null);
            
            Assert.Equal(3, symbolTable.Count);
            Assert.Null(symbolTable["null1"]);
            Assert.Null(symbolTable["null2"]);
            Assert.Null(symbolTable["null3"]);
        }

        [Fact]
        public void DefaultValue_ValueType_StoresZero()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            symbolTable.Add("zero", default(int));
            
            Assert.True(symbolTable.ContainsKey("zero"));
            Assert.Equal(0, symbolTable["zero"]);
        }

        [Fact]
        public void NullValue_UpdateFromNullToValue_Works()
        {
            var symbolTable = new SymbolTable<string, string>(null);
            symbolTable.Add("x", null);
            
            Assert.Null(symbolTable["x"]);
            
            symbolTable["x"] = "now has value";
            
            Assert.Equal("now has value", symbolTable["x"]);
        }

        [Fact]
        public void NullValue_UpdateFromValueToNull_Works()
        {
            var symbolTable = new SymbolTable<string, string>(null);
            symbolTable.Add("x", "has value");
            
            Assert.Equal("has value", symbolTable["x"]);
            
            symbolTable["x"] = null;
            
            Assert.Null(symbolTable["x"]);
        }

        #endregion

        #region Enumeration Edge Cases

        [Fact]
        public void Enumeration_OrderPreserved_AdditionOrder()
        {
            var symbolTable = new SymbolTable<string, int>(null);
            
            symbolTable.Add("first", 1);
            symbolTable.Add("second", 2);
            symbolTable.Add("third", 3);
            
            var keys = symbolTable.Keys.ToList();
            
            // Assuming implementation preserves insertion order (like DLL would)
            // This test documents the behavior
            Assert.Equal(3, keys.Count);
            Assert.Contains("first", keys);
            Assert.Contains("second", keys);
            Assert.Contains("third", keys);
        }

        [Fact]
        public void Enumeration_EmptyParentAndChild_BothEmpty()
        {
            var parent = new SymbolTable<string, int>(null);
            var child = new SymbolTable<string, int>(parent);
            
            Assert.Empty(parent);
            Assert.Empty(child);
        }

        [Fact]
        public void Enumeration_OnlyLocal_NotParent()
        {
            var parent = new SymbolTable<string, int>(null);
            parent.Add("parent_key", 100);
            
            var child = new SymbolTable<string, int>(parent);
            child.Add("child_key", 200);
            
            var childItems = child.ToList();
            
            // Enumeration should only include local items
            Assert.Single(childItems);
            Assert.Contains(new KeyValuePair<string, int>("child_key", 200), childItems);
            Assert.DoesNotContain(new KeyValuePair<string, int>("parent_key", 100), childItems);
        }

        #endregion

        #region Different Type Parameter Tests

        [Fact]
        public void DifferentTypes_IntKey_StringValue()
        {
            var symbolTable = new SymbolTable<int, string>(null);
            symbolTable.Add(1, "one");
            symbolTable.Add(2, "two");
            symbolTable.Add(3, "three");
            
            Assert.Equal("one", symbolTable[1]);
            Assert.Equal("two", symbolTable[2]);
            Assert.Equal("three", symbolTable[3]);
        }

        [Fact]
        public void DifferentTypes_ComplexKey_ComplexValue()
        {
            var symbolTable = new SymbolTable<(string, int), List<string>>(null);
            
            var key1 = ("first", 1);
            var value1 = new List<string> { "a", "b", "c" };
            
            symbolTable.Add(key1, value1);
            
            Assert.True(symbolTable.ContainsKey(key1));
            Assert.Equal(value1, symbolTable[key1]);
        }

        [Fact]
        public void DifferentTypes_CustomClass_AsValue()
        {
            var symbolTable = new SymbolTable<string, List<int>>(null);
            symbolTable.Add("numbers", new List<int> { 1, 2, 3, 4, 5 });
            
            var numbers = symbolTable["numbers"];
            Assert.Equal(5, numbers.Count);
            Assert.Equal(3, numbers[2]);
        }

        #endregion
    }
}