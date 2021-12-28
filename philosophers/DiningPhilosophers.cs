namespace DiningPhilosophers
{
    internal static class Configuration
    {
        public static int PhilosophersCount => 10;
        public static int FoodCount => 10;
        public static int MaxAttempts => 100;
    }

    internal class Philosopher
    {
        public Philosopher(int number, Chopstick leftChopstick, Chopstick rightChopstick)
        {
            Number = number;
            Food = Configuration.FoodCount;
            LeftChopstick = leftChopstick;
            RightChopstick = rightChopstick;
            Log = new List<string> { $"{DateTime.Now}: Философ {number} сел за стол." };
        }

        public int Number { get; }
        public int Food { get; set; }
        public Chopstick LeftChopstick { get; set; }
        public Chopstick RightChopstick { get; set; }
        public List<string> Log { get; set; }

        public void EatUntilDone()
        {
            int attempts = 1;
            while (Food > 0 && attempts < Configuration.MaxAttempts)
            {
                Thread.Sleep(100);
                Log.Add(@$"{DateTime.Now}: Философ {Number} пытается поесть в {attempts} раз.");
                if (PickUpChopstics())
                {
                    Log.Add(@$"{DateTime.Now}: Философ {Number} поднял палочки.");
                    this.Eat();
                    Log.Add(@$"{DateTime.Now}: Философ {Number} поел.");
                    this.PutDownLeftChopstick();
                    this.PutDownRightChopstick();
                    Log.Add(@$"{DateTime.Now}: Философ {Number} положил палочки.");
                }
                else
                {
                    Log.Add(@$"{DateTime.Now}: Философ {Number} не может поднять палочки.");
                }
                attempts++;
            }

            if (Food == 0)
            {
                Log.Add($"{DateTime.Now}: Философ {Number} наелся за {attempts} попыток.");
            }
            else
            {
                Log.Add($"{DateTime.Now}: Философ {Number} умер от истощения.");
            }
        }

        private bool PickUpChopstics()
        {
            if (Monitor.TryEnter(LeftChopstick))
            {
                if (Monitor.TryEnter(RightChopstick))
                {
                    Console.WriteLine($"Философ {Number} поднял палочки.");
                    return true;
                }
                else
                {
                    PutDownLeftChopstick();
                }
            }

            Console.WriteLine($"Философ {Number} не смог поднять палочки.");
            return false;
        }

        private void PutDownLeftChopstick()
        {
            Monitor.Exit(this.LeftChopstick);
        }

        private void PutDownRightChopstick()
        {
            Monitor.Exit(this.RightChopstick);
        }


        private void Eat()
        {
            Thread.Sleep(100);
            Food--;
        }
    }

    internal class Chopstick
    {
        public Chopstick(int number)
        {
            Number = number;
        }

        public int Number { get; }
    }

    internal class Table
    {
        public Table()
        {
            Philosophers = new Dictionary<int, Philosopher>();
            Chopsticks = new Dictionary<int, Chopstick>();

            for (int i = 1; i <= Configuration.PhilosophersCount; i++)
            {
                Chopsticks.Add(i, new Chopstick(i));
            }

            for (int i = 1; i <= Configuration.PhilosophersCount; i++)
            {
                var leftChopstick = Chopsticks[i];
                var rightChopstick = Chopsticks[i + 1 <= Configuration.PhilosophersCount ? i + 1 : 1];
                Philosophers.Add(i, new Philosopher(i, leftChopstick, rightChopstick));
            }
        }

        public Dictionary<int, Philosopher> Philosophers { get; private set; }
        public Dictionary<int, Chopstick> Chopsticks { get; private set; }

        public void Log()
        {
            Console.WriteLine($"======================================================");
            foreach ((var key, var value) in Philosophers)
            {
                Console.WriteLine($"Философ {key} | Еда {value.Food}");
            }
            Console.WriteLine($"======================================================");

        }

        public void LogPhilosophers()
        {
            foreach ((var key, var value) in Philosophers)
            {
                Console.WriteLine($"======================================================");
                Console.WriteLine($"История ужина философа {key}");
                foreach (var record in value.Log)
                {
                    Console.WriteLine(record);
                }
                Console.WriteLine($"======================================================");
            }
            foreach ((var key, var value) in Philosophers)
            {
                Console.WriteLine(value.Log.Last());
            }
        }
    }
}
