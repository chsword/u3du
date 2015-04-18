using System;
using CommandLine;
using Newtonsoft.Json;

namespace U3du
{
    public class ExtractOptions : ICommander
    {
        public void Process()
        {

            Console.WriteLine(JsonConvert.SerializeObject(this));
        }
        [CommandLine.ValueOption(0)]
        public string Unity3dFileName { get; set; }


        [HelpOption]
        public string GetUsage()
        {
            return @"extract";
        }
    }
}