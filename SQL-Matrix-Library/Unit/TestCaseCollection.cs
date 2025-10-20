using System.Collections;

namespace Matrix.MsSql.Unit
{
    /// <summary>
    /// This class provides properties and methods to represent a collection of zero or more test cases.
    /// </summary>
    public class TestCaseCollection : IReadOnlyList<TestCase>
    {
        /// <summary>
        /// Internal list of all test cases.
        /// </summary>
        private readonly List<TestCase> _TestCases;

        /// <summary>
        /// Initialize a new <see cref="TestCaseCollection"/> set.
        /// </summary>
        /// <remarks>This constructor is for internal use only.</remarks>
        internal TestCaseCollection(List<TestCase> cases)
        {
            _TestCases = cases;
        }

        #region Implementations

        #region IReadOnlyList

        /// <summary>
        /// Gets the test case at the specified index of this collection.
        /// </summary>
        /// <param name="index">The zero based index.</param>
        /// <returns>Returns the <see cref="TestCase"/>-instance at the requested index.</returns>
        public TestCase this[int index]
        {
            get => _TestCases[index];
        }

        /// <summary>
        /// Gets the number of test case of this collection.
        /// </summary>
        /// <value>The number of test cases in this collection.</value>
        public int Count
        {
            get => _TestCases.Count;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<TestCase> GetEnumerator()
        {
            return ((IEnumerable<TestCase>)_TestCases).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_TestCases).GetEnumerator();
        }

        #endregion

        #endregion
    }
}