#pragma once
#include <Windows.h>
#include "GameController.h"
#include "NameController.h"
#include "ScoreController.h"
#include "GameModel.h"
#include "MovesController.h"

using namespace std;

class MainController 
{
public:

	

	//constructors
	MainController(GameModel *mod);

	//public functions
	void Start();
	~MainController();

private:
	//console vars
	vector<char> originalTitle;
	CONSOLE_SCREEN_BUFFER_INFO	originalCSBI;
	CONSOLE_CURSOR_INFO			originalCCI;
	vector<CHAR_INFO>			originalBuffer;
	COORD						originalBufferCoord;
	DWORD						originalConsoleMode;

	enum views {All, Game, Moves, Name, Score};

	//window size
	WORD WINDOW_WIDTH = 38;
	WORD WINDOW_HEIGHT = 36;

	//model
	GameModel *model;

	//view controllers
	GameController *gameboardController;
	MovesController *movesController;
	NameController *nameController;
	ScoreController *scoreController;

	//text input data
	bool player1Namefocus = false;
	bool player2Namefocus = false;
	string::size_type player1CursorPos = 0;
	decltype(player1CursorPos) player1Aperture = 0;
	string::size_type player2CursorPos = 0;
	decltype(player2CursorPos) player2Aperture = 0;

	// Application data
	static bool applicationQuitting;
	static DWORD terminationEventIdx;

	//private functions
	void InitMainView();
	void InitControllerProperties();
	static BOOL CtrlHandler(DWORD ctrlType);
	void RestoreOldWindow();
	void Repaint();
	void ResetGame();
	void ProcessKeyEvent(KEY_EVENT_RECORD const& ke);
	void ProcessMouseEvent(MOUSE_EVENT_RECORD const& me);
	void CyclePlayerColor(int index, int other);
	void ShowConsoleCursor(bool flag);
	GameModel* GetModel();

};

