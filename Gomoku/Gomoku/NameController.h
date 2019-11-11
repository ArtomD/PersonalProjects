#pragma once
#include <Windows.h>
#include <string>
#include "IViewObserver.h"
#include "GameModel.h"

using namespace std;

class NameController : IViewObserver
{
public:
	//console view
	struct view {
		WORD PLAYERS_START;
		WORD PLAYERS_SIZE;
		WORD PLAYERS_ATTR;
		WORD PLAYERS_NAME_LENGTH;
		WORD PLAYER1_NAME_ATTR;
		WORD PLAYER2_NAME_ATTR;
		WORD PLAYER1_COLOR_ATTR;
		WORD PLAYER2_COLOR_ATTR;
	};
	view NameView;
	

	//playerNameData
	string player1StringName = "Player1";
	string player2StringName = "Player2";	
	//constructors
	NameController(GameModel *model);
	//public functions
	void InitPlayerSection();
	void PaintPlayerColor();
	void Redraw(int view);
	~NameController();
};

