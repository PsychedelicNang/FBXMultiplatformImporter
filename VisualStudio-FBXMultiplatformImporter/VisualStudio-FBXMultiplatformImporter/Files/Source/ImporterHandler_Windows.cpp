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

FBXIMPORTER_WINDOWSDLL_API void CPPDLLFillOutMesh(FBXHandler * _fbxHandler)
{
	_fbxHandler->FillOutMesh();
}

FBXIMPORTER_WINDOWSDLL_API int CPPDLLLoadMeshFromFBXFile(FBXHandler * _fbxHandler, const char* _filePath)
{
	return _fbxHandler->LoadMeshFromFBXFile(_filePath);
}

FBXIMPORTER_WINDOWSDLL_API int CPPDLLLoadMaterialFromFBXFile(FBXHandler * _fbxHandler, const char * _filePath)
{
	return _fbxHandler->LoadMaterialFromFBXFile(_filePath);
}
