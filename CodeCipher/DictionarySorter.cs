using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CodeCipher
{
    class DictionarySorter
    {
        // 2.5 MB text file dictionary
        String[] masterArray;
        
        public String[] getMasterArray() { return masterArray; }
        // List of string arrays, so..
        //foreach (String thing in sortedListByLen[1])
        //    Console.WriteLine(thing);
        // would list all the string of length 2 for example
        // Hoping to use this to speed up the program
        List<String[]> sortedListByLen;
        List<String> tempList;

        // Put all the words into a dictionary where you can look up the pattern
        SortedDictionary<String, List<String>> masterDictionary;
        public SortedDictionary<String, List<String>> getMasterDictionary() { return masterDictionary; }

        

        public DictionarySorter()
        {
            // Initialize the master dictionary
            masterDictionary = new SortedDictionary<String, List<String>>();
            
            // Clean up the old keys
            Console.WriteLine("Cleaning up...");
            System.IO.File.Delete(@"output.txt");

            // Load the dictionary, can use different dictionaries, so it cleans it up each time
            Console.WriteLine("Loading...");

            if (System.IO.File.Exists(@"words.txt"))
            {
                masterArray = System.IO.File.ReadAllLines(@"words.txt");
                for (int i = 0; i < masterArray.Length; i++)
                {
                    masterArray[i] = masterArray[i].ToLower();
                }

                masterArray = masterArray.Distinct().ToArray();
                sortByLength();

                // Generate the keys and populate the masterDictionary
                Console.WriteLine("Generating Keys...");
                masterDictionary = assignWordPatterns(masterArray);
                writeKeys();
            }
            else
                Console.WriteLine("Couldnt find words.txt!! Is it in the same directory as the exe?");
                
        }

        private void writeKeys()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"output.txt", true))
            {
                foreach (KeyValuePair<String, List<String>> thing in masterDictionary)
                {
                    file.WriteLine(thing.Key);
                }
            }
        }
        
        private void sortByLength()
        {
            tempList = new List<String>();
            sortedListByLen = new List<String[]>();
            
            // Current testing length
            int testLength = 1;
            // Wether we're done or not
            bool running = true;

            while (running)
            {
                bool sccs = false;
                
                for (int i = 0; i < masterArray.Length; i ++)
                {
                    if (masterArray[i].Length == testLength)
                    {
                        tempList.Add(masterArray[i]);
                        sccs = true;
                    }
                }

                // Go to the next testing length
                testLength++;
                // Put the list into an array for storage
                String[] transferString = tempList.ToArray();
                tempList.Clear();
                // Add it to the storage list
                sortedListByLen.Add(transferString);
                
                // If we reached the largest words in the dict, end the loop
                if (sccs == false)
                    running = false;
            }
            
        }

        public static SortedDictionary<String, List<String>> assignWordPatterns(String[] dictionary)
        {
            SortedDictionary<String, List<String>> tempDict = new SortedDictionary<String, List<String>>();
            
            foreach (String stringThing in dictionary)
            {
                // Poor ram
                String temp = getWordPattern(stringThing);

                if (tempDict.ContainsKey(temp))
                    tempDict[temp].Add(stringThing);
                else
                {
                    List<String> tmpList = new List<String>();
                    tmpList.Add(stringThing);
                    tempDict.Add(temp, tmpList);
                }
            }

            return tempDict;
        }

        // Give it a one String and it will return one word pattern
        public static String getWordPattern(String str)
        {
            // beginning array and end array, both the same size
            Char[] initCharArray = new Char[str.Length];
            initCharArray = str.ToCharArray();
            Char[] finalCharArray = new Char[initCharArray.Length];

            // The current key we are applying
            Char currentKey;
            //97 = a
            //122 = z
            // Key value in ASCII
            int keyValue = 97;
            
            // The meat
            for (int i = 0; i < initCharArray.Length; i++)
            {
                // Set the current key, based on the ASCII value
                currentKey = (char)keyValue;
                // The found match makes sure that we don't skip letters
                bool foundMatch = false;

                // Special case for the first letter
                if (i == 0)
                    finalCharArray[i] = currentKey;
                
                // Else reverse iterate through the word and see if there is a match, is so, give it the same
                // char and set the foundmatch to true
                else
                    for (int x = i - 1; x > -1; x--)
                    {
                        if (initCharArray[x] == initCharArray[i])
                        {
                            finalCharArray[i] = finalCharArray[x];
                            foundMatch = true;
                        }
                    }
                // If we weren't able to find a match, give it a new keycode out of the stack and iterate the next one by 1
                if (foundMatch == false)
                {
                    finalCharArray[i] = currentKey;
                    keyValue++;
                }
            }
            // Wahoo!
            return new String(finalCharArray);

        }
    }
}
