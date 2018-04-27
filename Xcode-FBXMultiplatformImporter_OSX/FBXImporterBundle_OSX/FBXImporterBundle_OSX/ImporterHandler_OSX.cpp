//
//  ImporterHandler_OSX.cpp
//  FBXImporterBundle_OSX
//
//  Created by Doghead 1 on 4/26/18.
//  Copyright © 2018 BrandonF. All rights reserved.
//

#include "ImporterHandler_OSX.h"
#include "iostream"

FBXHandler * CPPDLLCreateFBXHandler()
{
    FBXHandler* fbxHandler = new FBXHandler();
    
    return fbxHandler;
}

void CPPDLLDestroyFBXHandler(FBXHandler * _fbxHandler)
{
    if (_fbxHandler != 0)
    {
        delete _fbxHandler;
        _fbxHandler = 0;
    }
}

int CPPDLLLoadFBXFile(FBXHandler * _fbxHandler, const char * _filePath)
{
    return _fbxHandler->LoadFBXFile(_filePath);
}
