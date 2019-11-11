#include "ThreadUtils.h"

namespace multigrep {
	void ThreadPool::launch_threads() {
		//get system info to see how many threads to launch
		SYSTEM_INFO si;
		GetSystemInfo(&si);
		auto nThreads = si.dwNumberOfProcessors;

		for (unsigned i = 0; i < nThreads; ++i) {
			sharedvars::ThreadObj* threadobj = new sharedvars::ThreadObj;
			//create a win32 thread
			DWORD threadId;
			threadobj->thread32 = CreateThread(NULL, 0, threadRunner, NULL, 0, &threadId);
			threadobj->thread32_id = threadId;
			//populates the global threadpool object
			threadobj->isFinished = false;
			sharedvars::threads.push_back(threadobj);
		}
	}

	void ThreadPoolC11::launch_threads() {
		//get system info to see how many threads to launch
		SYSTEM_INFO si;
		GetSystemInfo(&si);
		auto nThreads = si.dwNumberOfProcessors;

		for (unsigned i = 0; i < nThreads; ++i) {
			sharedvars::ThreadObj* threadobj = new sharedvars::ThreadObj;
			//create a c++11 thread
			threadobj->thread = std::thread(ThreadPoolC11::threadRunner);
			//populates the global threadpool object
			threadobj->isFinished = false;
			sharedvars::threads.push_back(threadobj);
		}
	}



	void ThreadPool::get_results() {
		std::vector<HANDLE> threads32;
		for (sharedvars::ThreadObj *obj : sharedvars::threads) {
			WaitForSingleObject(obj->thread32, INFINITE);
		}			
	}

	void ThreadPoolC11::get_results() {
		for (auto& t : sharedvars::threads) {
			t->thread.join();
		}
	}

	DWORD WINAPI ThreadPool::threadRunner(LPVOID) {
		bool* threadFinished = NULL;
		for (auto& t : sharedvars::threads) {
			if (t->thread32_id == GetCurrentThreadId()) {
				threadFinished = &(t->isFinished);
				break;
			}
		}
		threadLoop(threadFinished);
		return 0;
	}

	void ThreadPoolC11::threadRunner() {
		//stores the thread id of the current thread
		bool* threadFinished = NULL;
		for (auto& t : sharedvars::threads) {
			if (t->thread.get_id() == std::this_thread::get_id()) {
				threadFinished = &(t->isFinished);
				break;
			}
		}
		threadLoop(threadFinished);
	}

	void ThreadPool::threadLoop(bool *threadFinished) {
		while (!sharedvars::finished) {
			std::string dir;
			bool hasWork = false;
			//checks if there is work to do
			{
				std::lock_guard<std::mutex> lk(sharedvars::commandMtx);
				if (!sharedvars::command.directories.empty()) {
					hasWork = true;
					dir = sharedvars::command.directories.front();
					sharedvars::command.directories.pop();
				}
			}
			//if there is work thread sets its finished flag to false and starts job
			if (hasWork) {
				{
					std::lock_guard<std::mutex> lk(sharedvars::finishedMtx);
					*threadFinished = false;
				}
				searchDirectory(dir);
			}
			else {
				bool allDone = true; //sets a local representation of the main loop flag to true
				{
					std::lock_guard<std::mutex> lk(sharedvars::finishedMtx);
					*threadFinished = true;//if there is no work thread sets its finished flag to true
					for (auto& t : sharedvars::threads) {//then it checks all the other thread's finished flags
						if (t->isFinished) {
							continue;//if all the flags are true that means there is no more work and it keeps the main loop flag as true
						}
						allDone = false;//if some thread is not done set main loop flag back to false
						break;
					}
				}
				sharedvars::finished = allDone;
				//if the main loop flag is true wake up any sleeping threads
				if (sharedvars::finished) {
					for (auto& t : sharedvars::threads)
						SetEvent(sharedvars::wakeEvent);
					break; //exit out of the loop before going to sleep
				}
				//if the main loop flag is false and there is no more work then go to sleep and wait for more work.
				WaitForSingleObject(sharedvars::wakeEvent, INFINITE);
			}
		}
	}
	
}