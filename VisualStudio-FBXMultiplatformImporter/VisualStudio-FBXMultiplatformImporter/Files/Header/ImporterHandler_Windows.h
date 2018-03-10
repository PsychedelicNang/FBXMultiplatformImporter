#pragma once
#include "FBXHandler_Windows.h"

extern "C" {
	FBXIMPORTER_WINDOWSDLL_API FBXHandler* CreateFBXHandler();
	FBXIMPORTER_WINDOWSDLL_API void DestroyFBXHandler(FBXHandler* _fbxHandler);
	FBXIMPORTER_WINDOWSDLL_API void FillOutArray(FBXHandler* _fbxHandler, int _length, int _array[]);
	FBXIMPORTER_WINDOWSDLL_API bool SetFloat(FBXHandler* _fbxHandler, float _float);
	FBXIMPORTER_WINDOWSDLL_API void FillOutMesh(FBXHandler* _fbxHandler);
}