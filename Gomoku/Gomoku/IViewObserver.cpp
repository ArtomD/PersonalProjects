#include "stdafx.h"
#include "IViewObserver.h"
#include "GameModel.h"
//parent class of all views
//takesa model pointer and stores it for future use
IViewObserver::IViewObserver(GameModel *mod) {
	model = mod;
	model->attach(this);

}
//returns the stored model pointer
GameModel *IViewObserver::getModel() {
	return model;
}