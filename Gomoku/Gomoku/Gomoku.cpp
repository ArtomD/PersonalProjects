#include "stdafx.h"
#include "MainController.h"
#include"GameModel.h"
using namespace std;
int main(){
	GameModel model;
	MainController con(&model);
	con.Start();
}