#include "stdafx.h"
#include "MainController.h"

#include <Windows.h>
#undef min

#include <algorithm>
#include <iomanip>
#include <sstream>
#include <string>
#include <vector>
#include <iostream>



using namespace std;

// Colours


//LAYOUT
//BLANK
WORD MAIN_SECTION_ATTR = 112;
//ADMIN COLOR
WORD ADMIN_ATTR = 15;

//static var init
bool MainController::applicationQuitting = false;
DWORD MainController::terminationEventIdx = -1;

//Entry point for the aplication.
//takes a pointer to a model object
MainController::MainController(GameModel *mod)
{
	model = mod;
	InitMainView();
	InitControllerProperties();
}

GameModel* MainController::GetModel() {
	return model;
}
//stores the initial console settings and sets up game settings
void MainController::InitMainView() {
	
	//title info
	originalTitle = vector<char>(64 * 1024);
	originalTitle.resize(GetConsoleTitleA(originalTitle.data(), (DWORD)originalTitle.size()) + 1);
	originalTitle.shrink_to_fit();
	SetConsoleTitleA("GOMOKU");


	//get STD HANDLE
	model->hConsoleInput = GetStdHandle(STD_INPUT_HANDLE);
	model->hConsoleOutput = GetStdHandle(STD_OUTPUT_HANDLE);

	//save console state
	//get buffer for console put into originalCSBI
	GetConsoleScreenBufferInfo(model->hConsoleOutput, &originalCSBI);
	//resize the console buffer using originalCSBI
	originalBuffer.resize(originalCSBI.dwSize.X*originalCSBI.dwSize.Y);
	originalBufferCoord = COORD{ 0 };
	//gets the window size
	ReadConsoleOutput(model->hConsoleOutput, originalBuffer.data(), originalCSBI.dwSize, originalBufferCoord, &originalCSBI.srWindow);

	//save cursor info
	GetConsoleCursorInfo(model->hConsoleOutput, &originalCCI);

	//resize the window
	//window size rectangle
	SMALL_RECT sr{ 0 };
	//uses buffer to make window and stores coords in sr
	SetConsoleWindowInfo(model->hConsoleOutput, TRUE, &sr);

	//sets buffer for window
	COORD bufferSize{ SHORT(WINDOW_WIDTH), SHORT(WINDOW_HEIGHT) };
	SetConsoleScreenBufferSize(model->hConsoleOutput, bufferSize);

	//sets buffer info into sbi
	CONSOLE_SCREEN_BUFFER_INFO sbi;
	GetConsoleScreenBufferInfo(model->hConsoleOutput, &sbi);

	sr.Top = sr.Left = 0;
	WINDOW_WIDTH = std::min((SHORT)WINDOW_WIDTH, sbi.dwMaximumWindowSize.X);
	WINDOW_HEIGHT = std::min((SHORT)WINDOW_HEIGHT, sbi.dwMaximumWindowSize.Y);
	sr.Right = WINDOW_WIDTH - 1;
	sr.Bottom = WINDOW_HEIGHT - 1;
	SetConsoleWindowInfo(model->hConsoleOutput, TRUE, &sr);
	model->currentConsoleWidth = sr.Right - sr.Left + 1;

	// Get the number of character cells in the current buffer	
	GetConsoleScreenBufferInfo(model->hConsoleOutput, &model->csbi);
	// Fill the entire screen area white

	DWORD consoleSize = model->csbi.dwSize.X * model->csbi.dwSize.Y;
	COORD cursorHomeCoord{ 0, 0 };
	FillConsoleOutputCharacterA(model->hConsoleOutput, ' ', consoleSize, cursorHomeCoord, &model->charsWritten);
	FillConsoleOutputAttribute(model->hConsoleOutput, MAIN_SECTION_ATTR, consoleSize, cursorHomeCoord, &model->charsWritten);
}

