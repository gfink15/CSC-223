using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Containers;

namespace Utilities.Containers.Tests
{
    /// <summary>
    /// Comprehensive unit tests for the DLL (Doubly Linked List) class.
    /// Tests cover all public methods, properties, edge cases, and exception handling.
    /// </summary>
    public class DLLTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_CreatesEmptyList()
        {
            // Arrange & Act
            var list = new DLL<int>();

            // Assert
            Assert.Equal(0, list.Count);
            Assert.True(list.IsEmpty());
        }

        [Fact]
        public void Constructor_InitializesSentinelNodes()
        {
            // Arrange & Act
            var list = new DLL<string>();

            // Assert
            Assert.True(list.IsEmpty());
            Assert.Equal(0, list.Size());
        }

        #endregion

        #region Size and Count Tests

        [Fact]
        public void Size_EmptyList_ReturnsZero()
        {
            // Arrange
            var list = new DLL<int>();

            // Assert
            Assert.Equal(0, list.Size());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void Size_AfterAddingElements_ReturnsCorrectCount(int count)
        {
            // Arrange
            var list = new DLL<int>();

            // Act
            for (int i = 0; i < count; i++)
            {
                list.Add(i);
            }

            // Assert
            Assert.Equal(count, list.Size());
        }

        [Fact]
        public void Count_MatchesSize()
        {
            // Arrange
            var list = new DLL<int>();

            // Act
            list.Add(1);
            list.Add(2);
            list.Add(3);

            // Assert
            Assert.Equal(list.Size(), list.Count);
        }

        #endregion

        #region IsEmpty Tests

        [Fact]
        public void IsEmpty_NewList_ReturnsTrue()
        {
            // Arrange
            var list = new DLL<int>();

            // Assert
            Assert.True(list.IsEmpty());
        }

        [Fact]
        public void IsEmpty_AfterAddingElement_ReturnsFalse()
        {
            // Arrange
            var list = new DLL<int>();

            // Act
            list.Add(1);

            // Assert
            Assert.False(list.IsEmpty());
        }

        [Fact]
        public void IsEmpty_AfterClear_ReturnsTrue()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);

            // Act
            list.Clear();

