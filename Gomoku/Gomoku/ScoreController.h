#pragma once
#include <Windows.h>
#include "IViewObserver.h"
#include "GameModel.h"

using namespace std;

class ScoreController : IViewObserver
{
public:
	//console view
	struct view {
		WORD SCORE_START;
		WORD  SCORE_SIZE;
		WORD  SCORE_ATTR;
	};
	view ScoreView;
	//constructors
	ScoreController(GameModel *model, int start);
	//public functions
	void InitScore();
	void DisplayScore();
	void Redraw(int view);
	~ScoreController();
};

