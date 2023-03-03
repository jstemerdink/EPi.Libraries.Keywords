# Add keywords to your page through Azure.

By Jeroen Stemerdink

[![Build status](https://ci.appveyor.com/api/projects/status/tt6pwtrm8k3k2gf9/branch/master?svg=true)](https://ci.appveyor.com/project/jstemerdink/epi-libraries-keywords/branch/master)
[![GitHub version](https://badge.fury.io/gh/jstemerdink%2FEPi.Libraries.Keywords.svg)](http://badge.fury.io/gh/jstemerdink%2FEPi.Libraries.Keywords)
[![Platform](https://img.shields.io/badge/platform-.NET%206-blue.svg?style=flat)](https://msdn.microsoft.com/en-us/library/w0x726c2%28v=vs.110%29.aspx)
[![Platform](https://img.shields.io/badge/platform-.NET%207-blue.svg?style=flat)](https://msdn.microsoft.com/en-us/library/w0x726c2%28v=vs.110%29.aspx)
[![Platform](https://img.shields.io/badge/EPiServer-%2012-orange.svg?style=flat)](http://world.episerver.com/cms/)
[![NuGet](https://img.shields.io/badge/NuGet-Release-blue.svg)](https://nuget.episerver.com/package/?id=EPi.Libraries.Keywords.Azure)
[![GitHub license](https://img.shields.io/badge/license-MIT%20license-blue.svg?style=flat)](license.txt)

## About

Add EPi.Libraries.Keywords to your project. When you add this package, the service will be injected into the functionality of the main module.

Next add TextAnalytics settings to your appsettings.json:
```
"Azure": {
  "TextAnalytics": {
    "Key": "changeme.api-key",
    "Endpoint": "https://westeurope.api.cognitive.microsoft.com"
  }
}
```

Azure cognitive services will analyze your content marked Searchable and they keywords it returns will be added to
the content of the property marked with ```[KeywordsMetaTag]```.


Note that not all languages are supported by [Azure](https://docs.microsoft.com/en-us/azure/cognitive-services/text-analytics/language-support)
