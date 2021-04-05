#ifndef STANDALONE

#ifndef __W_PATHFINDING_H__
#define __W_PATHFINDING_H__

#include "Window.h"
#include "MO_Pathfinding.h"
#include <vector>



class W_Pathfinding : public Window
{

public:
	W_Pathfinding();
	virtual ~W_Pathfinding();

	void Draw() override;

	void DrawBakingTab();

private:
	NavAgent* selectedNav;
};

#endif //__W_PATHFINDING_H__

#endif // !STANDALONE
