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
    }
}