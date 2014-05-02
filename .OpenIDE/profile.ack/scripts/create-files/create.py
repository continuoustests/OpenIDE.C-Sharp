#!/usr/bin/env python
import os, sys, importlib
copydir = importlib.import_module('preserved-data.copydir', None)

def print_definitions():
    print("""Creates C# projects|
        console|"Creates a new C# console project"
            PATH|"Path to the project" end
        end
        library|"Creates a new C# library project"
            PATH|"Path to the project" end
        end
        service|"Creates a new C# windows service project"
            PATH|"Path to the project" end
        end
    """)

def run_command(args):
    if len(args) != 2:
        print("Invalid number of arguments")
        return
    if args[0] == "console":
        copydir.copy("console", args[1])
        print("command|editor goto \""+os.path.join(args[1], "Program.cs")+"|8|3\"")
    if args[0] == "library":
        copydir.copy("library", args[1])
    if args[0] == "service":
        copydir.copy("service", args[1])
        print("command|editor goto \""+os.path.join(args[1], "Service.cs")+"|22|10\"")

if __name__ == "__main__":
    args = sys.argv
    if len(args) > 1 and args[1] == 'get-command-definitions':
        print_definitions()
    else:
        run_command(args[1:])
