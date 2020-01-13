/*
*Author: Artom Didinchuk
*Date: 2019-11-05
*Version 1.0
*This file houses gloabal variables needed by threads in a nested namespace
*/
#pragma once
#define _WINSOCKAPI_
#include <Windows.h>
#include <mutex>
#include <string>
#include <vector>
#include <queue>
#include <atomic>

#include "StringUtils.h"
#include "Sockets.h"
#include "TimeUtils.h"
namespace multigrep {
	namespace sharedvars {
		//parameters of grep search stored here
		struct SearchParams {
			std::vector<std::string> const extensionList;
			std::string const match;
			bool const verbose;
			SearchParams(std::string ext, std::string match, bool verbose) : match(match), extensionList(split_string(ext, '.')), verbose(verbose) {}
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
		extern HANDLE startEvent;
			   
		extern std::thread *outputThread;
		extern HANDLE outputThread32;
		//queue of output strings to be sent back
		struct OutputValues {
			std::queue<std::string> values;
		};
		extern OutputValues output;
		//lock for outputstring queue
		extern std::mutex outputMtx;
		extern std::atomic<bool> outputFinished;
		extern HANDLE outputWakeEvent;

		//socket thread
		extern std::thread* inboundSockThread;
		extern HANDLE inboundSockThread32;
		extern HANDLE outputStartEvent;

		//sockets
		extern sockets::InboundSocket* inboundSocket;
		extern sockets::OutboundSocket* outboundSocket;

		//connection data
		extern unsigned short hostPort;
		extern unsigned short clientPort;
		extern std::string hostAddress;
		extern std::string clientAddress;

		//internal server vars
		extern bool shutdownServer;
		extern Timer timer;
	}
}

