using CommandLine;

namespace U3du
{
    public class ExportOptions : ICommander
    {
        public void Process() {
            System.Console.WriteLine(FileName);
        }
        [Option('i',Required =true)]
        public string FileName { get; set; }
        [HelpOption]
        public string GetUsage()
        {
            return @"export
";
        }
    }
}