#include "TestCPPClassDLL.h"

CPPClass::CPPClass()
{
	m_float = 0;
	m_numberOfIndices = 0;
	m_indices = 0;

}

CPPClass::~CPPClass()
{
	m_float = 0;
	m_numberOfIndices = 0;

	delete[] m_indices;
	m_indices = 0;

}

//void CPPClass::FillOutArray()
//{
//	m_numberOfIndices = 10;
//
//	for (unsigned i = 0; i < m_numberOfIndices; i++)
//	{
//		m_indices[i] = i;
//	}
//}

void CPPClass::FillOutArray(int _length, int _array[])
{
	m_numberOfIndices = _length;

	for (int i = 0; i < m_numberOfIndices; i++)
	{
		_array[i] = i + 10;
	}
}

bool CPPClass::CPPClassMemberFunction(float _float)
{
	m_float = _float;

	return true;
}