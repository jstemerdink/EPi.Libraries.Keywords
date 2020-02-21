# Add keywords to your page through Azure.

By Jeroen Stemerdink

[![Build status](https://ci.appveyor.com/api/projects/status/tt6pwtrm8k3k2gf9/branch/master?svg=true)](https://ci.appveyor.com/project/jstemerdink/epi-libraries-keywords/branch/master)
[![GitHub version](https://badge.fury.io/gh/jstemerdink%2FEPi.Libraries.Keywords.Alchemy.svg)](http://badge.fury.io/gh/jstemerdink%2FEPi.Libraries.Keywords.Alchemy)
[![Platform](https://img.shields.io/badge/platform-.NET%204.6.1-blue.svg?style=flat)](https://msdn.microsoft.com/en-us/library/w0x726c2%28v=vs.110%29.aspx)
[![Platform](https://img.shields.io/badge/EPiServer-%2011.11.1-orange.svg?style=flat)](http://world.episerver.com/cms/)
[![NuGet](https://img.shields.io/badge/NuGet-Release-blue.svg)](https://nuget.episerver.com/package/?id=EPi.Libraries.Keywords.Azure)
[![GitHub license](https://img.shields.io/badge/license-MIT%20license-blue.svg?style=flat)](license.txt)

## About

Add EPi.Libraries.Keywords to your project. When you add this package, the service will be injected into the functionality of the main module.

Next add an TextAnalytics API key to your appsettings ```<add key="seo.textanalytics.key" value="YourKey" />```
and update the endpoint in ```<add key="seo.textanalytics.endpoint" value="YourKey" />```
Azure cognitive services will analyze your content marked Searchable and they keywords it returns will be added to
the content of the property marked with ```[KeywordsMetaTag]```.


Note that not all languages are supported by [Azure](https://docs.microsoft.com/en-us/azure/cognitive-services/text-analytics/language-support)

> *Powered by ReSharper*
> [![image](https://i0.wp.com/jstemerdink.files.wordpress.com/2017/08/logo_resharper.png)](http://jetbrains.com?from=epi.libraries)

> *Powered by OzCode*
> [![image](https://jstemerdink.files.wordpress.com/2019/03/ozcode.jpg)](http://www.oz-code.com)
