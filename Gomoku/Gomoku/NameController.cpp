#include "stdafx.h"
#include "NameController.h"
#include "GameModel.h"

//controller for settign up the name display view
//takes the model pointer as an argment as well as the start y value for drawing the view
NameController::NameController(GameModel *model ) :IViewObserver(model)
{
	NameView.PLAYERS_START = 0;
	NameView.PLAYERS_SIZE = 5;
	NameView.PLAYERS_ATTR = 128;
	NameView.PLAYERS_NAME_LENGTH = 30;
	NameView.PLAYER1_NAME_ATTR = 9;
	NameView.PLAYER2_NAME_ATTR = 12;
	NameView.PLAYER1_COLOR_ATTR = 25;
	NameView.PLAYER2_COLOR_ATTR = 76;

}

//draws the name view
void NameController::InitPlayerSection() {
	// Fill the player section, at the top
	DWORD consoleSize = getModel()->csbi.dwSize.X * NameView.PLAYERS_SIZE;
	COORD cursorHomeCoord = COORD{ 0, 0 };
	FillConsoleOutputCharacterA(getModel()->hConsoleOutput, ' ', consoleSize, cursorHomeCoord, &getModel()->charsWritten);
	FillConsoleOutputAttribute(getModel()->hConsoleOutput, NameView.PLAYERS_ATTR, consoleSize, cursorHomeCoord, &getModel()->charsWritten);

	//player input fields
	consoleSize = NameView.PLAYERS_NAME_LENGTH;
	cursorHomeCoord = COORD{ 2, 1 };
	FillConsoleOutputAttribute(getModel()->hConsoleOutput, NameView.PLAYER1_NAME_ATTR, consoleSize, cursorHomeCoord, &getModel()->charsWritten);
	cursorHomeCoord = COORD{ 2, 3 };
	FillConsoleOutputAttribute(getModel()->hConsoleOutput, NameView.PLAYER2_NAME_ATTR, consoleSize, cursorHomeCoord, &getModel()->charsWritten);

	//fill player names


}
//fills the name view with the player names in the player color
void NameController::PaintPlayerColor() {
	//fill player color
	getModel()->OutputString((SHORT)2, (SHORT)1, getModel()->players.at(1).name, getModel()->players.at(1).colorForeground);
	getModel()->OutputString((SHORT)2, (SHORT)3, getModel()->players.at(2).name, getModel()->players.at(2).colorForeground);
	DWORD charsWritten;
	COORD cursorHomeCoord = COORD{ 2 + NameView.PLAYERS_NAME_LENGTH + 2, 1 };
	DWORD consoleSize = 2;
	FillConsoleOutputCharacterA(getModel()->hConsoleOutput, ' ', consoleSize, cursorHomeCoord, &getModel()->charsWritten);
	FillConsoleOutputAttribute(getModel()->hConsoleOutput, getModel()->players.at(1).colorBackground, consoleSize, cursorHomeCoord, &getModel()->charsWritten);

	cursorHomeCoord = COORD{ 2 + NameView.PLAYERS_NAME_LENGTH + 2, 3 };
	consoleSize = 2;
	FillConsoleOutputCharacterA(getModel()->hConsoleOutput, ' ', consoleSize, cursorHomeCoord, &getModel()->charsWritten);
	FillConsoleOutputAttribute(getModel()->hConsoleOutput, getModel()->players.at(2).colorBackground, consoleSize, cursorHomeCoord, &getModel()->charsWritten);

}
//redraws the names
void NameController::Redraw(int view) {
	if (view == 0 || view == 3) {
		PaintPlayerColor();
	}
}

NameController::~NameController()
{
}