//initsthe controllers that control the seperate views
void MainController::InitControllerProperties() {
	nameController = new NameController(model);
	nameController->InitPlayerSection();

	//sets the default player settings
	GameModel::player player;
	player.name = "admin";
	player.colorForeground = ADMIN_ATTR;
	player.colorBackground = ADMIN_ATTR;
	model->players.push_back(player);
	player.name = nameController->player1StringName;
	player.colorForeground = nameController->NameView.PLAYER1_NAME_ATTR;
	player.colorBackground = nameController->NameView.PLAYER1_COLOR_ATTR;
	model->players.push_back(player);
	player.name = nameController->player2StringName;
	player.colorForeground = nameController->NameView.PLAYER2_NAME_ATTR;
	player.colorBackground = nameController->NameView.PLAYER2_COLOR_ATTR;
	model->players.push_back(player);
	//paints the name section
	model->refeshViews(Name);
	//creates the controllers and paints the inital views
	scoreController = new ScoreController(model, nameController->NameView.PLAYERS_START + nameController->NameView.PLAYERS_SIZE);
	scoreController->InitScore();
	model->refeshViews(Score);
	gameboardController = new GameController(model, scoreController->ScoreView.SCORE_START + scoreController->ScoreView.SCORE_SIZE);
	gameboardController->InitGameBoard();
	movesController = new MovesController(model, gameboardController->BoardView.GAME_START + gameboardController->BoardView.GAME_SIZE);
	movesController->InitMoves();
	model->refeshViews(Moves);
}


//refreshes all the views
void MainController::Repaint() {
	model->refeshViews(All);
}

//handles the exit from the game
BOOL MainController::CtrlHandler(DWORD ctrlType) {
	if (ctrlType <= CTRL_CLOSE_EVENT) {
		terminationEventIdx = ctrlType;
		applicationQuitting = true;
		return TRUE;
	}

	return FALSE;
}

