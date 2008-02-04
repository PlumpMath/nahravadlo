using System;
using System.Collections.Generic;
using TaskScheduler;

namespace Nahravadlo
{
	public class Schedules
	{
		private const string OLD_TASK_PREFIX = "Nahrávání - ";
		private const string NEW_TASK_PREFIX = "Nahrávadlo - ";

		private string vlcFilename;
		private string workingDirectory;
		private ScheduledTasks scheduledTasks;

		public Schedules(string vlcFilename, string workingDirectory)
		{
			this.vlcFilename = vlcFilename;
			this.workingDirectory = workingDirectory;
			scheduledTasks = new ScheduledTasks();
		}

		public Job create(string name)
		{
			Task t = scheduledTasks.CreateTask(NEW_TASK_PREFIX + name);
			if (t == null) throw new JobNotCreatedException();
			return new Job(t, vlcFilename, workingDirectory);
		}

		public Job get(string name)
		{
			Task t = scheduledTasks.OpenTask(NEW_TASK_PREFIX + name);
			if (t == null)
				t = scheduledTasks.OpenTask(OLD_TASK_PREFIX + name);
			if (t == null) throw new JobNotFoundException();
			return new Job(t, vlcFilename, workingDirectory);
		}

		public bool exist(String name)
		{
			Task t = scheduledTasks.OpenTask(NEW_TASK_PREFIX + name);

			if (t == null)
				t = scheduledTasks.OpenTask(OLD_TASK_PREFIX + name);
			if (t == null) return false;

			t.Close();

			return true;
		}

		public Job this[string name]
		{
			get { return get(name); }
		}

		public List<Job> getAll()
		{
			List<Job> list = new List<Job>();
			foreach(String taskName in scheduledTasks.GetTaskNames())
			{
				try
				{
					if (taskName.StartsWith(NEW_TASK_PREFIX))
						list.Add(get(taskName.Substring(NEW_TASK_PREFIX.Length)));
					else if (taskName.StartsWith(OLD_TASK_PREFIX))
						list.Add(get(taskName.Substring(OLD_TASK_PREFIX.Length)));
				} catch(JobNotFoundException) {}
			}
			return list;
		}

		public List<string> getAllNames()
		{
			List<string> list = new List<string>();
			foreach(String taskName in scheduledTasks.GetTaskNames())
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
				if (!scheduledTasks.DeleteTask(OLD_TASK_PREFIX + name))
					throw new JobNotFoundException();
		}

		public static void Remove(string name)
		{
			using(ScheduledTasks st = new ScheduledTasks())
			{
				if (!st.DeleteTask(NEW_TASK_PREFIX + name))
					if (!st.DeleteTask(OLD_TASK_PREFIX + name))
						throw new JobNotFoundException();
			}
		}

		public void Dispose()
		{
			scheduledTasks.Dispose();
		}

		private string DeleteJobExt(string filename)
		{
			if (filename.EndsWith(".job"))
				return filename.Substring(0, filename.Length - 4);
			return filename;
		}
	}

	internal class JobNotCreatedException : Exception
	{
		public JobNotCreatedException() : base() {}
		public JobNotCreatedException(string message) : base(message) {}
		public JobNotCreatedException(string message, Exception inner) : base(message, inner) {}
	}

	internal class JobNotFoundException : Exception
	{
		public JobNotFoundException() : base() {}
		public JobNotFoundException(string message) : base(message) {}
		public JobNotFoundException(string message, Exception inner) : base(message, inner) {}
	}
}