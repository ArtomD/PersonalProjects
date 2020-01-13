#include "DisplayController.h"

// Colours
WORD const FOREGROUND_BLACK = 0;
WORD const FOREGROUND_WHITE = FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE;
WORD const BACKGROUND_BLACK = 0;
WORD const BACKGROUND_WHITE = BACKGROUND_RED | BACKGROUND_GREEN | BACKGROUND_BLUE;

// System Data
HANDLE hConsoleInput, hConsoleOutput;
CONSOLE_SCREEN_BUFFER_INFO	originalCSBI;
CONSOLE_CURSOR_INFO			originalCCI;
std::vector<CHAR_INFO>			originalBuffer;
COORD						originalBufferCoord;
DWORD						originalConsoleMode;
WORD						currentConsoleWidth = 0;


// Utility Functions
void OutputString(WORD x, WORD y, std::string const& s, std::vector<WORD> const& attributes) {
	COORD loc{ (SHORT)x, (SHORT)y };
	DWORD nCharsWritten;
	DWORD nToWrite = DWORD(std::min(s.size(), std::size_t(currentConsoleWidth - x)));

	WriteConsoleOutputCharacterA(hConsoleOutput, s.c_str(), nToWrite, loc, &nCharsWritten);
	WriteConsoleOutputAttribute(hConsoleOutput, attributes.data(), nToWrite, loc, &nCharsWritten);
}
void OutputString(WORD x, WORD y, std::string const& s, WORD attributes) {
	OutputString(x, y, s, std::vector<WORD>(s.size(), attributes));
}



WORD constexpr OUTPUT_SECTION_START = 0;
WORD OUTPUT_SECTION_SIZE = 30;
WORD constexpr OUTPUT_SECTION_ATTR = BACKGROUND_BLUE | FOREGROUND_WHITE | FOREGROUND_INTENSITY;

WORD INPUT_SECTION_START = OUTPUT_SECTION_START + OUTPUT_SECTION_SIZE;
WORD constexpr INPUT_SECTION_SIZE = 1;
WORD constexpr INPUT_SECTION_ATTR = BACKGROUND_BLACK | FOREGROUND_WHITE;


WORD WINDOW_WIDTH = 110;



CONSOLE_SCREEN_BUFFER_INFO sbi;
DWORD charsWritten;
std::vector<std::string> outputData;
int maxOutputDataSize = 5000;
int outputDataIndex = 0;
int breakCount;
int maxBreakCount = 0;

// Application data
std::vector<char> originalTitle(64 * 1024);
bool applicationQuitting = false;
DWORD terminationEventIdx = -1;

bool editControlHasFocus = false;
std::string editControlString;
std::string commandString;
bool hasCommand;

std::string::size_type editControlCursorPos = 0;
decltype(editControlCursorPos) editControlAperture = 0;
//mutex for printing to console.
std::mutex printMtx;
std::mutex outputMtx;

