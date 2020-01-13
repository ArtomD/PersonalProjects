/*
*Author: Artom Didinchuk
*Date: 2019-11-07
*Version 2.0
*This is the main entry point for the application.
*This application do grep search based on a supplied starting location and regex and file extensions to search by
*/

#include "TimeUtils.h"
#include "ThreadUtils.h"
#include "ConsoleUtils.h"


using namespace multigrep;

int main(int argc, char* argv[])
{
	bool C11 = false; __cplusplus >= 201103L;
	bool isStopped = false;
	while (!sharedvars::shutdownServer) {
		int code = -1;// parse_input_to_params(argc, argv);

		ThreadPool* tPool;
		if (C11) {
			tPool = new ThreadPoolC11;
		}
		else {
			tPool = new ThreadPool;
		}
		tPool->setup_threads();
		if (code == -1) {
			tPool->launch_threads();
			tPool->get_results();
			sharedvars::timer.stop_timer();
			if (!sharedvars::shutdownServer) {
				print_results(sharedvars::timer.time_elapsed());
			}
			else {
				SetEvent(sharedvars::outputWakeEvent);
				sharedvars::outputFinished = true;
			}
		}

		tPool->stopResultsThread();
		delete tPool;
		std::cout << "Work completed, restarting threads." << std::endl;
	}
	std::cout << "Server shutting down." << std::endl;
}