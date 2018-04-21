#include "..\Header\ImporterHandler_Windows.h"

FBXIMPORTER_WINDOWSDLL_API FBXHandler * CPPDLLCreateFBXHandler()
{
	return new FBXHandler();
}

FBXIMPORTER_WINDOWSDLL_API void CPPDLLDestroyFBXHandler(FBXHandler * _fbxHandler)
{
	if (_fbxHandler != 0)
	{
		delete _fbxHandler;
		_fbxHandler = 0;
	}
}

FBXIMPORTER_WINDOWSDLL_API int CPPDLLLoadFBXFile(FBXHandler * _fbxHandler, const char * _filePath)
{
	return _fbxHandler->LoadFBXFile(_filePath);
}