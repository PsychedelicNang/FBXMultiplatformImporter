//
//  ImporterHandler_OSX.h
//  FBXImporterBundle_OSX
//
//  Created by Doghead 1 on 4/26/18.
//  Copyright © 2018 BrandonF. All rights reserved.
//

#ifndef ImporterHandler_OSX_h
#define ImporterHandler_OSX_h

#include "FBXHandler_OSX.h"
#include <dlfcn.h>

extern "C" {
    FBXHandler* CPPDLLCreateFBXHandler();
    void CPPDLLDestroyFBXHandler(FBXHandler* _fbxHandler);
    int CPPDLLLoadFBXFile(FBXHandler* _fbxHandler, const char* _filePath);
}

#endif /* ImporterHandler_OSX_h */
