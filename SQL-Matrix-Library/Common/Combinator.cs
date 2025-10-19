using System.Collections;

namespace Matrix.MsSql.Common
{
    /// <summary>
    /// Represents a utility for generating and managing multidimensional combinations of items.
    /// </summary>
    /// <remarks>The <see cref="Combinator"/> class allows you to define multiple dimensions, each with a
    /// specified number of items, and then generate all possible combinations of those items across the dimensions.
    /// </remarks>
    public class Combinator : IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Combinator"/> class.
        /// </summary>
        public Combinator()
        {
        }

        private int[] _Dimensions = [];

        /// <summary>
        /// Gets the maximum number of items across all dimensions.
        /// </summary>
        public int MaxItemsCount
        {
            get
            {
                return _Dimensions.Length == 0 ? 0 : _Dimensions.Max();
            }
        }

        /// <summary>
        /// Gets the total number of combinations based on the dimensions provided.
        /// </summary>
        public int TotalCombinations
        {
            get
            {
                if (_Dimensions.Length == 0)
                {
                    return 0;
                }

                int total = 1;
                foreach (int count in _Dimensions)
                {
                    total *= count;
                }
                return total;
            }
        }

        /// <summary>
        /// Adds a new dimension to the collection with the specified number of items.
        /// </summary>
        /// <param name="ItemsCount">The number of items in the new dimension. Must be greater than zero.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="ItemsCount"/> is less than 1.</exception>
        public void AddDimension(int ItemsCount)
        {
            if (ItemsCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(ItemsCount), "Number of items must be greater than zero.");
            }
            _Dimensions = [.. _Dimensions, ItemsCount];
        }

        /// <summary>
        /// Generates all possible combinations of values across multiple dimensions,  constrained by the maximum number
        /// of items per dimension.
        /// </summary>
        /// <remarks>This method calculates combinations based on the specified dimensions and their
        /// respective limits. Each combination is represented as an array of integers, where each integer corresponds
        /// to a value in a specific dimension. The method ensures that all generated combinations are valid within the
        /// defined constraints.</remarks>
        /// <returns>A jagged array of integers, where each inner array represents a valid combination of values across the
        /// dimensions. Returns an empty array if no dimensions are defined or if the total number of combinations is
        /// zero.</returns>
        public int[][] GetCombinations()
        {
            // Falls keine Dimension angegeben wurde eine leere Liste zurückgeben
            if (TotalCombinations == 0)
            {
                return [];
            }

            int[][] combinations = [];
            long[] significants = new long[_Dimensions.Length];

            // Wertigkeiten für jede Dimension berechnen
            significants[0] = 1;
            for (int i = 1; i < significants.Length; i++)
            {
                significants[i] = significants[i - 1] * MaxItemsCount;
            }

            // Kombinationen berechnen
            // Die maximale Anzahl an Kombinationen ist gleich der höchsten Wertigkeit um eine Potenz erhöht minus 1
            for (long i = 0; i < significants[significants.Length - 1] * MaxItemsCount; i++)
            {
                int[] combination = new int[_Dimensions.Length];
                long mod = i; // Restwert; Mit der auszuwertenden Zahl beginnen

                for (int dim = 0; dim < _Dimensions.Length - 1; dim++)
                {
                    // Mit der höchsten Wertigkeit beginnen
                    combination[dim] = (int)(mod / significants[significants.Length - dim - 1]);

                    // Fortsetzung nur, wenn der Wert innerhalb der Dimension liegt
                    if (combination[dim] + 1 > _Dimensions[dim])
                    {
                        break;
                    }

                    //Restwert für die nächste Dimension berechnen
                    mod = mod % significants[significants.Length - dim - 1];
               }

                // Für die letzte Dimension den Restwert verwenden
                combination[_Dimensions.Length - 1] = (int)mod;

                // Kombination nur hinzufügen, wenn alle Dimensionen gültig sind
                if (combination[_Dimensions.Length - 1] + 1 > _Dimensions[_Dimensions.Length - 1])
                {
                    continue;
                }
                combinations = [.. combinations, combination];
            }

            return combinations;
        }

        #region Implementations
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the combinations generated by this instance.
        /// </summary>
        /// <remarks>The enumerator provides sequential access to the combinations generated by the
        /// <c>GetCombinations</c> method. Ensure that the source data used to generate combinations remains unchanged
        /// while iterating.</remarks>
        /// <returns>An instance of <see cref="CombinatorEnumerator"/> that can be used to iterate through the combinations.</returns>
        public CombinatorEnumerator GetEnumerator()
        {
            return new CombinatorEnumerator(this.GetCombinations());
        }
        #endregion
    }

    /// <summary>
    /// Encapsulates an enumerator for iterating through combinations generated by the <see cref="Combinator"/> class.
    /// </summary>
    public class CombinatorEnumerator : IEnumerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CombinatorEnumerator"/> class with the specified combinations.
        /// </summary>
        /// <remarks>The <paramref name="combinations"/> parameter must not be null. Each inner array
        /// should represent a valid combination of elements.</remarks>
        /// <param name="combinations">A two-dimensional array representing the combinations to be enumerated. Each inner array represents a single
        /// combination.</param>
        public CombinatorEnumerator(int[][] combinations)
        {
            _combinations = combinations;
        }

        public int[][] _combinations;
        private int _position = -1;

        /// <summary>
        /// Advances the enumerator to the next element in the collection.
        /// </summary>
        /// <returns><see langword="true"/> if the enumerator successfully advanced to the next element; otherwise, <see
        /// langword="false"/> if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            _position++;
            return (_position < _combinations.Length);
        }

        /// <summary>
        /// Resets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <remarks>After calling this method, you must call <see cref="MoveNext"/> to advance the
        /// enumerator to the first element of the collection before reading the value of the current element.</remarks>
        public void Reset()
        {
            _position = -1;
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public object Current
        {
            get
            {
                try
                {
                    return _combinations[_position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
