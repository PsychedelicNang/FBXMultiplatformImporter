#include "fbx_interface.h" // <- Must be first
#include "psych_fbx_dll_interface.h"

FBXIMPORTER_WINDOWSDLL_API FBXHandler * CreateFBXHandler()
{
	return new FBXHandler();
}

FBXIMPORTER_WINDOWSDLL_API void DestroyFBXHandler(FBXHandler * _fbxHandler)
{
	if (_fbxHandler)
	{
		delete _fbxHandler;
		_fbxHandler = nullptr;
	}
}

FBXIMPORTER_WINDOWSDLL_API int LoadFBXFile(FBXHandler * _fbxHandler, const char * _filePath)
{
	return _fbxHandler->LoadFBXFile(_filePath);
}