BOOL CtrlHandler(DWORD ctrlType) {
	if (ctrlType <= CTRL_CLOSE_EVENT) {
		restoreWindow();
	}
	return FALSE;
}
//setsthe cursor in the right place in the input aperature
void set_cursor_to_start() {
	COORD cursorLoc = COORD{ (SHORT)0, (SHORT)INPUT_SECTION_START };
	cursorLoc.X += SHORT(editControlCursorPos - editControlAperture);
	SetConsoleCursorPosition(hConsoleOutput, cursorLoc);
}
//displays the text in the output vector to screen.
//if a line in the vector is too long it splits it up into multiple lines
void DisplayOutputText() {
	{
		//lock the cursor
		std::lock_guard<std::mutex> lk(printMtx);
		//set cursor
		CONSOLE_SCREEN_BUFFER_INFO csbi;
		GetConsoleScreenBufferInfo(hConsoleOutput, &csbi);
		DWORD charsWritten;
		DWORD consoleSize = csbi.dwSize.X * OUTPUT_SECTION_SIZE;
		COORD cursorHomeCoord = COORD{ (SHORT)0, (SHORT)OUTPUT_SECTION_START };
		SetConsoleCursorPosition(hConsoleOutput, cursorHomeCoord);
		//fill
		FillConsoleOutputCharacterA(hConsoleOutput, ' ', consoleSize, cursorHomeCoord, &charsWritten);
		FillConsoleOutputAttribute(hConsoleOutput, OUTPUT_SECTION_ATTR, consoleSize, cursorHomeCoord, &charsWritten);
		//keep track of how many lines were printed
		int linecount = 0;
		//keeps track of how many time a line had to be broken
		breakCount = 0;
		for (int i = 0; i < OUTPUT_SECTION_SIZE + 1; i++) {
			//the index of the output vector to be currently written
			int index = (outputDataIndex - OUTPUT_SECTION_SIZE) + linecount;
			//if the output vector runs out of values stop printing
			{
				//lock the output vector
				std::lock_guard<std::mutex> lk(outputMtx);
				if (index >= outputData.size()) {
					break;
				}
				else {
					//if the line is too long it will be split
					if (outputData.at(index).size() > WINDOW_WIDTH) {
						//make a temp string of the line
						std::string tmpText = outputData.at(index);
						while (tmpText.size() > WINDOW_WIDTH) {
							//print the first part of the string
							OutputString((SHORT)0, (SHORT)(OUTPUT_SECTION_START)+(i), tmpText.substr(0, WINDOW_WIDTH), OUTPUT_SECTION_ATTR);
							//remove the first part of the string
							tmpText = tmpText.substr(WINDOW_WIDTH);
							//incriment i which tracks the line on the screen
							++i;
							//incriment the line break amount
							++breakCount;
							//if there is no more space on the screen exit the function
							if (i >= OUTPUT_SECTION_SIZE + 1) {
								set_cursor_to_start();
								return;
							}
						}
						//print the last part of the line and incriment the line counter
						OutputString((SHORT)0, (SHORT)(OUTPUT_SECTION_START)+(i), tmpText, OUTPUT_SECTION_ATTR);
						++linecount;
					}
					else {
						//print the whole line and incriment the line counter
						OutputString((SHORT)0, (SHORT)(OUTPUT_SECTION_START)+(i), outputData.at(index), OUTPUT_SECTION_ATTR);
						++linecount;
					}

				}
			}
		}
		//return the cursor to the input aperature
		set_cursor_to_start();
	}
}

//adds text to the output vector and then refreshes the screen
void AddText(std::string text) {
	//if max is reached erase start
	{
		//lock the output vector
		std::lock_guard<std::mutex> lk(outputMtx);
		if (outputData.size() > maxOutputDataSize) {
			outputData.erase(outputData.begin());
		}
		//add the string and reset the view index to the end of the screen
		outputData.push_back(text);
		outputDataIndex = outputData.size();
	}
	//min size is the screen size
	if (outputDataIndex < OUTPUT_SECTION_SIZE) {
		outputDataIndex = OUTPUT_SECTION_SIZE;
	}
	//refresh view
	DisplayOutputText();
	
}

//scrolls the view up or down
void ScrollLine(int num, bool up) {
	//view is displayed based on outputDataIndex which tracks which part of the vector is being printed
	if (up) {
		int newIndex = outputDataIndex - num;		
		outputDataIndex = newIndex;
	}
	else {
		//break count messes up the line count for downward scrolling
		//this adds a buffer to make sure the bottom text can always be scrolled to
		if (maxBreakCount < breakCount) {
			maxBreakCount = breakCount;
		}
		int newIndex = outputDataIndex + num;
		{
			//lock the output vector
			std::lock_guard<std::mutex> lk(outputMtx);
			if (newIndex >= outputData.size() + maxBreakCount)
				newIndex = outputData.size() + maxBreakCount;
		}
		outputDataIndex = newIndex;
	}
	//index can't be smaller than the screen size
	if (outputDataIndex < OUTPUT_SECTION_SIZE)
		outputDataIndex = OUTPUT_SECTION_SIZE;
	//refresh view
	DisplayOutputText();
}

//changes the display variables and refreshes the main view
void ChangeDisplaySize(int width, int height) {
	WINDOW_WIDTH = width;
	if (height > 0) {
		OUTPUT_SECTION_SIZE = height;
	}
	setupScreen();
	ScrollLine(INT_MAX, false);
}

