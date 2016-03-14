using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tortuga.Anchor
{
    /// <summary>
    /// Helper methods for working with various types of locks.
    /// </summary>
    public static class LockUtilities
    {
        /// <summary>
        /// Acquires a read lock on the indicated ReaderWriterLockSlim.
        /// </summary>
        /// <param name="lock">The lock to be acequired.</param>
        /// <remarks>To not use this in an enviorment where Thread Abort exceptions are possible, as it may lead to dead locks.</remarks>
        public static IDisposable Read(this ReaderWriterLockSlim @lock)
        {
            return new ReaderWriterLockSlimReadToken(@lock);
        }

        /// <summary>
        /// Acquires a write lock on the indicated ReaderWriterLockSlim.
        /// </summary>
        /// <param name="lock">The lock to be acequired.</param>
        /// <remarks>To not use this in an enviorment where Thread Abort exceptions are possible, as it may lead to dead locks.</remarks>
        public static IDisposable Write(this ReaderWriterLockSlim @lock)
        {
            return new ReaderWriterLockSlimWriteToken(@lock);
        }

        /// <summary>
        /// Acquires a semaphore as a blocking operation. When this is disposed, the semaphore will be released.
        /// </summary>
        /// <param name="semaphore">The semaphore.</param>
        /// <returns>IDisposable.</returns>
        public static IDisposable Acquire(this SemaphoreSlim semaphore)
        {
            semaphore.Wait();
            return new SemaphoreSlimToken(semaphore);
        }

        /// <summary>
        /// Acquires a semaphore as an asynchronous operation. When this is disposed, the semaphore will be released.
        /// </summary>
        /// <param name="semaphore">The semaphore.</param>
        /// <returns>Task&lt;IDisposable&gt;.</returns>
        public static async Task<IDisposable> AcquireAsync(this SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            return new SemaphoreSlimToken(semaphore);
        }

        sealed class SemaphoreSlimToken : IDisposable
        {
            private SemaphoreSlim m_Semaphore;

            public SemaphoreSlimToken(SemaphoreSlim semaphore)
            {
                m_Semaphore = semaphore;
            }

            public void Dispose()
            {
                if (m_Semaphore == null)
                    return;

                m_Semaphore.Release();
                m_Semaphore = null;
            }
        }

        sealed class ReaderWriterLockSlimReadToken : IDisposable
        {
            private ReaderWriterLockSlim m_Lock;

            public ReaderWriterLockSlimReadToken(ReaderWriterLockSlim @lock)
            {
                m_Lock = @lock;
                m_Lock.EnterReadLock();
            }

            public void Dispose()
            {
                if (m_Lock == null)
                    return;

                m_Lock.ExitReadLock();
                m_Lock = null;
            }
        }

        sealed class ReaderWriterLockSlimWriteToken : IDisposable
        {
            private ReaderWriterLockSlim m_Lock;
            public ReaderWriterLockSlimWriteToken(ReaderWriterLockSlim @lock)
            {
                m_Lock = @lock;
                m_Lock.EnterWriteLock();
            }

            public void Dispose()
            {
                if (m_Lock == null)
                    return;

                m_Lock.ExitWriteLock();
                m_Lock = null;
            }
        }
    }
}
