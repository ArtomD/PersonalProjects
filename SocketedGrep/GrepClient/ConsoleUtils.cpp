#include "ConsoleUtils.h"

std::vector<std::string> split_string(std::string input, char delim) {
	//declare return vector
	std::vector<std::string> list;
	std::string tempString = "";
	bool hasStarted = false;
	bool openQuote = false;
	for (std::string::size_type i = 0; i < input.size(); i++) {
		char c = input[i];
		//skip any whitespace
		if (c == '\t' || c == '\n') {
			continue;
		}
		if (c=='"') {		
			if (openQuote) {
				openQuote = false;
				if (!hasStarted) {
					tempString += c;
					hasStarted = true;
				}
				else {
					list.push_back(tempString);
					tempString = "";
				}
				continue;
			}
			else {
				openQuote = true;
				continue;
			}
			
		}
		//split string according to '.' chars
		if (c == delim && !openQuote) {			
			list.push_back(tempString);
			tempString = "";			
		}
		else {
			tempString += c;
		}
	}
	list.push_back(tempString);

	return list;
}

void print_help() {
	AddText("This client connects to a server for the purposes of searching the files on it.");
	AddText("The following is a list of commands");
	AddText("display <width> [height] - changes the console size.");
	AddText("connect <ip address> - connects to a server at the specified IP address.");
	AddText("drop - disconnects from the current server.");
	AddText("stopserver - shuts down and disconnects from the currently connected server.");
	AddText("exit - closes the application.");
	AddText("grep [-v] <path> <regex> [extensions] - Searches the server for the specified matches");
	AddText("The [-v] flag is for verbose output.");
	AddText("By default .txt files are searched. To search by different extensions add them as one contiguous string");
	AddText("ex. .txt.pdf.docx");
	AddText("Example usage:");
	AddText("grep -v C:\\Users\\Public\\Documents text .txt.docx");
	AddText("");
}