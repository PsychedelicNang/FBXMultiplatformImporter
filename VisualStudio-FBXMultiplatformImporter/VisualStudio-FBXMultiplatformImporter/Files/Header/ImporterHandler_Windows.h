#pragma once
#include "FBXHandler_Windows.h"

extern "C" {
	FBXIMPORTER_WINDOWSDLL_API FBXHandler* CreateFBXHandler();
	FBXIMPORTER_WINDOWSDLL_API void DestroyFBXHandler(FBXHandler* _fbxHandler);
	FBXIMPORTER_WINDOWSDLL_API void FillOutMesh(FBXHandler* _fbxHandler);
}