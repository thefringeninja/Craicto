using System;
using System.Threading;
using SqlStreamStore;

namespace Craicto.Pipes.Example
{
    public static class UnitOfWork
    {
        private static readonly AsyncLocal<IUnitOfWork> s_current = new AsyncLocal<IUnitOfWork>();

        public static IUnitOfWork Current => s_current.Value;

        public static IUnitOfWork Start(Guid commitId, IStreamStore streamStore)
        {
            if (s_current.Value != null)
            {
                throw new InvalidOperationException();
            }

            s_current.Value = new UnitOfWorkImpl(commitId, streamStore, Complete);

            return s_current.Value;
        }

        private static void Complete() => s_current.Value = null;
    }
}