
#include "stdafx.h"

#include <fstream>
#include <sstream>
#include <iterator>
#include <string>
#include <vector>
#include <map>
#include <algorithm>
using namespace std;


class AutoExp
{
public:
    AutoExp(const wstring& file) : name(file), changed(false)
    {
        wifstream stream(file);
        if (stream)
        {
            int i = 0;
            wstring str;
            while (getline(stream, str))
            {
                if (str.compare(0, 12, L"[AutoExpand]") == 0)
                {
                    sections[L"AutoExpand"] = i;
                }
                else if (str.compare(0, 12, L"[Visualizer]") == 0)
                {
                    sections[L"Visualizer"] = i;
                }
                else if (str.compare(0, 15, L";QT_DEBUG_START") == 0)
                {
                    sections[L"QT_DEBUG_START"] = i;
                }
                else if (str.compare(0, 13, L";QT_DEBUG_END") == 0)
                {
                    sections[L"QT_DEBUG_END"] = i;
                }
                exps.push_back(str);
                i++; 
            }
        }
    }
    ~AutoExp()
    {
        if (changed)
        {
            wofstream stream(name);
            if (stream)
            {
                for (auto iter = exps.begin(); iter != exps.end(); ++iter)
                {
                    stream<<*iter<<endl;
                }
            }
        }
    }
    bool RemoveSection(const wstring& beg, const wstring& end)
    {
        auto begIter = sections.find(beg);
        auto endIter = sections.find(end);
        if (begIter != sections.end() && endIter != sections.end())
        {
            exps.erase(exps.begin() + begIter->second, exps.begin() + endIter->second + 1);
            changed = true;
            return true;
        }
        return false;
    }
    bool AppendSection(const wstring& aft, const vector<wstring>& exp)
    {
        if (sections.find(aft) != sections.end())
        {
            exps.insert(exps.begin() + sections.find(aft)->second + 1, exp.begin(), exp.end());
            changed = true;
            return true;
        }
        return false;
    }
private:
    wstring name;
    vector<wstring> exps;
    map<wstring, int> sections;
    bool changed;
};

vector<wstring> Tokenize(const wstring& str, const wstring& delimiters = L"\r\r\n")
{
    vector<wstring> tokens;
    // Skip delimiters at beginning.
    wstring::size_type end = str.find_first_not_of(delimiters, 0);
    // Find first "non-delimiter".
    wstring::size_type pos = str.find_first_of(delimiters, end);

    while (wstring::npos != pos || wstring::npos != end)
    {
        // Found a token, add it to the vector.
        tokens.push_back(str.substr(end, pos - end));
        // Skip delimiters.  Note the "not_of"
        end = str.find_first_not_of(delimiters, pos);
        // Find next "non-delimiter"
        pos = str.find_first_of(delimiters, end);
    }
    return tokens;
}

void AppendVisualizer(LPWSTR file, LPWSTR pwzQt4Visualizer)
{
    AutoExp autoExp(file);
    autoExp.RemoveSection(L"QT_DEBUG_START", L"QT_DEBUG_END");
    autoExp.AppendSection(L"Visualizer", Tokenize(pwzQt4Visualizer));
}

void RemoveVisualizer(LPWSTR file)
{
    AutoExp autoExp(file);
    autoExp.RemoveSection(L"QT_DEBUG_START", L"QT_DEBUG_END");
}

