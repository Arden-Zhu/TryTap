using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TryTap
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.UsageIoTapFunction();

            program.TryCpuConsuming();

            Console.ReadKey();
        }

        private async void UsageIoTapFunction()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage httpResponse = 
                await client.GetAsync(@"https://www.google.com/search?q=test");
            string content = await httpResponse.Content.ReadAsStringAsync();
            Console.WriteLine("content = " + content.Substring(0, 100));
        }

        private async void TryCpuConsuming()
        {
            int r = await FibAsync(6);
            Console.WriteLine("FibAsync(6)=" + r.ToString());
        }

        private int Fib(int pos)
        {
            if (pos < 1)
                throw new ArgumentException();
            else if (pos < 3)
            {
                return 1;
            }
            else
            {
                return Fib(pos - 1) + Fib(pos - 2);
            }
        }

        private Task<int> FibAsync(int pos)
        {
            return Task.Run<int>(() => Fib(pos));
        }
    }
}
