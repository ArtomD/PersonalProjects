#pragma once

#include <vector>
#include <Windows.h>
#include "IViewObserver.h"
#include <string>
using namespace std;



class GameModel
{
public:
	vector < IViewObserver* > views;

	//global console vars
	CONSOLE_SCREEN_BUFFER_INFO csbi;
	HANDLE hConsoleInput, hConsoleOutput;
	DWORD charsWritten;
	static WORD					currentConsoleWidth;


	struct player {
		string name;
		short colorForeground;
		short colorBackground;
		int moves = 0;
		int score = 0;
		int gamesWon = 0;
	};
	bool gameStarted = false;
	int gameWinner = 0;
	int totalMoves = 0;
	vector<int> gameboard = vector<int>(225,0);
	vector<player> players;
	unsigned currentPlayerIndex;

	GameModel();
	void processGameState();
	void attach(IViewObserver*obs);
	void refeshViews(int view);
	void OutputString(WORD x, WORD y, string const& s, WORD attributes);
	~GameModel();
};

