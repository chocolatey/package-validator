// Copyright © 2015 - Present RealDimensions Software, LLC
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// 
// You may obtain a copy of the License at
// 
// 	http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace chocolatey.package.validator.infrastructure.synchronization
{
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Threading;

    /// <summary>
    ///   Class providing transaction locks.
    /// </summary>
    public class TransactionLock
    {
        private static readonly ConcurrentDictionary<string, TransactionLockObject> _lockDictionary =
            new ConcurrentDictionary<string, TransactionLockObject>();
        private const int DEFAULT_SECONDS = 120;
        private static int _activeLocks;
        private static object _localLock = new object();

        /// <summary>
        ///   Enters the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        public static void enter(string name, Action action)
        {
            enter(name, DEFAULT_SECONDS, action);
        }

        /// <summary>
        ///   Enters lock setting  the specified seconds to timeout.
        /// </summary>
        /// <param name="name">The name of the object to use with locking</param>
        /// <param name="secondsToTimeout">The seconds to timeout.</param>
        /// <param name="action">The action.</param>
        public static void enter(string name, int? secondsToTimeout, Action action)
        {
            bool lockTaken = false;
            try
            {
                lockTaken = acquire(name, secondsToTimeout);
                if (lockTaken) action.Invoke();
                else
                {
                    throw new ApplicationException("Was not able to Acquire a lock on '{0}' within '{1}' seconds.".format_with(name, secondsToTimeout));
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                release(name, lockTaken);
            }
        }

        /// <summary>
        ///   Enters the specified name.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="func">The function to be used.</param>
        /// <returns>The result of entering the lock</returns>
        public static TResult enter<TResult>(string name, Func<TResult> func)
        {
            return enter(name, DEFAULT_SECONDS, func);
        }

        /// <summary>
        ///   Enters lock setting the specified seconds to timeout.
        /// </summary>
        /// <param name="name">The name of the object to use with locking</param>
        /// <param name="secondsToTimeout">The seconds to timeout.</param>
        /// <param name="func">The action.</param>
        /// <typeparam name="TResult">The Type to Enter</typeparam>
        /// <returns>The result of entering the lock</returns>
        public static TResult enter<TResult>(string name, int? secondsToTimeout, Func<TResult> func)
        {
            bool lockTaken = false;
            try
            {
                lockTaken = acquire(name, secondsToTimeout);
                if (lockTaken) return func.Invoke();
                else
                {
                    throw new ApplicationException(
                        "Was not able to Acquire a lock on '{0}' within '{1}' seconds.".format_with(name, secondsToTimeout));
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                release(name, lockTaken);
            }
        }

        /// <summary>
        ///   Acquires the  lock with the specified seconds to timeout.
        /// </summary>
        /// <param name="name">The name of the lock</param>
        /// <param name="secondsToTimeout">The seconds to timeout.</param>
        /// <returns>True if the lock was acquired</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static bool acquire(string name, int? secondsToTimeout)
        {
            "TransactionLock".Log().Debug("Attempting to Enter lock for {0}".format_with(name));
            var lockingObject = get_lock_object(name);
            bool lockTaken = false;

            if (secondsToTimeout.HasValue)
            {
                Monitor.TryEnter(
                    lockingObject,
                    (int)TimeSpan.FromSeconds(secondsToTimeout.GetValueOrDefault(0)).TotalMilliseconds,
                    ref lockTaken);
            }
            else Monitor.Enter(lockingObject, ref lockTaken);

            if (lockTaken)
            {
                _activeLocks += 1;
                "TransactionLock".Log().Debug("Entered lock for '{0}'. There are {1} active locks.".format_with(name, _activeLocks));
                return true;
            }

            return false;
        }

        /// <summary>
        ///   Releases locks.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="lockTaken">The lock Taken.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void release(string name, bool lockTaken)
        {
            if (_localLock == null) _localLock = new object();

            try
            {
                "TransactionLock".Log().Debug("Exiting lock for '{0}'".format_with(name));
                lock (_localLock)
                {
                    if (lockTaken)
                    {
                        var lockingObject = get_lock_object(name);
                        Monitor.Pulse(lockingObject);
                        Monitor.Exit(lockingObject);
                        _activeLocks -= 1;
                        "TransactionLock".Log().Debug(
                            "Exited lock for '{0}'. There are {1} active locks".format_with(
                                name,
                                _activeLocks));
                    }

                    Monitor.Pulse(_localLock);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                "TransactionLock".Log().Warn(
                    "An error occurred when releasing lock for '{0}':{1}{2}".format_with(
                        name,
                        Environment.NewLine,
                        ex));
            }
        }

        /// <summary>
        ///   Kills this instance.
        /// </summary>
        /// <param name="name">The name.</param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void kill(string name)
        {
            try
            {
                acquire(name, 2);
            }
            finally
            {
                release(name, true);
            }
        }

        /// <summary>
        ///   Gets the lock object by the name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The locked object</returns>
        private static TransactionLockObject get_lock_object(string name)
        {
            var lockObj = new TransactionLockObject(name);

            return _lockDictionary.GetOrAdd(name, lockObj);
        }
    }
}
