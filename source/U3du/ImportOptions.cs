using System;
using CommandLine;

namespace U3du
{
    public class ImportOptions : ICommander
    {
        [Option('i', Required = true)]
        public string FileName { get; set; }
        [HelpOption]
        public string GetUsage()
        {
            return @"export
";
        }

        public void Process()
        {
            
        }
    }
}