//sets the event handlers and starts the main game loop
void MainController::Start() {
	try {
		SetConsoleCtrlHandler((PHANDLER_ROUTINE)CtrlHandler, TRUE);
		GetConsoleMode(model->hConsoleInput, &originalConsoleMode);
		SetConsoleMode(model->hConsoleInput, ENABLE_WINDOW_INPUT | ENABLE_PROCESSED_INPUT | ENABLE_MOUSE_INPUT | ENABLE_EXTENDED_FLAGS);

		ShowConsoleCursor(false);

		vector<INPUT_RECORD> events(128);
		while (!MainController::applicationQuitting) {
			DWORD numEvents;
			ReadConsoleInput(model->hConsoleInput, events.data(), (DWORD)events.size(), &numEvents);
			for (decltype(numEvents) i = 0; i < numEvents; ++i) {
				auto& e = events[i];
				switch (e.EventType) {
				case KEY_EVENT:		ProcessKeyEvent(e.Event.KeyEvent); break;
				case MOUSE_EVENT:	ProcessMouseEvent(e.Event.MouseEvent); break;
				}
			}
		}

		RestoreOldWindow();
	}
	catch (const exception e) {
		//if there is an error try to restore the window before quiting
		RestoreOldWindow();
	}
}
//uses the stored settings to revert to the old console settings
void MainController::RestoreOldWindow() {
	SetConsoleMode(model->hConsoleInput, originalConsoleMode);

	// Restore the original settings/size
	SMALL_RECT srold{ 0 };
	SetConsoleWindowInfo(model->hConsoleOutput, TRUE, &srold);
	SetConsoleScreenBufferSize(model->hConsoleOutput, originalCSBI.dwSize);
	SetConsoleWindowInfo(model->hConsoleOutput, TRUE, &originalCSBI.srWindow);


	// Restore the desktop contents
	WriteConsoleOutputA(model->hConsoleOutput, originalBuffer.data(), originalCSBI.dwSize, originalBufferCoord, &originalCSBI.srWindow);
	SetConsoleTitleA(originalTitle.data());

	// Restore the cursor
	SetConsoleCursorInfo(model->hConsoleOutput, &originalCCI);
	SetConsoleCursorPosition(model->hConsoleOutput, originalCSBI.dwCursorPosition);
}
//resets the game model to a new game state
void MainController::ResetGame() {
	model->gameWinner = 0;
	model->players.at(1).score = 0;
	model->players.at(1).moves = 0;
	model->players.at(2).score = 0;
	model->players.at(2).moves = 0;
	model->gameboard = vector<int>(225, 0);
	model->totalMoves = 0;
	Repaint();
}
//process keyboard input
void MainController::ProcessKeyEvent(KEY_EVENT_RECORD const& ke) {
	if (ke.bKeyDown) {
		if (player1Namefocus) {
			switch (ke.wVirtualKeyCode) {
			case VK_BACK:
				// backspace to remove at cursor location
				if (0 < player1CursorPos && player1CursorPos <= nameController->player1StringName.size()) {
					--player1CursorPos;
					nameController->player1StringName.erase(player1CursorPos, 1);
				}
				break;

			case VK_DELETE:
				if (0 <= player1CursorPos && player1CursorPos < nameController->player1StringName.size()) {
					nameController->player1StringName.erase(player1CursorPos, 1);
				}

				break;

			case VK_LEFT:
				if (player1CursorPos > 0)
					--player1CursorPos;
				break;

			case VK_RIGHT:
				if (player1CursorPos < nameController->player1StringName.size())
					++player1CursorPos;
				break;

			case VK_END:
				player1CursorPos = nameController->player1StringName.size();
				break;

			case VK_HOME:
				player1CursorPos = 0;
				break;

			case VK_RETURN:
				model->players.at(1).name = nameController->player1StringName;
				model->refeshViews(Score);
				break;

			default:
				// add character
				char ch = ke.uChar.AsciiChar;
				if (isprint(ch))
					nameController->player1StringName.insert(player1CursorPos++ + nameController->player1StringName.begin(), ch);
			}

			// show the string in the control
			auto practicalSize = nameController->player1StringName.size() + 1;
			while (player1CursorPos < player1Aperture)
				--player1Aperture;

			while (player1CursorPos - player1Aperture >= nameController->NameView.PLAYERS_NAME_LENGTH)
				++player1Aperture;

			while (practicalSize - player1Aperture<nameController->NameView.PLAYERS_NAME_LENGTH && practicalSize > nameController->NameView.PLAYERS_NAME_LENGTH)
				--player1Aperture;

			auto s = nameController->player1StringName.substr(player1Aperture, nameController->NameView.PLAYERS_NAME_LENGTH);
			s += string(nameController->NameView.PLAYERS_NAME_LENGTH - s.size(), ' ');


			model->OutputString((SHORT)2, (SHORT)1, s, model->players.at(1).colorForeground);


			// place cursor in the control
			COORD cursorLoc = { (SHORT)2, (SHORT)1 };
			cursorLoc.X += SHORT(player1CursorPos - player1Aperture);
			SetConsoleCursorPosition(model->hConsoleOutput, cursorLoc);

		}
		else if (player2Namefocus) {
			switch (ke.wVirtualKeyCode) {
			case VK_BACK:
				// backspace to remove at cursor location
				if (0 < player2CursorPos && player2CursorPos <= nameController->player2StringName.size()) {
					--player2CursorPos;
					nameController->player2StringName.erase(player2CursorPos, 1);
				}
				break;

			case VK_DELETE:
				if (0 <= player2CursorPos && player2CursorPos < nameController->player2StringName.size()) {
					nameController->player2StringName.erase(player2CursorPos, 1);
				}
				break;

			case VK_LEFT:
				if (player2CursorPos > 0)
					--player2CursorPos;
				break;

			case VK_RIGHT:
				if (player2CursorPos < nameController->player2StringName.size())
					++player2CursorPos;
				break;

			case VK_END:
				player2CursorPos = nameController->player2StringName.size();
				break;

			case VK_HOME:
				player2CursorPos = 0;
				break;

			case VK_RETURN:
				model->players.at(2).name = nameController->player2StringName;
				model->refeshViews(Score);
				break;

			default:
				// add character
				char ch = ke.uChar.AsciiChar;
				if (isprint(ch))
					nameController->player2StringName.insert(player2CursorPos++ + nameController->player2StringName.begin(), ch);
			}

			// show the string in the control
			auto practicalSize = nameController->player2StringName.size() + 1;
			while (player2CursorPos < player2Aperture)
				--player2Aperture;

			while (player2CursorPos - player2Aperture >= nameController->NameView.PLAYERS_NAME_LENGTH)
				++player2Aperture;

			while (practicalSize - player2Aperture<nameController->NameView.PLAYERS_NAME_LENGTH && practicalSize > nameController->NameView.PLAYERS_NAME_LENGTH)
				--player2Aperture;

			auto s = nameController->player2StringName.substr(player2Aperture, nameController->NameView.PLAYERS_NAME_LENGTH);
			s += string(nameController->NameView.PLAYERS_NAME_LENGTH - s.size(), ' ');


			model->OutputString((SHORT)2, (SHORT)3, s, model->players.at(2).colorForeground);


			// place cursor in the control
			COORD cursorLoc = { (SHORT)2, (SHORT)3 };
			cursorLoc.X += SHORT(player2CursorPos - player2Aperture);
			SetConsoleCursorPosition(model->hConsoleOutput, cursorLoc);
		}
		else {
			switch (ke.wVirtualKeyCode) {
			case VK_UP:
				movesController->ScrollLine(1,true);
				break;
			case VK_DOWN:
				movesController->ScrollLine(1, false);
				break;
			case VK_HOME:
				movesController->ScrollLine(MAXINT, true);
				break;
			case VK_END:
				movesController->ScrollLine(MAXINT, false);
				break;
			case VK_PRIOR:
				movesController->ScrollLine(5, true);
				break;
			case VK_NEXT:
				movesController->ScrollLine(5, false);
				break;
			case VK_RETURN:
				if (model->gameWinner != 0) {
					ResetGame();
					movesController->AddText("Game Started.", 0,0);
					model->refeshViews(Score);
				}
			}
		}


	}
	else {
	}
}

