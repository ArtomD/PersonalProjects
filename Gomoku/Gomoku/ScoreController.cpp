#include "stdafx.h"
#include "ScoreController.h"
#include "GameModel.h"
#include <string>

//controller for settign up the score display view
//takes the model pointer as an argment as well as the start y value for drawing the view
ScoreController::ScoreController(GameModel *model,int start): IViewObserver(model)
{
	ScoreView.SCORE_START = start;
	ScoreView.SCORE_SIZE = 6;
	ScoreView.SCORE_ATTR = 15;
}

//draw the score view
void ScoreController::InitScore() {

	// Fill the score section, next to top
	DWORD consoleSize = getModel()->csbi.dwSize.X * ScoreView.SCORE_SIZE;
	COORD cursorHomeCoord = COORD{ (SHORT)0,(SHORT)ScoreView.SCORE_START };
	FillConsoleOutputCharacterA(getModel()->hConsoleOutput, ' ', consoleSize, cursorHomeCoord, &getModel()->charsWritten);
	FillConsoleOutputAttribute(getModel()->hConsoleOutput, ScoreView.SCORE_ATTR, consoleSize, cursorHomeCoord, &getModel()->charsWritten);

}
//goes over the player vector and draws their score, moves made and games won
void ScoreController::DisplayScore() {

	InitScore();

	for (unsigned i = 1; i <  getModel()->players.size();i++) {
		string name = getModel()->players.at(i).name.substr(0,13);
		getModel()->OutputString((SHORT)2, ScoreView.SCORE_START + 1 + ((i - 1) * 2), name, getModel()->players.at(i).colorForeground);
		string piecesPlayed = "Pieces Played: " + to_string(getModel()->players.at(i).moves);
		string score = "Score: " + to_string(getModel()->players.at(i).score);
		string gamesWon = "| Games Won: " + to_string(getModel()->players.at(i).gamesWon);
		int nameSpace = 12;
		int scoreSpace = score.length() + 1;
		getModel()->OutputString((SHORT)(2 + nameSpace), (SHORT)(ScoreView.SCORE_START + 1 + ((i-1)*2)), piecesPlayed, ScoreView.SCORE_ATTR);
		getModel()->OutputString((SHORT)(2 + nameSpace), (SHORT)(ScoreView.SCORE_START + 2 + ((i - 1) * 2)), score, ScoreView.SCORE_ATTR);
		getModel()->OutputString((SHORT)(2 + nameSpace + scoreSpace), (SHORT)(ScoreView.SCORE_START + 2 + ((i - 1) * 2)), gamesWon, ScoreView.SCORE_ATTR);
	}

}
//repaints the score text
void ScoreController::Redraw(int view) {
	if (view == 0 || view == 4) {
		DisplayScore();
	}
}



ScoreController::~ScoreController()
{
}
