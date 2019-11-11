#pragma once
#include <Windows.h>
#include <string>
#include "IViewObserver.h"
#include "GameModel.h"

class GameController : public IViewObserver

{
public:
	//console view
	struct view {
		short GAME_BORDER_SIZE;
		short GAME_BORDER_ATTR;
		short GAME_START;
		short GAME_SIZE;
		short GAME_BLACK_ATTR;
		short GAME_WHITE_ATTR;
	};	
	view BoardView;
	//constructors
	GameController(GameModel *model, int start);

	//public functions
	void InitGameBoard();
	void PrintGameMarker(int x, int y);
	void PrintGameMarker(int pos);
	std::string CoordsToString(int x, int y);
	int CoordsToIndex(int x, int y);
	bool TakeSquare(int x, int y);
	bool IsInsideBoard(int x, int y);
	void Redraw(int view);
	~GameController();

};