            // Assert
            Assert.True(list.IsEmpty());
        }

        #endregion

        #region PushFront Tests

        [Fact]
        public void PushFront_SingleElement_AddsToFront()
        {
            // Arrange
            var list = new DLL<int>();

            // Act
            list.PushFront(10);

            // Assert
            Assert.Equal(1, list.Count);
            Assert.Equal(10, list.Front());
        }

        [Fact]
        public void PushFront_MultipleElements_MaintainsOrder()
        {
            // Arrange
            var list = new DLL<int>();

            // Act
            list.PushFront(1);
            list.PushFront(2);
            list.PushFront(3);

            // Assert
            Assert.Equal(3, list.Front());
            Assert.Equal(1, list.Back());
            Assert.Equal(3, list.Count);
        }

        #endregion

        #region PushBack Tests

        [Fact]
        public void PushBack_SingleElement_AddsToBack()
        {
            // Arrange
            var list = new DLL<string>();

            // Act
            list.PushBack("test");

            // Assert
            Assert.Equal(1, list.Count);
            Assert.Equal("test", list.Back());
        }

        [Fact]
        public void PushBack_MultipleElements_MaintainsOrder()
        {
            // Arrange
            var list = new DLL<int>();

            // Act
            list.PushBack(1);
            list.PushBack(2);
            list.PushBack(3);

            // Assert
            Assert.Equal(1, list.Front());
            Assert.Equal(3, list.Back());
            Assert.Equal(3, list.Count);
        }

        #endregion

        #region Front Tests

        [Fact]
        public void Front_EmptyList_ThrowsInvalidOperationException()
        {
            // Arrange
            var list = new DLL<int>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => list.Front());
        }

        [Fact]
        public void Front_SingleElement_ReturnsElement()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(42);

            // Act
            var result = list.Front();

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public void Front_MultipleElements_ReturnsFirstElement()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            // Act
            var result = list.Front();

            // Assert
            Assert.Equal(1, result);
        }

        #endregion

        #region Back Tests

        [Fact]
        public void Back_EmptyList_ThrowsInvalidOperationException()
        {
            // Arrange
            var list = new DLL<int>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => list.Back());
        }

        [Fact]
        public void Back_SingleElement_ReturnsElement()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(42);

            // Act
            var result = list.Back();

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public void Back_MultipleElements_ReturnsLastElement()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            // Act
            var result = list.Back();

            // Assert
            Assert.Equal(3, result);
        }

        #endregion

        #region PopFront Tests

        [Fact]
        public void PopFront_EmptyList_ThrowsInvalidOperationException()
        {
            // Arrange
            var list = new DLL<int>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => list.PopFront());
        }

        [Fact]
        public void PopFront_SingleElement_ReturnsAndRemovesElement()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(42);

            // Act
            var result = list.PopFront();

            // Assert
            Assert.Equal(42, result);
            Assert.Equal(0, list.Count);
            Assert.True(list.IsEmpty());
        }

        [Fact]
        public void PopFront_MultipleElements_RemovesFirstElement()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            // Act
            var result = list.PopFront();

            // Assert
            Assert.Equal(1, result);
            Assert.Equal(2, list.Count);
            Assert.Equal(2, list.Front());
        }

        #endregion

        #region PopBack Tests

        [Fact]
        public void PopBack_EmptyList_ThrowsInvalidOperationException()
        {
            // Arrange
            var list = new DLL<int>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => list.PopBack());
        }

        [Fact]
        public void PopBack_SingleElement_ReturnsAndRemovesElement()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(42);

            // Act
            var result = list.PopBack();

            // Assert
            Assert.Equal(42, result);
            Assert.Equal(0, list.Count);
            Assert.True(list.IsEmpty());
        }

        [Fact]
        public void PopBack_MultipleElements_RemovesLastElement()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            // Act
            var result = list.PopBack();

            // Assert
            Assert.Equal(3, result);
            Assert.Equal(2, list.Count);
            Assert.Equal(2, list.Back());
        }

        #endregion

        #region Add Tests

        [Fact]
        public void Add_SingleElement_IncreasesCount()
        {
            // Arrange
            var list = new DLL<int>();

            // Act
            list.Add(10);

            // Assert
            Assert.Equal(1, list.Count);
        }

        [Fact]
        public void Add_MultipleElements_AddsToEnd()
        {
            // Arrange
            var list = new DLL<int>();

            // Act
            list.Add(1);
            list.Add(2);
            list.Add(3);

            // Assert
            Assert.Equal(3, list.Count);
            Assert.Equal(3, list.Back());
        }

        [Fact]
        public void Add_DuplicateElements_AllowsDuplicates()
        {
            // Arrange
            var list = new DLL<int>();

            // Act
            list.Add(5);
            list.Add(5);
            list.Add(5);

            // Assert
            Assert.Equal(3, list.Count);
        }

        #endregion

        #region Insert Tests

        [Fact]
        public void Insert_AtBeginning_AddsToFront()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(2);
            list.Add(3);

            // Act
            list.Insert(0, 1);

            // Assert
            Assert.Equal(3, list.Count);
            Assert.Equal(1, list[0]);
            Assert.Equal(2, list[1]);
        }

        [Fact]
        public void Insert_AtEnd_AddsToBack()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);

            // Act
            list.Insert(2, 3);

            // Assert
            Assert.Equal(3, list.Count);
            Assert.Equal(3, list[2]);
        }

        [Fact]
        public void Insert_InMiddle_ShiftsElements()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(3);

            // Act
            list.Insert(1, 2);

            // Assert
            Assert.Equal(3, list.Count);
            Assert.Equal(1, list[0]);
            Assert.Equal(2, list[1]);
            Assert.Equal(3, list[2]);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(5)]
        public void Insert_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => list.Insert(index, 99));
        }

        [Fact]
        public void Insert_AtCountIndex_AddsToEnd()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);

            // Act
            list.Insert(list.Count, 3);

            // Assert
            Assert.Equal(3, list.Count);
            Assert.Equal(3, list.Back());
        }

        #endregion

        #region Contains Tests

        [Fact]
        public void Contains_EmptyList_ReturnsFalse()
        {
            // Arrange
            var list = new DLL<int>();

            // Act
            var result = list.Contains(1);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(5, true)]
        [InlineData(10, false)]
        public void Contains_VariousElements_ReturnsCorrectResult(int value, bool expected)
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);
            list.Add(5);

            // Act
            var result = list.Contains(value);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Contains_DuplicateElements_ReturnsTrue()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(5);
            list.Add(5);
            list.Add(5);

            // Act
            var result = list.Contains(5);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_NullInReferenceTypeList_HandlesCorrectly()
        {
            // Arrange
            var list = new DLL<string>();
            list.Add("test");
            list.Add(null);

            // Act
            var result = list.Contains(null);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region IndexOf Tests

        [Fact]
        public void IndexOf_EmptyList_ReturnsNegativeOne()
        {
            // Arrange
            var list = new DLL<int>();

            // Act
            var result = list.IndexOf(1);

            // Assert
            Assert.Equal(-1, result);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 1)]
        [InlineData(3, 2)]
        [InlineData(99, -1)]
        public void IndexOf_VariousElements_ReturnsCorrectIndex(int value, int expectedIndex)
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            // Act
            var result = list.IndexOf(value);

            // Assert
            Assert.Equal(expectedIndex, result);
        }

        [Fact]
        public void IndexOf_DuplicateElements_ReturnsFirstOccurrence()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);
            list.Add(2);
            list.Add(3);

            // Act
            var result = list.IndexOf(2);

            // Assert
            Assert.Equal(1, result);
        }

        #endregion

        #region Remove Tests

        [Fact]
        public void Remove_EmptyList_ReturnsFalse()
        {
            // Arrange
            var list = new DLL<int>();

            // Act
            var result = list.Remove(1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Remove_ExistingElement_RemovesAndReturnsTrue()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            // Act
            var result = list.Remove(2);

            // Assert
            Assert.True(result);
            Assert.Equal(2, list.Count);
            Assert.False(list.Contains(2));
        }

        [Fact]
        public void Remove_NonExistingElement_ReturnsFalse()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);

            // Act
            var result = list.Remove(99);

            // Assert
            Assert.False(result);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public void Remove_DuplicateElements_RemovesFirstOccurrence()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(5);
            list.Add(5);
            list.Add(5);

            // Act
            var result = list.Remove(5);

            // Assert
            Assert.True(result);
            Assert.Equal(2, list.Count);
            Assert.Equal(5, list[0]);
            Assert.Equal(5, list[1]);
        }

        #endregion

        #region RemoveAt Tests

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void RemoveAt_EmptyListOrInvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            // Arrange
            var list = new DLL<int>();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => list.RemoveAt(index));
        }

        [Fact]
        public void RemoveAt_ValidIndex_RemovesElement()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            // Act
            list.RemoveAt(1);

            // Assert
            Assert.Equal(2, list.Count);
            Assert.Equal(1, list[0]);
            Assert.Equal(3, list[1]);
        }

        [Fact]
        public void RemoveAt_FirstElement_RemovesCorrectly()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            // Act
            list.RemoveAt(0);

            // Assert
            Assert.Equal(2, list.Count);
            Assert.Equal(2, list.Front());
        }

        [Fact]
        public void RemoveAt_LastElement_RemovesCorrectly()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            // Act
            list.RemoveAt(2);

            // Assert
            Assert.Equal(2, list.Count);
            Assert.Equal(2, list.Back());
        }

        #endregion

        #region Clear Tests

        [Fact]
        public void Clear_EmptyList_RemainsEmpty()
        {
            // Arrange
            var list = new DLL<int>();

            // Act
            list.Clear();

            // Assert
            Assert.Equal(0, list.Count);
            Assert.True(list.IsEmpty());
        }

        [Fact]
        public void Clear_PopulatedList_RemovesAllElements()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            // Act
            list.Clear();

            // Assert
            Assert.Equal(0, list.Count);
            Assert.True(list.IsEmpty());
        }

        [Fact]
        public void Clear_AfterClear_CanAddNewElements()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Clear();

            // Act
            list.Add(2);

            // Assert
            Assert.Equal(1, list.Count);
            Assert.Equal(2, list.Front());
        }

        #endregion

        #region Indexer Tests

        [Fact]
        public void Indexer_Get_ReturnsCorrectElement()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(10);
            list.Add(20);
            list.Add(30);

            // Act & Assert
            Assert.Equal(10, list[0]);
            Assert.Equal(20, list[1]);
            Assert.Equal(30, list[2]);
        }

        [Fact]
        public void Indexer_Set_UpdatesElement()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(10);
            list.Add(20);
            list.Add(30);

            // Act
            list[1] = 99;

            // Assert
            Assert.Equal(99, list[1]);
            Assert.Equal(3, list.Count);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(5)]
        public void Indexer_Get_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => list[index]);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(5)]
        public void Indexer_Set_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => list[index] = 99);
        }

        #endregion

        #region CopyTo Tests

        [Fact]
        public void CopyTo_NullArray_ThrowsArgumentNullException()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => list.CopyTo(null, 0));
        }

        [Fact]
        public void CopyTo_NegativeIndex_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            var array = new int[5];

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => list.CopyTo(array, -1));
        }

        [Fact]
        public void CopyTo_InsufficientSpace_ThrowsArgumentException()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            var array = new int[3];

            // Act & Assert
            Assert.Throws<ArgumentException>(() => list.CopyTo(array, 2));
        }

        [Fact]
        public void CopyTo_ValidParameters_CopiesElements()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            var array = new int[5];

            // Act
            list.CopyTo(array, 1);

            // Assert
            Assert.Equal(0, array[0]);
            Assert.Equal(1, array[1]);
            Assert.Equal(2, array[2]);
            Assert.Equal(3, array[3]);
            Assert.Equal(0, array[4]);
        }

        [Fact]
        public void CopyTo_EmptyList_DoesNotModifyArray()
        {
            // Arrange
            var list = new DLL<int>();
            var array = new int[] { 1, 2, 3 };

            // Act
            list.CopyTo(array, 0);

            // Assert
            Assert.Equal(new int[] { 1, 2, 3 }, array);
        }

        #endregion

        #region IEnumerable Tests

        [Fact]
        public void GetEnumerator_EmptyList_NoIterations()
        {
            // Arrange
            var list = new DLL<int>();
            var count = 0;

            // Act
            foreach (var item in list)
            {
                count++;
            }

            // Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public void GetEnumerator_PopulatedList_IteratesInOrder()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            var expected = new[] { 1, 2, 3 };
            var actual = new List<int>();

            // Act
            foreach (var item in list)
            {
                actual.Add(item);
            }

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetEnumerator_WorksWithLinq()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);

            // Act
            var evenNumbers = list.Where(x => x % 2 == 0).ToList();

            // Assert
            Assert.Equal(new[] { 2, 4 }, evenNumbers);
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_EmptyList_ReturnsEmptyRepresentation()
        {
            // Arrange
            var list = new DLL<int>();

            // Act
            var result = list.ToString();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ToString_PopulatedList_ReturnsStringRepresentation()
        {
            // Arrange
            var list = new DLL<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);

            // Act
            var result = list.ToString();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        #endregion

        #region IsReadOnly Tests

        [Fact]
        public void IsReadOnly_ReturnsFalse()
        {
            // Arrange
            var list = new DLL<int>();

            // Assert
            Assert.False(list.IsReadOnly);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void IntegrationTest_MixedOperations_MaintainsCorrectState()
        {
            // Arrange
            var list = new DLL<int>();

            // Act & Assert - Build up list
            list.PushFront(2);
            list.PushBack(3);
            list.PushFront(1);
            Assert.Equal(3, list.Count);
            Assert.Equal(1, list.Front());
            Assert.Equal(3, list.Back());

            // Remove from front
            var front = list.PopFront();
            Assert.Equal(1, front);
            Assert.Equal(2, list.Count);

            // Insert in middle
            list.Insert(1, 99);
            Assert.Equal(3, list.Count);
            Assert.Equal(99, list[1]);

            // Remove specific item
            list.Remove(99);
            Assert.Equal(2, list.Count);
            Assert.False(list.Contains(99));

            // Clear and verify
            list.Clear();
            Assert.True(list.IsEmpty());
        }

        [Fact]
        public void IntegrationTest_LargeDataSet_PerformsCorrectly()
        {
            // Arrange
            var list = new DLL<int>();
            const int count = 1000;

            // Act - Add many elements
            for (int i = 0; i < count; i++)
            {
                list.Add(i);
            }

            // Assert
            Assert.Equal(count, list.Count);
            Assert.Equal(0, list.Front());
            Assert.Equal(count - 1, list.Back());

            // Verify some middle elements
            Assert.Equal(500, list[500]);
            Assert.True(list.Contains(750));

            // Remove half the elements
            for (int i = 0; i < count / 2; i++)
            {
                list.RemoveAt(0);
            }

            Assert.Equal(count / 2, list.Count);
            Assert.Equal(500, list.Front());
        }

        [Fact]
        public void IntegrationTest_WithStrings_WorksCorrectly()
        {
            // Arrange
            var list = new DLL<string>();

            // Act
            list.Add("apple");
            list.Add("banana");
            list.Add("cherry");
            list.PushFront("aardvark");

            // Assert
            Assert.Equal(4, list.Count);
            Assert.Equal("aardvark", list.Front());
            Assert.Equal("cherry", list.Back());
            Assert.True(list.Contains("banana"));
            Assert.Equal(2, list.IndexOf("banana"));
        }

        [Fact]
        public void IntegrationTest_WithDuplicates_HandlesCorrectly()
        {
            // Arrange
            var list = new DLL<int>();

            // Act
            list.Add(5);
            list.Add(10);
            list.Add(5);
            list.Add(15);
            list.Add(5);

            // Assert
            Assert.Equal(5, list.Count);
            Assert.Equal(0, list.IndexOf(5)); // First occurrence
            
            // Remove first occurrence
            list.Remove(5);
            Assert.Equal(4, list.Count);
            Assert.Equal(1, list.IndexOf(5)); // Now at index 1
        }

        #endregion
    }
}