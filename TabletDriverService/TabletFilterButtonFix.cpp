#include "precompiled.h"
#include "TabletFilterButtonFix.h"


#define LOG_MODULE "ButtonFix"
#include "Logger.h"


//
// Constructor
//
TabletFilterButtonFix::TabletFilterButtonFix() {
	ignoreInvalidReports = 0;
	outputPosition = &outputState.position;
}

//
// Destructor
//
TabletFilterButtonFix::~TabletFilterButtonFix() {
}


//
// Set target
//
void TabletFilterButtonFix::SetTarget(TabletState* tabletState) {
	latestTarget.Set(tabletState->position);
	memcpy(&this->tabletState, tabletState, sizeof(TabletState));
	memcpy(&outputState, tabletState, sizeof(TabletState));
}


//
// Update filter
//
void TabletFilterButtonFix::Update() {
	//
	// Invalid position data detection.
	// Some tablets do send invalid/broken data when buttons are released
	//
	if (tabletState.buttons <= 1 && oldTabletState.buttons > 1) {
		tabletState.pressure = oldTabletState.pressure;
		tabletState.inputPressure = oldTabletState.inputPressure;
		if (oldTabletState.buttons == 5 || oldTabletState.buttons == 3)
			tabletState.buttons = 1;
		ignoreInvalidReports = 2;
		oldTarget.Set(latestTarget);
		memcpy(&this->oldTabletState, &this->tabletState, sizeof(TabletState));
		return;
	}

	if (ignoreInvalidReports == 0) {
		// Update old values
		oldTarget.Set(latestTarget);
		memcpy(&this->oldTabletState, &this->tabletState, sizeof(TabletState));
	}
	else {
		oldTabletState.time = tabletState.time;
		tabletState = oldTabletState;
		// TODO: extrapolate a new coordinate
		outputPosition->Set(oldTarget);
		ignoreInvalidReports--;
	}
}

