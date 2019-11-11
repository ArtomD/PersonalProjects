/*
*Author: Artom Didinchuk
*Date: 2019-11-05
*Version 1.0
*This file houses functions that print output to console as well as functions that read in and process user input.
*/
#pragma once
#include <string>
#include <iostream>
#include <stdio.h> 
#include <direct.h>
#define GetCurrentDir _getcwd

#include "ThreadUtils.h"
#include "SharedVars.h"
namespace multigrep {
	/*
	*This function takes in arguments from the console and populates various data objects that will be used by the search
	*Arguments:
	*	argc - the number of arguments
	*	argv - the arguments as a char* array
	*/
	void parse_input_to_params(int argc, char* argv[]);
	/*
	*This function takes in a double as seconds and prints out the results of the search
	*Arguments:
	*	timeFinished - the number seconds it took to complete the search
	*/
	void print_results(double timeFinished);
	/*
	*This function prints out the help message to the console
	*/
	void print_help();
	/*
	*This function prints out an error message to the console
	*/
	void print_error(std::string msg);
}


