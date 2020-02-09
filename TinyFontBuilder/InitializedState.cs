using System;
using System.ComponentModel;

namespace Terka
{
    internal class InitializedState : IDisposable
    {
        private ISupportInitialize _target;

        private InitializedState(ISupportInitialize target)
        {
            _target = target;
            _target.BeginInit();
        }

        public void Dispose()
        {
            _target.EndInit();
        }

        public static InitializedState Of(ISupportInitialize target)
        {
            return new InitializedState(target);
        }
    }
}
