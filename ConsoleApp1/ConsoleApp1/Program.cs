using System;
using System.Diagnostics;
using System.Threading;


namespace ConsoleApp1
{

    class Program
    {
        public static Mutex mutex = new Mutex();
        public static int NumThread = 4;
        public static double Result = 0;


        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Встановлення кодової сторінки UTF-8

            while (true)
            {
                Console.WriteLine("Виберіть варіант обчислення:");
                Console.WriteLine("1. ∑ i / (1 + i^4)");
                Console.WriteLine("2. ∑ i^(2) / i^(2) * (1 + i^4)");
                Console.WriteLine("3. ∑ 5 / (1 + i^2)*i^2");
                Console.WriteLine("4. ∑ i^(2/3) / (1 + i^4)^(1/2)");
                Console.WriteLine("5. ∑ i / i");
                Console.WriteLine("0. Завершити роботу");

                int choice;
                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Введено некоректне значення. Спробуйте ще раз.");
                    continue;
                }

                if (choice == 0)
                {
                    Console.WriteLine("Робота програми завершена.");
                    break;
                }

                if (choice < 1 || choice > 5)
                {
                    Console.WriteLine("Введено некоректний варіант. Спробуйте ще раз.");
                    continue;
                }

                Calculate(choice);
                Console.WriteLine($"Результат: {Result}");
            }
        }

        public static void Calculate(int choice)
        {
            int iterations = 1000000; // Загальна кількість ітерацій
            Thread[] threads = new Thread[NumThread];

            for (int i = 0; i < NumThread; i++)
            {
                threads[i] = new Thread(new ParameterizedThreadStart(CalculateThread));
                threads[i].Start(new ThreadParams(i * (iterations / NumThread), (i == NumThread - 1) ? iterations : (i + 1) * (iterations / NumThread), choice));
            }

            for (int i = 0; i < NumThread; i++)
                threads[i].Join();
        }

        public static void CalculateThread(object param)
        {
            if (param is ThreadParams)
            {
                ThreadParams threadParams = (ThreadParams)param;
                double result = 0;

                for (double i = threadParams.begin; i < threadParams.end; i++)
                {
                    if (threadParams.choice == 1)
                    {
                        // Варіант 1: "1. ∑ 𝑖 / (1 + 𝑖^4)"
                        result += i / (1 + Math.Pow(i, 4));
                    }

                    if (threadParams.choice == 4)
                    {
                        // Варіант 4: "4. ∑ 𝑖^(2/3) / (1 + 𝑖^4)^(1/2)"
                        result += Math.Pow(i, 2.0 / 3.0) / Math.Sqrt(1 + Math.Pow(i, 4));
                    }


                }
                //избегаю деления на ноль
                for (double i = threadParams.begin + 1; i < threadParams.end; i++)
                {
                    if (threadParams.choice == 2)
                    {
                        // Варіант 2: "2. ∑ 𝑖^(2) / 𝑖^(2) * (1 + 𝑖^4)"
                        result += Math.Pow(i, 2) / (Math.Pow(i, 2) * (1 + Math.Pow(i, 4)));

                    }
                    if (threadParams.choice == 3)
                    {
                        // Варіант 3: "3. ∑ 5 / (1 + 𝑖^2)*𝑖^2"
                        result += 5 / ((1 + Math.Pow(i, 2)) * Math.Pow(i, 2));

                    }
                    if (threadParams.choice == 5)
                    {
                        // Варіант 5: "5. ∑ 𝑖 / 𝑖"
                        result += i / i;
                    }
                }

                mutex.WaitOne();
                Result += result;
                mutex.ReleaseMutex();
            }
        }
    }

    class ThreadParams
    {
        public int begin, end, choice;

        public ThreadParams(int b, int e, int c)
        {
            begin = b;
            end = e;
            choice = c;
        }
    }
}
