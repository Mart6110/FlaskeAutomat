using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace FlaskeAutomat
{
    class Program
    {
        static Queue<int> _item = new Queue<int>();
        static Queue<string> _beer = new Queue<string>();
        static Queue<string> _soda = new Queue<string>();
        static Random rnd = new Random();
        static void Main(string[] args)
        {
            Thread producer = new Thread(new ThreadStart(Produce));
            Thread splitter = new Thread(new ThreadStart(Sorting));
            Thread getBeer = new Thread(new ThreadStart(GetBeer));
            Thread getSoda = new Thread(new ThreadStart(GetSoda));

            producer.Start();
            splitter.Start();
            getBeer.Start();
            getSoda.Start();

            producer.Join();
            splitter.Join();
            getBeer.Join();
            getSoda.Join();

        }


        public static void Produce()
        {
            while (true)
            {
                Monitor.Enter(_item);
                try
                {
                    // Checking if _item is equal to 0
                    if (_item.Count == 0)
                    {
                        // For loop that loops 10 times and producess a beer or a soda depending on the random number that is generated.
                        for (int i = 0; i < 10; i++)
                        {
                            int bevrage = rnd.Next(1, 3);

                            // A switch that looks at what kind of bevrage we have produced.
                            switch (bevrage)
                            {
                                case 1:
                                    _item.Enqueue(bevrage);
                                    Console.WriteLine("Producer has produced a beer");
                                    break;
                                case 2:
                                    _item.Enqueue(bevrage);
                                    Console.WriteLine("Producer has produced a soda");
                                    break;
                            }

                            Thread.Sleep(TimeSpan.FromSeconds(1));
                        }
                        Console.WriteLine("Producer Waits...");
                        Monitor.PulseAll(_item);
                    }
                }
                finally
                {
                    Monitor.Exit(_item);
                }
            }
        }

        public static void Sorting()
        {
            while (true)
            {
                Monitor.Enter(_item);
                Monitor.Enter(_beer);
                Monitor.Enter(_soda);
                try
                {
                    // Checking if _item is equal to 0
                    while (_item.Count == 0)
                    {
                        Monitor.Wait(_item);
                    }

                    Console.WriteLine("Sorting bottles...");
                    // For loop that loops 10 times and sort the two types of bevrage.
                    for (int i = 0; i < 10; i++)
                    {
                        if (_item.Peek() == 1)
                        {
                            _item.Dequeue();
                            _beer.Enqueue("Beer");
                            Console.WriteLine("Beer bottle has been sorted.");
                        }
                        else
                        {
                            _item.Dequeue();
                            _soda.Enqueue("Soda");
                            Console.WriteLine("Soda bottle has been sorted.");
                        }
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }

                    if (_item.Count == 0)
                    {
                        Console.WriteLine("Sorter waits...");
                    }
                    Console.WriteLine("Bottles have been sorted.");
                    Monitor.PulseAll(_item);
                    Monitor.PulseAll(_beer);
                    Monitor.PulseAll(_soda);
                }
                finally
                {
                    Monitor.Exit(_item);
                    Monitor.Exit(_beer);
                    Monitor.Exit(_soda);
                }
            }
        }

        public static void GetBeer()
        {
            while (true)
            {
                Monitor.Enter(_beer);
                try
                {
                    while (_beer.Count == 0)
                    {
                        Monitor.Wait(_beer);
                    }
                    
                    Console.WriteLine("Beer consumer has consumed one beer");
                    Console.WriteLine("Number of beer bottles in beer consumer: " + _beer.Count);
                    _beer.Dequeue();
                    if (_beer.Count == 0)
                    {
                        Console.WriteLine("Beer consumer waits...");
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                finally
                {
                    Monitor.Exit(_beer);
                }
            }
        }
        public static void GetSoda()
        {
            while (true)
            {
                Monitor.Enter(_soda);
                try
                {
                    while (_soda.Count == 0)
                    {
                        Monitor.Wait(_soda);
                    }
                    
                    Console.WriteLine("Soda consumer has consumed one soda");
                    Console.WriteLine("Number of Soda bottles in soda consumer: " + _soda.Count);
                    _soda.Dequeue();
                    if (_soda.Count == 0)
                    {
                        Console.WriteLine("Soda consumer waits...");
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                finally
                {
                    Monitor.Exit(_soda);
                }
            }
        }
    }
}