HRESULT ExtractBinary(__in LPCWSTR wzBinaryId, __out LPWSTR* pwzText)
{
    HRESULT hr = S_OK;
    LPWSTR pwzSql = NULL;
    PMSIHANDLE hView;
    PMSIHANDLE hRec;
    BYTE* pbData = 0;
    DWORD cbData = 0;

    ExitOnNull(wzBinaryId, hr, E_INVALIDARG, "Binary ID cannot be null");
    ExitOnNull(*wzBinaryId, hr, E_INVALIDARG, "Binary ID cannot be empty string");

    hr = StrAllocFormatted(&pwzSql, L"SELECT `Data` FROM `Binary` WHERE `Name`=\'%s\'", wzBinaryId);
    ExitOnFailure(hr, "Failed to allocate Binary table query.");

    hr = WcaOpenExecuteView(pwzSql, &hView);
    ExitOnFailure(hr, "Failed to open view on Binary table");

    hr = WcaFetchSingleRecord(hView, &hRec);
    ExitOnFailure(hr, "Failed to retrieve request from Binary table");

    hr = WcaGetRecordStream(hRec, 1, &pbData, &cbData);
    ExitOnFailure(hr, "Failed to read Binary.Data.");

    int pcwText = MultiByteToWideChar(CP_ACP, 0, (LPCSTR)pbData, cbData, 0, 0);
    StrAlloc(pwzText, pcwText + 1);
    MultiByteToWideChar(CP_ACP, 0, (LPCSTR)pbData, cbData, *pwzText, pcwText);
    (*pwzText)[pcwText] = 0;

LExit:
    WcaFreeStream(pbData);
    ReleaseStr(pwzSql);
    return hr;
}

UINT __stdcall AppendQt4Visualizer(MSIHANDLE hInstall)
{
	HRESULT hr = S_OK;
	UINT er = ERROR_SUCCESS;
    INSTALLSTATE isQt4VSAddin2010Action;
    INSTALLSTATE isQt4VSAddin2010Installed;
    INSTALLSTATE isQt4VSAddin2012Action;
    INSTALLSTATE isQt4VSAddin2012Installed;
    INSTALLSTATE isQt4VSAddin2013Action;
    INSTALLSTATE isQt4VSAddin2013Installed;

    LPWSTR pwzVs2010AutoExpDat = NULL;
    LPWSTR pwzVs2012AutoExpDat = NULL;
    LPWSTR pwzVs2013AutoExpDat = NULL;

    LPWSTR pwzQt4Visualizer2010 = NULL;
    LPWSTR pwzQt4Visualizer2012 = NULL;
    LPWSTR pwzQt4Visualizer2013 = NULL;

	hr = WcaInitialize(hInstall, "AppendQt4Visualizer");
	ExitOnFailure(hr, "Failed to initialize");

	WcaLog(LOGMSG_STANDARD, "Initialized.");

    ::MsiGetFeatureState(hInstall, L"Qt4VSAddin2010", &isQt4VSAddin2010Installed, &isQt4VSAddin2010Action);
    ::MsiGetFeatureState(hInstall, L"Qt4VSAddin2012", &isQt4VSAddin2012Installed, &isQt4VSAddin2012Action);
    ::MsiGetFeatureState(hInstall, L"Qt4VSAddin2013", &isQt4VSAddin2013Installed, &isQt4VSAddin2013Action);
    
    if (isQt4VSAddin2010Action == 3)
    {
        hr = WcaGetProperty(L"VS2010_AUTOEXPDAT", &pwzVs2010AutoExpDat);
        ExitOnFailure(hr, "failed to get install location");
        ExtractBinary(L"Qt4Visualizer2010", &pwzQt4Visualizer2010);
        ExitOnFailure(hr, "failed to get Qt4Visualizer2010");
        AppendVisualizer(pwzVs2010AutoExpDat, pwzQt4Visualizer2010);
    }

    if (isQt4VSAddin2012Action == 3)
    {
        hr = WcaGetProperty(L"VS2012_AUTOEXPDAT", &pwzVs2012AutoExpDat);
        ExitOnFailure(hr, "failed to get install location");
        ExtractBinary(L"Qt4Visualizer2012", &pwzQt4Visualizer2012);
        ExitOnFailure(hr, "failed to get Qt4Visualizer2012");
        AppendVisualizer(pwzVs2012AutoExpDat, pwzQt4Visualizer2012);
    }

    if (isQt4VSAddin2013Action == 3)
    {
        hr = WcaGetProperty(L"VS2013_AUTOEXPDAT", &pwzVs2013AutoExpDat);
        ExitOnFailure(hr, "failed to get install location");
        ExtractBinary(L"Qt4Visualizer2013", &pwzQt4Visualizer2013);
        ExitOnFailure(hr, "failed to get Qt4Visualizer2013");
        AppendVisualizer(pwzVs2013AutoExpDat, pwzQt4Visualizer2013);
    }

LExit:
    ReleaseStr(pwzQt4Visualizer2010);
    ReleaseStr(pwzQt4Visualizer2012);
    ReleaseStr(pwzQt4Visualizer2013);
    ReleaseStr(pwzVs2010AutoExpDat);
    ReleaseStr(pwzVs2012AutoExpDat);
    ReleaseStr(pwzVs2013AutoExpDat);
	er = SUCCEEDED(hr) ? ERROR_SUCCESS : ERROR_INSTALL_FAILURE;
	return WcaFinalize(er);
}