void ProcessKeyEvent(KEY_EVENT_RECORD const& ke) {
	if (ke.bKeyDown) {
		switch (ke.wVirtualKeyCode) {
		case VK_BACK:
			// backspace to remove at cursor location
			if (0 < editControlCursorPos && editControlCursorPos <= editControlString.size()) {
				--editControlCursorPos;
				editControlString.erase(editControlCursorPos, 1);
			}
			break;

		case VK_DELETE:
			if (0 <= editControlCursorPos && editControlCursorPos < editControlString.size())
				editControlString.erase(editControlCursorPos, 1);
			break;

		case VK_LEFT:
			if (editControlCursorPos > 0)
				--editControlCursorPos;
			break;

		case VK_RIGHT:
			if (editControlCursorPos < editControlString.size())
				++editControlCursorPos;
			break;
			//scroll key events
		case VK_UP:
			ScrollLine(1, true);
			break;
		case VK_DOWN:
			ScrollLine(1, false);
			break;
		case VK_HOME:
			ScrollLine(INT_MAX, true);
			break;
		case VK_END:
			ScrollLine(INT_MAX, false);
			break;
		case VK_PRIOR:
			ScrollLine(15, true);
			break;
		case VK_NEXT:
			ScrollLine(15, false);
			break;
			//sets the command flag and reset input
		case VK_RETURN:
			commandString = editControlString;
			editControlString = "";
			editControlCursorPos = 0;
			hasCommand = true;
			break;

		default:
			// add character
			char ch = ke.uChar.AsciiChar;
			if (isprint(ch))
				editControlString.insert(editControlCursorPos++ + editControlString.begin(), ch);
		}

		// show the string in the control
		auto practicalSize = editControlString.size() + 1;
		while (editControlCursorPos < editControlAperture)
			--editControlAperture;

		while (editControlCursorPos - editControlAperture >= WINDOW_WIDTH)
			++editControlAperture;

		while (practicalSize - editControlAperture<WINDOW_WIDTH && practicalSize > WINDOW_WIDTH)
			--editControlAperture;

		auto s = editControlString.substr(editControlAperture, WINDOW_WIDTH);
		s += std::string(WINDOW_WIDTH - s.size(), ' ');
		{
			//lock the cursor
			std::lock_guard<std::mutex> lk(printMtx);
			OutputString(0, INPUT_SECTION_START, s, INPUT_SECTION_ATTR);

			// place cursor in the control

			COORD cursorLoc = COORD{ (SHORT)0, (SHORT)INPUT_SECTION_START };
			cursorLoc.X += SHORT(editControlCursorPos - editControlAperture);
			SetConsoleCursorPosition(hConsoleOutput, cursorLoc);
		}
	}
}

