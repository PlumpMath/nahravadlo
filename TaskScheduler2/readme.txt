Zmeny o proti puvodnimu Task Scheduler 2.0 wraperu z: http://www.codeplex.com/taskscheduler

1. Podpora spousteni ulohy pod jinym uctem (TaskFolder.RegisterTaskDefinition)
--------------------------------------------------------------------------------
public Task RegisterTaskDefinition(string Path, TaskDefinition definition, TaskCreation createType, string UserId, string password, TaskLogonType LogonType, string sddl)
{
	if (v2Folder != null)
		return new Task(v2Folder.RegisterTaskDefinition(Path, definition.v2Def, (int)createType, UserId, password, LogonType, sddl));

		TaskFlags flags = definition.v1Task.GetFlags();

		IntPtr pwd;
		switch (LogonType)
		{
			case TaskLogonType.Group:
			case TaskLogonType.S4U:
			case TaskLogonType.None:
				throw new NotV1SupportedException("This LogonType is not supported on Task Scheduler 1.0.");
			case TaskLogonType.InteractiveToken:
				flags |= TaskFlags.RunOnlyIfLoggedOn;
				flags |= TaskFlags.Interactive;
				if (String.IsNullOrEmpty(UserId)) UserId = WindowsIdentity.GetCurrent().Name;
				definition.v1Task.SetAccountInformation(UserId, IntPtr.Zero);
				break;
			case TaskLogonType.ServiceAccount:
				if (String.IsNullOrEmpty(UserId)) definition.v1Task.SetAccountInformation(String.Empty, IntPtr.Zero);
				else definition.v1Task.SetAccountInformation(UserId, IntPtr.Zero);
				break;
			case TaskLogonType.InteractiveTokenOrPassword:
				pwd = Marshal.StringToCoTaskMemUni(password);
				definition.v1Task.SetAccountInformation(UserId, pwd);
				Marshal.FreeCoTaskMem(pwd);
				flags |= TaskFlags.Interactive;
				break;
			case TaskLogonType.Password:
				pwd = Marshal.StringToCoTaskMemUni(password);
				definition.v1Task.SetAccountInformation(UserId, pwd);
				Marshal.FreeCoTaskMem(pwd);
				break;
			default:
				break;
		}
		definition.v1Task.SetFlags(flags);

	switch (createType)
	{
		case TaskCreation.Create:
		case TaskCreation.CreateOrUpdate:
		case TaskCreation.Disable:
		case TaskCreation.Update:
			if (createType == TaskCreation.Disable)
				definition.Settings.Enabled = false;
			definition.V1Save(Path);
			break;
		case TaskCreation.DontAddPrincipalAce:
			throw new NotV1SupportedException("Security settings are not available on Task Scheduler 1.0.");
		case TaskCreation.IgnoreRegistrationTriggers:
			throw new NotV1SupportedException("Registration triggers are not available on Task Scheduler 1.0.");
		case TaskCreation.ValidateOnly:
			throw new NotV1SupportedException("Xml validation not available on Task Scheduler 1.0.");
		default:
			break;
	}
	return new Task(definition.v1Task);
}
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

2. Moznost vytvoreni ulohy s prazdnym trigerem (TaskDefinition.V1Save)
--------------------------------------------------------------------------------
nahradit:
  this.triggers.Bind();
za
  if (triggers != null)
		this.triggers.Bind();
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

3. Umozneni smazani ulohy, pokud jiz neni naplanovana (TaskSettings.)
--------------------------------------------------------------------------------
public TimeSpan DeleteExpiredTaskAfter
{
	get
	{
		if (v2Settings != null)
			return Task.StringToTimeSpan(v2Settings.DeleteExpiredTaskAfter);
			return (v1Task.GetFlags() & V1Interop.TaskFlags.DeleteWhenDone) == V1Interop.TaskFlags.DeleteWhenDone ? TimeSpan.FromSeconds(1) : TimeSpan.Zero;
	}
	set
	{
		if (v2Settings != null)
			v2Settings.DeleteExpiredTaskAfter = Task.TimeSpanToString(value);
		else
		{
			V1Interop.TaskFlags flags = v1Task.GetFlags();
			if (value >= TimeSpan.FromSeconds(1))
				v1Task.SetFlags(flags |= V1Interop.TaskFlags.DeleteWhenDone);
			else
				v1Task.SetFlags(flags &= ~V1Interop.TaskFlags.DeleteWhenDone);
		}
	}
}