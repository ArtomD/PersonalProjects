#include "TimeUtils.h"
namespace multigrep {
	void Timer::start_timer() {
		QueryPerformanceCounter(&threadStart);
		QueryPerformanceFrequency(&frequency);
	}

	void Timer::stop_timer() {
		QueryPerformanceCounter(&threadStop);		
	}

	double Timer::time_elapsed() {
		return (threadStop.QuadPart - threadStart.QuadPart) / double(frequency.QuadPart);
	}
}