/*
*Author: Artom Didinchuk
*Date: 2019-11-05
*Version 1.0
*This file holds the thread and socket functions
*/
#pragma once

#include <thread>
#include "Sockets.h"
#include "DisplayController.h"
#include "ConsoleUtils.h"

//launches the main thread
void start_ui_thread();