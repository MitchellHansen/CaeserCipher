using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCipher
{
    class Program
    {
        static String inputCipher;
        static DictionarySorter dictSorter;
        static Processor inputProcessor;

        static String defaultCipher = "ng ngnlicpc bo hum embyitpwbgjvlmpv nvpe objge pg hum mhuqbpe zbqmt nge fns wbgmc vbgoptqme hunh hum cdmlmhnl btpapgc smtm qbch lpdmli bo njchtnlbrphumvjc btpapg";
        static char[] defaultCipherArr;

        static void Main(string[] args)
        {
            // Populate the cipher array
            defaultCipherArr = defaultCipher.ToCharArray();

            bool running = true;
            bool initialized = false;

            while (running)
            {
                Console.WriteLine("1.) Initialize\n" +
                                  "2.) Enter custom cipher\n" + 
                                  "3.) Complete assigned cipher\n" +
                                  "4.) Help\n5.) Quit");

                String input = Console.ReadLine();

                switch (input)
                {

                    case "1":
                        dictSorter = new DictionarySorter();
                        inputProcessor = new Processor(dictSorter);
                        initialized = true;
                        break;
                    case "2":
                        if (initialized == false)
                        {
                            dictSorter = new DictionarySorter();
                            inputProcessor = new Processor(dictSorter);
                            initialized = true;
                        }
                        Console.WriteLine("Please enter the cipher, spaces must be intact");
                        inputCipher = Console.ReadLine();
                        inputProcessor.processInput(inputCipher);
                        break;
                    case "3":
                        if (initialized == false)
                        {
                            dictSorter = new DictionarySorter();
                            inputProcessor = new Processor(dictSorter);
                            initialized = true;
                        }
                        inputProcessor.processInput(defaultCipher);
                        break;
                    case "4":
                        Console.WriteLine("Help");
                        break;
                    case "5":
                        Console.WriteLine("Bye!");
                        running = false;
                        break;
                    default:
                        Console.WriteLine("I don't understand...");
                        break;
                }
                Console.WriteLine("\n");
            }
        }
    }
}
