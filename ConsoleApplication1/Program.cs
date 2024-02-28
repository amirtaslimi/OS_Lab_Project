namespace ConsoleApplication1
{
using System;
using System.Collections.Generic;
using System.Threading;

public class Process
{
    public int id;
    public string name;
    public int cpuUsage;
    public int ramUsage;
    public int bandwidthUsage;
    public Thread thread;

    public Process(int id, string name, int cpuUsage, int ramUsage, int bandwidthUsage)
    {
        this.id = id;
        this.name = name;
        this.cpuUsage = cpuUsage;
        this.ramUsage = ramUsage;
        this.bandwidthUsage = bandwidthUsage;
    }

    public override string ToString()
    {
        return $"{id}\t{name}\t{cpuUsage}%\t{ramUsage}GB\t{bandwidthUsage}Mbps";
    }


    public void Start(OS os)
    {
        switch (this.id)
        {
            case 0:
            {
                 thread = new Thread(
                     () => os.RunProcessCheck(this));

                // Start the thread.
                thread.Start();
            }
                break;
            case 1:
            {
                thread = new Thread(() => os.RunProcessManager(id, "kill"));

                // Start the thread.
                thread.Start();
            }
                break;
            case 2:
            {
                thread = new Thread(
                    () => os.RunVPN(this));

                // Start the thread.
                thread.Start();
            }
                break;
            case 3:
            {
                 thread = new Thread(() => os.RunMine(this));

                // Start the thread.
                thread.Start();
            }
                break;
            case 4:
            {
                 thread = new Thread(() => os.RunCounter(this));

                // Start the thread.
                thread.Start();
            }
                break;

                
        }


    }

    public void Stop(OS os)
    {
        thread.Abort();
        os.RemoveProcess(this);
    }
}

public class OS
{
    private int cpu;
    private int ram;
    private int bandwidth;
    public List<Process> processes;
    public List<Process> notActiveProcesses;

    public OS(int cpu, int ram, int bandwidth)
    {
        this.cpu = cpu;
        this.ram = ram;
        this.bandwidth = bandwidth;
        this.processes = new List<Process>();
        this.notActiveProcesses = new List<Process>();
    }

    
    
    public void RunProcessCheck(Process process)
    {
        Console.WriteLine("Running process check...");
        Console.WriteLine("ID\tName\tCPU\tRAM\tBandwidth");
        foreach (Process p in processes)
        {
            Console.WriteLine(p);
        }

        RemoveProcess(process);

    }

    public void RunProcessManager(int pid, string command)
    {
        Process target = null;
        foreach (Process p in processes)
        {
            if (p.id == pid)
            {
                target = p;
                break;
            }
        }

        if (target == null)
        {
            Console.WriteLine($"Process with ID {pid} not found.");
            return;
        }

        switch (command)
        {
            case "kill":
                Console.WriteLine($"Killing process {target.name} (ID {target.id})...");
                target.Stop(this);
                break;
            default:
                Console.WriteLine($"Invalid command: {command}");
                break;
        }
    }

    public void RunVPN(Process process)
    {
        Console.WriteLine("Starting VPN...");
        while (true)
        {
            Thread.Sleep(1000);
        }
        //RemoveProcess(process);
    }

    public void RunMine(Process process)
    {
        Process vpn = null;
        foreach (Process p in processes)
        {
            if (p.name == "VPN")
            {
                vpn = p;
                break;
            }
        }

        if (vpn == null)
        {
            Console.WriteLine("Network error: VPN not running.");
            return;
        }

        Console.WriteLine("Mining started.");
        Thread.Sleep(3000);
        Console.WriteLine("Mining completed successfully.");
        RemoveProcess(process);

    }
    

    public void RunCounter(Process process)
    {
        Console.WriteLine("Counter started.");
        for (int i = 1; i <= 10000; i++)
        {
            if (i % 1000 == 0)
            {
                Console.WriteLine(i);
            }
            Thread.Sleep(1);
        }
        Console.WriteLine("Counter completed.");
        RemoveProcess(process);

    }

    public bool CanRunProcess(int cpuUsage, int ramUsage, int bandwidthUsage)
    {
        return cpuUsage <= cpu && ramUsage <= ram && bandwidthUsage <= bandwidth;
    }

    public void AddProcess(Process p)
    {
        if (CanRunProcess(p.cpuUsage, p.ramUsage, p.bandwidthUsage))    
        {
            processes.Add(p);
            cpu -= p.cpuUsage;
            ram -= p.ramUsage;
            bandwidth -= p.bandwidthUsage;
            p.Start(this);
        }
        else
        {
            Console.WriteLine("can not run this process because of resources, added to list");
            notActiveProcesses.Add(p);
        }

    }

    public void RemoveProcess(Process p)
    {
        processes.Remove(p);
        cpu += p.cpuUsage;
        ram += p.ramUsage;
        bandwidth += p.bandwidthUsage;
        
        Schedule();
    }

    public void Schedule()
    {

        if (notActiveProcesses[0] != null)
        {
            this.AddProcess(notActiveProcesses[0]);
            
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        OS os = new OS(100, 6, 10);

        Process processCheck = new Process(0, "Process Check", 0, 0, 0);
        Process processManager = new Process(1, "Process Manager", 0, 0, 0);
        Process vpn = new Process(2, "VPN", 0, 0, 2);
        Process mine = new Process(3, "Mine", 80, 4, 8);
        Process counter = new Process(4, "Counter", 10, 3, 0);

        


        while (true)
        {
            
            Console.WriteLine("Enter a number to choose an option:");
            Console.WriteLine("0: Check for running processes");
            Console.WriteLine("1: Kill a process");
            Console.WriteLine("2: Connect to VPN");
            Console.WriteLine("3: Start mining");
            Console.WriteLine("4: Run counter");
            
            
            string input = Console.ReadLine();
            int choice;

            int.TryParse(input, out choice);

            switch (choice)
            {
                case 0:
                    os.AddProcess(processCheck);
                    Thread.Sleep(100);
                    break;
                
                
                case 1:
                    Console.WriteLine("Enter the process ID to kill:");
                    string input1 = Console.ReadLine();
                    int processToKill;
                    int.TryParse(input1, out processToKill);
                    os.RunProcessManager(processToKill, "kill");
                    Thread.Sleep(100);
                    break;
                
                
                case 2:
                    os.AddProcess(vpn);
                    Thread.Sleep(100);
                    break;
                
                case 3:
                    os.AddProcess(mine);
                    Thread.Sleep(100);
                    break;
                
                case 4:
                    os.AddProcess(counter);
                    Thread.Sleep(100);
                    break;
            }
        }
        

    }
}
}