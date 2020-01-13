/*
*Author: Artom Didinchuk
*Date: 2019-11-05
*Version 1.0
*This file holds the console utils
*/
#pragma once
#include <string>
#include <vector>
#include "DisplayController.h"
/*
splits a string into a vector according to a supplied deliminator
*/
std::vector<std::string> split_string(std::string input, char delim);
/*
outputs help documentation to the screen
*/
void print_help();