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
		HANDLE wakeEvent;
		HANDLE startEvent;

		std::thread *outputThread;
		HANDLE outputThread32;
		OutputValues output;
		std::mutex outputMtx;
		std::atomic<bool> outputFinished;
		HANDLE outputWakeEvent;

		std::thread* inboundSockThread;
		HANDLE inboundSockThread32;
		HANDLE outputStartEvent;

		sockets::InboundSocket* inboundSocket;
		sockets::OutboundSocket* outboundSocket;

		unsigned short hostPort = 27015;
		unsigned short clientPort;

		std::string hostAddress = "0.0.0.0";
		std::string clientAddress;

		bool shutdownServer = false;
		Timer timer;
	}
}
