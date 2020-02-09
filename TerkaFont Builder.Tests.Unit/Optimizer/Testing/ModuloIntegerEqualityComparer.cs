namespace Terka.FontBuilder.Optimizer.Testing
{
    using System.Collections.Generic;

    public class ModuloIntegerEqualityComparer : IEqualityComparer<int>
    {
        private readonly int divisor;

        // This is used to introduce conflics
        private readonly int numberOfClasses;

        public ModuloIntegerEqualityComparer(int divisor, int numberOfClasses = 999)
        {
            this.divisor = divisor;
            this.numberOfClasses = numberOfClasses;
        }

        public bool Equals(int x, int y)
        {
            return x % this.divisor == y % this.divisor;
        }

        public int GetHashCode(int obj)
        {
            return (obj % this.divisor) % this.numberOfClasses;
        }
    }
}
