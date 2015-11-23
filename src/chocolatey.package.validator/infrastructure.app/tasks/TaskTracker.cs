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

namespace chocolatey.package.validator.infrastructure.app.tasks
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using synchronization;

    /// <summary>
    ///   Used for tracking tasks
    /// </summary>
    public class TaskTracker
    {
        private const string TASK_TRACKER_NAME = "TaskTracker_31235688345";
        private static readonly Lazy<ConcurrentDictionary<string, int>> _activeTasks =
            new Lazy<ConcurrentDictionary<string, int>>(() => new ConcurrentDictionary<string, int>());
        private static int _activeTasksCount;

        /// <summary>
        ///   Notifies the tracker of work beginning for a task
        /// </summary>
        /// <param name="taskName">Name of the task.</param>
        public static void work_begin(string taskName)
        {
            "TaskTracker".Log().Debug("Starting work for {0}".format_with(taskName));
            TransactionLock.enter(
                TASK_TRACKER_NAME,
                () =>
                {
                    _activeTasksCount += 1;
                    _activeTasks.Value.AddOrUpdate(taskName, 1, (key, oldValue) => oldValue + 1);
                });
        }

        /// <summary>
        ///   Notifies the tracker work has completed
        /// </summary>
        /// <param name="taskName">Name of the task.</param>
        public static void work_complete(string taskName)
        {
            "TaskTracker".Log().Debug("Completing work for {0}".format_with(taskName));
            TransactionLock.enter(
                TASK_TRACKER_NAME,
                () =>
                {
                    _activeTasksCount -= 1;
                    var taskCount = 0;
                    _activeTasks.Value.TryRemove(taskName, out taskCount);
                    if (taskCount != 0)
                    {
                        taskCount -= 1;
                        _activeTasks.Value.AddOrUpdate(taskName, taskCount, (key, oldValue) => taskCount);
                    }
                });
        }

        /// <summary>
        ///   Gets the active tasks.
        /// </summary>
        /// <returns>The IEnumerable of active tasks.</returns>
        public static IEnumerable<string> get_active_tasks()
        {
            IList<string> activeTasks = new List<string>();

            TransactionLock.enter(
                TASK_TRACKER_NAME,
                () =>
                {
                    foreach (var task in _activeTasks.Value)
                    {
                        activeTasks.Add(task.Key);
                    }
                });

            return activeTasks;
        }

        /// <summary>
        ///   Determines if there are active tasks running.
        /// </summary>
        /// <returns>A boolean value</returns>
        public static bool are_active_tasks()
        {
            bool activeTasksRunning = true;

            TransactionLock.enter(TASK_TRACKER_NAME, 1, () => { if (_activeTasksCount == 0) activeTasksRunning = false; });

            return activeTasksRunning;
        }
    }
}
