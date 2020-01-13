/*
*Author: Artom Didinchuk
*Date: 2019-11-05
*Version 1.0
*This file holds the function for console interface control
*/
#pragma once
#define _WIN32_WINNT 0x0500
#include <iostream>
#include <vector>
#include <Windows.h>
#undef min
#include <algorithm>
#include <mutex>
#include <algorithm>
#include <string>


extern HANDLE hConsoleInput;
extern HANDLE hConsoleOutput;
extern std::string commandString;
extern bool hasCommand;
/*
this function processes key events, records input and moves the cursor
*/
void ProcessKeyEvent(KEY_EVENT_RECORD const& ke);
/*
this function adds a stringinto the vector to be dsiplayed on the console and refreshes the screen
*/
void AddText(std::string text);
/*
This function sets up the interface
*/
void setupScreen();
/*
This function restores the interface to what it was before the program started
*/
void restoreWindow();
/*
This function changes the size of the interface screen.
It takes a int for a width and an optional int for a height(height of 0 means no change)
*/
void ChangeDisplaySize(int width, int height);