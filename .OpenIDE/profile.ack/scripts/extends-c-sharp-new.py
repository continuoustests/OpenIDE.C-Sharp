#!/usr/bin/env python
import os, sys, subprocess

def get_handlers():
    handlers = {
        "commandhandler": get_command_handler
    }
    return handlers

def get_command_handler():
    body = '''using System;
using CSharp.Responses;

namespace {namespace}
{
    class {itemname} : ICommandHandler
    {
        public string Usage {
            get {
                return
                    Command+"|\"???\" " +
                    "end ";
            }
        }

        public string Command { get { return "???"; } }

        public void Execute(IResponseWriter writer, string[] arguments) {
        }
    }
}'''
    return "command handler", 19, 10, body

######################## Generic content ########################

def print_definitions():
    print("Adds custom new commands for C#|")
    handlers = get_handlers()
    for key in handlers.keys():
        description, line, column, body = handlers[key]()
        print("[[C#]]|\"\" ")
        print("  [[new]]|\"\" ")
        print("    "+key+"|\"Creates a new "+description+"\" ")
        print("      PATH|\"Path to the file to create\" end ")
        print("    end ")
        print("  end ")
        print("end ")

def write(msg):
    sys.stdout.write(msg+"\n")
    sys.stdout.flush()

def run_process(exe,workingDir=""):    
    if workingDir == "":
        workingDir = os.getcwd()
    lines = []
    p = subprocess.Popen(exe, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, cwd=workingDir)
    while(True):
        retcode = p.poll()
        line = p.stdout.readline().strip('\n')
        if line != "":
            lines.append(line)
        if(retcode is not None):
            break
    return lines

def get_namespace(run_location, path):
    ns = ""
    for line in run_process(["oi","C#","get-namespace",path]):
        ns = line
    return ns

def prepare_body(run_location, path, body):
    ns = get_namespace(run_location, path)
    name = os.path.splitext(os.path.basename(path))[0]
    body = body.replace("{namespace}", ns)
    body = body.replace("{itemname}", name)
    return body

def get_path(run_location, path):
    if not path.lower().endswith(".cs"):
        path = path+".cs"
    if os.path.isabs(path):
        return path
    return os.path.abspath(os.path.join(run_location, path))

def write_file(path, body):
    f = open(path, 'w')
    f.write(body)
    f.close()

def run_command(run_location, global_profile, local_profile, args):
    if len(args) < 2:
        write("error|Invalid number of arguments")
        return

    path = get_path(run_location, args[1])
    handlers = get_handlers()
    description, line, column, body = handlers[args[0]]()
    body = prepare_body(run_location, path, body)
    write("command|C# addfile \""+path+"\"")
    write_file(path, body)
    write("command|editor goto \""+path+"|"+str(line+1)+"|"+str(column)+"\"")

if __name__ == "__main__":
    args = sys.argv
    if len(args) > 1 and args[2] == 'get-command-definitions':
        print_definitions()
    else:
        run_command(args[1], args[2], args[3], args[4:])
