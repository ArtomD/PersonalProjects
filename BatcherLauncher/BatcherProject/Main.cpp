/*
*Author: Artom Didinchuk
*Date: 2019-10-01
*Version 1.0
*This is the main entry point for the application.
*This application will read in a file that denotes processes, arguments for them and the order to 
*launch them in and then will launch the process sequentially or concurrently as requested.
*/
#include "LaunchUtils.h"
#include "ConsoleUtils.h"
#include "FileUtils.h"

using namespace std;

int main(int argc, char* argv[]) {

	//get the file with commands
	string fileName = get_file_name(argc, argv);
	//make the command list
	vector<vector<string>> commands = parse_command_file(fileName);	
	//execute command list
	launch_processes_by_launch_group(commands);
	return 0;
}