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
            program.UseCompletionSource();
            ShowThreadId("key to next step:Main() 4");
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

        private async void UseCompletionSource()
        {
            ShowThreadId("UseCompletionSource() 1");
            int pos = -1;
            int r = await FibUsingCompletionSourceAsync(pos);
            ShowThreadId("UseCompletionSource() 2");
            Console.WriteLine($"FibUsingCompletionSourceAsync({pos})={r}");
        }

        private Task<int> FibUsingCompletionSourceAsync(int pos)
        {
            ShowThreadId("FibUsingCompletionSourceAsync() 1");
            var tcs = new TaskCompletionSource<int>();
            Task task = Task.Run(() =>
            {
                ShowThreadId($"Fib()");
                try
                {
                    var r = Fib(pos);
                    tcs.SetResult(r);
                }
                catch (Exception ex)
                {
                    ShowThreadId("Exception happens in FibUsingCompletionSourceAsync()");
                    //tcs.SetException(ex);
                }
            });
            ShowThreadId("FibUsingCompletionSourceAsync() 2");
            return tcs.Task;
        }
    }
}
