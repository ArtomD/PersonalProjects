#pragma once
#include <Windows.h>
#include <vector>
#include <string>
#include "IViewObserver.h"
#include "GameModel.h"

using namespace std;

class MovesController : public IViewObserver
{
public:

	//console view
	struct view {
		WORD MOVES_START;
		WORD  MOVES_SIZE = 6;
		WORD  MOVES_ATTR = 0 | 7 | FOREGROUND_INTENSITY;
	};
	view MovesView;

	//constructors
	MovesController(GameModel *model, int start);

	//public functions
	void InitMoves();
	void AddText(string text, int color, int moveNum);
	void DisplayGameText();
	void ScrollLine(int line, bool up);
	void Redraw(int view);
	~MovesController();

private:
	//game text vars
	vector<string> gameTextList;
	vector<int> gameTextColorList;
	vector<int> gameTextMoveNum;
	int gameTextIndex = 0;
	int maxListSize = 1000;
	//main controller ref
	GameModel *model;
};

