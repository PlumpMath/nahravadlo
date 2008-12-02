using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Win32.TaskScheduler;

namespace Nahravadlo
{
    /// <summary>
    /// Seznam stavu, ktere muze nabivat uloha
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        /// Uloha je pripravena ke spusteni v zadany cas
        /// </summary>
        Ready = TaskState.Ready,
        /// <summary>
        /// Uloha prave bezi.
        /// </summary>
        Running = TaskState.Running,
        
        ///// <summary>
        ///// Uloha neni jiz naplanovana. 
        ///// </summary>
        //NotScheduled = TaskState.Unknown,
        
        /// <summary>
        /// Uloha je zakazana.
        /// </summary>
        Disabled = TaskState.Disabled,
        
        ///// <summary>
        ///// Uloha byla nasilne ukoncena.
        ///// </summary>
        //Terminated = TaskState.Terminated,
        
        /// <summary>
        /// Stav neni znamy.
        /// </summary>
        Unknown = TaskState.Unknown
    }

    /// <summary>
    /// Reprezentuje ulohu nahravani
    /// </summary>
    public class Job : IDisposable
    {
        private Task task;
        private String taskName;
        private readonly TaskDefinition definition;
        private readonly TaskFolder folder;
        private readonly JobVersion version;
        
        private readonly string vlcFilename;
        private readonly string workingDirectory;

        /// <summary>
        /// Konstruktor objektu
        /// </summary>
        /// <param name="task">Naplanovana uloha</param>
        /// <param name="folder">Misto ulozeni naplanovane ulohy</param>
        /// <param name="version">Verze naplanovane ulohy</param>
        /// <param name="vlcFilename">Nazev souboru i s cestou k VLC</param>
        /// <param name="workingDirectory">Pracovni adresar, ve kterem bude bezet nahravani.</param>
        internal Job(Task task, TaskFolder folder, JobVersion version, string vlcFilename, string workingDirectory)
        {
            this.task = task;
            this.folder = folder;
            this.version = version;
            this.vlcFilename = vlcFilename;
            this.workingDirectory = workingDirectory;

            taskName = task.Name;
            definition = task.Definition;
            
            TriggerCollection triggers = definition.Triggers;
            if (triggers.Count == 1 && triggers[0] is TimeTrigger)
            {
                TimeTrigger trigger = (TimeTrigger) triggers[0];
                Start = trigger.StartBoundary;
                Length = (int) definition.Settings.ExecutionTimeLimit.TotalMinutes;
            }

            ActionCollection actions = definition.Actions;
            if (actions.Count == 1 && actions[0] is ExecAction)
            {
                ExecAction action = (ExecAction) actions[0];
                String args = action.Arguments;
                if (!String.IsNullOrEmpty(args))
                {
                    UseMPEGTS = args.Contains("dst=std{access=file,mux=ps,");

                    var r =
                            new Regex(
                                    "(?<uri>((udp(stream)?|rtp):(//)?([0-9:@.]+)))?.*(:demuxdump-file=\"|:sout=#duplicate{dst=std{access=file,mux=ps,(url|dst)=\")(?<filename>([^\"]+))?(\"|\"}})");
                    Match m = r.Match(action.Arguments);
                    Uri = m.Groups["uri"].Value;
                    Filename = m.Groups["filename"].Value;
                }
            }

        }

        /// <summary>
        /// Konstruktor objektu
        /// </summary>
        /// <param name="definition">Definice ulohy</param>
        /// <param name="taskName">Nazev ulohy</param>
        /// <param name="folder">Misto ulozeni naplanovane ulohy</param>
        /// <param name="version">Verze naplanovane ulohy</param>
        /// <param name="vlcFilename">Nazev souboru i s cestou k VLC</param>
        /// <param name="workingDirectory">Pracovni adresar, ve kterem bude bezet nahravani.</param>
        internal Job(TaskDefinition definition, String taskName, TaskFolder folder, JobVersion version, string vlcFilename, string workingDirectory)
        {
            task = null;
            this.folder = folder;
            this.version = version;
            this.vlcFilename = vlcFilename;
            this.workingDirectory = workingDirectory;
            this.taskName = taskName;

            this.definition = definition;
        }

        public void Save(String userName, String password)
        {
            //nastaveni hodnot
            definition.Triggers.Clear();
            definition.Triggers.Add(new TimeTrigger {StartBoundary = Start, Enabled = true, EndBoundary = End});

            String args = UseMPEGTS ? string.Format("{0} :demux=dump :demuxdump-file=\"{1}\"", Uri, Filename) : string.Format("{0} :sout=#duplicate{{dst=std{{access=file,mux=ps,url=\"{1}\"}}}}", Uri, Filename);
            
            if (definition.Actions.Count > 0 && version == JobVersion.V2)
            {
                if (!(definition.Actions[0] is ExecAction))
                {
                    definition.Actions.Clear();
                    definition.Actions.Add(new ExecAction(vlcFilename, args, workingDirectory));
                } else
                {
                    ExecAction action = (ExecAction) definition.Actions[0];
                    action.Path = vlcFilename;
                    action.Arguments = args;
                    action.WorkingDirectory = workingDirectory;
                }
            }
            else
            {
                definition.Actions.Add(new ExecAction(vlcFilename, args, workingDirectory));
            }
            
            definition.Settings.ExecutionTimeLimit = TimeSpan.FromMinutes(Length);
            definition.Settings.DeleteExpiredTaskAfter = TimeSpan.FromMinutes(Length);

            definition.Settings.Priority = ProcessPriorityClass.High;

            //Pokud je v ceste adresar a neexistuje, vytvorime ho
            if (Filename.IndexOf('\\') != -1) Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(workingDirectory, Filename)));

            //ulozime ulohu
            TaskLogonType logonType = String.IsNullOrEmpty(password)
                                              ? TaskLogonType.InteractiveToken
                                              : TaskLogonType.Password;
            if (version == JobVersion.V2) definition.Principal.UserId = userName;
            task = folder.RegisterTaskDefinition(taskName, definition, TaskCreation.CreateOrUpdate, userName, password, logonType, null);
        }

        /// <summary>
        /// Vraci/Nastavuje nazev nahravani. Pri zmene nazvu nahravani prejmenuje celou ulohu v naplanovanych ulohach
        /// </summary>
        public string Name
        {
            get
            {
                string name = taskName;
                if (version == JobVersion.V2) return name;
                if (name.StartsWith(Schedules.NEW_TASK_PREFIX)) return name.Substring(Schedules.NEW_TASK_PREFIX.Length);
                if (name.StartsWith(Schedules.OLD_TASK_PREFIX)) return name.Substring(Schedules.OLD_TASK_PREFIX.Length);
                return String.Empty;
            }
            set
            {
                if (value == Name) return;

                if (version == JobVersion.V2)
                {
                    taskName = value;
                } 
                else
                {
                    taskName = Schedules.NEW_TASK_PREFIX + value;
                }
            }
        }

        /// <summary>
        /// Vraci/Nastavuje URI zdroje nahravani. Pri zmene provede znovuvytvoreni parametru u naplanovane ulohy.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Vraci/Nastavuje zacatek nahravani.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Vraci/Nastavuje delku nahravani v minutach.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Vraci/Nastavuje konec nahravani. Konec nahravani primarne zavisi na delce nahravani. Pokud se zmeni konec nahravani, prenastavi se delka nahravani. Vracena hodnota se vypocitava jako start nahravani + delka nahravani.
        /// </summary>
        public DateTime End
        {
            get { return Start.AddMinutes(Length); }
            set { Length = (int) Decimal.Round((decimal) value.Subtract(Start).TotalMinutes); }
        }

        /// <summary>
        /// Vraci/Nastavuje soubor, do ktereho se budou ukladat nahrana data (obraz+zvuk). Pri zmene provede znovuvytvoreni parametru u naplanovane ulohy.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Vraci/Nastavuje zda vysledny soubor bude ulozen v kontejneru MPEG TS (true) nebo MPEG PS (false). Pri zmene provede znovuvytvoreni parametru u naplanovane ulohy.
        /// </summary>
        public bool UseMPEGTS { get; set; }

        /// <summary>
        /// Vraci stav ulohy.
        /// </summary>
        public JobStatus Status
        {
            get
            {
                switch (task.State)
                {
                    case TaskState.Disabled:
                        return JobStatus.Disabled;
                    case TaskState.Running:
                        return JobStatus.Running;
                    case TaskState.Ready:
                        return JobStatus.Ready;
                    default:
                        return JobStatus.Unknown;
                }
            }
        }

        /// <summary>
        /// Vraci stav ulohy jako textovy retezec.
        /// </summary>
        public string StatusText
        {
            get
            {
                switch (Status)
                {
                    case JobStatus.Disabled:
                        return "Zakázaný";
                    case JobStatus.Ready:
                        return "Pøipraveno k nahrávání";
                    case JobStatus.Running:
                        return "Bìží";
                    default:
                        return "Neznámý";
                }
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Zruseni vsech inicializovanych objektu.
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        #endregion

        /// <summary>
        /// Vypne bezici ulohu
        /// </summary>
        public void Terminate()
        {
            task.Stop();
            //pokud po Terminate zavolame ihned Dispose / Close, tak se uloha nevypne. Pockame tedy sekundu.
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Uzavreni objektu ulohy nahravani.
        /// </summary>
        public void Close()
        {
            if (task != null)
            {
                task.Dispose();
                task = null;
            }
        }
    }
}