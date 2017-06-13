using System.ServiceProcess;

namespace IPChecker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Checker()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
