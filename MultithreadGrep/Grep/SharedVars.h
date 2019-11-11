/*
*Author: Artom Didinchuk
*Date: 2019-11-05
*Version 1.0
*This file houses gloabal variables needed by threads in a nested namespace
*/
#pragma once
#include <Windows.h>
#include <mutex>
#include <string>
#include<vector>
#include <queue>
#include <atomic>

#include "StringUtils.h"

namespace multigrep {
	namespace sharedvars {
		//parameters of grep search stored here
		struct SearchParams {
			std::vector<std::string> const extensionList;
			std::string const match;
			bool const verbose;
			SearchParams(std::string ext, std::string match, bool verbose) : match(match), extensionList(split_extensions(ext)), verbose(verbose) {}
		};
		extern SearchParams* params_ptr;

		//Results of grep stored here
		struct FileMatch {
			std::vector<std::vector<unsigned>> lineNumbers;
			std::vector<std::string> lines;
			std::string fileName;
			bool isFound;
		};
		extern std::vector<FileMatch> results;
		extern std::mutex resultsMtx;

		//list of search location stored here before being executed
		struct SearchCommand {
			std::queue<std::string> directories;
		};
		extern SearchCommand command;
		extern std::mutex commandMtx;

		//threadpool object
		struct ThreadObj {
			std::thread thread;
			HANDLE thread32;
			DWORD thread32_id;
			bool isFinished;
		};
		extern std::vector<ThreadObj*> threads;
		//thread management vars
		extern std::mutex finishedMtx;
		extern std::atomic<bool> finished;
		extern HANDLE wakeEvent;
			   
		//lock for multithread printing to console
		extern std::mutex printMtx;
		/*
		*This function takes in an bool if the program is runnning with c++ 11+ compatability or not. 
		*It cleanes up all the used threads and pointers
		*Arguments:
		*	win32 - if the program is running below c++ 11 this is true
		*/
		void cleanup(bool win32);
	}
}

