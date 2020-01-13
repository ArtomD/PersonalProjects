#include "ThreadUtils.h"

namespace multigrep {

	void ThreadPool::setup_threads() {
		//launch the results thread
		sharedvars::finished = false;
		sharedvars::outputFinished = false;
		sharedvars::wakeEvent = CreateEvent(NULL, false, false, NULL);
		sharedvars::startEvent = CreateEvent(NULL, false, false, NULL);
		sharedvars::outputWakeEvent = CreateEvent(NULL, false, false, NULL);
		sharedvars::outputStartEvent = CreateEvent(NULL, false, false, NULL);
		sharedvars::outputThread32 = CreateThread(NULL, 0, threadResultsRunner, NULL, 0, NULL);
		sharedvars::inboundSockThread32 = CreateThread(NULL, 0, inboundSocketThreadRunner, NULL, 0, NULL);
	}

	void ThreadPoolC11::setup_threads() {
		//launch the results thread
		sharedvars::finished = false;
		sharedvars::outputFinished = false;
		sharedvars::wakeEvent = CreateEvent(NULL, false, false, NULL);
		sharedvars::startEvent = CreateEvent(NULL, false, false, NULL);
		sharedvars::outputWakeEvent = CreateEvent(NULL, false, false, NULL);
		sharedvars::outputStartEvent = CreateEvent(NULL, false, false, NULL);
		sharedvars::outputThread = &std::thread(ThreadPoolC11::threadResultsRunner);
		sharedvars::inboundSockThread = &std::thread(ThreadPoolC11::inboundSocketThreadRunner);
	}


