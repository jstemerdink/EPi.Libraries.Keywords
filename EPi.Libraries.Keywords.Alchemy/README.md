# Add keywords to your page through Alchemy.

By Jeroen Stemerdink

## About

Add EPi.Libraries.SEO to your project. When you add this, the service will be injected into the functionality of the main module.

Next add an Alchemy API key to your appsettings ```<add key="seo.alchemy.key" value="YourKey" />```
Alchemy will analyze your content marked Searchable and they keywords it returns will be added to
the content of the property marked with ```[KeywordsMetaTag]```.

Add an maximum amount key to your appsettings ```<add key="seo.alchemy.maxitems" value="YourValue" />``` if you want less/more then 20 items to be returned.

Note that not all languages are supported by Alchemy (http://www.alchemyapi.com/)

## Requirements

* EPiServer >= 7.7.1
* log4net
* .Net 4.0
* EPiServer.Libraries.SEO 1.0.0.0

## Deploy

* Compile the project.
* Drop the dll in the bin.
