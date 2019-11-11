#include "stdafx.h"
#include "GameController.h"
#include "GameModel.h"
#include <string>

using namespace std;

//controller for settign up the game board
//takes the model pointer as an argment as well as the start y value for drawing the board
GameController::GameController(GameModel *model, int start) : IViewObserver(model) 
{

	BoardView.GAME_BLACK_ATTR = 0;
	BoardView.GAME_WHITE_ATTR = 8 * 16;
	BoardView.GAME_BORDER_SIZE = 2;
	BoardView.GAME_BORDER_ATTR = 2 * 16 + 7;
	BoardView.GAME_START = start;
	BoardView.GAME_SIZE = (BoardView.GAME_BORDER_SIZE * 2) + 15;
}

//draws the board squares and border
void GameController::InitGameBoard() {
	DWORD consoleSize = model->csbi.dwSize.X *BoardView.GAME_BORDER_SIZE;
	COORD cursorHomeCoord = COORD{ (SHORT)0,(SHORT)BoardView.GAME_START };
	FillConsoleOutputCharacterA(model->hConsoleOutput, ' ', consoleSize, cursorHomeCoord, &model->charsWritten);
	FillConsoleOutputAttribute(model->hConsoleOutput, BoardView.GAME_BORDER_ATTR, consoleSize, cursorHomeCoord, &model->charsWritten);
	//fill the game board
	for (SHORT i = 0; i < 15; i++) {
		int c = 0;
		// left game border
		cursorHomeCoord = COORD{ 0, BoardView.GAME_START + BoardView.GAME_BORDER_SIZE + i };
		FillConsoleOutputCharacterA(model->hConsoleOutput, ' ', 4, cursorHomeCoord, &model->charsWritten);
		FillConsoleOutputAttribute(model->hConsoleOutput, BoardView.GAME_BORDER_ATTR, 4, cursorHomeCoord, &model->charsWritten);
		//game board
		for (SHORT s = 4; s < 34; s += 2) {
			cursorHomeCoord = COORD{ s, BoardView.GAME_START + BoardView.GAME_BORDER_SIZE + i };
			FillConsoleOutputCharacterA(model->hConsoleOutput, ' ', 2, cursorHomeCoord, &model->charsWritten);
			if (i % 2 == 0) {
				if (c % 2 == 0)
					FillConsoleOutputAttribute(model->hConsoleOutput, BoardView.GAME_BLACK_ATTR, 2, cursorHomeCoord, &model->charsWritten);
				else
					FillConsoleOutputAttribute(model->hConsoleOutput, BoardView.GAME_WHITE_ATTR, 2, cursorHomeCoord, &model->charsWritten);
			}
			else {
				if (c % 2 != 0)
					FillConsoleOutputAttribute(model->hConsoleOutput, BoardView.GAME_BLACK_ATTR, 2, cursorHomeCoord, &model->charsWritten);
				else
					FillConsoleOutputAttribute(model->hConsoleOutput, BoardView.GAME_WHITE_ATTR, 2, cursorHomeCoord, &model->charsWritten);
			}
			c++;
		}
		//right game border
		cursorHomeCoord = COORD{ 34, BoardView.GAME_START + BoardView.GAME_BORDER_SIZE + i };
		FillConsoleOutputCharacterA(model->hConsoleOutput, ' ', 4, cursorHomeCoord, &model->charsWritten);
		FillConsoleOutputAttribute(model->hConsoleOutput, BoardView.GAME_BORDER_ATTR, 4, cursorHomeCoord, &model->charsWritten);
	}
	//bottom game border
	consoleSize = model->csbi.dwSize.X *BoardView.GAME_BORDER_SIZE;
	cursorHomeCoord = COORD{ 0,BoardView.GAME_START + 17 };
	FillConsoleOutputCharacterA(model->hConsoleOutput, ' ', consoleSize, cursorHomeCoord, & model->charsWritten);
	FillConsoleOutputAttribute(model->hConsoleOutput, BoardView.GAME_BORDER_ATTR, consoleSize, cursorHomeCoord, & model->charsWritten);
}
//overloaded function
void GameController::PrintGameMarker(int x, int y) {
	PrintGameMarker(CoordsToIndex(x, y));
}
//prints a marker on the game board from the index of the game board vector
void GameController::PrintGameMarker(int pos) {
	string marker;
	if ( model->gameboard.at(pos) != 0) {
		marker = "><";
	}
	else {
		marker = "  ";
	}
	int x = pos % 15;
	int y = pos / 15;
	x = (x * 2) + 4;
	y = y + BoardView.GAME_START + BoardView.GAME_BORDER_SIZE;
	WORD background = pos % 2 ? BoardView.GAME_WHITE_ATTR : BoardView.GAME_BLACK_ATTR;
	 model->OutputString((SHORT)x, (SHORT)y, marker,  model->players.at( model->gameboard.at(pos)).colorForeground | background);

}
//converts the x and y coords to an index in the game board vector
int GameController::CoordsToIndex(int x, int y) {
	x = ((x - 4) / 2);
	y = (y - BoardView.GAME_START - BoardView.GAME_BORDER_SIZE);
	return x + (y * 15);
}
//converts the x and y coords to a coresponding letter and number on the game grid
string GameController::CoordsToString(int x, int y) {
	int pos = CoordsToIndex(x, y);
	int X = pos % 15;
	int Y = pos / 15;
	string result;
	switch (X) {
		case 0: result = "A" + to_string(Y);
			break;
		case 1: result = "B" + to_string(Y);
			break;
		case 2: result = "C" + to_string(Y);
			break;
		case 3: result = "D" + to_string(Y);
			break;
		case 4: result = "E" + to_string(Y);
			break;
		case 5: result = "F" + to_string(Y);
			break;
		case 6: result = "G" + to_string(Y);
			break;
		case 7: result = "H" + to_string(Y);
			break;
		case 8: result = "I" + to_string(Y);
			break;
		case 9: result = "J" + to_string(Y);
			break;
		case 10: result = "K" + to_string(Y);
			break;
		case 11: result = "L" + to_string(Y);
			break;
		case 12: result = "M" + to_string(Y);
			break;
		case 13: result = "N" + to_string(Y);
			break;
		case 14: result = "O" + to_string(Y);
			break;
	}
	return result;
}
//tries to take the square at the given x and y coords.
//returns if the square has not yet been taken
bool GameController::TakeSquare(int x, int y) {
	int pos = CoordsToIndex(x, y);
	int space =  model->gameboard.at(pos);

	if (space == 0) {
		return true;		
	}
	return false;
}
//checks if the given x and y coords are inside the game board(not counting game board)
bool GameController::IsInsideBoard(int x, int y) {
	return y >= BoardView.GAME_START + BoardView.GAME_BORDER_SIZE && y <= BoardView.GAME_START + BoardView.GAME_SIZE - BoardView.GAME_BORDER_SIZE - 1 && x >= BoardView.GAME_BORDER_SIZE * 2 && x < (BoardView.GAME_SIZE - BoardView.GAME_BORDER_SIZE) * 2;

}
//redraws the markers for the entire board
void GameController::Redraw(int view) {
	if (view == 0 || 1) {
		for (int i = 0; i < model->gameboard.size(); i++) {
			PrintGameMarker(i);
		}
	}
}

GameController::~GameController()
{
}
