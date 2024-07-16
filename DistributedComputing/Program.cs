using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
namespace Lab01
{
    class SqrMatrix
    {
        public int Size { get; set; }
        int[,] arr;
        public static Mutex mutex = new Mutex();

        public SqrMatrix(int size)
        {
            Size = size;
            arr = new int[size, size];
        }

        public int GetLen()
        {
            int res = arr.GetLength(0);
            return res;
        }

        public int this[int i, int j]
        {
            get { return arr[i, j]; }
            set { arr[i, j] = value; }
        }

        // Populate matrix with random values
        public SqrMatrix RandomValues(int range)
        {
            Random rnd = new Random();
            for (int i = 0; i < GetLen(); i++)
                for (int j = 0; j < GetLen(); j++)
                    arr[i, j] = rnd.Next(range);
            return this;
        }

        //Print matrix
        public void Print()
        {
            for (int i = 0; i < GetLen(); i++)
            {
                for (int j = 0; j < GetLen(); j++)
                    Console.Write(arr[i, j] + " ");
                Console.WriteLine();
            }
        }

        // Process matrix to set the max value for a diagonal element
        public static SqrMatrix ProcessMatrix(SqrMatrix a, int treadscnt)
        {
            if (a == null) throw new ArgumentNullException();
            if (treadscnt < 1) throw new ArgumentOutOfRangeException();
            if (treadscnt > 1) 
                //Multithreading
                return MTSortRows(a, treadscnt);
            else
                //Single thread
                return STSortRows(a);
        }

        static SqrMatrix MTSortRows(SqrMatrix a, int treadscnt)
        {
            for (int k = 0; k < a.GetLen(); k += treadscnt)
            {

                List<Thread> threads = new List<Thread>();
                //Console.WriteLine("Iteration {0}", k);

                // Adding threads
                for (int i = 0; i < treadscnt && k+i < a.GetLen(); i++)
                {
                    int tempi = i+k;
                    Thread thread = new Thread(() => SortRow(tempi, a));
                    thread.Start();
                    threads.Add(thread);
                }
                                    
                // Joining 
                foreach (Thread t in threads)
                    t.Join();
            }   
      
            return a;
        }

        // Sort in loop
        static SqrMatrix STSortRows(SqrMatrix a)
        {
            for (int i = 0; i < a.GetLen(); i++)
                 SortRow(i, a);
             return a;
        }

        // Sort a rof
        static void SortRow(int tempi, SqrMatrix a)
        {
 //           mutex.WaitOne();
            int i = tempi;
            //Console.WriteLine("Calculate row{0}", i);
            for (int j = 0; j < a.GetLen(); j++)
            {
                if (a[i, j] > a[i, i] && j != i)
                {
                    int tmp = a[i, i];
                    a[i, i] = a[i, j];
                    a[i, j] = tmp;
                }

            }
//            mutex.ReleaseMutex();
        }
    }





    class Program
    {
        public static void Main(string[] args)
        {
            int random_range = 100;

            while (true)
            {
                int size = 0;
                Console.Write("Define the size of the square matrix:");
                size = Convert.ToInt32(Console.ReadLine());
                if (size < 2)
                {
                    Console.WriteLine("The value {0} is not correct", size);
                    continue;
                }

                SqrMatrix A = new SqrMatrix(size).RandomValues(random_range);

                int threadscnt = 0;
                Console.Write("Define the number of parrallel threads:");
                threadscnt = Convert.ToInt32(Console.ReadLine());
                if (threadscnt < 1)
                {
                    Console.WriteLine("The value {0} is not correct", threadscnt);
                    continue;
                }
                else
                {
                    Console.WriteLine("Soring the matrix {0}x{0} with {1} threads", size, threadscnt);
                    
                    //  A.Print();
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    SqrMatrix.ProcessMatrix(A, threadscnt);
                    sw.Stop();
                    Console.WriteLine("Elapsed={0}", sw.Elapsed);
                }
                     //  A.Print();

                Console.WriteLine(" Press Q to exit.");
                Console.WriteLine("Any other key - Start from scratch");
                if (Console.ReadKey().Key == ConsoleKey.Q)
                    break;
                else
                    A = null;

            }
        }
    }
}


   
