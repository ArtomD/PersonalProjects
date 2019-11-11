#include "FileUtils.h"
#include "StringUtils.h"
#include "ConsoleUtils.h"

vector<vector<string>> parse_command_file(string fileName) {
	//declare the converter to go from requal to wide charset
	wstring_convert<codecvt_utf8_utf16<wchar_t>> converter;
	vector<vector<string>> commands;
	ifstream inputFile(fileName);
	string line;
	if (inputFile.is_open())
	{
		//read in each line of the file
		//each line is taken to be a seperate command
		while (getline(inputFile, line))
		{
			vector<string> command;
			//call the parser to separate commands into components
			splitString(line, &command, ',');
			commands.push_back(command);

		}
		inputFile.close();
	}
	else {
		print_error_to_console("File not found. Make sure to supply full path name if file is not located with the executable");
	}
	//sort the command vector
	sort(commands.begin(), commands.end(), sortByLaunchGroup);
	return commands;
}