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

using CommandLine;

namespace Ktos.DnsSdQuery
{
    public class ConsoleOptions
    {
        /// <summary>
        /// The domain to be searched on
        /// </summary>
        [Option('d', HelpText = "Domain to be searched", Required = false, DefaultValue = "local")]
        public string Domain { get; set; }

        /// <summary>
        /// The protocol to be searched, tcp by default
        /// </summary>
        [ValueOption(1)]
        [Option('p', HelpText = "Protocol to be searched, without _ at the beginning", Required = false, DefaultValue = "tcp")]
        public string Protocol { get; set; }

        /// <summary>
        /// The name of the service (without _ at beginning) to be searched
        /// </summary>
        [ValueOption(0)]
        [Option('s', HelpText = "Service name to be searched, without _ at the beginning", Required = true)]
        public string Service { get; set; }

        [Option('w', HelpText = "Should app wait to end of enumeration, or just a defined time", Required = false, DefaultValue = false)]        
        public bool WaitToEnd { get; set; }

        [Option("waittime", DefaultValue = 1, HelpText = "How long application should wait before exiting, in seconds", Required = false)]
        public int WaitTime { get; set; }

        [HelpOption]
        public string Help()
        {
            string v = "";            
            v += CommandLine.Text.HelpText.AutoBuild(this);

            return v;
        }        

        [ParserState]
        public IParserState State { get; set; }
    }
}