void setupScreen() {
	originalTitle.resize(GetConsoleTitleA(originalTitle.data(), (DWORD)originalTitle.size()) + 1);
	originalTitle.shrink_to_fit();

	SetConsoleTitleA("Grep Console");

	hConsoleInput = GetStdHandle(STD_INPUT_HANDLE);
	hConsoleOutput = GetStdHandle(STD_OUTPUT_HANDLE);

	//save old console state --------
	GetConsoleScreenBufferInfo(hConsoleOutput, &originalCSBI);

	// Save the desktop
	originalBuffer.resize(originalCSBI.dwSize.X * originalCSBI.dwSize.Y);
	originalBufferCoord = COORD{ 0 };
	ReadConsoleOutput(hConsoleOutput, originalBuffer.data(), originalCSBI.dwSize, originalBufferCoord, &originalCSBI.srWindow);

	// Save the cursor
	GetConsoleCursorInfo(hConsoleOutput, &originalCCI);
	//-----------------------------

	//resize window ---------------
	{
		SMALL_RECT sr{ 0 };
		SetConsoleWindowInfo(hConsoleOutput, TRUE, &sr);
		INPUT_SECTION_START = OUTPUT_SECTION_START + OUTPUT_SECTION_SIZE;
		WORD WINDOW_HEIGHT = INPUT_SECTION_START + INPUT_SECTION_SIZE;
		COORD bufferSize{ SHORT(WINDOW_WIDTH), SHORT(WINDOW_HEIGHT) };
		SetConsoleScreenBufferSize(hConsoleOutput, bufferSize);


		GetConsoleScreenBufferInfo(hConsoleOutput, &sbi);

		sr.Top = sr.Left = 0;
		WINDOW_WIDTH = std::min((SHORT)WINDOW_WIDTH, sbi.dwMaximumWindowSize.X);
		WINDOW_HEIGHT = std::min((SHORT)WINDOW_HEIGHT, sbi.dwMaximumWindowSize.Y);
		sr.Right = WINDOW_WIDTH - 1;
		sr.Bottom = WINDOW_HEIGHT - 1;
		SetConsoleWindowInfo(hConsoleOutput, TRUE, &sr);
		currentConsoleWidth = sr.Right - sr.Left + 1;
	}
	//----------------------------

	HWND consoleWindow = GetConsoleWindow();
	SetWindowLong(consoleWindow, GWL_STYLE, GetWindowLong(consoleWindow, GWL_STYLE) & ~WS_MAXIMIZEBOX & ~WS_SIZEBOX);

	//hide cursor ----------------
	{
		auto newCCI = originalCCI;
		newCCI.bVisible = FALSE;
		SetConsoleCursorInfo(hConsoleOutput, &newCCI);
	}
	//---------------------------

	//paint screen --------------
	{
		// Get the number of character cells in the current buffer
		CONSOLE_SCREEN_BUFFER_INFO csbi;
		GetConsoleScreenBufferInfo(hConsoleOutput, &csbi);

		// Fill the entire screen area white
		DWORD charsWritten;
		DWORD consoleSize = csbi.dwSize.X * csbi.dwSize.Y;
		COORD cursorHomeCoord{ 0, 0 };
		FillConsoleOutputCharacterA(hConsoleOutput, ' ', consoleSize, cursorHomeCoord, &charsWritten);
		FillConsoleOutputAttribute(hConsoleOutput, OUTPUT_SECTION_ATTR, consoleSize, cursorHomeCoord, &charsWritten);

		// Fill the title section, at the top
		consoleSize = csbi.dwSize.X * OUTPUT_SECTION_SIZE;
		cursorHomeCoord = COORD{ 0, 0 };
		FillConsoleOutputCharacterA(hConsoleOutput, ' ', consoleSize, cursorHomeCoord, &charsWritten);
		FillConsoleOutputAttribute(hConsoleOutput, OUTPUT_SECTION_ATTR, consoleSize, cursorHomeCoord, &charsWritten);

		// Fill the Edit section after the mouse section.
		consoleSize = csbi.dwSize.X * INPUT_SECTION_SIZE;
		cursorHomeCoord = COORD{ 0, (SHORT)INPUT_SECTION_START };
		FillConsoleOutputCharacterA(hConsoleOutput, ' ', consoleSize, cursorHomeCoord, &charsWritten);
		FillConsoleOutputAttribute(hConsoleOutput, INPUT_SECTION_ATTR, consoleSize, cursorHomeCoord, &charsWritten);

		// Put the cursor at its home coordinates
		SetConsoleCursorPosition(hConsoleOutput, COORD{ 0, 0 });
	}
	CONSOLE_CURSOR_INFO cci{ 10, TRUE };
	SetConsoleCursorInfo(hConsoleOutput, &cci);
	editControlCursorPos = std::min(0 + editControlAperture, editControlString.size());
	COORD loc{ SHORT(editControlCursorPos - editControlAperture + 0), INPUT_SECTION_START };
	SetConsoleCursorPosition(hConsoleOutput, loc);

	SetConsoleCtrlHandler((PHANDLER_ROUTINE)CtrlHandler, TRUE);
}


void restoreWindow() {
	SetConsoleMode(hConsoleInput, originalConsoleMode);
	// Restore the original settings/size
	SMALL_RECT sr{ 0 };
	SetConsoleWindowInfo(hConsoleOutput, TRUE, &sr);
	SetConsoleScreenBufferSize(hConsoleOutput, originalCSBI.dwSize);
	SetConsoleWindowInfo(hConsoleOutput, TRUE, &originalCSBI.srWindow);


	// Restore the desktop contents
	WriteConsoleOutputA(hConsoleOutput, originalBuffer.data(), originalCSBI.dwSize, originalBufferCoord, &originalCSBI.srWindow);
	SetConsoleTitleA(originalTitle.data());

	// Restore the cursor
	SetConsoleCursorInfo(hConsoleOutput, &originalCCI);
	SetConsoleCursorPosition(hConsoleOutput, originalCSBI.dwCursorPosition);
}
