using System;
using CommandLine;
using CommandLine.Text;

namespace OpcExporter
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Parser p = new Parser(config => config.HelpWriter = null);
            
            var parseResult = p.ParseArguments<Options>(args);

            parseResult.WithParsed<Options>(o =>
            {
                new Startup(o).Run();
            });
                
            parseResult.WithNotParsed(errs =>
            {
                HelpText myHelpText = HelpText.AutoBuild(parseResult, onError =>
                    {
                        HelpText nHelpText = new HelpText(SentenceBuilder.Create(), "OpcInspect 1.0\nIVM Technical Consultants - Patrick Klima <klima@ivm.at> 2019");
                        nHelpText.AdditionalNewLineAfterOption = false;
                        nHelpText.AddDashesToOption = true;
                        nHelpText.MaximumDisplayWidth = 4000;
                        nHelpText.AddOptions(parseResult);
                        return HelpText.DefaultParsingErrorsHandler(parseResult, nHelpText);
                    },
                    onExample => { return onExample; });

                Console.Error.WriteLine(myHelpText);
            });

            Console.ReadKey();
        }
    }
}
