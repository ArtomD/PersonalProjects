#include "stdafx.h"
#include <Windows.h>
#include "MovesController.h"
#include "GameModel.h"

//controller for settign up the move text view
//takes the model pointer as an argment as well as the start y value for drawing the view
MovesController::MovesController(GameModel *model, int start) : IViewObserver(model)
{
	MovesView.MOVES_START = start;
	MovesView.MOVES_ATTR = 15;
	MovesView.MOVES_SIZE = 6;
}

//draws the moves view and adds the welcome string to the text vector
void MovesController::InitMoves() {
	DWORD consoleSize = getModel()->csbi.dwSize.X * MovesView.MOVES_SIZE;
	COORD cursorHomeCoord = COORD{ (SHORT)0, (SHORT)MovesView.MOVES_START };
	FillConsoleOutputCharacterA(getModel()->hConsoleOutput, ' ', consoleSize, cursorHomeCoord, &getModel()->charsWritten);
	FillConsoleOutputAttribute(getModel()->hConsoleOutput, MovesView.MOVES_ATTR, consoleSize, cursorHomeCoord, &getModel()->charsWritten);

	AddText("Click on the board to start the game", 0,0);

}
//adds a string to the text vector to disply in the the view. color int is used to save the index of the player who saved the message
//the moveNum int is used to record what move has been made, 0 for not move to display
void MovesController::AddText(string text, int color, int moveNum) {
	if (gameTextList.size() > maxListSize) {
		gameTextList.erase(gameTextList.begin());
		gameTextColorList.erase(gameTextColorList.begin());
		gameTextMoveNum.erase(gameTextMoveNum.begin());
	}
	gameTextList.push_back(text);
	gameTextColorList.push_back(color);
	gameTextMoveNum.push_back(moveNum);
	gameTextIndex = gameTextList.size()-1;
	
	DisplayGameText();
}
//updates the move view with the text in the text vector
//displays only the text that can fit in the view based on the gameTextIndex var
void MovesController::DisplayGameText() {
	CONSOLE_SCREEN_BUFFER_INFO csbi;
	GetConsoleScreenBufferInfo(getModel()->hConsoleOutput, &csbi);
	DWORD charsWritten;
	DWORD consoleSize = csbi.dwSize.X * MovesView.MOVES_SIZE;
	COORD cursorHomeCoord = COORD{ (SHORT)0, (SHORT)MovesView.MOVES_START };
	FillConsoleOutputCharacterA(getModel()->hConsoleOutput, ' ', consoleSize, cursorHomeCoord, &getModel()->charsWritten);
	FillConsoleOutputAttribute(getModel()->hConsoleOutput, MovesView.MOVES_ATTR, consoleSize, cursorHomeCoord, &getModel()->charsWritten);
	for (int i = MovesView.MOVES_SIZE - 1; i >= 0; i--) {
		if ((gameTextIndex - i) >= 0) {
			int temp = (gameTextIndex - i);
			bool tmp = (gameTextIndex - i) >= 0;
			getModel()->OutputString((SHORT)1, (SHORT)(MovesView.MOVES_START) + (MovesView.MOVES_SIZE - 1 - i), gameTextList.at(gameTextIndex - i), getModel()->players.at(gameTextColorList.at(gameTextIndex - i)).colorForeground);
			if (gameTextMoveNum.at(gameTextIndex - i) != 0) {
				getModel()->OutputString((SHORT)(1+4), (SHORT)(MovesView.MOVES_START)+(MovesView.MOVES_SIZE - 1 - i), "Move "+to_string(gameTextMoveNum.at(gameTextIndex - i)), getModel()->players.at(0).colorForeground);
			}
		}
	}
}

//moves the gameTextIndex to display different parts of the text vector
void MovesController::ScrollLine(int num,bool up) {
	if (up) {
		int newIndex = gameTextIndex - num;
		if (newIndex < 0)
			newIndex = 0;
		gameTextIndex = newIndex;
	}
	else {
		int newIndex = gameTextIndex + num;
		if (newIndex >= gameTextList.size())
			newIndex = gameTextList.size()-1;
		gameTextIndex = newIndex;
	}		
	DisplayGameText();
}
//redraws the moves view
void MovesController::Redraw(int view) {
	if (view == 0 || view == 2) {
		DisplayGameText();
	}
}

MovesController::~MovesController()
{
}
