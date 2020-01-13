/*
*Author: Artom Didinchuk
*Date: 2019-11-05
*Version 1.0
*This file houses thread functions
*/
#pragma once
#define _WINSOCKAPI_
#include <Windows.h>
#include <queue>
#include <string>
#include <thread>
#include <mutex>
#include <atomic>
#include <vector>

#include "SharedVars.h"
#include "Parser.h"
#include "Sockets.h"

namespace multigrep {

	
	
	class ThreadPool {
	public:
		
		void setup_threads();

		/*
		*This function launches 1 thread per logical processor on the machine.
		*/
		void launch_threads();

		/*
		*This function waits for all threads to be finshed
		*/
		void get_results();

		/*
		*Starting function for win32 thread
		*/
		static DWORD WINAPI threadRunner(LPVOID);

		/*
		*Starting function for results win32 thread
		*/
		static DWORD WINAPI threadResultsRunner(LPVOID);

		/*
		*Starting function for inbound socket win32 thread
		*/
		static DWORD WINAPI inboundSocketThreadRunner(LPVOID);

		/*
		*Starting function for c++ thread
		*/
		static void stopResultsThread();

		~ThreadPool();

		/*
		*This function runs inside the thread and makes it look through avaliable directories or sends it to sleep if there is no work.
		*Arguments:
			threadFinished - the pointer to a bool tracking if the current thread has finished all avaliable work
		*/
		static void threadLoop(bool* threadFinished);

		/*
		*This function runs inside the thread and makes it look for more values to sendback to client
		*/
		static void threadResultsLoop();

		/*
		*This function runs inside the thread and reads inbound socket data
		*/
		static void inboundSocketLoop();



	};

	class ThreadPoolC11 : public ThreadPool {
	public:
		void setup_threads();
		/*
		*This function launches 1 thread per logical processor on the machine.
		*/
		void launch_threads();

		/*
		*This function waits for all threads to be finshed
		*/
		void get_results();

		/*
		*Starting function for c++ thread
		*/
		static void threadRunner();

		/*
		*Starting function for c++ results thread
		*/
		static void threadResultsRunner();

		/*
		*Starting function for c++ inbound socket thread thread
		*/
		static void inboundSocketThreadRunner();


		/*
		*Starting function for c++ thread
		*/
		static void stopResultsThread();

		 ~ThreadPoolC11();

	};
}
