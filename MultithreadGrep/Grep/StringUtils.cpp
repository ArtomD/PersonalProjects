#include "StringUtils.h"
namespace multigrep {
	std::vector<std::string> split_extensions(std::string input) {
		//declare return vector
		std::vector<std::string> list;
		std::string tempString = "";
		bool hasStarted = false;
		for (std::string::size_type i = 0; i < input.size(); i++) {
			char c = input[i];
			//skip any whitespace
			if (c == '\t' || c == '\n') {
				continue;
			}
			//split string according to '.' chars
			if (c == '.') {
				if (!hasStarted) {
					tempString += c;
					hasStarted = true;
				}
				else {
					list.push_back(tempString);
					tempString = "";
				}
			}
			else {
				tempString += c;
			}
		}
		list.push_back(tempString);

		return list;
	}

}