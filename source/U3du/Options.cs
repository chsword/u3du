using CommandLine;
using CommandLine.Text;

namespace U3du
{
    class Options
    {
        public Options()
        {
            ExportOptions = new ExportOptions();
            ImportOptions = new ImportOptions();
            ExtractOptions = new ExtractOptions();
        }
        [VerbOption("export")]
        public ExportOptions ExportOptions { get; set; }
        [VerbOption("import")]
        public ImportOptions ImportOptions { get; set; }

        [VerbOption("extract")]
        public ExtractOptions ExtractOptions { get; set; }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }
    }
}