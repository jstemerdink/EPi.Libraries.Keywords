# Add keywords to your page through a provider.

By Jeroen Stemerdink

[![Build status](https://ci.appveyor.com/api/projects/status/tt6pwtrm8k3k2gf9/branch/master?svg=true)](https://ci.appveyor.com/project/jstemerdink/epi-libraries-keywords/branch/master)

## About

Analyze content on your page (marked Searchable), including used blocks, and add them to your keywords property,
which you need to mark with the ```[KeywordsMetaTag]``` attribute.

## NOTE
The service you want to use needs to be injected.
You can use either the Alchemy provider I created in ```EPi.Libraries.Keywords.Alchemy```. 
Or write your own for the service you would like to use. In that case you will need to implement  ```IExtractionService``` and add the following attribute to your class ```[ServiceConfiguration(typeof(IExtractionService))]``` 


## Requirements

* EPiServer >= 8.0.0
* .Net 4.5

## Deploy

* Compile the project.
* Drop the dll in the bin.
