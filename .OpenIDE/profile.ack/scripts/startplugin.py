#!/usr/bin/env python
import sys, os, subprocess, threading

process = None

def print_definitions():
    # Definition format usually represented as a single line:

    # Script description|
    # command1|"Command1 description"
    #   param|"Param description" end
    # end
    # command2|"Command2 description"
    #   param|"Param description" end
    # end
    print("Starts the C# language plugins and performs crawl")

def readThread():
    while True:
        line = sys.stdin.readline().decode(encoding='windows-1252').strip('\n').strip('\r')
        process.stdin.write(line + "\n")
        process.stdin.flush()

def run_command(run_location, global_profile, local_profile, args):
    # Script parameters
    #   Param 1: Script run location
    #   Param 2: global profile name
    #   Param 3: local profile name
    #   Param 4-: Any passed argument
    #
    # When calling oi use the --profile=PROFILE_NAME and 
    # --global-profile=PROFILE_NAME argument to ensure calling scripts
    # with the right profile.
    #
    # To post back oi commands print command prefixed by command| to standard output
    # To post a comment print to std output prefixed by comment|
    # To post an error print to std output prefixed by error|
    # Place script core here
    global process
    process = subprocess.Popen(['/home/ack/src/OpenIDE.C-Sharp/src/CSharp/bin/AutoTest.Net/C#.exe', 'initialize', '/home/ack/src/OpenIDE.C-Sharp'], stdout=subprocess.PIPE, stdin=subprocess.PIPE, stderr=subprocess.STDOUT, cwd=os.getcwd())
    forwardInput = threading.Thread(target = readThread)
    forwardInput.start()
    while(True):
        line = process.stdout.readline().decode(encoding='windows-1252').strip('\n').strip('\r')
        retcode = process.poll() # returns None while subprocess is running
        if line != "":
            if line == "initialized":
                process.stdin.write("crawl-source /home/ack/src/OpenIDE.C-Sharp\n")
                process.stdin.flush()
            sys.stdout.write(line+"\n")
            sys.stdout.flush()
        if(retcode is not None):
            break

if __name__ == "__main__":
    args = sys.argv
    if len(args) > 1 and args[2] == 'get-command-definitions':
        print_definitions()
    else:
        run_command(args[1], args[2], args[3], args[4:])
