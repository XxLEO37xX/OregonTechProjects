# Python Command Shell

## Author

Leonardo Espinosa

## Course

CST 240 - Linux Programming

## Description

This project is a custom command-line shell written in Python and developed on Oregon Tech's Loki Linux server.

The shell accepts user commands, processes input, and executes supported operations through a simple interactive interface. The project demonstrates Linux programming concepts, file system interaction, process execution, input parsing, and control flow.

---

## Features

* List files in the current working directory
* List files in a specified directory
* Execute selected programs
* Interactive command prompt
* Input validation and error handling
* Exit command for terminating the shell

---

## Supported Commands

### List Current Directory

```text
dir
```

Displays files in the current working directory as a numbered list.

### List a Specific Directory

```text
dir <directory_name>
```

Displays files contained within the specified directory.

### Execute a Program

```text
run <number>
```

Executes the selected file from the displayed directory listing.

If the selected file is not executable, an error message is displayed.

### Exit the Shell

```text
exit
```

Terminates the shell and ends the program.

---

## Requirements

* Python 3
* Linux environment (developed and tested on Oregon Tech's Loki server)

---

## Running the Program

Make the Python files executable:

```bash
chmod +x *.py
```

Run the shell:

```bash
python3 my_shell.py
```

or

```bash
./my_shell.py
```

If successful, the program will display:

```text
Enter command <dir>, <dir directory_name>, <run number>, or <exit>:
```

---

## Files

* `my_shell.py` - Main shell application
* `exec1.py` - Test executable
* `exec2.py` - Test executable
* `test_dir/` - Directory used for testing shell functionality

---

## Concepts Demonstrated

* Python Programming
* Linux Programming
* Command-Line Interfaces (CLI)
* File System Navigation
* Process Execution
* Input Parsing
* Error Handling
* Interactive Program Design
