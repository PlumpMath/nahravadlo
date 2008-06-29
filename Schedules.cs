using System;
using System.Collections.Generic;
using TaskScheduler;

namespace Nahravadlo
{
    public class Schedules
    {
        private const string NEW_TASK_PREFIX = "Nahr�vadlo - ";
        private const string OLD_TASK_PREFIX = "Nahr�v�n� - ";
        private readonly ScheduledTasks scheduledTasks;

        private readonly string vlcFilename;
        private readonly string workingDirectory;

        public Schedules(string vlcFilename, string workingDirectory)
        {
            this.vlcFilename = vlcFilename;
            this.workingDirectory = workingDirectory;
            scheduledTasks = new ScheduledTasks();
        }

        public Job this[string name]
        {
            get { return get(name); }
        }

        public Job create(string name)
        {
            Task t = scheduledTasks.CreateTask(NEW_TASK_PREFIX + name);
            if (t == null) throw new JobNotCreatedException();
            return new Job(t, vlcFilename, workingDirectory);
        }

        public Job get(string name)
        {
            Task t = scheduledTasks.OpenTask(NEW_TASK_PREFIX + name) ?? scheduledTasks.OpenTask(OLD_TASK_PREFIX + name);
            if (t == null) throw new JobNotFoundException();
            return new Job(t, vlcFilename, workingDirectory);
        }

        public bool exist(String name)
        {
            Task t = scheduledTasks.OpenTask(NEW_TASK_PREFIX + name) ?? scheduledTasks.OpenTask(OLD_TASK_PREFIX + name);

            if (t == null) return false;

            t.Close();

            return true;
        }

        public List<Job> getAll()
        {
            var list = new List<Job>();
            foreach (String taskName in scheduledTasks.GetTaskNames())
            {
                try
                {
                    if (taskName.StartsWith(NEW_TASK_PREFIX))
                        list.Add(get(taskName.Substring(NEW_TASK_PREFIX.Length)));
                    else if (taskName.StartsWith(OLD_TASK_PREFIX))
                        list.Add(get(taskName.Substring(OLD_TASK_PREFIX.Length)));
                }
                catch (JobNotFoundException) {}
            }
            return list;
        }

        public List<string> getAllNames()
        {
            var list = new List<string>();
            foreach (String taskName in scheduledTasks.GetTaskNames())
            {
                if (taskName.StartsWith(NEW_TASK_PREFIX))
                    list.Add(DeleteJobExt(taskName.Substring(NEW_TASK_PREFIX.Length)));
                else if (taskName.StartsWith(OLD_TASK_PREFIX))
                    list.Add(DeleteJobExt(taskName.Substring(OLD_TASK_PREFIX.Length)));
            }
            return list;
        }

        public void remove(string name)
        {
            if (!scheduledTasks.DeleteTask(NEW_TASK_PREFIX + name))
            {
                if (!scheduledTasks.DeleteTask(OLD_TASK_PREFIX + name))
                    throw new JobNotFoundException();
            }
        }

        public static void Remove(string name)
        {
            using (var st = new ScheduledTasks())
            {
                if (!st.DeleteTask(NEW_TASK_PREFIX + name))
                {
                    if (!st.DeleteTask(OLD_TASK_PREFIX + name))
                        throw new JobNotFoundException();
                }
            }
        }

        public void Dispose()
        {
            scheduledTasks.Dispose();
        }

        private static string DeleteJobExt(string filename)
        {
            if (filename.EndsWith(".job"))
                return filename.Substring(0, filename.Length - 4);
            return filename;
        }
    }

    internal class JobNotCreatedException : Exception
    {
        public JobNotCreatedException() {}
        public JobNotCreatedException(string message) : base(message) {}
        public JobNotCreatedException(string message, Exception inner) : base(message, inner) {}
    }

    internal class JobNotFoundException : Exception
    {
        public JobNotFoundException() {}
        public JobNotFoundException(string message) : base(message) {}
        public JobNotFoundException(string message, Exception inner) : base(message, inner) {}
    }
}