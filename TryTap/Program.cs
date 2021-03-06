﻿using System;
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
            program.UseCancellationToken();
            ShowThreadId("key to next step:Main() 5");
            Console.ReadKey();
            program.UseProgress();
            ShowThreadId("key to next step:Main() 6");
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
            foreach (int pos in new[] { 20, -1 })
            {
                try
                {
                    int r = await FibAsync(pos);
                    ShowThreadId("UseCpuTapFuntion() 2");
                    Console.WriteLine($"FibAsync({pos})={r}");
                }
                catch (Exception ex)
                {
                    ShowThreadId($"Exception: {ex.Message}");
                }
            }
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

        /// <summary>
        /// Use TaskCompletionSource, I have the control of setting the result, exception & cancel
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
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

        private async void UseCancellationToken()
        {
            ShowThreadId("UseCancellationToken() 1");
            int pos = 100;
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource(10);
                //cts.Cancel();
                int r = await FibAsync(pos, cts.Token);
                ShowThreadId("UseCancellationToken() 2");
                Console.WriteLine($"UseCancellationToken({pos})={r}");
            }
            catch (OperationCanceledException cancelEx)
            {
                Console.WriteLine($"UseCancellationToken() cancelled");
            }
        }

        private Task<int> FibAsync(int pos, CancellationToken cancellationToken)
        {
            ShowThreadId("FibAsync() 1");
            Task<int> task = Task.Run<int>(() =>
            {
                ShowThreadId($"Fib()");
                Thread.Sleep(20);
                cancellationToken.ThrowIfCancellationRequested();
                return Fib(pos);
            });
            ShowThreadId("FibAsync() 2");
            return task;
        }

        private async void UseProgress()
        {
            ShowThreadId("UseProgress() 1");
            int pos = 100;
            try
            {
                int r = await FibAsync(pos, new Progress<string>((msg)=>ShowThreadId(msg)));

                ShowThreadId("UseProgress() 2");
                Console.WriteLine($"UseProgress({pos})={r}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UseProgress() {ex.Message}");
            }
        }

        private Task<int> FibAsync(int pos, IProgress<string> progress)
        {
            ShowThreadId("FibAsync() 1");
            Task<int> task = Task.Run<int>(() =>
            {
                ShowThreadId($"Fib() 1");
                progress.Report("Fib() 2");
                Thread.Sleep(20);
                ShowThreadId($"Fib() 3");
                progress.Report("Fib() 4");
                return Fib(pos);
            });
            ShowThreadId("FibAsync() 2");
            return task;
        }

    }
}
