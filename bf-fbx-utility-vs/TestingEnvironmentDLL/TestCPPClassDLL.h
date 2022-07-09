#pragma once

#ifdef TESTINGENVIRONMENTDLL_EXPORTS
#define TESTFUNCDLL_API __declspec(dllexport)
#else
#define TESTFUNCDLL_API __declspec(dllimport)
#endif

extern "C" {
	class TESTFUNCDLL_API CPPClass {
		float m_float;
		int m_numberOfIndices;
		int* m_indices;

	public:
		CPPClass();
		~CPPClass();
		//void FillOutArray();
		void FillOutArray(int _length, int _array[]);
		bool CPPClassMemberFunction(float _float);
	};
}