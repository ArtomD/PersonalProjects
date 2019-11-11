#include "stdafx.h"
#include <Windows.h>
#include "GameModel.h"

WORD GameModel::currentConsoleWidth = 0;
GameModel::GameModel()
{
}
//attaches the views to the can be updated
void GameModel::attach(IViewObserver*obs) {
	views.push_back(obs);
}
//checks the gameboard for runs and records the highest one found to the player vector
//checks if a score of 5 or greater has been reached and change the gameWinner variable to the player who scores it
void GameModel::processGameState()
{
	for (unsigned i = 0; i < gameboard.size(); i++) {
		if (gameboard.at(i) != 0) {
			int player = gameboard.at(i);
			int range = 1;
			int score = 1;
			while (i + (range) < gameboard.size() && ( i + (range))%15 != 0) {
				if (gameboard.at(i + range) == player) {
					score++;
					range++;
				}
				else {
					break;
				}
			}
			if (players.at(player).score<score) {
				players.at(player).score = score;
			}
			range = 1;
			score = 1;
			while(i + (range * 15)<gameboard.size() ){
				if (gameboard.at(i + (range * 15)) == player) {
					score++;
					range++;
				}
				else {
					break;
				}
			}
			if (players.at(player).score<score) {
				players.at(player).score = score;
			}
			range = 1;
			score = 1;
			while (i + (range * 16) < gameboard.size() && (i + (range * 16)) % 15 != 0) {
				if (gameboard.at(i + (range * 16)) == player) {
					score++;
					range++;
				}
				else {
					break;
				}
			}
			if (players.at(player).score<score) {
				players.at(player).score = score;
			}
			range = 1;
			score = 1;
			while (i + (range * 14) < gameboard.size() && (i + (range * 14)) % 15 != 14) {
				if (gameboard.at(i + (range * 14)) == player) {
					score++;
					range++;
				}
				else {
					break;
				}
			}
			if (players.at(player).score<score) {
				players.at(player).score = score;
			}
		}
	}
	for (int i = 1; i <= 2; i++) {
		if (players.at(i).score >= 5) {
			players.at(i).gamesWon++;
			gameWinner = i;
		}
	}	
}

//utility function to print a string to the console at the supplied x and y coords with the supplied attributes
void GameModel::OutputString(WORD x, WORD y, std::string const& s, WORD attributes) {
	vector<WORD> attr(s.size(), attributes);
	COORD loc{ (SHORT)x, (SHORT)y };
	DWORD nCharsWritten;
	DWORD nToWrite = DWORD(min(s.size(), std::size_t(currentConsoleWidth - x)));

	WriteConsoleOutputCharacterA(hConsoleOutput, s.c_str(), nToWrite, loc, &nCharsWritten);
	WriteConsoleOutputAttribute(hConsoleOutput, attr.data(), nToWrite, loc, &nCharsWritten);
}

//sends a call to all registered views to update based on what int is put in
//an int of 0 refreshes all views
void GameModel::refeshViews(int view) {
	for (int i = 0; i < views.size(); i++) {
		views[i]->Redraw(view);
	}
}


GameModel::~GameModel()
{
}
