#pragma once

extern "C" {
	FBXIMPORTER_WINDOWSDLL_API FBXHandler* CreateFBXHandler();
	FBXIMPORTER_WINDOWSDLL_API void DestroyFBXHandler(FBXHandler* _fbxHandler);
	FBXIMPORTER_WINDOWSDLL_API int LoadFBXFile(FBXHandler* _fbxHandler, const char* _filePath);
}