	void ThreadPool::launch_threads() {
		//waits for params to be loaded
		WaitForSingleObject(sharedvars::startEvent, INFINITE);
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
		//waits for params to be loaded
		WaitForSingleObject(sharedvars::startEvent, INFINITE);
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
		std::cout << "Worker threads finished." << std::endl;
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

	DWORD WINAPI ThreadPool::threadResultsRunner(LPVOID) {
		WaitForSingleObject(sharedvars::outputStartEvent, INFINITE);
		threadResultsLoop();
		return 0;
	}
	//launches the inbound listening socket loop
	DWORD WINAPI ThreadPool::inboundSocketThreadRunner(LPVOID) {
		inboundSocketLoop();
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

	void ThreadPoolC11::threadResultsRunner() {
		WaitForSingleObject(sharedvars::outputStartEvent, INFINITE);
		threadResultsLoop();
	}

	void ThreadPoolC11::inboundSocketThreadRunner() {
		inboundSocketLoop();
	}

	ThreadPool::~ThreadPool() {
		//delete params object declared as a pointer
		delete sharedvars::params_ptr;
		//close and delete all threads
		for (auto& t : sharedvars::threads) {
			CloseHandle(t->thread32);
			delete t;
		}
		sharedvars::threads.clear();
		//close wake handle event
		CloseHandle(sharedvars::wakeEvent);
		CloseHandle(sharedvars::startEvent);
		CloseHandle(sharedvars::outputWakeEvent);
		CloseHandle(sharedvars::outputStartEvent);

	}

	ThreadPoolC11::~ThreadPoolC11() {
		//delete params object declared as a pointer
		delete sharedvars::params_ptr;
		//close and delete all threads
		for (auto& t : sharedvars::threads)
			delete t;
		sharedvars::threads.clear();
		//close wake handle event
		CloseHandle(sharedvars::wakeEvent);
		CloseHandle(sharedvars::startEvent);
		CloseHandle(sharedvars::outputWakeEvent);
		CloseHandle(sharedvars::outputStartEvent);
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
						if (!t->isFinished) {
							allDone = false;//if some thread is not done set main loop flag back to false
							break;
						}						
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

	void ThreadPool::threadResultsLoop() {
		std::cout << "Output thread started." << std::endl;
		bool hasWork = true;
		while (!sharedvars::outputFinished || hasWork) {
			std::string output;			
			{
				std::lock_guard<std::mutex> lk(sharedvars::outputMtx);
				if (!sharedvars::output.values.empty()) {
					hasWork = true;
					output = sharedvars::output.values.front();
					sharedvars::output.values.pop();
				}
				else {
					hasWork = false;
				}
			}
			if (hasWork) {
				sharedvars::outboundSocket->sendPayload(output);
			}
			else {
				if (sharedvars::outputFinished) {
					break;
				}
				WaitForSingleObject(sharedvars::outputWakeEvent, INFINITE);
				hasWork = true;
			}
		}
		sharedvars::outboundSocket->closeConnection();
		delete sharedvars::outboundSocket;
		std::cout << "Output thread finished." << std::endl;
	}

	void ThreadPool::inboundSocketLoop() {
		try {
			sharedvars::inboundSocket = new sockets::InboundSocket(sharedvars::hostPort);
		}
		catch (std::exception e) {
			std::cout << e.what() << std::endl;
		}
		std::cout << "Listener thread started on port " + sharedvars::hostPort << std::endl;
		sharedvars::inboundSocket->acceptConnection();
		sockets::CommandData data = {sockets::Standby,0,nullptr};
		bool isConnecting = false;
		bool hasPort = false;
		bool hasAddress = false;

		bool gettingParams = false;
		bool verbose;
		std::string path;
		std::string regex;
		std::string extensions;
		while (data.command != sockets::Exit) {
			data = sharedvars::inboundSocket->receiveCommand();
			if (data.command == sockets::Connect) {
				isConnecting = true;
				continue;
			}
			else if (data.command == sockets::Params) {
				gettingParams = true;
				continue;
			}
			else if (data.command == sockets::ShutdownServer) {
				sharedvars::shutdownServer = true;
				break;
			}
			if (isConnecting) {
				if (!hasPort && data.command == sockets::IncomingInt) {
					//int i = 0;
					std::copy(
						&data.data[0],
						&data.data[0] + sizeof(int),
						reinterpret_cast<char*>(&sharedvars::clientPort)
					);
					//outboundPort = i;
					hasPort = true;
					continue;
				}
				else if (!hasAddress && data.command == sockets::IncomingString) {
					sharedvars::clientAddress = data.data;
					hasAddress = true;
				}
				if (hasPort && hasAddress) {
					try{
						sharedvars::outboundSocket = new sockets::OutboundSocket(sharedvars::clientPort);
						sharedvars::outboundSocket->attemptConnection(sharedvars::clientAddress, sharedvars::clientPort);
					}
					catch (std::exception e) {
						std::cout << e.what() << std::endl;
					}
					std::cout << "Connected to " << sharedvars::clientAddress << ":" << sharedvars::clientPort << std::endl;
					SetEvent(sharedvars::outputStartEvent);
					isConnecting = false;
					continue;
				}
			}
			else if(gettingParams) {
				if (data.command == sockets::IncomingBool) {
					//bool b = false;
					std::copy(
						&data.data[0],
						&data.data[0] + sizeof(bool),
						reinterpret_cast<char*>(&verbose)
					);
					//verbose = b;
					continue;
				}
				else if (path.empty() && data.command == sockets::IncomingString) {
					path = data.data;
					continue;
				}
				else if (regex.empty() && data.command == sockets::IncomingString) {
					regex = data.data;
					continue;
				}
				else if (extensions.empty() && data.command == sockets::IncomingString) {
					sharedvars::timer.start_timer();
					extensions = data.data;
					std::cout << "Params received\n" << "Path: " << path << std::endl;
					std::cout << "Verbose: " << verbose << std::endl;
					std::cout << "Regex: " << regex << std::endl;
					std::cout << "Extensions: " << extensions << std::endl;
					//set the first location to search with the supplied start location
					sharedvars::command.directories.push(path);
					//create the parameter object
					sharedvars::params_ptr = new sharedvars::SearchParams(extensions, regex, verbose);
					SetEvent(sharedvars::startEvent);
					gettingParams = false;
					continue;
				}			
			}
			

			//more commands from client
		}
		sharedvars::params_ptr = new sharedvars::SearchParams("", "", false);
		SetEvent(sharedvars::startEvent);
		delete sharedvars::inboundSocket;
	}


	void ThreadPool::stopResultsThread() {
		WaitForSingleObject(sharedvars::outputThread32, INFINITE);
		WaitForSingleObject(sharedvars::inboundSockThread32, INFINITE);
	}

	void ThreadPoolC11::stopResultsThread() {
		sharedvars::outputThread->join();
		sharedvars::inboundSockThread->join();
	}
	
}