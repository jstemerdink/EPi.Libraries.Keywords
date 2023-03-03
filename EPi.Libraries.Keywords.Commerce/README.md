# Add keywords to your commerce content through a provider.

By Jeroen Stemerdink

[![Build status](https://ci.appveyor.com/api/projects/status/tt6pwtrm8k3k2gf9/branch/master?svg=true)](https://ci.appveyor.com/project/jstemerdink/epi-libraries-keywords/branch/master)
[![GitHub version](https://badge.fury.io/gh/jstemerdink%2FEPi.Libraries.Keywords.svg)](http://badge.fury.io/gh/jstemerdink%2FEPi.Libraries.Keywords)
[![Platform](https://img.shields.io/badge/platform-.NET%206-blue.svg?style=flat)](https://msdn.microsoft.com/en-us/library/w0x726c2%28v=vs.110%29.aspx)
[![Platform](https://img.shields.io/badge/platform-.NET%207-blue.svg?style=flat)](https://msdn.microsoft.com/en-us/library/w0x726c2%28v=vs.110%29.aspx)
[![Platform](https://img.shields.io/badge/EPiServer%20Commerce-%2014-orange.svg?style=flat)](http://world.episerver.com/commerce/)
[![NuGet](https://img.shields.io/badge/NuGet-Release-blue.svg)](https://nuget.episerver.com/package/?id=EPi.Libraries.Keywords.Azure)
[![GitHub license](https://img.shields.io/badge/license-MIT%20license-blue.svg?style=flat)](license.txt)

## About

Analyze content on your commerce content (marked Searchable), including used blocks, and add them to your ```SeoInformation.Keywords``` property

## NOTE
The service you want to use needs to be injected.
You can use either the Azure provider I created in ```EPi.Libraries.Keywords.Azure```. 
Or write your own for the service you would like to use. In that case you will need to implement  ```IExtractionService``` and add the following attribute to your class ```[ServiceConfiguration(typeof(IExtractionService))]``` 
