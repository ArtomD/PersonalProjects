#include "LaunchUtils.h"
#include "ConsoleUtils.h"

void launch_processes_by_launch_group(vector<vector<string>> commands) {
	//converter to change between wide are regular charset
	wstring_convert<codecvt_utf8_utf16<wchar_t>> converter;
	//sets the first launchgroup
	string currentLaunchGroup = commands.at(0).at(0).c_str();
	//vector of threads to run concurrently
	vector<future<Program_Return>> currentProcesses;
	//vector of error objects to track
	vector<Program_Return> errors;
	for (vector<string> const& command : commands) {
		//launches all threads for the current launchgroup
		if (command.at(0) == currentLaunchGroup) {
			currentProcesses.push_back(async(launch_using_create_process, converter.from_bytes(command.at(1)), converter.from_bytes(command.at(2))));
		}
		//next launchgroup is found
		else{
			//waits for all current threads to complete;
			wait_for_current_group(currentLaunchGroup, currentProcesses, errors);
			//sets the current launchgroup to the next one
			currentLaunchGroup = command.at(0).c_str();			
			//launches the first thread of the new current luanchgroup
			currentProcesses.push_back(async(launch_using_create_process, converter.from_bytes(command.at(1)), converter.from_bytes(command.at(2))));
		}
	}
	//when end of list is found
	//waits for final launchgroup to finish
	wait_for_current_group(currentLaunchGroup, currentProcesses, errors);
	//if there are errors print them at the end
	if(!errors.empty())
		print_process_errors_to_console(errors);
}

Program_Return launch_using_create_process(wstring application, wstring params) {
	//builds the command to launch the process
	wstring command = application + L" " + params;
	//initilize the return object
	Program_Return output;

	//set process data and start it up
	STARTUPINFO sinfo = { 0 };
	sinfo.cb = sizeof(STARTUPINFO);
	PROCESS_INFORMATION pi = { 0 };
	unsigned long const CP_MAX_COMMANDLINE = 32768;
	try {
		wchar_t* commandline = new wchar_t[CP_MAX_COMMANDLINE];
		wcsncpy_s(commandline, CP_MAX_COMMANDLINE, command.c_str(), command.size() + 1);
		auto res = CreateProcess(
			NULL,//name of the application, NULL means pick it up from command line
			commandline,// name of program + params plus space if it needs to be modified
			NULL,//security attributes
			NULL,//thread attributes
			false,//inhibit handles
			CREATE_NEW_CONSOLE, //creation flags
			NULL, //enviroment
			NULL,// current dir to launch from
			&sinfo,
			&pi
		);
		delete[] commandline;
	}
	//catch error in memory allocation
	catch (std::bad_alloc&) {
		output.kTime = { 0 };
		output.uTime = { 0 };
		output.exitCode = EXIT_FAILURE;
		output.hasError = true;
		output.application = application;
		output.params = params;
		return output;
	}
	//catch general error in launching or finishing process
	if (WAIT_FAILED == WaitForSingleObject(pi.hProcess, INFINITE)) {
		output.kTime = { 0 };
		output.uTime = { 0 };
		output.exitCode = EXIT_FAILURE;
		output.hasError = true;
		output.application = application;
		output.params = params;
		return output;
	}
	//after process finishes get relevant process data
	DWORD exitCode = 0;
	GetExitCodeProcess(pi.hProcess, &exitCode);
	FILETIME createTime, exitTime, kernelTime, userTime;
	GetProcessTimes(pi.hProcess, &createTime, &exitTime, &kernelTime, &userTime);
	SYSTEMTIME kTime;
	SYSTEMTIME uTime;
	FileTimeToSystemTime(&kernelTime, &kTime);
	FileTimeToSystemTime(&userTime, &uTime);
	//close the process
	CloseHandle(pi.hThread);
	CloseHandle(pi.hProcess);
	//populate and return the return object
	output.application = application;
	output.params = params;
	output.exitCode = exitCode;
	output.kTime = kTime;
	output.uTime = uTime;
	output.hasError = false;
	return output;

}



void wait_for_current_group(string& currentLaunchGroup, vector<future<Program_Return>>& currentProcesses, vector<Program_Return> &errors) {

	for (future<Program_Return>& currentThread : currentProcesses) {
		//if process is not yet finished wait for it to get done
		Program_Return output = currentThread.get();
		//set the launchgroup number
		output.launchGroup = currentLaunchGroup;
		//if process had no errors print it to console, otherwise store error object
		if (!output.hasError) {
			print_Program_Return_to_console(output);
		}
		else {
			errors.push_back(output);
		}
	}
	//clear the process vector
	currentProcesses.clear();
}