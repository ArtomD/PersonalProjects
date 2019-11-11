/*
*Author: Artom Didinchuk
*Date: 2019-11-05
*Version 1.0
*This file houses string utils functions
*/
#pragma once
#include <string>
#include <vector>
namespace multigrep {
	/*
	*This function takes in a string and breaks it up into extensions(delimited by '.')
	*It cleanes up all the used threads and pointers
	*Arguments:
	*	input - the string to be split
	*	return - a vector of the split strings
	*/
	std::vector<std::string> split_extensions(std::string input);
}