using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
namespace Lab01
{
    class SqrMatrix
    {
        public int Size { get; set; }
        int[,] arr;
        //public static Mutex mutex = new Mutex();

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
            //Multithreading
            if (treadscnt > 1)
                return MTSortRowsWithThreads(a, treadscnt);
            else
                return SortRows(a,0);
        }



        static SqrMatrix MTSortRowsWithThreads(SqrMatrix a, int treadscnt)
        {

            List<Thread> threads = new List<Thread>();

            // Adding threads
            for (int i = 0; i < treadscnt; i++)
            {
                Thread thread = new Thread(() => SortRows(a, i));
                threads.Add(thread);
                thread.Start();
            }

            // Joining 
            foreach (Thread t in threads)
                t.Join();

            return a;
        }

        // Sort in loop
        static SqrMatrix SortRows(SqrMatrix a, int k)
        {
            for (int i = k; i < a.GetLen(); i += k+1)
                for (int j = 0; j < a.GetLen(); j++)
                {
                    if (a[i, j] > a[i, i] && j != i)
                    {
                        int tmp = a[i, i];
                        a[i, i] = a[i, j];
                        a[i, j] = tmp;
                    }

                }
            return a;
        }

    }





    class Program
    {
        public static void Main(string[] args)
        {
            int random_range = 100;
            int size = 0;

            while (size == 0)
            {
                Console.Write("\n Define the size of the square matrix:");
                String val =  Console.ReadLine();
                if (!string.IsNullOrEmpty(val))
                    size = Convert.ToInt32(val);

                if (size < 2)
                {
                    Console.WriteLine("\n The value {0} is not correct", size);
                    continue;
                }
            }

            SqrMatrix A = new SqrMatrix(size).RandomValues(random_range);
            
            while (true)
            {
                int threadscnt = 0;
                SqrMatrix B = A;
                Console.Write("\n Define the number of parrallel threads:");
                threadscnt = Convert.ToInt32(Console.ReadLine());
                if (threadscnt < 1)
                {
                    Console.WriteLine("\n The value {0} is not correct", threadscnt);
                    continue;
                }
                else 
                {
                    
                    Console.WriteLine("\n Soring the matrix {0}x{0} with {1} threads", size, threadscnt);
                    //  B.Print();
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                        SqrMatrix.ProcessMatrix(B, threadscnt);

                    sw.Stop();

                    Console.WriteLine("\n Elapsed={0}", sw.Elapsed);
                      
                    
                }
                     //  B.Print();
                B = null;
                Console.WriteLine("Start Again? - y");

                if (Console.ReadKey().Key == ConsoleKey.Y)
                    continue;
                else          
                    break;                   

            }
        }
    }
}


   
