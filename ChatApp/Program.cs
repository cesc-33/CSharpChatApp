namespace ChatProject
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
       
        //g�ngige Methode Mutex (Mutual Exclusion) um sicher zu stellen,
        //dass nur 1 Instanz der Anwendung gleichzeitig ausgef�hrt werden kann

        //private static Mutex mutex;
        [STAThread] //Der STA-Modus erm�glicht es, dass nur ein Thread gleichzeitig auf die Benutzeroberfl�che zugreift (da nicht Thread-Sicher)
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
            //Application.Run(new ChatForm("test", new RTFTextFormatter())); // NUR F�R TESTS!
        }
    }
}