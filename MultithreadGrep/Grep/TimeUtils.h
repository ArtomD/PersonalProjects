/*
*Author: Artom Didinchuk
*Date: 2019-11-05
*Version 1.0
*This file is for the custom timer used to track how long the process took in seconds
*/
#pragma once
#include <Windows.h>
namespace multigrep {
	class Timer {
	private:
		LARGE_INTEGER threadStart, threadStop, frequency;
	public:
		void start_timer();
		void stop_timer();
		double time_elapsed();
	};
}
