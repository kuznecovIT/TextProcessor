using TextProcessor.Classes;

namespace TextProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                new FrequentWordsDictionary(args);
            }
            else
            {
                WordsAutoCompletion.RunAutoCompletion(5);
            }                
        }
    }
}
