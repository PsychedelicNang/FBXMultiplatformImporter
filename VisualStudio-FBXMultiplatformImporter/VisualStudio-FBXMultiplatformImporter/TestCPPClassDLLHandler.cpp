#include "TestCPPClassDLLHandler.h"

TESTFUNCDLL_API CPPClass * CreateCPPClass()
{
	return new CPPClass();
}

TESTFUNCDLL_API void DestroyCPPClass(CPPClass * _cppClassObject)
{
	if (_cppClassObject != 0)
	{
		delete _cppClassObject;
		_cppClassObject = 0;
	}
}

TESTFUNCDLL_API void FillOutArray(CPPClass * _cppClassObject, int _length, int _array[])
{
	_cppClassObject->FillOutArray(_length, _array);
}
//
//TESTFUNCDLL_API void FillOutArray(CPPClass * _cppClassObject)
//{
//	_cppClassObject->FillOutArray();
//}
//
TESTFUNCDLL_API bool CPPClassMemberFunciton(CPPClass* _cppClassObject, float _float)
{
	return _cppClassObject->CPPClassMemberFunction(_float);
}
