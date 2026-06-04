#===================================================================
#   Author: Leonardo Espinosa
#   This program can access files in a specified directory and 
#   run those programs
#==================================================================

#!/usr/bin/env python3
import subprocess
import sys
import os

last_files = [] #remember previous files from last dir call
last_directory = "." #remember previous directory from last dir call

#loop logic (making sure the file does not end unless the user specifies q to quit)
while True:
    command = input("Enter command <dir>, <dir directory_name>, <run number>, or <exit>: ")
   
    if command == "exit":
        sys.exit("Exiting")
    
    #split command to match syntax in lab doc
    parts = command.split(maxsplit = 1)

    if parts[0] == "dir":
        count = 0 #reset count every time dir is called so that it doesn't cause bugs when running files later
        last_files = [] #also reset the data in this array for similar reasons above

        if len(parts) == 1: #if the only command entered was <dir>, look through current directory
            directory = "."
        else:
            directory = parts[1]

        directory = os.path.expanduser(directory)
        directory = os.path.abspath(directory)

        #check if directory exists
        if not os.path.isdir(directory):
            print("directory does not exist")
            continue

        last_directory = directory

        #list the file names
        for filename in os.listdir(directory):
            full_path = os.path.join(directory, filename)

            if os.path.isfile(full_path):
                last_files.append(filename)
                count = count + 1
                print(count, "-", filename)

        if count == 0:
            print("No files in specified directory")

    elif parts[0] == "run":
        if len(parts) != 2:
            print("Usage: run <number>")
            continue

        if not last_files:
            print("No previous file or directory listings found")
            continue

        if not parts[1].isdigit():
            print("Please enter a valid number.")
            continue

        index = int(parts[1]) - 1 #non inclusive

        if index < 0 or index >= len(last_files):
            print("Invalid entry, that number does not correspond to a listed file")
            continue

        filename = last_files[index]
        full_path = os.path.join(last_directory, filename)
        full_path = os.path.abspath(full_path)

        print("Running: ", full_path)

        #exception handling
        try:
            if filename.endswith(".py"):
                subprocess.run(["python3", full_path])
            else:
                subprocess.run([full_path])
        except Exception as e:
            print("Could not run file", e)

    else:
        print("Invalid command")
