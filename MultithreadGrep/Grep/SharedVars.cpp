#include "SharedVars.h"

namespace multigrep {
	namespace sharedvars {

		SearchParams* params_ptr;
		std::vector<FileMatch> results;
		std::mutex resultsMtx;
		SearchCommand command;
		std::mutex commandMtx;
		std::vector<ThreadObj*> threads;
		std::mutex finishedMtx;
		std::atomic<bool> finished;
		HANDLE wakeEvent = NULL;
		std::mutex printMtx;

		void cleanup(bool win32) {
			//delete params object declared as a pointer
			delete params_ptr;
			//close and delete all threads
			if (win32) {
				for (auto& t : threads)
					CloseHandle(t->thread32);
			}
			for (auto& t : threads)
				delete t;
			threads.clear();
			//close wake handle event
			CloseHandle(wakeEvent);
		}
	}
}
