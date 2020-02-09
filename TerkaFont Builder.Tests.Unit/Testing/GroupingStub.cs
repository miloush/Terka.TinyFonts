namespace Terka.FontBuilder.Testing
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// <see cref="IGrouping{X,Y}"/> for testing purposes.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TElement">The type of the elements.</typeparam>
    public class GroupingStub<TKey, TElement> : List<TElement>, IGrouping<TKey, TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupingStub{TKey,TElement}" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public GroupingStub(TKey key)
        {
            this.Key = key;
        }

        /// <inheritdoc />
        public TKey Key { get; private set; }
    }
}
