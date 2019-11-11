/*
*Author: Artom Didinchuk
*Date: 2019-10-01
*Version 1.0
*This file houses functions that print output to console as well as functions that read in and process user input.
*/
#pragma once
#include <iostream>
#include <codecvt>
#include "LaunchUtils.h"

/*
*This function takes a result object from a process launch and prints the details to console.
*Arguments: 
*	Program_Return output - the return object that houses details to be printed out for the user.
*/
void print_Program_Return_to_console(Program_Return output);
/*
*This function takes all the error objects from all process launches and prints the details to console.
*Arguments:
*	vector<Program_Return> errors - a vector of all the error objects that house details to be printed out for the user.
*/
void print_process_errors_to_console(vector<Program_Return> errors);
/*
*This function prints the preset help message to console.
*/
void print_help_to_console();
/*
*This function takes in user input and then passes back the relevant argument or calls the help function;
*Arguments:
*	int argc, char* argv[] - the console arguments the program launched with
*/
string get_file_name(int argc, char* argv[]);
/*
*This function prints an error to console if there is an issue with the arguments or file input
*Arguments:
*	int argc, char* argv[] - the console arguments the program launched with
*/
void print_error_to_console(string errorMsg);


