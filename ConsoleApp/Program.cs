using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ParserINI;

namespace IniParserApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IniParser iniParser = new IniParser();
            string filenameIn = "C:\\Users\\User\\Documents\\GitHub\\TESTAIS\\ConsoleApp\\AISTest.ini";
            iniParser.Load(filenameIn);
            iniParser.PrintAll();
            string filenameOut = "C:\\Users\\User\\Documents\\GitHub\\TESTAIS\\ConsoleApp\\AISTestObr.ini";
            iniParser.WriteInFile(filenameOut);
        }
    }
}
