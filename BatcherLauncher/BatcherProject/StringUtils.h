/*
*Author: Artom Didinchuk
*Date: 2019-10-01
*Version 1.0
*This file houses functions that parse and order strings.
*/
#pragma once
#include <string>
#include <vector>

using namespace std;

//strips leading and trailing whitespace and populates given string vector with strings split from given string delimited by given delimitor
/*
*This function strips leading and trailing whitespace and populates given string vector 
*with strings split from given string delimited by given delimitor.
*Arguments:
*	string input - the string to split into commands.
*	vector<string>* strings - the command vector to populate.
*	char delim - the delimitor to separate commands into components
*Return:
*	vector<vector<string>> - the parsed commands from the file.
*/
void splitString(string input, vector<string>* strings, char delim);
/*
*This function sorts a vector<vector<string>> by comparing the first string in the vectors.
*/
bool sortByLaunchGroup(const vector<string>& lhs, const vector<string>& rhs);