#include "StringUtils.h"
#include "ConsoleUtils.h"

//strips leading and trailing whitespace and populates given string vector with strings split from given string delimited by given delimitor
void splitString(string input, vector<string>* strings, char delim) {
	bool stringStart = false;
	string tempString = "";
	string tempSpaces = "";
	//keeps track of which command is being currently parsed
	int commandCount = 0;
	for (std::string::size_type i = 0; i < input.size(); i++) {
		char c = input[i];
		//skip leading whitespace
		if (!stringStart && (c == ' ' || c == '\t')) {
			continue;
		}
		//if delimiter is hit add current string to vector and reset for next string
		if (c == delim) {
			commandCount++;
			if (commandCount >= 4) {
				print_error_to_console("Input file command has too many arguments.\nFile commands must be in the following format: \n'<Launchgroup #>, <Process name>, <Argument list>'");
			}
			else if (commandCount == 1) {
				if (!isdigit(tempString[0])) {
					print_error_to_console("Input file launchgroup command is invalid.\nLaunchgroup must be an integer but was '" + tempString + "'");
				}
			}
			stringStart = false;
			strings->push_back(tempString);
			tempString = "";
			tempSpaces = "";
		}
		else {			
			//set the new string flag
			stringStart = true;
			//if whitespace is encountered in string store it in a seperate string
			//add it to string after non-whitespace chraracter is found
			//if delimiter is hit before that whitespace string will be discarded
			if (c == ' ' || c == '\t') {
				tempSpaces += input[i];
			}
			//add characters
			else {
				tempString += tempSpaces;
				tempSpaces = "";
				tempString += c;
			}

		}
	}
	//add the last string to the vector since string (probably) wont end on a delimiter
	strings->push_back(tempString);
}
//function to sort vector of vector of strings by the first element in the vector converted to an int
bool sortByLaunchGroup(const vector<string>& lhs, const vector<string>& rhs) {
	return atoi(lhs.at(0).c_str()) < atoi(rhs.at(0).c_str());	
}
