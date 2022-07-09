#pragma once

extern "C" {
	FBXIMPORTER_WINDOWSDLL_API FBXHandler* CPPDLLCreateFBXHandler();
	FBXIMPORTER_WINDOWSDLL_API void CPPDLLDestroyFBXHandler(FBXHandler* _fbxHandler);
	FBXIMPORTER_WINDOWSDLL_API int CPPDLLLoadFBXFile(FBXHandler* _fbxHandler, const char* _filePath);
}