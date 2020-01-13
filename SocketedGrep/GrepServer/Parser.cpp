#include "Parser.h"
namespace multigrep {
	bool extensionMatch(std::string extensionMatch) {
		for (const std::string& extension : sharedvars::params_ptr->extensionList) {
			if (extension == extensionMatch)
				return true;
		}
		return false;
	}

	sharedvars::FileMatch fileContains(std::string name, std::string match) {
		
		//sets up the file object to return
		sharedvars::FileMatch fileMatch;
		fileMatch.fileName = name;
		fileMatch.isFound = false;
		//sets up theregex object
		std::regex r(match);
		std::smatch matchR;
		std::string line;
		std::fstream file(name);
		//keeps track of line number
		unsigned index = 1;
		while (std::getline(file, line))
		{
			bool isMatch = false;
			unsigned count = 0;
			std::string partial = line;
			//tries to match a line as much as possible. Keeps track of how many times it was able to match.
			while (std::regex_search(partial, matchR, r)) {
				if (!isMatch)
					isMatch = true;
				++count;
				partial = matchR.suffix();
			}
			//if a match was found stores the match data in the file object
			if (isMatch) {
				std::vector<unsigned> m(2);
				m.at(0) = index;
				m.at(1) = count;
				fileMatch.lineNumbers.push_back(m);
				fileMatch.lines.push_back(line);
				fileMatch.isFound = isMatch;
			}
			++index;
		}
		return fileMatch;
	}

	void searchDirectory(std::string path) {
		if (sharedvars::params_ptr->verbose) {
			{
				std::lock_guard<std::mutex> lk(sharedvars::outputMtx);
				sharedvars::output.values.push("Scanning: " + boost::filesystem::system_complete(path).string());
			}
		}
		try {
			//iterates over all items in the directory
			using iterator = boost::filesystem::directory_iterator;
			for (iterator iter(path); iter != iterator(); ++iter) {
				//additional directories are stored to search later
				if (boost::filesystem::is_directory(iter->path())) {
					{
						std::lock_guard<std::mutex> lk(sharedvars::commandMtx);
						sharedvars::command.directories.push(boost::filesystem::system_complete(iter->path()).string());
					}
					//wakeup any sleeping threads and set finishd flag to false if a directory is found and added
					SetEvent(sharedvars::wakeEvent);
					sharedvars::finished = false;
				}
				else {
					//if a file is found it is first matched by extension andthen if it passes a regex search is done on it.
					if (extensionMatch(iter->path().extension().string())) {
						sharedvars::FileMatch found = fileContains(boost::filesystem::system_complete(iter->path()).string(), sharedvars::params_ptr->match);
						if (found.isFound) {
							if (sharedvars::params_ptr->verbose) {
								{
									//Comment is printed out after file has been searched to keep lines togethr and not block other threads while search is going
									std::lock_guard<std::mutex> lk(sharedvars::outputMtx);
									sharedvars::output.values.push("Grepping: \"" + boost::filesystem::system_complete(iter->path()).string() + "\"");
									sharedvars::output.values.push("Match Found");
								}
							}
							
							{
								std::lock_guard<std::mutex> lk(sharedvars::resultsMtx);
								sharedvars::results.push_back(found);
							}
						}else{		
							if (sharedvars::params_ptr->verbose) {
								{
									std::lock_guard<std::mutex> lk(sharedvars::outputMtx);
									sharedvars::output.values.push("Grepping: " + boost::filesystem::system_complete(iter->path()).string());
								}
							}
						}//end match regex if
					}//end match ext if
				} //end dir/file if
			}//end for
			SetEvent(sharedvars::outputWakeEvent);
		}
		catch (const std::exception&) {
				print_error("The directory \"" + path + "\" could not be accessed");
		}
	}

}