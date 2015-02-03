using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CodeCipher
{
    class Processor
    {
        /// <summary>
        /// The input cipher, unmolested, separated out into an array
        /// </summary>
        String[] inputCipher;

        /// <summary>
        /// Final answer key, mapping a single char to a single char
        /// </summary>
        SortedDictionary<Char, Char> finalAnswerKeyDict;

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
        /// <para>[a][list of possible values for that separate a</para>
        /// <para>So I can use this to find the char with the least possibilities and then</para>
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
            finalAnswerKeyDict = new SortedDictionary<Char, Char>();
            for (int i = 0; i < 26; i++)
            {
                Char value = (Char)(i + 97);

                finalAnswerKeyDict.Add(value, '-');
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
            // Go through the possible word dict and find what each letters possible matches are
            buildPossibleValueDict();
            // Go through that and compare each letter with its multiple sets of options
            compareValues();
            // Go through the possible word dict with our possible values and try to find matches
            findWordMatches();
            // This can be recursive if it's for a hard cipher, should do it in one go most of the time though
            for (int i = 0; i < 2; i++)
            {
                // If we have confirmed words, see if it confirmed any letters
                updateFinalAnswerForConfirmedWords();
                // If more letters have been confirmed, try and find more matches
                findWordMatches();
            }

            foreach (KeyValuePair<Char, Char> value in finalAnswerKeyDict)
                Console.WriteLine(value.Key + " = " + value.Value);

            Console.WriteLine(translateFinaloutput(inputCipher));
        }

        // Calculate the words into their keys and put it into the inputKeyDictionary
        private void assignWordKeys()
        {

            foreach (String stringThing in inputCipher)
            {
                // Get the key from the getWord method
                String temp = dictSorter.getWordPattern(stringThing);

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
            possibleValuesDict = new Dictionary<string, List<string>>();
            // For each key inside the dictionary
            foreach (String thing in inputKeyDictionary.Keys)
            {
                // Get each value from it
                foreach (String thing1 in inputKeyDictionary[thing])
                {
                    if (dictSorter.getMasterDictionary().ContainsKey(thing))
                    {
                        List<String> wordList = new List<String>(dictSorter.getMasterDictionary()[thing]);
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
                        finalAnswerKeyDict[KVP.Key] = KVP.Value[i][0];
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


        // Match the known letters with the values in the possible values dict
        private void findWordMatches()
        {
            // possibleValuesDict holds the encrypted words[Key] and all the possibilities for that word[Value]
            // We need to go through each Key and it's values and try to start narrowing them down based on the letters we have decrypted

            for (int i = 0; i < possibleValuesDict.Count(); i++)
            {

                for (int x = 0; x < possibleValuesDict.ElementAt(i).Value.Count;)
                {
                    // One liner hell, compares the decrypted value with possible matches using the possible values dict
                    String keyDecrypt = compareAndReplace(possibleValuesDict.ElementAt(i).Key);
                    String valueDecrypt = formatForCompareAndReplace(possibleValuesDict.ElementAt(i).Value[x]);
                    if (!keyDecrypt.Equals(valueDecrypt))
                        possibleValuesDict.ElementAt(i).Value.Remove(possibleValuesDict.ElementAt(i).Value[x]);
                    else
                        x++;
                }
            }

        }

        private String compareAndReplace(String encryptedString)
        {
            String output = "";

            for (int i = 0; i < encryptedString.Length; i++)
            {
                output += finalAnswerKeyDict[encryptedString[i]];
            }

            return output;
        }

        private String formatForCompareAndReplace(String unencryptedString)
        {
            String output = "";

            for (int i = 0; i < unencryptedString.Length; i++)
            {
                if (finalAnswerKeyDict.ContainsValue(unencryptedString[i]))
                {
                    output += unencryptedString[i];
                }
                else
                    output += "-";
            }

            return output;
        }

        private void updateFinalAnswerForConfirmedWords()
        {

            foreach (KeyValuePair<String, List<String>> KVP in possibleValuesDict)
            {

                if (KVP.Value.Count == 1)
                {
                    for (int i = 0; i < KVP.Value[0].Length; i++)
                    {
                        finalAnswerKeyDict[KVP.Key[i]] = KVP.Value[0][i];
                    }
                }
                
            }
            
        }

        private String translateFinaloutput(String[] input)
        {
            String finalString = "";

            foreach (String stringThing in input)
            {
                String temp = "";

                for (int i = 0; i < stringThing.Length; i++)
                {
                    temp += finalAnswerKeyDict[stringThing[i]];
                }

                finalString += temp + " ";
            }

            return finalString;
        }


    }
}
