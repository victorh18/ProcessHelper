using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ProcessHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessHelper.Run();
        }
    }

    public static class ProcessHelper
    {
        private static Dictionary<int, Process> processes;
        private static Dictionary<int, string> options = new Dictionary<int, string>() {
            { 1, "List all processes" },
            { 2, "List a process information" },
            { 3, "List a process' ID" },
            { 4, "Kill a process (needs administrator priviledges)" },
            { 5, "Exit" }
        };

        static ProcessHelper()
        {
            GetProcesses();
        }

        public static string DisplayMenu()
        {
            if ((options?.Count ?? 0) == 0)
            {
                throw new Exception("No options have been defined for the program, contact its developer.");
            }
            Console.WriteLine("Welcome to the process helper app. Please, select one of the options below:");
            foreach (var option in options)
            {
                Console.WriteLine($"{option.Key}. {option.Value}");
            }

            return Console.ReadLine(); 
        }

        public static void Run()
        {
            int optionSelected = 0;
            bool validOption = false;
            do
            {
                if (int.TryParse(DisplayMenu(), out optionSelected))
                {
                    validOption = options.ContainsKey(optionSelected);
                    if (!validOption)
                    {
                        Console.WriteLine("Please select one of the options available.");
                    }
                } else
                {
                    Console.WriteLine("Please select a valid option.");
                }
            } while (!validOption);

            switch (optionSelected)
            {
                case 1:
                    ListProcesses();
                    Console.WriteLine("Press any key to go back to the menu.");
                    Console.ReadKey();
                    Run();
                    break;
                case 2:
                    ListProcessInformation();
                    Console.WriteLine("Press any key to go back to the menu.");
                    Console.ReadKey();
                    Run();
                    break;
                case 3:
                    ListProcessID();
                    Console.WriteLine("Press any key to go back to the menu.");
                    Console.ReadKey();
                    Run();
                    break;
                case 4:
                    KillProcess();
                    Console.WriteLine("The process was killed suscessfully");
                    Console.WriteLine("Press any key to go back to the menu.");
                    Console.ReadKey();
                    Run();
                    break;
                default:
                    break;
            }
        }

        public static void ListProcesses()
        {
            if (processes.Count > 0)
            {
                foreach (var p in processes.Values)
                {
                    Console.WriteLine(GetProcessInformation(p));
                }
                Console.WriteLine($"Total processes listed: {processes.Count}");
            }
        }

        public static void ListProcessInformation()
        {
            int processId = 0;
            Console.Write("Write the process ID you want to get info from: ");
            var id = Console.ReadLine();
            if (id == "")
            {
                return;
            }

            if (int.TryParse(id, out processId))
            {
                if (processes.ContainsKey(processId))
                {
                    Console.WriteLine(GetProcessInformation(processes[processId]));
                }
                else
                {
                    Console.WriteLine("There is no process that matches that ID (maybe it was killed in the meantime...)");
                }
            } else
            {
                Console.WriteLine($"\"{id}\" is not a valid process ID (it should be a number)");
                ListProcessInformation();
            }
        }

        public static void ListProcessID()
        {
            Console.Write("Write the name of the process: ");
            string processName = Console.ReadLine();
            var process = processes.Where(p => p.Value.ProcessName.Equals(processName)).Select(p => p.Value).FirstOrDefault();
            if (!(process == null))
            {
                Console.WriteLine($"The process {processName} has the ID {process.Id}");
            } else
            {
                Console.WriteLine($"Couldn't find a process with the name {processName}");
                ListProcessID();
            }
            

        }

        public static void KillProcess()
        {
            int processId = 0;
            Console.Write("Write the process ID you want to kill: ");
            var id = Console.ReadLine();
            if (id == "")
            {
                return;
            }

            if (int.TryParse(id, out processId))
            {
                if (processes.ContainsKey(processId))
                {
                    processes[processId].Kill();
                }
                else
                {
                    Console.WriteLine("There is no process that matches that ID (maybe it was killed in the meantime...)");
                }
            }
            else
            {
                Console.WriteLine($"\"{id}\" is not a valid process ID (it should be a number)");
                ListProcessInformation();
            }

        }

        public static void GetProcesses()
        {
            processes = new Dictionary<int, Process>();
            foreach(var p in Process.GetProcesses())
            {
                processes.Add(p.Id, p);
            }
        }

        public static string GetProcessInformation(Process p)
        {
            var output = new StringBuilder();
            output.Append($"Process ID: {p.Id} \n");
            output.Append($"Process Name: {p.ProcessName} \n");
            output.Append($"Process Memory: {p.PagedMemorySize64} \n");
            return output.ToString();
        }
    }
}
