#include "ConsoleUtils.h"
namespace multigrep {
	void parse_input_to_params(int argc, char* argv[]) {
		bool verbose = false;
		int index = 0;
		//if no arguments or the help flag is set print help and exit
		if (argc==1 || std::strcmp(argv[1], "-h") == 0 || std::strcmp(argv[1], "-help") == 0) {
			print_help();
			exit(0);
		}
		//check for the verbose flag
		if (std::strcmp(argv[1],"-v")==0) {
			++index;
			verbose = true;			
		}
		//check to make sure there are not too many arguments
		if (argc > index + 4) {
			std::cout << "The program has encountered an error and will terminate." << std::endl;
			print_error("Too many arguments in command. Use -h or -help to display instruction on how to use this program.");
			exit(1);
		}
		//set the starting location of the search
		std::string startPath = argv[++index];
		//if the path starts with . substitute the current directory path for it 
		if (argv[index][0]=='.' && argv[index][1] == '\\') {
			std::string tempPath = argv[index];
			tempPath.erase(0,1);
			char cCurrentPath[FILENAME_MAX];
			_getcwd(cCurrentPath, sizeof(cCurrentPath));
			std::string current(cCurrentPath);
			if (!tempPath.empty())
				current += tempPath;
			startPath = current;
		}
		//check to make sure start location is valid
		if (!boost::filesystem::is_directory(startPath)) {
			std::cout << "The program has encountered an error and will terminate." << std::endl;
			print_error("Specified directory \"" + startPath +"\" not found.");
			exit(1);
		}
		//set the regex and extensions if supplied 
		std::string regex = argv[++index];
		std::string ext = ".txt";
		if (argc > ++index) {
			ext = argv[index];
		}
		//set the first location to search with the supplied start location
		sharedvars::command.directories.push(startPath);
		//create the parameter object
		sharedvars::params_ptr = new sharedvars::SearchParams(ext, regex, verbose);
	}

	void print_results(double timeFinished) {
		std::cout << std::endl << "SEARCH COMPLETED IN " << timeFinished << " SECONDS" << std::endl;
		unsigned totalMatches = 0;
		for (const sharedvars::FileMatch& file : sharedvars::results) {
			std::cout << file.fileName << " with " << file.lineNumbers.size() << " matches on lines:" << '\n';
			for (unsigned i = 0; i < file.lineNumbers.size(); ++i) {
				std::cout << "[" << file.lineNumbers.at(i).at(0) << " : " << file.lineNumbers.at(i).at(1) << "] ";
				std::cout << file.lines.at(i) << std::endl;
				++totalMatches;
			}
			std::cout << std::endl;
		}
		std::cout << " Total files found: " << sharedvars::results.size() << std::endl;
		std::cout << " Total matches: " << totalMatches << std::endl;
	}

	void print_help() {
		std::cout << "This program takes a file path and regular expression and searches all files on that path recursively for matches." << std::endl;
		std::cout << "For verbose output a [-v] flag can be set before the argument list." << std::endl;
		std::cout << "By default .txt files are searched. To search by different extensions add them as one contigous string ex." << std::endl << "[.txt.pdf.docx]" << std::endl;
		std::cout << std::endl;
		std::cout << "Example usage:" << std::endl;
		std::cout << "Grep.exe -v C:\\Users\\Public\\Documents text .txt.docx" << std::endl;
	}

	void print_error(std::string msg) {
		std::cout << "Error  message: " << msg << std::endl;
	}
}
