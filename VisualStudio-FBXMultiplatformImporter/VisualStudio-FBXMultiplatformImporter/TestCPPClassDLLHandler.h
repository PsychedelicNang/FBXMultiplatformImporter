#pragma once

#include "TestCPPClassDLL.h"

//#ifdef __cplusplus
extern "C" {
//#endif

	 TESTFUNCDLL_API CPPClass* CreateCPPClass();
	 TESTFUNCDLL_API void DestroyCPPClass(CPPClass* _cppClassObject);
	 TESTFUNCDLL_API void FillOutArray(CPPClass* _cppClassObject, int _length, int _array[]);
	 //TESTFUNCDLL_API void FillOutArray(CPPClass* _cppClassObject);
	 TESTFUNCDLL_API bool CPPClassMemberFunciton(CPPClass* _cppClassObject, float _float);

//#ifdef __cplusplus
}
//#endif