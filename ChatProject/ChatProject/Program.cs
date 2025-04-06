namespace ChatProject
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
       
        //gängige Methode Mutex (Mutual Exclusion) um sicher zu stellen,
        //dass nur 1 Instanz der Anwendung gleichzeitig ausgeführt werden kann

        //private static Mutex mutex;
        [STAThread] //Der STA-Modus ermöglicht es, dass nur ein Thread gleichzeitig auf die Benutzeroberfläche zugreift (da nicht Thread-Sicher)
        static void Main()
        {
            //const string appName = "ChatApplication";
            //bool createdNew;

            //mutex = new Mutex(true, appName, out createdNew);
            //if (!createdNew) return;

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new ClientForm());
        }
    }
}