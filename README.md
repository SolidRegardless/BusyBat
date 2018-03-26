# BusyBat

Provides standard input, parameter-based or file-based regular expression matching, Json query, XML XPath query and console foreground/background support in an all-in-one executable similar in nature to busybox.

BusyBat.exe [-help] [-fg {foreground colour}] [-bg {bg colour}] [-pr {output}] [-nolf] [-ret] [-rst]
    [-cls] [-rx {expression} {-val input value | -file filepath} [-grp group name]] [-enum
    {root} {pattern}] [-json {json path} [-arr]] [-xml {xpath} [-fns {namespace}]]

  -help : Shows help about Busy Bat.
    -fg : Change the foreground colour to that specified. The colour to change to is any of the
          standard ConsoleColor enumeration values, e.g. DarkRed, Blue, White, Green, Cyan, etc.
    -bg : Change the background colour to that specified. The colour to change to is any of the
          standard ConsoleColor enumeration values as with the foreground colour.
    -pr : Print a message as provided using the colour settings specified. If the -ret parameter
          was not specified, the colour settings are reset back to the colours before the Busy
          Bat was executed.
  -nolf : Prints any output from the -pr option without adding a new line.
   -ret : Retain the colour settings after the application has exited. This will persist any of
          the colour settings specified.
   -rst : Reset the console to its default colours.
   -cls : Clears the console buffer and display before writing anything to standard output.
    -rx : Search the given value (-val) or file (-file) for the regular expression passed in using
          this parameter.
   -val : String value provided to match the regular expression against (-rx) or to search for a
          Json path (-json) for extracting a property.
  -file : References a file that should be used for either matching a regular expression (-rx)
          within or for extracting a Json value (-json) given a Json path expression.
   -grp : If searching for a regular expression match within a value (-val) or file (-file) either
          the whole match is returned or by providing this parameter just the matched group
          value is returned.
  -enum : Enumerate the indicated folder for files with a filename that matches the specific
          pattern. When using this option, the root folder and search pattern must be given
          alongside the parameter, e.g. -enum c:\ *.log
  -json : Extract a Json property using the provided Json path. The source to use for parsing
          is either taken from the passed in value (-val) or from a file (-file). Alternatively, the
          data can be piped into the application through standard input. If the -arr parameter
          is passed on the command line with -json collections are returned on multiple lines.
   -xml : Extract an XML value using the provided XML path. The source to use for parsing
          is either taken from the passed in value (-val) or from a file (-file). Alternatively, the
          data can be piped into the application through standard input.
