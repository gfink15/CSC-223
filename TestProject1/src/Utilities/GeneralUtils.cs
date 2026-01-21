/**
 * GeneralUtils Class: Provides a series of useful methods
 * to use as utilities in other programs.
 * Claude AI was only used for writing XML and inline comments.
 * 
 * @author Graham Fink
 * @date   1/20/2026
 */

namespace Utilities;
public class GeneralUtils
{
    /// <summary>
    /// Checks if an array contains a specific item using equality comparison.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array</typeparam>
    /// <param name="arr">The array to search</param>
    /// <param name="target">The item to search for</param>
    /// <returns>True if the array contains the target, false otherwise</returns>
    public static bool Contains<T>(T[] arr, T target)
    {
        // Iterate through each element in the array
        for (int i = 0; i < arr.Length; i++)
        {
            // Use EqualityComparer to check if current element equals target
            if (EqualityComparer<T>.Default.Equals(arr[i],(target))) return true;
        }
        // Target not found in array
        return false;
    }

    /// <summary>
    /// Returns a string with spaces for the specified indentation level.
    /// </summary>
    /// <param name="level">The indentation level (4 spaces per level)</param>
    /// <returns>A string containing 4 * level spaces</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when level is negative</exception>
    public static string GetIndentation(int level)
    {
        // Validate that level is non-negative
        if (level < 0) throw new ArgumentOutOfRangeException("Level must be positive");
        
        string indentation = "";
        // Build indentation string by adding 4 spaces per level
        for (int i = 0; i < level; i++)
        {
            indentation += "    ";
        }
        return indentation;
    }

