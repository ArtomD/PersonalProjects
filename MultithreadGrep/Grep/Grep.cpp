/*
*Author: Artom Didinchuk
*Date: 2019-11-07
*Version 2.0
*This is the main entry point for the application.
*This application do grep search based on a supplied starting location and regex and file extensions to search by
*Bug causing threads to occasinally hang removed
*/

#include "TimeUtils.h"
#include "ThreadUtils.h"
#include "ConsoleUtils.h"
using namespace multigrep;

int main(int argc, char* argv[])
{
	bool C11 = __cplusplus >= 201103L;
	Timer timer;
	timer.start_timer();
	parse_input_to_params(argc, argv);
	ThreadPool *tPool;
	if (!C11) {
		tPool = new ThreadPoolC11();
	}
	else {
		tPool = new ThreadPool();
	}
	tPool->launch_threads();
	tPool->get_results();
	delete tPool;
	timer.stop_timer();
	print_results(timer.time_elapsed());
	sharedvars::cleanup(C11);
}