using TesterApp.Testers;

namespace TesterApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ITester t = new Tester00();
            t.Run();
        }
    }
}