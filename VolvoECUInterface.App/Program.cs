/// <summary>
/// Entry point for the Volvo ECU Interface application.
/// This class contains the main method that starts the Windows Forms application.
/// </summary>
using System;
using System.Windows.Forms;

namespace VolvoECUInterface.App;

static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// Configures the application environment and starts the main form.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new UserInterface());
    }    
}