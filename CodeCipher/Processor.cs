using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCipher
{
    class Processor
    {
        /// <summary>
        /// The input cipher, unmolested, seperated out into an array
        /// </summary>
        String[] inputCipher;
        /// <summary>
        /// Final answer key, mapping a single char to a single char
        /// </summary>
        SortedDictionary<Char, Char> finalLetterDict;
        // The "runners up"
        SortedDictionary<Char, List<Char>> possibleLetterDict;

        // Contains the keys for every [value] in inputCipher
        /// <summary>
        /// (Calculated KEY)(Value from INPUT)
        /// </summary>
        SortedDictionary<String, List<String>> inputKeyDictionary;
        // Matches the key to possible values in the dictionary
        /// <summary>
        /// <para>First value is the word that we need to translate</para>
        /// <para>Second is the list of all words that could be it</para>
        /// </summary>
        Dictionary<String, List<String>> possibleValuesDict;

        /// <summary>
        /// The Char, A-Z, and then a list of lists of possible values
        /// <para>[a][list of possible values for that a]</para>
        /// <para>[b][list of possible values for that b</para>
        /// <para>[a][list of possible values for that seperate a</para>
        /// <para>So I can use this to find the char with the least possibilites and then</para>
        /// <para>use that to cross reference and widdle down the options</para>
        /// </summary>
        Dictionary<Char, List<List<Char>>> possibleCharValDict;

        // Reference to the dictSorter
        DictionarySorter dictSorter;

        public Processor(DictionarySorter dictSorter)
        {
            // Get a reference to the dictSorter for comparing values
            this.dictSorter = dictSorter;
            inputKeyDictionary = new SortedDictionary<String, List<String>>();

            // populate the final dict
            finalLetterDict = new SortedDictionary<Char, Char>();
            for (int i = 0; i < 26; i++)
            {
                Char value = (Char)(i + 97);

                finalLetterDict.Add(value, '-');
            }
        }

        public void processInput(String input)
        {
            Console.WriteLine("Processing input...");
            // Split it out into the string array
            inputCipher = input.Split(' ');
            // get the patterns and put them into the inputKeyDictionary
            assignWordKeys();
            // Find all the matches and populate possibleMatchDictionary
            buildPossibleWordDict();
            buildPossibleValueDict();
            compareValues();
            compareValues();
            compareValues();
            foreach (KeyValuePair<Char, Char> value in finalLetterDict)
                Console.WriteLine(value.Key + " " + value.Value);
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

        private void buildPossibleWordDict()
        {
            List<String> wordList;
            possibleValuesDict = new Dictionary<string, List<string>>();
            // For each key inside the dictionary
            foreach (String thing in inputKeyDictionary.Keys)
            {
                // Get each value from it
                foreach (String thing1 in inputKeyDictionary[thing])
                {
                    if (dictSorter.getMasterDictionary().ContainsKey(thing))
                    {
                        wordList = dictSorter.getMasterDictionary()[thing];
                        if (!possibleValuesDict.ContainsKey(thing1))
                            possibleValuesDict.Add(thing1, wordList);
                    }
                }
            }
        }

        private void buildPossibleValueDict()
        {

            possibleCharValDict = new Dictionary<Char, List<List<Char>>>();
            
            // Populate the encrypted char alphabet
            for (int i = 0; i < 26; i++)
            {
                Char value = (Char)(i + 97);

                possibleCharValDict.Add(value, new List<List<Char>>());
            }
            // Start going through the possible values dict
            foreach (KeyValuePair<String, List<String>> dictItem in possibleValuesDict)
            {
                // Cycle through the string and get all the possible chars for the encrypted char for that string
                for (int i = 0; i < dictItem.Key.Length; i++)
                {
                    Char currentEncryptedChar = dictItem.Key[i];
                    List<Char> listOfPosiblVals = new List<char>();

                    foreach (String possibleString in dictItem.Value)
                    {
                        if (!listOfPosiblVals.Contains(possibleString[i]))
                            listOfPosiblVals.Add(possibleString[i]);
                    }

                    possibleCharValDict[currentEncryptedChar].Add(listOfPosiblVals);
                }
            }
        }

        // Go through all the combinations and find the one with the least amount of possible options
        // then compare
        private void compareValues()
        {

            foreach (KeyValuePair<Char, List<List<Char>>> KVP in possibleCharValDict)
            {
                List<Char> lowestValueSet = new List<Char>();
                bool empty = true;
                foreach (List<Char> charList in KVP.Value)
                {
                    if (charList.Count < lowestValueSet.Count || empty == true)
                    {
                        empty = false;
                        lowestValueSet = charList;
                    }
                }

                // KVP.Value.Count is theamount of seperate list of possible chars for the letter
                for (int i = 0; i < KVP.Value.Count; i++)
                {
                    
                    // So I first get the amount of seperate lists, and now i go into a single list and count its values
                    for (int x = KVP.Value[i].Count - 1; x > -1; x--)
                    {
                        if (!lowestValueSet.Contains(KVP.Value[i][x]))
                        {
                            KVP.Value[i].Remove(KVP.Value[i][x]);
                        }
                    }    
    
                    if (KVP.Value[i].Count == 1)
                    {
                        finalLetterDict[KVP.Key] = KVP.Value[i][0];
                        cullCorrectValues(KVP.Value[i][0]);
                    }
                } 
            }
        }

        private void cullCorrectValues(Char valueToCull)
        {

            foreach (KeyValuePair<Char, List<List<Char>>> KVP in possibleCharValDict)
            {
                foreach (List<Char> charList in KVP.Value)
                {
                    charList.RemoveAll(item => item == valueToCull);
                }
            }
        }
    }
}
