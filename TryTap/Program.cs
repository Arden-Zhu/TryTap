using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TryTap
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            ShowThreadId("Main() 1");
            program.UseIoTapFunction();
            ShowThreadId("key to next step:Main() 2");
            Console.ReadKey();
            program.UseCpuTapFuntion();
            ShowThreadId("key to next step:Main() 3");
            Console.ReadKey();
        }

        private static void ShowThreadId(string context)
        {
            Console.WriteLine($"{context} in thread# {Thread.CurrentThread.ManagedThreadId}");
        }

        private async void UseIoTapFunction()
        {
            ShowThreadId("UseIoTapFunction() 1");
            HttpClient client = new HttpClient();
            HttpResponseMessage httpResponse = 
                await client.GetAsync(@"https://www.google.com/search?q=test");
            ShowThreadId("UseIoTapFunction() 2");
            string content = await httpResponse.Content.ReadAsStringAsync();
            ShowThreadId("UseIoTapFunction() 3");
            Console.WriteLine("content = " + content.Substring(0, 100));
        }

        private async void UseCpuTapFuntion()
        {
            ShowThreadId("UseCpuTapFuntion() 1");
            int pos = 20;
            int r = await FibAsync(pos);
            ShowThreadId("UseCpuTapFuntion() 2");
            Console.WriteLine($"FibAsync({pos})={r}");
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
            ShowThreadId("FibAsync() 1");
            Task<int> task =  Task.Run<int>(() =>
            {
                ShowThreadId($"Fib()");
                return Fib(pos);
            });
            ShowThreadId("FibAsync() 2");
            return task;
        }
    }
}
