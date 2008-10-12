using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using TaskScheduler;

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
        Ready = TaskStatus.Ready,
        /// <summary>
        /// Uloha prave bezi.
        /// </summary>
        Running = TaskStatus.Running,
        /// <summary>
        /// Uloha neni jiz naplanovana. 
        /// </summary>
        NotScheduled = TaskStatus.NotScheduled,
        /// <summary>
        /// Uloha je zakazana.
        /// </summary>
        Disabled = TaskStatus.Disabled,
        /// <summary>
        /// Uloha byla nasilne ukoncena.
        /// </summary>
        Terminated = TaskStatus.Terminated,
        /// <summary>
        /// Stav neni znamy.
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// Reprezentuje ulohu nahravani
    /// </summary>
    public class Job : IDisposable
    {
        private Task task;
        private bool useMPEGTS;

        /// <summary>
        /// Konstruktor objektu
        /// </summary>
        /// <param name="task">Naplanovana uloha</param>
        /// <param name="vlcFilename">Nazev souboru i s cestou k VLC</param>
        /// <param name="workingDirectory">Pracovni adresar, ve kterem bude bezet nahravani.</param>
        internal Job(Task task, string vlcFilename, string workingDirectory)
        {
            this.task = task;
            this.task.ApplicationName = vlcFilename;
            this.task.WorkingDirectory = workingDirectory;

            useMPEGTS = task.Parameters.Contains("dst=std{access=file,mux=ps,");

            task.Flags |= TaskFlags.DeleteWhenDone;
        }

        /// <summary>
        /// Vraci/Nastavuje nazev nahravani. Pri zmene nazvu nahravani prejmenuje celou ulohu v naplanovanych ulohach
        /// </summary>
        public string Name
        {
            get
            {
                string name = task.Name;
                if (name.Substring(0, 12).CompareTo("Nahrávání - ") == 0) return name.Substring(12);
                if (name.Substring(0, 13).CompareTo("Nahrávadlo - ") == 0) return name.Substring(13);
                return "";
            }
            set
            {
                if (value == Name) return;

                //preulozime nazev
                string oldName = task.Name;

                task.Save("Nahrávadlo - " + value);

                //smazeme starou naplanovanou ulohu
                Schedules.Remove(oldName);
            }
        }

        /// <summary>
        /// Vraci/Nastavuje URI zdroje nahravani. Pri zmene provede znovuvytvoreni parametru u naplanovane ulohy.
        /// </summary>
        public string Uri
        {
            get
            {
                var r =
                    new Regex(
                        "(?<uri>((udp(stream)?|rtp):(//)?([0-9:@.]+)))?.*(:demuxdump-file=\"|:sout=#duplicate{dst=std{access=file,mux=ps,(url|dst)=\")(?<filename>([^\"]+))?(\"|\"}})");
                Match m = r.Match(task.Parameters);
                return m.Groups["uri"].Value;
            }
            set
            {
                RegenerateParameters(value, Filename);
                task.Save();
            }
        }

        /// <summary>
        /// Vraci/Nastavuje zacatek nahravani.
        /// </summary>
        public DateTime Start
        {
            get
            {
                foreach (Trigger tr in task.Triggers)
                {
                    if (tr is RunOnceTrigger)
                    {
                        DateTime dt = (tr as RunOnceTrigger).BeginDate;

                        return
                            new DateTime(dt.Year, dt.Month, dt.Day, (tr as RunOnceTrigger).StartHour, (tr as RunOnceTrigger).StartMinute, 0);
                    }
                }
                return new DateTime();
            }
            set
            {
                task.Triggers.Clear();
                task.Triggers.Add(new RunOnceTrigger(value));
                task.Save();
            }
        }

        /// <summary>
        /// Vraci/Nastavuje delku nahravani v minutach.
        /// </summary>
        public int Length
        {
            get { return (int) task.MaxRunTime.TotalMinutes; }
            set
            {
                task.MaxRunTime = TimeSpan.FromMinutes(value);
                task.Save();
            }
        }

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
        public string Filename
        {
            get
            {
                var r =
                    new Regex(
                        "(?<uri>((udp(stream)?|rtp):(//)?([0-9:@.]+)))?.*(:demuxdump-file=\"|:sout=#duplicate{dst=std{access=file,mux=ps,(url|dst)=\")(?<filename>([^\"]+))?(\"|\"}})");
                Match m = r.Match(task.Parameters);
                return m.Groups["filename"].Value;
            }
            set
            {
                RegenerateParameters(Uri, value);
                task.Save();
            }
        }

        /// <summary>
        /// Vraci uzivatelske jmeno, pod kterym bude spusteno nahravani
        /// </summary>
        public string UserName
        {
            get { return task.AccountName; }
        }

        /// <summary>
        /// Vraci/Nastavuje zda vysledny soubor bude ulozen v kontejneru MPEG TS (true) nebo MPEG PS (false). Pri zmene provede znovuvytvoreni parametru u naplanovane ulohy.
        /// </summary>
        public bool UseMPEGTS
        {
            get { return useMPEGTS; }
            set
            {
                useMPEGTS = value;
                RegenerateParameters(Uri, Filename);
                task.Save();
            }
        }

        /// <summary>
        /// Vraci stav ulohy.
        /// </summary>
        public JobStatus Status
        {
            get
            {
                switch (task.Status)
                {
                    case TaskStatus.Disabled:
                        return JobStatus.Disabled;
                    case TaskStatus.Running:
                        return JobStatus.Running;
                    case TaskStatus.Terminated:
                        return JobStatus.Terminated;
                    case TaskStatus.NoTriggers:
                    case TaskStatus.NoTriggerTime:
                    case TaskStatus.NotScheduled:
                    case TaskStatus.NoMoreRuns:
                        return JobStatus.NotScheduled;
                    case TaskStatus.NeverRun:
                    case TaskStatus.Ready:
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
                    case JobStatus.NotScheduled:
                        return "Nenaplánováno";
                    case JobStatus.Running:
                        return "Bìží";
                    case JobStatus.Terminated:
                        return "Neúspìšnì vykonáno";
                    default:
                        return "";
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
        /// Nastavuje uzivatelske jmeno a heslo, pod kterym bude spusteno nahravani. Pokud predame null nebo prazdny retezec parametru username, nastavi se uzivatelske jmeno prave prihlaseneho uzivatele.
        /// </summary>
        /// <param name="username">Uzivatelske jmeno, pod kterym bude spusteno nahravani.</param>
        /// <param name="password">Heslo uzivatele, pod kterym bude spusteno nahravani.</param>
        public void SetUsernameAndPassword(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                username = WindowsIdentity.GetCurrent().Name;
                password = null;
                task.Flags |= TaskFlags.RunOnlyIfLoggedOn;
            }
            task.SetAccountInformation(username, password);
            task.Save();
        }

        /// <summary>
        /// Vypne bezici ulohu
        /// </summary>
        public void Terminate()
        {
            task.Terminate();
            //pokud po Terminate zavolame ihned Dispose / Close, tak se uloha nevypne. Pockame tedy sekundu.
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Pregeneruje parametry u naplanovane ulohy.
        /// </summary>
        /// <param name="uri">URI zdroje nahravani</param>
        /// <param name="filename">Soubor, do ktereho se budou ukladat nahrana data (obraz+zvuk).</param>
        private void RegenerateParameters(string uri, string filename)
        {
            //pokud je v ceste adresar, ktery neexistuje, tak ho vytvorime
            if (filename.IndexOf('\\') != -1)
            {
                string path = Path.Combine(task.WorkingDirectory, filename);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            // HACK chtelo by tro moznost nastavovat prioritu uzivatelsky
            task.Priority = ProcessPriorityClass.High;
            task.Parameters = useMPEGTS ? string.Format("{0} :demux=dump :demuxdump-file=\"{1}\"", uri, filename) : string.Format("{0} :sout=#duplicate{{dst=std{{access=file,mux=ps,url=\"{1}\"}}}}", uri, filename);
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