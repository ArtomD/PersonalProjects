/*
*Author: Artom Didinchuk
*Date: 2019-10-01
*Version 1.0
*This is the implimentation of the functions that print output to console as well as functions that read in and process user input.
*/
#include "ConsoleUtils.h"


void print_Program_Return_to_console(Program_Return output) {
	cout << "Launch group: " << output.launchGroup << endl;
	cout << "Kernel time: " << output.kTime.wHour << ":" << output.kTime.wMinute << ":" << output.kTime.wSecond << "." << output.kTime.wMilliseconds << endl;
	cout << "User time: " << output.uTime.wHour << ":" << output.uTime.wMinute << ":" << output.uTime.wSecond << "." << output.uTime.wMilliseconds << endl;
	cout << "Exit code: " << output.exitCode << endl;
	wcout << L"Program Name: " << output.application << endl;
	wcout << "Command parameters: " << output.params << endl << endl;
}

void print_process_errors_to_console(vector<Program_Return> errors) {
	cout << "The following processes had errors during launch" << endl <<endl;
	for (Program_Return output : errors) {
		print_Program_Return_to_console(output);
	}
}

void print_help_to_console() {
	cout << "This program takes one argument which is the name of the file with processes to run." << endl;
	exit(0);
}

string get_file_name(int argc, char* argv[]) {
	string fileName;
	if (argc == 2) {
		if (strcmp(argv[1],"-h") == 0 || strcmp(argv[1], "-H") == 0 || strcmp(argv[1], "-help") == 0) {
			print_help_to_console();
		}
		else {
			fileName = argv[1];
		}
	}
	else {
		print_help_to_console();
	}
	return fileName;
}

void print_error_to_console(string errorMsg) {
	cout << "There was an error processing the input. Please check the arguments and input file and try again." << endl;
	cout << "Error message:" << endl << errorMsg << endl;
	cout << "Application will now terminate." << endl;
	exit(1);
}
