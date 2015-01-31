using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCipher
{
    class Processor
    {
        String[] inputCipher;
        SortedDictionary<String, List<String>> inputDictionary;
        SortedDictionary<String, List<String>> possibleMatchDictionary;

        DictionarySorter dictSorter;

        public Processor(DictionarySorter dictSorter)
        {
            // Get a reference to the dictSorter for comparing values
            this.dictSorter = dictSorter;
            inputDictionary = new SortedDictionary<String, List<String>>();
        }

        public void processInput(String input)
        {
            inputCipher = input.Split(' ');
            assignWordPatters();
            buildComparisonList();
        }

        private void assignWordPatters()
        {

            foreach (String stringThing in inputCipher)
            {
                String temp = DictionarySorter.getWordPattern(stringThing);

                if (inputDictionary.ContainsKey(temp))
                    inputDictionary[temp].Add(stringThing);
                else
                {
                    List<String> tmpList = new List<String>();
                    tmpList.Add(stringThing);
                    inputDictionary.Add(temp, tmpList);
                }
            }
        }

        private void buildComparisonList()
        {
            List<String> inputKeys = new List<string>();
            List<String> dictKeys;

            foreach (String thing in inputDictionary.Keys)
                inputKeys.Add(thing);

            foreach (String thing in inputKeys)
            {
                if (dictSorter.getMasterDictionary().ContainsKey(thing))
                {
                    dictKeys = dictSorter.getMasterDictionary()[thing];
                    foreach (String thing1 in dictKeys)
                        Console.WriteLine(thing1);
                }

                // australopithecus is a direct match for embyitpwbgjvlmpv
            }
        }
    }
}
