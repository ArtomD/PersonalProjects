#pragma once
class GameModel;
class IViewObserver
{
public:
	GameModel * model;
	IViewObserver(GameModel *mod);
	virtual  void Redraw(int view) = 0;
protected:
	GameModel * getModel();
};

