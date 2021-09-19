using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace TennisClub
{
    class Program
    {
        static StringBuilder Log;

        //Function that calculates the sum of the first and last digit of a number
        private static int SumFirstAndLast(long number)
        {
            int last = (int)(number % 10);
            int count = (int)Math.Log10(number);
            int first = (int)(number / Math.Pow(10, count));
            return first + last;
        }

        //Recursive Function that reduces the number to two digits
        private static string Reduce(string number)
        {
            if (number.Length <= 2)
                return number;

            StringBuilder builder = new StringBuilder();

            while (number.Length > 1)
            {
                builder.Append(SumFirstAndLast(long.Parse(number)));
                number = number.Substring(1, number.Length - 2);
            }

            if (number.Length == 1)
                builder.Append(number);

            return Reduce(builder.ToString());
        }

        //Function that calculates the percentage that two names match eachother
        private static int CalcPercentage(string names)
        {
            Dictionary<char, int> freq = new Dictionary<char, int>();
            foreach (char c in names)
            {
                if (c.Equals(' '))
                    continue;

                int count;
                if (!freq.TryGetValue(c, out count))
                {
                    count = 0;
                }
                freq[c] = count + 1;
            }

            StringBuilder builder = new StringBuilder();
            foreach (var character in freq)
                builder.Append(character.Value);

            string reducedNum = Reduce(builder.ToString());

            return int.Parse(reducedNum);
        }

        //Function that returns a dictionary with the percentage matches of two names
        private static Dictionary<string, int> GetMatchPercentages(HashSet<string> set1, HashSet<string> set2)
        {
            Log.Append("\nMatch Percentaged Calculated. " + DateTime.Now.ToString());
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (var name1 in set1)
            {
                foreach (var name2 in set2)
                {
                    string sentence = name1 + " matches " + name2;
                    int percentage = CalcPercentage(sentence);
                    dict.Add(sentence, percentage);
                }
            }
            return dict;
        }

        //Function that returns a dictionary with the average percentage matches of two names
        private static Dictionary<string, int> GetAvgMatchPercentages(HashSet<string> set1, HashSet<string> set2)
        {
            Log.Append("\nMatch Percentaged Calculated. " + DateTime.Now.ToString());
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (var name1 in set1)
            {
                foreach (var name2 in set2)
                {
                    string sentence = name1 + " matches " + name2;
                    string sentence_reversed = name2 + " matches " + name1;
                    int percentage = CalcPercentage(sentence);
                    int percentage_reversed = CalcPercentage(sentence_reversed);
                    int avg = (percentage + percentage_reversed) / 2;
                    dict.Add(sentence, avg);
                }
            }
            return dict;
        }

        private static void endOfLog(DateTime date1, string LogPath)
        {
            Log.Append("\nProgram Executed. " + DateTime.Now.ToString());
            DateTime date2 = DateTime.Now;
            Log.Append("\nEnd Time: " + date2.ToString());
            TimeSpan ts = date2.Subtract(date1);
            Log.Append(string.Format("\nProgram took {0} seconds to complete.", ts.TotalSeconds.ToString()));
            using (StreamWriter sw = File.AppendText(LogPath))
            {
                sw.WriteLine(Log.ToString());
            }
        }
        //Function that sorts the dictionary by value in descending order, then by key in ascending order
        private static IOrderedEnumerable<KeyValuePair<string, int>> DictionarySort(Dictionary<string, int> dict)
        {
            return dict.OrderByDescending(i => i.Value).ThenBy(i => i.Key);

        }

        static void Main(string[] args)
        {
            DateTime date1 = DateTime.Now;
            Log = new StringBuilder();
            string LogPath = string.Format(@"{0}\log.txt", Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));
            Console.WriteLine("Log file can be found here: {0}", LogPath);

            if (!File.Exists(LogPath))
            {
                using (StreamWriter sw = File.CreateText(LogPath))
                {
                    sw.WriteLine("Log file created. " + DateTime.Now.ToString());
                }
            }

            Log.Append("Start Time: " + date1.ToString());

            string done;
            do
            {
                done = "";

                string filePath = "";
                while (filePath.Length == 0) //Ensures a file path is entered
                {
                    Console.Write("Enter file path: ");
                    filePath = Console.ReadLine().Trim();
                    Log.Append("\nFile Path was entered. " + DateTime.Now.ToString());
                    if (filePath.Length == 0)
                    {
                        Console.WriteLine("Please enter a file path.");
                        Log.Append("\nNo File Path was entered." + DateTime.Now.ToString());
                    }
                }
                Log.Append("\nFile Path: " + filePath + " " + DateTime.Now.ToString());

                try
                {
                    if (!File.Exists(filePath)) //Ensures file exists
                    {
                        Log.Append("\nFile does not exist. File path: " + filePath + " " + DateTime.Now.ToString());
                        throw new FileNotFoundException();
                    }
                    else if (new FileInfo(filePath).Length == 0) //Ensures file is not empty
                    {
                        Log.Append("\nFile exists, but file is empty. File path: " + filePath + " " + DateTime.Now.ToString());
                        throw new Exception();
                    }

                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("The reqested file at path: {0}, was not found!", filePath);
                    endOfLog(date1, LogPath);
                    Environment.Exit(0);
                }
                catch (Exception)
                {
                    Console.WriteLine("The reqested file at path: {0}, is empty!", filePath);
                    endOfLog(date1, LogPath);
                    Environment.Exit(0);

                }

                //Sets of names to be matched          
                HashSet<string> male = new HashSet<string>();
                HashSet<string> female = new HashSet<string>();

                //Read in names and gender from the file
                StreamReader reader = new StreamReader(File.OpenRead(filePath));
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');

                    if (values.Length >= 2) //Safety check to ensure IndexOutOfRangeException does not occur
                    {
                        if (values[1].Trim().Equals("m"))
                        {
                            if (Regex.IsMatch(values[0], @"^[a-zA-Z]+$")) //Ensures only alphabetic characters are accepted
                            {
                                male.Add(values[0]);
                            }
                        }
                        else if (values[1].Trim().Equals("f"))
                        {
                            if (Regex.IsMatch(values[0], @"^[a-zA-Z]+$"))
                            {
                                female.Add(values[0]);
                            }
                        }
                    }
                }
                reader.Close();
                Log.Append("\nData from file extracted. Data stored in two sets. " + DateTime.Now.ToString());

                // Ensures Sets have data to perform the matching function
                if ((male.Count == 0) && (female.Count == 0))
                {
                    Console.WriteLine("NO DATA IN FILE");
                    Log.Append("\nNo male or female data in file. " + DateTime.Now.ToString());
                    endOfLog(date1, LogPath);
                    Environment.Exit(0);
                }
                else if (male.Count == 0)
                {
                    Console.WriteLine("NO MALE DATA IN FILE");
                    Log.Append("\nNo male data in file. " + DateTime.Now.ToString());
                    endOfLog(date1, LogPath);
                    Environment.Exit(0);
                }
                else if (female.Count == 0)
                {
                    Console.WriteLine("NO FEMALE DATA IN FILE");
                    Log.Append("\nNo female data in file. " + DateTime.Now.ToString());
                    endOfLog(date1, LogPath);
                    Environment.Exit(0);
                }

                //Answers optional question 2
                string average = "";
                while (!(average.Equals("YES") || average.Equals("NO")))
                {
                    Console.WriteLine("Would you like the average of data (Per optional question 2)? Type YES or NO");
                    average = Console.ReadLine().ToUpper().Trim();
                }

                IOrderedEnumerable<KeyValuePair<string, int>> matches;
                if (average.Equals("YES"))
                {
                    matches = DictionarySort(GetAvgMatchPercentages(male, female));
                    Log.Append("\nAverage match percentage of names was requested. " + DateTime.Now.ToString());
                }
                else
                {
                    matches = DictionarySort(GetMatchPercentages(male, female));
                    Log.Append("\nMatch percentage of names was requested. " + DateTime.Now.ToString());
                }

                //Writing to the output file
                string outputPath = string.Format(@"{0}\output.txt", Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));
                using (StreamWriter sw = File.CreateText(outputPath))
                {
                    foreach (var match in matches)
                    {
                        string sentence = match.Key;
                        int percentage = match.Value;
                        if (percentage > 80)
                        {
                            sw.WriteLine("{0} {1}%, good match!", sentence, percentage);
                        }
                        else
                        {
                            sw.WriteLine("{0} {1}%", sentence, percentage);
                        }
                    }
                }
                Console.WriteLine("File created at: {0}", outputPath);
                Log.Append("\nOutput file was created. Output file path: " + outputPath + " " + DateTime.Now.ToString());

                reader = new StreamReader(File.OpenRead(outputPath));
                while (!reader.EndOfStream)
                {
                    Console.WriteLine(reader.ReadLine());
                }
                reader.Close();


                while (!(done.Equals("YES") || done.Equals("NO")))
                {
                    Console.WriteLine("Enter another file path? Type YES or NO");
                    done = Console.ReadLine().ToUpper().Trim();
                }

                if (done.Equals("YES"))
                {
                    Log.Append("\nAnother File Path was requested. " + DateTime.Now.ToString());

                }
                else if (done.Equals("NO"))
                {
                    Log.Append("\nNo other File Path was requested. " + DateTime.Now.ToString());

                }

            } while (!done.Equals("NO"));

            Console.WriteLine("Done!");
            endOfLog(date1, LogPath);
        }
    }
}


