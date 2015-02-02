using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CodeCipher
{
    class DictionarySorter
    {
        /// <summary>
        /// Every word in the dictionary, in an array
        /// </summary>
        String[] masterArray;
        public String[] getMasterArray() { return masterArray; }

        /// <summary>
        /// <para>String arrays sorted by length</para>
        /// <para>[WordLength][Word]</para>
        /// </summary>
        List<String[]> masterArraySortedByLength;
        public List<String[]> getSortedMasterArray() { return masterArraySortedByLength; }

        /// <summary>
        /// <para>Dictionary containing a pattern and all the words that match it</para>
        /// <para>Key: Pattern</para>
        /// <para>Value: List of Strings</para>
        /// </summary>
        SortedDictionary<String, List<String>> masterPatternDictionary;
        public SortedDictionary<String, List<String>> getMasterDictionary() { return masterPatternDictionary; }

        

        public DictionarySorter()
        {
            // Initialize the master dictionary
            masterPatternDictionary = new SortedDictionary<String, List<String>>();
            
            // Clean up the old keys
            Console.WriteLine("Cleaning up...");
            System.IO.File.Delete(@"output.txt");

            // Load the dictionary
            Console.WriteLine("Loading...");

            if (System.IO.File.Exists(@"words.txt"))
            {
                masterArray = System.IO.File.ReadAllLines(@"words.txt");

                // Go through and put all to lowercase
                for (int i = 0; i < masterArray.Length; i++)
                {
                    masterArray[i] = masterArray[i].ToLower();
                }

                // Delete the dupes
                masterArray = masterArray.Distinct().ToArray();

                // Populate the sorted dictionary
                sortByLength();

                // Generate the keys
                Console.WriteLine("Generating Keys...");
                masterPatternDictionary = assignWordPatterns(masterArray);

                // And write them to file
                writeKeys();

                Console.WriteLine("Done!");
            }
            else
                Console.WriteLine("Couldnt find words.txt!! Is it in the same directory as the exe?");
                
        }

        private void writeKeys()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"output.txt", true))
            {
                foreach (KeyValuePair<String, List<String>> KVP in masterPatternDictionary)
                {
                    file.WriteLine(KVP.Key);
                }
            }
        }
        
        private void sortByLength()
        {
            List<String> tempList = new List<String>();
            masterArraySortedByLength = new List<String[]>();
            
            // Current length were testing
            int testLength = 1;
            // Wether we're done or not
            bool running = true;

            while (running)
            {
                bool sccs = false;
                
                // Iterate through all the words
                for (int i = 0; i < masterArray.Length; i ++)
                {
                    // If the current word is equal to our testing length
                    if (masterArray[i].Length == testLength)
                    {
                        // Add it to the temp list and set sccs to true
                        tempList.Add(masterArray[i]);
                        sccs = true;
                    }
                }
                // Add the list of similar length strings to the master array
                masterArraySortedByLength.Add(tempList.ToArray());
                tempList.Clear();

                if (sccs == false)
                    running = false;
                else
                    testLength++;
            }
            
        }

        // This is public, as we will be using it for both keying the dictionary, and the string that we are decoding
        public SortedDictionary<String, List<String>> assignWordPatterns(String[] dictionary)
        {
            SortedDictionary<String, List<String>> tempDict = new SortedDictionary<String, List<String>>();
            
            // Get the pattern for the set of words that we are given
            foreach (String stringThing in dictionary)
            {
                // Get the pattern for the current word
                String temp = getWordPattern(stringThing);

                // If the dict contains that pattern, add the string to the list of values(strings) that match that pattern
                if (tempDict.ContainsKey(temp))
                    tempDict[temp].Add(stringThing);

                // If the dict doesn't contain that pattern, add it and map the value to the key
                else
                {
                    List<String> tmpList = new List<String>();
                    tmpList.Add(stringThing);
                    tempDict.Add(temp, tmpList);
                }
            }

            // Return the dict of patterns(key), and matched words(value)
            return tempDict;
        }

        // Take a string, and return its pattern
        public String getWordPattern(String str)
        {
            // A pattern is the words "signature"
            // So, words like ENERGETIC and EMERGENCY have the same "signature", being ABACDAEFG
            // We can use this to find possible matches for words based on their signature

            // Start and end array so we don't have to char jockey
            Char[] initCharArray = str.ToCharArray();
            Char[] finalCharArray = new Char[initCharArray.Length];

            // The current key we are applying
            Char currentKey;
            // Key value in ASCII, 97 = a, 122 = z
            int keyValue = 97;
            

            for (int i = 0; i < initCharArray.Length; i++)
            {
                // Set the current key, based on the ASCII value
                currentKey = (char)keyValue;
                // The found match makes sure that we don't skip letters
                bool foundMatch = false;

                // If this is the first letter, set it to a
                if (i == 0)
                    finalCharArray[i] = currentKey;
                
                // If it isn't the first letter, reverse through the list and see if it matches any of the previous letters
                else
                    for (int x = i - 1; x > -1; x--)
                    {
                        if (initCharArray[x] == initCharArray[i])
                        {
                            // If it does match, set it to the same letter
                            finalCharArray[i] = finalCharArray[x];
                            foundMatch = true;
                        }
                    }

                // If we weren't able to find a match, we assign it a new key and then step the key one further
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
