/*
*Author: Artom Didinchuk
*Date: 2019-10-01
*Version 1.0
*This file houses functions that start up and monitor the processes to be launched.
*/
#pragma once
#include <Windows.h>
#include <string>
#include <vector>
#include <future>

using namespace std;
/*
*This struct holds the information the user needs to see after a process finishes running
*/
struct Program_Return {
	SYSTEMTIME kTime;
	SYSTEMTIME uTime;
	DWORD exitCode;
	wstring application;
	wstring params;
	string launchGroup;
	bool hasError;
};
/*
*This function takes a vector of commands that will instruct it on how to launch the processes.
*Arguments:
*	vector<vector<string>> commands - the commands that instruct on which process to launch and in what order;
*/
void launch_processes_by_launch_group(vector<vector<string>> commands);
/*
*This function takes the process name and arguments and launches it in a new window. 
*After the process completes or errors out it returns a Program_Return object that holds data about the opeartion of the process.
*Arguments:
*	wstring application, wstring params - the process name and arguments it is to be launched with.
*Return:
*	Program_Return - the parsed commands from the file.
*/
Program_Return launch_using_create_process(wstring application, wstring params);
/*
*This function waits for the current launch group to finish and then prints out the results to console. 
*Any errors are passed back to the parent function.
*Arguments:
*	string& currentLaunchGroup - the current launchgroup for the process being launched
*	vector<future<Program_Return>>& currentProcesses - the current processes in that launch group
*	vector<Program_Return> &errors - the vector of error objects eto be updatd incase errors are found.
*/
void wait_for_current_group(string& currentLaunchGroup, vector<future<Program_Return>>& currentProcesses, vector<Program_Return> &errors);
