#pragma once
#include "TabletFilter.h"
#include "PositionRingBuffer.h"

#include <chrono>


class TabletFilterButtonFix : public TabletFilter {
private:
	int ignoreInvalidReports;

public:
	Vector2D latestTarget;
	Vector2D oldTarget;
	Vector2D* outputPosition;

	TabletState tabletState;
	TabletState oldTabletState;

	void SetTarget(TabletState* tabletState);
	void Update();

	TabletFilterButtonFix();
	~TabletFilterButtonFix();

};

