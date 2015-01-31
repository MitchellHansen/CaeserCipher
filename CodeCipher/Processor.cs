using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCipher
{
    class Processor
    {
        // The input string seperated out into an array
        String[] inputCipher;
        // The final answer key, where one char is mapped to one other char
        SortedDictionary<Char, Char> finalLetterDict;
        // The "runners up"
        SortedDictionary<Char, List<Char>> possibleLetterDict;

        // Contains the keys for every [value] in inputCipher
        SortedDictionary<String, List<String>> inputKeyDictionary;
        // Matches the key to possible values in the dictionary
        SortedDictionary<String, List<String>> possibleMatchDictionary;

        // Reference to the dictSorter
        DictionarySorter dictSorter;

        public Processor(DictionarySorter dictSorter)
        {
            // Get a reference to the dictSorter for comparing values
            this.dictSorter = dictSorter;
            inputKeyDictionary = new SortedDictionary<String, List<String>>();
        }

        public void processInput(String input)
        {
            // Split it out into the string array
            inputCipher = input.Split(' ');
            // get the patterns and put them into the inputKeyDictionary
            assignWordKeys();
            // Find all the matches and populate possibleMatchDictionary
            buildComparisonList();
        }

        // Calculate the words into their keys and put it into the inputKeyDictionary
        private void assignWordKeys()
        {
            
            foreach (String stringThing in inputCipher)
            {
                // Get the key from the getWord method
                String temp = DictionarySorter.getWordPattern(stringThing);

                // If the key is already in the dict, add the value to the list
                if (inputKeyDictionary.ContainsKey(temp))
                    inputKeyDictionary[temp].Add(stringThing);
                // If not, create a new list, and add the key value pair to the dict
                else
                {
                    List<String> tmpList = new List<String>();
                    tmpList.Add(stringThing);
                    inputKeyDictionary.Add(temp, tmpList);
                }
            }
        }

        private void buildComparisonList()
        {
            List<String> inputKeys = new List<string>();
            List<String> keyList;
            possibleMatchDictionary = new SortedDictionary<string, List<string>>();

            foreach (String thing in inputKeyDictionary.Keys)
                inputKeys.Add(thing);

            // Keep track of where we are, shame on me for not using a for loop
            int i = 0;
            foreach (String thing in inputKeyDictionary.Keys)
            {
                if (dictSorter.getMasterDictionary().ContainsKey(thing))
                {
                    keyList = dictSorter.getMasterDictionary()[thing];
                    if (!possibleMatchDictionary.ContainsKey(inputCipher[i]))
                        possibleMatchDictionary.Add(inputCipher[i], keyList);
                    Console.WriteLine(inputCipher[i]);
                    Console.WriteLine(keyList[1]);
                }
                i++;

                // australopithecus is a direct match for embyitpwbgjvlmpv
            }
        }
    }
}
