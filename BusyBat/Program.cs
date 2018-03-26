// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Stuart Hart">
//   This software is free software and the source code can be used and modified
//   as desired.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BusyBat
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The program provides extended support for controlling the console to give a better
    /// user experience.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Clean the string from standard input so that it is as it would be if passed in as an
        /// argument. Standard input may have been piped in via an echo and 
        /// </summary>
        /// <param name="value">
        /// Uncleaned Json string.
        /// </param>
        /// <returns>
        /// Returns the cleaned Json string.
        /// </returns>
        private static string CleanStandardInputString(string value)
        {
            if (value == null || value.Length <= 0)
            {
                return string.Empty;
            }

            if (value.EndsWith(Environment.NewLine))
            {
                value = value.Substring(0, value.Length - Environment.NewLine.Length).Trim();
            }

            if (value.StartsWith("\"") && value.Length > 1)
            {
                value = value.Substring(1);
                if (value.EndsWith("\""))
                {
                    value = value.Substring(0, value.Length - 1);
                }
            }

            return value;
        }

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="arguments">
        /// Application arguments to control the context.
        /// </param>
        private static void Main(string[] arguments)
        {
            if (arguments == null || arguments.Length == 0)
            {
                PrintUsage();
                Environment.Exit(0);
            }

            ProcessArgumentList(arguments);
        }

        /// <summary>
        /// Print the usage statement.
        /// </summary>
        /// <param name="argumentList">
        /// The argument list to extract any associated parameters from.
        /// </param>
        /// <param name="index">
        /// The index of the identified command.
        /// </param>
        private static void PrintUsage(IList argumentList = null, int index = -1)
        {
            var defaultForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(
                ParameterInformation.PrintUsageHeaderLine,
                Assembly.GetExecutingAssembly().GetName().Version);

            Console.ForegroundColor = defaultForegroundColor;
            Console.WriteLine(ParameterInformation.PrintUsageDescriptionLine);
            Console.WriteLine(ParameterInformation.PrintUsageParameterOptions, AppDomain.CurrentDomain.FriendlyName);
            Console.WriteLine(ParameterInformation.PrintUsageHelpDescription);

            Console.WriteLine(ParameterInformation.PrintUsageForegroundDescriptionLine);
            Console.WriteLine(ParameterInformation.PrintUsageBackgroundDescriptionLine);

            Console.WriteLine(ParameterInformation.PrintUsagePrintDescription);
            Console.WriteLine(ParameterInformation.PrintUsageNoLineFeedDescription);
            Console.WriteLine(ParameterInformation.PrintUsageRetainDescriptionLine);
            Console.WriteLine(ParameterInformation.PrintUsageResetDescription);
            Console.WriteLine(ParameterInformation.PrintUsageClearConsoleDescription);

            Console.WriteLine(ParameterInformation.PrintUsageRegularExpressionDescription);
            Console.WriteLine(ParameterInformation.PrintUsageValueDescription);
            Console.WriteLine(ParameterInformation.PrintUsageFileDescription);
            Console.WriteLine(ParameterInformation.PrintUsageGroupDescription);
            Console.WriteLine(ParameterInformation.PrintUsageEnumDescription);
            Console.WriteLine(ParameterInformation.PrintUsageJsonDescription);
            Console.WriteLine(ParameterInformation.PrintUsageXmlDescription);

            if (argumentList != null && index >= 0)
            {
                argumentList.RemoveAt(index);
            }
        }

        /// <summary>
        /// Process the argument list according to known parameters.
        /// </summary>
        /// <param name="arguments">
        /// The arguments passed into the application. These will be copied and as each
        /// command is processed the list will be reduced to make processing more precise.
        /// </param>
        private static void ProcessArgumentList(IEnumerable<string> arguments)
        {
            var defaultForegroundColour = Console.ForegroundColor;
            var defaultBackgroundColour = Console.BackgroundColor;

            var argumentList = new List<string>(arguments);

            var index = argumentList.IndexOf(ParameterInformation.HelpParameter);
            if (index >= 0)
            {
                PrintUsage(argumentList, index);
            }

            index = argumentList.IndexOf(ParameterInformation.ForegroundParameter);
            if (index >= 0)
            {
                ProcessForegroundColour(argumentList, index);
            }

            index = argumentList.IndexOf(ParameterInformation.BackgroundParameter);
            if (index >= 0)
            {
                ProcessBackgroundColour(argumentList, index);
            }

            index = argumentList.IndexOf(ParameterInformation.ResetParameter);
            if (index >= 0)
            {
                ProcessResetConsole(argumentList, index);
            }

            index = argumentList.IndexOf(ParameterInformation.ClearConsoleParameter);
            if (index >= 0)
            {
                ProcessClearConsole(argumentList, index);
            }

            index = argumentList.IndexOf(ParameterInformation.EnumParameter);
            if (index >= 0)
            {
                ProcessEnumerateFiles(argumentList, index);
            }

            index = argumentList.IndexOf(ParameterInformation.RegularExpressionParameter);
            if (index >= 0)
            {
                ProcessRegularExpression(argumentList, index);
            }

            index = argumentList.IndexOf(ParameterInformation.JsonParameter);
            if (index >= 0)
            {
                ProcessJsonQuery(argumentList, index);
            }

            index = argumentList.IndexOf(ParameterInformation.XmlParameter);
            if (index >= 0)
            {
                ProcessXmlQuery(argumentList, index);
            }

            index = argumentList.IndexOf(ParameterInformation.PrintParameter);
            if (index >= 0)
            {
                ProcessConsoleOutput(argumentList, index);
            }

            index = argumentList.IndexOf(ParameterInformation.RetainParameter);
            if (index < 0)
            {
                Console.ForegroundColor = defaultForegroundColour;
                Console.BackgroundColor = defaultBackgroundColour;
            }
            else
            {
                argumentList.RemoveAt(index);
            }
        }

        /// <summary>
        /// Process the set background colour command using whatever colour is specified
        /// in the argument list.
        /// </summary>
        /// <param name="argumentList">
        /// The argument list to extract any associated parameters from.
        /// </param>
        /// <param name="index">
        /// The index of the identified command.
        /// </param>
        private static void ProcessBackgroundColour(IList<string> argumentList, int index)
        {
            var backgroundColorText = string.Empty;
            if (argumentList.Count > index + 1)
            {
                backgroundColorText = argumentList[index + 1];
                argumentList.RemoveAt(index + 1);
            }

            argumentList.RemoveAt(index);
            ConsoleColor backgroundColor;
            if (Enum.TryParse(backgroundColorText, true, out backgroundColor))
            {
                Console.BackgroundColor = backgroundColor;
            }
        }

        /// <summary>
        /// Process the clear console command which clears both the console buffers and
        /// the console display.
        /// </summary>
        /// <param name="argumentList">
        /// The argument list to extract any associated parameters from.
        /// </param>
        /// <param name="index">
        /// The index of the identified command.
        /// </param>
        private static void ProcessClearConsole(IList<string> argumentList, int index)
        {
            argumentList.RemoveAt(index);
            Console.Clear();
        }

        /// <summary>
        /// Process console output using whatever foreground and background colours
        /// are currently set.
        /// </summary>
        /// <param name="argumentList">
        /// The argument list to extract any associated parameters from.
        /// </param>
        /// <param name="index">
        /// The index of the identified command.
        /// </param>
        private static void ProcessConsoleOutput(IList<string> argumentList, int index)
        {
            var printText = string.Empty;
            if (Console.IsInputRedirected)
            {
                printText = Console.In.ReadToEnd();
            }
            else if (argumentList.Count > index + 1)
            {
                printText = argumentList[index + 1];
                argumentList.RemoveAt(index + 1);
            }

            argumentList.RemoveAt(index);

            index = argumentList.IndexOf(ParameterInformation.NoLineFeedParameter);
            if (index >= 0)
            {
                argumentList.RemoveAt(index);
                Console.Write(printText);
            }
            else
            {
                Console.WriteLine(printText);
            }
        }

        /// <summary>
        /// Search for files that match the pattern within the path provided. This command
        /// expects there to be an additional 2 parameters available but will silently exit
        /// if not.
        /// </summary>
        /// <param name="argumentList">
        /// The argument list to extract any associated parameters from.
        /// </param>
        /// <param name="index">
        /// The index of the identified command.
        /// </param>
        private static void ProcessEnumerateFiles(IList<string> argumentList, int index)
        {
            if (argumentList.Count >= index + 2)
            {
                var path = argumentList[index + 1];
                var pattern = argumentList[index + 2];

                Parallel.ForEach(
                    Directory.EnumerateFiles(path, pattern, SearchOption.AllDirectories),
                    s => { Console.WriteLine(s); });

                argumentList.RemoveAt(index + 2);
                argumentList.RemoveAt(index + 1);
            }

            argumentList.RemoveAt(index);
        }

        /// <summary>
        /// Process a Json query across the content of a file and return the property
        /// value.
        /// </summary>
        /// <param name="jsonQuery">
        /// Json query to use for extracting data.
        /// </param>
        /// <param name="file">
        /// File path containing the contents to run the Json query against.
        /// </param>
        /// <param name="selectTokens">
        /// If the <paramref name="selectTokens"/> is true then collections are written to
        /// the output as separate lines.
        /// </param>
        private static void ProcessFileBasedJsonQuery(string jsonQuery, string file, bool selectTokens)
        {
            ProcessValueBasedJsonQuery(jsonQuery, File.ReadAllText(file), selectTokens);
        }

        /// <summary>
        /// Process a regular expression match across the content of a file and return any
        /// matches.
        /// </summary>
        /// <param name="regularExpression">
        /// Regular expression to use for matching.
        /// </param>
        /// <param name="file">
        /// File path containing the contents to run the regular expression through.
        /// </param>
        /// <param name="groupName">
        /// Optional parameter specifying if just a particular match group should be returned
        /// or whether the whole match should be returned.
        /// </param>
        private static void ProcessFileBasedRegularExpression(string regularExpression, string file, string groupName)
        {
            ProcessValueBasedRegularExpression(regularExpression, File.ReadAllText(file), groupName);
        }

        /// <summary>
        /// Process an XML query across the content of a file and return the matching XPath
        /// value.
        /// </summary>
        /// <param name="xpathQuery">
        /// XPath query to use for extracting data.
        /// </param>
        /// <param name="file">
        /// File path containing the contents to run the XPath query against.
        /// </param>
        /// <param name="fakeNS">
        /// If provided, a fake namespace will be injected into the namespace manager to help
        /// support canonicalised queries.
        /// </param>
        private static void ProcessFileBasedXmlQuery(string xpathQuery, string file, string fakeNS)
        {
            ProcessValueBasedXmlQuery(xpathQuery, File.ReadAllText(file), fakeNS);
        }

        /// <summary>
        /// Process the set foreground colour command using whatever colour is specified
        /// in the argument list.
        /// </summary>
        /// <param name="argumentList">
        /// The argument list to extract any associated parameters from.
        /// </param>
        /// <param name="index">
        /// The index of the identified command.
        /// </param>
        private static void ProcessForegroundColour(IList<string> argumentList, int index)
        {
            var foregroundColorText = string.Empty;
            if (argumentList.Count > index + 1)
            {
                foregroundColorText = argumentList[index + 1];
                argumentList.RemoveAt(index + 1);
            }

            argumentList.RemoveAt(index);
            ConsoleColor foregroundColor;
            if (Enum.TryParse(foregroundColorText, true, out foregroundColor))
            {
                Console.ForegroundColor = foregroundColor;
            }
        }

        /// <summary>
        /// Process Json query finding matches against values from either the command line, standard
        /// input or from a file.
        /// </summary>
        /// <param name="argumentList">
        /// The argument list to extract any associated parameters from.
        /// </param>
        /// <param name="index">
        /// The index of the identified command.
        /// </param>
        private static void ProcessJsonQuery(IList<string> argumentList, int index)
        {
            var jsonQuery = string.Empty;
            var selectTokens = false;

            if (argumentList.Count > index + 1)
            {
                jsonQuery = argumentList[index + 1];
                argumentList.RemoveAt(index + 1);
            }

            argumentList.RemoveAt(index);

            if ((index = argumentList.IndexOf(ParameterInformation.ArrayParameter)) >= 0)
            {
                selectTokens = true;
                argumentList.RemoveAt(index);
            }

            if ((index = argumentList.IndexOf(ParameterInformation.ValueParameter)) >= 0
                && argumentList.Count > index + 1)
            {
                var value = argumentList[index + 1];
                argumentList.RemoveAt(index + 1);
                argumentList.RemoveAt(index);

                ProcessValueBasedJsonQuery(jsonQuery, value, selectTokens);
            }
            else if ((index = argumentList.IndexOf(ParameterInformation.FileParameter)) >= 0
                     && argumentList.Count > index + 1)
            {
                var file = argumentList[index + 1];
                argumentList.RemoveAt(index + 1);
                argumentList.RemoveAt(index);

                ProcessFileBasedJsonQuery(jsonQuery, file, selectTokens);
            }
            else if (Console.IsInputRedirected)
            {
                ProcessValueBasedJsonQuery(
                    jsonQuery,
                    CleanStandardInputString(Console.In.ReadToEnd()),
                    selectTokens);
            }
        }

        /// <summary>
        /// Process regular expression search finding matches against values from either
        /// the command line or via standard input. Options exist for returning only matches
        /// or complete lines which match.
        /// </summary>
        /// <param name="argumentList">
        /// The argument list to extract any associated parameters from.
        /// </param>
        /// <param name="index">
        /// The index of the identified command.
        /// </param>
        private static void ProcessRegularExpression(IList<string> argumentList, int index)
        {
            var regularExpression = string.Empty;
            var groupName = string.Empty;

            if (argumentList.Count > index + 1)
            {
                regularExpression = argumentList[index + 1];
                argumentList.RemoveAt(index + 1);
            }

            argumentList.RemoveAt(index);

            if ((index = argumentList.IndexOf(ParameterInformation.GroupParameter)) >= 0
                && argumentList.Count > index + 1)
            {
                groupName = argumentList[index + 1];
                argumentList.RemoveAt(index + 1);
                argumentList.RemoveAt(index);
            }

            if ((index = argumentList.IndexOf(ParameterInformation.ValueParameter)) >= 0
                && argumentList.Count > index + 1)
            {
                var value = argumentList[index + 1];
                argumentList.RemoveAt(index + 1);
                argumentList.RemoveAt(index);

                ProcessValueBasedRegularExpression(regularExpression, value, groupName);
            }
            else if ((index = argumentList.IndexOf(ParameterInformation.FileParameter)) >= 0
                     && argumentList.Count > index + 1)
            {
                var file = argumentList[index + 1];
                argumentList.RemoveAt(index + 1);
                argumentList.RemoveAt(index);

                ProcessFileBasedRegularExpression(regularExpression, file, groupName);
            }
            else if (Console.IsInputRedirected)
            {
                var value = Console.In.ReadToEnd();
                ProcessValueBasedRegularExpression(regularExpression, value, groupName);
            }
        }

        /// <summary>
        /// Process the reset console command which resets the colours back to the console
        /// default values.
        /// </summary>
        /// <param name="argumentList">
        /// The argument list to extract any associated parameters from.
        /// </param>
        /// <param name="index">
        /// The index of the identified command.
        /// </param>
        private static void ProcessResetConsole(IList<string> argumentList, int index)
        {
            argumentList.RemoveAt(index);
            Console.ResetColor();
        }

        /// <summary>
        /// Process a Json query across the value and return the property value.
        /// </summary>
        /// <param name="jsonQuery">
        /// Json query to use for extracting data.
        /// </param>
        /// <param name="value">
        /// String value to run the Json query against.
        /// </param>
        /// <param name="selectTokens">
        /// If the <paramref name="selectTokens"/> is true then collections are written to
        /// the output as separate lines.
        /// </param>
        private static void ProcessValueBasedJsonQuery(string jsonQuery, string value, bool selectTokens)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                var json = JObject.Parse(value);

                if (selectTokens)
                {
                    var jsonValues = json.SelectTokens(jsonQuery);
                    foreach (var jsonValue in jsonValues.Values())
                    {
                        Console.WriteLine(jsonValue.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty));
                    }
                }
                else
                {
                    var jsonValue = json.SelectToken(jsonQuery);
                    if (jsonValue != null)
                    {
                        Console.WriteLine(jsonValue.ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                WriteError(ErrorStrings.JsonErrorMessage, exception.Message);
            }
        }

        /// <summary>
        /// Process a regular expression match across the content of a string and return any
        /// matches.
        /// </summary>
        /// <param name="regularExpression">
        /// Regular expression to use for matching.
        /// </param>
        /// <param name="value">
        /// String value to run the regular expression through.
        /// </param>
        /// <param name="groupName">
        /// Optional parameter specifying if just a particular match group should be returned
        /// or whether the whole match should be returned.
        /// </param>
        private static void ProcessValueBasedRegularExpression(string regularExpression, string value, string groupName)
        {
            var regex = new Regex(regularExpression);
            var matches = regex.Matches(value);
            if (matches.Count <= 0)
            {
                return;
            }

            foreach (var match in matches.Cast<Match>().Where(x => x.Success))
            {
                if (string.IsNullOrWhiteSpace(groupName))
                {
                    Console.WriteLine(match.Value);
                }
                else
                {
                    if (match.Groups[groupName].Success)
                    {
                        Console.WriteLine(match.Groups[groupName].Value);
                    }
                }
            }
        }

        /// <summary>
        /// Process an XML query across the content of a file and return the matching XPath
        /// value.
        /// </summary>
        /// <param name="xpathQuery">
        /// XPath query to use for extracting data.
        /// </param>
        /// <param name="value">
        /// Value containing the contents to run the XPath query against.
        /// </param>
        /// <param name="fakeNS">
        /// If provided, a fake namespace will be injected into the namespace manager to help
        /// support canonicalised queries.
        /// </param>
        private static void ProcessValueBasedXmlQuery(string xpathQuery, string value, string fakeNS)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                var namespaceManager = new XmlNamespaceManager(new NameTable());
                var document = XDocument.Parse(value);

                if (!string.IsNullOrWhiteSpace(fakeNS))
                {
                    var @namespace = document.Root?.Name.Namespace ?? string.Empty;
                    namespaceManager.AddNamespace(fakeNS, @namespace.NamespaceName);
                }
                else
                {
                    var navigator = document.CreateNavigator();
                    navigator.MoveToFollowing(XPathNodeType.Element);

                    var namespaces = navigator.GetNamespacesInScope(XmlNamespaceScope.All);
                    if (namespaces != null)
                    {
                        foreach (var ns in namespaces)
                        {
                            namespaceManager.AddNamespace(ns.Key, ns.Value);
                        }
                    }
                }

                dynamic xpathResult = document.XPathEvaluate(xpathQuery, namespaceManager);
                if (xpathResult is IEnumerable)
                {
                    foreach (var result in xpathResult)
                    {
                        Console.WriteLine(result.Value);
                    }
                }
                else
                {
                    Console.WriteLine(xpathResult.Value);
                }
            }
            catch (Exception exception)
            {
                WriteError(ErrorStrings.XmlErrorMessage, exception.Message);
            }
        }

        /// <summary>
        /// Process XML query finding matches against values from either the command line, standard
        /// input or from a file.
        /// </summary>
        /// <param name="argumentList">
        /// The argument list to extract any associated parameters from.
        /// </param>
        /// <param name="index">
        /// The index of the identified command.
        /// </param>
        private static void ProcessXmlQuery(IList<string> argumentList, int index)
        {
            var xpathQuery = string.Empty;
            var fakeNS = string.Empty;

            if (argumentList.Count > index + 1)
            {
                xpathQuery = argumentList[index + 1];
                argumentList.RemoveAt(index + 1);
            }

            argumentList.RemoveAt(index);

            if ((index = argumentList.IndexOf(ParameterInformation.FakeNameSpaceParameter)) >= 0
                && argumentList.Count > index + 1)
            {
                fakeNS = argumentList[index + 1];
                argumentList.RemoveAt(index + 1);
                argumentList.RemoveAt(index);
            }

            if ((index = argumentList.IndexOf(ParameterInformation.ValueParameter)) >= 0
                && argumentList.Count > index + 1)
            {
                var value = argumentList[index + 1];
                argumentList.RemoveAt(index + 1);
                argumentList.RemoveAt(index);

                ProcessValueBasedXmlQuery(xpathQuery, value, fakeNS);
            }
            else if ((index = argumentList.IndexOf(ParameterInformation.FileParameter)) >= 0
                     && argumentList.Count > index + 1)
            {
                var file = argumentList[index + 1];
                argumentList.RemoveAt(index + 1);
                argumentList.RemoveAt(index);

                ProcessFileBasedXmlQuery(xpathQuery, file, fakeNS);
            }
            else if (Console.IsInputRedirected)
            {
                ProcessValueBasedXmlQuery(xpathQuery, CleanStandardInputString(Console.In.ReadToEnd()), fakeNS);
            }
        }

        /// <summary>
        /// Keep a reference to the current background and foreground colours before calling the
        /// passed in <see cref="Action"/>. Revert the colours back to the previous state after
        /// the <see cref="Action"/> has been called.
        /// </summary>
        /// <param name="action">
        /// The delegate <see cref="Action"/> to invoke.
        /// </param>
        private static void RevertColoursAfterAction(Action action)
        {
            var bg = Console.BackgroundColor;
            var fg = Console.ForegroundColor;

            action();

            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
        }

        /// <summary>
        /// Write an error to standard error ensuring that the console colours are reverted
        /// to what they were before the errors colours are used.
        /// </summary>
        /// <param name="format">
        /// The format string of the error.
        /// </param>
        /// <param name="parameters">
        /// The parameters to pass into the output format string.
        /// </param>
        private static void WriteError(string format, params object[] parameters)
        {
            RevertColoursAfterAction(
                () =>
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Error.WriteLine(format, parameters);
                    });
        }
    }
}