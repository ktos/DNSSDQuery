#region License
/*
 * DNSSDQuery
 *
 * Copyright (C) Marcin Badurowicz <m at badurowicz dot net> 2016
 *
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files
 * (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge,
 * publish, distribute, sublicense, and/or sell copies of the Software,
 * and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
 * BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
 * ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE. 
 */
#endregion

using System;
using System.Threading;
using Windows.Devices.Enumeration;

namespace Ktos.DnsSdQuery
{
    // small portion of the code (C) by Microsoft 2016
    // https://github.com/Microsoft/Windows-appsample-networkhelper/blob/master/NetworkHelper/DnssdParticipant.cs

    /// <summary>
    /// A small console app to perform DNS-SD queries on the network, using Windows 10 built-in UWP APIs
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The protocol ID that identifies DNS-SD.
        /// </summary>
        private const string PROTOCOL_GUID = "{4526e8c1-8aac-4153-9b16-55e86ada0e54}";

        /// <summary>
        /// The host name property.
        /// </summary>
        private const string HOSTNAME_PROPERTY = "System.Devices.Dnssd.HostName";

        /// <summary>
        /// The service name property.
        /// </summary>
        private const string SERVICENAME_PROPERTY = "System.Devices.Dnssd.ServiceName";

        /// <summary>
        /// The instance name property.
        /// </summary>
        private const string INSTANCENAME_PROPERTY = "System.Devices.Dnssd.InstanceName";

        /// <summary>
        /// The IP address property.
        /// </summary>
        private const string IPADDRESS_PROPERTY = "System.Devices.IpAddress";

        /// <summary>
        /// The port number property.
        /// </summary>
        private const string PORTNUMBER_PROPERTY = "System.Devices.Dnssd.PortNumber";

        /// <summary>
        /// The text attributes
        /// </summary>
        private const string TEXTATTRIBUTES_PROPERTY = "System.Devices.Dnssd.TextAttributes";

        /// <summary>
        /// All of the properties that will be returned when a DNS-SD
        /// instance has been found.
        /// </summary>
        private static string[] _propertyKeys = new string[] {
            HOSTNAME_PROPERTY,
            SERVICENAME_PROPERTY,
            INSTANCENAME_PROPERTY,
            IPADDRESS_PROPERTY,
            PORTNUMBER_PROPERTY,
            TEXTATTRIBUTES_PROPERTY
        };

        private static bool isWorking = false;

        private static void Main(string[] args)
        {
            var options = new ConsoleOptions();
            var result = CommandLine.Parser.Default.ParseArguments(args, options);

            if (!result)
            {
                Environment.Exit(1);
            }

            SearchForDevices(options.Service, options.Protocol, options.Domain);

            if (options.WaitToEnd)
            {
                while (isWorking) ;
            }
            else
            {
                // state-of-the-art synchronization
                Thread.Sleep(options.WaitTime * 1000);
            }
        }

        private static void SearchForDevices(string service, string protocol, string domain)
        {
            // Searching requires a lot of magic strings - seriously?
            string query = $"System.Devices.AepService.ProtocolId:={PROTOCOL_GUID} AND " + $"System.Devices.Dnssd.Domain:=\"{domain}\" AND System.Devices.Dnssd.ServiceName:=\"_{service}._{protocol}\"";
            var wat = DeviceInformation.CreateWatcher(query, _propertyKeys, DeviceInformationKind.AssociationEndpointService);

            wat.EnumerationCompleted += Wat_EnumerationCompleted;
            wat.Added += Wat_Added;
            wat.Updated += Wat_Updated;

            isWorking = true;
            wat.Start();
        }

        /// <summary>
        /// If there is no handler for updated, whole enumeration works
        /// slowly and not reliable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Wat_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
        }

        private static void Wat_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            Console.WriteLine();
            Console.WriteLine("Enumeration completed.");
            isWorking = false;            
        }

        private static void Wat_Added(DeviceWatcher sender, DeviceInformation args)
        {
            Console.WriteLine($"{args.Name}");
            Console.WriteLine($"\tHostname: {args.Properties[HOSTNAME_PROPERTY]}");
            Console.WriteLine($"\tService name: {args.Properties[SERVICENAME_PROPERTY]}");
            Console.WriteLine($"\tInstance name: {args.Properties[INSTANCENAME_PROPERTY]}");
            Console.WriteLine($"\tPort number: {args.Properties[PORTNUMBER_PROPERTY]}");

            Console.WriteLine($"\tIP Addresses:");
            foreach (var s in (string[])args.Properties[IPADDRESS_PROPERTY])
            {
                Console.WriteLine($"\t\t{s}");
            }

            Console.WriteLine($"\tText Attributes:");
            foreach (var s in (string[])args.Properties[TEXTATTRIBUTES_PROPERTY])
            {
                Console.WriteLine($"\t\t{s}");
            }
            Console.WriteLine();
        }
    }
}