#pragma once
#include "FBXHandler_Windows.h"

extern "C" {
	FBXIMPORTER_WINDOWSDLL_API FBXHandler* CPPDLLCreateFBXHandler();
	FBXIMPORTER_WINDOWSDLL_API void CPPDLLDestroyFBXHandler(FBXHandler* _fbxHandler);
	FBXIMPORTER_WINDOWSDLL_API void CPPDLLFillOutMesh(FBXHandler* _fbxHandler);
	FBXIMPORTER_WINDOWSDLL_API int CPPDLLLoadMeshFromFBXFile(FBXHandler* _fbxHandler, const char* _filePath);
	FBXIMPORTER_WINDOWSDLL_API int CPPDLLLoadMaterialFromFBXFile(FBXHandler* _fbxHandler, const char* _filePath);
}