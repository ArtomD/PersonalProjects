/*
*Author: Artom Didinchuk
*Date: 2019-11-05
*Version 1.0
*This file houses thread functions
*/
#pragma once
#include <Windows.h>
#include <queue>
#include <string>
#include <thread>
#include <mutex>
#include <atomic>
#include <vector>

#include "SharedVars.h"
#include "Parser.h"
namespace multigrep {
	
	class ThreadPool {
	public:
		/*
			*This function launches 1 thread per logical processor on the machine.
			*Arguments:
			*	win32 - whether or not to use c++11 style threads
			*/
		void launch_threads();

		/*
		*This function waits for all threads to be finshed
		*Arguments:
		*	win32 - whether or not to use c++11 style threads
		*/
		void get_results();

		/*
		*Starting function for win32 thread
		*/
		static DWORD WINAPI threadRunner(LPVOID);

		/*
		This function runs inside the thread and makes it look through avaliable directories or sends it to sleep if there is no work.
		Arguments:
			threadFinished - the pointer to a bool tracking if the current thread has finished all avaliable work
		*/
		static void threadLoop(bool* threadFinished);
	};

	class ThreadPoolC11 : public ThreadPool {
		/*
			*This function launches 1 thread per logical processor on the machine.
			*Arguments:
			*	win32 - whether or not to use c++11 style threads
			*/
		void launch_threads();

		/*
		*This function waits for all threads to be finshed
		*Arguments:
		*	win32 - whether or not to use c++11 style threads
		*/
		void get_results();

		/*
		*Starting function for c++ thread
		*/
		static void threadRunner();

		/*
		This function runs inside the thread and makes it look through avaliable directories or sends it to sleep if there is no work.
		Arguments:
			threadFinished - the pointer to a bool tracking if the current thread has finished all avaliable work
		*/

	};
}
