/*
*Author: Artom Didinchuk
*Date: 2019-10-01
*Version 1.0
*This file houses the function that reads in data from an external file.
*/
#pragma once
#include <string>
#include <vector>
#include <codecvt>
#include <fstream>
#include <algorithm>

using namespace std;
/*
*This function takes a file name and path and returns a command list in the form of a vector.
*Arguments:
*	string fileName - the name and path to the file to be read.
*Return:
*	vector<vector<string>> - the parsed commands from the file.
*/
vector<vector<string>> parse_command_file(string fileName);