    /// <summary>
    /// Checks if a string contains only lowercase letters.
    /// </summary>
    /// <param name="name">The string to validate</param>
    /// <returns>True if the string contains only lowercase letters, false otherwise</returns>
    public static bool IsValidVariable(string name)
    {
        // Empty strings are not valid
        if (name.Equals("")) return false;
        
        // Check each character to ensure it's a lowercase letter
        foreach (char c in name)
        {
            // Character must be between 'a' and 'z' inclusive
            if (c > 'z' || c < 'a')
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Checks if a string is a valid mathematical operator.
    /// </summary>
    /// <param name="op">The string to validate</param>
    /// <returns>True if the string is one of: +, -, *, /, //, %, **</returns>
    public static bool IsValidOperator(string op)
    {
        // Check if operator matches any of the valid operators
        if (op.Equals("+") || op.Equals("-") || op.Equals("*") || op.Equals("/") || 
            op.Equals("//") || op.Equals("%") || op.Equals("**"))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Counts how many times a given character appears in a string.
    /// </summary>
    /// <param name="s">The string to search</param>
    /// <param name="c">The character to count</param>
    /// <returns>The number of occurrences of the character in the string</returns>
    public static int CountOccurrences(string s, char c)
    {
        int numberOfCharacters = 0;
        
        // Iterate through each character in the string
        foreach (char d in s)
        {
            // Increment counter if character matches
            if (d.Equals(c)) numberOfCharacters++;
        }
        return numberOfCharacters;
    }

    /// <summary>
    /// Converts space-separated words to camelCase format.
    /// The first character is lowercase, spaces are removed, and the first letter 
    /// after each space is capitalized.
    /// </summary>
    /// <param name="s">The space-separated string to convert</param>
    /// <returns>The string in camelCase format</returns>
    /// <example>
    /// "Hello world test string" returns "helloWorldTestString"
    /// </example>
    public static string ToCamelCase(string s)
    {
        string result = "";
        bool capitalizeNextLetter = false;
        bool firstLetter = true;
        
        // Process each character in the input string
        foreach (char c in s)
        {
            // Skip spaces but mark that next letter should be capitalized
            if (c.Equals(' '))
            {
                capitalizeNextLetter = true;
                continue;
            }
            
            // Capitalize if this follows a space
            if (capitalizeNextLetter && !firstLetter)
            {
                result += char.ToUpper(c);
            }
            else 
            {
                // Lowercase the character
                result += char.ToLower(c);
                firstLetter = false;
            }
            capitalizeNextLetter = false;
        }
        return result;
    }

    /// <summary>
    /// Checks if a password meets strength requirements.
    /// A strong password must be at least 8 characters long and contain at least 
    /// one lowercase letter, one uppercase letter, one digit, and one special character.
    /// </summary>
    /// <param name="pwd">The password to validate</param>
    /// <returns>True if the password meets all strength requirements, false otherwise</returns>
    public static bool IsPasswordStrong(string pwd)
    {
        // Track whether each requirement is met
        bool lengthReq = false;
        bool lowerReq = false;
        bool upperReq = false;
        bool digitReq = false;
        bool otherReq = false;

        // Check length requirement (at least 8 characters)
        if (pwd.Length >= 8) lengthReq = true;

        // Check each character for character type requirements
        foreach (char c in pwd)
        {
            if (c >= 'a' && c <= 'z') lowerReq = true;
            else if (c >= 'A' && c <= 'Z') upperReq = true;
            else if (c >= '0' && c <= '9') digitReq = true;
            else if (c != ' ') otherReq = true; // Special character
            
            // Early exit if all requirements are met
            if (lengthReq && lowerReq && upperReq && digitReq && otherReq) return true;
        }
        return false;
    }

    /// <summary>
    /// Returns a new list containing only unique items from the given list.
    /// The original list is not modified.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list</typeparam>
    /// <param name="list">The list to extract unique items from</param>
    /// <returns>A new list containing only unique items</returns>
    /// <exception cref="ArgumentException">Thrown when the list is null</exception>
    public static List<T> GetUniqueItems<T>(List<T> list)
    {
        // Validate input is not null
        if (list == null) throw new ArgumentException("List cannot be null");
        
        List<T> results = new List<T>();
        bool unique;
        
        // Check each item in the original list
        foreach (T item in list)
        {
            unique = true;
            
            // See if this item is already in the results list
            foreach (T result in results)
            {
                if (EqualityComparer<T>.Default.Equals(result, item))
                {
                    unique = false;
                }
            }
            
            // Add item to results only if it's unique
            if (unique) results.Add(item);
        }
        return results;
    }

    /// <summary>
    /// Calculates the average value of an array of integers.
    /// </summary>
    /// <param name="numbers">The array of integers to average</param>
    /// <returns>The average as a double</returns>
    /// <exception cref="ArgumentException">Thrown when the array is null or empty</exception>
    public static double CalculateAverage(int[] numbers)
    {
        // Validate input is not null
        if (numbers == null) throw new ArgumentException("Input cannot be null");
        // Validate input is not empty
        if (numbers.Length == 0) throw new ArgumentException("Input array must have one or more items");
        
        int count = 0;
        double sum = 0.0;
        
        // Sum all numbers and count them
        foreach (int n in numbers)
        {
            sum += n;
            count++;
        }
        
        // Return the average
        return (sum / count);
    }

    /// <summary>
    /// Returns the set of all items from an array that appear more than once.
    /// Uses a dictionary to track occurrence counts.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array</typeparam>
    /// <param name="array">The array to search for duplicates</param>
    /// <returns>An array containing all items that appear more than once</returns>
    public static T[] Duplicates<T>(T[] array) 
    {
        // Create dictionary to count occurrences of each item
        Dictionary<T, int> dict = new Dictionary<T, int>();
        
        // Count occurrences of each item in the array
        foreach (T item in array)
        {
            if (dict.ContainsKey(item)) dict[item]++;
            else dict[item] = 1;
        }
        
        // Remove items that only appear once (not duplicates)
        foreach (KeyValuePair<T, int> key in dict)
        {
            if (key.Value == 1) dict.Remove(key.Key);
        }
        
        // Build output array from remaining dictionary keys
        T[] output = new T[dict.Count];
        int count = 0;
        foreach (KeyValuePair<T, int> key in dict)
        {
            output[count] = key.Key;
            count++;
        }
        return output;
    }
}