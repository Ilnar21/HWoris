namespace SingletonConfHW
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = AppConfig.Instance;
            Console.WriteLine(config.Domain);
            Console.WriteLine(config.Port);
            Console.WriteLine(config.StaticDirectoryPath);
        }
    }
}