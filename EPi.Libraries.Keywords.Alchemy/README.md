# Add keywords to your page through Alchemy.

By Jeroen Stemerdink

[![Build status](https://ci.appveyor.com/api/projects/status/tt6pwtrm8k3k2gf9/branch/master?svg=true)](https://ci.appveyor.com/project/jstemerdink/epi-libraries-keywords/branch/master)
[![GitHub version](https://badge.fury.io/gh/jstemerdink%2FEPi.Libraries.Keywords.Alchemy.svg)](http://badge.fury.io/gh/jstemerdink%2FEPi.Libraries.Keywords.Alchemy)
[![Platform](https://img.shields.io/badge/platform-.NET 4.5-blue.svg?style=flat)](https://msdn.microsoft.com/en-us/library/w0x726c2%28v=vs.110%29.aspx)
[![Platform](https://img.shields.io/badge/EPiServer-%2010.0.0-orange.svg?style=flat)](http://world.episerver.com/cms/)
[![NuGet](https://img.shields.io/badge/NuGet-Release-blue.svg)](http://nuget.episerver.com/en/OtherPages/Package/?packageId=EPi.Libraries.Keywords.Alchemy)
[![GitHub license](https://img.shields.io/badge/license-MIT%20license-blue.svg?style=flat)](license.txt)
[![Platform](https://img.shields.io/badge/platform-.NET 4.5-blue.svg?style=flat)](https://msdn.microsoft.com/en-us/library/w0x726c2%28v=vs.110%29.aspx)

## About

Add EPi.Libraries.SEO to your project. When you add this, the service will be injected into the functionality of the main module.

Next add an Alchemy API key to your appsettings ```<add key="seo.alchemy.key" value="YourKey" />```
Alchemy will analyze your content marked Searchable and they keywords it returns will be added to
the content of the property marked with ```[KeywordsMetaTag]```.

Add an maximum amount key to your appsettings ```<add key="seo.alchemy.maxitems" value="YourValue" />``` if you want less/more then 20 items to be returned.

Note that not all languages are supported by Alchemy (http://www.alchemyapi.com/)

> *Powered by ReSharper*

> [![image](http://resources.jetbrains.com/assets/media/open-graph/jetbrains_250x250.png)](http://jetbrains.com)