//process mouse  input
void MainController::ProcessMouseEvent(MOUSE_EVENT_RECORD const& me) {
	if (me.dwButtonState == 1) {

		auto clickPos = me.dwMousePosition;


		// test for edit control hit.
		auto player1NameHit = clickPos.Y == 1 && 2 <= clickPos.X && clickPos.X < 2 + nameController->NameView.PLAYERS_NAME_LENGTH;
		auto player2NameHit = clickPos.Y == 3 && 2 <= clickPos.X && clickPos.X < 2 + nameController->NameView.PLAYERS_NAME_LENGTH;
		auto player1ColorHit = clickPos.Y == 1 && (2 + nameController->NameView.PLAYERS_NAME_LENGTH + 2) <= clickPos.X && clickPos.X < 2 + nameController->NameView.PLAYERS_NAME_LENGTH + 4;
		auto player2ColorHit = clickPos.Y == 3 && (2 + nameController->NameView.PLAYERS_NAME_LENGTH + 2) <= clickPos.X && clickPos.X < 2 + nameController->NameView.PLAYERS_NAME_LENGTH + 4;
		auto boardHit = gameboardController->IsInsideBoard(clickPos.X, clickPos.Y);
		bool player1WasFocused = player1Namefocus;
		bool player2WasFocused = player2Namefocus;
		player1Namefocus = false;
		player2Namefocus = false;
		if (player1NameHit) {
			player1Namefocus = true;
			CONSOLE_CURSOR_INFO cci{ 10, TRUE };
			SetConsoleCursorInfo(model->hConsoleOutput, &cci);
			player1CursorPos = min(me.dwMousePosition.X - 2 + player1Aperture, nameController->player1StringName.size());
			COORD loc{ SHORT(player1CursorPos - player1Aperture + 2), 1 };
			SetConsoleCursorPosition(model->hConsoleOutput, loc);
		}
		else if (player2NameHit) {
			player2Namefocus = true;

			CONSOLE_CURSOR_INFO cci{ 10, TRUE };
			SetConsoleCursorInfo(model->hConsoleOutput, &cci);
			player2CursorPos = min(me.dwMousePosition.X - 2 + player2Aperture, nameController->player2StringName.size());
			COORD loc{ SHORT(player2CursorPos - player2Aperture + 2), 3 };
			SetConsoleCursorPosition(model->hConsoleOutput, loc);

		}
		else if (player1ColorHit) {
			CyclePlayerColor(1, 2);
			Repaint();
			ShowConsoleCursor(false);
		}
		else if (player2ColorHit) {
			CyclePlayerColor(2, 1);
			Repaint();
			ShowConsoleCursor(false);
		}
		else if (boardHit && model->gameWinner==0) {
			if (!model->gameStarted) {
				model->gameStarted = true;
				model->currentPlayerIndex = 1;
				movesController->AddText("Game Started.", 0,0);
				model->refeshViews(Score);
			}
			else {
				if (gameboardController->TakeSquare(clickPos.X, clickPos.Y)) {
					//update game model
					model->gameboard.at(gameboardController->CoordsToIndex(clickPos.X, clickPos.Y)) = model->currentPlayerIndex;
					model->players.at(model->currentPlayerIndex).moves++;
					model->processGameState();
					model->totalMoves++;
					//print taken square coords
					movesController->AddText(gameboardController->CoordsToString(clickPos.X, clickPos.Y), model->currentPlayerIndex, model->totalMoves);
					model->refeshViews(Moves);
					//print taken square
					gameboardController->PrintGameMarker(clickPos.X, clickPos.Y);
					//update score
					model->refeshViews(Score);
					//next player
					model->currentPlayerIndex++;
					if (model->currentPlayerIndex >= model->players.size()) {
						model->currentPlayerIndex = 1;
					}

					if (model->gameWinner != 0) {
						movesController->AddText(model->players.at(model->gameWinner).name + " has won the game.", model->gameWinner,0);
						movesController->AddText("Press 'Return' to play again.", 0,0);
						model->refeshViews(Score);
					}


				}
			}
			ShowConsoleCursor(false);
		}
		else {
			if (player1WasFocused && !player1Namefocus) {
				model->players.at(1).name = nameController->player1StringName;
				model->refeshViews(Score);
			}
			else if (player2WasFocused && !player2Namefocus) {
				model->players.at(2).name = nameController->player2StringName;
				model->refeshViews(Score);
			}
			ShowConsoleCursor(false);
		}


	}
	if (me.dwEventFlags == 4) {
		if (me.dwButtonState > 17864320) {
			movesController->ScrollLine(1, false);
		}
		else {
			movesController->ScrollLine(1, true);
		}
	}

}

//cycles the player color, the first argument is the payer to change and the next one is the other player
//used to make sure the colors picked are not the same
void MainController::CyclePlayerColor(int index, int other) {
	model->players.at(index).colorForeground++;
	if (model->players.at(index).colorForeground == model->players.at(other).colorForeground) {
		model->players.at(index).colorForeground++;
	}
	if (model->players.at(index).colorForeground == 7) {
		model->players.at(index).colorForeground += 2;
	}
	if (model->players.at(index).colorForeground == 16) {
		model->players.at(index).colorForeground = 1;
	}
	model->players.at(index).colorBackground = model->players.at(index).colorForeground * 16;
}

//toggles the console cursor
void MainController::ShowConsoleCursor(bool flag)
{
	HANDLE out = GetStdHandle(STD_OUTPUT_HANDLE);
	CONSOLE_CURSOR_INFO     cursorInfo;
	GetConsoleCursorInfo(out, &cursorInfo);
	cursorInfo.bVisible = flag;
	SetConsoleCursorInfo(out, &cursorInfo);
}


MainController::~MainController()
{
	delete gameboardController;
	delete nameController;
	delete scoreController;
	delete movesController;
}
