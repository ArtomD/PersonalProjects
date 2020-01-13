#include "ConsoleUtils.h"
namespace multigrep {

	void print_results(double timeFinished) {
		{
			std::lock_guard<std::mutex> lk(sharedvars::outputMtx);
			sharedvars::output.values.push("SEARCH COMPLETED IN " + std::to_string(timeFinished) + " SECONDS");
		}
		unsigned totalMatches = 0;
		for (const sharedvars::FileMatch& file : sharedvars::results) {
			{
				std::lock_guard<std::mutex> lk(sharedvars::outputMtx);
				sharedvars::output.values.push(file.fileName + " with " + std::to_string(file.lineNumbers.size()) + " matches on lines:");
				for (unsigned i = 0; i < file.lineNumbers.size(); ++i) {
					sharedvars::output.values.push("[" + std::to_string(file.lineNumbers.at(i).at(0)) + " : " + std::to_string(file.lineNumbers.at(i).at(1)) + "] " + file.lines.at(i));
					
					++totalMatches;
				}
				
			}
		}
		{
			std::lock_guard<std::mutex> lk(sharedvars::outputMtx);
			sharedvars::output.values.push("Total files found : " + std::to_string(sharedvars::results.size()));
			sharedvars::output.values.push("Total matches : " + std::to_string(totalMatches));
		}
		sharedvars::results.clear();
		SetEvent(sharedvars::outputWakeEvent);
		sharedvars::outputFinished = true;
	}

	void print_error(std::string msg) {
		{
			std::lock_guard<std::mutex> lk(sharedvars::outputMtx);
			sharedvars::output.values.push("Error  message: " + msg);
		}
	}
}
