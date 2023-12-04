using System;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CodeSpider
{
    class Program
    {
        // public static string expdate = "12/31/2010 23:59:59"; //program license expiration date
        public static string regmatch = ""; //exposed publicly as a work around until i turn results into an object or string array

        static void Main(string[] args)
        {
            try
            {
                SetupConsole(); //Display Console Header
                //CheckProgramExpiration(); //Check for Program Expiration against static variable expdate

                Array commandlineops = Environment.GetCommandLineArgs(); //get command line options

                HelpCheck(commandlineops); //Display Help if necessary

                string csvloc = InputFile(commandlineops); //get location of csv list from command line
                string searchloc = SearchLoc(commandlineops); //get location to search from command line
                string searchstr = SearchStr(commandlineops); //get string to search for within web pages
                string regexp = GetRegExp(commandlineops); //get regular expression
                string outputfile = OutputFile(commandlineops); //get location of output target

                OnBlankParams(searchloc, csvloc, searchstr, regexp, outputfile); //error out on blank parameters

                Array searchlocArray = BuildSearchArray(searchloc, csvloc); //build array of pages to search from command line and csv
                List<string> resultlist = new List<string>(); //Create Lists to fill for CSV output

                string phrase;
                if (regexp != "")
                {
                    phrase = regexp;
                }
                else
                {
                    phrase = searchstr;
                }

                Console.WriteLine("Searching for phrase '" + phrase + "'..."); //output results to console

                foreach (string loc in searchlocArray)
                {

                    int result = ProcessUrlSearch(loc, searchstr, regexp);
                    //string matchedterm = "";
                    if (searchstr != "")
                    {
                        phrase = searchstr;
                    }
                    else
                    {
                        phrase = regmatch;
                    }
                    ConsoleOutput(loc, result, searchstr, phrase);
                    if (outputfile != "") { BuildCSV(loc, phrase, result, resultlist); }
                }
                if (outputfile != "") { WriteCSV(resultlist, outputfile); } //write to csv
            }

            catch (Exception e)
            {
                Console.WriteLine("General Exception Error\n" + e.ToString() + "\nPress enter to end program.");
                Console.ReadLine();
            }
        }

        public static void HelpCheck(Array commandlineops)
        {
            if (commandlineops.Length < 2) { DisplayHelp(); Environment.Exit(1); } //display help if no command lines are used
            if (Array.IndexOf(commandlineops, "/?") > 0) { DisplayHelp(); Environment.Exit(1); } //display help and end if help is used
        }

        public static void OnBlankParams(string searchloc, string csvloc, string searchstr, string regexp, string outputfile)
        {
            if (searchloc == "" && csvloc == "") { Console.WriteLine("No web sources specified for search! \n Use /? to display help."); Environment.Exit(1); } //if no search string specified then end
            if (searchstr == "" && regexp == "") { Console.WriteLine("No Search string specified! \nUse /? to display help."); Environment.Exit(1); } //if no pages to crawl are specified then end
        }

        public static void DisplayHelp()
        {
            Console.WriteLine("\tThe CodeSpider is a source-code searching utility.");
            Console.WriteLine("(c) 2009 Isaac Wyatt https://isaacwyatt.com");
            Console.WriteLine("\nSyntax:");
            Console.WriteLine("Codespider /i:[csvfile] /w:[website to search] /s:[searchstring] /r:[regularexpression] /o:[outputfile]");

            Console.WriteLine("\nArguments:");
            Console.WriteLine("/i: \t the path and location of a csv containing urls of pages to search. Each entry should be on its own line.");
            Console.WriteLine("/w: \t a single web address to search. Can also be used in conjunction with /i: .");
            Console.WriteLine("/s: \t the search term that you want CodeSpider to find. Use \\ to escape double quotes.");
            Console.WriteLine("/r: \t a regular expression search (advanced user option).");
            Console.WriteLine("/o: \t the file location for text output in csv.");

            Console.WriteLine("\nExample Input:");
            Console.WriteLine("Codespider /i:c:\\mycsv.csv /o:results.csv /w:http://www.yahoo.com/index.html /s:\"<P ALIGN>\"");

            Console.WriteLine("\nExample Output:");
            Console.WriteLine("https://isaacwyatt.com/index.html       Matched phrase \"<P ALIGN>\" at line 14");
            Console.WriteLine("http://www.yahoo.com/index.html    No match found for phrase \"<P ALIGN>\" -1\n");
        }

        public static void SetupConsole()
        {
            Console.Title = "CodeSpider Beta by Isaac Wyatt (c) 2009 https://isaacwyatt.com";
            Console.WriteLine("CodeSpider V.0.0.6\n");
        }

        // public static void CheckProgramExpiration()
        // {
        //     DateTime currtime = DateTime.Now;
        //     DateTime expiredate = DateTime.Parse(expdate);
        //     if (currtime > expiredate)
        //     {
        //         try
        //         {
        //             Console.WriteLine("This version of CodeSpider has been disabled. Redirecting to https://isaacwyatt.com/ for support...");
        //             System.Diagnostics.Process.Start("https://isaacwyatt.com");
        //         }
        //         catch
        //         {
        //             Console.WriteLine("This version of CodeSpider has been disabled. Please visit https://isaacwyatt.com/ for support.");
        //         }
        //     }
        // }

        public static string InputFile(Array commandline)  //i:
        {
            foreach (string line in commandline)
            {
                int lIndex = line.IndexOf("/i:");
                if (lIndex == 0)
                {
                    string ifile = line.Substring(3);
                    return ifile;
                }
            }
            return "";
        }

        public static string SearchLoc(Array commandline) //w:
        {
            foreach (string line in commandline)
            {
                int lIndex = line.IndexOf("/w:");
                if (lIndex == 0)
                {
                    string ifile = line.Substring(3);
                    return ifile;
                }
            }
            return "";
        }

        public static string SearchStr(Array commandline) //s:
        {
            foreach (string line in commandline)
            {
                int lIndex = line.IndexOf("/s:");
                if (lIndex == 0)
                {
                    string ifile = line.Substring(3);
                    return ifile;
                }
            }
            return "";
        }

        public static string GetRegExp(Array commandline) //r:
        {
            foreach (string line in commandline)
            {
                int lIndex = line.IndexOf("/r:");
                if (lIndex == 0)
                {
                    string ifile = line.Substring(3);
                    return ifile;
                }
            }
            return "";
        }

        public static string OutputFile(Array commandline) //o:
        {
            foreach (string line in commandline)
            {
                int lIndex = line.IndexOf("/o:");
                if (lIndex == 0)
                {
                    string ifile = line.Substring(3);
                    return ifile;
                }
            }
            return "";
        }

        public static Array BuildSearchArray(string searchloc, string csvloc)
        {
            try
            {
                List<string> allLines = new List<string>();
                if (searchloc != "")
                {
                    allLines.Add(searchloc); //add command line search location to array
                }

                if (csvloc != "")
                {
                    using (StreamReader reader = new StreamReader(csvloc))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            allLines.Add(line); // "line" is a line in the file. Add it to our List.
                        }
                    }
                }
                string[] arrayoflines = allLines.ToArray();
                return arrayoflines;
            }

            catch (FileNotFoundException)
            {
                Console.WriteLine("Specified file not found! \nUse /? to display help.");
                Environment.Exit(1);
                string[] arrayoflines = null;
                return arrayoflines;
            }
        }

        public static int ProcessUrlSearch(string searchloc, string searchstr, string regexp)
        {
            string sourcecode = GetSourceCode(searchloc);
            int searchindex = -1;
            if (regexp.Length > 0)
            {
                searchindex = RegExSearch(searchloc, regexp); //regular expression serch
            }
            else
            {
                searchindex = sourcecode.IndexOf(searchstr); //standard search
            }

            searchindex = GetLineNumber(sourcecode, searchindex);
            return searchindex;
        }

        public static int RegExSearch(string searchloc, string regexp)
        {
            try
            {
                string sourcecode = GetSourceCode(searchloc);
                Regex regex = new Regex(regexp);
                Match rxMatch = regex.Match(sourcecode);
                regmatch = rxMatch.ToString();
                int searchindex = rxMatch.Index;
                return searchindex;
            }
            catch (Exception)
            {
                Console.WriteLine("Error in Regular Expression Search! \nUse /? to display help.");
                Environment.Exit(1);
                int err = -1;
                return err;
            }
        }

        public static int GetLineNumber(string searchstr, int searchindex)
        {
            int count = 1;
            int start = 0;
            if (searchindex < 1)
            {
                count = -1;
            }
            else
            {
                string newstring = searchstr.Substring(1, searchindex);
                while ((start = newstring.IndexOf('\n', start)) != -1)
                {
                    count++;
                    start++;
                }
            }
            return count;
        }

        public static string GetSourceCode(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.UserAgent = "CodeSpider";
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string htmlText = reader.ReadToEnd();
                return htmlText;
            }

            catch (UriFormatException)
            {
                Console.WriteLine("Invalid or blank URI! \nUse /? to display help.");
                string htmlText = "{Blank or Incorrect Format}";
                return htmlText;
            }

            catch (WebException)
            {
                Console.WriteLine("{Website Not Found} \nUse /? to display help.");
                string htmlText = "{Website Not Found}";
                return htmlText;
            }
        }

        public static void ConsoleOutput(string searchloc, int result, string phrase, string matchedterm)
        {
            if (result > 0)
            {
                Console.WriteLine(searchloc + "\t" + "Matched phrase \"" + matchedterm + "\" at line " + result);
            }
            else
            {
                Console.WriteLine(searchloc + "\t" + "No match found for phrase \"" + phrase + "\" " + result);
            }
        }

        public static void BuildCSV(string loc, string matchedterm, int result, List<string> resultlist) //add results to list for CSV output
        {
            string csventry;
            csventry = loc + "," + matchedterm + "," + result + "\r\n";
            resultlist.Add(csventry);
        }

        public static void WriteCSV(List<string> resultlist, string outputfile) //writecsv from list
        {
            try
            {
                TextWriter tw = new StreamWriter(outputfile);
                foreach (string csvout in resultlist)
                {
                    tw.Write(csvout);
                }
                tw.Close();
                Console.WriteLine("\nCreated File: '" + outputfile + "'\n");
            }
            catch
            {
                Console.WriteLine("\nError writing file to disk! \nUse /? to display help.");
            }
        }
    }
}