UINT __stdcall RemoveQt4Visualizer(MSIHANDLE hInstall)
{
	HRESULT hr = S_OK;
	UINT er = ERROR_SUCCESS;

    LPWSTR pwzVs2010AutoExpDat = NULL;
    LPWSTR pwzVs2012AutoExpDat = NULL;
    LPWSTR pwzVs2013AutoExpDat = NULL;
    INSTALLSTATE isQt4VSAddin2010Action;
    INSTALLSTATE isQt4VSAddin2010Installed;
    INSTALLSTATE isQt4VSAddin2012Action;
    INSTALLSTATE isQt4VSAddin2012Installed;
    INSTALLSTATE isQt4VSAddin2013Action;
    INSTALLSTATE isQt4VSAddin2013Installed;

	hr = WcaInitialize(hInstall, "RemoveQt4Visualizer");
	ExitOnFailure(hr, "Failed to initialize");

	WcaLog(LOGMSG_STANDARD, "Initialized.");

    ::MsiGetFeatureState(hInstall, L"Qt4VSAddin2010", &isQt4VSAddin2010Installed, &isQt4VSAddin2010Action);
    ::MsiGetFeatureState(hInstall, L"Qt4VSAddin2012", &isQt4VSAddin2012Installed, &isQt4VSAddin2012Action);
    ::MsiGetFeatureState(hInstall, L"Qt4VSAddin2013", &isQt4VSAddin2013Installed, &isQt4VSAddin2013Action);

    if (isQt4VSAddin2010Installed == 3)
    {
        hr = WcaGetProperty(L"VS2010_AUTOEXPDAT", &pwzVs2010AutoExpDat);
        ExitOnFailure(hr, "failed to get install location");
        RemoveVisualizer(pwzVs2010AutoExpDat);
    }
	if (isQt4VSAddin2012Installed == 3)
    {
        hr = WcaGetProperty(L"VS2012_AUTOEXPDAT", &pwzVs2012AutoExpDat);
        ExitOnFailure(hr, "failed to get install location");
        RemoveVisualizer(pwzVs2012AutoExpDat);
    }
    if (isQt4VSAddin2013Installed == 3)
    {
        hr = WcaGetProperty(L"VS2013_AUTOEXPDAT", &pwzVs2013AutoExpDat);
        ExitOnFailure(hr, "failed to get install location");
        RemoveVisualizer(pwzVs2013AutoExpDat);
    }

LExit:
    ReleaseStr(pwzVs2010AutoExpDat);
    ReleaseStr(pwzVs2012AutoExpDat);
    ReleaseStr(pwzVs2013AutoExpDat);
	er = SUCCEEDED(hr) ? ERROR_SUCCESS : ERROR_INSTALL_FAILURE;
	return WcaFinalize(er);
}

// DllMain - Initialize and cleanup WiX custom action utils.
extern "C" BOOL WINAPI DllMain(__in HINSTANCE hInst, __in ULONG ulReason, __in LPVOID)
{
	switch(ulReason)
	{
	case DLL_PROCESS_ATTACH:
		WcaGlobalInitialize(hInst);
		break;

	case DLL_PROCESS_DETACH:
		WcaGlobalFinalize();
		break;
	}

	return TRUE;
}
