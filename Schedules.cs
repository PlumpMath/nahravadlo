using System;
using System.Collections.Generic;
using Microsoft.Win32.TaskScheduler;

namespace Nahravadlo
{
	public class Schedules
	{
		public const string NEW_TASK_PREFIX = TASK_FOLDER_NAME + " - ";
		public const string OLD_TASK_PREFIX = "Nahrávání - ";
		public const string TASK_FOLDER_NAME = "Nahrávadlo";
		private readonly TaskFolder taskFolder;
		private readonly TaskService taskService;

		private readonly string vlcFilename;
		private readonly string workingDirectory;

		public Schedules(string vlcFilename, string workingDirectory)
		{
			this.vlcFilename = vlcFilename;
			this.workingDirectory = workingDirectory;
			taskService = new TaskService();

			//U Task Service 2.0 budeme naplanovane nahravani vkladat do vlastni slozky
			taskFolder = taskService.RootFolder;
			if (Version == JobVersion.V2)
				taskFolder = !FolderExist(taskFolder, TASK_FOLDER_NAME) ? taskFolder.CreateFolder(TASK_FOLDER_NAME, null) : taskFolder.SubFolders[TASK_FOLDER_NAME];
		}

		public Job this[string name]
		{
			get { return Get(name); }
		}

		public JobVersion Version
		{
			get { return taskService.HighestSupportedVersion == new Version(1, 2) ? JobVersion.V2 : JobVersion.V1; }
		}

		public Job Create(string name)
		{
			var definition = taskService.NewTask();

			//Task Scheduler 2 musi obsahovat alespon jednu akci
			//definition.Actions.Add(new ExecAction("dummy", "", ""));

			//U Task Scheduler 2.0 se jiz nepouziva prefix, je to rozliseno vlastni slozkou
			if (Version == JobVersion.V1)
				name = NEW_TASK_PREFIX + name;

			//Task t = taskFolder.RegisterTaskDefinition(name, definition);
			//if (t == null) throw new JobNotCreatedException();
			return new Job(definition, name, taskFolder, Version, vlcFilename, workingDirectory);
		}

		public Job Get(string name)
		{
			Task t = null;
			if (Version == JobVersion.V2)
			{
				if (TaskExist(taskFolder, name))
					t = taskFolder.Tasks[name];
			}
			else
			{
				if (TaskExist(taskFolder, NEW_TASK_PREFIX + name))
					t = taskFolder.Tasks[NEW_TASK_PREFIX + name];
				else if (TaskExist(taskFolder, OLD_TASK_PREFIX + name))
					t = taskFolder.Tasks[OLD_TASK_PREFIX + name];
			}
			if (t == null)
				throw new JobNotFoundException();
			return new Job(t, taskFolder, Version, vlcFilename, workingDirectory);
		}

		public bool Exist(string name)
		{
			return (Version == JobVersion.V2 && TaskExist(taskFolder, name)) || TaskExist(taskFolder, NEW_TASK_PREFIX + name) || TaskExist(taskFolder, OLD_TASK_PREFIX + name);
		}

		private static bool TaskExist(TaskFolder parent, string name)
		{
			var tasks = parent.Tasks.GetEnumerator();
			while (tasks.MoveNext())
			{
				if (tasks.Current.Name.Equals(name))
					return true;
			}
			return false;
		}

		private static bool FolderExist(TaskFolder parent, string name)
		{
			var folders = parent.SubFolders.GetEnumerator();
			while (folders.MoveNext())
			{
				if (folders.Current.Name.Equals(name))
					return true;
			}
			return false;
		}

		public List<Job> GetAll()
		{
			var list = new List<Job>();
			foreach (var task in taskFolder.Tasks)
			{
				var taskName = task.Name;
				try
				{
					if (Version == JobVersion.V2)
						list.Add(Get(taskName));
					else if (taskName.StartsWith(NEW_TASK_PREFIX))
						list.Add(Get(taskName.Substring(NEW_TASK_PREFIX.Length)));
					else if (taskName.StartsWith(OLD_TASK_PREFIX))
						list.Add(Get(taskName.Substring(OLD_TASK_PREFIX.Length)));
				}
				catch (JobNotFoundException) {}
			}
			return list;
		}

		public List<string> GetAllNames()
		{
			var list = new List<string>();
			foreach (var task in taskFolder.Tasks)
			{
				var taskName = task.Name;
				if (Version == JobVersion.V2)
					list.Add(taskName);
				else if (taskName.StartsWith(NEW_TASK_PREFIX))
					list.Add(DeleteJobExt(taskName.Substring(NEW_TASK_PREFIX.Length)));
				else if (taskName.StartsWith(OLD_TASK_PREFIX))
					list.Add(DeleteJobExt(taskName.Substring(OLD_TASK_PREFIX.Length)));
			}
			return list;
		}

		public void Remove(string name)
		{
			if (Version == JobVersion.V2 && TaskExist(taskFolder, name))
				taskFolder.DeleteTask(name);
			else if (TaskExist(taskFolder, NEW_TASK_PREFIX + name))
				taskFolder.DeleteTask(NEW_TASK_PREFIX + name);
			else if (TaskExist(taskFolder, OLD_TASK_PREFIX + name))
				taskFolder.DeleteTask(OLD_TASK_PREFIX + name);
		}

		/*public static void Remove(string name)
        {
            using (var st = new ScheduledTasks())
            {
                if (!st.DeleteTask(NEW_TASK_PREFIX + name))
                {
                    if (!st.DeleteTask(OLD_TASK_PREFIX + name))
                        throw new JobNotFoundException();
                }
            }
        }*/

		public void Dispose()
		{
			taskService.Dispose();
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

	public enum JobVersion
	{
		V1,
		V2
	}
}