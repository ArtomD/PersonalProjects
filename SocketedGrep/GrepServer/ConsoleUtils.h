/*
*Author: Artom Didinchuk
*Date: 2019-11-05
*Version 1.0
*This file houses functions that print output to console as well as functions that read in and process user input.
*/
#pragma once
#define _WINSOCKAPI_
#include <string>
#include <iostream>
#include <stdio.h> 
#include <direct.h>
#define GetCurrentDir _getcwd

#include "ThreadUtils.h"
#include "SharedVars.h"
namespace multigrep {
	/*
	*This function takes in a double as seconds and populates the result vector to send back to the client
	*Arguments:
	*	timeFinished - the number seconds it took to complete the search
	*/
	void print_results(double timeFinished);
	/*
	*This function and populates the result vector with an error to send back to the client
	*/
	void print_error(std::string msg);
}


