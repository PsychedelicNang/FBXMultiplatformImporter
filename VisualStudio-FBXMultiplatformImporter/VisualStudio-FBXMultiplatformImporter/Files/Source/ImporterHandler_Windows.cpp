#include "..\Header\ImporterHandler_Windows.h"

FBXIMPORTER_WINDOWSDLL_API FBXHandler * CreateFBXHandler()
{
	return new FBXHandler();
}

FBXIMPORTER_WINDOWSDLL_API void DestroyFBXHandler(FBXHandler * _fbxHandler)
{
	if (_fbxHandler != 0)
	{
		delete _fbxHandler;
		_fbxHandler = 0;
	}
}

FBXIMPORTER_WINDOWSDLL_API void FillOutMesh(FBXHandler * _fbxHandler)
{
	_fbxHandler->FillOutMesh();
}