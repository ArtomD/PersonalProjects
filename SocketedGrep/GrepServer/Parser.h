/*
*Author: Artom Didinchuk
*Date: 2019-11-05
*Version 1.0
*This file houses functions that parse files to look for extensions or content defined by regex.
*/
#pragma once
#define _WINSOCKAPI_
#include <Windows.h>
#include <iostream>
#include <string>
#include <vector>
#include <regex>
#include <boost/filesystem.hpp>
#include <iostream>
#include <mutex>

#include "ThreadUtils.h"
#include "ConsoleUtils.h"

namespace multigrep {
	/*
	*This function takes in an string and tries to match it to a list of allowed extensions
	*Arguments:
	*	extensionMatch - the extension to be matched
	*	return - true if the extension matched
	*/
	bool extensionMatch(std::string extensionMatch);

	/*
	*This function takes in the name of a the file and the regex to search it by. If it findsmatches it records them and returns a FileMatch object. 
	*Arguments:
	*	name - the name of the file
	*	match - the regex to be matched
	*	return - The object housing the name of the file and the maches
	*/
	sharedvars::FileMatch fileContains(std::string name, std::string match);

	/*
	*This function searches a specified directory for file that match the desired extension and regex. 
	*If it finds another directory it stores it to be searched later.
	*Arguments:
	*	path - the path to the directory
	*/
	void searchDirectory(std::string path);
}