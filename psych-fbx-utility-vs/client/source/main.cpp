// main.cpp : Defines the entry point for the console application.

#define _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>
#include <string>
#include <iostream>

// Include from psych-fbx-utility/utility-dll project
#include "fbx_interface.h"

std::ostream& operator<<(std::ostream& out, const CRESULT value) {
	const char* s = 0;
#define PROCESS_VAL(p) case(p): s = #p; break;
	switch (value) {
		PROCESS_VAL(CRESULT_SUCCESS);
		PROCESS_VAL(CRESULT_INCORRECT_FILE_PATH);
		PROCESS_VAL(CRESULT_NO_OBJECTS_IN_SCENE);
		PROCESS_VAL(CRESULT_NODE_WAS_NOT_GEOMETRY_TYPE);
		PROCESS_VAL(CRESULT_ROOT_NODE_NOT_FOUND);
	}
#undef PROCESS_VAL

	return out << s;
}

int main()
{
	// Memory Leak Detection
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
	//_CrtSetBreakAlloc(152);  // <- Example breakpoint for memory leak location

	FBXHandler* fbxHandler = new FBXHandler();

	const CRESULT result = fbxHandler->LoadFBXFile("../resources/Lamp.fbx");

	std::cout << result << "\n";

	delete fbxHandler;

    return 0;
}
