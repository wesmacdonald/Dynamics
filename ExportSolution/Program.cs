using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace ExportSolution
{
    class Program
    {

        public static readonly Version Version = Assembly.GetExecutingAssembly().GetName().Version;
        public static readonly string CurrentVersionDate = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToString("f", CultureInfo.CreateSpecificCulture("en-ca"));

        static void Main(string[] args)
        {
            Console.WriteLine("ExportSolution Version {0}", Version.ToString());
            Console.WriteLine("{0}", CurrentVersionDate);
            Console.WriteLine("");
            // Test if input arguments were supplied:
            if (args.Length == 0)
            {
                System.Console.WriteLine("Please enter the arguments below.");
                System.Console.WriteLine("Usage: ExportSolution <SolutionName> <outputFolder> <asManaged>");
                return;
            }

            if (!Directory.Exists(args[1]))
            {
                System.Console.WriteLine("Please provide a valid output folder.");
                System.Console.WriteLine("Usage: ExportSolution <SolutionName> <outputFolder> <asManaged>");
                return;
            }

            bool asManaged = false;

            bool.TryParse(args[2], out asManaged);

            ExportSolution(args[0], args[1], asManaged, false);
        }

        static void ExportSolution(string solutionName, string outputFolder, bool asManaged, bool allOtherOptions)
        {             
            using (new LogTimer(string.Format("ExportSolution", "{0} ({2}) to {1}",
                        /*0*/ solutionName,
                        /*1*/ ConfigurationManager.AppSettings["outputFolder"],
                        /*2*/ asManaged ? "Managed" : "UNManaged")))
            {
                try
                {
                    // Call service to Export
                    ExportSolutionRequest request = new ExportSolutionRequest();
                    request.ExportAutoNumberingSettings = allOtherOptions;
                    request.ExportCalendarSettings = allOtherOptions;
                    request.ExportCustomizationSettings = allOtherOptions;
                    request.ExportEmailTrackingSettings = allOtherOptions;
                    request.ExportGeneralSettings = allOtherOptions;
                    request.ExportIsvConfig = allOtherOptions;
                    request.ExportMarketingSettings = allOtherOptions;
                    request.ExportOutlookSynchronizationSettings = allOtherOptions;
                    request.ExportRelationshipRoles = allOtherOptions;
                    request.Managed = asManaged;
                    request.SolutionName = solutionName;

                    CrmServiceClient service = new CrmServiceClient(ConfigurationManager.ConnectionStrings["CRMConnectionString"].ConnectionString);
                    if (service.IsReady)
                    {
                        ExportSolutionResponse response = (ExportSolutionResponse)service.Execute(request);

                        // Save received solution package
                        //string managed = asManaged ? "_managed" : string.Empty;
                        //string filename = $"{solutionName}{managed}.zip";
                        string filename = $"{solutionName}.zip";
                        string outputFilename = Path.Combine(outputFolder, filename);
                        File.WriteAllBytes(outputFilename, response.ExportSolutionFile);
                        System.Console.WriteLine("ExportSolution {0} {1} {2}", solutionName, outputFilename, asManaged);
                        System.Console.WriteLine("Wrote {0} Bytes", response.ExportSolutionFile.Length);

                    }
                    else
                    {
                        System.Console.WriteLine("Usage: ExportSolution <SolutionName> <outputFolder> <asManaged>");
                        System.Console.WriteLine("Dynamics CRM Service not ready, please verify your username and password");
                        System.Console.WriteLine("If Two Factor Authentication (2FA) is enabled on your AAD account please make sure");
                        System.Console.WriteLine("you are using an App Password in your connection string.");
                    }
                                        
                }
                catch (System.Exception ex)
                {
                    LogHelper.Error(ex, "ExportSolution");
                }
            }
        }
    }
}
