using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace EventStoreService
{
    public class EventStoreProcessWrapper
    {
        private readonly List<Process> _processes;
        private readonly IPAddress _address;
        private readonly ServiceInstanceCollection _instances;

        public EventStoreProcessWrapper(IPAddress address, ServiceInstanceCollection instances)
        {
            _address = address;
            _instances = instances;
            _processes = new List<Process>();
        }

        public void Start()
        {
            foreach (ServiceInstance instance in _instances)
            {
                var info = instance.GetProcessStartInfo(instance.FilePath, _address);
                
                var process = new Process {StartInfo = info, EnableRaisingEvents = true};

                DataReceivedEventHandler outputHandler = (s, e) => File.AppendAllLines("output.log", e.Data.Split(Environment.NewLine.ToCharArray()));
                DataReceivedEventHandler errorHandler = (s, e) => File.AppendAllLines("error.log", e.Data.Split(Environment.NewLine.ToCharArray()));

                process.ErrorDataReceived += errorHandler;
                process.OutputDataReceived += outputHandler;

                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                process.Exited += (sender, args) => Stop();
                _processes.Add(process);
            }
        }

        public void Stop()
        {
            _processes.ForEach(p =>
            {
                p.Refresh();

                if (p.HasExited) return;

                p.Kill();
                p.WaitForExit(TimeSpan.FromSeconds(10).Milliseconds);
                p.Dispose();
            });
        }
    }
}