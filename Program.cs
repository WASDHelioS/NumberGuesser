using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace NumberGuesser
{
    public enum GameState
    {
        Started, Finished
    }

    class Program
    {
        static void Main(string[] args)
        {
            GameState state = new GameState();
            Guesser guesser = new Guesser();
            FileHandler fh = new FileHandler();
            Random rand = new Random();

            NumberGenerator.Seed = fh.GetFileContent();
            int randomNumber = NumberGenerator.Generate(rand);
            int guess;

            Console.WriteLine("Welcome to the Number Guesser game! I have picked a random number between 1 and 10. Your job is to guess said number. Good luck!");
            state = GameState.Started;
            int score = 1;

            

            while (state == GameState.Started)
            {
                Console.WriteLine("Please enter your guess: ");

                int? temp = guesser.ConvertStringToInt(Console.ReadLine());

                if (temp.HasValue)
                {
                    guess = temp.GetValueOrDefault();
                    if (guesser.IsGuessedNumberCorrect(randomNumber, guess))
                    {
                        state = GameState.Finished;
                    }
                    else
                    {
                        score++;
                    }
                }
            }

            Console.WriteLine("Saving data..");
            fh.UpdateFileContent(randomNumber, score);
            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
    class Guesser
    {
        public bool IsGuessedNumberCorrect(int randomNumber, int guess)
        {

            if (randomNumber == guess)
            {
                Console.WriteLine("You did it! Congratulations.");
                return true;
            }
            else
            {
                Console.WriteLine("Incorrect number. Please try again.");
                return false;
            }
        }
        public int? ConvertStringToInt(string guess)
        {
            int? temp = null;
            try
            {
                temp = Convert.ToInt32(guess);
            }
            catch (FormatException)
            {
                Console.WriteLine("This is not a number!");
                return null;
            }
            catch (OverflowException)
            {
                Console.WriteLine("This number is too big. Please try a number between 1 and 10.");
                return null;
            }
            return temp;
        }
    }

    public class FileHandler
    {
        string currentdir = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string[] init =
        {
            "1,1","2,1","3,1","4,1","5,1","6,1","7,1","8,1","9,1","10,1"
        };

        public void UpdateFileContent(int randomNumber, int score)
        {
            int oldScore;
            int newScore;

            string[] text = File.ReadAllLines(currentdir + @"\Data\Scores.txt");
            for(int i = 0; i < text.Length; i++)
            {
                string [] temp = Regex.Split(text[i], ",");

                if(temp[0] == randomNumber.ToString())
                {
                    Int32.TryParse(temp[1],out oldScore);
                    newScore = oldScore + score;

                    text[i] = randomNumber + "," + newScore;
                    Console.WriteLine(text[i]);
                }
            }
            File.WriteAllLines(currentdir + @"\Data\Scores.txt", text);
        }
        public string[] GetFileContent()
        {
            try
            {
                Console.WriteLine("Initializing.. ");

                return File.ReadAllLines(currentdir + @"\Data\Scores.txt");
            }
            catch(System.IO.DirectoryNotFoundException)
            {
                Console.WriteLine("First startup detected. Creating necessary folders..");
                Directory.CreateDirectory(currentdir + @"\Data\");
                File.AppendAllLines(currentdir + @"\Data\Scores.txt", init);
                Console.WriteLine("Done!");

                return init;
            }
            catch(System.IO.FileNotFoundException)
            {
                Console.WriteLine("File missing. Creating new file..");
                File.AppendAllLines(currentdir + @"\Data\Scores.txt", init);
                Console.WriteLine("Done!");
                
                return init;
            }
        }
    }

    static class NumberGenerator
    {
        private static string[] seed;

        public static string[] Seed { get => seed; set => seed = value; }
        private static Dictionary<int, int> numbers = new Dictionary<int, int>();

        public static int Generate(Random rand)
        {
            Console.WriteLine("Entered Generate");
            PopulateNumbers();
            Console.WriteLine("Numbers Populated in dict");
            int sum = CalculateSum();
            int random = rand.Next(1,sum-1);

            for(int i = 1; i<numbers.Count; i++)
            {
                numbers.TryGetValue(i, out int t);
                Console.WriteLine("Current sum:"+ random + ". Subtracting "+ t);
                random -= t;
                
                if (random <= 0) return i;
            }
            return rand.Next(1, 11);
        }

        private static void PopulateNumbers()
        {
            foreach (string s in seed)
            {
                string[] temp = Regex.Split(s, ",");
                int t1;
                int t2;
                Int32.TryParse(temp[0], out t1);
                Int32.TryParse(temp[1], out t2);
                numbers.Add(t1, t2);
            }
        }
        private static int CalculateSum()
        {
            int sum = 0;
            for (int i = 1; i < numbers.Count; i++)
            {
                numbers.TryGetValue(i, out int t);
                sum += t;
            }
            return sum;
        }
    }
}

