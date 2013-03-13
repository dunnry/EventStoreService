using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace EventStoreService
{
    public class EventStoreServiceConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true, IsKey = false, IsRequired = true)]
        public ServiceInstanceCollection Instances
        {
            get { return (ServiceInstanceCollection) this[""]; }
            set { this[""] = value; }
        }
    }

    public class ServiceInstanceCollection : ConfigurationElementCollection
    {
        protected override string ElementName
        {
            get { return "instance"; }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        public ServiceInstance this[int index]
        {
            get { return BaseGet(index) as ServiceInstance; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceInstance();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceInstance) element).Name;
        }

        protected override bool IsElementName(string elementName)
        {
            return !String.IsNullOrEmpty(elementName) && elementName == ElementName;
        }
    }

    public class ServiceInstance : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string) this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("useLoopback", IsRequired = false, DefaultValue = false)]
        public bool UseLoopback
        {
            get { return (bool) this["useLoopback"]; }
            set { this["useLoopback"] = value; }
        }

        [ConfigurationProperty("tcpPort", IsRequired = false, DefaultValue = 1113)]
        public int TcpPort
        {
            get { return (int) this["tcpPort"]; }
            set { this["tcpPort"] = value; }
        }

        [ConfigurationProperty("httpPort", IsRequired = false, DefaultValue = 2113)]
        public int HttpPort
        {
            get { return (int) this["httpPort"]; }

            set { this["httpPort"] = value; }
        }

        [ConfigurationProperty("dbPath", IsRequired = true)]
        public string DbPath
        {
            get { return (string) this["dbPath"]; }
            set { this["dbPath"] = value; }
        }

        [ConfigurationProperty("filePath", IsRequired = true)]
        public string FilePath
        {
            get { return (string) this["filePath"]; }
            set { this["filePath"] = value; }
        }

        [ConfigurationProperty("cachedChunkCount", IsRequired = true)]
        public int CachedChunkCount
        {
            get { return (int) this["cachedChunkCount"]; }
            set { this["cachedChunkCount"] = value; }
        }

        [ConfigurationProperty("runProjections", IsRequired = false)]
        public bool RunProjections
        {
            get
            {
                object run = this["runProjections"];
                if (run != null)
                {
                    return (bool) run;
                }
                return false;
            }
            set { this["runProjections"] = value; }
        }

        public ProcessStartInfo GetProcessStartInfo(string file, IPAddress address)
        {
            string arguments = GetProcessArguments(address);

            return new ProcessStartInfo(file, arguments)
            {
                FileName = file,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };
        }

        private string GetProcessArguments(IPAddress address)
        {
            address = UseLoopback ? IPAddress.Loopback : address;
            
            if (address == null) throw new ArgumentNullException("address");
            
            var sb = new StringBuilder();
            sb.AppendFormat("--ip {0} ", address);
            sb.AppendFormat("--tcp-port {0} ", TcpPort);
            sb.AppendFormat("--http-port {0} ", HttpPort);
            sb.AppendFormat("--db {0} ", DbPath);
            sb.AppendFormat("--c {0}", CachedChunkCount);
            
            if (RunProjections)
            {
                sb.Append(" --run-projections");
            }
            
            return sb.ToString();
        }
    }
}